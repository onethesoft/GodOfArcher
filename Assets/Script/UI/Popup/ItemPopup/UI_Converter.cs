using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Converter : UI_Base
{
    public System.Action OnClick;

    [SerializeField]
    Text ConverterText;

    [SerializeField]
    Text CountText;

    [SerializeField]
    Button button;

    [SerializeField]
    GameObject ItemPrefab;

    string ItemId;

    Converter converter;

    public string TitleText 
    { 
        get
        {
            return ConverterText.text;
        }
        set
        {
            ConverterText.text = value;
        }
    }

    public override void Init()
    {
        ItemId = string.Empty;
        CountText.text = "º¸À¯ °¹¼ö : --";
        converter = GetComponentInParent<Converter>();
        Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
        Managers.Game.GetInventory().OnItemChanged += UpdateCountText;
        button.onClick.AddListener(() =>
        {
            OnClick?.Invoke();
        });

    }

    public void SelectTarget(BaseItem item)
    {
        UI_BaseItem _item = GetComponentInChildren<UI_BaseItem>();
        if(_item == null)
        {
            _item = Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate(ItemPrefab, button.transform));
        }


        _item.Item = item;
        ItemId = item.ItemId;
        UpdateCountText();
        

    }

    void UpdateCountText()
    {
        if (string.IsNullOrEmpty(ItemId))
        {
            CountText.text = $"º¸À¯ °¹¼ö : --";
        }
        else if(!Managers.Game.GetInventory().IsFindItem(ItemId) )
        {
            CountText.text = $"º¸À¯ °¹¼ö : {0}/{converter.RequireCount}";
        }
        else
            CountText.text = $"º¸À¯ °¹¼ö : {Managers.Game.GetInventory().Find(x=>x.ItemId == ItemId).GetUsableCount()}/{converter.RequireCount}";

    }

    private void OnDestroy()
    {
        Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
    }





}
