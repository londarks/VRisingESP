using ExtrasensoryPerception.Utils;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

public class AimController : MonoBehaviour
{
	private const float AimSmoothing = 0.45f;
	private const float MaxDeltaPerFrame = 80f;

	private static readonly Color LockColor = new Color(0.95f, 0.25f, 0.25f, 0.85f);

	private void OnGUI()
	{
		if ((int)Event.current.type != 7 || !Config.Aimbot.Enabled) return;

		if (Aimbot.HasLockedTarget)
		{
			Vector2 aim = Aimbot.GetAimData();
			if (aim != Vector2.zero)
				DrawLockReticle(aim);
		}

		if (Aimbot.HasValidTarget() && Config.Aimbot.DrawAimPosition.Enabled)
		{
			Vector2 aim = Aimbot.GetAimData();
			if (aim != Vector2.zero)
				Primitives.DrawX(aim, 4f, Aimbot.HasLockedTarget ? LockColor : Color.white);
		}
	}

	private void Update()
	{
		if (!Config.Aimbot.Enabled) return;

		Aimbot.CursorPosition = MouseSimulator.CursorPosition;
		Aimbot.UpdateAimData();

		// Shift + Left Click to lock/unlock target
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
			Aimbot.TryLockTarget();

		if (!Config.SmartAssist.Enabled)
		{
			if (Config.Aimbot.Mode.Value == 1)
			{
				if (Input.GetKeyDown(Config.Aimbot.Key.Value))
					Aimbot.Active = !Aimbot.Active;
			}
			else
			{
				Aimbot.Active = Input.GetKey(Config.Aimbot.Key.Value);
			}
		}
	}

	private void LateUpdate()
	{
		if (!Application.isFocused || !Config.Aimbot.Enabled || !Aimbot.Active || !Aimbot.HasValidTarget())
			return;
		if (Plugin.IsMenuOpen) return;

		Vector2 delta = Aimbot.GetAimDelta();
		if (delta == Vector2.zero) return;

		Vector2 smoothed = delta * AimSmoothing;
		if (smoothed.magnitude > MaxDeltaPerFrame)
			smoothed = smoothed.normalized * MaxDeltaPerFrame;

		int dx = Mathf.RoundToInt(smoothed.x);
		int dy = Mathf.RoundToInt(smoothed.y);
		if (dx != 0 || dy != 0)
			MouseSimulator.MoveDelta(dx, dy);
	}

	/// <summary>
	/// Minimal lock reticle: small crosshair ticks around the aim point.
	/// </summary>
	private static void DrawLockReticle(Vector2 p)
	{
		float g = 4f;   // gap from center
		float l = 6f;   // tick length

		// Horizontal ticks
		Primitives.DrawLine(new Vector2(p.x - g - l, p.y), new Vector2(p.x - g, p.y), LockColor);
		Primitives.DrawLine(new Vector2(p.x + g, p.y), new Vector2(p.x + g + l, p.y), LockColor);

		// Vertical ticks
		Primitives.DrawLine(new Vector2(p.x, p.y - g - l), new Vector2(p.x, p.y - g), LockColor);
		Primitives.DrawLine(new Vector2(p.x, p.y + g), new Vector2(p.x, p.y + g + l), LockColor);
	}
}
