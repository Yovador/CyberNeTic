using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string firstConversation;
    private string branchToLoad = null;
    private JsonUnloader jsonUnloader = new JsonUnloader();

    private SaveManager saveManager = new SaveManager();

    public List<Conversation> conversations { get; set; } = new List<Conversation>();
    public List<Character> charactersSet { get; set; } = new List<Character>();

    [HideInInspector]
    public ConversationDisplayer conversationDisplayer { get; set; }
    [HideInInspector]
    public string nextConversation { get; set; }

    private List<Conversation.Message> messageList = new List<Conversation.Message>() ;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        BetterStreamingAssets.Initialize();
        GetAllConversation();
        GetCharacterSet();
        Debug.developerConsoleVisible = true;
    }

    public void StartNewGame()
    {
        saveManager.SaveGame(firstConversation, new List<Conversation.Message>(), charactersSet, null);
        StartCoroutine(StartGame());
    }

    public void ContinueGame()
    {
        StartCoroutine(StartGame());
        firstConversation = saveManager.LoadSave().currentConversation;
        messageList = saveManager.LoadSave().currentMessageList;
        branchToLoad = saveManager.LoadSave().currentBranch;
    }

    IEnumerator StartGame()
    {
        SceneManager.LoadScene("ConversationScene");
        yield return new WaitUntil(() => conversationDisplayer != null);

        conversationDisplayer.LaunchAConv(FindConvById(firstConversation), messageList, branchToLoad);

        StartCoroutine(WaitToLaunchNextConversation());
    }

    IEnumerator WaitToLaunchNextConversation()
    {
        yield return new WaitUntil(() => nextConversation != null);

        SceneManager.LoadScene("TransitionScene");

        yield return new WaitForSecondsRealtime(1);

        SceneManager.LoadScene("ConversationScene");

        yield return new WaitUntil(() => conversationDisplayer != null);

        if (FindConvById(nextConversation) != null)
        {
            branchToLoad = null;
            conversationDisplayer.LaunchAConv(FindConvById(nextConversation), messageList, branchToLoad);
            nextConversation = null;
            UpdateRelationships();
            StartCoroutine(WaitToLaunchNextConversation());
        }
        else
        {
            SceneManager.LoadScene("EndScene");
        }
    }

    private void UpdateRelationships()
    {
        List<JsonUnloader.Relationship> relationships = saveManager.LoadSave().relationshipsList;

        foreach (var character in charactersSet)
        {
            character.relationships = new List<Character.Relationship>();
        }

        foreach (var relationship in relationships)
        {
            foreach (var character in charactersSet)
            {
                if(character.id == relationship.me)
                {
                    Character.Relationship newRelation = new Character.Relationship();
                    newRelation.them = relationship.them;
                    newRelation.confidenceMeToThem = relationship.confidenceMeToThem;
                    character.relationships.Add(newRelation);
                }
            }
        }

    }

    public Conversation FindConvById(string id)
    {
        Conversation conversation = null;

        foreach (var conv in conversations)
        {
            if(conv.id == id)
            {
                conversation = conv;
            }
        }

        return conversation;
    }


    private void GetAllConversation()
    {

        string[] paths = BetterStreamingAssets.GetFiles("conversations", "*.json", SearchOption.AllDirectories);

        foreach (var path in paths)
        {
            conversations.Add(jsonUnloader.LoadConversationFromJson(path));
        }
        conversations[0].DebugLogConversation();


    }

    private void GetCharacterSet()
    {
        string[] paths = BetterStreamingAssets.GetFiles("characters", "*.json", SearchOption.AllDirectories);

        foreach (var path in paths)
        {
            charactersSet = jsonUnloader.LoadCharacterListFromJson(path);
        }
    }


    #region Apply settings to scene
    private const float maxWaitTime = 4f;

    private void OnLevelWasLoaded(int level)
    {
        ApplySettingsToScene();
    }

    public static void ApplySettingsToScene()
    {
        if(SceneManager.GetActiveScene().name == "ConversationScene")
        {
            ConversationDisplayer displayer = FindObjectOfType<ConversationDisplayer>();
            if (displayer != null)
            {
                displayer.waitTime = maxWaitTime - (SaveManager.settings.readSpeed * maxWaitTime);
            }
        }

        UpdateColorBlindFilter(SaveManager.settings.colorBlind);
    }

    public static void UpdateColorBlindFilter(int colorBlindType)
    {
        Wilberforce.Colorblind colorblindScript = Camera.main.GetComponent<Wilberforce.Colorblind>();

        if (colorblindScript != null)
        {
            if(colorBlindType > 0)
            {
                colorblindScript.enabled = true;
                colorblindScript.Type = colorBlindType;
            }else
            {
                colorblindScript.enabled = false;
            }
        }
    }
    #endregion
}
