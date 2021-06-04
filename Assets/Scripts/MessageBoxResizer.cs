using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxResizer : MonoBehaviour
{
    Text textComponent;
    RectTransform myRectTransform;
    [SerializeField]
    private float padding;

    public void ResizeBox()
    {
        textComponent = transform.Find("Text").gameObject.GetComponent<Text>();
        myRectTransform = GetComponent<RectTransform>();
        Debug.Log("Box size: " + (textComponent.preferredHeight + 2 * padding) );
        myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, textComponent.preferredHeight+ 2*padding );

    }
}
