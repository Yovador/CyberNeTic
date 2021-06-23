using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameButton : MenuButton
{
    protected override IEnumerator LaunchSequence()
    {
        StartCoroutine(base.LaunchSequence());
        yield return new WaitWhile(() => loadingPanel.isFading);

        gameManager.StartNewGame();
        yield return null;
    }


}
