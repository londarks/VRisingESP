using System;
using System.Threading;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ProjectM;
using Unity.Entities;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

/// <summary>
/// Auto-Parry: so ativa parry quando o ataque VAI ATINGIR de verdade.
/// Usa CollisionRadius do player e do inimigo + linha de mira pra calcular
/// se a trajetoria do ataque passa pelo nosso hitbox.
/// </summary>
public static class AutoParry
{
    private static float _lastParryTime;

    internal static void CheckEnemy(Entity enemy, float distance)
    {
        if (!Config.AutoParry.Enabled) return;
        if (distance > Config.AutoParry.Range.Value) return;

        float now = Time.time;
        if (now - _lastParryTime < Config.AutoParry.Cooldown.Value) return;

        // Verificar se esta castando
        if (!enemy.TryGetComponent<AbilityBar_Shared>(out var abilityBar)) return;
        if (!abilityBar.SyncedIsCasting) return;

        var localChar = EntityList.LocalCharacter;
        if (localChar == Entity.Null || !localChar.Exists()) return;

        // Precisa de TargetDirection pra calcular linha de mira
        if (!enemy.TryGetComponent<TargetDirection>(out var targetDir)) return;

        Vector3 mobPos = enemy.GetPosition();
        Vector3 playerPos = localChar.GetPosition();
        Vector3 mobToPlayer = (playerPos - mobPos).normalized;
        Vector3 aimDir = new Vector3(targetDir.AimDirection.x, 0, targetDir.AimDirection.z).normalized;

        // Dot check basico — mirando na nossa direcao?
        float dot = Vector3.Dot(aimDir, mobToPlayer);
        if (dot < 0.3f) return; // nem perto de mirar em nos

        // === CALCULO DE HITBOX ===
        // Distancia do player à linha de mira do inimigo
        Vector3 toPlayer = playerPos - mobPos;
        float projLen = Vector3.Dot(toPlayer, aimDir);
        if (projLen < 0) return; // ataque vai pra tras, nao pra frente

        Vector3 closestPoint = mobPos + aimDir * projLen;
        float distToAimLine = Vector3.Distance(playerPos, closestPoint);

        // Raios de colisao
        float playerRadius = 0.6f; // padrao
        if (localChar.TryGetComponent<CollisionRadius>(out var playerColl))
            playerRadius = playerColl.Radius;

        float mobRadius = 0.5f; // padrao
        if (enemy.TryGetComponent<CollisionRadius>(out var mobColl))
            mobRadius = mobColl.Radius;

        // Threshold: se a linha de mira passa dentro da soma dos raios + margem
        float hitThreshold = playerRadius + mobRadius + 0.5f;
        bool willHit = distToAimLine < hitThreshold;

        if (!willHit) return; // VAI ERRAR — nao ativa parry

        // === VAI ATINGIR — PARRY! ===
        _lastParryTime = now;

        new Thread(() =>
        {
            KeySimulator.PressKey(Config.AutoParry.ParryKey.Value);
        }).Start();

        Plugin.Logger.LogInfo($"[AutoParry] VAI ATINGIR! dist={distance:F1}m aimLine={distToAimLine:F2}m threshold={hitThreshold:F2}m -> Parry!");
    }
}
