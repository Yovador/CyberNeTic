using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceButton : ConversationButtons
{
    [HideInInspector]
    public Conversation.Branche branche { get; set; }
    private TMP_Text textInput;
    private TMP_Text myText;
    string defaultTxt;

    protected override void Start()
    {
        base.Start();

        textInput = GameObject.Find("InputTxt").GetComponent<TMP_Text>();
        defaultTxt = textInput.text;
        myText = GetComponentInChildren<TMP_Text>();
    }

    protected override void OnHold()
    {
        base.OnHold();

        int numberOfLetter = Mathf.RoundToInt((myText.text.Length * waitIncrement) / waitLimit);
        if (numberOfLetter < myText.text.Length)
        {
            string tempString = "" + myText.text[0];

            for (int i = 1; i < numberOfLetter; i++)
            {
                tempString += myText.text[i];
            }

            textInput.text = tempString;
        }

    }

    public override void OnPointerUp()
    {
        base.OnPointerUp();
        if (waitIncrement < waitLimit)
        {
            textInput.text = defaultTxt;
        }
    }

    protected override IEnumerator ButtonAction()
    {
        StartCoroutine(base.ButtonAction());

        isHold = false;
        GameObject.FindGameObjectWithTag("SendButtonSprite").GetComponent<Animator>().SetTrigger("Active");
        yield return new WaitForSecondsRealtime(0.3f);
        textInput.text = defaultTxt;
        // Send button's animation
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
