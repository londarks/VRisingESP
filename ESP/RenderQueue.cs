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
    
    private static readonly List<QString> StringQueue = [];
    private static readonly List<QBox> BoxQueue = [];
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
    
    public static void Clear()
    {
        lock (Lock)
        {
            StringQueue.Clear();
            BoxQueue.Clear();
        }
    }
    
    public static void DrawQueued()
    {
        lock (Lock)
        {
            foreach (var item in StringQueue) item.Draw();
            foreach (var item in BoxQueue) item.Draw();
        }
    }
}