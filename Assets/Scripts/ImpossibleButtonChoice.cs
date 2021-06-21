using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpossibleButtonChoice : ConversationButtons
{
    bool vibrate = false;
    Vector3 initialPosition;
    RectTransform shakingGameObject;
    float shakeDuration = 0f;
    [SerializeField]
    float shakeWaitTime = 0.5f;
    [SerializeField]
    float shakeMagnitude = 1f;
    [SerializeField]
    float dampingSpeed = 2f;
    protected override void Start()
    {
        base.Start();
        shakingGameObject = GameManager.instance.conversationDisplayer.transform.Find("Scroll").GetComponent<RectTransform>();
        initialPosition = shakingGameObject.localPosition;
    }

    protected override void OnHold()
    {
        if (!vibrate)
        {
            StartCoroutine(StartVibrate());
        }
        shakeDuration = shakeWaitTime;
        ShakeScreen(shakingGameObject);
    }

    IEnumerator StartVibrate()
    {
        vibrate = true;
        Handheld.Vibrate();
        yield return new WaitForSecondsRealtime(shakeWaitTime);
        vibrate = false;
    }


    void ShakeScreen(RectTransform canvasTransform)
    {
        Debug.Log("Shaking ! ");
        if (shakeDuration > 0)
        {
            canvasTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            canvasTransform.localPosition = initialPosition;
        }
    }
}
