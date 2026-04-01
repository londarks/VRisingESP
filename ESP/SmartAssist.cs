using System;
using System.Threading;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

/// <summary>
/// Smart Assist:
/// 1) Auto-activates aimbot when local player casts aimed abilities
/// 2) Auto-presses weapon skill on weapon swap
/// </summary>
public static class SmartAssist
{
    private static string _lastWeaponBuff = "";
    private static float _lastSwapTime;
    private const float SwapCooldown = 1.5f; // prevent rapid-fire swaps
    private static bool _wasCasting;
    private static bool _aimAssistActive;
    private static bool _counterDetected;

    // Enemy counter/parry buffs — when active on target, STOP attacking
    private static readonly string[] CounterBuffPrefixes = {
        "AB_Blood_BloodRite_Buff",         // Rito de Sangue
        "AB_Illusion_MistTrance_Buff",     // Transe de Névoa
        "AB_FrostBarrier_Buff",            // Onda de Frio
        "AB_Storm_Discharge_Buff",         // Descarga
        "AB_Storm_Discharge_StormShield",  // Descarga (variante)
    };

    // Spells that should NOT trigger aim-lock (dashes, shields, counters, self-buffs)
    private static readonly string[] AimIgnorePrefixes = {
        // Veils / Dashes
        "AB_Vampire_VeilOf",
        "AB_Vampire_Slashers_Camouflage_Main",
        "AB_Vampire_Slashers_ElusiveStrike_Dash",
        "AB_Vampire_Whip_Dash",
        "AB_Pistols_ExplosiveShot_DashCast",
        "AB_GreatSword_GreatCleaver_DashStrike",
        // AoE (não precisa mirar em alguém)
        "AB_Chaos_Aftershock",
        "AB_Frost_ColdSnap",
        "AB_Frost_IceNova",
        "AB_Frost_Nova",
        "AB_Unholy_CorpseExplosion",
        "AB_Unholy_Pestilence",
        "AB_Blood_UnstableMosquito",
        "AB_Blood_Mosquito",
        "AB_Storm_Discharge",
        // Shields / Barriers
        "AB_FrostBarrier",
        "AB_Chaos_Barrier",
        "AB_Unholy_WardOfTheDamned",
        "AB_Unholy_Ward",
        // Counters / Parry / Defensivos (quando EU uso)
        "AB_Blood_BloodRite",
        "AB_Illusion_MistTrance",
        "AB_Storm_Discharge",
        "AB_Vampire_Parry",
        "AB_General_Counter",
        // Self-buffs / utility
        "AB_Chaos_PowerSurge",
        "AB_Illusion_PhantomAegis",
        "AB_Illusion_MistTrance",
        "AB_Shapeshift",
        // Interações (portas, itens, etc)
        "AB_Interact",
        // Feed / consumíveis / montaria
        "AB_Feed",
        "AB_FeedBoss",
        "AB_Subdue",
        "AB_Consumable",
        "AB_Gallop",
        "AB_Horse_Vampire",
        "AB_VampireMountLeap",
        // AoE / dash-attacks
        "AB_Frost_ArcticLeap",
        "AB_Chaos_MercilessCharge",
        "AB_Storm_LightningTyphoon",
        // Montaria
        "AB_Vampire_Spear_Primary_Mounted",
    };

    // Weapon swap → auto-cast key mapping
    // Key = substring to match in EquipBuff_Weapon_*, Value = key to press
    private static readonly (string weapon, KeyCode key)[] WeaponSwapActions = {
        ("Pistols", KeyCode.E),
        ("Crossbow", KeyCode.E),
        ("GreatSword", KeyCode.Q),
        ("Longbow", KeyCode.E),
        ("Reaper", KeyCode.Q),
        ("Slashers", KeyCode.E),
        ("Mace", KeyCode.Q),
        ("Sword", KeyCode.E),
        ("Axe", KeyCode.E),
        ("Whip", KeyCode.Q),
    };

    /// <summary>
    /// Called every frame from Overlay.Update()
    /// </summary>
    internal static void Update()
    {
        if (!Config.SmartAssist.Enabled) return;

        var localChar = EntityList.LocalCharacter;
        if (localChar == Entity.Null || !localChar.Exists()) return;

        CheckEnemyCounter();
        CheckAimOnCast(localChar);
        CheckWeaponSwap(localChar);
    }

    /// <summary>
    /// Module 1: Activate aimbot when local player is casting aimed abilities.
    /// </summary>
    private static void CheckAimOnCast(Entity localChar)
    {
        if (!localChar.TryGetComponent<AbilityBar_Shared>(out var abilityBar)) return;

        bool isCasting = abilityBar.SyncedIsCasting;

        if (isCasting && !_wasCasting)
        {
            // Cast just started — check if it's an aimed ability
            string spellName = GetCastSpellName(abilityBar);

            if (!string.IsNullOrEmpty(spellName) && !IsAimIgnored(spellName))
            {
                _aimAssistActive = true;
                Aimbot.Active = true;
                FileLogger.Log($"[SmartAssist] AIM ON - casting: {spellName}");
            }
        }
        else if (!isCasting && _wasCasting)
        {
            // Cast ended — deactivate aimbot
            if (_aimAssistActive)
            {
                _aimAssistActive = false;
                Aimbot.Active = false;
                FileLogger.Log("[SmartAssist] AIM OFF - cast ended");
            }
        }

        _wasCasting = isCasting;
    }

