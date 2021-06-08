using UnityEngine;

public class SettingsSectionMainMenu : SectionMainMenu
{
    public FloatSettingsField volume;
    public BoolSettingsField easyMode;

    private PlayerSettings settings;

    public override void Land()
    {
        settings = SaveManager.LoadSettings();

        base.Land();

        volume.SetValue(settings.volume);
        easyMode.SetValue(settings.easyMode);

        // Callbacks
        volume.OnChanged += SaveSettings;
        easyMode.OnChanged += SaveSettings;
    }

    private void SaveSettings()
    {
        settings.volume = volume.GetValue();
        settings.easyMode = easyMode.GetValue();

        SaveManager.SaveSettings(settings);
    }
}

public class PlayerSettings
{
    public float volume = 0.5f;
    public bool easyMode = false;
}