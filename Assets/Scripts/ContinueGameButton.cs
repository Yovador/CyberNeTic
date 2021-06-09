using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueGameButton : MenuButton
{
    protected override IEnumerator LaunchSequence()
    {
        StartCoroutine(base.LaunchSequence());

        yield return new WaitWhile(() => loadingPanel.isFading);

        Debug.Log("In ContinueGame");
        gameManager.ContinueGame();
        yield return null;
    }

}
