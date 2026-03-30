using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using UnityEngine;

namespace ExtrasensoryPerception.Utils;

internal static class Config
{
	internal class FeatureConfig
	{
		[field: CompilerGenerated]
		private ConfigEntry<bool> StatusEntry
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		private ConfigEntry<int>? ColorEntry
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		private ConfigEntry<int>? OptionEntry
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		private ConfigEntry<float>? QualityEntry
		{
			[CompilerGenerated]
			get;
		}

		internal bool Enabled
		{
			get
			{
				return StatusEntry.Value;
			}
			set
			{
				StatusEntry.Value = value;
			}
		}

		internal int Color
		{
			get
			{
				return ColorEntry.Value;
			}
			set
			{
				ColorEntry.Value = value;
			}
		}

		internal int Option
		{
			get
			{
				return OptionEntry.Value;
			}
			set
			{
				OptionEntry.Value = value;
			}
		}

		internal float MinimumQuality
		{
			get
			{
				return QualityEntry?.Value ?? 0f;
			}
			set
			{
				QualityEntry.Value = value;
			}
		}

		internal FeatureConfig(string section, string key, int defaultColor, float minQuality)
		{
			StatusEntry = ConfigFile.Bind<bool>(section, key, false, "Enable/disable " + key + ".");
			ColorEntry = ConfigFile.Bind<int>(section, key + "Color", defaultColor, key + " color index.");
			if (minQuality != 0f)
			{
				QualityEntry = ConfigFile.Bind<float>(section, key + "Quality", minQuality, key + " minimum quality.");
			}
		}

		internal FeatureConfig(string section, string key, int option)
		{
			StatusEntry = ConfigFile.Bind<bool>(section, key, false, "Enable/disable " + key);
			OptionEntry = ConfigFile.Bind<int>(section, key + "Option", option, key + " selected option.");
		}

		internal FeatureConfig(string section, string key)
		{
			StatusEntry = ConfigFile.Bind<bool>(section, key, false, "Enable/disable " + key + ".");
		}
	}

	internal static class Aimbot
	{
		internal static readonly FeatureConfig Status = new FeatureConfig("Aimbot", "Enabled");

		internal static readonly FeatureConfig Players = new FeatureConfig("Aimbot", "Players");

		internal static readonly FeatureConfig Bosses = new FeatureConfig("Aimbot", "Bosses");

		internal static readonly FeatureConfig Mobs = new FeatureConfig("Aimbot", "Mobs");

		internal static readonly FeatureConfig DrawAimPosition = new FeatureConfig("Aimbot", "DrawAimPosition");

		internal static readonly ConfigEntry<int> Mode = ConfigFile.Bind<int>("Aimbot", "Mode", 0, "Aimbot mode (Hold/Toggle).");

		internal static readonly ConfigEntry<KeyCode> Key = ConfigFile.Bind<KeyCode>("Aimbot", "Key", (KeyCode)327, "Aimbot triggering key.");

		internal static readonly ConfigEntry<float> MaxDistance = ConfigFile.Bind<float>("Aimbot", "MaxDistance", 15f, "Max target distance.");

		internal static readonly ConfigEntry<float> MaxCursorDistance = ConfigFile.Bind<float>("Aimbot", "MaxCursorDistance", 500f, "Max target distance from cursor.");

		internal static readonly ConfigEntry<float> SwitchCooldown = ConfigFile.Bind<float>("Aimbot", "SwitchCooldown", 0.2f, "Minimum time between target switch.");

		internal static readonly ConfigEntry<float> DistanceWeight = ConfigFile.Bind<float>("Aimbot", "Distance", 0.5f, "How much its distance (from player) affects target selection.");

		internal static readonly ConfigEntry<float> CursorDistanceWeight = ConfigFile.Bind<float>("Aimbot", "CursorDistance", 0.75f, "How much its distance (from cursor) affects target selection.");

