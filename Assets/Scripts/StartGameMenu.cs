using UnityEngine;

public class StartGameMenu : MonoBehaviour
{
    void Start()
    {
        if (SaveManager.settings.speechHelp)
            SpeechController.ReadText("D�marrer une nouvelle partie ou continuer la pr�c�dente");
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
