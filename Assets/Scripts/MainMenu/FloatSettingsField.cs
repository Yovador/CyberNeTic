using UnityEngine;

public class FloatSettingsField : SettingsField
{
    private float value = 0.0f;

    private void Awake()
    {
        base.Init();

        SetSliderValue(value);
    }

    public override void OnValueChanged()
    {
        value = base.GetSliderValue();
        base.OnValueChanged();
    }

    protected override void UpdateUI()
    {
        SetSliderValue(value);
        base.UpdateUI();
    }

    public float GetValue()
    {
        return value;
    }

    public void SetValue(float newValue)
    {
        value = newValue;
        UpdateUI();
    }
}
