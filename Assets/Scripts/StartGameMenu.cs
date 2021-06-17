using UnityEngine;

public class StartGameMenu : MonoBehaviour
{
    void Start()
    {
        if (SaveManager.settings.speechHelp)
            SpeechController.ReadText("Démarrer une nouvelle partie ou continuer la précédente");
    }

    public void OnNewGame ()
    {
        if (SaveManager.settings.speechHelp)
            SpeechController.ReadText("Nouvelle partie");
    }

    public void OnContinue()
    {
        if (SaveManager.settings.speechHelp)
            SpeechController.ReadText("Continuer");
    }

}
