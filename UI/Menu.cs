using System;
using BepInEx.Configuration;
using ExtrasensoryPerception.ESP;
using ExtrasensoryPerception.Utils;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace ExtrasensoryPerception.UI;

internal class Menu : MonoBehaviour
{
	private static Rect _windowRect = new Rect(20f, 20f, 300f, 750f);

	private Rect _aimbotSettingsRect = new Rect(_windowRect.x + _windowRect.width + 5f, _windowRect.y, 300f, 500f);

	private bool _showMenu;

	private bool _showAimbotSettings;

	private readonly string[] _boxTypes = new string[2] { "Full Boxes", "Corners Only" };

	private readonly string[] _aimbotTypes = new string[2] { "Hold", "Toggle" };

	private void OnGUI()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (_showMenu)
		{
			MenuTheme.SetupDarkTheme();
			_windowRect = GUI.Window(1, _windowRect, (GUI.WindowFunction)DrawWindow, "vMenu", MenuTheme.WindowStyle);
			if (_showAimbotSettings)
			{
				_aimbotSettingsRect = GUI.Window(2, _aimbotSettingsRect, (GUI.WindowFunction)AimSettingsWindow, "Aimbot Settings", MenuTheme.WindowStyle);
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown((KeyCode)277))
		{
			_showMenu = !_showMenu;
		}
	}

	private void DrawWindow(int windowID)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
		CustomHeader("General");
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		CustomToggle(Config.ModToggle, "Enabled");
		if (CustomButton("Reset Camera", GUILayout.Width(135f)))
		{
			Logic.MainCamera = Camera.main ?? throw new InvalidOperationException();
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		CustomToggle(Config.ESP.Boxes, "Draw Boxes");
		CustomButton(_boxTypes[Config.ESP.Boxes.Option], Config.ESP.Boxes, _boxTypes.Length);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		CustomToggle(Config.ESP.Outlines, new GUIContent("Draw Outlines", (Texture)null, "Warning: May cause frame drops."));
		CustomButton("Quality: {}", Config.ESP.Outlines, 4);
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		CustomHeader("Aimbot");
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		CustomToggle(Config.Aimbot.Status, "Enabled");
		CustomButton(_aimbotTypes[Config.Aimbot.Mode.Value], Config.Aimbot.Mode, _aimbotTypes.Length);
		GUILayout.EndHorizontal();
		if (CustomButton("Settings", GUILayout.ExpandWidth(true)))
		{
			_showAimbotSettings = !_showAimbotSettings;
		}
		GUILayout.Space(15f);
		CustomHeader("ESP");
		DrawSection("Players", Config.ESP.Players);
		DrawSection("VBlood Carriers", Config.ESP.VBloodCarriers);
		DrawSection("Blood Sources", Config.ESP.BloodSources);
		DrawSection("Gate Bosses", Config.ESP.GateBosses);
		DrawSection("Items", Config.ESP.Items);
		DrawSection("Containers", Config.ESP.Containers);
		DrawSection("Ores", Config.ESP.Ores);
		DrawSection("Plants", Config.ESP.Plants);
		DrawSection("Fishing Spots", Config.ESP.FishingSpots);
		DrawSection("Horses", Config.ESP.Horses);
		DrawSection("Servants", Config.ESP.Servants);
		DrawSection("Carriages", Config.ESP.Carriages);
		GUILayout.Space(15f);
		CustomHeader("Extra");
		CustomToggle(Config.Extras.AutoFishing, "Auto-Fishing");
		CustomToggle(Config.Extras.AutoLoot, "Auto-Loot (WIP)");
		CustomToggle(Config.Extras.NoFog, "No Fog");
		GUILayout.EndVertical();
		ShowTooltip();
		GUI.DragWindow(new Rect(0f, 0f, _windowRect.width, 20f));
	}

