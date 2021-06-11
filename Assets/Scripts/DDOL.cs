using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DDOL : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    #region Singleton
    public static DDOL instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    void Start()
    {
        SceneManager.LoadScene(sceneName);
    }
}
