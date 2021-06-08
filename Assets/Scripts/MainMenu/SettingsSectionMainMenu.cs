using UnityEngine;
using UnityEngine.UI;

public class SettingsSectionMainMenu : SectionMainMenu
{
    public FloatSettingsField volume;
    public BoolSettingsField easyMode;

    public override void Land()
    {
        SaveManager.LoadSettings();

        base.Land();

        volume.SetValue(SaveManager.settings.volume);
        easyMode.SetValue(SaveManager.settings.easyMode);
    }

    public void SaveSettings()
    {
        SaveManager.settings.volume = volume.GetValue();
        SaveManager.settings.easyMode = easyMode.GetValue();

        SaveManager.SaveSettings();
    }

}