		internal static readonly ConfigEntry<float> HealthWeight = ConfigFile.Bind<float>("Aimbot", "Health", 0.25f, "How much its health affects target selection.");

		internal static readonly ConfigEntry<float> EntityTypeWeight = ConfigFile.Bind<float>("Aimbot", "Entity", 1f, "How much its type affects target selection.");

		internal static bool Enabled => Status.Enabled;
	}

	internal static class ESP
	{
		internal static readonly FeatureConfig Boxes = new FeatureConfig("ESP", "Boxes", 0);

		internal static readonly FeatureConfig Outlines = new FeatureConfig("ESP", "Outlines", 1);

		internal static readonly FeatureConfig Players = new FeatureConfig("ESP", "Players", 1, 0f);

		internal static readonly FeatureConfig VBloodCarriers = new FeatureConfig("ESP", "VBloodCarriers", 5, 0f);

		internal static readonly FeatureConfig BloodSources = new FeatureConfig("ESP", "BloodSources", 6, 90f);

		internal static readonly FeatureConfig GateBosses = new FeatureConfig("ESP", "GateBosses", 8, 0f);

		internal static readonly FeatureConfig Items = new FeatureConfig("ESP", "Items", 0, 0f);

		internal static readonly FeatureConfig Containers = new FeatureConfig("ESP", "Containers", 10, 0f);

		internal static readonly FeatureConfig Ores = new FeatureConfig("ESP", "Ores", 19, 0f);

		internal static readonly FeatureConfig Plants = new FeatureConfig("ESP", "Plants", 15, 0f);

		internal static readonly FeatureConfig FishingSpots = new FeatureConfig("ESP", "FishingSpots", 13, 0f);

		internal static readonly FeatureConfig Horses = new FeatureConfig("ESP", "Horses", 16, 90f);

		internal static readonly FeatureConfig Servants = new FeatureConfig("ESP", "Servants", 4, 0f);

		internal static readonly FeatureConfig Carriages = new FeatureConfig("ESP", "Carriages", 7, 0f);
	}

	internal static class Radar
	{
		internal static readonly FeatureConfig Status = new FeatureConfig("Radar", "Enabled");
		internal static readonly ConfigEntry<float> Size = ConfigFile.Bind<float>("Radar", "Size", 200f, "Tamanho do radar na tela (pixels).");
		internal static readonly ConfigEntry<float> Range = ConfigFile.Bind<float>("Radar", "Range", 100f, "Alcance do radar (metros).");
		internal static bool Enabled => Status.Enabled;
	}

	internal static class AutoParry
	{
		internal static readonly FeatureConfig Status = new FeatureConfig("AutoParry", "Enabled");
		internal static readonly ConfigEntry<KeyCode> ParryKey = ConfigFile.Bind<KeyCode>("AutoParry", "ParryKey", KeyCode.G, "Tecla do parry.");
		internal static readonly ConfigEntry<float> Range = ConfigFile.Bind<float>("AutoParry", "Range", 8f, "Distancia maxima pra ativar parry (metros).");
		internal static readonly ConfigEntry<float> Cooldown = ConfigFile.Bind<float>("AutoParry", "Cooldown", 0.3f, "Cooldown entre parrys (segundos).");
		internal static bool Enabled => Status.Enabled;
	}

	internal static class Extras
	{
		internal static readonly FeatureConfig AutoFishing = new FeatureConfig("Extras", "AutoFishing");

		internal static readonly FeatureConfig AutoLoot = new FeatureConfig("Extras", "AutoLoot");

		internal static readonly FeatureConfig NoFog = new FeatureConfig("Extras", "NoFog");
	}

	internal static readonly ConfigEntry<bool> ModToggle = ConfigFile.Bind<bool>("Options", "Enabled", false, "Toggle the mod.");

	private static ConfigFile ConfigFile => ((BasePlugin)Plugin.Instance).Config;
}
