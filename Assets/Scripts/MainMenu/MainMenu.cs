using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SectionMainMenu startSection, menuSection, settingsSection, informationsSection;
    [Space]
    public AudioClip music;
    
    public static int minSwipeDistance = 100;

    private SectionMainMenu currentSection;

    #region Singleton
    public static MainMenu instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public void SetSection (SectionMainMenu newSection)
    {
        if (currentSection != null) currentSection.Exit();
        currentSection = newSection;
        currentSection.Land();
    } 

    private void Start()
    {
        SaveManager.LoadSettings();
        GameManager.ApplySettingsToScene();

        if (!SaveManager.sessionGameStarted)
        {
            SetSection(startSection);

            if (SaveManager.settings.speechHelp)
                SpeechController.ReadText("Menu principal. Glisser vers le haut pour commencer.");
        }
        else
        {
            SetSection(menuSection);

            if (SaveManager.settings.speechHelp)
                SpeechController.ReadText("Menu principal.");
        }

        GameObject.Find("MusicLoader").GetComponent<AudioSource>().PlayOneShot(music);
    }

    private void Update()
    {
        currentSection.Frame();
    }

    // Buttons
    public void StartGame()
    {
        SaveManager.sessionGameStarted = true;
        SceneManager.LoadScene("MainMenu");
    }

    public void SettingsVisible (bool visible)
    {
        if (visible)
        {
            SetSection(settingsSection);

            if (SaveManager.settings.speechHelp)
                SpeechController.ReadText("Paramètres.");
        }
        else
        {
            SetSection(menuSection);

            if (SaveManager.settings.speechHelp)
                SpeechController.ReadText("Accueil.");
        }

    }

    public void InformationsVisible (bool visible)
    {
        if (visible)
        {
            SetSection(informationsSection);

            if (SaveManager.settings.speechHelp)
                SpeechController.ReadText("Prévention.");
        }
        else
        {
            SetSection(menuSection);

            if (SaveManager.settings.speechHelp)
                SpeechController.ReadText("Accueil.");
        }
    }
}
