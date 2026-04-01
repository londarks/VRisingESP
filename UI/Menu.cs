using System;
using BepInEx.Configuration;
using ExtrasensoryPerception.ESP;
using ExtrasensoryPerception.Utils;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace ExtrasensoryPerception.UI;

internal class Menu : MonoBehaviour
{
    private enum Tab { ESP, Combat, Radar, Extras }

    private static Rect _windowRect = new Rect(80f, 50f, 620f, 0f);
    private bool _showMenu;
    private Tab _currentTab = Tab.ESP;

    private bool _aimbotOpen = true;
    private bool _parryOpen  = true;
    private bool _assistOpen = true;

    private readonly string[] _boxTypes    = { "Cheias", "Cantos" };
    private readonly string[] _aimbotModes = { "Segurar", "Alternar" };

    private static readonly string[] TabLabels = { "ESP", "Combate", "Radar", "Extras" };
    private static readonly string[] TabDescs =
    {
        "Controle a visibilidade de entidades e overlays.",
        "Configure aimbot, auto-parry e smart assist.",
        "Mini-mapa radar com alcance customizavel.",
        "Auto-fish, auto-loot, fog e camera."
    };

    // ── Lifecycle ──────────────────────────────────────────────────

    private void OnGUI()
    {
        if (!_showMenu) return;
        MenuTheme.Init();
        _windowRect = GUILayout.Window(1, _windowRect, (GUI.WindowFunction)DrawWindow, "", MenuTheme.WindowStyle);
    }

    private void Update()
    {
        if (Input.GetKeyDown((KeyCode)277))
            _showMenu = !_showMenu;
    }

    // ── Main Window ────────────────────────────────────────────────

    private void DrawWindow(int id)
    {
        GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);

