using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationButtons : MonoBehaviour
{
    [HideInInspector]
    protected ConversationDisplayer conversationDisplayer { get; set; }

    private void Start()
    {
        conversationDisplayer = GameObject.Find("ConversationDisplayer").GetComponent<ConversationDisplayer>();
    }

    virtual public void OnClick()
    {
    }
}
