using UnityEngine;
using TextSpeech;

public class SpeechController : MonoBehaviour
{
    public static string lang = "fr-FR";
    public static bool isReading = false;

    // Set up
    private void Start()
    {
        TextToSpeech.instance.onDoneCallback += OnReadingDone;
        TextToSpeech.instance.onStartCallBack += OnReadingStart;
        SetUp();
    }

    private void OnDisable()
    {
        TextToSpeech.instance.onDoneCallback -= OnReadingDone;
        TextToSpeech.instance.onStartCallBack += OnReadingStart;
    }

    private void SetUp ()
    {
        TextToSpeech.instance.Setting(lang, TextToSpeech.instance.pitch, TextToSpeech.instance.rate);
    }

    // Actions
    public static void ReadText (string text)
    {
        if (!isReading && SaveManager.settings.speechHelp)
        {
            OnReadingStart();
            TextToSpeech.instance.StartSpeak(text);
        }
    }

    public static void StopReading()
    {
        OnReadingDone();
        TextToSpeech.instance.StopSpeak();
    }

    // Callbacks
    private static void OnReadingStart()
    {
        isReading = true;
    }

    private static void OnReadingDone()
    {
        isReading = false;
    }
}
