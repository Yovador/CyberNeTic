using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsSectionMainMenu : SectionMainMenu
{
    public FloatSettingsField volumeEffectsField, volumeMusicField, readSpeedField;
    public DropdownSettingsField colorBlindField;
    [Space]
    public TMP_Text versionText;

    public override void Land()
    {
        SaveManager.LoadSettings();
        versionText.text = Application.version;

        base.Land();

        colorBlindField.OnChanged += OnColorBlindChange;

        UpdateUI();
    }

    public override void Exit()
    {
        colorBlindField.OnChanged -= OnColorBlindChange;

        base.Exit();
    }

    public void SaveSettings()
    {
        SaveManager.settings.volumeEffects = volumeEffectsField.GetValue();
        SaveManager.settings.volumeMusic = volumeMusicField.GetValue();
        SaveManager.settings.readSpeed = readSpeedField.GetValue();
        SaveManager.settings.colorBlind = colorBlindField.GetValue();

        SaveManager.SaveSettings();
    }

    private void UpdateUI()
    {
        volumeEffectsField.SetValue(SaveManager.settings.volumeEffects);
        volumeMusicField.SetValue(SaveManager.settings.volumeMusic);
        readSpeedField.SetValue(SaveManager.settings.readSpeed);
        colorBlindField.SetValue(SaveManager.settings.colorBlind);
    }

    private void OnColorBlindChange()
    {
        DDOL.UpdateColorBlindFilter(colorBlindField.GetValue());
    }

}