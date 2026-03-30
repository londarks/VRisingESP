using System;
using System.Threading;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ProjectM;
using Unity.Entities;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

/// <summary>
/// Auto-Parry: detecta quando inimigo esta castando perto e aperta parry automaticamente.
/// Se o parry esta em cooldown, o jogo ignora o input — sem consequencias.
///
/// Logica:
/// - Inimigo a menos de X metros
/// - IsCasting == true (inimigo esta atacando)
/// - AimDirection.dot > 0.5 (mirando em voce)
/// - Cooldown interno pra nao spammar
/// - Aperta Mouse5 (parry)
/// </summary>
public static class AutoParry
{
    private static float _lastParryTime;

    /// <summary>
    /// Chamado pelo EntityDebugger.AnalyzeNearbyEntity ou pelo ProcessMobs/ProcessPlayers
    /// quando detecta inimigo castando perto.
    /// </summary>
    internal static void CheckEnemy(Entity enemy, float distance)
    {
        if (!Config.AutoParry.Enabled) return;

        // Distancia maxima
        if (distance > Config.AutoParry.Range.Value) return;

        // Cooldown interno
        float now = Time.time;
        if (now - _lastParryTime < Config.AutoParry.Cooldown.Value) return;

        // Verificar se esta castando
        if (!enemy.TryGetComponent<AbilityBar_Shared>(out var abilityBar)) return;
        if (!abilityBar.SyncedIsCasting) return;

        // Verificar se esta mirando em nos
        var localChar = EntityList.LocalCharacter;
        if (localChar == Entity.Null || !localChar.Exists()) return;

        if (enemy.TryGetComponent<TargetDirection>(out var targetDir))
        {
            Vector3 mobPos = enemy.GetPosition();
            Vector3 playerPos = localChar.GetPosition();
            Vector3 mobToPlayer = (playerPos - mobPos).normalized;
            Vector3 aimDir = new Vector3(targetDir.AimDirection.x, 0, targetDir.AimDirection.z).normalized;
            float dot = Vector3.Dot(aimDir, mobToPlayer);

            // dot > 0.5 = mirando na nossa direcao geral
            if (dot < 0.5f) return;
        }

        // PARRY!
        _lastParryTime = now;

        new Thread(() =>
        {
            KeySimulator.PressKey(Config.AutoParry.ParryKey.Value);
        }).Start();

        Plugin.Logger.LogInfo($"[AutoParry] Parry ativado! Inimigo castando a {distance:F1}m");
    }
}
