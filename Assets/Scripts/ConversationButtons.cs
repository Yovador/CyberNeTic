using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationButtons : MonoBehaviour
{
    [HideInInspector]
    protected ConversationDisplayer conversationDisplayer { get; set; }
    protected bool isHold = false;
    protected int waitIncrement = 0;
    protected int waitLimit = 60;

    virtual protected void Start()
    {
        conversationDisplayer = GameObject.Find("ConversationDisplayer").GetComponent<ConversationDisplayer>();
    }

    private void Update()
    {
        if (isHold)
        {
            OnHold();
        }
        if(waitIncrement > waitLimit)
        {
            StartCoroutine(ButtonAction());
            waitIncrement = 0;
        }
    }

    virtual protected void OnHold()
    {
        waitIncrement++;
    }

    virtual public void OnPointerDown()
    {
        isHold = true;
    }

    virtual public void OnPointerUp()
    {
        isHold = false;
        if (waitIncrement > waitLimit)
        {
            ButtonAction();
        }
        else
        {
            waitIncrement = 0;
        }
    }

    virtual protected IEnumerator ButtonAction()
    {
        yield return null;
    }
}
