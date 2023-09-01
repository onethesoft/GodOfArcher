using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_Revive : UI_Popup
{
    enum Buttons
    {
        Close,
        AdButton,
        OKButton,
        CancelButton
    }
    enum Texts
    {
        RubyBonusInfoText,
        RubyBonusText,
        CPText,
        RubyText,
        CancelButtonText,
        MessageText,
        InfoText
    }

    enum GameObjects
    {
        CurrencyPanel,
        CPPanel,
        RubyPanel,
        InfoPanel
    }
    public enum Mode
    {
        Block,
        NonBlock
    }
    public int Level { get; set; } = 0;
    public int Stage { get; set; } = 0;
    

    Mode _mode = Mode.NonBlock;
    GameData _playerData;
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        


         _mode = EvaluateMode();
        string _rubyBonusText = string.Empty;
        foreach(PlayerDatabase.ReviveBonusData rubyBonus in Managers.Game.PlyaerDataBase.ListRubyBonus)
        {
            _rubyBonusText += $"{Util.GetBigIntegerUnit(rubyBonus.Stage)}층 이상 +{Util.GetBigIntegerUnit(rubyBonus.RubyBonusCount)} 루비{System.Environment.NewLine}";
        }
        GetText((int)Texts.RubyBonusText).text = _rubyBonusText;

        _playerData = FindObjectOfType<GameData>();
        GetText((int)Texts.InfoText).text = $"{_playerData.ReviveInfo.ReviveCount} / {Managers.Game.PlyaerDataBase.GetReviveCoutLimit} ( 하루 환생제한 {Managers.Game.PlyaerDataBase.GetReviveCoutLimit}번 )";

        OnUpdate(_mode);

        AddUIEvent(GetButton((int)Buttons.AdButton).gameObject, (data) => {

            if (_playerData.ReviveInfo.CanRevive())
            {
                if(Managers.Ad.ShowAd("Revive",()=> { }))
                {
                    Managers.Game.Revive(true);
                }
               // Managers.Ad.ShowRewardVideoIronSource(() =>
                //{
                    
               // });

                ClosePopupUI();
                // 광고 본후
            }

        });

        AddUIEvent(GetButton((int)Buttons.OKButton).gameObject, (data) => {

           
            
            if (_playerData.ReviveInfo.CanRevive())
            {
                Managers.Game.Revive();
                ClosePopupUI();
            }
            else
            {
                if (UnityEngine.Object.FindObjectOfType<UI_FadeText>() == null)
                {
                    UI_FadeText _info = Util.GetOrAddComponent<UI_FadeText>(Managers.Resource.Instantiate("UI/SubItem/UI_FadeText", gameObject.transform));
                    _info.text = "더 이상 환생할 수 없습니다.";

                }
                else
                    UnityEngine.Object.FindObjectOfType<UI_FadeText>().RePlay();
            }
             
            
               
        });

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });

        AddUIEvent(GetButton((int)Buttons.CancelButton).gameObject, (data) => {
            ClosePopupUI();
        });

    }
    

    public Mode EvaluateMode()
    {
#if UNITY_EDITOR
        PlayerDatabase _getdatabase = Resources.Load<PlayerDatabase>($"Database/PlayerDatabase");
        if (Level < (int)PlayerRank.Rank.E && Stage < _getdatabase.RankDic[(int)PlayerRank.Rank.E].Condition)
            return Mode.Block;
        else
            return Mode.NonBlock;
#else
        if (Level < (int)PlayerRank.Rank.E && Stage < Managers.Game.PlyaerDataBase.RankDic[(int)PlayerRank.Rank.E].Condition)
            return Mode.Block;
        else
            return Mode.NonBlock;
#endif

    }
    void OnUpdate(Mode mode)
    {
        if(mode == Mode.Block)
        {
            Get<GameObject>((int)GameObjects.CurrencyPanel).SetActive(false);
            GetButton((int)Buttons.AdButton).gameObject.SetActive(false);
            GetButton((int)Buttons.OKButton).gameObject.SetActive(false);
            GetText((int)Texts.RubyBonusInfoText).gameObject.SetActive(false);
            GetText((int)Texts.RubyBonusText).gameObject.SetActive(false);
         
            GetButton((int)Buttons.CancelButton).gameObject.SetActive(true);
            GetText((int)Texts.CancelButtonText).text = "닫기";
            GetText((int)Texts.MessageText).text = $"{Managers.Game.PlyaerDataBase.RankDic[(int)PlayerRank.Rank.E].Condition}층 이하 시 환생이 불가합니다.{System.Environment.NewLine}{Managers.Game.PlyaerDataBase.RankDic[(int)PlayerRank.Rank.E].Condition}층 달성 후 환생해 주세요!";
        
        }
        else
        {
            Get<GameObject>((int)GameObjects.CurrencyPanel).SetActive(true);
            GetButton((int)Buttons.AdButton).gameObject.SetActive(true);
            GetButton((int)Buttons.OKButton).gameObject.SetActive(true);
            GetText((int)Texts.RubyBonusInfoText).gameObject.SetActive(true);
            GetText((int)Texts.RubyBonusText).gameObject.SetActive(true);


            StageTask _mainStageTask = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.Main).FirstOrDefault();

            GetText((int)Texts.CPText).text = Util.GetBigIntegerUnit(Managers.Game.CalculateDropRateAmount(Define.CurrencyID.CP, _mainStageTask.GetMonsterHP(Managers.Game.Stage)));
            GetText((int)Texts.RubyText).text = Util.GetBigIntegerUnit(_playerData.GetReviveRubyAmount());

            //무한 패키지 구매 확인 후 광고 보상 버튼 안보이도록 수정할것
            if (Managers.Game.IsAdSkipped)
            {
                GetButton((int)Buttons.AdButton).gameObject.SetActive(false);
            }
        }
    }

    
}
