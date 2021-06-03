using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameButton : MenuButton
{

    public override void OnClick()
    {
        gameManager.StartNewGame();
    }

}
