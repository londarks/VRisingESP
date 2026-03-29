using Il2CppSystem;
using ProjectM.UI;
using Stunlock.Core;
using Stunlock.Localization;

namespace ExtrasensoryPerception.Utils.OptionsGUI.Components;

public class Slider
{
    private OptionsPanel_Interface? _panel;
    private string? _label;
    private float _minvalue;
    private float _maxvalue;
    private float _defaultvalue;
    private float _initialvalue;
    private bool _wholeNumbers;
    private Action<float>? _onValueChanged;

    public Slider Panel(OptionsPanel_Interface? panel)
    {
        _panel = panel;
        return this;
    }

    public Slider Label(string? label)
    {
        _label = label;
        return this;
    }

    public Slider MinValue(float defaultValue)
    {
        _minvalue = defaultValue;
        return this;
    }

    public Slider MaxValue(float initialValue)
    {
        _maxvalue = initialValue;
        return this;
    }

    public Slider DefaultValue(float defaultValue)
    {
        _defaultvalue = defaultValue;
        return this;
    }

    public Slider InitialValue(float initialValue)
    {
        _initialvalue = initialValue;
        return this;
    }

    public Slider WholeNumbers()
    {
        _wholeNumbers = true;
        return this;
    }

    public Slider OnValueChanged(Action<float>? callback)
    {
        _onValueChanged = callback;
        return this;
    }

    public SettingsEntry_Slider Build()
    {
        var headerGuid = AssetGuid.NewGuid();
        var headerKey = new LocalizationKey(headerGuid);

        Localization.LocalizedStrings.Add(headerGuid, _label);

        var entry = OptionsHelper.AddSlider(
            _panel?.ContentNode,
            _panel?.SliderPrefab,
            false,
            headerKey,
            new Nullable_Unboxed<LocalizationKey>(),
            _minvalue,
            _maxvalue,
            _defaultvalue,
            _initialvalue,
            1,
            _wholeNumbers,
            _onValueChanged
        );

        //_panel.EntriesSelectionGroup.Entries.Add(entry);

        return entry;
    }
}