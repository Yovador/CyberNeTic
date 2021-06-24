using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class OpenHyperlinks : MonoBehaviour
{

    public void OnPointerClick(string link)
    {
        Debug.Log(link);
        Application.OpenURL(link);
    }

}