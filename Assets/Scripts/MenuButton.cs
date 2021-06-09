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
        Debug.Log("Click " + gameObject.name);
        StartCoroutine(LaunchSequence());
    }

    protected virtual IEnumerator LaunchSequence()
    {
        Debug.Log("LaunchSequence " + gameObject.name);
        loadingPanel = loadingPanelObj.GetComponent<LoadingPanel>();

        StartCoroutine(loadingPanel.Appear());
        yield return new WaitWhile(() => loadingPanel.isFading);


    }
}
