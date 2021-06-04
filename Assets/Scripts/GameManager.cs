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
    public ConversationDisplayer conversationDisplayer { get; set; }
    [HideInInspector]
    public string nextConversation { get; set; }

    private List<string> branchList = new List<string>() ;


    // Start is called before the first frame update
    void Start()
    {
        GetAllConversation();
        GetCharacterSet();
    }



    public void StartNewGame()
    {
        saveManager.SaveGame(firstConversation, new List<string>(), charactersSet);
        StartCoroutine(StartGame());
    }

    public void ContinueGame()
    {
        StartCoroutine(StartGame());
        firstConversation = saveManager.LoadSave().currentConversation;
        branchList = saveManager.LoadSave().currentConversationBranches;

    }

    IEnumerator StartGame()
    {
        SceneManager.LoadScene("ConversationScene");

        yield return new WaitUntil(() => conversationDisplayer != null);

        conversationDisplayer.LaunchAConv(FindConvById(firstConversation), branchList);



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
            conversationDisplayer.LaunchAConv(FindConvById(nextConversation), branchList);
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
