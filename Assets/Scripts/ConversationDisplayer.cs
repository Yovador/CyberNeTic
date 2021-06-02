using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class ConversationDisplayer : MonoBehaviour
{
    [SerializeField]
    private string defaultMedium;
    [SerializeField]
    private float spaceBetweenMessages;
    [SerializeField]
    private float waitTime;
    private Medium medium;
    private Conversation conversation;
    private JsonUnloader jsonUnloader = new JsonUnloader();
    private GameObject conversationFlux;
    public static Character npCharacter { get; set; }
    private List<Character> characterList;

    private void Start()
    {
        conversation = jsonUnloader.LoadConversationFromJson(Path.Combine(Application.streamingAssetsPath, "Conversations", "conversationExample.json"));
        conversation.DebugLogConversation();
        conversationFlux = GameObject.Find("ConversationFlux");
        medium = LoadMedium(conversation.medium);
        characterList = jsonUnloader.LoadCharacterListFromJson(Path.Combine(Application.streamingAssetsPath, "Characters", "characterSetExample.json"));

        foreach (var character in characterList)
        {
            if(character.id == conversation.npCharacter)
            {
                npCharacter = character;
            }
        }

        StartCoroutine(LoadBranches(conversation.branches[0]));

    }



    private Medium LoadMedium(string mediumID)
    {
        Medium medium;

        string mediumPath = $"Medium/{mediumID}";

        medium = Resources.Load<Medium>(mediumPath);


        if(medium == null)
        {
            mediumPath = $"Medium/{defaultMedium}";
            Debug.Log(Resources.Load<Medium>(mediumPath));
            medium = Resources.Load<Medium>(mediumPath);
        }

        Instantiate(medium.background, transform).transform.SetSiblingIndex(0);

        return medium;
    }

    private GameObject LoadMessage(Conversation.Message message)
    {
        GameObject messageBoxPrefab;

        Debug.Log(message.isNpc);

        if (message.isNpc)
        {
            messageBoxPrefab = medium.npcMessageBox;
        }
        else
        {
            messageBoxPrefab = medium.playerMessageBox;
        }

        GameObject messageBox = Instantiate(messageBoxPrefab, transform);

        Text textComponent = messageBox.GetComponentsInChildren<Text>()[0];
        textComponent.text = message.content.data;

        return messageBox;
    }

    private IEnumerator LoadBranches(Conversation.Branche branche )
    {
        foreach (var message in branche.messagesList)
        {
            GameObject newMessage = LoadMessage(message);
            RectTransform rectTransform = newMessage.GetComponent<RectTransform>();
            Vector2 newPos = new Vector2(conversationFlux.transform.position.x, conversationFlux.transform.position.y + spaceBetweenMessages + rectTransform.sizeDelta.y );
            conversationFlux.transform.position = newPos;
            yield return new WaitForSecondsRealtime(waitTime);
            newMessage.transform.SetParent(conversationFlux.transform);
        }
    }

}
