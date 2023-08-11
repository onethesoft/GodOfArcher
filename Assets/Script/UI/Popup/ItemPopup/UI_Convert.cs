using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Convert : UI_Popup
{
    public enum ItemType
    {
        Rune,
        Pet
    }
    
    enum State
    {
        None,
        WaitForSelectConsumeItem,
        WaitForSelectTargetItem,
        SelectedTargetItem,

    }

    State state;
    
    public ItemType ConvertItemType = ItemType.Rune;

    [SerializeField]
    Text TitleText;

    [SerializeField]
    Text SubTitleText;

    [SerializeField]
    UI_Converter Consume;

    [SerializeField]
    UI_Converter Target;

    [SerializeField]
    UI_ConvertTargetSelector ConvertSelector;

    [SerializeField]
    Button OneConvertButton;

    [SerializeField]
    Button AllConvertButton;

    [SerializeField]
    Button CloseButton;


    Converter converter;
    BlockerController[] blockers;


    public override void Init()
    {
        base.Init();
        converter = GetComponent<Converter>();
        state = State.None;
        if (ConvertItemType == ItemType.Rune)
        {
            TitleText.text = "�� ��ȯ";
            SubTitleText.text = "*�� ��ȯ���� ��ᰡ 2�� �����˴ϴ�.";
            Consume.TitleText = "��ȯ �� ��";
            Target.TitleText = "��ȯ �� ��";
        }
        else
        {
            TitleText.text = "�� ��ȯ";
            SubTitleText.text = "*�� ��ȯ���� ��ᰡ 2�� �����˴ϴ�.";
            Consume.TitleText = "��ȯ �� ��";
            Target.TitleText = "��ȯ �� ��";
        }
        ConvertSelector.ItemType = ConvertItemType;

        ConvertSelector.OnSelectItem -= OnHandleSelectTarget;
        ConvertSelector.OnSelectItem += OnHandleSelectTarget;

        blockers = GetComponentsInChildren<BlockerController>();

        Consume.OnClick += () => {
           
            ConvertSelector.gameObject.SetActive(true);
            ConvertSelector.UpdateItem(null);
            state = State.WaitForSelectConsumeItem;
        };



        OneConvertButton.onClick.AddListener(() => {
            if (state != State.SelectedTargetItem)
                return;
            if (!converter.IsConvertable(1))
                return;
            converter.Convert(1);
            foreach(BlockerController blocker in blockers)
            {
                blocker.Block();
            }
        });

        AllConvertButton.onClick.AddListener(() => {
            if (state != State.SelectedTargetItem)
                return;
            if (converter.GetMaxConvertCount() <= 0)
                return;
            if (!converter.IsConvertable(converter.GetMaxConvertCount()))
                return;

            converter.Convert(converter.GetMaxConvertCount());
            foreach (BlockerController blocker in blockers)
            {
                blocker.Block();
            }
        });

        CloseButton.onClick.AddListener(() => {
            ClosePopupUI();
        });
    }

    void OnHandleSelectTarget(BaseItem select)
    {
        if(state == State.WaitForSelectConsumeItem)
        {
            if (Managers.Game.GetInventory().IsFindItem(select.ItemId) == false)
                return;
            if (Managers.Game.GetInventory().Find(x => x.ItemId == select.ItemId).GetUsableCount() < converter.RequireCount)
                return;

            Consume.SelectTarget(select);
            ConvertSelector.UpdateItem(select);
            converter.ConsumeItem = select.ItemId;
            state = State.WaitForSelectTargetItem;
        }
        
        else if(state == State.WaitForSelectTargetItem)
        {
            Target.SelectTarget(select);
            ConvertSelector.gameObject.SetActive(false);
            converter.TargetItem = select.ItemId;
            state = State.SelectedTargetItem;
        }
        
    }

    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
        ConvertSelector.OnSelectItem -= OnHandleSelectTarget;
        Managers.Resource.Destroy(ConvertSelector.gameObject);
    }
}
