using System.Collections.Generic;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

public static class Primitives
{
    public interface IDrawMode { }
    public class Normal : IDrawMode { }
    public class Inverted : IDrawMode { }

    private static readonly Dictionary<Color, Texture2D> HollowTextures = new();

    // Cached GUIStyle + GUIContent to avoid per-frame allocation in DrawString
    private static readonly GUIStyle CachedStringStyle = new();
    private static readonly GUIContent CachedContent = new();

    public static void DrawString<T>(Vector2 position, string label, Color color, int fontsize = 12,
        bool centered = true) where T : IDrawMode, new()
    {
        if (typeof(T) == typeof(Inverted)) position.y = Screen.height - position.y;

        CachedStringStyle.fontSize = fontsize;
        CachedStringStyle.richText = true;
        CachedStringStyle.normal.textColor = color;
        CachedStringStyle.fontStyle = FontStyle.Bold;

        CachedContent.text = label;
        var size = CachedStringStyle.CalcSize(CachedContent);
        GUI.Label(new Rect(centered ? position - size / 2f : position, size), CachedContent, CachedStringStyle);
    }

    public static void DrawString(Vector2 position, string label, Color color, int fontsize = 12, bool centered = true)
    {
        DrawString<Normal>(position, label, color, fontsize, centered);
    }

    public static void DrawBox<T>(Vector2 position, Vector2 size, Color color) where T : IDrawMode, new()
    {
        if (typeof(T) == typeof(Inverted)) position.y = Screen.height - position.y;

        if (!HollowTextures.TryGetValue(color, out var texture2D) || !texture2D)
        {
            texture2D = CreateHollowTexture((int)size.x, (int)size.y, color);
            HollowTextures[color] = texture2D;
        }

        GUI.DrawTexture(new Rect(position, size), texture2D);
    }

    public static void DrawBox(Vector2 position, Vector2 size, Color color)
    {
        DrawBox<Normal>(position, size, color);
    }

    public static void DrawX(Vector2 position, float size, Color color)
    {
        var halfSize = size / 2f;
        DrawLine(new Vector2(position.x - halfSize, position.y - halfSize), new Vector2(position.x + halfSize, position.y + halfSize), color);
        DrawLine(new Vector2(position.x - halfSize, position.y + halfSize), new Vector2(position.x + halfSize, position.y - halfSize), color);
    }

    public static void DrawLine(Vector2 from, Vector2 to, Color color)
    {
        var prevColor = GUI.color;
        GUI.color = color;
        var angle = Vector2.SignedAngle(to - from, Vector2.right);
        GUIUtility.RotateAroundPivot(angle, from);
        GUI.DrawTexture(new Rect(from.x, from.y, Vector2.Distance(from, to), 2f), Texture2D.whiteTexture);
        GUIUtility.RotateAroundPivot(-angle, from);
        GUI.color = prevColor;
    }

    public static void DrawCorners(Vector2 position, Vector2 size, Color color)
    {
        float cl = Mathf.Min(size.x, size.y) * 0.28f;
        float x = position.x - size.x / 2f, y = position.y, w = size.x, h = size.y;
        // Top-left
        DrawLine(new Vector2(x, y), new Vector2(x + cl, y), color);
        DrawLine(new Vector2(x, y), new Vector2(x, y + cl), color);
        // Top-right
        DrawLine(new Vector2(x + w, y), new Vector2(x + w - cl, y), color);
        DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + cl), color);
        // Bottom-left
        DrawLine(new Vector2(x, y + h), new Vector2(x + cl, y + h), color);
        DrawLine(new Vector2(x, y + h), new Vector2(x, y + h - cl), color);
        // Bottom-right
        DrawLine(new Vector2(x + w, y + h), new Vector2(x + w - cl, y + h), color);
        DrawLine(new Vector2(x + w, y + h), new Vector2(x + w, y + h - cl), color);
    }

    public static void DrawFilledRect(Vector2 position, Vector2 size, Color color)
    {
        var prev = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(new Rect(position.x, position.y, size.x, size.y), Texture2D.whiteTexture);
        GUI.color = prev;
    }

    public static void DrawHealthBar(Vector2 position, float width, float height, float percent, Color color)
    {
        DrawFilledRect(position, new Vector2(width, height), new Color(0f, 0f, 0f, 0.55f));
        DrawFilledRect(position, new Vector2(width * Mathf.Clamp01(percent), height), color);
    }

    private static Texture2D CreateHollowTexture(int width, int height, Color borderColor)
    {
        var texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Color[] array = texture2D.GetPixels();
        for (var i = 0; i < array.Length; i++) array[i] = borderColor;

        for (var j = 1; j < height - 1; j++)
            for (var k = 1; k < width - 1; k++)
                array[j * width + k] = Color.clear;

        texture2D.SetPixels(array);
        texture2D.Apply();
        return texture2D;
    }
}