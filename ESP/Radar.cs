using System;
using System.Collections.Generic;
using ExtrasensoryPerception.API;
using ExtrasensoryPerception.Utils;
using ProjectM;
using Unity.Entities;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

/// <summary>
/// Radar/minimapa com pontos coloridos representando entidades.
/// </summary>
public static class Radar
{
    internal struct RadarEntity
    {
        public Vector3 WorldPos;
        public Color Color;
        public string Label;
        public bool IsImportant;
    }

    internal static readonly List<RadarEntity> Entities = new();

    internal static void AddEntity(Vector3 worldPos, Color color, string label, bool isImportant = false)
    {
        Entities.Add(new RadarEntity
        {
            WorldPos = worldPos,
            Color = color,
            Label = label,
            IsImportant = isImportant
        });
    }

    public static void Clear()
    {
        Entities.Clear();
    }

    public static void DrawRadar()
    {
        if (!Config.Radar.Enabled) return;

        var localChar = EntityList.LocalCharacter;
        if (localChar == Entity.Null || !localChar.Exists()) return;

        Vector3 playerPos = localChar.GetPosition();

        float radarSize = Config.Radar.Size.Value;
        float margin = 15f;
        float radarX = Screen.width - radarSize - margin;
        float radarY = margin;
        float radarCenterX = radarX + radarSize / 2f;
        float radarCenterY = radarY + radarSize / 2f;
        float radarRadius = radarSize / 2f;
        float radarRange = Config.Radar.Range.Value;

        // === FUNDO ===
        DrawBackground(radarX, radarY, radarSize);

        // === ANEIS DE DISTANCIA ===
        Color ringColor = new Color(0.3f, 0.6f, 0.3f, 0.15f);
        DrawRing(radarCenterX, radarCenterY, radarRadius * 0.33f, ringColor);
        DrawRing(radarCenterX, radarCenterY, radarRadius * 0.66f, ringColor);
        DrawRing(radarCenterX, radarCenterY, radarRadius * 0.95f, new Color(0.2f, 0.8f, 0.2f, 0.4f));

        // === CRUZ CENTRAL ===
        Color crossColor = new Color(1, 1, 1, 0.1f);
        float cx = radarCenterX, cy = radarCenterY;
        DrawLine(cx, radarY + 5, cx, radarY + radarSize - 5, crossColor);
        DrawLine(radarX + 5, cy, radarX + radarSize - 5, cy, crossColor);

        // === ENTIDADES ===
        foreach (var ent in Entities)
        {
            float dx = ent.WorldPos.x - playerPos.x;
            float dz = ent.WorldPos.z - playerPos.z;

            float radarDX = (dx / radarRange) * radarRadius;
            float radarDZ = (dz / radarRange) * radarRadius;

            float dotX = radarCenterX + radarDX;
            float dotY = radarCenterY - radarDZ;

            // Clampar dentro do radar
            float distFromCenter = Mathf.Sqrt((dotX - cx) * (dotX - cx) + (dotY - cy) * (dotY - cy));
            if (distFromCenter > radarRadius - 8f)
            {
                // Na borda — mostrar como ponto na borda
                float angle = Mathf.Atan2(dotY - cy, dotX - cx);
                dotX = cx + Mathf.Cos(angle) * (radarRadius - 8f);
                dotY = cy + Mathf.Sin(angle) * (radarRadius - 8f);
            }

            float dotSize = ent.IsImportant ? 5f : 3f;
            DrawDot(dotX, dotY, dotSize, ent.Color);
        }

        // === PLAYER NO CENTRO ===
        DrawDot(cx, cy, 4f, Color.white);
        // Seta indicando "norte" (Z+)
        DrawLine(cx, cy - 6, cx - 3, cy - 2, Color.white);
        DrawLine(cx, cy - 6, cx + 3, cy - 2, Color.white);

        // === LEGENDA ===
        float legendY = radarY + radarSize + 8f;
        Primitives.DrawString(new Vector2(cx, legendY), $"Radar ({(int)radarRange}m)", Color.white, 10);
        legendY += 14f;

        // Mini legenda de cores
        float legendX = radarX;
        DrawLegendItem(legendX, legendY, Color.white, "Voce");
        DrawLegendItem(legendX + 55, legendY, Color.red, "Inimigo");
        DrawLegendItem(legendX + 125, legendY, Color.magenta, "VBlood");
        legendY += 13f;
        DrawLegendItem(legendX, legendY, Color.green, "Aliado");
        DrawLegendItem(legendX + 55, legendY, Color.yellow, "Sangue");

        // Contagem
        if (Entities.Count > 0)
        {
            Primitives.DrawString(new Vector2(cx, legendY + 16f),
                $"{Entities.Count} entidades", new Color(0.6f, 0.6f, 0.6f), 9);
        }
    }

    // === HELPERS DE DESENHO ===

    private static Texture2D? _pixelTex;

    private static Texture2D PixelTexture
    {
        get
        {
            if (_pixelTex == null)
            {
                _pixelTex = new Texture2D(1, 1);
                _pixelTex.SetPixel(0, 0, Color.white);
                _pixelTex.Apply();
            }
            return _pixelTex;
        }
    }

    private static void DrawDot(float x, float y, float size, Color color)
    {
        var prev = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(new Rect(x - size, y - size, size * 2, size * 2), PixelTexture);
        GUI.color = prev;
    }

    private static void DrawBackground(float x, float y, float size)
    {
        // Fundo escuro com transparencia
        var prev = GUI.color;
        GUI.color = new Color(0.05f, 0.05f, 0.08f, 0.75f);
        GUI.DrawTexture(new Rect(x, y, size, size), PixelTexture);
        GUI.color = prev;
    }

    private static void DrawRing(float cx, float cy, float radius, Color color)
    {
        // Simular circulo com linhas
        int segments = 24;
        for (int i = 0; i < segments; i++)
        {
            float a1 = (float)i / segments * Mathf.PI * 2f;
            float a2 = (float)(i + 1) / segments * Mathf.PI * 2f;
            Primitives.DrawLine(
                new Vector2(cx + Mathf.Cos(a1) * radius, cy + Mathf.Sin(a1) * radius),
                new Vector2(cx + Mathf.Cos(a2) * radius, cy + Mathf.Sin(a2) * radius),
                color);
        }
    }

    private static void DrawLine(float x1, float y1, float x2, float y2, Color color)
    {
        Primitives.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color);
    }

    private static void DrawLegendItem(float x, float y, Color color, string text)
    {
        DrawDot(x + 4, y + 5, 3f, color);
        Primitives.DrawString(new Vector2(x + 12, y), text, color, 9, false);
    }
}
