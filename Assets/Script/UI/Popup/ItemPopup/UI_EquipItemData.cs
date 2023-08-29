using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UI_EquipItemData 
{
    public string ItemId;
    public string ItemClass;
    public string DisplayName;
    public OutlineData DisplayNameOutline;




    public Sprite IconSprite;
    public Sprite IconWrapperSprite;

    public Sprite ItemPanelSprite;
    public OutlineData ItemPanelOutline;

    public Sprite BackgroundSprite;
    public Color BackgroundColor;
    public OutlineData BackgroundOutline;

}
