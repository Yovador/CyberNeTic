using UnityEngine;

public class SettingsSectionMainMenu : SectionMainMenu
{
    public FloatSettingsField volume;
    public BoolSettingsField easyMode;

    public override void Land()
    {
        base.Land();

        volume.OnChanged += SaveSettings;
        easyMode.OnChanged += SaveSettings;
    }

    private void SaveSettings()
    {
        Debug.Log("Saving user settings...");
    }
}
