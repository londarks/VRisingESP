using System.Runtime.InteropServices;
using UnityEngine;

namespace ExtrasensoryPerception.Utils;

// I can't believe I had to resort to this
public static class MouseSimulator
{
    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;

    public static Vector2 CursorPosition
    {
        get
        {
            GetCursorPos(out var point);
            return new Vector2(point.X, point.Y);
        }
    }

    public static void SetPos(Vector2 position)
    {
        SetCursorPos((int)position.x, (int)position.y);
    }

    /// <summary>
    /// Move the mouse by a relative delta using mouse_event.
    /// Works with V Rising's camera system which reads mouse delta, not absolute position.
    /// </summary>
    public static void MoveDelta(int dx, int dy)
    {
        mouse_event(MOUSEEVENTF_MOVE, (uint)dx, (uint)dy, 0, 0);
    }

    public static void LeftClick()
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
}