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
    private SaveManager saveManager = new SaveManager();
    private GameObject conversationFlux;
    private Character npCharacter;
    private Character playerCharacter;
    private Character.Relationship npcToPlayerRelationhship;
    private GameObject footer;
    [HideInInspector]
    public bool isInChoice { get; set; } = false;
    [HideInInspector]
    public Conversation.Branche nextBranch {get; set;}
    private List<string> branchList = new List<string>();
    private GameManager gameManager;
    [HideInInspector]
    public bool endConversation;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.conversationDisplayer = this;
    }

    void OnApplicationQuit()
    {
        saveManager.SaveGame(conversation.id, branchList, gameManager.charactersSet);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            saveManager.SaveGame(conversation.id, branchList, gameManager.charactersSet);
        }
    }
    public void LaunchAConv(Conversation currentConversation, List<string> branchesList)
    {
        conversation = currentConversation;
        conversationFlux = GameObject.Find("ConversationFlux");
        medium = LoadMedium(conversation.medium);
        if(branchesList.Count > 0)
        {
            conversation.startingBranch = branchesList[branchesList.Count - 1];
        }

        foreach (var character in gameManager.charactersSet)
        {
            character.DebugLogCharacter();
            if(character.id == conversation.npCharacter)
            {
                npCharacter = character;
            }
            if (character.id == conversation.playerCharacter)
            {
                playerCharacter = character;
            }
        }


        StartCoroutine(LoadBranches(GetBrancheByID(conversation.startingBranch)));


        foreach (var relationship in npCharacter.relationships)
        {
            if (relationship.them == playerCharacter.id)
            {
                npcToPlayerRelationhship = relationship;
            }
        }

        saveManager.SaveGame(conversation.id, branchList, gameManager.charactersSet);

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
            if(message.content is ImageContent)
            {
                messageBoxPrefab = medium.npcMessageBoxImage;
            }
            else
            {
                messageBoxPrefab = medium.npcMessageBox;
            }
        }
        else
        {
            if (message.content is ImageContent)
            {
                messageBoxPrefab = medium.playerMessageBoxImage;
            }
            else
            {
                messageBoxPrefab = medium.playerMessageBox;
            }
        }




        GameObject messageBox = Instantiate(messageBoxPrefab, transform);

        if(message.content is ImageContent)
        {
            Image image = messageBox.transform.Find("Mask").Find("MediaImage").GetComponent<Image>();
            ImageContent imgContent = (ImageContent)message.content;
            Sprite newSprite = imgContent.LoadNewSprite();
            image.overrideSprite = newSprite;
        }
        else
        {
            Text textComponent = messageBox.GetComponentsInChildren<Text>()[0];
            textComponent.text = message.content.data;
        }

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

    private void MoveFooter(bool isMoveUp)
    {
        RectTransform rectTransform = footer.GetComponent<RectTransform>();
        if (isMoveUp)
        {
            isInChoice = true;
            float upValue = rectTransform.sizeDelta.y - medium.footerHeigth;
            MoveFlux(upValue);
            rectTransform.position += new Vector3(0, upValue, 0);
        }
        else
        {

            float downValue = -rectTransform.sizeDelta.y + medium.footerHeigth;
            MoveFlux(downValue);
            rectTransform.position += new Vector3(0, downValue, 0);
        }
    }

    private IEnumerator LoadChoiceBranch(List<Conversation.Possibility> choicePossibilities)
    {
        Conversation.Branche branch = GetBrancheByID(choicePossibilities[0].branch);

        MoveFooter(true);

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

        MoveFooter(false);

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
                            Debug.Log("Plus value : " + newButton.Value.confidenceMod);

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
                    Debug.Log(poss.thresholds[0] + " / " + npcToPlayerRelationhship.confidenceMeToThem  + " / " +  poss.thresholds[1]);
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



                StartCoroutine(EndConversation());
                break;
            default:
                Debug.LogError("Type de branching Point Inconnue");
                break;
        }
    }



    private IEnumerator EndConversation()
    {
        Debug.LogWarning("Conversation End");

        MoveFooter(true);
        Instantiate(medium.nextConvoButton, footer.transform);
        saveManager.SaveGame(conversation.id, branchList, gameManager.charactersSet);

        yield return new WaitUntil(() => endConversation);

        gameManager.nextConversation = conversation.nextConversation;
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
