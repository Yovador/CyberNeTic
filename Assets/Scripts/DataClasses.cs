using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Ce fichier contient toutes les classes stockant des données à propos de l'histoire


//Classe contenant les informations d'une conversation
[System.Serializable]
public class Content
{
    public string data { get; set; }

}
[System.Serializable]
public class ImageContent : Content
{
    public string GetPath()
    {
        string path;


        path = $"/pictures/{this.data}";

        return path;
    }

    public Sprite LoadNewSprite(float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Sprite NewSprite;
        Texture2D SpriteTexture = LoadTexture(GetPath());
        NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

        return NewSprite;
    }

    private Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData = JsonUnloader.LoadFromStreamingAssets(FilePath);


        if (FileData != null)
        {
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }
}

public class Conversation
{
    [System.Serializable]
    public class Message
    {
        public bool isNpc{ get; set; }
        public Content content{ get; set; }
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
    }

    public class TestPossibility : Possibility
    {
        public int threshold{ get; set; }
        public bool isDefault { get; set; }
        public bool checkIfSup { get; set; }
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
    public string date{ get; set; }
    public string time{ get; set; }

    public List<Branche> branches { get; set; } = new List<Branche>();

    //Fonction affichant dans la console les informations de la conversation
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
                logString += $"\t \t \tisNPC: {message.isNpc}, type : {message.content.GetType()}, data : {message.content.data}\n ";
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
                        logString += $"\t \t \t \tisNPC: {choicePoss.message.isNpc}, type : {choicePoss.message.content.GetType()}, data : {choicePoss.message.content.data}\n ";

                        break;
                    case "test":
                        var testPoss = (TestPossibility)possibility;
                        logString += $"threshold : {testPoss.threshold}, isDefault : {testPoss.isDefault}, checkIfSup : {testPoss.checkIfSup}  \n";
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
        public string them { get; set; }
        public int confidenceMeToThem { get; set; }
    }

    public string id {get; set;}
    public ImageContent profilePicture { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public List<Relationship> relationships { get; set; } = new List<Relationship>();

    //Fonction affichant dans la console les informations du personnage
    public void DebugLogCharacter()
    {
        string logData = $"Character : \n";

        logData += $"\t id : {this.id}, pfp: {this.profilePicture}, firstName: {this.firstName}, lastName: {this.lastName}\n";

        logData += $"\t Relationship : \n";

        foreach (var relationship in this.relationships)
        {
            logData += $"\t\t them : {relationship.them}, confidenceMeToThem : {relationship.confidenceMeToThem}\n";
        }

        Debug.Log(logData);
    }

}

public class End
{
    public class Branche
    {
        public class Test
        {
            public class Result
            {
                public int value {get; set;}
                public List<string> text { get; set; } = new List<string>();
            }

            public string characterTo { get; set; }
            public string characterFrom { get; set; }
            public Result bad { get; set; }
            public Result neutral { get; set; }
            public Result good { get; set; }
        }
        public List<string> text { get; set; } = new List<string>();
        public Test test { get; set; }
    }

    public List<Branche> branches;

    public void DebugLogEnd()
    {
        string logString = "";

        logString += "End : \n";

        foreach (var branche in branches)
        {
            logString += "\t Branch : \n";
            logString += "\t\t Text : \n";
            foreach (var text in branche.text)
            {
                logString += "\t\t\t";

                logString += $" text : {text} ";

                logString += "\n";
            }

            logString += "\t\t Test : \n";
            logString += $"\t\t\tCharacterTo :{branche.test.characterTo}\n";
            logString += $"\t\t\tCharacterFrom :{branche.test.characterFrom}\n";

            logString += $"\t\t\tBad :\n";
            logString += $"\t\t\t\tvalue :{branche.test.bad.value}\n";
            logString += $"\t\t\t\ttext :\n";
            foreach (var text in branche.test.bad.text)
            {
                logString += "\t\t\t\t";

                logString += $"{text} ";

                logString += "\n";
            }

            logString += $"\t\t\tNeutral :\n";
            logString += $"\t\t\t\tvalue :{branche.test.neutral.value}\n";
            logString += $"\t\t\t\ttext :\n";
            foreach (var text in branche.test.neutral.text)
            {
                logString += "\t\t\t\t";

                logString += $"{text} ";

                logString += "\n";
            }

            logString += $"\t\t\tGood :\n";
            logString += $"\t\t\t\tvalue :{branche.test.good.value}\n";
            logString += $"\t\t\t\ttext :\n";
            foreach (var text in branche.test.good.text)
            {
                logString += "\t\t\t\t";

                logString += $"{text} ";

                logString += "\n";
            }


        }
        
        Debug.Log(logString);

    }

}




