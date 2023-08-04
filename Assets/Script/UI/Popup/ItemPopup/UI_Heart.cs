using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Heart : UI_Base
{
    Artifact _heart;
    Artifactpiece _heartpiece;
    public Artifact Heart { set { _heart = value; } }
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

        if (_heart != null)
        {
            GetImage((int)Images.Icon).sprite = _heart.Icon;
            GetImage((int)Images.UpgradeIcon).sprite = _heart.Icon;

            Artifact _getitem = Managers.Game.GetInventory().Find(_heart) as Artifact;

            if (_getitem == null)
                GetText((int)Texts.Description).text = $"{_heart.Description} : 레벨 {0}";
            else
                GetText((int)Texts.Description).text = $"{_heart.Description} : 레벨 {_getitem.Level}";

            GetText((int)Texts.DisplayName).text = $"{_heart.DisplayName} : {_heart.IncrementstatPerLevel}%";
            GetText((int)Texts.MaxLevel).text = $"최고레벨 {_heart.MaxLevel}";

            _heartpiece = Managers.Item.GetConsumeItems(_heart)[0] as Artifactpiece;
            UpdateCountText();
            Managers.Game.GetInventory().OnItemChanged -= UpdateCountText;
            Managers.Game.GetInventory().OnItemChanged += UpdateCountText;

            AddUIEvent(GetButton((int)Buttons.OneUpgradeButton).gameObject, (data) => {

                
                Managers.Item.UpgradeOneItem(_heart, true);
            });
            AddUIEvent(GetButton((int)Buttons.TotalUpgradeButton).gameObject, (data) => {
                Managers.Item.UpgradeOneItem(_heart);
            });
        }
    }

    public void UpdateCountText()
    {
        Artifactpiece _GetItem = Managers.Game.GetInventory().Find(_heartpiece) as Artifactpiece;
        if (_GetItem == null)
            GetText((int)Texts.CountText).text = $"갯수 : {0}";
        else
            GetText((int)Texts.CountText).text = $"갯수 : {_GetItem.GetUsableCount().Value}";

        Artifact _getHeart = Managers.Game.GetInventory().Find(_heart) as Artifact;
        if (_getHeart == null)
        {
            GetText((int)Texts.Description).text = $"{_heart.Description} : 레벨 {0}";
            GetText((int)Texts.Stat).text = $"{_heart.Description}량 : {0}% 증가";
        }
        else
        {
            GetText((int)Texts.Description).text = $"{_heart.Description} : 레벨 {_getHeart.Level}";
            GetText((int)Texts.Stat).text = $"{_heart.Description}량 : {_getHeart.StatModifier.Value}% 증가";
        }


    }
    private void OnDestroy()
    {
        Managers.Game.GetInventory().OnItemChanged -= UpdateCountText; ;
    }


}
