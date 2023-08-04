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
        CPText,
        RubyText,
        WarningText,
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

        _playerData = FindObjectOfType<GameData>();
        GetText((int)Texts.InfoText).text = $"{_playerData.ReviveInfo.ReviveCount} / {5} ( �Ϸ� ȯ������ {5}�� )";

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
                // ���� ����
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
                    _info.text = "�� �̻� ȯ���� �� �����ϴ�.";

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

            GetButton((int)Buttons.CancelButton).gameObject.SetActive(true);
            GetText((int)Texts.CancelButtonText).text = "�ݱ�";
            GetText((int)Texts.MessageText).text = $"{Managers.Game.PlyaerDataBase.RankDic[(int)PlayerRank.Rank.E].Condition}�� ���� �� ȯ���� �Ұ��մϴ�.{System.Environment.NewLine}{Managers.Game.PlyaerDataBase.RankDic[(int)PlayerRank.Rank.E].Condition}�� �޼� �� ȯ���� �ּ���!";
        
        }
        else
        {
            Get<GameObject>((int)GameObjects.CurrencyPanel).SetActive(true);
            GetButton((int)Buttons.AdButton).gameObject.SetActive(true);
            GetButton((int)Buttons.OKButton).gameObject.SetActive(true);

            GetText((int)Texts.WarningText).gameObject.SetActive(false);

            StageTask _mainStageTask = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.Main).FirstOrDefault();

            GetText((int)Texts.CPText).text = Util.GetBigIntegerUnit(Managers.Game.CalculateDropRateAmount(Define.CurrencyID.CP, _mainStageTask.GetMonsterHP(Managers.Game.Stage)));
            GetText((int)Texts.RubyText).text = Util.GetBigIntegerUnit(Math.Max(1, Managers.Game.Stage / 10));

            //���� ��Ű�� ���� Ȯ�� �� ���� ���� ��ư �Ⱥ��̵��� �����Ұ�
            if (Managers.Game.IsAdSkipped)
            {
                GetButton((int)Buttons.AdButton).gameObject.SetActive(false);
            }
        }
    }

    
}
