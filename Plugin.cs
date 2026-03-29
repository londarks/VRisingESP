using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using ExtrasensoryPerception.ESP;
using ExtrasensoryPerception.UI;
using HarmonyLib;

namespace ExtrasensoryPerception;

[BepInPlugin("ExtrasensoryPerception", "ExtrasensoryPerception", "0.2.1.1")]
public class Plugin : BasePlugin
{
	private Harmony _harmony;

	public static bool IsMenuOpen;

	public static bool IsInGame;

	[field: CompilerGenerated]
	public static Plugin Instance
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public static ManualLogSource Logger => ((BasePlugin)Instance).Log;

	public override void Load()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		Instance = this;
		((BasePlugin)this).Config.SaveOnConfigSet = true;
		ManualLogSource log = ((BasePlugin)this).Log;
		bool flag = default(bool);
		BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(27, 2, out flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Plugin ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>("ExtrasensoryPerception");
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" version ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>("0.2.1.1");
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" is loaded!");
		}
		log.LogInfo(val);
		_harmony = new Harmony("ExtrasensoryPerception");
		_harmony.PatchAll(Assembly.GetExecutingAssembly());
		((BasePlugin)this).AddComponent<Menu>();
		((BasePlugin)this).AddComponent<Overlay>();
		((BasePlugin)this).AddComponent<AimController>();
	}

	public void OnGameInitialized()
	{
		EntityList.InitializeQueries();
	}

	public override bool Unload()
	{
		_harmony.UnpatchSelf();
		return true;
	}
}