        // ── Sidebar + Content ──
        GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);

        // ════ SIDEBAR ════
        GUILayout.BeginVertical(MenuTheme.SidebarBgStyle,
            new GUILayoutOption[] { GUILayout.Width(140f), GUILayout.ExpandHeight(true) });

        // Brand
        GUILayout.Label("B A B E L", MenuTheme.TitleStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
        HLine();
        GUILayout.Space(4f);

        // Nav
        NavBtn("ESP",      Tab.ESP);
        NavBtn("Combate",  Tab.Combat);
        NavBtn("Radar",    Tab.Radar);
        NavBtn("Extras",   Tab.Extras);

        // Spacer to push credit down
        GUILayout.Label("", new GUILayoutOption[] { GUILayout.ExpandHeight(true) });

        // Master toggle at bottom of sidebar
        GUILayout.Space(4f);
        HLine();
        GUILayout.Space(4f);
        Row(() =>
        {
            GUILayout.Label("Ativo", MenuTheme.LabelStyle,
                new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            MasterSwitch();
        });
        GUILayout.Space(6f);
        GUILayout.Label("babel \u00b7 londarks", MenuTheme.CreditStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
        GUILayout.Space(6f);

        GUILayout.EndVertical();

        // ════ CONTENT ════
        GUILayout.BeginVertical(MenuTheme.ContentBgStyle, (Il2CppReferenceArray<GUILayoutOption>)null);

        // Page title + description
        GUILayout.Label(TabLabels[(int)_currentTab], MenuTheme.PageTitleStyle,
            (Il2CppReferenceArray<GUILayoutOption>)null);
        GUILayout.Label(TabDescs[(int)_currentTab], MenuTheme.PageDescStyle,
            (Il2CppReferenceArray<GUILayoutOption>)null);
        HLine();
        GUILayout.Space(4f);

        switch (_currentTab)
        {
            case Tab.ESP:    DrawESPTab();    break;
            case Tab.Combat: DrawCombatTab(); break;
            case Tab.Radar:  DrawRadarTab();  break;
            case Tab.Extras: DrawExtrasTab(); break;
        }

        GUILayout.Space(6f);
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        ShowTooltip();
        GUI.DragWindow(new Rect(0f, 0f, _windowRect.width, 50f));
    }

    // ── Master Switch ──────────────────────────────────────────────

    private static void MasterSwitch()
    {
        var style = Config.ModToggle.Value ? MenuTheme.SwitchOnStyle : MenuTheme.SwitchOffStyle;
        if (GUILayout.Button(Config.ModToggle.Value ? "ON" : "OFF", style,
                new GUILayoutOption[] { GUILayout.Width(44f), GUILayout.Height(24f) }))
            Config.ModToggle.Value = !Config.ModToggle.Value;
    }

    // ── Sidebar Navigation ─────────────────────────────────────────

    private void NavBtn(string label, Tab tab)
    {
        var style = tab == _currentTab ? MenuTheme.NavActiveStyle : MenuTheme.NavStyle;
        if (GUILayout.Button(label, style,
                new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(36f) }))
            _currentTab = tab;
    }

    // ── ESP Tab ────────────────────────────────────────────────────

    private void DrawESPTab()
    {
        Header("Exibicao");
        GUILayout.Space(4f);
        Row(() =>
        {
            SwitchRow("Caixas", Config.ESP.Boxes);
            OptionBtn(_boxTypes[Config.ESP.Boxes.Option], Config.ESP.Boxes, _boxTypes.Length);
        });
        Row(() =>
        {
            GUILayout.Label(new GUIContent("Contornos", (Texture)null, "Pode causar queda de FPS"),
                MenuTheme.LabelStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            OptionBtn("Qual: {}", Config.ESP.Outlines, 4);
            SwitchBtn(Config.ESP.Outlines);
        });

        GUILayout.Space(8f);
        Header("Entidades");
        GUILayout.Space(4f);
        ESPRow("Jogadores",  Config.ESP.Players);
        ESPRow("VBlood",     Config.ESP.VBloodCarriers);
        ESPRow("Sangue",     Config.ESP.BloodSources);
        ESPRow("Gate Boss",  Config.ESP.GateBosses);
        ESPRow("Itens",      Config.ESP.Items);
        ESPRow("Containers", Config.ESP.Containers);
        ESPRow("Minerios",   Config.ESP.Ores);
        ESPRow("Plantas",    Config.ESP.Plants);
        ESPRow("Pesca",      Config.ESP.FishingSpots);
        ESPRow("Cavalos",    Config.ESP.Horses);
        ESPRow("Servos",     Config.ESP.Servants);
        ESPRow("Carruagens", Config.ESP.Carriages);
    }

    // ── Combat Tab ─────────────────────────────────────────────────

    private void DrawCombatTab()
    {
        _aimbotOpen = CollapsibleSection("Aimbot", _aimbotOpen, Config.Aimbot.Status);
        if (_aimbotOpen)
        {
            GUILayout.Space(6f);
            Row(() =>
            {
                GUILayout.Label("Modo", MenuTheme.LabelDimStyle,
                    new GUILayoutOption[] { GUILayout.Width(60f) });
                OptionBtn(_aimbotModes[Config.Aimbot.Mode.Value], Config.Aimbot.Mode, _aimbotModes.Length);
            });

            GUILayout.Space(6f);
            SubHeader("ALVOS");
            GUILayout.Space(2f);
            Row(() =>
            {
                Toggle(Config.Aimbot.Players, "Players");
                Toggle(Config.Aimbot.Bosses,  "Bosses");
                Toggle(Config.Aimbot.Mobs,    "Mobs");
            });
            SwitchRow("Mostrar Mira", Config.Aimbot.DrawAimPosition);

            GUILayout.Space(6f);
            SubHeader("LIMITES");
            GUILayout.Space(2f);
            Slider("Distancia: {}m",  Config.Aimbot.MaxDistance,       1f, 50f);
            Slider("Cursor: {}",      Config.Aimbot.MaxCursorDistance, 0f, 1000f);
            Slider("CD Troca: {}s",   Config.Aimbot.SwitchCooldown,   0f, 1f, "F1");

            GUILayout.Space(6f);
            SubHeader("PESOS");
            GUILayout.Space(2f);
            Slider("Distancia: {}",   Config.Aimbot.DistanceWeight,       0f, 1f, "F2");
            Slider("Cursor: {}",      Config.Aimbot.CursorDistanceWeight, 0f, 1f, "F2");
            Slider("Vida: {}",        Config.Aimbot.HealthWeight,         0f, 1f, "F2");
            Slider("Tipo: {}",        Config.Aimbot.EntityTypeWeight,     0f, 1f, "F2");
        }

        GUILayout.Space(6f);
        HLine();
        GUILayout.Space(2f);

        _parryOpen = CollapsibleSection("Auto-Parry", _parryOpen, Config.AutoParry.Status);
        if (_parryOpen)
        {
            GUILayout.Space(6f);
            SubHeader("ALVOS");
            GUILayout.Space(2f);
            Row(() =>
            {
                Toggle(Config.AutoParry.Players, "Players");
                Toggle(Config.AutoParry.Bosses,  "Bosses");
                Toggle(Config.AutoParry.Mobs,    "Mobs");
            });

            GUILayout.Space(6f);
            SubHeader("AJUSTES");
            GUILayout.Space(2f);
            Slider("Range: {}m", Config.AutoParry.Range,    1f,    20f);
            Slider("CD: {}s",    Config.AutoParry.Cooldown, 0.05f, 1f, "F2");
        }

        GUILayout.Space(6f);
        HLine();
        GUILayout.Space(2f);

        _assistOpen = SimpleCollapsible("Smart Assist", _assistOpen);
        if (_assistOpen)
        {
            GUILayout.Space(6f);
            SwitchRow("Aim-on-Cast",     Config.SmartAssist.Status);
            SwitchRow("Quick-Cast Swap", Config.SmartAssist.QuickCast);
        }
    }

    // ── Radar Tab ──────────────────────────────────────────────────

    private void DrawRadarTab()
    {
        Row(() =>
        {
            GUILayout.Label("Radar", MenuTheme.HeaderStyle,
                new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            SwitchBtn(Config.Radar.Status);
        });
        GUILayout.Space(8f);
        Slider("Tamanho: {}",  Config.Radar.Size,  100f, 400f);
        Slider("Range: {}m",   Config.Radar.Range,  30f, 200f);
    }

    // ── Extras Tab ─────────────────────────────────────────────────

    private void DrawExtrasTab()
    {
        Header("Automatizacao");
        GUILayout.Space(6f);
        SwitchRow("Auto-Fish", Config.Extras.AutoFishing);
        GUILayout.Space(2f);
        SwitchRow("Auto-Loot", Config.Extras.AutoLoot);

        GUILayout.Space(8f);
        HLine();
        GUILayout.Space(2f);

        Header("Visual");
        GUILayout.Space(6f);
        SwitchRow("No Fog", Config.Extras.NoFog);

        GUILayout.Space(8f);
        HLine();
        GUILayout.Space(2f);

        Header("Sistema");
        GUILayout.Space(6f);
        Row(() =>
        {
            GUILayout.Label("Camera", MenuTheme.LabelStyle,
                new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            if (Btn("Reset", GUILayout.Width(70f)))
                Logic.MainCamera = Camera.main ?? throw new InvalidOperationException();
        });
    }

    // ═══════════════════════════════════════════════════════════════
    //  UI Components
    // ═══════════════════════════════════════════════════════════════

    private static void Row(Action content)
    {
        GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
        content();
        GUILayout.EndHorizontal();
    }

    private static void HLine()
    {
        GUILayout.Box("", MenuTheme.SeparatorStyle,
            new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1f) });
    }

    private static void Header(string text)
    {
        GUILayout.Label(text, MenuTheme.HeaderStyle,
            (Il2CppReferenceArray<GUILayoutOption>)null);
    }

    private static void SubHeader(string text)
    {
        GUILayout.Label(text, MenuTheme.SubHeaderStyle,
            (Il2CppReferenceArray<GUILayoutOption>)null);
    }

    // ── Collapsible ────────────────────────────────────────────────

    private static bool CollapsibleSection(string label, bool open, Config.FeatureConfig status)
    {
        GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
        var arrow = open ? "\u25BC " : "\u25B6 ";
        if (GUILayout.Button(arrow + label, MenuTheme.CollapsibleStyle,
                new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(30f) }))
            open = !open;
        SwitchBtn(status);
        GUILayout.EndHorizontal();
        return open;
    }

    private static bool SimpleCollapsible(string label, bool open)
    {
        var arrow = open ? "\u25BC " : "\u25B6 ";
        if (GUILayout.Button(arrow + label, MenuTheme.CollapsibleStyle,
                new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(30f) }))
            open = !open;
        return open;
    }

    // ── Switch Button (ON/OFF) ─────────────────────────────────────

    private static void SwitchBtn(Config.FeatureConfig config)
    {
        var style = config.Enabled ? MenuTheme.SwitchOnStyle : MenuTheme.SwitchOffStyle;
        if (GUILayout.Button(config.Enabled ? "ON" : "OFF", style,
                new GUILayoutOption[] { GUILayout.Width(44f), GUILayout.Height(24f) }))
            config.Enabled = !config.Enabled;
    }

    private static void SwitchRow(string label, Config.FeatureConfig config)
    {
        GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
        GUILayout.Label(label, MenuTheme.LabelStyle,
            new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
        SwitchBtn(config);
        GUILayout.EndHorizontal();
    }

    // ── ESP Entity Row ─────────────────────────────────────────────

    private static void ESPRow(string name, Config.FeatureConfig config)
    {
        GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
        GUILayout.Label(name, MenuTheme.LabelStyle,
            new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
        if (Btn(ColorOptions.GetColorName(config.Color), GUILayout.Width(70f)))
            config.Color = (config.Color + 1) % ColorOptions.AllColors.Count;
        SwitchBtn(config);
        GUILayout.EndHorizontal();

        if (config.MinimumQuality != 0f)
            Slider("  Min: {}%", config, 1f, 100f);
    }

    // ── Buttons ────────────────────────────────────────────────────

    private static bool Btn(string label, params GUILayoutOption[] options)
    {
        return GUILayout.Button(label, MenuTheme.SmallBtnStyle, options);
    }

    private static void OptionBtn(string label, ConfigEntry<int> config, int max, int add = 0)
    {
        if (GUILayout.Button(label.Replace("{}", config.Value.ToString()),
                MenuTheme.SmallBtnStyle,
                (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(80f) }))
            config.Value = (config.Value - add + 1) % max + add;
    }

    private static void OptionBtn(string label, Config.FeatureConfig config, int max, int add = 0)
    {
        if (GUILayout.Button(label.Replace("{}", config.Option.ToString()),
                MenuTheme.SmallBtnStyle,
                (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(80f) }))
            config.Option = (config.Option - add + 1) % max + add;
    }

    // ── Sliders ────────────────────────────────────────────────────

    private static float Slider(string label, float value, float min, float max, string fmt = "F0")
    {
        GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
        GUILayout.Label(label.Replace("{}", value.ToString(fmt)),
            MenuTheme.LabelDimStyle, new GUILayoutOption[] { GUILayout.Width(140f) });
        GUILayout.Space(8f);
        GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
        GUILayout.Space(7f);
        value = GUILayout.HorizontalSlider(value, min, max,
            MenuTheme.HSliderStyle, MenuTheme.HSliderThumbStyle,
            System.Array.Empty<GUILayoutOption>());
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        return value;
    }

    private static void Slider(string label, Config.FeatureConfig config, float min, float max, string fmt = "F0")
    {
        config.MinimumQuality = Slider(label, config.MinimumQuality, min, max, fmt);
    }

    private static void Slider(string label, ConfigEntry<float> config, float min, float max, string fmt = "F0")
    {
        config.Value = Slider(label, config.Value, min, max, fmt);
    }

    // ── Toggles ────────────────────────────────────────────────────

    private static void Toggle(Config.FeatureConfig config, string text)
    {
        config.Enabled = GUILayout.Toggle(config.Enabled, text,
            MenuTheme.ToggleStyle, System.Array.Empty<GUILayoutOption>());
    }

    // ── Tooltip ────────────────────────────────────────────────────

    private void ShowTooltip()
    {
        if (string.IsNullOrEmpty(GUI.tooltip)) return;
        var mp = Event.current.mousePosition;
        var gc = new GUIContent(GUI.tooltip);
        var sz = MenuTheme.TooltipStyle.CalcSize(gc);
        GUI.Label(new Rect(mp.x + 18f, mp.y - 4f, sz.x + 16f, sz.y + 8f), gc, MenuTheme.TooltipStyle);
    }
}
