using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public void OnClick (string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReadText (string text)
    {
        if (SaveManager.settings.speechHelp)
            SpeechController.ReadText(text);
    }
}
