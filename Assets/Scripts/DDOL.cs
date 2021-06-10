using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DDOL : MonoBehaviour
{
    [SerializeField]
    private string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.LoadScene(sceneName);

    }

    private void OnLevelWasLoaded (int level)
    {
        UpdateColorBlindFilter();
    }

    public static void UpdateColorBlindFilter()
    {
        Wilberforce.Colorblind colorblindScript = Camera.main.GetComponent<Wilberforce.Colorblind>();

        if (colorblindScript != null)
        {
            colorblindScript.Type = SaveManager.settings.colorBlind;
        }
    }

    public static void UpdateColorBlindFilter (int colorBlindType)
    {
        Wilberforce.Colorblind colorblindScript = Camera.main.GetComponent<Wilberforce.Colorblind>();

        if (colorblindScript != null)
        {
            colorblindScript.Type = colorBlindType;
        }
    }


}
