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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery mainQuery = __instance._MainQuery1;
		var entities = mainQuery.ToEntityArray(Allocator.Temp);
		foreach (var current in entities)
		{
			if (current.IsAlly(EntityList.LocalPlayer))
			{
				Aimbot.ProjectileSpeed = current.Read<Projectile>().Speed;
			}
		}
		if (entities.IsCreated) entities.Dispose();
	}
}
