using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
 

public class EndManager : MonoBehaviour
{
    LoadingPanel loadingPanel;
    VideoPlayer videoPlayer;
    IEnumerator Start()
    {
        yield return null;
        loadingPanel = GameObject.Find("LoadingPanel").GetComponent<LoadingPanel>();
        videoPlayer = gameObject.GetComponent<VideoPlayer>();

        string[] path = null;
        Debug.Log("Exist : " + BetterStreamingAssets.DirectoryExists("videos"));

        path = BetterStreamingAssets.GetFiles("videos", "*.mp4", SearchOption.AllDirectories);

        byte[] bytes = BetterStreamingAssets.ReadAllBytes(path[0]);

        File.WriteAllBytes(Application.persistentDataPath + "/end.mp4", bytes);

        videoPlayer.url = Application.persistentDataPath + "/end.mp4";
        
        StartCoroutine(loadingPanel.Disappear());
        videoPlayer.SetDirectAudioVolume(0, GameManager.musicVolume);
        videoPlayer.Play();
        yield return new WaitWhile(() => loadingPanel.isFading);
        Debug.LogWarning("END");
    }
}
