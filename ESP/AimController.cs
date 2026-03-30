using ExtrasensoryPerception.Utils;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

public class AimController : MonoBehaviour
{
	// Smoothing factor for mouse movement (0-1). Lower = smoother but slower.
	private const float AimSmoothing = 0.45f;

	// Max pixels to move per frame to avoid huge jumps
	private const float MaxDeltaPerFrame = 80f;

	private void OnGUI()
	{
		if ((int)Event.current.type == 7 && Config.Aimbot.Enabled && Aimbot.HasValidTarget())
		{
			Vector2 aimData = Aimbot.GetAimData();
			if (aimData != Vector2.zero && Config.Aimbot.DrawAimPosition.Enabled)
			{
				Primitives.DrawX(aimData, 5f, Color.white);
			}
		}
	}

	private void Update()
	{
		if (!Config.Aimbot.Enabled)
		{
			return;
		}
		Aimbot.CursorPosition = MouseSimulator.CursorPosition;
		Aimbot.UpdateAimData();
		if (Config.Aimbot.Mode.Value == 1)
		{
			if (Input.GetKeyDown(Config.Aimbot.Key.Value))
			{
				Aimbot.Active = !Aimbot.Active;
			}
		}
		else
		{
			Aimbot.Active = Input.GetKey(Config.Aimbot.Key.Value);
		}
	}

	private void LateUpdate()
	{
		if (!Application.isFocused || !Config.Aimbot.Enabled || !Aimbot.Active || !Aimbot.HasValidTarget())
			return;

		// Skip if in-game menu is open
		if (Plugin.IsMenuOpen) return;

		Vector2 delta = Aimbot.GetAimDelta();
		if (delta == Vector2.zero) return;

		// Apply smoothing: only move a fraction of the delta per frame
		Vector2 smoothed = delta * AimSmoothing;

		// Clamp to max speed to prevent camera jerking on large jumps
		if (smoothed.magnitude > MaxDeltaPerFrame)
		{
			smoothed = smoothed.normalized * MaxDeltaPerFrame;
		}

		// Use relative mouse movement — this works WITH V Rising's camera system
		// instead of fighting it with SetCursorPos
		int dx = Mathf.RoundToInt(smoothed.x);
		int dy = Mathf.RoundToInt(smoothed.y);

		if (dx != 0 || dy != 0)
		{
			MouseSimulator.MoveDelta(dx, dy);
		}
	}
}
