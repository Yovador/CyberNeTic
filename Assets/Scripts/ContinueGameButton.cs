using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueGameButton : MenuButton
{
    public override void OnClick()
    {

        gameManager.ContinueGame();

    }
}