	private void AimSettingsWindow(int windowID)
	{
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
		CustomHeader("General");
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		CustomToggle(Config.Aimbot.Players, "Players");
		CustomToggle(Config.Aimbot.Bosses, "Bosses");
		GUILayout.EndHorizontal();
		CustomToggle(Config.Aimbot.Mobs, "Mobs");
		GUILayout.Space(20f);
		CustomToggle(Config.Aimbot.DrawAimPosition, "Draw Aim Position");
		GUILayout.Space(15f);
		CustomHeader("Limits");
		CustomSlider("Distance: {}m", Config.Aimbot.MaxDistance, 1f, 50f);
		CustomSlider("Cursor Distance: {}", Config.Aimbot.MaxCursorDistance, 0f, 1000f);
		CustomSlider("Target Switch CD: {}s", Config.Aimbot.SwitchCooldown, 0f, 1f, "F1");
		GUILayout.Space(15f);
		CustomHeader("Weights");
		CustomSlider("Distance: {}", Config.Aimbot.DistanceWeight, 0f, 1f, "F2");
		CustomSlider("Cursor Distance: {}", Config.Aimbot.CursorDistanceWeight, 0f, 1f, "F2");
		CustomSlider("Health: {}", Config.Aimbot.HealthWeight, 0f, 1f, "F2");
		CustomSlider("Type: {}", Config.Aimbot.EntityTypeWeight, 0f, 1f, "F2");
		GUILayout.Space(10f);
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(true) }))
		{
			_showAimbotSettings = false;
		}
		GUILayout.EndVertical();
		GUI.DragWindow(new Rect(0f, 0f, _aimbotSettingsRect.width, 20f));
	}

	private static bool CustomButton(string label, params GUILayoutOption[] options)
	{
		return GUILayout.Button(label, MenuTheme.ButtonStyle, options);
	}

	private static void CustomButton(string label, ConfigEntry<int> config, int maxValue, int addValue = 0)
	{
		if (GUILayout.Button(label.Replace("{}", config.Value.ToString()), MenuTheme.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(135f) }))
		{
			config.Value = (config.Value - addValue + 1) % maxValue + addValue;
		}
	}

	private static void CustomButton(string label, Config.FeatureConfig config, int maxValue, int addValue = 0)
	{
		if (GUILayout.Button(label.Replace("{}", config.Option.ToString()), MenuTheme.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(135f) }))
		{
			config.Option = (config.Option - addValue + 1) % maxValue + addValue;
		}
	}

	private static void CustomHeader(string label)
	{
		GUILayout.Label(label, MenuTheme.BoxStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
	}

	private static float CustomSlider(string label, float value, float min, float max, string format = "F0")
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		GUIStyle label2 = GUI.skin.label;
		label2.contentOffset = new Vector2(0f, -4f);
		GUILayout.Label(label.Replace("{}", value.ToString(format)), label2, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(135f) });
		GUILayout.Space(6f);
		GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
		GUILayout.Space(4f);
		value = GUILayout.HorizontalSlider(value, min, max, MenuTheme.HSliderStyle, MenuTheme.HSliderThumbStyle, System.Array.Empty<GUILayoutOption>());
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		return value;
	}

	private static void CustomSlider(string label, Config.FeatureConfig config, float min, float max, string format = "F0")
	{
		config.MinimumQuality = CustomSlider(label, config.MinimumQuality, min, max, format);
	}

	private static void CustomSlider(string label, ConfigEntry<float> config, float min, float max, string format = "F0")
	{
		config.Value = CustomSlider(label, config.Value, min, max, format);
	}

	private static void CustomToggle(Config.FeatureConfig config, string text)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		CustomToggle(config, new GUIContent(text));
	}

	private static void CustomToggle(Config.FeatureConfig config, GUIContent content)
	{
		config.Enabled = GUILayout.Toggle(config.Enabled, content, MenuTheme.ToggleStyle, System.Array.Empty<GUILayoutOption>());
	}

	private static void CustomToggle(ConfigEntry<bool> entry, string text)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		CustomToggle(entry, new GUIContent(text));
	}

	private static void CustomToggle(ConfigEntry<bool> entry, GUIContent content)
	{
		entry.Value = GUILayout.Toggle(entry.Value, content, MenuTheme.ToggleStyle, System.Array.Empty<GUILayoutOption>());
	}

	private static void DrawSection(string sectionName, Config.FeatureConfig config)
	{
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		CustomToggle(config, sectionName);
		if (CustomButton(ColorOptions.GetColorName(config.Color)))
		{
			config.Color = (config.Color + 1) % ColorOptions.AllColors.Count;
		}
		GUILayout.EndHorizontal();
		if (config.MinimumQuality != 0f)
		{
			CustomSlider("Min. Quality: {}%", config, 1f, 100f);
		}
	}

	private void ShowTooltip()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(GUI.tooltip))
		{
			Vector2 mousePosition = Event.current.mousePosition;
			GUIContent val = new GUIContent(GUI.tooltip);
			Vector2 val2 = MenuTheme.TooltipStyle.CalcSize(val);
			GUI.Label(new Rect(mousePosition.x + 35f, mousePosition.y - 1f, val2.x, val2.y), val, MenuTheme.TooltipStyle);
		}
	}
}
