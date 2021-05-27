using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class JsonUnloader : MonoBehaviour
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
    }

    [System.Serializable]
    class Content
    {
        public string type;
        public string data;

    }

    [System.Serializable]
    class Message
    {
        public bool side;
        public Content content;
        public int sendTime;
    }

    [System.Serializable]
    class Possibility
    {
        public string branch;
        public bool possible;
        public int confidenceMod;
        public Message message;
        public int sendTime;
        public int[] thresholds;

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

    //Fonction retournant un objet de classe Conversation à partir d'un chemin vers un .json

    public Conversation LoadConversationFromJson(string jsonPath)
    {

        Conversation.Message CreateMessage(Message message)
        {
            Conversation.Message newMessage = new Conversation.Message();
            newMessage.side = message.side;
            newMessage.sendTime = message.sendTime;
            Conversation.Content newContent = new Conversation.Content();
            newContent.type = message.content.type;
            newContent.data = message.content.data;
            newMessage.content = newContent;
            return newMessage;
        }

        JsonConversationObject jsonConversation;
        Conversation conversationFinal = new Conversation();

        jsonConversation = JsonUtility.FromJson<JsonConversationObject>(File.ReadAllText(jsonPath));

        conversationFinal.id = jsonConversation.Parameters.id;
        conversationFinal.startingBranch = jsonConversation.Parameters.startingBranch;
        conversationFinal.medium = jsonConversation.Parameters.medium;
        conversationFinal.playerCharacter = jsonConversation.Parameters.playerCharacter;
        conversationFinal.npCharacter = jsonConversation.Parameters.npCharacter;
        conversationFinal.nextConversation = jsonConversation.Parameters.nextConversation;

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
                        newTest.thresholds = possibility.thresholds;
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
}
