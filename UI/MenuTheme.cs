using System.Collections.Generic;
using UnityEngine;

namespace ExtrasensoryPerception.UI;

public static class MenuTheme
{
    private static readonly Dictionary<Color, Texture2D> Textures = new();

    internal static GUIStyle WindowStyle = new();
    internal static GUIStyle ButtonStyle = new();
    internal static GUIStyle ToggleStyle = new();
    internal static GUIStyle BoxStyle = new();
    internal static GUIStyle HSliderStyle = new();
    internal static GUIStyle HSliderThumbStyle = new();
    internal static GUIStyle TooltipStyle = new();

    internal static void SetupDarkTheme()
    {
        // Color palette
        var darkBg = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        var mediumBg = new Color(0.25f, 0.25f, 0.25f, 1f);
        var hoverBg = new Color(0.45f, 0.45f, 0.45f, 1f);
        var activeBg = new Color(0.2f, 0.2f, 0.2f, 1f);
        var focusedBg = new Color(0.3f, 0.4f, 0.6f, 1f);
        var selectedBg = new Color(0.2f, 0.5f, 0.8f, 1f);
        var whiteText = Color.white;

        // === WINDOW ===
        WindowStyle = GUI.skin.window;
        WindowStyle.normal.background = MakeTexture(darkBg);
        WindowStyle.onNormal.background = MakeTexture(darkBg);
        WindowStyle.hover.background = MakeTexture(darkBg);
        WindowStyle.onHover.background = MakeTexture(darkBg);
        WindowStyle.active.background = MakeTexture(darkBg);
        WindowStyle.onActive.background = MakeTexture(darkBg);
        WindowStyle.focused.background = MakeTexture(darkBg);
        WindowStyle.onFocused.background = MakeTexture(darkBg);

        WindowStyle.normal.textColor = GUI.skin.label.normal.textColor;
        WindowStyle.active.textColor = whiteText;
        WindowStyle.hover.textColor = whiteText;
        WindowStyle.onHover.textColor = whiteText;
        WindowStyle.focused.textColor = whiteText;


        // === BUTTON ===
        ButtonStyle = GUI.skin.button;
        ButtonStyle.normal.background = MakeTexture(mediumBg);
        ButtonStyle.hover.background = MakeTexture(hoverBg);
        ButtonStyle.active.background = MakeTexture(activeBg);
        ButtonStyle.focused.background = MakeTexture(focusedBg);
        ButtonStyle.onNormal.background = MakeTexture(selectedBg);
        ButtonStyle.onHover.background = MakeTexture(new Color(0.3f, 0.6f, 0.9f, 1f));
        ButtonStyle.onActive.background = MakeTexture(new Color(0.1f, 0.4f, 0.7f, 1f));
        ButtonStyle.onFocused.background = MakeTexture(selectedBg);

        ButtonStyle.hover.textColor = whiteText;
        ButtonStyle.onHover.textColor = whiteText;

        // === TOGGLE ===
        ToggleStyle = GUI.skin.toggle;
        ToggleStyle.normal.background = MakeTexture(mediumBg);
        ToggleStyle.hover.background = MakeTexture(hoverBg);
        ToggleStyle.active.background = MakeTexture(activeBg);
        ToggleStyle.focused.background = MakeTexture(focusedBg);
        ToggleStyle.onNormal.background = MakeTexture(selectedBg);
        ToggleStyle.onHover.background = MakeTexture(new Color(0.3f, 0.6f, 0.9f, 1f));
        ToggleStyle.onActive.background = MakeTexture(new Color(0.1f, 0.4f, 0.7f, 1f));
        ToggleStyle.onFocused.background = MakeTexture(selectedBg);

        ToggleStyle.hover.textColor = whiteText;
        ToggleStyle.onHover.textColor = whiteText;

        ToggleStyle.alignment = TextAnchor.MiddleCenter;
        ToggleStyle.contentOffset = new Vector2(-8.5f, 0);

        // === BOX ===
        BoxStyle = GUI.skin.box;
        BoxStyle.normal.background = MakeTexture(new Color(0.2f, 0.2f, 0.2f, 0.8f));
        BoxStyle.hover.background = MakeTexture(new Color(0.25f, 0.25f, 0.25f, 0.8f));
        BoxStyle.active.background = MakeTexture(new Color(0.15f, 0.15f, 0.15f, 0.8f));
        BoxStyle.focused.background = MakeTexture(new Color(0.2f, 0.3f, 0.5f, 0.8f));
        BoxStyle.onNormal.background = MakeTexture(new Color(0.2f, 0.2f, 0.2f, 0.8f));
        BoxStyle.onHover.background = MakeTexture(new Color(0.25f, 0.25f, 0.25f, 0.8f));
        BoxStyle.onActive.background = MakeTexture(new Color(0.15f, 0.15f, 0.15f, 0.8f));
        BoxStyle.onFocused.background = MakeTexture(new Color(0.2f, 0.3f, 0.5f, 0.8f));

        BoxStyle.hover.textColor = whiteText;
        BoxStyle.onHover.textColor = whiteText;

        // === HORIZONTAL SLIDER ===
        HSliderStyle = GUI.skin.horizontalSlider;
        HSliderStyle.normal.background = MakeTexture(new Color(0.2f, 0.2f, 0.2f, 1f));
        HSliderStyle.hover.background = MakeTexture(new Color(0.25f, 0.25f, 0.25f, 1f));
        HSliderStyle.active.background = MakeTexture(new Color(0.15f, 0.15f, 0.15f, 1f));
        HSliderStyle.focused.background = MakeTexture(focusedBg);

        // === HORIZONTAL SLIDER THUMB ===
        HSliderThumbStyle = GUI.skin.horizontalSliderThumb;
        HSliderThumbStyle.normal.background = MakeTexture(mediumBg);
        HSliderThumbStyle.hover.background = MakeTexture(hoverBg);
        HSliderThumbStyle.active.background = MakeTexture(selectedBg);
        HSliderThumbStyle.focused.background = MakeTexture(focusedBg);

        // === TOOLTIP ===
        TooltipStyle = GUI.skin.box;
        TooltipStyle.normal.background = MakeTexture(darkBg);
        TooltipStyle.normal.textColor = whiteText;
        TooltipStyle.fontSize = 12;
        TooltipStyle.wordWrap = true;
    }

    private static Texture2D MakeTexture(Color color)
    {
        if (!Textures.TryGetValue(color, out var texture) || !texture)
        {
            var pixels = new Color[1];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = color;

            texture = new Texture2D(1, 1);
            texture.SetPixels(pixels);
            texture.Apply();

            Textures[color] = texture;
        }

        return texture;
    }
}