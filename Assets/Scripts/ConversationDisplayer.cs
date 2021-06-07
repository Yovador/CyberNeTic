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
    private bool animationOn = false;

    [SerializeField, Range(0.5f, 5f)]
    private float messageSpeed;

    private ScrollRect scrollRect;
    private RectTransform scrollTransform;

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
        conversation.DebugLogConversation();
        scrollRect = GetComponentInChildren<ScrollRect>();
        scrollRect.enabled = false;
        scrollTransform = scrollRect.gameObject.GetComponent<RectTransform>();
        scrollTransform.sizeDelta = new Vector2(0, footer.GetComponent<RectTransform>().sizeDelta.y + medium.navBar.GetComponent<RectTransform>().sizeDelta.y);

        if (branchesList.Count > 0)
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

        // Update header name
        GameObject.FindGameObjectWithTag("ContactName").GetComponent<Text>().text = npCharacter.firstName;

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

    private IEnumerator LoadMessage(Conversation.Message message)
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

        GameObject messageBox = Instantiate(messageBoxPrefab, transform.Find("Scroll") );
        GameObject backgroungMessage = messageBox.transform.Find("Background").gameObject;
        MessageBoxResizer messageBoxResizer = backgroungMessage.GetComponent<MessageBoxResizer>();



        if (message.content is ImageContent)
        {
            Image image = backgroungMessage.transform.Find("Mask").Find("MediaImage").GetComponent<Image>();
            ImageContent imgContent = (ImageContent)message.content;
            Sprite newSprite = imgContent.LoadNewSprite();
            image.overrideSprite = newSprite;
        }
        else
        {
            Text textComponent = backgroungMessage.transform.Find("Text").GetComponent<Text>();
            textComponent.text = message.content.data;
            messageBoxResizer.ResizeBox();
        }


        LayoutGroup rectTransform = messageBox.GetComponent<LayoutGroup>();
        yield return null;
        
        float heigth = rectTransform.preferredHeight;
        float size = medium.spaceBetweenMessages + heigth;
        Debug.Log("Content : " + message.content.data + "\nHeight : " + heigth + "\nSize: " + size);

        scrollTransform.sizeDelta += new Vector2(0, size);


        if (conversationFlux.transform.childCount > 0)
        {

            yield return new WaitWhile(() => animationOn);

            StartCoroutine(MoveFlux(size));

            yield return new WaitWhile(() => animationOn);
        }


        messageBox.transform.SetParent(conversationFlux.transform);


    }


    private IEnumerator MoveFlux(float value)
    {
        Debug.Log("Flux move : " + value);
        animationOn = true;
        Vector2 newPos = new Vector2(conversationFlux.transform.position.x, conversationFlux.transform.position.y +  value);

        while (Vector2.Distance(conversationFlux.transform.position, newPos) > 0.5f)
        {
            conversationFlux.transform.position = Vector2.Lerp(conversationFlux.transform.position, newPos, messageSpeed * Time.deltaTime);
            yield return null;
        }
        conversationFlux.transform.position = newPos;

        animationOn = false;


    }

    private IEnumerator LoadBranches(Conversation.Branche branche )
    {
        foreach (var message in branche.messagesList)
        {
            yield return new WaitWhile(() => animationOn);

            StartCoroutine(LoadMessage(message));
            yield return new WaitForSecondsRealtime(waitTime);
            yield return new WaitWhile(() => animationOn);

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

    private IEnumerator MoveFooter(bool isMoveUp)
    {

        animationOn = true;

        RectTransform rectTransform = footer.GetComponent<RectTransform>();
        Vector2 newPos;


        if (isMoveUp)
        {
            isInChoice = true;
            float upValue = rectTransform.sizeDelta.y - medium.footerHeigth;
            StartCoroutine(MoveFlux(upValue));
            newPos = rectTransform.position + new Vector3(0, upValue, 0);

        }
        else
        {
            float downValue = -(rectTransform.sizeDelta.y - medium.footerHeigth);
            StartCoroutine(MoveFlux(downValue));
            newPos = rectTransform.position + new Vector3(0, downValue, 0);

        }


        while (Vector2.Distance(rectTransform.transform.position, newPos) > 0.5f)
        {
            rectTransform.transform.position = Vector2.Lerp(rectTransform.transform.position, newPos, messageSpeed * Time.deltaTime);
            yield return null;
        }

        animationOn = false;

    }

    private IEnumerator LoadChoiceBranching(List<Conversation.Possibility> choicePossibilities)
    {

        Conversation.Branche branch = GetBrancheByID(choicePossibilities[0].branch);

        StartCoroutine(MoveFooter(true));


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

        yield return new WaitWhile(() => animationOn);
        scrollRect.enabled = true;


        //yield return new WaitForSecondsRealtime(waitTime);
        yield return new WaitWhile(() => isInChoice);

        scrollRect.enabled = false;
        StartCoroutine(MoveFooter(false));
        foreach (var newButton in buttonList)
        {
            newButton.Key.SetActive(false);
        }
        yield return new WaitWhile(() => animationOn);


        foreach (var newButton in buttonList)
        {
            ChoiceButton choiceButton = newButton.Key.GetComponent<ChoiceButton>();
            if(choiceButton != null)
            {
                if (choiceButton.branche == nextBranch)
                {
                    StartCoroutine(LoadMessage(newButton.Value.message));


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
        yield return new WaitWhile(() => animationOn);
        yield return new WaitForSecondsRealtime(waitTime);
        StartCoroutine(LoadBranches(nextBranch));

    }

    private void LoadBranchingPoint(Conversation.BranchingPoint branchingPoint)
    {
        switch (branchingPoint.type)
        {
            case "choice":
                StartCoroutine(LoadChoiceBranching((branchingPoint.possibilities)));
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



                StartCoroutine(EndConversation());
                break;
            default:
                Debug.LogError("Type de branching Point Inconnue");
                break;
        }
    }



    private IEnumerator EndConversation()
    {
        StartCoroutine(MoveFooter(true));
        Instantiate(medium.nextConvoButton, footer.transform);
        yield return new WaitWhile(() => animationOn);
        scrollRect.enabled = true;

        saveManager.SaveGame(conversation.id, branchList, gameManager.charactersSet);

        yield return new WaitUntil(() => endConversation);

        gameManager.nextConversation = conversation.nextConversation;
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
