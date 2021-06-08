public class BoolSettingsField : SettingsField
{
    public bool defaultValue;
    private bool value = false;

    private void Start()
    {
        value = defaultValue;
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
}
