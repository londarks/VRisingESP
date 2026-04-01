using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExtrasensoryPerception.ESP;

public static class RenderQueue
{
    private static Vector2 GetPosition(Type type, Vector2 position)
    {
        if (type == typeof(Primitives.Inverted)) position.y = Screen.height - position.y;
        return position;
    }
    
    private readonly struct QString(Type drawMode, Vector2 position, string text, Color color, int fontSize = 12, bool centered = true)
    {
        public void Draw() => Primitives.DrawString(GetPosition(drawMode, position), text, color, fontSize, centered);
    }
    
    private readonly struct QBox(Type drawMode, Vector2 position, Vector2 size, Color color)
    {
        public void Draw() => Primitives.DrawBox(GetPosition(drawMode, position), size, color);
    }

    private readonly struct QLine(Vector2 from, Vector2 to, Color color)
    {
        public void Draw() => Primitives.DrawLine(from, to, color);
    }

    private readonly struct QCorners(Vector2 position, Vector2 size, Color color)
    {
        public void Draw() => Primitives.DrawCorners(position, size, color);
    }

    private readonly struct QFilledRect(Vector2 position, Vector2 size, Color color)
    {
        public void Draw() => Primitives.DrawFilledRect(position, size, color);
    }

    private readonly struct QHealthBar(Vector2 position, float width, float height, float percent, Color color)
    {
        public void Draw() => Primitives.DrawHealthBar(position, width, height, percent, color);
    }

    private static readonly List<QString> StringQueue = [];
    private static readonly List<QBox> BoxQueue = [];
    private static readonly List<QLine> LineQueue = [];
    private static readonly List<QCorners> CornersQueue = [];
    private static readonly List<QFilledRect> FilledRectQueue = [];
    private static readonly List<QHealthBar> HealthBarQueue = [];
    private static readonly object Lock = new();
    
    // Strings
    public static void String<TMode>(Vector2 position, string text, Color color, int fontSize = 12, bool centered = true) where TMode : Primitives.IDrawMode, new()
    {
        lock (Lock)
        {
            StringQueue.Add(new QString(typeof(TMode), position, text, color, fontSize, centered));
        }
    }
    
    public static void String(Vector2 position, string text, Color color, int fontSize = 12, bool centered = true)
    {
        String<Primitives.Normal>(position, text, color, fontSize, centered);
    }
    
    
    // Boxes
    public static void Box<TMode>(Vector2 position, Vector2 size, Color color) where TMode : Primitives.IDrawMode, new()
    {
        lock (Lock)
        {
            BoxQueue.Add(new QBox(typeof(TMode), position, size, color));
        }
    }
    
    public static void Box(Vector2 position, Vector2 size, Color color)
    {
        Box<Primitives.Normal>(position, size, color);
    }

    // Lines
    public static void Line(Vector2 from, Vector2 to, Color color)
    {
        lock (Lock) { LineQueue.Add(new QLine(from, to, color)); }
    }

    // Corners
    public static void Corners(Vector2 position, Vector2 size, Color color)
    {
        lock (Lock) { CornersQueue.Add(new QCorners(position, size, color)); }
    }

    // Filled rect
    public static void FilledRect(Vector2 position, Vector2 size, Color color)
    {
        lock (Lock) { FilledRectQueue.Add(new QFilledRect(position, size, color)); }
    }

    // Health bar
    public static void HealthBar(Vector2 position, float width, float height, float percent, Color color)
    {
        lock (Lock) { HealthBarQueue.Add(new QHealthBar(position, width, height, percent, color)); }
    }

    public static void Clear()
    {
        lock (Lock)
        {
            StringQueue.Clear();
            BoxQueue.Clear();
            LineQueue.Clear();
            CornersQueue.Clear();
            FilledRectQueue.Clear();
            HealthBarQueue.Clear();
        }
    }

    public static void DrawQueued()
    {
        lock (Lock)
        {
            foreach (var item in FilledRectQueue) item.Draw();
            foreach (var item in HealthBarQueue) item.Draw();
            foreach (var item in BoxQueue) item.Draw();
            foreach (var item in CornersQueue) item.Draw();
            foreach (var item in LineQueue) item.Draw();
            foreach (var item in StringQueue) item.Draw();
        }
    }
}