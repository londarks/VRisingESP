using System.Collections.Generic;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

public static class Primitives
{
    public interface IDrawMode { }
    public class Normal : IDrawMode { }
    public class Inverted : IDrawMode { }

    private static readonly Dictionary<Color, Texture2D> HollowTextures = new();

    public static void DrawString<T>(Vector2 position, string label, Color color, int fontsize = 12,
        bool centered = true) where T : IDrawMode, new()
    {
        if (typeof(T) == typeof(Inverted)) position.y = Screen.height - position.y;

        var stringStyle = new GUIStyle
        {
            fontSize = fontsize,
            richText = true,
            normal =
            {
                textColor = color
            },
            fontStyle = FontStyle.Bold
        };

        var guiContent = new GUIContent(label);
        var size = stringStyle.CalcSize(guiContent);
        GUI.Label(new Rect(centered ? position - size / 2f : position, size), guiContent, stringStyle);
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