using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Ce fichier contient toutes les classes stockant des données à propos de l'histoire


//Classe contenant les informations d'une conversation
public class Conversation
{
    public class Content
    {
        public string type{ get; set; }
        public string data{ get; set; }

    }

    public class Message
    {
        public bool side{ get; set; }
        public Content content{ get; set; }
        public int sendTime{ get; set; }
    }

    public class Possibility
    {
        public string branch{ get; set; }

    }

    public class ChoicePossibility : Possibility
    {
        public bool possible{ get; set; }
        public int confidenceMod{ get; set; }
        public Message message{ get; set; }
        public int sendTime{ get; set; }
    }

    public class TestPossibility : Possibility
    {
        public int[] thresholds{ get; set; }
    }


    public class BranchingPoint
    {
        public string type{ get; set; }
        public List<Possibility> possibilities{ get; set; } = new List<Possibility>();

    }

    public class Branche
    {
        public string id{ get; set; }
        public List<Message> messagesList { get; set; } = new List<Message>();

        public BranchingPoint branchingPoint { get; set; } = new BranchingPoint();
    
    }

    public string id { get; set; }
    public string startingBranch{ get; set; }
    public string medium{ get; set; }
    public string playerCharacter{ get; set; }
    public string npCharacter{ get; set; }
    public string nextConversation{ get; set; }

    public List<Branche> branches { get; set; } = new List<Branche>();

    public void DebugLogConversation()
    {
        string logString = "Conversation : \n";
        logString += $"\t id: {this.id}, startingBranch: {this.startingBranch}, medium: {this.medium}, playerCharacter: {this.playerCharacter}, npCharacter: {this.npCharacter}, nextConversation: {this.nextConversation}\n";
        logString += $"\t Branches : \n";

        foreach (Branche branch in this.branches)
        {
            logString += $"\t \tBranch : {branch.id}\n";
            logString += $"\t \tMessages : \n";
            foreach (Message message in branch.messagesList)
            {
                logString += $"\t \t \tSide: {message.side}, type : {message.content.type}, data : {message.content.data}, sendTime :{message.sendTime}\n ";
            }


            logString += $"\t \tbranchingPoint Type : {branch.branchingPoint.type}\n";
            foreach (var possibility in branch.branchingPoint.possibilities)
            {
                logString += $"\t \t \tBranche suivante : {possibility.branch}, ";
                switch (branch.branchingPoint.type)
                {
                    case "choice":
                        var choicePoss = (ChoicePossibility)possibility;
                        logString += $"Possible ? {choicePoss.possible}, confidenceMod : {choicePoss.possible}, message :\n";
                        logString += $"\t \t \t \tSide: {choicePoss.message.side}, type : {choicePoss.message.content.type}, data : {choicePoss.message.content.data}, sendTime :{choicePoss.message.sendTime}\n ";

                        break;
                    case "test":
                        var testPoss = (TestPossibility)possibility;
                        logString += $"thresholdMin : {testPoss.thresholds[0]}, thresholdMax : {testPoss.thresholds[1]}\n";
                        break;
                    default:
                        logString += "\n";
                        break;
                }

            }
        }

        Debug.Log(logString);
    }

}

//Class contenant les informations d'un personnage
public class Character
{
    public class Relationship
    {
        public string myId { get; set; }
        public string theirId { get; set; }
        public int confidenceMetoThem { get; set; }
    }

    public string id {get; set;}
    public List<Relationship> relationships { get; set; } = new List<Relationship>();
}

//ScriptableObject contenant les informations d'un medium de communication
[CreateAssetMenu(fileName = "New Medium", menuName = "Medium", order = 51)]
public class Medium : ScriptableObject
{
    [SerializeField]
    public string id;
    [SerializeField]
    public GameObject playerMessageBox;

}


