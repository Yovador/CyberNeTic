using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScriptableObject contenant les informations d'un medium de communication
[CreateAssetMenu(fileName = "New Medium", menuName = "Medium", order = 51)]
public class Medium : ScriptableObject
{
    [SerializeField]
    public float spaceBetweenMessages;
    [SerializeField]
    public GameObject playerMessageBox;
    [SerializeField]
    public GameObject npcMessageBox;
    [SerializeField]
    public GameObject playerMessageBoxImage;
    [SerializeField]
    public GameObject npcMessageBoxImage;
    [SerializeField]
    public GameObject background;
    [SerializeField]
    public GameObject navBar;
    [SerializeField]
    public GameObject footer;
    [SerializeField]
    public float footerHeigth;
    [SerializeField]
    public GameObject choiceButton;
    [SerializeField]
    public GameObject impossibleChoiceButton;
    [SerializeField]
    public GameObject nextConvoButton;
    [SerializeField]
    public float spaceBetweenChoices;

    private string id;
    private void Awake()
    {
        id = name;
    }


}
