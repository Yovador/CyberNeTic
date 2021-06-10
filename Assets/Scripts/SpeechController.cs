using UnityEngine;
using TextSpeech;

public class SpeechController : MonoBehaviour
{
    public string lang = "fr-FR";
    
    private void Start()
    {
        SetUp();
    }

    private void SetUp ()
    {
        TextToSpeech.instance.Setting(lang, TextToSpeech.instance.pitch, TextToSpeech.instance.rate);
    }

    public static void ReadText (string text)
    {
        TextToSpeech.instance.StartSpeak(text);
    }

    public static void StopReading()
    {
        TextToSpeech.instance.StopSpeak();
    }
}
