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
    private Character npCharacter { get; set; }
    private Character playerCharacter { get; set; }
    private List<Character> characterList;
    private Character.Relationship npcToPlayerRelationhship;

    private void Start()
    {
        conversation = jsonUnloader.LoadConversationFromJson(Path.Combine(Application.streamingAssetsPath, "Conversations", "ChangeTest.json"));
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
            if (character.id == conversation.playerCharacter)
            {
                playerCharacter = character;
            }
        }
        Debug.Log(playerCharacter.id);
        StartCoroutine(LoadBranches(conversation.branches[0]));

        foreach (var relationship in npCharacter.relationships)
        {
            if (relationship.them == playerCharacter.id)
            {
                npcToPlayerRelationhship = relationship;
            }
        }
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
        Instantiate(medium.navBar, transform);
        Instantiate(medium.footer, transform);

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

            //Monter des messages sans animation **TEMPORAIRE**
            Vector2 newPos = new Vector2(conversationFlux.transform.position.x, conversationFlux.transform.position.y + spaceBetweenMessages + rectTransform.sizeDelta.y );
            conversationFlux.transform.position = newPos;
            //**TEMPORAIRE**

            yield return new WaitForSecondsRealtime(waitTime);
            newMessage.transform.SetParent(conversationFlux.transform);
        }

        LoadBranchingPoint(branche.branchingPoint);

    }

    private Conversation.Branche GetBrancheByID (string id)
    {
        Conversation.Branche foundBranch = null;
        foreach (var branche in conversation.branches)
        {
            if(branche.id == id)
            {
                foundBranch = branche;
            }
        }
        return foundBranch;
    }

    private void LoadBranchingPoint(Conversation.BranchingPoint branchingPoint)
    {
        switch (branchingPoint.type)
        {
            case "choice":
                break;
            case "test":

                Conversation.Branche branch = GetBrancheByID(branchingPoint.possibilities[0].branch);
                foreach (Conversation.TestPossibility poss in branchingPoint.possibilities)
                {
                    if (npcToPlayerRelationhship.confidenceMeToThem >= poss.thresholds[0] && npcToPlayerRelationhship.confidenceMeToThem <= poss.thresholds[0])
                    {
                        branch = GetBrancheByID(poss.branch);
                    }
                }
                StartCoroutine(LoadBranches(branch));

                break;
            case "change":

                StartCoroutine(LoadBranches(GetBrancheByID(branchingPoint.possibilities[0].branch)));

                break;
            case "stop":
                Debug.LogWarning("Conversation End");
                break;
            default:
                Debug.LogError("Type de branching Point Inconnue");
                break;
        }
    }

    public void LoadProfilePicture(Image imageComponent, string character)
    {
        Sprite newSprite;
        switch (character)
        {
            case "npc":
                newSprite = npCharacter.profilePicture.LoadNewSprite();
                break;
            case "player":
                newSprite = playerCharacter.profilePicture.LoadNewSprite();
                break;
            default:
                newSprite = null;
                break;
        }
        if(newSprite != null)
        {
            imageComponent.sprite = newSprite;
        }
    }

}
