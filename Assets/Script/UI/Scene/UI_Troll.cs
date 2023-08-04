using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Troll : UI_Scene
{
    public enum Buttons
    {
        Skill,
        Quest,
        Ranking,
        Daily,
        MailBox,
        Setting,
        Stat,
        Pet,
        Rune,
        Artifact,
        Item,
        Dongeon,
        Shop,
        Revive,
        Shotdown,
        Giveup
    }
    enum Sliders
    {
        Timer
    }
    enum Texts
    {
        TimerText,
        MonsterHPText,
        StageTitle
    }
    enum GameObjects
    {
        TopLeftPanel
    }

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
    public void SetMonsterHPText(string text)
    {
        if (GetText((int)Texts.MonsterHPText) != null)
            GetText((int)Texts.MonsterHPText).text = text;
    }
    public void SetLevelText(int level)
    {
        if (GetText((int)Texts.StageTitle) != null)
            GetText((int)Texts.StageTitle).text = "트롤의 서식지 "+ level.ToString()+"층";
    }

    public void SetEnableButton(Buttons button , bool enable)
    {
        if (GetButton((int)button) != null)
            GetButton((int)button).gameObject.SetActive(enable);
    }
    public bool IsEnableButton(Buttons button)
    {
        if (GetButton((int)button) != null)
            return GetButton((int)button).gameObject.activeSelf;
        return false;
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Slider>(typeof(Sliders));
        Bind<Text>(typeof(Texts));

        AddUIEvent(GetButton((int)Buttons.Giveup).gameObject, (data) => {
            Managers.UI.ShowPopupUI<UI_DongeonMessagePopup>().OnOk += () => {
                Managers.Scene.LoadScene(Define.Scene.Main);
            };
            
        });



     




    }
}
