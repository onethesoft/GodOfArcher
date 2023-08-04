using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Essence : UI_Base
{
    Artifact _essence;
    Artifactpiece _essencepiece;
    public Artifact Essence { set { _essence = value; } }
    enum Images
    {
        Icon,
        UpgradeIcon
    }
    enum Buttons
    {
        TotalUpgradeButton,
        OneUpgradeButton
    }
    enum Texts
    {
        DisplayName,
        Stat,
        CountText,
        Description,
        MaxLevel
    }
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        if(_essence != null)
        {
            GetImage((int)Images.Icon).sprite = _essence.Icon;
            GetImage((int)Images.UpgradeIcon).sprite = _essence.Icon;


            Artifact _getitem = Managers.Game.GetInventory().Find(_essence) as Artifact;

            if (_getitem == null)
                GetText((int)Texts.Description).text = $"{_essence.Description} : 레벨 {0}";
            else
                GetText((int)Texts.Description).text = $"{_essence.Description} : 레벨 {_getitem.Level}";


            GetText((int)Texts.DisplayName).text = $"{_essence.DisplayName} : {_essence.IncrementstatPerLevel}%";
            GetText((int)Texts.MaxLevel).text =$"최고레벨 {_essence.MaxLevel}";

            

            _essencepiece = Managers.Item.GetConsumeItems(_essence)[0] as Artifactpiece;
            UpdateCountText();

            Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
            Managers.Game.GetInventory().OnItemChanged += UpdateCountText;

            


            AddUIEvent(GetButton((int)Buttons.OneUpgradeButton).gameObject, (data) => {
                Managers.Item.UpgradeOneItem(_essence, true);
            });
            AddUIEvent(GetButton((int)Buttons.TotalUpgradeButton).gameObject, (data) => {
                Managers.Item.UpgradeOneItem(_essence);
            });
        }
    }

    public void UpdateCountText()
    {
        Artifactpiece _GetItem = Managers.Game.GetInventory().Find(_essencepiece) as Artifactpiece;
        if (_GetItem == null)
            GetText((int)Texts.CountText).text = $"갯수 : {0}";
        else
            GetText((int)Texts.CountText).text = $"갯수 : {_GetItem.GetUsableCount().Value}";

        Artifact _getEssence = Managers.Game.GetInventory().Find(_essence) as Artifact;
        if (_getEssence == null)
        {
            GetText((int)Texts.Description).text = $"{_essence.Description} : 레벨 {0}";
            GetText((int)Texts.Stat).text = $"{_essence.Description}량 : {0}% 증가";
        }
        else
        {
            GetText((int)Texts.Description).text = $"{_getEssence.Description} : 레벨 {_getEssence.Level}";
            GetText((int)Texts.Stat).text = $"{_getEssence.Description}량 : {_getEssence.StatModifier.Value}% 증가";
        }


    }
    private void OnDestroy()
    {
        Managers.Game.GetInventory().OnItemChanged -= UpdateCountText; ;
    }
}
