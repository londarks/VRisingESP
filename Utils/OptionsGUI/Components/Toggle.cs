using System;
using ProjectM.UI;
using Stunlock.Core;
using Stunlock.Localization;

namespace ExtrasensoryPerception.Utils.OptionsGUI.Components;

public class Toggle
{
    private OptionsPanel_Interface? _panel;
    private string? _label;
    private string? _tooltip;
    private bool _defaultValue;
    private bool _initialValue;
    private Action<bool>? _onValueChanged;

    public Toggle Panel(OptionsPanel_Interface? panel)
    {
        _panel = panel;
        return this;
    }

    public Toggle Label(string? label)
    {
        _label = label;
        return this;
    }

    public Toggle Tooltip(string? tooltip)
    {
        _tooltip = tooltip;
        return this;
    }

    public Toggle DefaultValue(bool defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public Toggle InitialValue(bool initialValue)
    {
        _initialValue = initialValue;
        return this;
    }

    public Toggle OnValueChanged(Action<bool>? callback)
    {
        _onValueChanged = callback;
        return this;
    }

    public SettingsEntry_Checkbox Build()
    {
        var headerGuid = AssetGuid.NewGuid();
        var headerKey = new LocalizationKey(headerGuid);

        var tooltipGuid = AssetGuid.NewGuid();
        var tooltipKey = new LocalizationKey(tooltipGuid);

        Localization.LocalizedStrings.Add(headerGuid, _label);

        Localization.LocalizedStrings.Add(tooltipGuid, _tooltip);

        var entry = OptionsHelper.AddCheckbox(
            _panel?.ContentNode,
            _panel?.CheckboxPrefab,
            false,
            headerKey,
            tooltipKey,
            _defaultValue,
            _initialValue,
            _onValueChanged
        );

        _panel?.EntriesSelectionGroup.Entries.Add(entry);

        return entry;
    }
}