using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextBoxResizer : MonoBehaviour
{
    TMP_Text textComponent;
    RectTransform myRectTransform;
    [SerializeField]
    private float padding;
    [SerializeField]
    private bool resizeByHeigth = true;

    public void ResizeBox()
    {
        textComponent = transform.Find("Text").gameObject.GetComponent<TMP_Text>();
        myRectTransform = GetComponent<RectTransform>();
        if (resizeByHeigth)
        {
            myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, textComponent.preferredHeight+ 2*padding );
        }
        else
        {
            myRectTransform.sizeDelta = new Vector2(textComponent.preferredWidth + 2 * padding, myRectTransform.sizeDelta.y);
        }

    }
}
