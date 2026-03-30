using System;
using System.Threading;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

public static class AutoParry
{
    private static float _lastParryTime;
    private static EntityQuery _projectileQuery;
    private static bool _queryInit;

    internal static void CheckEnemy(Entity enemy, float distance, bool isPlayer, bool isBoss)
    {
        if (!Config.AutoParry.Enabled) return;
        if (isPlayer && !Config.AutoParry.Players.Enabled) return;
        if (isBoss && !Config.AutoParry.Bosses.Enabled) return;
        if (!isPlayer && !isBoss && !Config.AutoParry.Mobs.Enabled) return;
        if (distance > Config.AutoParry.Range.Value) return;

        float now = Time.time;
        if (now - _lastParryTime < Config.AutoParry.Cooldown.Value) return;

        if (!enemy.TryGetComponent<AbilityBar_Shared>(out var abilityBar)) return;
        if (!abilityBar.SyncedIsCasting) return;

        var localChar = EntityList.LocalCharacter;
        if (localChar == Entity.Null || !localChar.Exists()) return;
        if (!enemy.TryGetComponent<TargetDirection>(out var targetDir)) return;

        Vector3 mobPos = enemy.GetPosition();
        Vector3 playerPos = localChar.GetPosition();
        Vector3 aimDir = new Vector3(targetDir.AimDirection.x, 0, targetDir.AimDirection.z).normalized;
        Vector3 mobToPlayer = (playerPos - mobPos).normalized;
        if (Vector3.Dot(aimDir, mobToPlayer) < 0.3f) return;

        Vector3 toPlayer = playerPos - mobPos;
        float projLen = Vector3.Dot(toPlayer, aimDir);
        if (projLen < 0) return;
        Vector3 closestPoint = mobPos + aimDir * projLen;
        float distToAimLine = Vector3.Distance(playerPos, closestPoint);

        float pr = 0.6f;
        if (localChar.TryGetComponent<CollisionRadius>(out var pc)) pr = pc.Radius;
        float mr = 0.5f;
        if (enemy.TryGetComponent<CollisionRadius>(out var mc)) mr = mc.Radius;

        if (distToAimLine > pr + mr + 0.5f) return;

        _lastParryTime = now;
        new Thread(() => KeySimulator.PressKey(Config.AutoParry.ParryKey.Value)).Start();
    }

    internal static void CheckProjectiles()
    {
        if (!Config.AutoParry.Enabled) return;

        float now = Time.time;
        if (now - _lastParryTime < Config.AutoParry.Cooldown.Value) return;

        var localChar = EntityList.LocalCharacter;
        if (localChar == Entity.Null || !localChar.Exists()) return;

        try
        {
            if (!_queryInit)
            {
                _projectileQuery = VWorld.EntityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<Projectile>(),
                    ComponentType.ReadOnly<LocalToWorld>()
                );
                _queryInit = true;
            }

            if (_projectileQuery.IsEmpty) return;

            Vector3 playerPos = localChar.GetPosition();
            float pr = 0.6f;
            if (localChar.TryGetComponent<CollisionRadius>(out var pc)) pr = pc.Radius;
            float detectRadius = pr + 4f;

            var entities = _projectileQuery.ToEntityArray(Allocator.Temp);
            try
            {
                foreach (var entity in entities)
                {
                    if (!entity.Exists()) continue;

                    Vector3 projPos = entity.GetPosition();
                    float dist = Vector3.Distance(projPos, playerPos);

                    if (dist > detectRadius) continue;

                    // Filtrar nossos projeteis (indo embora muito perto)
                    if (entity.TryGetComponent<Velocity>(out var vel))
                    {
                        Vector3 v = (Vector3)vel.Value;
                        if (v.sqrMagnitude > 0.1f)
                        {
                            Vector3 tp = (playerPos - projPos).normalized;
                            if (Vector3.Dot(v.normalized, tp) < -0.5f && dist < 2f) continue;
                        }
                    }

                    _lastParryTime = now;
                    new Thread(() => KeySimulator.PressKey(Config.AutoParry.ParryKey.Value)).Start();
                    entities.Dispose();
                    return;
                }
            }
            finally
            {
                if (entities.IsCreated) entities.Dispose();
            }
        }
        catch (Exception) { }
    }
}
