using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;

public class UI_Shutdown : UI_Popup
{
    enum Buttons
    {
        Close
    }
    enum Texts
    {
        StageLabelText,
        StageText,
        TimeLabelText,
        TimeText,
        GoldLabelText,
        GoldText,
        CPLabelText,
        CPText,
        RubyLabelText,
        RubyText,
        CloseText
    }

    int clicked = 0;
    float clicktime = 0;
    float clickDelay = 1.0f;

    DateTime _time;


    BigInteger _gold;
   
    StageTask _mainStageTask;

    int _Stage;

    int _beforeruby;
    int _ruby;



    Color _buttneTextColor;

    GameData _playerData;
  
    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        _playerData = FindObjectOfType<GameData>();

        _time = GlobalTime.Now.ToLocalTime();
        _gold = Managers.Game.GetCurrency(Define.CurrencyID.Gold.ToString());
        _Stage = Managers.Game.Stage;
        _ruby = (Managers.Game.Stage / 10) + Managers.Game.PlyaerDataBase.GetReviveBonus(Managers.Game.Stage);
        _mainStageTask = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.Main).FirstOrDefault();
        clicked = 0;
        OnDemandRendering.renderFrameInterval = 3;

        _buttneTextColor = GetText((int)Texts.CloseText).color;

      
       

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            if(Time.time - clicktime < clickDelay)
            {
                clicked++;
                if (clicked >= 1)
                    ClosePopupUI();
            }
            clicktime = Time.time;
            
            
        });

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {

            GetText((int)Texts.CloseText).color = Color.yellow;
        },  Define.UIEvent.Pressed);

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            GetText((int)Texts.CloseText).color = _buttneTextColor;


        }, Define.UIEvent.Released);

     

        GetText((int)Texts.StageText).text = $"{_Stage}";
        GetText((int)Texts.TimeText).text =$"00:00:00";

      

        GetText((int)Texts.GoldText).text = $"{Util.GetBigIntegerUnit(Managers.Game.GetCurrency(Define.CurrencyID.Gold.ToString()) - _gold)}";
        GetText((int)Texts.CPText).text = $"{Util.GetBigIntegerUnit(Managers.Game.CalculateDropRateAmount(Define.CurrencyID.CP, _mainStageTask.GetMonsterHP(Managers.Game.Stage)))}";
        GetText((int)Texts.RubyText).text = $"환생 시 얻는 루비 : {Util.GetBigIntegerUnit(_playerData.GetReviveRubyAmount())}";
    }

    public override void ClosePopupUI()
    {
      
        base.ClosePopupUI();
        OnDemandRendering.renderFrameInterval = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Update()
    {
        UpdatePlayer();
    }

    void UpdatePlayer()
    {
        /*
        _beforeruby = _ruby;
        _ruby = Managers.Game.Stage / 10;

        if (_beforeruby != _ruby)
        {
            GetText((int)Texts.RubyText).DOCounter(_beforeruby, _ruby, 0.8f);
        }
        */
        TimeSpan _diff = GlobalTime.Now.ToLocalTime() - _time;


        GetText((int)Texts.TimeText).text = _diff.ToString(@"hh\:mm\:ss");



        // GetText((int)Texts.StageText).text = $"스테이지 : {Managers.Game.Stage}";
        GetText((int)Texts.StageText).text = Managers.Game.Stage.ToString();
        GetText((int)Texts.GoldText).text = $"{Util.GetBigIntegerUnit(Managers.Game.GetCurrency(Define.CurrencyID.Gold.ToString()) - _gold)}";
        GetText((int)Texts.CPText).text = $"{Util.GetBigIntegerUnit(Managers.Game.CalculateDropRateAmount(Define.CurrencyID.CP, _mainStageTask.GetMonsterHP(Managers.Game.Stage)))}";
        GetText((int)Texts.RubyText).text = $"환생 시 얻는 루비 : {Util.GetBigIntegerUnit(_playerData.GetReviveRubyAmount())}";
    }



}
