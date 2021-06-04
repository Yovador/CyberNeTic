using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxResizer : MonoBehaviour
{
    RectTransform textRectTransform;
    RectTransform myRectTransform;
    [SerializeField]
    private float padding;

    private void Start()
    {
        textRectTransform = transform.Find("Text").gameObject.GetComponent<RectTransform>();
        myRectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if(textRectTransform.rect.height + padding != myRectTransform.sizeDelta.x)
        {
            myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, textRectTransform.rect.height +padding );
        }

    }
}
