using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageBoxResizer : MonoBehaviour
{
    TMP_Text textComponent;
    RectTransform myRectTransform;
    [SerializeField]
    private float padding;

    public void ResizeBox()
    {
        textComponent = transform.Find("Text").gameObject.GetComponent<TMP_Text>();
        myRectTransform = GetComponent<RectTransform>();
        myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, textComponent.preferredHeight+ 2*padding );

    }
}
