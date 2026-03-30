using System;
using System.Collections.Generic;
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

    // Track enemy casting state to detect cast START (not repeat every frame)
    private static readonly Dictionary<Entity, bool> _wasCasting = new();

    // IGNORE LIST: spells impossible to parry (AoE, shields, dashes, buffs).
    // Everything NOT here goes through aim-check and can trigger parry.
    private static readonly string[] IgnorePrefixes = {
        // === Veils / Dashes ===
        "AB_Vampire_VeilOfChaos_DashCast",
        "AB_Vampire_VeilOfChaos_Recast_DashCast",
        "AB_Vampire_VeilOfIllusion_DashCast",
        "AB_Vampire_VeilOfIllusion_Recast",
        "AB_Vampire_VeilOfStorm_DashCast",
        "AB_Vampire_VeilOfFrost_DashCast",
        "AB_Vampire_VeilOfBones",
        "AB_Vampire_Slashers_Camouflage_Main",
        "AB_Vampire_Slashers_ElusiveStrike_Dash",
        "AB_Vampire_Whip_Dash",
        "AB_Pistols_ExplosiveShot_DashCast",
        // === AoE (impossivel dar parry) ===
        "AB_Chaos_Aftershock",
        "AB_Frost_IceNova",
        "AB_Frost_Nova",
        "AB_Unholy_CorpseExplosion",
        "AB_Unholy_Pestilence",
        "AB_Blood_UnstableMosquito",
        "AB_Blood_Mosquito",
        // === Shields / Barriers ===
        "AB_FrostBarrier",
        "AB_Chaos_Barrier",
        "AB_Chaos_PowerSurge",
        "AB_Unholy_WardOfTheDamned",
        "AB_Unholy_Ward",
        // === Self-buffs / utility ===
        "AB_Illusion_PhantomAegis",
        "AB_Illusion_MistTrance",
        "AB_Shapeshift",
        "AB_Storm_Discharge",
    };

    // Spells with extremely short cast times (melee) — parry on cast START, don't wait for aim check
    private static readonly string[] InstantMeleePrefixes = {
        "AB_Vampire_Spear_Primary_Attack",
        "AB_Vampire_Whip_Primary",
        "AB_Vampire_GreatSword_Primary",
        "AB_Vampire_Reaper_TendonSwing",
        "AB_Vampire_Sword_Shockwave",
        "AB_Vampire_Axe_",
        "AB_Vampire_Slashers_Primary_MeleeAttack",
        "AB_Vampire_Unarmed_Primary_MeleeAttack",
        "AB_Spear_AThousandSpears",
    };

    internal static void CheckEnemy(Entity enemy, float distance, bool isPlayer, bool isBoss)
    {
        if (!Config.AutoParry.Enabled) return;
        if (isPlayer && !Config.AutoParry.Players.Enabled) return;
        if (isBoss && !Config.AutoParry.Bosses.Enabled) return;
        if (!isPlayer && !isBoss && !Config.AutoParry.Mobs.Enabled) return;

        float now = Time.time;
        if (now - _lastParryTime < Config.AutoParry.Cooldown.Value) return;

        if (!enemy.TryGetComponent<AbilityBar_Shared>(out var abilityBar)) return;

        bool isCasting = abilityBar.SyncedIsCasting;
        bool wasCasting = _wasCasting.TryGetValue(enemy, out var prev) && prev;
        _wasCasting[enemy] = isCasting;

        if (!isCasting) return;

        var localChar = EntityList.LocalCharacter;
        if (localChar == Entity.Null || !localChar.Exists()) return;

        // Get spell name to classify the attack
        string spellName = "";
        try
        {
            var castGuid = abilityBar.CastAbilityPrefabGuid;
            if (castGuid != PrefabGUID.Empty)
            {
                var prefabMap = VWorld.PrefabLookupMap;
                spellName = prefabMap.GetName(castGuid);
            }
        }
        catch { }

        // Skip dashes and movement abilities
        if (IsIgnored(spellName)) return;

        bool isInstantMelee = IsInstantMelee(spellName);

        // For melee range: use generous detection (from logs, melee hits at 7-13m)
        float maxRange = isInstantMelee ? 15f : Config.AutoParry.Range.Value;
        if (distance > maxRange) return;

        if (!enemy.TryGetComponent<TargetDirection>(out var targetDir)) return;

        Vector3 mobPos = enemy.GetPosition();
        Vector3 playerPos = localChar.GetPosition();
        Vector3 aimDir = new Vector3(targetDir.AimDirection.x, 0, targetDir.AimDirection.z).normalized;
        Vector3 mobToPlayer = (playerPos - mobPos).normalized;
        float dot = Vector3.Dot(aimDir, mobToPlayer);

        if (isInstantMelee)
        {
            // Instant melee (CastTime 0.01-0.04s): parry on cast START if aiming vaguely at us
            // From logs: dot >= 0.85 + DistToAimLine <= ~2m = reliable hit
            // But we only trigger on the FIRST frame of casting (not every frame)
            if (!wasCasting)
            {
                // First frame of this cast - check if aimed at us
                if (dot < 0.7f) return;

                Vector3 toPlayer = playerPos - mobPos;
                float projLen = Vector3.Dot(toPlayer, aimDir);
                if (projLen < 0) return;
                Vector3 closestPoint = mobPos + aimDir * projLen;
                float distToAimLine = Vector3.Distance(playerPos, closestPoint);

                float playerRadius = 0.6f;
                if (localChar.TryGetComponent<CollisionRadius>(out var pc)) playerRadius = pc.Radius;
                float mobRadius = 0.5f;
                if (enemy.TryGetComponent<CollisionRadius>(out var mc)) mobRadius = mc.Radius;

                // Generous threshold for melee: includes lunge distance
                if (distToAimLine > playerRadius + mobRadius + 2.0f) return;

                _lastParryTime = now;
                new Thread(() => KeySimulator.PressKey(Config.AutoParry.ParryKey.Value)).Start();
            }
        }
        else
        {
            // Ranged/spell attacks (CastTime 0.2-0.8s): stricter aim check, can check every frame
            if (dot < 0.85f) return;

            Vector3 toPlayer = playerPos - mobPos;
            float projLen = Vector3.Dot(toPlayer, aimDir);
            if (projLen < 0) return;
            Vector3 closestPoint = mobPos + aimDir * projLen;
            float distToAimLine = Vector3.Distance(playerPos, closestPoint);

            float playerRadius = 0.6f;
            if (localChar.TryGetComponent<CollisionRadius>(out var pc)) playerRadius = pc.Radius;
            float mobRadius = 0.5f;
            if (enemy.TryGetComponent<CollisionRadius>(out var mc)) mobRadius = mc.Radius;

            if (distToAimLine > playerRadius + mobRadius + 1.0f) return;

            _lastParryTime = now;
            new Thread(() => KeySimulator.PressKey(Config.AutoParry.ParryKey.Value)).Start();
        }
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
            float playerRadius = 0.6f;
            if (localChar.TryGetComponent<CollisionRadius>(out var pc)) playerRadius = pc.Radius;

            // Detect projectiles heading toward us within a radius
            float detectRadius = playerRadius + 4f;

            var entities = _projectileQuery.ToEntityArray(Allocator.Temp);
            try
            {
                foreach (var entity in entities)
                {
                    if (!entity.Exists()) continue;

                    Vector3 projPos = entity.GetPosition();
                    float dist = Vector3.Distance(projPos, playerPos);
                    if (dist > detectRadius) continue;

                    // Check velocity: projectile must be moving TOWARD us
                    if (entity.TryGetComponent<Velocity>(out var vel))
                    {
                        Vector3 v = (Vector3)vel.Value;
                        if (v.sqrMagnitude > 0.1f)
                        {
                            Vector3 toPlayer = (playerPos - projPos).normalized;
                            float dotToPlayer = Vector3.Dot(v.normalized, toPlayer);

                            // Must be heading toward player (dot > 0.3)
                            if (dotToPlayer < 0.3f) continue;
                        }
                    }

                    // Skip if it's our own projectile (same team)
                    if (entity.IsAlly(localChar)) continue;

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

    private static bool IsIgnored(string spellName)
    {
        if (string.IsNullOrEmpty(spellName)) return false;
        foreach (var prefix in IgnorePrefixes)
        {
            if (spellName.StartsWith(prefix, StringComparison.Ordinal)) return true;
        }
        return false;
    }

    private static bool IsInstantMelee(string spellName)
    {
        if (string.IsNullOrEmpty(spellName)) return false;
        foreach (var prefix in InstantMeleePrefixes)
        {
            if (spellName.StartsWith(prefix, StringComparison.Ordinal)) return true;
        }
        return false;
    }
}
