using UnityEngine;
using TMPro;

public class ChoiceButton : ConversationButtons
{
    [HideInInspector]
    public Conversation.Branche branche { get; set; }

    public override void OnClick()
    {
        conversationDisplayer.isInChoice = false;
        conversationDisplayer.nextBranch = branche;
        conversationDisplayer.choiceButton = this;

        // Read button content
        if (SaveManager.settings.speechHelp)
            SpeechController.ReadText(GetComponentInChildren<TMP_Text>().text);

        // Send button's animation
        GameObject.FindGameObjectWithTag("SendButtonSprite").GetComponent<Animator>().SetTrigger("Active");
    }
}
