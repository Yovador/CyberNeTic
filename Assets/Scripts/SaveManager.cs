using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager
{

    #region Game save
    [System.Serializable]
    public class Save
    {
        public string currentConversation { get; set; }
        public List<string> currentConversationBranches { get; set; } = new List<string>();
        public List<JsonUnloader.Relationship> relationshipsList { get; set; } = new List<JsonUnloader.Relationship>();

    }

    public void SaveGame(string currentConversation, List<string> currentConversationBranches, List<Character> charactersList)
    {
        Debug.Log("Saving to : " + Application.persistentDataPath);
        string savePath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "gamesave.save"));
        Save newSave = new Save();
        newSave.currentConversation = currentConversation;
        newSave.currentConversationBranches = currentConversationBranches;
        newSave.relationshipsList = new List<JsonUnloader.Relationship>();
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
        string savePath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "gamesave.save"));
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
        public float volume = 0.5f;
        public bool easyMode = false;
    }
    public static PlayerSettings settings = new PlayerSettings();

    public static void SaveSettings()
    {
        PlayerPrefs.SetFloat("volume", settings.volume);
        PlayerPrefs.SetInt("easyMode", settings.easyMode ? 1 : 0);
    }

    public static void LoadSettings()
    {
        settings.volume = PlayerPrefs.GetFloat("volume", 0.5f);
        settings.easyMode = PlayerPrefs.GetInt("easyMode", 0) > 0;
    }

    #endregion
}
