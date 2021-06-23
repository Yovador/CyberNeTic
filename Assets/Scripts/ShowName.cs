using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowName : MonoBehaviour
{
    TMP_Text textComponent;
    ConversationDisplayer conversationDisplayer;
    [SerializeField]
    bool isNPC = true;
    [SerializeField]
    bool showLastName;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        conversationDisplayer = GameManager.instance.conversationDisplayer;
        if (isNPC)
        {

            textComponent.text = conversationDisplayer.npCharacter.firstName;
            if (showLastName)
            {
                textComponent.text += $" {conversationDisplayer.npCharacter.lastName}";
            }

        }
        else
        {
            textComponent.text = conversationDisplayer.playerCharacter.firstName;
            if (showLastName)
            {
                textComponent.text += $" {conversationDisplayer.playerCharacter.lastName}";
            }
        }

    }

}
