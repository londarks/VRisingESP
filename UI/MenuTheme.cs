using System.Collections.Generic;
using UnityEngine;

namespace ExtrasensoryPerception.UI;

public static class MenuTheme
{
    private static readonly Dictionary<Color, Texture2D> SolidTextures = new();

    // ── Color Palette ──────────────────────────────────────────────
    internal static readonly Color BgDark      = new(0.06f, 0.06f, 0.07f, 0.98f);
    internal static readonly Color BgSidebar   = new(0.075f, 0.075f, 0.085f, 0.99f);
    internal static readonly Color BgContent   = new(0.10f, 0.10f, 0.11f, 0.99f);
    internal static readonly Color BgElevated  = new(0.15f, 0.15f, 0.17f, 1f);
    internal static readonly Color BgHover     = new(0.20f, 0.20f, 0.23f, 1f);
    internal static readonly Color BgField     = new(0.12f, 0.12f, 0.14f, 1f);

    internal static readonly Color Accent      = new(0.85f, 0.18f, 0.18f, 1f);
    internal static readonly Color AccentHover = new(0.95f, 0.28f, 0.28f, 1f);
    internal static readonly Color AccentDim   = new(0.55f, 0.12f, 0.12f, 1f);
    internal static readonly Color AccentBg    = new(0.85f, 0.18f, 0.18f, 0.12f);

    internal static readonly Color TextPrimary   = new(0.92f, 0.92f, 0.93f, 1f);
    internal static readonly Color TextSecondary = new(0.50f, 0.50f, 0.55f, 1f);
    internal static readonly Color TextDim       = new(0.30f, 0.30f, 0.34f, 1f);

    internal static readonly Color SwitchOn  = new(0.85f, 0.18f, 0.18f, 1f);
    internal static readonly Color SwitchOff = new(0.22f, 0.22f, 0.26f, 1f);
    internal static readonly Color Sep       = new(0.14f, 0.14f, 0.16f, 1f);

    // ── Styles ─────────────────────────────────────────────────────
    internal static GUIStyle WindowStyle = new();
    internal static GUIStyle TitleStyle = new();
    internal static GUIStyle NavStyle = new();
    internal static GUIStyle NavActiveStyle = new();
    internal static GUIStyle SwitchOnStyle = new();
    internal static GUIStyle SwitchOffStyle = new();
    internal static GUIStyle SidebarBgStyle = new();
    internal static GUIStyle ContentBgStyle = new();
    internal static GUIStyle PageTitleStyle = new();
    internal static GUIStyle PageDescStyle = new();
    internal static GUIStyle HeaderStyle = new();
    internal static GUIStyle CollapsibleStyle = new();
    internal static GUIStyle SubHeaderStyle = new();
    internal static GUIStyle LabelStyle = new();
    internal static GUIStyle LabelDimStyle = new();
    internal static GUIStyle ButtonStyle = new();
    internal static GUIStyle SmallBtnStyle = new();
    internal static GUIStyle HSliderStyle = new();
    internal static GUIStyle HSliderThumbStyle = new();
    internal static GUIStyle ToggleStyle = new();
    internal static GUIStyle TooltipStyle = new();
    internal static GUIStyle CreditStyle = new();
    internal static GUIStyle SeparatorStyle = new();

    // ── Init ───────────────────────────────────────────────────────
    private static bool _initialized;

    internal static void Init()
    {
        if (_initialized) return;
        BuildStyles();
        _initialized = true;
    }

