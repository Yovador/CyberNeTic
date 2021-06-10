using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private List<Conversation.Message> currentMessageList = new List<Conversation.Message>();
    private GameManager gameManager;
    [HideInInspector]
    public bool endConversation;
    private bool animationOn = false;

    [SerializeField, Range(0.5f, 5f)]
    private float messageSpeed;

    private ScrollRect scrollRect;
    private RectTransform scrollTransform;

    [HideInInspector]
    public ChoiceButton choiceButton;
    private string currentBranch = null;
    private bool canAddMessageOfPreviousBranch = true;
    private LoadingPanel loadPanel;
    private float screenSensitiveSpaceBetweenMessage;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.conversationDisplayer = this;
    }


    public void LaunchAConv(Conversation currentConversation, List<Conversation.Message> messagesList, string branchToLoad)
    {
        conversation = currentConversation;
        conversationFlux = GameObject.Find("ConversationFlux");
        medium = LoadMedium(conversation.medium);
        conversation.DebugLogConversation();
        scrollRect = GetComponentInChildren<ScrollRect>();
        scrollRect.enabled = false; 
        scrollTransform = scrollRect.gameObject.GetComponent<RectTransform>();
        scrollTransform.sizeDelta = new Vector2(0, 100 + footer.GetComponent<RectTransform>().sizeDelta.y + medium.navBar.GetComponent<RectTransform>().sizeDelta.y);
        loadPanel = GameObject.Find("LoadingPanel").GetComponent<LoadingPanel>();

        if (branchToLoad != null)
        {
            conversation.startingBranch = branchToLoad;
        }

        if(messagesList.Count > 0)
        {
            StartCoroutine(LoadPreviousMessage(messagesList));
        }
        else
        {
            StartCoroutine(loadPanel.Disappear());
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

        GameObject dateAndHour = Instantiate(medium.dateAndHour, conversationFlux.transform);
        dateAndHour.transform.SetParent(conversationFlux.transform);
        scrollTransform.sizeDelta += new Vector2(0, dateAndHour.GetComponent<RectTransform>().sizeDelta.y + medium.spaceBetweenMessages);
        Debug.Log(scrollTransform.sizeDelta.y);
        dateAndHour.GetComponentInChildren<TMP_Text>().text = GetDateAndTimeToDisplay(conversation.date, conversation.time);
        

        StartCoroutine(LoadBranches(GetBrancheByID(conversation.startingBranch)));


        foreach (var relationship in npCharacter.relationships)
        {
            if (relationship.them == playerCharacter.id)
            {
                npcToPlayerRelationhship = relationship;
            }
        }

        // Update header name
        GameObject.FindGameObjectWithTag("ContactName").GetComponent<TMP_Text>().text = $"{npCharacter.firstName} {npCharacter.lastName}" ;

        saveManager.SaveGame(conversation.id, currentMessageList, gameManager.charactersSet, currentBranch);

    }
    private IEnumerator LoadPreviousMessage(List<Conversation.Message> messagesToLoad)
    {
        currentMessageList.AddRange(messagesToLoad);
        float previousMessageSpeed = messageSpeed;
        messageSpeed = 100000000f;
        foreach (var message in messagesToLoad)
        {
            StartCoroutine(LoadMessage(message));
            yield return new WaitWhile(() => animationOn);

        }
        yield return new WaitForSecondsRealtime(0.2f);
        messageSpeed = previousMessageSpeed;
        StartCoroutine(loadPanel.Disappear());
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
        screenSensitiveSpaceBetweenMessage = (medium.spaceBetweenMessages * Screen.height) / 100 ;
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
            TMP_Text textComponent = backgroungMessage.transform.Find("Text").GetComponent<TMP_Text>();
            Debug.Log(textComponent);
            textComponent.text = message.content.data;
            messageBoxResizer.ResizeBox();
        }


        LayoutGroup rectTransform = messageBox.GetComponent<LayoutGroup>();
        yield return null;
        
        float heigth = rectTransform.preferredHeight;
        float size = screenSensitiveSpaceBetweenMessage + heigth;
        //yield return new WaitForSecondsRealtime(100f);
        Debug.Log("expected heigth : " + size);
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
        Debug.Log("Starting pos : " + conversationFlux.transform.localPosition);
        animationOn = true;
        Vector2 newPos = new Vector2(conversationFlux.transform.localPosition.x, conversationFlux.transform.localPosition.y +  value);
        Debug.Log("expected pos : " + newPos);


        while (Vector2.Distance(conversationFlux.transform.localPosition, newPos) > 0.5f)
        {
            conversationFlux.transform.localPosition = Vector2.Lerp(conversationFlux.transform.localPosition, newPos, messageSpeed * Time.deltaTime);
            yield return null;
        }
        conversationFlux.transform.localPosition = newPos;

        animationOn = false;
        Debug.Log("end pos : " + conversationFlux.transform.localPosition);


    }

    private IEnumerator LoadBranches(Conversation.Branche branche )
    {
        canAddMessageOfPreviousBranch = true;
        currentBranch = branche.id;
        List<Conversation.Message> tempMessageList = new List<Conversation.Message>();
        foreach (var message in branche.messagesList)
        {
            yield return new WaitWhile(() => animationOn);
            tempMessageList.Add(message);
            StartCoroutine(LoadMessage(message));
            yield return new WaitForSecondsRealtime(waitTime);
            yield return new WaitWhile(() => animationOn);

        }

        LoadBranchingPoint(branche.branchingPoint);
        yield return new WaitUntil(() => canAddMessageOfPreviousBranch);
        currentMessageList.AddRange(tempMessageList);
        saveManager.SaveGame(conversation.id, currentMessageList, gameManager.charactersSet, currentBranch);

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
            newPos = rectTransform.localPosition + new Vector3(0, upValue, 0);

        }
        else
        {
            float downValue = -(rectTransform.sizeDelta.y - medium.footerHeigth);
            StartCoroutine(MoveFlux(downValue));
            newPos = rectTransform.localPosition + new Vector3(0, downValue, 0);

        }


        while (Vector2.Distance(rectTransform.transform.localPosition, newPos) > 0.5f)
        {
            rectTransform.transform.localPosition = Vector2.Lerp(rectTransform.transform.localPosition, newPos, messageSpeed * Time.deltaTime);
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
            newButton.GetComponentInChildren<TMP_Text>().text = poss.message.content.data;
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
            ChoiceButton choiceButtonSelected = newButton.Key.GetComponent<ChoiceButton>();
            if(choiceButtonSelected != null)
            {
                if (choiceButtonSelected == choiceButton)
                {
                    currentMessageList.Add(newButton.Value.message);
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
        canAddMessageOfPreviousBranch = false;
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

        currentMessageList = new List<Conversation.Message>();
        saveManager.SaveGame(conversation.id, currentMessageList, gameManager.charactersSet, currentBranch);

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

    string GetDateAndTimeToDisplay(string date, string time)
    {


        string pattern = "yyyy-MM-dd HH:mm";
        System.DateTime parsedDate;
        string display = null;

        if (System.DateTime.TryParseExact($"{date} {time}", pattern, null, System.Globalization.DateTimeStyles.None, out parsedDate))
        {
            var culture = new System.Globalization.CultureInfo("fr-FR");
            string dayName = FirstLetterToUpper(culture.DateTimeFormat.GetDayName(parsedDate.DayOfWeek));
            int dayNumber = parsedDate.Day;
            string monthName = culture.DateTimeFormat.GetMonthName(parsedDate.Month);
            var timeDay = $"{parsedDate.TimeOfDay.Hours}:{parsedDate.TimeOfDay.Minutes} "  ;

            display = $"{dayName} {dayNumber} {monthName} • {timeDay}";
        }
        else
        {
            Debug.LogError("Couldn't parse date");
        }


        return display;
        

    }

    public string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }
}
