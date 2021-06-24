using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextConversationButton : ConversationButtons
{

    protected override void Start()
    {
        base.Start();

    }

    protected override IEnumerator ButtonAction()
    {
        if (SaveManager.settings.speechHelp)
            yield return new WaitWhile(() => SpeechController.isReading);
        conversationDisplayer.endConversation = true;
        completionBackground.localPosition = defaultPosition;
        yield return null;
    }

}