    // ── Style Builder ──────────────────────────────────────────────
    private static void BuildStyles()
    {
        // ─── WINDOW ───
        WindowStyle = GUI.skin.window;
        SetAllBg(WindowStyle, BgDark);
        WindowStyle.normal.textColor = BgDark;
        WindowStyle.fontSize = 1;
        WindowStyle.padding = Offset(0, 0, 4, 0);
        WindowStyle.contentOffset = new Vector2(0, -2);

        // ─── TITLE (brand) ───
        TitleStyle = new GUIStyle();
        TitleStyle.normal.textColor = Accent;
        TitleStyle.fontSize = 18;
        TitleStyle.fontStyle = FontStyle.Bold;
        TitleStyle.alignment = TextAnchor.MiddleCenter;
        TitleStyle.padding = Offset(0, 0, 12, 12);

        // ─── NAV (sidebar buttons) ───
        NavStyle = new GUIStyle();
        NavStyle.normal.background  = Tex(Color.clear);
        NavStyle.hover.background   = Tex(new Color(1, 1, 1, 0.04f));
        NavStyle.active.background  = Tex(new Color(1, 1, 1, 0.06f));
        NavStyle.focused.background = Tex(Color.clear);
        NavStyle.normal.textColor   = TextSecondary;
        NavStyle.hover.textColor    = TextPrimary;
        NavStyle.active.textColor   = TextPrimary;
        NavStyle.fontSize  = 12;
        NavStyle.alignment = TextAnchor.MiddleLeft;
        NavStyle.padding   = Offset(18, 10, 0, 0);
        NavStyle.margin    = Offset(0, 0, 0, 0);

        NavActiveStyle = new GUIStyle();
        NavActiveStyle.normal.background  = Tex(AccentBg);
        NavActiveStyle.hover.background   = Tex(AccentBg);
        NavActiveStyle.active.background  = Tex(AccentBg);
        NavActiveStyle.focused.background = Tex(AccentBg);
        NavActiveStyle.normal.textColor   = Accent;
        NavActiveStyle.hover.textColor    = Accent;
        NavActiveStyle.active.textColor   = AccentHover;
        NavActiveStyle.fontSize  = 12;
        NavActiveStyle.fontStyle = FontStyle.Bold;
        NavActiveStyle.alignment = TextAnchor.MiddleLeft;
        NavActiveStyle.padding   = Offset(18, 10, 0, 0);
        NavActiveStyle.margin    = Offset(0, 0, 0, 0);

        // ─── SWITCH ON/OFF ───
        SwitchOnStyle = new GUIStyle();
        SwitchOnStyle.normal.background = Tex(SwitchOn);
        SwitchOnStyle.hover.background  = Tex(AccentHover);
        SwitchOnStyle.active.background = Tex(AccentDim);
        SwitchOnStyle.normal.textColor  = TextPrimary;
        SwitchOnStyle.hover.textColor   = TextPrimary;
        SwitchOnStyle.active.textColor  = TextPrimary;
        SwitchOnStyle.fontSize  = 10;
        SwitchOnStyle.fontStyle = FontStyle.Bold;
        SwitchOnStyle.alignment = TextAnchor.MiddleCenter;
        SwitchOnStyle.padding   = Offset(4, 4, 2, 2);

        SwitchOffStyle = new GUIStyle();
        SwitchOffStyle.normal.background = Tex(SwitchOff);
        SwitchOffStyle.hover.background  = Tex(BgHover);
        SwitchOffStyle.active.background = Tex(BgElevated);
        SwitchOffStyle.normal.textColor  = TextSecondary;
        SwitchOffStyle.hover.textColor   = TextPrimary;
        SwitchOffStyle.active.textColor  = TextPrimary;
        SwitchOffStyle.fontSize  = 10;
        SwitchOffStyle.fontStyle = FontStyle.Bold;
        SwitchOffStyle.alignment = TextAnchor.MiddleCenter;
        SwitchOffStyle.padding   = Offset(4, 4, 2, 2);

        // ─── SIDEBAR BG ───
        SidebarBgStyle = new GUIStyle();
        SidebarBgStyle.normal.background = Tex(BgSidebar);
        SidebarBgStyle.padding = Offset(0, 0, 0, 0);

        // ─── CONTENT BG ───
        ContentBgStyle = new GUIStyle();
        ContentBgStyle.normal.background = Tex(BgContent);
        ContentBgStyle.padding = Offset(20, 20, 14, 14);

        // ─── PAGE TITLE (content header) ───
        PageTitleStyle = new GUIStyle();
        PageTitleStyle.normal.textColor = TextPrimary;
        PageTitleStyle.fontSize  = 16;
        PageTitleStyle.fontStyle = FontStyle.Bold;
        PageTitleStyle.alignment = TextAnchor.MiddleLeft;
        PageTitleStyle.padding   = Offset(0, 0, 0, 2);

        // ─── PAGE DESCRIPTION ───
        PageDescStyle = new GUIStyle();
        PageDescStyle.normal.textColor = TextSecondary;
        PageDescStyle.fontSize  = 10;
        PageDescStyle.alignment = TextAnchor.MiddleLeft;
        PageDescStyle.padding   = Offset(0, 0, 0, 6);

        // ─── HEADER (section) ───
        HeaderStyle = new GUIStyle();
        HeaderStyle.normal.textColor = TextPrimary;
        HeaderStyle.fontSize  = 14;
        HeaderStyle.fontStyle = FontStyle.Bold;
        HeaderStyle.alignment = TextAnchor.MiddleLeft;
        HeaderStyle.padding   = Offset(0, 0, 6, 3);

        // ─── COLLAPSIBLE HEADER ───
        CollapsibleStyle = new GUIStyle();
        CollapsibleStyle.normal.background  = Tex(Color.clear);
        CollapsibleStyle.hover.background   = Tex(new Color(1, 1, 1, 0.03f));
        CollapsibleStyle.active.background  = Tex(new Color(1, 1, 1, 0.05f));
        CollapsibleStyle.focused.background = Tex(Color.clear);
        CollapsibleStyle.normal.textColor   = TextPrimary;
        CollapsibleStyle.hover.textColor    = Accent;
        CollapsibleStyle.active.textColor   = Accent;
        CollapsibleStyle.fontSize  = 14;
        CollapsibleStyle.fontStyle = FontStyle.Bold;
        CollapsibleStyle.alignment = TextAnchor.MiddleLeft;
        CollapsibleStyle.padding   = Offset(0, 0, 4, 4);
        CollapsibleStyle.margin    = Offset(0, 0, 0, 0);

        // ─── SUB HEADER ───
        SubHeaderStyle = new GUIStyle();
        SubHeaderStyle.normal.textColor = TextSecondary;
        SubHeaderStyle.fontSize  = 11;
        SubHeaderStyle.fontStyle = FontStyle.Bold;
        SubHeaderStyle.alignment = TextAnchor.UpperLeft;
        SubHeaderStyle.padding   = Offset(2, 0, 6, 3);

        // ─── LABEL ───
        LabelStyle = new GUIStyle();
        LabelStyle.normal.textColor = TextPrimary;
        LabelStyle.fontSize  = 13;
        LabelStyle.alignment = TextAnchor.MiddleLeft;

        LabelDimStyle = new GUIStyle();
        LabelDimStyle.normal.textColor = TextSecondary;
        LabelDimStyle.fontSize  = 12;
        LabelDimStyle.alignment = TextAnchor.MiddleLeft;

        // ─── BUTTON ───
        ButtonStyle = GUI.skin.button;
        ButtonStyle.normal.background   = Tex(BgElevated);
        ButtonStyle.hover.background    = Tex(BgHover);
        ButtonStyle.active.background   = Tex(AccentDim);
        ButtonStyle.focused.background  = Tex(BgElevated);
        ButtonStyle.onNormal.background = Tex(Accent);
        ButtonStyle.onHover.background  = Tex(AccentHover);
        ButtonStyle.onActive.background = Tex(AccentDim);
        ButtonStyle.normal.textColor    = TextPrimary;
        ButtonStyle.hover.textColor     = TextPrimary;
        ButtonStyle.active.textColor    = TextPrimary;
        ButtonStyle.onNormal.textColor  = TextPrimary;
        ButtonStyle.onHover.textColor   = TextPrimary;
        ButtonStyle.fontSize = 11;
        ButtonStyle.padding  = Offset(10, 10, 4, 4);
        ButtonStyle.margin   = Offset(2, 2, 1, 1);

        SmallBtnStyle = new GUIStyle();
        SmallBtnStyle.normal.background  = Tex(BgElevated);
        SmallBtnStyle.hover.background   = Tex(BgHover);
        SmallBtnStyle.active.background  = Tex(AccentDim);
        SmallBtnStyle.focused.background = Tex(BgElevated);
        SmallBtnStyle.normal.textColor   = TextPrimary;
        SmallBtnStyle.hover.textColor    = TextPrimary;
        SmallBtnStyle.active.textColor   = TextPrimary;
        SmallBtnStyle.fontSize  = 11;
        SmallBtnStyle.alignment = TextAnchor.MiddleCenter;
        SmallBtnStyle.padding   = Offset(10, 10, 4, 4);

        // ─── TOGGLE ───
        ToggleStyle = GUI.skin.toggle;
        ToggleStyle.normal.background   = Tex(BgElevated);
        ToggleStyle.hover.background    = Tex(BgHover);
        ToggleStyle.active.background   = Tex(BgElevated);
        ToggleStyle.onNormal.background = Tex(Accent);
        ToggleStyle.onHover.background  = Tex(AccentHover);
        ToggleStyle.onActive.background = Tex(AccentDim);
        ToggleStyle.normal.textColor    = TextPrimary;
        ToggleStyle.hover.textColor     = TextPrimary;
        ToggleStyle.onNormal.textColor  = TextPrimary;
        ToggleStyle.onHover.textColor   = TextPrimary;
        ToggleStyle.fontSize  = 11;
        ToggleStyle.alignment = TextAnchor.MiddleCenter;

        // ─── SLIDER ───
        HSliderStyle = GUI.skin.horizontalSlider;
        HSliderStyle.normal.background = Tex(BgField);
        HSliderStyle.hover.background  = Tex(BgHover);
        HSliderStyle.fixedHeight = 6f;

        HSliderThumbStyle = GUI.skin.horizontalSliderThumb;
        HSliderThumbStyle.normal.background = Tex(Accent);
        HSliderThumbStyle.hover.background  = Tex(AccentHover);
        HSliderThumbStyle.active.background = Tex(AccentDim);
        HSliderThumbStyle.fixedWidth  = 16f;
        HSliderThumbStyle.fixedHeight = 16f;

        // ─── TOOLTIP ───
        TooltipStyle = new GUIStyle();
        TooltipStyle.normal.background = Tex(new Color(0.04f, 0.04f, 0.05f, 0.97f));
        TooltipStyle.normal.textColor  = TextPrimary;
        TooltipStyle.fontSize = 11;
        TooltipStyle.wordWrap = true;
        TooltipStyle.padding  = Offset(8, 8, 5, 5);

        // ─── CREDIT ───
        CreditStyle = new GUIStyle();
        CreditStyle.normal.textColor = TextDim;
        CreditStyle.fontSize  = 9;
        CreditStyle.alignment = TextAnchor.MiddleCenter;

        // ─── SEPARATOR ───
        SeparatorStyle = new GUIStyle();
        SeparatorStyle.normal.background = Tex(Sep);
        SeparatorStyle.margin = Offset(0, 0, 6, 6);
    }

    // ── Helpers ─────────────────────────────────────────────────────

    private static RectOffset Offset(int l, int r, int t, int b)
    {
        var o = new RectOffset();
        o.left = l; o.right = r; o.top = t; o.bottom = b;
        return o;
    }

    internal static Texture2D Tex(Color c)
    {
        if (!SolidTextures.TryGetValue(c, out var t) || !t)
        {
            t = new Texture2D(1, 1);
            t.SetPixels(new[] { c });
            t.Apply();
            SolidTextures[c] = t;
        }
        return t;
    }

    private static void SetAllBg(GUIStyle s, Color c)
    {
        var t = Tex(c);
        s.normal.background    = t; s.onNormal.background  = t;
        s.hover.background     = t; s.onHover.background   = t;
        s.active.background    = t; s.onActive.background  = t;
        s.focused.background   = t; s.onFocused.background = t;
    }
}
