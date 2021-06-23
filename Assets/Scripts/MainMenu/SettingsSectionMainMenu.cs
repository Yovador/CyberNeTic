using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsSectionMainMenu : SectionMainMenu
{
    public FloatSettingsField volumeEffectsField, volumeMusicField, readSpeedField;
    public DropdownSettingsField colorBlindField;
    public BoolSettingsField speechHelpField, vibrationsField;
    [Space]
    public TMP_Text versionText;

    public override void Land()
    {
        SaveManager.LoadSettings();
        versionText.text = Application.version;

        base.Land();

        colorBlindField.OnChanged += OnColorBlindChange;
        volumeEffectsField.OnChanged += OnVolumeChanged;
        volumeMusicField.OnChanged += OnVolumeChanged;

        UpdateUI();
    }

    public override void Exit()
    {
        colorBlindField.OnChanged -= OnColorBlindChange;
        volumeEffectsField.OnChanged -= OnVolumeChanged;
        volumeMusicField.OnChanged -= OnVolumeChanged;

        base.Exit();
    }

    public void SaveSettings()
    {
        SaveManager.settings.volumeEffects = volumeEffectsField.GetValue();
        SaveManager.settings.volumeMusic = volumeMusicField.GetValue();
        SaveManager.settings.readSpeed = readSpeedField.GetValue();
        SaveManager.settings.colorBlind = colorBlindField.GetValue();
        SaveManager.settings.speechHelp = speechHelpField.GetValue();
        SaveManager.settings.vibrations = vibrationsField.GetValue();

        SaveManager.SaveSettings();
    }

    private void UpdateUI()
    {
        volumeEffectsField.SetValue(SaveManager.settings.volumeEffects);
        volumeMusicField.SetValue(SaveManager.settings.volumeMusic);
        readSpeedField.SetValue(SaveManager.settings.readSpeed);
        colorBlindField.SetValue(SaveManager.settings.colorBlind);
        speechHelpField.SetValue(SaveManager.settings.speechHelp);
        vibrationsField.SetValue(SaveManager.settings.vibrations);
    }

    private void OnVolumeChanged()
    {
        GameManager.UpdateSourceVolume(volumeMusicField.GetValue(), volumeEffectsField.GetValue());
    }

    private void OnColorBlindChange()
    {
        GameManager.UpdateColorBlindFilter(colorBlindField.GetValue());
    }

}