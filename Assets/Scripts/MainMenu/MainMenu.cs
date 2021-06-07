using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static int minSwipeDistance = 150;
    public SectionMainMenu startSection, menuSection, settingsSection;

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
        SetSection(startSection);
    }

    private void Update()
    {
        currentSection.Frame();
    }

    // Methods
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
