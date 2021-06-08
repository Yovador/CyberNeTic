using UnityEngine;

public class FloatSettingsField : SettingsField
{
    [Range(0f, 1f)]
    public float defaultValue;

    private float value = 0.0f;

    private void Start()
    {
        value = defaultValue;
        base.Init();

        SetSliderValue(defaultValue);

    }

    public override void OnValueChanged()
    {
        value = base.GetSliderValue();
        base.OnValueChanged();
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();
    }

    public float GetValue()
    {
        return value;
    }
}
