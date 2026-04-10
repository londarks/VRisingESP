using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

/// <summary>
/// Plays a short beep when an enemy player is first detected nearby.
/// Uses Windows Console.Beep on a background thread to avoid IL2CPP issues.
/// </summary>
internal static class AlertSound
{
    private static readonly HashSet<int> _alerted = new();

    private const float Cooldown = 3f;
    private static float _lastBeepTime;

    internal static void BeginFrame()
    {
        _alerted.Clear();
    }

    internal static void OnEnemyPlayerDetected(int entityIndex)
    {
        if (!Utils.Config.ESP.EnemyAlert.Enabled) return;
        if (!_alerted.Add(entityIndex)) return;

        float now = Time.time;
        if (now - _lastBeepTime < Cooldown) return;
        _lastBeepTime = now;

        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                Console.Beep(800, 150);
            }
            catch { }
        });
    }
}
