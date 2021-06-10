using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SectionMainMenu startSection, menuSection, settingsSection, informationsSection;

    public static int minSwipeDistance = 150;

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
        }else
        {
            SetSection(menuSection);
        }
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
            SetSection(settingsSection);
        else
            SetSection(menuSection);
    }

    public void InformationsVisible (bool visible)
    {
        if (visible)
            SetSection(informationsSection);
        else
            SetSection(menuSection);
    }
}
