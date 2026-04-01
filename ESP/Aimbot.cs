using System.Collections.Generic;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ProjectM;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

public static class Aimbot
{
	internal enum EntityType
	{
		Player,
		Boss,
		Mob
	}

	internal class TargetCandidate
	{
		public Entity Entity;

		public float Score;
	}

	internal static Vector2 CursorPosition;

	internal static float ProjectileSpeed;

	internal static bool Active;

	private static Entity _currentTarget = Entity.Null;
	internal static Entity CurrentTarget => _currentTarget;

	private static Vector2 _cachedAimData = Vector2.zero;

	private static float _lastTargetSwitchTime;

	private static bool _hasValidTarget;

	// Smoothing: last valid aim position for interpolation
	private static Vector2 _smoothedAimPosition = Vector2.zero;

	// How many consecutive frames the target was lost (for grace period)
	private static int _targetLostFrames;

	// Grace period: keep tracking last known position for N frames before giving up
	private const int TargetLostGraceFrames = 15;

	// Dead zone: don't move mouse if already within this pixel radius of the target
	private const float DeadZoneRadius = 3f;

	// ── Lock Target System ─────────────────────────────────────────
	private static Entity _lockedTarget = Entity.Null;
	internal static bool HasLockedTarget => _lockedTarget != Entity.Null && _lockedTarget.Exists();
	private static float _lockLostTime;
	private const float LockGraceSeconds = 1.5f;

	internal static readonly List<TargetCandidate> Candidates = new List<TargetCandidate>();

	/// <summary>
	/// Shift+Click: lock the best candidate near cursor, or unlock if already locked.
	/// </summary>
	internal static void TryLockTarget()
	{
		if (HasLockedTarget)
		{
			UnlockTarget();
			return;
		}

		if (Candidates.Count == 0) return;

		TargetCandidate best = null;
		float bestScore = float.MinValue;
		foreach (var c in Candidates)
		{
			if (c.Score > bestScore)
			{
				bestScore = c.Score;
				best = c;
			}
		}

		if (best != null)
		{
			_lockedTarget = best.Entity;
			_currentTarget = best.Entity;
			_lastTargetSwitchTime = Time.time;
			_lockLostTime = 0f;
			_targetLostFrames = 0;
		}
	}

	internal static void UnlockTarget()
	{
		_lockedTarget = Entity.Null;
		_lockLostTime = 0f;
	}

	private static bool IsLockedTargetStillValid()
	{
		if (!_lockedTarget.Exists() || _lockedTarget.IsDisabled() || !_lockedTarget.IsAlive())
			return false;

		Vector3 pos = _lockedTarget.GetPosition();
		Vector3 playerPos = EntityList.LocalPlayer.GetPosition();
		float dist = Vector3.Distance(playerPos, pos);

		bool onScreen = Logic.GetScreenPoint(pos, out _);
		bool inRange = dist <= Config.Aimbot.MaxDistance.Value * 1.5f;

		if (!onScreen || !inRange)
		{
			if (_lockLostTime == 0f) _lockLostTime = Time.time;
			return Time.time - _lockLostTime < LockGraceSeconds;
		}

		_lockLostTime = 0f;
		return true;
	}

	internal static void TryAddCandidate(Entity entity, Vector2 screenPoint, float distance, EntityType type)
	{
		if (!(distance > Config.Aimbot.MaxDistance.Value))
		{
			float num = Vector2.Distance(CursorPosition, screenPoint);
			if (!(num > Config.Aimbot.MaxCursorDistance.Value))
			{
				TargetCandidate targetCandidate = new TargetCandidate
				{
					Entity = entity,
					Score = CalculateTargetScore(entity, distance, num, type)
				};
				Candidates.Add(targetCandidate);
			}
		}
	}

	private static float CalculateTargetScore(Entity entity, float distance, float screenDistance, EntityType type)
	{
		float num = 1f - distance / Config.Aimbot.MaxDistance.Value;
		float num2 = 1f - screenDistance / Config.Aimbot.MaxCursorDistance.Value;
		Health val = entity.Read<Health>();
		float num3 = val.Value / val.MaxHealth.Value;
		float num4 = ((num3 < 0.3f) ? 1f : (1f - num3));
		float threatBonus = IsAttackingMe(entity) ? 2.0f : 0f;
		return num * Config.Aimbot.DistanceWeight.Value
			+ num2 * Config.Aimbot.CursorDistanceWeight.Value
			+ num4 * Config.Aimbot.HealthWeight.Value
			+ type switch
			{
				EntityType.Player => 1f,
				EntityType.Boss => 0.5f,
				_ => 0.25f,
			} * Config.Aimbot.EntityTypeWeight.Value
			+ threatBonus;
	}

	/// <summary>
	/// Check if an entity is currently attacking us (casting + aimed at local player).
	/// </summary>
	private static bool IsAttackingMe(Entity entity)
	{
		try
		{
			if (!entity.TryGetComponent<AbilityBar_Shared>(out var abilityBar)) return false;
			if (!abilityBar.SyncedIsCasting) return false;
			if (!entity.TryGetComponent<TargetDirection>(out var targetDir)) return false;

			var localChar = EntityList.LocalCharacter;
			if (localChar == Entity.Null || !localChar.Exists()) return false;

			Vector3 enemyPos = entity.GetPosition();
			Vector3 playerPos = localChar.GetPosition();
			Vector3 aimDir = new Vector3(targetDir.AimDirection.x, 0, targetDir.AimDirection.z).normalized;
			Vector3 toPlayer = (playerPos - enemyPos).normalized;

			// dot > 0.6 = aiming roughly at us
			return Vector3.Dot(aimDir, toPlayer) > 0.6f;
		}
		catch { return false; }
	}

