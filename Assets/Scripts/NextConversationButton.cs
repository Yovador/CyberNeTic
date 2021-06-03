using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextConversationButton : ConversationButtons
{

    public override void OnClick()
    {
        conversationDisplayer.endConversation = true;
    }

}
