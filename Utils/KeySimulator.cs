using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace ExtrasensoryPerception.Utils;

/// <summary>
/// Simula teclas usando SendInput com SCANCODE.
/// V Rising ignora keybd_event, precisa de SendInput com scan code.
/// </summary>
public static class KeySimulator
{
    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct INPUT_UNION
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public INPUT_UNION u;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    private const uint INPUT_KEYBOARD = 1;
    private const uint INPUT_MOUSE = 0;
    private const uint KEYEVENTF_SCANCODE = 0x0008;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint MOUSEEVENTF_XDOWN = 0x0080;
    private const uint MOUSEEVENTF_XUP = 0x0100;
    private const uint XBUTTON1 = 0x0001;
    private const uint XBUTTON2 = 0x0002;

    public static void PressKey(KeyCode key)
    {
        if (key == KeyCode.Mouse3 || key == KeyCode.Mouse4 || key == KeyCode.Mouse5)
        {
            PressMouseButton(key);
            return;
        }

        ushort scanCode = KeyCodeToScanCode(key);
        if (scanCode == 0) return;

        var inputs = new INPUT[1];
        inputs[0].type = INPUT_KEYBOARD;
        inputs[0].u.ki.wVk = 0;
        inputs[0].u.ki.wScan = scanCode;
        inputs[0].u.ki.dwFlags = KEYEVENTF_SCANCODE;
        inputs[0].u.ki.time = 0;
        inputs[0].u.ki.dwExtraInfo = IntPtr.Zero;

        SendInput(1, inputs, Marshal.SizeOf<INPUT>());
        Thread.Sleep(80);
        inputs[0].u.ki.dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP;
        SendInput(1, inputs, Marshal.SizeOf<INPUT>());
    }

    private static void PressMouseButton(KeyCode key)
    {
        // Mouse3 = XBUTTON1, Mouse4 = XBUTTON2, Mouse5 = XBUTTON2 (mesmo fisico, diferente no Unity)
        uint xButton = key == KeyCode.Mouse3 ? XBUTTON1 : XBUTTON2;
        // Mouse5 no Unity nao tem mapeamento direto no Windows, tentar XBUTTON2
        if (key == KeyCode.Mouse5) xButton = XBUTTON2;

        var inputs = new INPUT[1];
        inputs[0].type = INPUT_MOUSE;
        inputs[0].u.mi.mouseData = xButton;
        inputs[0].u.mi.dwFlags = MOUSEEVENTF_XDOWN;
        inputs[0].u.mi.dwExtraInfo = IntPtr.Zero;

        SendInput(1, inputs, Marshal.SizeOf<INPUT>());
        Thread.Sleep(80);
        inputs[0].u.mi.dwFlags = MOUSEEVENTF_XUP;
        SendInput(1, inputs, Marshal.SizeOf<INPUT>());
    }

    private static ushort KeyCodeToScanCode(KeyCode key)
    {
        return key switch
        {
            KeyCode.A => 0x1E, KeyCode.B => 0x30, KeyCode.C => 0x2E,
            KeyCode.D => 0x20, KeyCode.E => 0x12, KeyCode.F => 0x21,
            KeyCode.G => 0x22, KeyCode.H => 0x23, KeyCode.I => 0x17,
            KeyCode.J => 0x24, KeyCode.K => 0x25, KeyCode.L => 0x26,
            KeyCode.M => 0x32, KeyCode.N => 0x31, KeyCode.O => 0x18,
            KeyCode.P => 0x19, KeyCode.Q => 0x10, KeyCode.R => 0x13,
            KeyCode.S => 0x1F, KeyCode.T => 0x14, KeyCode.U => 0x16,
            KeyCode.V => 0x2F, KeyCode.W => 0x11, KeyCode.X => 0x2D,
            KeyCode.Y => 0x15, KeyCode.Z => 0x2C,
            KeyCode.Alpha1 => 0x02, KeyCode.Alpha2 => 0x03,
            KeyCode.Alpha3 => 0x04, KeyCode.Alpha4 => 0x05,
            KeyCode.Alpha5 => 0x06, KeyCode.Alpha6 => 0x07,
            KeyCode.Space => 0x39,
            KeyCode.LeftShift => 0x2A,
            KeyCode.LeftControl => 0x1D,
            KeyCode.LeftAlt => 0x38,
            KeyCode.Tab => 0x0F,
            KeyCode.Escape => 0x01,
            KeyCode.F1 => 0x3B, KeyCode.F2 => 0x3C,
            KeyCode.F3 => 0x3D, KeyCode.F4 => 0x3E,
            _ => 0
        };
    }
}
