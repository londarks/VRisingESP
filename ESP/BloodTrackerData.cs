using System;
using System.Collections.Generic;
using ExtrasensoryPerception.API;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

/// <summary>
/// Armazena posicoes 3D das fontes de sangue e desenha linhas/setas no OnGUI.
/// Se a fonte esta na tela: linha do player ate ela.
/// Se esta fora da tela: seta na borda apontando a direcao.
/// </summary>
public static class BloodTrackerData
{
    private static readonly List<(Vector3 worldPos, Color color)> _targets = new();

    public static void Add(Vector3 worldPos, Color color)
    {
        _targets.Add((worldPos, color));
    }

    public static void Clear()
    {
        _targets.Clear();
    }

    /// <summary>
    /// Desenha linhas/setas do player ate cada fonte de sangue.
    /// Chamado dentro do OnGUI.
    /// </summary>
    public static void DrawLines()
    {
        if (_targets.Count == 0) return;

        var localChar = EntityList.LocalCharacter;
        if (localChar == Unity.Entities.Entity.Null) return;

        Vector3 playerWorldPos = localChar.GetPosition();

        // Posicao do player na tela
        var cam = Logic.MainCamera;
        if (cam == null) return;

        Vector3 playerScreenV3 = cam.WorldToScreenPoint(playerWorldPos);
        Vector2 playerScreen = new Vector2(playerScreenV3.x, Screen.height - playerScreenV3.y);

        float sw = Screen.width;
        float sh = Screen.height;

        foreach (var (worldPos, color) in _targets)
        {
            // Projetar a fonte de sangue na tela
            Vector3 targetScreenV3 = cam.WorldToScreenPoint(worldPos);
            Vector2 targetScreen = new Vector2(targetScreenV3.x, sh - targetScreenV3.y);

            // Calcular direcao do player ate o alvo em coordenadas de tela
            Vector2 dir = targetScreen - playerScreen;
            float distance = dir.magnitude;
            if (distance < 5f) continue;

            // Se esta dentro da tela, desenhar linha completa
            bool onScreen = targetScreenV3.z > 0 &&
                            targetScreen.x > 0 && targetScreen.x < sw &&
                            targetScreen.y > 0 && targetScreen.y < sh;

            if (onScreen)
            {
                // Linha do player ate a fonte
                Primitives.DrawLine(playerScreen, targetScreen, color);
            }
            else
            {
                // Fora da tela: desenhar seta na borda apontando a direcao
                // Se atras da camera, inverter direcao
                if (targetScreenV3.z < 0)
                {
                    dir = -dir;
                }

                dir.Normalize();

                // Encontrar ponto na borda da tela
                Vector2 borderPoint = GetBorderPoint(playerScreen, dir, sw, sh);

                // Linha do player ate a borda
                Primitives.DrawLine(playerScreen, borderPoint, color);

                // Desenhar seta na borda
                DrawArrow(borderPoint, dir, color);

                // Distancia em metros
                float dist3d = Vector3.Distance(playerWorldPos, worldPos);
                Primitives.DrawString(borderPoint + new Vector2(0, -20), $"{dist3d:F0}m", color, 14);
            }
        }
    }

    /// <summary>
    /// Encontra o ponto onde a linha cruza a borda da tela.
    /// </summary>
    private static Vector2 GetBorderPoint(Vector2 origin, Vector2 dir, float sw, float sh)
    {
        float margin = 30f;
        float minX = margin, maxX = sw - margin;
        float minY = margin, maxY = sh - margin;

        float t = float.MaxValue;

        // Borda direita
        if (dir.x > 0.001f) t = Math.Min(t, (maxX - origin.x) / dir.x);
        // Borda esquerda
        if (dir.x < -0.001f) t = Math.Min(t, (minX - origin.x) / dir.x);
        // Borda inferior
        if (dir.y > 0.001f) t = Math.Min(t, (maxY - origin.y) / dir.y);
        // Borda superior
        if (dir.y < -0.001f) t = Math.Min(t, (minY - origin.y) / dir.y);

        if (t == float.MaxValue) t = 100f;

        Vector2 point = origin + dir * t;

        // Clampar dentro da tela
        point.x = Mathf.Clamp(point.x, minX, maxX);
        point.y = Mathf.Clamp(point.y, minY, maxY);

        return point;
    }

    /// <summary>
    /// Desenha uma seta (triangulo) na direcao indicada.
    /// </summary>
    private static void DrawArrow(Vector2 tip, Vector2 dir, Color color)
    {
        float size = 15f;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        Vector2 left = tip - dir * size + perp * (size * 0.5f);
        Vector2 right = tip - dir * size - perp * (size * 0.5f);

        Primitives.DrawLine(tip, left, color);
        Primitives.DrawLine(tip, right, color);
        Primitives.DrawLine(left, right, color);
    }
}
