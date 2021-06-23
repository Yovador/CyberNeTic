using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FirstMessageLoad : MonoBehaviour
{

    TMP_Text nameTextComponent;
    TMP_Text messageTextComponent;
    Conversation firstConv;
    Character firstCharacter;
    Conversation.Message firstMessage;
    [SerializeField]
    bool showLastName;

    private void Start()
    {
        nameTextComponent = transform.Find("Name").GetComponent<TMP_Text>();
        messageTextComponent = transform.Find("Content").GetComponent<TMP_Text>();
        firstConv = GameManager.instance.FindConvById(GameManager.instance.firstConversation);
        firstMessage = firstConv.GetFirstMessage();
        messageTextComponent.text = firstMessage.content.data;
        if (firstMessage.isNpc)
        {
            firstCharacter = GameManager.GetCharacterByID(firstConv.npCharacter);
        }
        else
        {
            firstCharacter = GameManager.GetCharacterByID(firstConv.playerCharacter);
        }

        nameTextComponent.text = firstCharacter.firstName;
        if (showLastName)
        {
            nameTextComponent.text += $" {firstCharacter.lastName}";
        }


    }

}
