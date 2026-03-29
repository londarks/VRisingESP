using ExtrasensoryPerception.API;
using HarmonyLib;
using ProjectM;
using Stunlock.Core;
using Unity.Entities;

namespace ExtrasensoryPerception.Patches;

[HarmonyPatch(typeof(WorldBootstrapUtilities), nameof(WorldBootstrapUtilities.AddSystemsToWorld))]
public static class WorldBootstrapPatch
{
    [HarmonyPostfix]
    public static void Postfix(World world, WorldBootstrap worldConfig, WorldSystemConfig worldSystemConfig)
    {
        VWorld.Game = world;
        Plugin.Instance.OnGameInitialized();
    }
}

[HarmonyPatch(typeof(GameDataManager), nameof(GameDataManager.OnUpdate))]
public static class GameDataManagerPatch
{
    [HarmonyPostfix]
    static void Postfix(GameDataManager __instance)
    {
        if (__instance.GameDataInitialized)
        {
            Plugin.IsInGame = true;
            Plugin.Logger.LogInfo($"Entering in game!");
        }
    }
}

[HarmonyPatch(typeof(ClientBootstrapSystem), nameof(ClientBootstrapSystem.OnDestroy))]
public static class ClientBootstrapPatch
{
    [HarmonyPrefix]
    static void Prefix()
    {
        Plugin.IsInGame = false;
    }
}