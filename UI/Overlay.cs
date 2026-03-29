using System;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using ExtrasensoryPerception.ESP;
using ExtrasensoryPerception.Utils;
using UnityEngine;

namespace ExtrasensoryPerception.UI;

public class Overlay : MonoBehaviour
{
	private void OnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 7 && Config.ModToggle.Value)
		{
if (Plugin.IsInGame && !Plugin.IsMenuOpen)
			{
				RenderQueue.DrawQueued();
			}
			RenderQueue.Clear();
		}
	}

	private void Update()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (!Config.ModToggle.Value || !Plugin.IsInGame)
		{
			return;
		}
		try
		{
			Logic.ProcessAllEntities();
		}
		catch (System.Exception ex)
		{
			ManualLogSource logger = Plugin.Logger;
			bool flag = default(bool);
			BepInExErrorLogInterpolatedStringHandler val = new BepInExErrorLogInterpolatedStringHandler(34, 1, out flag);
			if (flag)
			{
				((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Overlay.LateUpdate() failed with: ");
				((BepInExLogInterpolatedStringHandler)val).AppendFormatted<System.Exception>(ex);
			}
			logger.LogError(val);
			throw;
		}
	}
}
