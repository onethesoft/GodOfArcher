using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RuneItem : UI_Base
{
    Rune _rune;
    public Rune Rune { set { _rune = value; } }

    public delegate void EquipHandler(Rune rune);
    public EquipHandler OnEquipRune;

    public delegate void UpgradeHandler();
    public UpgradeHandler OnUprade;
    enum Images
    {
        Background,
        EquipBackground ,
        DescriptionBackground,
        Icon , 
        IconBackground
    }

    enum Texts
    {
        DisplayName,
        CountText,
        Description
    }
    enum Buttons
    {
        EquipButton,
        OneUpgradeButton,
        AllUpgradeButton
    }

    enum Blocker
    {
        OneUpgradeButtonBlocker,
        AllUpgradeButtonBlocker
    }

    
    UI_RunePopup _parent;
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(Blocker));

        if (_rune != null)
        {
            _parent = GetComponentInParent<UI_RunePopup>();
            GetImage((int)Images.Background).sprite = _rune.Background;
            GetImage((int)Images.DescriptionBackground).sprite = _rune.DescriptionBackground;
            GetImage((int)Images.EquipBackground).sprite = _rune.DescriptionBackground;

            GetImage((int)Images.Icon).sprite = _rune.Icon;
            GetImage((int)Images.IconBackground).sprite = _rune.IconBackground;

            if(_rune.Level > 6)
            {
                GetImage((int)Images.Icon).gameObject.SetActive(false);
            }

            GetText((int)Texts.DisplayName).text = _rune.DisplayName;
            string _itemDescription = "";
            foreach (StatModifier _stat in _rune.StatModifiers)
                _itemDescription += _stat.Description.Replace("\\n", System.Environment.NewLine).Replace("%s", _stat.Value.ToString()) + System.Environment.NewLine;
            GetText((int)Texts.Description).text = _itemDescription; 
            GetText((int)Texts.CountText).text = $"º¸À¯ °¹¼ö : {(Managers.Game.GetInventory().Find(_rune) == null ? 0 : Managers.Game.GetInventory().Find(_rune).GetUsableCount())} / 3";

            Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
            Managers.Game.GetInventory().OnItemChanged += UpdateCountText;

            AddUIEvent(GetButton((int)Buttons.EquipButton).gameObject, (data) => {
                if (Managers.Game.GetInventory().Find(_rune) != null)
                    OnEquipRune?.Invoke(Managers.Game.GetInventory().Find(_rune) as Rune);
                UpdateCountText();
            });

            AddUIEvent(GetButton((int)Buttons.OneUpgradeButton).gameObject, (data) => {
                if (Managers.Item.UpgradeOneItem(_rune, true) == true)
                {
                    OnUprade?.Invoke();
                  
                }
              
                UpdateCountText();


            });

            AddUIEvent(GetButton((int)Buttons.AllUpgradeButton).gameObject, (data) => {
                if (Managers.Item.UpgradeOneItem(_rune) == true)
                {
                    OnUprade?.Invoke();
                   
                }
              

                UpdateCountText();
            });

        }

    }

    public void UpdateCountText()
    {
        //Debug.Log(Managers.Game.GetInventory().Find(_rune) != null ? Managers.Game.GetInventory().Find(_rune).RemainingUses : -1);
        Debug.Log($"{_rune.ItemId} : {(Managers.Game.GetInventory().Find(x=>x.ItemId == _rune.ItemId) == null ? -1 : (Managers.Game.GetInventory().Find(_rune) as EquipableItem).EquipCount)}");
        GetText((int)Texts.CountText).text = $"º¸À¯ °¹¼ö : {(Managers.Game.GetInventory().Find(_rune) == null ? 0 : Managers.Game.GetInventory().Find(_rune).GetUsableCount())} / 3";
    }

    public void OnDestroy()
    {
        Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
        OnEquipRune = null;
        OnUprade = null;
    }

    
}
