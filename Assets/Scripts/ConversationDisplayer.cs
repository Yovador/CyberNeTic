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
    public float waitTime;
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
    public float messageSpeed;

    private ScrollRect scrollRect;
    private RectTransform scrollTransform;

    [HideInInspector]
    public ChoiceButton choiceButton;
    private string currentBranch = null;
    private bool canAddMessageOfPreviousBranch = true;
    private LoadingPanel loadPanel;
    private float screenSensitiveSpaceBetweenMessage;
    private FooterController footerController;
    public bool footerLoaded = false;

    private AudioSource musicSource;
    private bool messageLoaded = false;
    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.conversationDisplayer = this;

        musicSource = GameObject.Find("MusicLoader").GetComponent<AudioSource>();
    }


    public IEnumerator LaunchAConv(Conversation currentConversation, List<Conversation.Message> messagesList, string branchToLoad)
    {
        conversation = currentConversation;
        conversationFlux = GameObject.Find("ConversationFlux");
        medium = LoadMedium(conversation.medium);
        conversation.DebugLogConversation();
        scrollRect = GetComponentInChildren<ScrollRect>();
        scrollRect.enabled = false; 
        scrollTransform = scrollRect.gameObject.GetComponent<RectTransform>();

        loadPanel = GameObject.Find("LoadingPanel").GetComponent<LoadingPanel>();


        GameObject dateAndHour = Instantiate(medium.dateAndHour, conversationFlux.transform);
        dateAndHour.transform.SetParent(conversationFlux.transform);

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
            messageLoaded = true;
        }

        foreach (var character in gameManager.charactersSet)
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

        yield return new WaitUntil(() => footerLoaded);

        scrollTransform.sizeDelta = new Vector2(0, ((3 * Screen.height)/100) + footerController.footerHeigth + Mathf.Abs(medium.navBar.GetComponent<RectTransform>().rect.y * 2));


        scrollTransform.sizeDelta += new Vector2(0, dateAndHour.GetComponent<RectTransform>().sizeDelta.y + screenSensitiveSpaceBetweenMessage);

        dateAndHour.GetComponentInChildren<TMP_Text>().text = GetDateAndTimeToDisplay(conversation.date, conversation.time);





        yield return new WaitUntil(() => messageLoaded);

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
        float previousMessageVolume = GameManager.soundEffectVolume;
        GameManager.soundEffectVolume = 0;
        foreach (var message in messagesToLoad)
        {
            yield return new WaitWhile(() => animationOn);
            StartCoroutine(LoadMessage(message));
            yield return new WaitWhile(() => animationOn);

        }
        yield return new WaitForSecondsRealtime(0.2f);
        messageSpeed = previousMessageSpeed;
        GameManager.soundEffectVolume = previousMessageVolume;
        StartCoroutine(loadPanel.Disappear());
        messageLoaded = true;
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
        footerController = footer.GetComponent<FooterController>();
        footerController.conversationDisplayer = this;

        screenSensitiveSpaceBetweenMessage = (medium.spaceBetweenMessages * Screen.height) / 100;

        musicSource.Stop();
        musicSource.PlayOneShot(medium.musicClip);

        return medium;
    }
    private IEnumerator LoadMessage(Conversation.Message message)
    {
        GameObject messageBoxPrefab;

        if(SaveManager.settings.speechHelp)
            SpeechController.StopReading();

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
        messageBox.GetComponent<AudioSource>().volume = GameManager.soundEffectVolume;

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
            textComponent.text = message.content.data;
            messageBoxResizer.ResizeBox();

            if (SaveManager.settings.speechHelp)
                SpeechController.ReadText(message.content.data);
        }


        LayoutGroup rectTransform = messageBox.GetComponent<LayoutGroup>();
        yield return null;
        
        float heigth = rectTransform.preferredHeight;
        float size = screenSensitiveSpaceBetweenMessage + heigth;
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
        animationOn = true;
        Vector2 newPos = new Vector2(conversationFlux.transform.localPosition.x, conversationFlux.transform.localPosition.y +  value);


        while (Vector2.Distance(conversationFlux.transform.localPosition, newPos) > 0.5f)
        {
            conversationFlux.transform.localPosition = Vector2.Lerp(conversationFlux.transform.localPosition, newPos, messageSpeed * Time.deltaTime);
            yield return null;
        }
        conversationFlux.transform.localPosition = newPos;

        animationOn = false;


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
            float upValue = footerController.footerHeigth - footerController.keyboardBarHeigth ;
            StartCoroutine(MoveFlux(upValue));
            newPos = rectTransform.localPosition + new Vector3(0, upValue, 0);

        }
        else
        {
            float downValue = -(footerController.footerHeigth - footerController.keyboardBarHeigth);

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
        yield return null;
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

            newButton.GetComponentInChildren<TMP_Text>().text = poss.message.content.data;
            if (poss.possible)
            {
                newButton.GetComponent<ChoiceButton>().branche = GetBrancheByID(poss.branch);
            }
            newButton.name = "choiceButton" + i;
            buttonList.Add(newButton, poss);
        }

        List<GameObject> buttonGameObjects = new List<GameObject>(buttonList.Keys);
        footerController.DisplayChoices(buttonGameObjects);

        yield return new WaitWhile(() => animationOn);
        scrollRect.enabled = true;


        yield return new WaitWhile(() => isInChoice);

        scrollRect.enabled = false;
        StartCoroutine(MoveFooter(false));
        foreach (var newButton in buttonList)
        {
            newButton.Key.SetActive(false);
        }
        yield return new WaitWhile(() => animationOn);

        Conversation.Message messageToAdd = null; 
        foreach (var newButton in buttonList)
        {
            ChoiceButton choiceButtonSelected = newButton.Key.GetComponent<ChoiceButton>();
            if(choiceButtonSelected != null)
            {
                if (choiceButtonSelected == choiceButton)
                {
                    messageToAdd = newButton.Value.message;
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
        yield return new WaitWhile(() => currentMessageList.Count == 0);
        currentMessageList.Add(messageToAdd);
        saveManager.SaveGame(conversation.id, currentMessageList, gameManager.charactersSet, currentBranch);

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
        List<GameObject> footerButtons = new List<GameObject>();
        footerButtons.Add(Instantiate(medium.nextConvoButton, footer.transform));

        footerController.DisplayChoices(footerButtons);

        yield return new WaitWhile(() => animationOn);
        scrollRect.enabled = true;


        yield return new WaitUntil(() => endConversation);

        currentMessageList = new List<Conversation.Message>();
        Debug.Log("StartFade:");
        StartCoroutine(loadPanel.Appear());

        yield return new WaitWhile(() => loadPanel.isFading);

        Debug.Log("AfterFade: ");

        saveManager.SaveGame(conversation.id, currentMessageList, gameManager.charactersSet, currentBranch);
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
