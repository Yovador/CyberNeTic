using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConversationButtons : MonoBehaviour
{
    [HideInInspector]
    protected ConversationDisplayer conversationDisplayer { get; set; }
    protected bool isHold = false;
    protected int waitIncrement = 0;
    protected int waitLimit = 60;
    protected TMP_Text myText;
    protected RectTransform completionBackground;
    protected Vector3 defaultPosition;

    virtual protected void Start()
    {
        conversationDisplayer = GameObject.Find("ConversationDisplayer").GetComponent<ConversationDisplayer>();
        myText = GetComponentInChildren<TMP_Text>();
        completionBackground = transform.Find("CompletionMask").Find("CompletionBackground").GetComponent<RectTransform>();

        StartCoroutine(WaitOneFrame());

    }
    protected IEnumerator WaitOneFrame()
    {
        yield return null;
        completionBackground.localPosition = new Vector3(0, completionBackground.rect.y, 0);
        defaultPosition = new Vector3(0, completionBackground.rect.y, 0);

    }
    private void Update()
    {
        if (isHold)
        {
            OnHold();
        }
        if(waitIncrement > waitLimit)
        {
            StartCoroutine(ButtonAction());
            waitIncrement = 0;
            isHold = false;
        }
    }

    virtual protected void OnHold()
    {
        if (waitIncrement <= waitLimit)
        {
            waitIncrement++;
        }
        int currentSize = Mathf.RoundToInt(((completionBackground.rect.y * waitIncrement) / waitLimit) + (-completionBackground.rect.y / 2));
        //Debug.Log(currentSize);
        completionBackground.localPosition = new Vector3(0, -currentSize, 0);
    }

    virtual public void OnPointerDown()
    {
        isHold = true;
        if (SaveManager.settings.speechHelp)
            SpeechController.ReadText("Vous appuyez sur : " + myText.text);

    }

    virtual public void OnPointerUp()
    {
        isHold = false;
        if (waitIncrement > waitLimit)
        {
            ButtonAction();
        }
        else
        {
            completionBackground.localPosition = defaultPosition;

            waitIncrement = 0;
        }
    }

    virtual protected IEnumerator ButtonAction()
    {
        yield return null;
        if (SaveManager.settings.speechHelp)
            yield return new WaitWhile(() => SpeechController.isReading);
    }
}
