using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public void OnClick (string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
