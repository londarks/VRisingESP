using System;
using System.Threading;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ProjectM;
using Unity.Entities;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

/// <summary>
/// Auto-Parry baseado em PROXIMIDADE DE IMPACTO, não em CastTime.
///
/// Logica:
/// - Inimigo está castando (IsCasting=true)
/// - Inimigo está mirando em mim (AimDirection check com CollisionRadius)
/// - Inimigo está MUITO PERTO (dentro do TriggerRange, que deve ser curto ~2-3m)
/// - O ataque VAI me atingir (geometria da linha de mira)
/// - → Parry AGORA
///
/// Isso significa que o parry ativa no ULTIMO MOMENTO possivel,
/// quando o inimigo já está perto o suficiente pra não cancelar.
/// </summary>
public static class AutoParry
{
    private static float _lastParryTime;

    internal static void CheckEnemy(Entity enemy, float distance)
    {
        if (!Config.AutoParry.Enabled) return;

        // Só checar inimigos dentro do range de trigger (deve ser curto, ~2-4m)
        if (distance > Config.AutoParry.Range.Value) return;

        // Cooldown minimo entre parrys
        float now = Time.time;
        if (now - _lastParryTime < Config.AutoParry.Cooldown.Value) return;

        // Precisa estar castando
        if (!enemy.TryGetComponent<AbilityBar_Shared>(out var abilityBar)) return;
        if (!abilityBar.SyncedIsCasting) return;

        var localChar = EntityList.LocalCharacter;
        if (localChar == Entity.Null || !localChar.Exists()) return;

        // Precisa de TargetDirection pra calcular mira
        if (!enemy.TryGetComponent<TargetDirection>(out var targetDir)) return;

        Vector3 mobPos = enemy.GetPosition();
        Vector3 playerPos = localChar.GetPosition();
        Vector3 mobToPlayer = (playerPos - mobPos).normalized;
        Vector3 aimDir = new Vector3(targetDir.AimDirection.x, 0, targetDir.AimDirection.z).normalized;

        // Dot check — mirando na minha direcao?
        float dot = Vector3.Dot(aimDir, mobToPlayer);
        if (dot < 0.3f) return;

        // Calculo de hitbox — linha de mira passa pela minha CollisionRadius?
        Vector3 toPlayer = playerPos - mobPos;
        float projLen = Vector3.Dot(toPlayer, aimDir);
        if (projLen < 0) return;

        Vector3 closestPoint = mobPos + aimDir * projLen;
        float distToAimLine = Vector3.Distance(playerPos, closestPoint);

        float playerRadius = 0.6f;
        if (localChar.TryGetComponent<CollisionRadius>(out var playerColl))
            playerRadius = playerColl.Radius;

        float mobRadius = 0.5f;
        if (enemy.TryGetComponent<CollisionRadius>(out var mobColl))
            mobRadius = mobColl.Radius;

        float hitThreshold = playerRadius + mobRadius + 0.5f;
        if (distToAimLine > hitThreshold) return;

        // VAI ATINGIR + PERTO = PARRY!
        _lastParryTime = now;

        new Thread(() =>
        {
            KeySimulator.PressKey(Config.AutoParry.ParryKey.Value);
        }).Start();
    }
}
