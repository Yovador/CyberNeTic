using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class JsonUnloader
{
    //Classes permettant l'extraction du fichier JSON
    [System.Serializable]
    class Parameters
    {
        public string id;
        public string startingBranch;
        public string medium;
        public string playerCharacter;
        public string npCharacter;
        public string nextConversation;
        public string date;
        public string time;
    }

    [System.Serializable]
    class ContentJSON
    {
        public string type;
        public string data;

    }

    [System.Serializable]
    class Message
    {
        public bool isNpc;
        public ContentJSON content;
    }

    [System.Serializable]
    class Possibility
    {
        public string branch;
        public bool possible;
        public int confidenceMod;
        public Message message;
        public int threshold;
        public bool isDefault;
        public bool checkIfSup;

    }

    [System.Serializable]
    class BranchingPoint
    {
        public string type;
        public Possibility[] possibilities;

    }

    [System.Serializable]
    class Branche
    {
        public string id;
        public Message[] messagesList;

        public BranchingPoint branchingPoint;


    }

    [System.Serializable]
    class JsonConversationObject
    {
        public Parameters Parameters;
        public Branche[] Branches;

    }

    [System.Serializable]
    class CharacterJson
    {
        public string id;
        public string profilePicture;
        public string firstName;
        public string lastName;

    }

    [System.Serializable]
    public class Relationship
    {
        public string me;
        public string them;
        public int confidenceMeToThem;

    }

    [System.Serializable]
    class JsonCharacterSetObject
    {
        public CharacterJson[] Characters; 
        public Relationship[] Relationships;
    }
    [System.Serializable]
    class EndTestContent
    {
        public int value;
        public string[] text;
    }

    [System.Serializable]
    class EndTest
    {
        public string characterTo;
        public string characterFrom;
        public EndTestContent bad;
        public EndTestContent neutral;
        public EndTestContent good;
    }

    [System.Serializable]
    class EndBranche
    {
        public string[] text;
        public EndTest test;
    }


    [System.Serializable]
    class EndJson
    {
        public EndBranche[] endBranches;
    }

    //Fonction retournant un objet de classe Conversation ? partir d'un chemin vers un .json
    public Conversation LoadConversationFromJson(string jsonPath)
    {

        Conversation.Message CreateMessage(Message message)
        {
            Conversation.Message newMessage = new Conversation.Message();
            newMessage.isNpc = message.isNpc;
            Content newContent;
            switch (message.content.type)
            {
                case "image":
                    newContent = new ImageContent();
                    break;
                default:
                    newContent = new Content();
                    break;
            }
            newContent.data = message.content.data;
            newMessage.content = newContent;
            return newMessage;
        }

        JsonConversationObject jsonConversation;
        Conversation conversationFinal = new Conversation();

        jsonConversation = JsonUtility.FromJson<JsonConversationObject>(Encoding.Default.GetString(LoadFromStreamingAssets(jsonPath)));

        conversationFinal.id = jsonConversation.Parameters.id;
        conversationFinal.startingBranch = jsonConversation.Parameters.startingBranch;
        conversationFinal.medium = jsonConversation.Parameters.medium;
        conversationFinal.playerCharacter = jsonConversation.Parameters.playerCharacter;
        conversationFinal.npCharacter = jsonConversation.Parameters.npCharacter;
        conversationFinal.nextConversation = jsonConversation.Parameters.nextConversation;
        conversationFinal.date = jsonConversation.Parameters.date;
        conversationFinal.time = jsonConversation.Parameters.time;

        foreach (Branche branche in jsonConversation.Branches)
        {
            Conversation.Branche newBranch = new Conversation.Branche();
            newBranch.id = branche.id;
            foreach (Message message in branche.messagesList)
            {
                Conversation.Message newMessage = CreateMessage(message);

                newBranch.messagesList.Add(newMessage);

            }

            Conversation.BranchingPoint newBranchingPoint = new Conversation.BranchingPoint();
            newBranchingPoint.type = branche.branchingPoint.type;
            foreach (Possibility possibility in branche.branchingPoint.possibilities)
            {
                switch (branche.branchingPoint.type)
                {
                    case "choice":
                        Conversation.ChoicePossibility newChoice = new Conversation.ChoicePossibility();
                        newChoice.branch = possibility.branch;
                        newChoice.possible = possibility.possible;
                        newChoice.confidenceMod = possibility.confidenceMod;
                        newChoice.message = CreateMessage(possibility.message);
                        newBranchingPoint.possibilities.Add(newChoice);


                        break;
                    case "test":
                        Conversation.TestPossibility newTest = new Conversation.TestPossibility();
                        newTest.branch = possibility.branch;
                        newTest.threshold = possibility.threshold;
                        newTest.checkIfSup = possibility.checkIfSup;
                        newTest.isDefault = possibility.isDefault;
                        newBranchingPoint.possibilities.Add(newTest);
                        break;
                    case "change":
                        Conversation.Possibility newChange = new Conversation.Possibility();
                        newChange.branch = possibility.branch;
                        newBranchingPoint.possibilities.Add(newChange);
                        break;
                    default:
                        break;
                }
            }
            newBranch.branchingPoint = newBranchingPoint;

            conversationFinal.branches.Add(newBranch);
        }



        return conversationFinal;
    }

    public List<Character> LoadCharacterListFromJson(string jsonPath)
    {
        List<Character> listFinal = new List<Character>();

        JsonCharacterSetObject jsonCharacterSet = JsonUtility.FromJson<JsonCharacterSetObject>( Encoding.Default.GetString(LoadFromStreamingAssets(jsonPath)) );


        foreach (var characterJson in jsonCharacterSet.Characters)
        {
            Character character = new Character();

            character.id = characterJson.id;
            character.firstName = characterJson.firstName;
            character.lastName = characterJson.lastName;
            character.profilePicture = new ImageContent();
            character.profilePicture.data = characterJson.profilePicture;

            List<Character.Relationship> relationships = new List<Character.Relationship>();

            foreach (var relationshipJson in jsonCharacterSet.Relationships)
            {
                if (relationshipJson.me == character.id)
                {
                    Character.Relationship relationship = new Character.Relationship();
                    relationship.them = relationshipJson.them;
                    relationship.confidenceMeToThem = relationshipJson.confidenceMeToThem;
                    relationships.Add(relationship);
                }
                

            }

            character.relationships = relationships;

            listFinal.Add(character);
        }


        return listFinal;
    }

    public End LoadEndFromJson(string jsonPath)
    {
        EndJson endJson = JsonUtility.FromJson<EndJson>(Encoding.Default.GetString(LoadFromStreamingAssets(jsonPath)));
        End end = new End();
        List<End.Branche> endBranches = new List<End.Branche>();
        foreach (var branche in endJson.endBranches)
        {
            End.Branche newBranch = new End.Branche();
            newBranch.text = new List<string>(branche.text);
            End.Branche.Test test = new End.Branche.Test();
            test.characterFrom = branche.test.characterFrom;
            test.characterTo = branche.test.characterTo;
            End.Branche.Test.Result bad = new End.Branche.Test.Result();
            bad.value = 0;
            bad.text = new List<string>();
            if(branche.test.bad != null)
            {
                bad.value = branche.test.bad.value;
                if(branche.test.bad.text != null)
                {
                    bad.text = new List<string>(branche.test.bad.text);
                }
            }

            End.Branche.Test.Result neutral = new End.Branche.Test.Result();
            neutral.value = 0;
            neutral.text = new List<string>();
            if (branche.test.neutral != null)
            {
                neutral.value = branche.test.neutral.value;
                if (branche.test.neutral.text != null)
                {
                    neutral.text = new List<string>(branche.test.neutral.text);
                }
            }

            End.Branche.Test.Result good = new End.Branche.Test.Result();
            good.value = 0;
            good.text = new List<string>();
            if (branche.test.good != null)
            {
                good.value = branche.test.good.value;
                if (branche.test.good.text != null)
                {
                    good.text = new List<string>(branche.test.good.text);
                }

            }

            test.bad = bad;
            test.neutral = neutral;
            test.good = good;

            newBranch.test = test;

            endBranches.Add(newBranch);
        }
        end.branches = endBranches;

        return end;
       
    }

    static public byte[] LoadFromStreamingAssets(string path)
    {
        byte[] bytes = null;

        bytes = BetterStreamingAssets.ReadAllBytes(path);

        return bytes;
    }
}
