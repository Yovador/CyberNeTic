using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceButton : MonoBehaviour
{
    private ConversationDisplayer conversationDisplayer;
    [HideInInspector]
    public Conversation.Branche branche {get; set;}

    private void Start()
    {
        conversationDisplayer = GameObject.FindGameObjectWithTag("ConversationDisplayer").GetComponent<ConversationDisplayer>();
    }

    public void OnClick()
    {
        conversationDisplayer.isInChoice = false;
        conversationDisplayer.nextBranch = branche;
    }
}
