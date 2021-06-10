using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterController : MonoBehaviour
{

    private List<GameObject> choiceRows;
    public float footerHeigth { get; set; }
    public float keyboardBarHeigth { get; set; }

    IEnumerator Start()
    {
        yield return null;
        choiceRows = new List<GameObject>(GameObject.FindGameObjectsWithTag("footerRow"));
        RectTransform rectTransform = GetComponent<RectTransform>();
        footerHeigth = rectTransform.rect.y;
        keyboardBarHeigth = transform.Find("KeyboardBarParent").GetComponent<RectTransform>().rect.y * 2;
        Debug.Log("Footer Heigth : " + footerHeigth + " keyboardBarHeigth : " + keyboardBarHeigth);
        rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y + footerHeigth - keyboardBarHeigth );
    }

    public void DisplayChoices(List<GameObject> choicesButton)
    {

        switch (choicesButton.Count)
        {
            case 1:
                choiceRows[1].SetActive(false);
                choiceRows[2].SetActive(false);

                choicesButton[0].transform.SetParent(choiceRows[0].transform);

                break;
            case 2:

                choiceRows[1].SetActive(true);
                choiceRows[2].SetActive(false);

                choicesButton[0].transform.SetParent(choiceRows[0].transform);
                choicesButton[1].transform.SetParent(choiceRows[1].transform);

                break;
            case 3:

                choiceRows[1].SetActive(true);
                choiceRows[2].SetActive(true);

                choicesButton[0].transform.SetParent(choiceRows[0].transform);
                choicesButton[1].transform.SetParent(choiceRows[1].transform);
                choicesButton[2].transform.SetParent(choiceRows[2].transform);
                break;
            case 4:

                choiceRows[1].SetActive(true);
                choiceRows[2].SetActive(false);

                choicesButton[0].transform.SetParent(choiceRows[0].transform);
                choicesButton[1].transform.SetParent(choiceRows[0].transform);
                choicesButton[2].transform.SetParent(choiceRows[1].transform);
                choicesButton[3].transform.SetParent(choiceRows[1].transform);

                break;
            default:
                Debug.LogError("An error with the display of choices as occured");
                break;
        }
    }


}
