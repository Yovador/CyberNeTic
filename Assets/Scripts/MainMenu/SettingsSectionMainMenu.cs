using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsSectionMainMenu : SectionMainMenu
{
    public FloatSettingsField volume;
    public BoolSettingsField easyMode;
    [Space]
    public TMP_Text versionText;

    public override void Land()
    {
        SaveManager.LoadSettings();
        versionText.text = Application.version;

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