using ExtrasensoryPerception.API;
using ExtrasensoryPerception.ESP;
using HarmonyLib;
using ProjectM;
using Unity.Collections;
using Unity.Entities;

namespace ExtrasensoryPerception.Patches;

[HarmonyPatch(typeof(ProjectileSystem_Spawn_Client), "OnUpdate")]
public class ProjectileSystemPatch
{
	[HarmonyPostfix]
	private static void Postfix(ProjectileSystem_Spawn_Client __instance)
	{
		EntityQuery mainQuery = __instance._MainQuery1;
		var entities = mainQuery.ToEntityArray(Allocator.Temp);
		foreach (var current in entities)
		{
			if (current.IsAlly(EntityList.LocalCharacter))
			{
				Aimbot.ProjectileSpeed = current.Read<Projectile>().Speed;
			}
		}
		if (entities.IsCreated) entities.Dispose();
	}
}
