using ExtrasensoryPerception.API;
using ExtrasensoryPerception.ESP;
using ExtrasensoryPerception.Utils;
using HarmonyLib;
using ProjectM;
using Unity.Collections;

namespace ExtrasensoryPerception.Patches;

[HarmonyPatch(typeof(BuffSystem_Spawn_Client), nameof(BuffSystem_Spawn_Client.OnUpdate))]
public class BuffSystemPatch
{
    [HarmonyPostfix]
    private static void Postfix(BuffSystem_Spawn_Client __instance)
    {
        if (!Config.Extras.AutoFishing.Enabled || __instance._Query.IsEmpty) return;

        foreach (var buffs in __instance._Query.ToEntityArray(Allocator.Temp))
            if (buffs.GetName() == "AB_Fishing_Target_ReadyBuff") // 1753229314
            {
                var owner = buffs.Read<EntityOwner>().Owner;
                if (owner.TryGetComponent<PlayerCharacter>(out var player) && EntityList.IsLocalCharacter(player))
                {
                    Plugin.Logger.LogDebug("There's a fish ready to catch!");
                    MouseSimulator.LeftClick();
                    break;
                }
            }
    }
}