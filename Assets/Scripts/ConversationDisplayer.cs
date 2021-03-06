using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ConversationDisplayer : MonoBehaviour
{
    [SerializeField]
    private string defaultMedium;
    [SerializeField]
    public float timeBetweenMessage;
    private Medium medium;
    private Conversation conversation;
    private SaveManager saveManager = new SaveManager();
    private GameObject conversationFlux;
    public Character npCharacter;
    public Character playerCharacter;
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
    [HideInInspector]
    public bool footerLoaded = false;
    private bool messageLoaded = false;
    [SerializeField]
    GameObject tutorialPanel;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.conversationDisplayer = this;

    }


    public IEnumerator LaunchAConv(Conversation currentConversation, List<Conversation.Message> messagesList, string branchToLoad)
    {
        conversation = currentConversation;



        currentConversation.DebugLogConversation();
        conversationFlux = GameObject.Find("ConversationFlux");
        medium = LoadMedium(conversation.medium);
        conversation.DebugLogConversation();
        scrollRect = GetComponentInChildren<ScrollRect>();
        scrollRect.enabled = false; 
        scrollTransform = scrollRect.gameObject.GetComponent<RectTransform>();

        loadPanel = GameObject.Find("LoadingPanel").GetComponent<LoadingPanel>();


        GameObject dateAndHour = Instantiate(medium.dateAndHour, conversationFlux.transform);
        dateAndHour.transform.SetParent(conversationFlux.transform);

        if (saveManager.LoadSave().currentConversation != conversation.id)
        {
            saveManager.SaveGame(conversation.id, currentMessageList, GameManager.charactersSet, currentBranch);
        }

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

        npCharacter = GameManager.GetCharacterByID(conversation.npCharacter);
        playerCharacter = GameManager.GetCharacterByID(conversation.playerCharacter);

        yield return new WaitUntil(() => footerLoaded);


        scrollTransform.sizeDelta += new Vector2(0, ((3 * Screen.height)/100) + footerController.footerHeigth + Mathf.Abs(medium.navBar.GetComponent<RectTransform>().rect.y * 2));



        scrollTransform.sizeDelta += new Vector2(0, dateAndHour.GetComponent<RectTransform>().sizeDelta.y + screenSensitiveSpaceBetweenMessage);



        dateAndHour.GetComponentInChildren<TMP_Text>().text = GameManager.GetDateAndTimeToDisplay(conversation.date, conversation.time);





        yield return new WaitUntil(() => messageLoaded);

        StartCoroutine(LoadBranches(GetBrancheByID(conversation.startingBranch)));


        foreach (var relationship in npCharacter.relationships)
        {
            if (relationship.them == playerCharacter.id)
            {
                npcToPlayerRelationhship = relationship;
            }
        }

        saveManager.SaveGame(conversation.id, currentMessageList, GameManager.charactersSet, currentBranch);

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

        Debug.Log("medium post : " + medium);


        Instantiate(medium.background, transform).transform.SetSiblingIndex(0);
        Instantiate(medium.navBar, transform);
        footer = Instantiate(medium.footer, transform);
        footerController = footer.GetComponent<FooterController>();
        footerController.conversationDisplayer = this;

        screenSensitiveSpaceBetweenMessage = (medium.spaceBetweenMessages * Screen.height) / 100;

        // Change music
        GameManager.instance.ChangeMusic(medium.musicClip, 0.22f);

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

        if (SaveManager.settings.speechHelp)
        {
            if (!(message.content is ImageContent))
            {
                yield return new WaitWhile(() => SpeechController.isReading);
                if (message.isNpc)
                {
                    SpeechController.ReadText($" {npCharacter.firstName} a envoy? : {message.content.data}");
                }
                else
                {
                    SpeechController.ReadText($" vous avez envoy? : {message.content.data}");
                }
                yield return new WaitWhile(() => SpeechController.isReading);
            }
            else
            {
                yield return new WaitWhile(() => SpeechController.isReading);
                if (message.isNpc)
                {
                    SpeechController.ReadText($" {npCharacter.firstName} a envoy? : une image");
                }
                else
                {
                    SpeechController.ReadText($" vous avez envoy? : une image");
                }
                yield return new WaitWhile(() => SpeechController.isReading);
            }

        }

        GameObject messageBox = Instantiate(messageBoxPrefab, transform.Find("Scroll") );
        GameObject backgroungMessage = messageBox.transform.Find("Background").gameObject;
        TextBoxResizer messageBoxResizer = backgroungMessage.GetComponent<TextBoxResizer>();
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
        }


        LayoutGroup rectTransform = messageBox.GetComponent<LayoutGroup>();
        yield return new WaitUntil(() => rectTransform.preferredHeight != 0);
        
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
            if (SaveManager.settings.speechHelp)
            {
                yield return new WaitWhile(() => SpeechController.isReading);
            }
            yield return new WaitWhile(() => animationOn);
            tempMessageList.Add(message);
            StartCoroutine(LoadMessage(message));
            yield return new WaitForSecondsRealtime(timeBetweenMessage);
            yield return new WaitWhile(() => animationOn);


        }

        if(currentMessageList.Count == 0)
        {
            currentMessageList.AddRange(tempMessageList);
        }
        LoadBranchingPoint(branche.branchingPoint);
        if (currentMessageList.Count > 0)
        {
            yield return new WaitUntil(() => canAddMessageOfPreviousBranch);
            currentMessageList.AddRange(tempMessageList);
        }

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
        if (SaveManager.settings.speechHelp)
            yield return new WaitWhile(() => SpeechController.isReading);
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

        // Send button's animation
        GameObject.FindGameObjectWithTag("SendButtonSprite").GetComponent<Animator>().SetBool("canInteract", isMoveUp);

    }
    private IEnumerator LoadChoiceBranching(List<Conversation.Possibility> choicePossibilities)
    {



        GameObject currentTutorialPanel = null;
        if (!gameManager.tutorialPlayed)
        {

            if (SaveManager.settings.speechHelp)
            {
                yield return new WaitWhile(() => SpeechController.isReading);
            }

            gameManager.tutorialPlayed = true;
            currentTutorialPanel = Instantiate(tutorialPanel, transform);


            if (SaveManager.settings.speechHelp)
            {
                TMP_Text[] textChildren = currentTutorialPanel.transform.Find("Background").GetComponentsInChildren<TMP_Text>();
                foreach (var txt in textChildren)
                {
                    SpeechController.ReadText(txt.text);
                    yield return new WaitWhile(() => SpeechController.isReading);
                }
                SpeechController.ReadText(currentTutorialPanel.GetComponentInChildren< TMP_Text>().text);


                yield return new WaitWhile(() => SpeechController.isReading);
            }

        }

        yield return null;
        Conversation.Branche branch = GetBrancheByID(choicePossibilities[0].branch);

        StartCoroutine(MoveFooter(true));

        Dictionary<GameObject, Conversation.ChoicePossibility> buttonList = new Dictionary<GameObject, Conversation.ChoicePossibility>();
        List<string> buttonsTexts = new List<string>();

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
            buttonsTexts.Add(poss.message.content.data);

            if (poss.possible)
            {
                newButton.GetComponent<ChoiceButton>().branche = GetBrancheByID(poss.branch);
            }
            newButton.name = "choiceButton" + i;
            buttonList.Add(newButton, poss);
        }
        List<GameObject> buttonGameObjects = new List<GameObject>(buttonList.Keys);
        footerController.DisplayChoices(buttonGameObjects);

        if (SaveManager.settings.speechHelp)
        {
            SpeechController.ReadText("Que souhaitez - vous r?pondre ?");

            for (int i = 0; i < choicePossibilities.Count; i++)
            {
                yield return new WaitWhile(() => SpeechController.isReading);

                SpeechController.ReadText($"Choix num?ro : {(i + 1)}.");
                yield return new WaitWhile(() => SpeechController.isReading);
                yield return new WaitForSeconds(0.1f);
                SpeechController.ReadText($"{buttonsTexts[i]}");

                yield return new WaitWhile(() => SpeechController.isReading);
            }

        }

        yield return new WaitWhile(() => animationOn);

        if(currentTutorialPanel != null)
        {
            if(Input.touchSupported)
            {
                bool isTouching = false;
                while (!isTouching)
                {
                    if (Input.touchCount == 1)
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Began)
                        {
                            isTouching = true;
                        }
                    }

                    yield return null;
                }
            }
            else
            {
                while (!Input.GetKeyDown(KeyCode.Mouse0))
                {
                    yield return null;
                }
            }

            currentTutorialPanel.SetActive(false);

            yield return new WaitWhile(() => currentTutorialPanel.activeSelf);
        }

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
                    scrollTransform.anchoredPosition3D -= new Vector3(0, scrollTransform.anchoredPosition3D.y, 0);


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



        currentMessageList.Add(messageToAdd);
        yield return new WaitWhile(() => animationOn);
        yield return new WaitForSecondsRealtime(timeBetweenMessage);
        StartCoroutine(LoadBranches(nextBranch));
        yield return new WaitWhile(() => currentMessageList.Count == 0);
        saveManager.SaveGame(conversation.id, currentMessageList, GameManager.charactersSet, currentBranch);


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
                foreach (Conversation.TestPossibility poss in branchingPoint.possibilities)
                {
                    if (poss.isDefault)
                    {
                        nextBranch = GetBrancheByID(poss.branch);
                    }
                }

                List<Conversation.TestPossibility> testPossToCheck = new List<Conversation.TestPossibility>();

                foreach (Conversation.TestPossibility poss in branchingPoint.possibilities)
                {
                    Debug.Log($"Looking at : branch: {poss.branch} / isDefault: {poss.isDefault} / threshold: {poss.threshold} / checkIfSup: {poss.checkIfSup}" );
                    if (!poss.isDefault)
                    {
                        if(npcToPlayerRelationhship.confidenceMeToThem == 0)
                        {
                            testPossToCheck.Add(poss);
                        }
                        if (npcToPlayerRelationhship.confidenceMeToThem > 0)
                        {
                            if (poss.checkIfSup)
                            {
                                testPossToCheck.Add(poss);
                            }
                        }
                        else if (npcToPlayerRelationhship.confidenceMeToThem < 0)
                        {
                            if (!poss.checkIfSup)
                            {
                                testPossToCheck.Add(poss);
                            }
                        }
                    }
                }

                if(npcToPlayerRelationhship.confidenceMeToThem >= 0)
                {
                    testPossToCheck = testPossToCheck.OrderByDescending(w => w.threshold).ToList();
                }
                else if (npcToPlayerRelationhship.confidenceMeToThem <= 0)
                {
                    testPossToCheck = testPossToCheck.OrderBy(w => w.threshold).ToList();

                }

                foreach (var poss in testPossToCheck)
                {

                    if (poss.checkIfSup)
                    {
                        if(npcToPlayerRelationhship.confidenceMeToThem >= poss.threshold)
                        {
                            nextBranch = GetBrancheByID( poss.branch );
                            break;
                        }
                    }
                    if (!poss.checkIfSup)
                    {
                        if (npcToPlayerRelationhship.confidenceMeToThem <= poss.threshold)
                        {
                            nextBranch = GetBrancheByID(poss.branch);
                            break;
                        }
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
        StartCoroutine(loadPanel.Appear());

        yield return new WaitWhile(() => loadPanel.isFading);


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
