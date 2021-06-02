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
    private float waitTime;
    private Medium medium;
    private Conversation conversation;
    private JsonUnloader jsonUnloader = new JsonUnloader();
    private GameObject conversationFlux;
    private Character npCharacter;
    private Character playerCharacter;
    private List<Character> characterList;
    private Character.Relationship npcToPlayerRelationhship;
    private GameObject footer;
    [HideInInspector]
    public bool isInChoice { get; set; } = false;
    [HideInInspector]
    public Conversation.Branche nextBranch {get; set;}
    private SaveManager saveManager = new SaveManager();
    private List<string> branchList = new List<string>();

private void Start()
    {
        conversation = jsonUnloader.LoadConversationFromJson(Path.Combine(Application.streamingAssetsPath, "Conversations", "olivier-sophie.json"));
        conversationFlux = GameObject.Find("ConversationFlux");
        medium = LoadMedium(conversation.medium);
        characterList = jsonUnloader.LoadCharacterListFromJson(Path.Combine(Application.streamingAssetsPath, "Characters", "characterSet2.json"));

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


        StartCoroutine(LoadBranches(conversation.branches[0]));


        foreach (var relationship in npCharacter.relationships)
        {
            if (relationship.them == playerCharacter.id)
            {
                npcToPlayerRelationhship = relationship;
            }
        }

        saveManager.SaveGame(conversation.id, branchList, characterList);

    }



    private Medium LoadMedium(string mediumID)
    {
        Medium medium;

        string mediumPath = $"Medium/{mediumID}";

        medium = Resources.Load<Medium>(mediumPath);


        if(medium == null)
        {
            mediumPath = $"Medium/{defaultMedium}";
            medium = Resources.Load<Medium>(mediumPath);
        }

        Instantiate(medium.background, transform).transform.SetSiblingIndex(0);
        Instantiate(medium.navBar, transform);
        footer = Instantiate(medium.footer, transform);

        return medium;
    }

    private GameObject LoadMessage(Conversation.Message message)
    {
        GameObject messageBoxPrefab;

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

        RectTransform rectTransform = messageBox.GetComponent<RectTransform>();
        MoveFlux(medium.spaceBetweenMessages + rectTransform.sizeDelta.y );

        messageBox.transform.SetParent(conversationFlux.transform);

        return messageBox;
    }

    private void MoveFlux(float value)
    {
        //Monter des messages sans animation **TEMPORAIRE**
        Vector2 newPos = new Vector2(conversationFlux.transform.position.x, conversationFlux.transform.position.y +  value);
        conversationFlux.transform.position = newPos;
        //**TEMPORAIRE**
    }

    private IEnumerator LoadBranches(Conversation.Branche branche )
    {
        foreach (var message in branche.messagesList)
        {
            LoadMessage(message);
            yield return new WaitForSecondsRealtime(waitTime);
        }

        branchList.Add(branche.id);
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

    private IEnumerator LoadChoiceBranch(List<Conversation.Possibility> choicePossibilities)
    {
        Conversation.Branche branch = GetBrancheByID(choicePossibilities[0].branch);
        RectTransform rectTransform = footer.GetComponent<RectTransform>();
        isInChoice = true;
        float upValue = rectTransform.sizeDelta.y - medium.footerHeigth;
        MoveFlux(upValue);
        rectTransform.position += new Vector3(0, upValue, 0);
        Dictionary<GameObject, Conversation.ChoicePossibility> buttonList = new Dictionary<GameObject, Conversation.ChoicePossibility>();
        for (int i = 0; i < choicePossibilities.Count; i++)
        {
            Conversation.ChoicePossibility poss = (Conversation.ChoicePossibility)choicePossibilities[i];
            GameObject newButton;
            if (poss.possible)
            {
                newButton = Instantiate(medium.choiceButton, footer.transform);
            }
            else
            {
                newButton = Instantiate(medium.impossibleChoiceButton, footer.transform);

            }
            RectTransform rectTransformButton = newButton.GetComponent<RectTransform>();
            rectTransformButton.position -= new Vector3(0, i * (medium.spaceBetweenChoices + (rectTransformButton.sizeDelta.y/2)) , 0 );
            newButton.GetComponentInChildren<Text>().text = poss.message.content.data;
            if (poss.possible)
            {
                newButton.GetComponent<ChoiceButton>().branche = GetBrancheByID(poss.branch);
            }
            newButton.name = "choiceButton";
            buttonList.Add(newButton, poss);
        }

        //yield return new WaitForSecondsRealtime(waitTime);
        yield return new WaitWhile(() => isInChoice);

        float downValue = -rectTransform.sizeDelta.y + medium.footerHeigth;
        MoveFlux(downValue);
        rectTransform.position += new Vector3(0, downValue, 0);

        foreach (var newButton in buttonList)
        {
            ChoiceButton choiceButton = newButton.Key.GetComponent<ChoiceButton>();
            if(choiceButton != null)
            {
                if (choiceButton.branche == nextBranch)
                {
                    LoadMessage(newButton.Value.message);


                    foreach (var relationship in npCharacter.relationships)
                    {
                        if (relationship.them == playerCharacter.id)
                        {
                            relationship.confidenceMeToThem += newButton.Value.confidenceMod;

                        }
                    }

                }
            }


            Destroy(newButton.Key);
        }
        
        yield return new WaitForSecondsRealtime(waitTime);
        StartCoroutine(LoadBranches(nextBranch));

    }

    private void LoadBranchingPoint(Conversation.BranchingPoint branchingPoint)
    {
        switch (branchingPoint.type)
        {
            case "choice":
                StartCoroutine(LoadChoiceBranch((branchingPoint.possibilities)));
                break;
            case "test":
                nextBranch = GetBrancheByID(branchingPoint.possibilities[0].branch);
                foreach (Conversation.TestPossibility poss in branchingPoint.possibilities)
                {
                    if (npcToPlayerRelationhship.confidenceMeToThem >= poss.thresholds[0] && npcToPlayerRelationhship.confidenceMeToThem <= poss.thresholds[1])
                    {
                        nextBranch = GetBrancheByID(poss.branch);
                    }
                }
                StartCoroutine(LoadBranches(nextBranch));


                break;
            case "change":
                nextBranch = GetBrancheByID(branchingPoint.possibilities[0].branch);
                StartCoroutine(LoadBranches(nextBranch));

                break;
            case "stop":
                EndConversation();
                break;
            default:
                Debug.LogError("Type de branching Point Inconnue");
                break;
        }
    }

    private void EndConversation()
    {
        Debug.LogWarning("Conversation End");
        saveManager.SaveGame(conversation.id, branchList, characterList);
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
