using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MailIcon : MonoBehaviour
{
    [SerializeField]
    Image image;

    public Sprite Icon
    {
        get
        {
            return image.sprite;
        }
        set
        {
            image.sprite = value;
        }
    }
}
