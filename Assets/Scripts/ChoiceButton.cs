using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceButton : ConversationButtons
{
    [HideInInspector]
    public Conversation.Branche branche { get; set; }
    [HideInInspector]

    public override void OnClick()
    {

        conversationDisplayer.isInChoice = false;
        conversationDisplayer.nextBranch = branche;
        conversationDisplayer.choiceButton = this;

    }
}
