using UnityEngine;

public class BoolSettingsField : SettingsField
{
    private bool value = false;

    private void Awake()
    {
        base.Init();
    }

    public override void OnValueChanged()
    {
        value = !value;
        base.OnValueChanged();
    }

    protected override void UpdateUI()
    {
        if (value == true)
        {
            SetSliderValue(1);
        }
        else
        {
            SetSliderValue(0);
        }

        base.UpdateUI();
    }

    public bool GetValue()
    {
        return value;
    }

    public void SetValue(bool newValue)
    {
        value = newValue;
        UpdateUI();
    }
}
