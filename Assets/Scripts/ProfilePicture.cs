using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class ProfilePicture : PictureLoader
{
    protected enum PPChar { player, npc }
    [SerializeField]
    protected PPChar profilesPictureCharacter;
    protected override void LoadPicture()
    {
        base.LoadPicture();
        conversationDisplayer.LoadProfilePicture(imageComponent, profilesPictureCharacter.ToString());
    }



}
