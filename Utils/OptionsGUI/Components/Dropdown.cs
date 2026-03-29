using System;
using Il2CppSystem.Collections.Generic;
using ProjectM.UI;
using Stunlock.Core;
using Stunlock.Localization;

namespace ExtrasensoryPerception.Utils.OptionsGUI.Components;

public class Dropdown
{
    private OptionsPanel_Interface? _panel;
    private string? _label;
    private string? _tooltip;
    private readonly List<string> _options = new();

    private int _defaultValue;
    private int _initialValue;
    private Action<int>? _onValueChanged;

    public Dropdown Panel(OptionsPanel_Interface? panel)
    {
        _panel = panel;
        return this;
    }

    public Dropdown Options<T>(System.Collections.Generic.List<T> options)
    {
        foreach (var option in options) _options.Add(option?.ToString() ?? string.Empty);
        return this;
    }

    public Dropdown Label(string label)
    {
        _label = label;
        return this;
    }

    public Dropdown Tooltip(string tooltip)
    {
        _tooltip = tooltip;
        return this;
    }

    public Dropdown DefaultValue(int defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public Dropdown InitialValue(int initialValue)
    {
        _initialValue = initialValue;
        return this;
    }

    public Dropdown OnValueChanged(Action<int>? callback)
    {
        _onValueChanged = callback;
        return this;
    }

    public SettingsEntry_Dropdown Build()
    {
        var headerGuid = AssetGuid.NewGuid();
        var headerKey = new LocalizationKey(headerGuid);

        var tooltipGuid = AssetGuid.NewGuid();
        var tooltipKey = new LocalizationKey(tooltipGuid);

        Localization.LocalizedStrings.Add(headerGuid, _label);

        Localization.LocalizedStrings.Add(tooltipGuid, _tooltip);

        var entry = OptionsHelper.AddDropdown(
            _panel?.ContentNode,
            _panel?.DropdownPrefab,
            false,
            headerKey,
            tooltipKey,
            _options,
            _defaultValue,
            _initialValue,
            _onValueChanged
        );

        _panel?.EntriesSelectionGroup.Entries.Add(entry);

        return entry;
    }
}