	internal static void UpdateAimData()
	{
		// ── Locked target takes priority ──
		if (HasLockedTarget)
		{
			if (!IsLockedTargetStillValid())
			{
				UnlockTarget();
				// Fall through to normal logic
			}
			else
			{
				_currentTarget = _lockedTarget;
				_hasValidTarget = true;
				_targetLostFrames = 0;

				Vector2 lockAim = GetPredictedScreenPosition(_currentTarget);
				if (lockAim != Vector2.zero)
					ApplySmoothing(lockAim);
				return;
			}
		}

		// ── Lock-only mode: no auto-select without lock ──
		if (Config.Aimbot.LockOnly.Enabled)
		{
			_currentTarget = Entity.Null;
			_hasValidTarget = false;
			_cachedAimData = Vector2.zero;
			_smoothedAimPosition = Vector2.zero;
			return;
		}

		// ── Normal auto-select logic ──
		if (Candidates.Count == 0)
		{
			_targetLostFrames++;
			if (_targetLostFrames > TargetLostGraceFrames || !IsCurrentTargetValid())
			{
				_currentTarget = Entity.Null;
				_hasValidTarget = false;
				_cachedAimData = Vector2.zero;
				_smoothedAimPosition = Vector2.zero;
			}
			return;
		}

		_targetLostFrames = 0;

		// Find current target in candidates + best scoring candidate (no LINQ, no allocations)
		TargetCandidate currentCandidate = null;
		TargetCandidate bestCandidate = null;
		float bestScore = float.MinValue;
		for (int i = 0; i < Candidates.Count; i++)
		{
			var c = Candidates[i];
			if (c.Entity == _currentTarget) currentCandidate = c;
			if (c.Score > bestScore) { bestScore = c.Score; bestCandidate = c; }
		}
		currentCandidate ??= new TargetCandidate();

		float time = Time.time;
		if (!IsCurrentTargetValid() || time - _lastTargetSwitchTime > Config.Aimbot.SwitchCooldown.Value)
		{
			if (bestCandidate != null && bestCandidate.Score > currentCandidate.Score)
			{
				_currentTarget = bestCandidate.Entity;
				_lastTargetSwitchTime = time;
			}
		}

		_hasValidTarget = _currentTarget.Exists();
		Vector2 rawAim = GetPredictedScreenPosition(_currentTarget);
		if (rawAim != Vector2.zero)
			ApplySmoothing(rawAim);
	}

	private static void ApplySmoothing(Vector2 rawAim)
	{
		if (_smoothedAimPosition == Vector2.zero)
		{
			_smoothedAimPosition = rawAim;
		}
		else
		{
			float dist = Vector2.Distance(_smoothedAimPosition, rawAim);
			float smoothFactor = Mathf.Clamp(dist / 200f, 0.15f, 1f);
			_smoothedAimPosition = Vector2.Lerp(_smoothedAimPosition, rawAim, smoothFactor);
		}
		_cachedAimData = _smoothedAimPosition;
	}

	private static bool IsCurrentTargetValid()
	{
		if (_currentTarget.Exists() && !_currentTarget.IsDisabled())
		{
			return _currentTarget.IsAlive();
		}
		return false;
	}

	private static Vector2 GetPredictedScreenPosition(Entity entity)
	{
		Vector3 position = EntityList.LocalPlayer.GetPosition();
		Vector3 val = entity.GetPosition();
		Vector3 val2 = (Vector3)entity.Read<Velocity>().Value;
		Vector3 val3 = val - position;
		float projectileSpeed = ProjectileSpeed;
		float num = val2.sqrMagnitude - projectileSpeed * projectileSpeed;
		float num2 = 2f * Vector3.Dot(val3, val2);
		float sqrMagnitude = val3.sqrMagnitude;
		float num3 = num2 * num2 - 4f * num * sqrMagnitude;
		if (num3 >= 0f && Mathf.Abs(num) >= 0.001f)
		{
			float num4 = (0f - num2 + Mathf.Sqrt(num3)) / (2f * num);
			float num5 = (0f - num2 - Mathf.Sqrt(num3)) / (2f * num);
			float num6 = ((num4 > 0f && num5 > 0f) ? Mathf.Min(num4, num5) : Mathf.Max(num4, num5));
			if (num6 > 0f)
			{
				val += val2 * num6;
			}
		}
		if (!Logic.GetScreenPoint(val, out var screenPoint))
		{
			return Vector2.zero;
		}
		return screenPoint;
	}

	public static Vector2 GetAimData()
	{
		return _cachedAimData;
	}

	public static bool HasValidTarget()
	{
		return _hasValidTarget;
	}

	/// <summary>
	/// Returns the pixel delta from current cursor to the aim target.
	/// Dead zone applied: returns zero if already close enough.
	/// </summary>
	public static Vector2 GetAimDelta()
	{
		if (_cachedAimData == Vector2.zero) return Vector2.zero;

		Vector2 delta = _cachedAimData - CursorPosition;

		// Dead zone: don't micro-correct when already on target
		if (delta.magnitude < DeadZoneRadius) return Vector2.zero;

		return delta;
	}
}
