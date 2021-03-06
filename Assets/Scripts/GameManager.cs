using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public string firstConversation;
    private string branchToLoad = null;
    private JsonUnloader jsonUnloader = new JsonUnloader();

    private SaveManager saveManager = new SaveManager();

    public List<Conversation> conversations { get; set; } = new List<Conversation>();
    static public List<Character> charactersSet { get; set; } = new List<Character>();
    public static End end;

    [HideInInspector]
    public ConversationDisplayer conversationDisplayer { get; set; }
    [HideInInspector]
    public string nextConversation { get; set; }

    private List<Conversation.Message> messageList = new List<Conversation.Message>() ;
    [HideInInspector]
    public static float soundEffectVolume = 0.5f;
    [HideInInspector]
    public static float musicVolume = 0.5f;
    [SerializeField]
    private float transitionTime;
    private bool inTransition = false;

    private AudioSource musicSource;
    [SerializeField]
    private AudioClip transitionSound;
    [SerializeField]
    private AudioClip endMusic;

    public static GameManager instance;
    [HideInInspector]
    public bool tutorialPlayed = false;

    private void Awake()
    {
        // Singleton
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }

        musicSource = GameObject.Find("MusicLoader").GetComponent<AudioSource>();
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        BetterStreamingAssets.Initialize();
        GetAllConversation();
        GetCharacterSet();
        GetEnd();
    }

    public void StartNewGame()
    {
        GetCharacterSet();
        GetAllConversation();
        saveManager.SaveGame(firstConversation, new List<Conversation.Message>(), charactersSet, null);
        tutorialPlayed = false;
        StartCoroutine(StartGame());
    }

    public void ContinueGame()
    {
        tutorialPlayed = true;
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        StartCoroutine(MakeTransitionBetweenConv());
        yield return new WaitWhile(() => inTransition);

        SceneManager.LoadScene("ConversationScene");
        yield return new WaitUntil(() => conversationDisplayer != null);

        firstConversation = saveManager.LoadSave().currentConversation;
        messageList = saveManager.LoadSave().currentMessageList;
        branchToLoad = saveManager.LoadSave().currentBranch;

        StartCoroutine(conversationDisplayer.LaunchAConv(FindConvById(firstConversation), messageList, branchToLoad));
        StartCoroutine(WaitToLaunchNextConversation());
    }

    static public Character GetCharacterByID(string id)
    {
        Character foundCharacter = null;

        foreach (var character in charactersSet)
        {
            if(character.id == id)
            {
                foundCharacter = character;
            }
        }

        return foundCharacter;
    }

    IEnumerator MakeTransitionBetweenConv()
    {
        inTransition = true;
        SceneManager.LoadScene("TransitionScene");
        yield return new WaitForSecondsRealtime(1);

        ChangeMusic(transitionSound, 0.5f);

        TransitionText transitionText = GameObject.Find("TransitionText").GetComponent<TransitionText>();
        Conversation convToLoad;
        Debug.Log(nextConversation);
        if(nextConversation != null)
        {
            convToLoad = FindConvById(nextConversation);
        }
        else
        {
            convToLoad = FindConvById(firstConversation);
        }
        Debug.Log(convToLoad.id);

        string date = GetDateAndTimeToDisplay(convToLoad.date, convToLoad.time);
        string character1 = FirstLetterToUpper(GetCharacterByID(convToLoad.playerCharacter).firstName);
        string character2 = FirstLetterToUpper(GetCharacterByID(convToLoad.npCharacter).firstName);
        transitionText.ChangeTransition(date, character1, character2);

        LoadingPanel loadingPanel = GameObject.Find("LoadingPanel").GetComponent<LoadingPanel>();
        StartCoroutine(loadingPanel.Disappear());

        yield return new WaitWhile(() => loadingPanel.isFading);

        if (SaveManager.settings.speechHelp)
        {
            yield return new WaitWhile(() => SpeechController.isReading);
            SpeechController.ReadText(transitionText.gameObject.GetComponent<TMP_Text>().text);
            yield return new WaitWhile(() => SpeechController.isReading);
        }

        yield return new WaitForSecondsRealtime(transitionTime);


        StartCoroutine(loadingPanel.Appear());
        yield return new WaitWhile(() => loadingPanel.isFading);
        inTransition = false;
    }

    IEnumerator WaitToLaunchNextConversation()
    {
        yield return new WaitUntil(() => nextConversation != null);

        if (FindConvById(nextConversation) != null)
        {

            StartCoroutine(MakeTransitionBetweenConv());
            yield return new WaitWhile(() => inTransition);

            SceneManager.LoadScene("ConversationScene");
            yield return new WaitUntil(() => conversationDisplayer != null);

            branchToLoad = null;
            StartCoroutine(conversationDisplayer.LaunchAConv(FindConvById(nextConversation), messageList, branchToLoad));
            nextConversation = null;
            UpdateRelationships();
            StartCoroutine(WaitToLaunchNextConversation());
        }
        else
        {

            ChangeMusic(endMusic, 1f);

            firstConversation = saveManager.LoadSave().currentConversation;
            messageList = saveManager.LoadSave().currentMessageList;
            branchToLoad = saveManager.LoadSave().currentBranch;
            UpdateRelationships();

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
                    Debug.Log("Update relation : Me : "+ character.id + " Them : " + newRelation.them  + " value : " + newRelation.confidenceMeToThem);
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

    private void GetEnd()
    {

        string[] paths = BetterStreamingAssets.GetFiles("end", "*.json", SearchOption.AllDirectories);

        foreach (var path in paths)
        {
            end = jsonUnloader.LoadEndFromJson(path);   
        }


    }

    private void GetCharacterSet()
    {
        string[] paths = BetterStreamingAssets.GetFiles("characters", "*.json", SearchOption.AllDirectories);

        foreach (var path in paths)
        {
            charactersSet = jsonUnloader.LoadCharacterListFromJson(path);
        }
    }

    public static string GetDateAndTimeToDisplay(string date, string time)
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
            int hours = parsedDate.TimeOfDay.Hours;
            int minutes = parsedDate.TimeOfDay.Minutes;

            string hoursShown = hours.ToString();
            string minutesShown = minutes.ToString();
            if(hours < 10)
            {
                hoursShown = "0" + hours.ToString();
            }
            if (minutes < 10)
            {
                minutesShown = "0" + minutes.ToString();
            }
            var timeDay = $"{hoursShown}:{minutesShown} ";

            display = $"{dayName} {dayNumber} {monthName} ? {timeDay}";
        }
        else
        {
            Debug.LogError("Couldn't parse date");
        }


        return display;
    }
    public static string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }


    public void ChangeMusic (AudioClip nextMusic, float animationSpeed)
    {
        StopCoroutine("ChangeMusicCoroutine");
        StartCoroutine(ChangeMusicCoroutine(nextMusic, animationSpeed));
    }
    private IEnumerator ChangeMusicCoroutine (AudioClip nextMusic, float animationSpeed)
    {
        float startVolume = musicSource.volume;
        while (musicSource.volume > 0.01f)
        {
            musicSource.volume -= animationSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        musicSource.volume = 0;

        musicSource.Stop();
        musicSource.PlayOneShot(nextMusic);

        while (musicSource.volume < startVolume)
        {
            musicSource.volume += animationSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        musicSource.volume = startVolume;

        yield return null;
    }

    #region Apply settings to scene
    private const float maxWaitTime = 4f;

    private void OnLevelWasLoaded(int level)
    {
        ApplySettingsToScene();
    }

    public static void ApplySettingsToScene()
    {
        // Read speed
        if(SceneManager.GetActiveScene().name == "ConversationScene")
        {
            ConversationDisplayer displayer = FindObjectOfType<ConversationDisplayer>();
            if (displayer != null)
            {
                displayer.timeBetweenMessage = maxWaitTime - (SaveManager.settings.readSpeed * maxWaitTime);
            }
        }

        // Volumes
        UpdateSourceVolume(SaveManager.settings.volumeMusic, SaveManager.settings.volumeEffects);
    }

    public static void UpdateSourceVolume (float music, float effects)
    {
        AudioSource musicSource = DDOL.instance.GetComponentInChildren<AudioSource>();

        if(musicSource)
        {
            musicVolume  = music;
            musicSource.volume = musicVolume;
        }
        soundEffectVolume = effects;
    }
    #endregion
}
