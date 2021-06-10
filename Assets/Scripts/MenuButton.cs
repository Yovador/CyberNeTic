using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    protected GameManager gameManager;
    protected LoadingPanel loadingPanel;
    protected GameObject loadingPanelObj;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        loadingPanelObj = GameObject.Find("LoadingPanel");
    }

    public void OnClick()
    {
        StartCoroutine(LaunchSequence());
    }

    protected virtual IEnumerator LaunchSequence()
    {
        loadingPanel = loadingPanelObj.GetComponent<LoadingPanel>();

        StartCoroutine(loadingPanel.Appear());
        yield return new WaitWhile(() => loadingPanel.isFading);


    }
}
