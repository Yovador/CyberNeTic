using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
 

public class EndManager : MonoBehaviour
{
    LoadingPanel loadingPanel;
    TMP_Text endText;
    VideoPlayer videoPlayer;
    bool testShown = false;
    [SerializeField]
    float timeBetweenEndText = 1f;
    IEnumerator Start()
    {
        yield return null;
        loadingPanel = GameObject.Find("LoadingPanel").GetComponent<LoadingPanel>();
        videoPlayer = gameObject.GetComponent<VideoPlayer>();
        endText = GameObject.Find("EndCanvas").GetComponentInChildren<TMP_Text>();

        string[] path = null;
        Debug.Log("Exist : " + BetterStreamingAssets.DirectoryExists("videos"));

        path = BetterStreamingAssets.GetFiles("videos", "*.mp4", SearchOption.AllDirectories);

        byte[] bytes = BetterStreamingAssets.ReadAllBytes(path[0]);

        File.WriteAllBytes(Application.persistentDataPath + "/end.mp4", bytes);

        videoPlayer.url = Application.persistentDataPath + "/end.mp4";
        

        StartCoroutine(ShowEnd(GameManager.end));
        yield return new WaitUntil(() => testShown);

        yield return new WaitForSecondsRealtime(timeBetweenEndText);
        StartCoroutine(loadingPanel.Disappear());
        videoPlayer.SetDirectAudioVolume(0, GameManager.musicVolume);
        videoPlayer.Play();
        Debug.LogWarning("END");
    }

    IEnumerator ShowEnd(End end)
    {
        testShown = false;
        foreach (var branche in end.branches)
        {
            foreach (var text in branche.text)
            {

                endText.text = text;

                StartCoroutine(loadingPanel.Disappear());
                yield return new WaitWhile(() => loadingPanel.isFading);

                if (SaveManager.settings.speechHelp)
                {
                    yield return new WaitWhile(() => SpeechController.isReading);
                    SpeechController.ReadText(text);
                    yield return new WaitWhile(() => SpeechController.isReading);
                }

                yield return new WaitForSecondsRealtime(timeBetweenEndText);

                StartCoroutine(loadingPanel.Appear());
                yield return new WaitWhile(() => loadingPanel.isFading);

            }

            List<Character.Relationship> relationshipsToCheck = GameManager.GetCharacterByID(branche.test.characterFrom).relationships;
            int confidenceValue = 0;
            foreach (var relationship in relationshipsToCheck)
            {
                if(relationship.them == branche.test.characterTo)
                {
                    confidenceValue = relationship.confidenceMeToThem;
                }
            }

            Debug.Log("conf : " + confidenceValue);

            List<string> textToShow = new List<string>();


            //Debug.Log("Neutral text : " + branche.test.neutral.text[0]);
            textToShow = branche.test.neutral.text;
            

            if (branche.test.bad.value != 0 && branche.test.bad.text.Count != 0)
            {
                if(confidenceValue <= branche.test.bad.value)
                {
                    Debug.Log("Neutral text : " + branche.test.bad.text[0]);

                    textToShow = branche.test.bad.text;
                }
            }

            if (branche.test.good.value != 0 && branche.test.good.text.Count != 0)
            {
                if (confidenceValue >= branche.test.good.value)
                {
                    Debug.Log("Neutral text : " + branche.test.good.text[0]);

                    textToShow = branche.test.good.text;
                }
            }


            foreach (var text in textToShow)
            {

                endText.text = text;
                //Debug.Log("Text end : " + endText.text);
                StartCoroutine(loadingPanel.Disappear());
                yield return new WaitWhile(() => loadingPanel.isFading);

                if (SaveManager.settings.speechHelp)
                {
                    yield return new WaitWhile(() => SpeechController.isReading);
                    SpeechController.ReadText(text);
                    yield return new WaitWhile(() => SpeechController.isReading);
                }

                yield return new WaitForSecondsRealtime(timeBetweenEndText);

                StartCoroutine(loadingPanel.Appear());
                yield return new WaitWhile(() => loadingPanel.isFading);

            }

        }
        endText.gameObject.SetActive(false);
        testShown = true;

    }

}
