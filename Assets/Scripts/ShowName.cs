using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowName : MonoBehaviour
{
    TMP_Text textComponent;
    ConversationDisplayer conversationDisplayer;
    [SerializeField]
    bool showFirstName;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        conversationDisplayer = GameManager.instance.conversationDisplayer;
        textComponent.text = conversationDisplayer.npCharacter.firstName;

        if (showFirstName)
        {
            textComponent.text += $" {conversationDisplayer.npCharacter.lastName}";
        }
    }

}