    /// <summary>
    /// Module 2: Detect weapon swap and auto-press the configured ability.
    /// </summary>
    private static void CheckWeaponSwap(Entity localChar)
    {
        if (!Config.SmartAssist.QuickCast.Enabled) return;

        try
        {
            if (!VWorld.EntityManager.HasBuffer<BuffBuffer>(localChar)) return;

            var buffs = VWorld.EntityManager.GetBuffer<BuffBuffer>(localChar);
            var map = VWorld.PrefabLookupMap;

            string currentWeapon = "";
            for (int i = 0; i < buffs.Length; i++)
            {
                string name = map.GetName(buffs[i].PrefabGuid);
                if (name.Contains("EquipBuff_Weapon_") && name.Contains("Ability"))
                {
                    currentWeapon = name;
                    break;
                }
            }

            if (string.IsNullOrEmpty(currentWeapon)) return;
            if (currentWeapon == _lastWeaponBuff) return;

            // Weapon changed!
            string previousWeapon = _lastWeaponBuff;
            _lastWeaponBuff = currentWeapon;

            // Don't trigger on first detection (game start)
            if (string.IsNullOrEmpty(previousWeapon)) return;

            // Cooldown to prevent rapid-fire triggers
            float now = Time.time;
            if (now - _lastSwapTime < SwapCooldown) return;
            _lastSwapTime = now;

            // Find matching action
            foreach (var (weapon, key) in WeaponSwapActions)
            {
                if (currentWeapon.Contains(weapon))
                {
                    FileLogger.Log($"[SmartAssist] WEAPON SWAP: {currentWeapon} -> auto-press {key}");
                    System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Thread.Sleep(50);
                        KeySimulator.PressKey(key);
                    });
                    break;
                }
            }
        }
        catch (Exception) { }
    }

    private static string GetCastSpellName(AbilityBar_Shared abilityBar)
    {
        try
        {
            var castGuid = abilityBar.CastAbilityPrefabGuid;
            if (castGuid != PrefabGUID.Empty)
            {
                return VWorld.PrefabLookupMap.GetName(castGuid);
            }
        }
        catch { }
        return "";
    }

    private static bool IsAimIgnored(string spellName)
    {
        foreach (var prefix in AimIgnorePrefixes)
        {
            if (spellName.StartsWith(prefix, StringComparison.Ordinal)) return true;
        }
        return false;
    }

    /// <summary>
    /// Module 3: Detect if aimbot target has counter/parry buff active.
    /// If so, disable aim to avoid feeding the counter.
    /// </summary>
    private static void CheckEnemyCounter()
    {
        if (!_aimAssistActive && !Aimbot.Active)
        {
            _counterDetected = false;
            return;
        }

        // Check if current aimbot target has a counter buff
        if (!Aimbot.HasValidTarget())
        {
            _counterDetected = false;
            return;
        }

        try
        {
            var target = Aimbot.CurrentTarget;
            if (target == Entity.Null || !target.Exists())
            {
                _counterDetected = false;
                return;
            }

            string counterBuff = FindCounterBuffName(target);
            bool hasCounter = counterBuff != null;

            if (hasCounter && !_counterDetected)
            {
                _counterDetected = true;
                Aimbot.Active = false;
                FileLogger.Log($"[SmartAssist] COUNTER DETECTED on target — aim disabled (buff: {counterBuff})");
            }
            else if (!hasCounter && _counterDetected)
            {
                // Counter ended
                _counterDetected = false;
                FileLogger.Log($"[SmartAssist] COUNTER ENDED — aim re-enabled");
            }
        }
        catch { _counterDetected = false; }
    }

    /// <summary>
    /// Returns the counter buff name if found, or null if no counter is active.
    /// Single-pass: replaces both HasCounterBuff + FindCounterBuffName.
    /// </summary>
    private static string FindCounterBuffName(Entity entity)
    {
        try
        {
            if (!VWorld.EntityManager.HasBuffer<BuffBuffer>(entity)) return null;
            var buffs = VWorld.EntityManager.GetBuffer<BuffBuffer>(entity);
            var map = VWorld.PrefabLookupMap;
            for (int i = 0; i < buffs.Length; i++)
            {
                string name = map.GetName(buffs[i].PrefabGuid);
                foreach (var prefix in CounterBuffPrefixes)
                {
                    if (name.StartsWith(prefix, StringComparison.Ordinal)) return name;
                }
            }
        }
        catch { }
        return null;
    }

    internal static bool IsAimAssistActive => _aimAssistActive;
    internal static bool IsCounterDetected => _counterDetected;
}
