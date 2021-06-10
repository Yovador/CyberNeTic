using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager
{

    #region Game save
    private string saveName = $"gamedata1.save";
    [System.Serializable]
    public class Save
    {
        public string currentConversation { get; set; }
        public string currentBranch { get; set; }
        public List<Conversation.Message> currentMessageList { get; set; } = new List<Conversation.Message>();
        public List<JsonUnloader.Relationship> relationshipsList { get; set; } = new List<JsonUnloader.Relationship>();

    }

    public void SaveGame(string currentConversation, List<Conversation.Message> currentMessageList, List<Character> charactersList, string currentBranch)
    {
        Debug.Log("Saving to : " + Application.persistentDataPath);
        string savePath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, saveName));
        Save newSave = new Save();
        newSave.currentConversation = currentConversation;
        newSave.currentMessageList = currentMessageList;
        newSave.relationshipsList = new List<JsonUnloader.Relationship>();
        newSave.currentBranch = currentBranch;
        foreach (var character in charactersList)
        {
            foreach (var relationship in character.relationships)
            {
                JsonUnloader.Relationship newRelationship = new JsonUnloader.Relationship();
                newRelationship.me = character.id;
                newRelationship.them = relationship.them;
                newRelationship.confidenceMeToThem = relationship.confidenceMeToThem;
                newSave.relationshipsList.Add(newRelationship);
            }
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, newSave);
        file.Close();

    }

    public Save LoadSave()
    {
        string savePath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, saveName));
        Save save = null;
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            save = (Save)bf.Deserialize(file);
            file.Close();
        }
        return save;
    }

    #endregion

    #region Settings save

    public static bool sessionGameStarted = false;

    public class PlayerSettings
    {
        public float volumeEffects = 0.5f;
        public float volumeMusic = 0.5f;
        public float readSpeed = 0.5f;
        public bool colorBlind = false;
    }
    public static PlayerSettings settings = new PlayerSettings();

    public static void SaveSettings()
    {
        PlayerPrefs.SetFloat("volumeEffects", settings.volumeEffects);
        PlayerPrefs.SetFloat("volumeMusic", settings.volumeMusic);
        PlayerPrefs.SetFloat("readSpeed", settings.readSpeed);
        PlayerPrefs.SetInt("colorBlind", settings.colorBlind ? 1 : 0);
    }

    public static void LoadSettings()
    {
        settings.volumeEffects = PlayerPrefs.GetFloat("volumeEffects", 0.5f);
        settings.volumeMusic = PlayerPrefs.GetFloat("volumeMusic", 0.5f);
        settings.readSpeed = PlayerPrefs.GetFloat("readSpeed", 0.5f);
        settings.colorBlind = PlayerPrefs.GetInt("colorBlind", 0) > 0;
    }

    #endregion
}
