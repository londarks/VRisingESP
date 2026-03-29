using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ProjectM;
using Unity.Entities;
using Unity.Mathematics;
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

	private static Vector2 _cachedAimData = Vector2.zero;

	private static float _lastTargetSwitchTime;

	private static bool _hasValidTarget;

	internal static readonly List<TargetCandidate> Candidates = new List<TargetCandidate>();

	internal static void TryAddCandidate(Entity entity, Vector2 screenPoint, float distance, EntityType type)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f - distance / Config.Aimbot.MaxDistance.Value;
		float num2 = 1f - screenDistance / Config.Aimbot.MaxCursorDistance.Value;
		Health val = entity.Read<Health>();
		float num3 = val.Value / val.MaxHealth.Value;
		float num4 = ((num3 < 0.3f) ? 1f : (1f - num3));
		return num * Config.Aimbot.DistanceWeight.Value + num2 * Config.Aimbot.CursorDistanceWeight.Value + num4 * Config.Aimbot.HealthWeight.Value + type switch
		{
			EntityType.Player => 1f, 
			EntityType.Boss => 0.5f, 
			_ => 0.25f, 
		} * Config.Aimbot.EntityTypeWeight.Value;
	}

	internal static void UpdateAimData()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		if (Candidates.Count == 0)
		{
			_currentTarget = Entity.Null;
			_hasValidTarget = false;
			_cachedAimData = Vector2.zero;
			return;
		}
		TargetCandidate currentTarget = Enumerable.FirstOrDefault<TargetCandidate>(Enumerable.Where<TargetCandidate>((System.Collections.Generic.IEnumerable<TargetCandidate>)Candidates, (Func<TargetCandidate, bool>)((TargetCandidate candidate) => candidate.Entity == _currentTarget))) ?? new TargetCandidate();
		float time = Time.time;
		if (!IsCurrentTargetValid() || time - _lastTargetSwitchTime > Config.Aimbot.SwitchCooldown.Value)
		{
			System.Collections.Generic.IEnumerator<TargetCandidate> enumerator = Enumerable.Where<TargetCandidate>((System.Collections.Generic.IEnumerable<TargetCandidate>)Candidates, (Func<TargetCandidate, bool>)((TargetCandidate candidate) => candidate.Score > currentTarget.Score)).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					TargetCandidate current = enumerator.Current;
					currentTarget = current;
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
			if (_currentTarget != currentTarget.Entity)
			{
				_currentTarget = currentTarget.Entity;
				_lastTargetSwitchTime = time;
			}
		}
		_hasValidTarget = _currentTarget.Exists();
		_cachedAimData = GetPredictedScreenPosition(_currentTarget);
	}

	private static bool IsCurrentTargetValid()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (_currentTarget.Exists() && !_currentTarget.IsDisabled())
		{
			return _currentTarget.IsAlive();
		}
		return false;
	}

	private static Vector2 GetPredictedScreenPosition(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return _cachedAimData;
	}

	public static bool HasValidTarget()
	{
		return _hasValidTarget;
	}
}
