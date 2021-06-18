using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TransitionText : MonoBehaviour
{
    public void ChangeTransition(string dateTime, string character1, string character2 )
    {
        TMP_Text textComponent = GetComponent<TMP_Text>();

        textComponent.text = $"{dateTime}\n {character1} parle à {character2}";
        if (SaveManager.settings.speechHelp)
            SpeechController.ReadText(textComponent.text);

    }




}
