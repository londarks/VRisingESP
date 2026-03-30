using ExtrasensoryPerception.Utils;
using HarmonyLib;
using ProjectM.UI;
using UnityEngine.Rendering.HighDefinition;

namespace ExtrasensoryPerception.Patches;

[HarmonyPatch(typeof(MiniMapHUDSystem), nameof(MiniMapHUDSystem.OnUpdate))]
public class MiniMapHUDPatch
{
    [HarmonyPostfix]
    private static void Postfix(MiniMapHUDSystem __instance)
    {
        if (!Config.Extras.NoFog.Enabled) return;

        try
        {
            var miniMapParent = __instance._MiniMapParent;
            if (miniMapParent == null) return;
            var curseDebuff = miniMapParent.CurseDebuffVisualization;
            if (curseDebuff != null && curseDebuff.enabled) curseDebuff.gameObject.SetActive(false);
        }
        catch { }
    }
}

[HarmonyPatch]
public class MapMenuPatch
{
    [HarmonyPatch(typeof(MapMenuMapper), nameof(MapMenuMapper.OnUpdate))]
    [HarmonyPostfix]
    private static void OnUpdate(MapMenuMapper __instance)
    {
        if (!Config.Extras.NoFog.Enabled) return;

        var mapMenu = __instance._MapMenu;
        var curseDebuff = mapMenu.CurseDebuffVisualization;
        if (curseDebuff && curseDebuff.enabled)
        {
            curseDebuff.gameObject.SetActive(false);
            mapMenu.MapTexture.gameObject.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(MapMenuMapper), nameof(MapMenuMapper.UpdateCurseAreaDebuffVisuals))]
    [HarmonyPrefix]
    private static bool UpdateCurseAreaDebuffVisuals() // Not really needed
    {
        return !Config.Extras.NoFog.Enabled;
    }
}

[HarmonyPatch(typeof(Fog), "IsFogEnabled")]
public class FogPatch
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return !Config.Extras.NoFog.Enabled;
    }
}