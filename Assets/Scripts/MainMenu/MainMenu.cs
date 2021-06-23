using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SectionMainMenu startSection, menuSection, settingsSection, informationsSection;
    public NewGameSectionMainMenu newGameSection;
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

            SpeechController.ReadText("Menu principal. Glisser vers le haut pour commencer.");
        }
        else
        {
            SetSection(menuSection);

            SpeechController.ReadText("Menu principal.");
        }

        if(SaveManager.sessionGameStarted)
            GameManager.instance.ChangeMusic(music, 0.1f);
        else
            GameManager.instance.ChangeMusic(music, 100f);
    }

    private void Update()
    {
        if(currentSection != null)
            currentSection.Frame();
        else
        {
            Debug.LogError("Current section is null! Switching to start section");
            SetSection(startSection);
        }
    }

    // Buttons
    public void StartGame()
    {
        SaveManager.sessionGameStarted = true;
        newGameSection.OpenPanel();
    }

    public void SettingsVisible (bool visible)
    {
        if (visible)
        {
            SetSection(settingsSection);
            SpeechController.ReadText("Paramètres.");
        }
        else
        {
            SetSection(menuSection);
            SpeechController.ReadText("Accueil.");
        }

    }

    public void InformationsVisible (bool visible)
    {
        if (visible)
        {
            SetSection(informationsSection);
            SpeechController.ReadText("Prévention.");
        }
        else
        {
            SetSection(menuSection);
            SpeechController.ReadText("Accueil.");
        }
    }
}
