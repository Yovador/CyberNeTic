using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string firstConversation;
    private string pathConversation = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "Conversations"));
    private string pathCharacterSet = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "Characters"));
    private JsonUnloader jsonUnloader = new JsonUnloader();
    private SaveManager saveManager = new SaveManager();


    public List<Conversation> conversations { get; set; } = new List<Conversation>();
    public List<Character> charactersSet { get; set; } = new List<Character>();

    [HideInInspector]
    public Conversation currentConversation { get; set; }
    [HideInInspector]
    public ConversationDisplayer conversationDisplayer { get; set; }
    [HideInInspector]
    public string nextConversation { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        GetAllConversation();
        GetCharacterSet();
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitUntil(() => conversationDisplayer != null);

        conversationDisplayer.LaunchAConv(FindConvById(firstConversation));

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
            conversationDisplayer.LaunchAConv(FindConvById(nextConversation));
            nextConversation = null;
            StartCoroutine(WaitToLaunchNextConversation());
        }
        else
        {
            SceneManager.LoadScene("EndScene");
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
        var info = new DirectoryInfo(pathConversation);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            if(file.Extension == ".json")
            {
                conversations.Add(jsonUnloader.LoadConversationFromJson(file.FullName));
            }
        }
    }

    private void GetCharacterSet()
    {
        var info = new DirectoryInfo(pathCharacterSet);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            if (file.Extension == ".json")
            {
                charactersSet = jsonUnloader.LoadCharacterListFromJson(file.FullName);
            }
        }
    }
    
}
