using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureLoader : MonoBehaviour
{

    protected Image imageComponent;
    protected ConversationDisplayer conversationDisplayer;

    protected void Start()
    {
        conversationDisplayer = GameObject.FindGameObjectWithTag("ConversationDisplayer").GetComponent<ConversationDisplayer>();

        LoadPicture();

        imageComponent.SetNativeSize();
        GetComponent<AspectRatioFitter>().aspectRatio = imageComponent.preferredWidth / imageComponent.preferredHeight;

    }

    virtual protected void LoadPicture()
    {
        imageComponent = GetComponent<Image>();

    }
}
