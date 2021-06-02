using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class ProfilePicture : MonoBehaviour
{
    private enum PPChar {player, npc}
    [SerializeField]
    private PPChar profilesPictureCharacter;
    private Image imageComponent;
    private ConversationDisplayer conversationDisplayer;

    private void Start()
    {
        imageComponent = GetComponent<Image>();
        conversationDisplayer = GameObject.FindGameObjectWithTag("ConversationDisplayer").GetComponent<ConversationDisplayer>();
        Debug.Log(profilesPictureCharacter.ToString());
        conversationDisplayer.LoadProfilePicture(imageComponent, profilesPictureCharacter.ToString());
    }



}
