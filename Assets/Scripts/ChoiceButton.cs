using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceButton : ConversationButtons
{
    [HideInInspector]
    public Conversation.Branche branche { get; set; }
    private TMP_Text textInput;
    string defaultTxt;

    protected override void Start()
    {
        base.Start();

        textInput = GameObject.Find("InputTxt").GetComponent<TMP_Text>();
        defaultTxt = textInput.text;
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
        else
        {
            textInput.text = myText.text;
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
        if (SaveManager.settings.speechHelp)
            yield return new WaitWhile(() => SpeechController.isReading);
        
        GameObject.FindGameObjectWithTag("SendButtonSprite").GetComponent<Animator>().SetTrigger("Active");
        yield return new WaitForSecondsRealtime(0.3f);
        textInput.text = defaultTxt;
        // Read button content

        conversationDisplayer.isInChoice = false;
        conversationDisplayer.nextBranch = branche;
        conversationDisplayer.choiceButton = this;



        // Send button's animation
        GameObject.FindGameObjectWithTag("SendButtonSprite").GetComponent<Animator>().SetTrigger("Active");
    }


}
