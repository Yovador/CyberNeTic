using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextConversationButton : ConversationButtons
{
    RectTransform completionBackground;
    Vector3 defaultPosition;
    protected override void Start()
    {
        base.Start();
        completionBackground = GameObject.Find("CompletionBackground").GetComponent<RectTransform>();

        StartCoroutine(WaitOneFrame());
    }
    private IEnumerator WaitOneFrame()
    {
        yield return null;
        Debug.Log(completionBackground.rect.y);
        completionBackground.localPosition = new Vector3(0, completionBackground.rect.y, 0);
        defaultPosition = new Vector3(0, completionBackground.rect.y, 0);

    }
    protected override IEnumerator ButtonAction()
    {
        if (SaveManager.settings.speechHelp)
            yield return new WaitWhile(() => SpeechController.isReading);
        conversationDisplayer.endConversation = true;
        completionBackground.localPosition = defaultPosition;
        yield return null;
    }

    

    protected override void OnHold()
    {
        base.OnHold();

        int currentSize = Mathf.RoundToInt( ( (completionBackground.rect.y * waitIncrement) / waitLimit) + (-completionBackground.rect.y/2));
        //Debug.Log(currentSize);
        completionBackground.localPosition = new Vector3(0, -currentSize, 0);

    }

    public override void OnPointerUp()
    {
        base.OnPointerUp();
        if (waitIncrement < waitLimit)
        {
            completionBackground.localPosition = defaultPosition;
        }
    }
}
