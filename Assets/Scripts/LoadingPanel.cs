using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    Image image;
    float targetAlpha;
    [SerializeField]
    private float fadeRate;
    [HideInInspector]
    public bool isFading = false;

    private void Start()
    {
        image = transform.Find("Panel").GetComponent<Image>();
        image.raycastTarget = false;
    }
    public IEnumerator Disappear()
    {
        Debug.Log("Disappear");
        isFading = true;

        image.raycastTarget = true;
        targetAlpha = 0f;
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(Fade(targetAlpha));
        yield return new WaitWhile(() => isFading);
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        Color curColor = image.color;
        float alphaDiff = Mathf.Abs(curColor.a - targetAlpha);
        while (alphaDiff > 0.05f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, fadeRate * Time.deltaTime);
            image.color = curColor;
            alphaDiff = Mathf.Abs(curColor.a - targetAlpha);
            yield return new WaitForEndOfFrame();
        }
        isFading = false;
    }

    public IEnumerator Appear()
    {
        isFading = true;
        gameObject.SetActive(true);

        isFading = true;
        image.raycastTarget = true;
        targetAlpha = 1f;
        StartCoroutine(Fade(targetAlpha));
        yield return new WaitWhile(() => isFading);


    }
}
