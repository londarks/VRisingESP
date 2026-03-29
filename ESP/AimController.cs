using ExtrasensoryPerception.Utils;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

public class AimController : MonoBehaviour
{
	private void OnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isFocused && Config.Aimbot.Enabled && Aimbot.Active && Aimbot.HasValidTarget())
		{
			Vector2 aimData = Aimbot.GetAimData();
			if (aimData != Vector2.zero)
			{
				AimAt(aimData);
			}
		}
	}

	private static void AimAt(Vector2 aimData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		MouseSimulator.SetPos(aimData);
	}
}
