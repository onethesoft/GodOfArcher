using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : UI_Scene
{
    
    enum Buttons
    {
        Skill ,
        Quest ,
        Ranking ,
        Daily,
        MailBox ,
        Setting,
        Stat ,
        Pet,
        Rune ,
        Artifact,
        Item ,
        Dongeon,
        Shop ,
        Revive,
        Shotdown,
        RankPanel,
        SeasonPassButton,
        SeasonPass2Button,
        RankHelp,
        RubyPurchaseButton,
        RouletteButton,
        // Tests





        Test,
        

    }

    enum Sliders
    {
        Timer
    }
    enum Texts
    {
        TimerText,
        StageText,

        //Test
        MonsterHPText,
    }
    enum GameObjects
    {
        TopLeftPanel,
        UI_AnimatedSP,
        TutorialBuffPanel
    }

    public bool IsBeginner { get; set; } = false;
    public void SetTimerProgress(float progress)
    {
        if (Get<Slider>((int)Sliders.Timer) == null) return;
        Get<Slider>((int)Sliders.Timer).value = progress;
    }
    public void SetTimerText(string text)
    {
        if (GetText((int)Texts.TimerText) == null) return;
        GetText((int)Texts.TimerText).text = text;
    }

    
   

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Slider>(typeof(Sliders));
        Bind<Text>(typeof(Texts));
        
        
        
        Get<Slider>((int)Sliders.Timer).value = 0.8f;
        //Get<GameObject>((int)GameObjects.UI_AnimatedSP).SetActive(false);

        AddUIEvent(GetButton((int)Buttons.Stat).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_Stat>(); });
        AddUIEvent(GetButton((int)Buttons.Quest).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_Quest>(); });
        AddUIEvent(GetButton((int)Buttons.Item).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_Item>(); });
        AddUIEvent(GetButton((int)Buttons.Rune).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_RunePopup>(); });
        AddUIEvent(GetButton((int)Buttons.Pet).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_PetPopup>(); });
        AddUIEvent(GetButton((int)Buttons.Shop).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_ShopPopup>().Setup(UI_ShopPopup.Panel.ShopView); });
        AddUIEvent(GetButton((int)Buttons.Artifact).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_Artifact>(); });
        AddUIEvent(GetButton((int)Buttons.Dongeon).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_Dongeon>(); });
        AddUIEvent(GetButton((int)Buttons.Revive).gameObject, (data) => {
            UI_Revive ui_revive = Managers.UI.ShowPopupUI<UI_Revive>();
            ui_revive.Level = Managers.Game.Level;
            ui_revive.Stage = Managers.Game.Stage;
        });
        AddUIEvent(GetButton((int)Buttons.MailBox).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_MailBox>(); });
        AddUIEvent(GetButton((int)Buttons.RankPanel).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_CharacterInfo>(); });
        AddUIEvent(GetButton((int)Buttons.Setting).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_Setting>(); });
        AddUIEvent(GetButton((int)Buttons.Daily).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_DailyCheckout>(); });
        AddUIEvent(GetButton((int)Buttons.SeasonPassButton).gameObject, (data) => { 
            Managers.UI.ShowPopupUI<UI_Seasonpass>().mode = UI_Seasonpass.Mode.PASS; 
        });
        AddUIEvent(GetButton((int)Buttons.SeasonPass2Button).gameObject, (data) => { 
            Managers.UI.ShowPopupUI<UI_Seasonpass>().mode = UI_Seasonpass.Mode.PASS2; 
        });
        AddUIEvent(GetButton((int)Buttons.RankHelp).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_RankHelp>(); });
        AddUIEvent(GetButton((int)Buttons.Shotdown).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_Shutdown>(); });
        AddUIEvent(GetButton((int)Buttons.Ranking).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_Ranking>(); });

        AddUIEvent(GetButton((int)Buttons.RubyPurchaseButton).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_ShopPopup>().Setup(UI_ShopPopup.Panel.RubyView); });
        AddUIEvent(GetButton((int)Buttons.RouletteButton).gameObject, (data) => { Managers.UI.ShowPopupUI<UI_Roulette>(); });





#if ENABLE_LOG

        AddUIEvent(GetButton((int)Buttons.Test).gameObject, (data) =>
        {
            Managers.UI.ShowPopupUI<UI_Test>();

        });
#endif






    }
#if UNITY_EDITOR
    public void SetMonsterHPText(string text)
    {
        if(GetText((int)Texts.MonsterHPText) != null)
            GetText((int)Texts.MonsterHPText).text = text;
    }
#endif
    private void Update()
    {
        if(GetText((int)Texts.StageText) != null)
            GetText((int)Texts.StageText).text = $"{Managers.Game.Stage}";

        //_attackspeedTest = Managers.Game.GetAttackSpeed;

        //GetText((int)Texts.AttackSpeedInput).text = _attackspeedTest.ToString();
        



    }
    private void OnDestroy()
    {
        
    }
}
