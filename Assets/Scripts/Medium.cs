using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScriptableObject contenant les informations d'un medium de communication
[CreateAssetMenu(fileName = "New Medium", menuName = "Medium", order = 51)]
public class Medium : ScriptableObject
{
    [SerializeField]
    public GameObject playerMessageBox;
    [SerializeField]
    public GameObject npcMessageBox;
    [SerializeField]
    public GameObject background;
    [SerializeField]
    public GameObject navBar;
    [SerializeField]
    public GameObject footer;

    private string id;

    private void Awake()
    {
        id = name;
    }


}