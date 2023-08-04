using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_GoldPig : UI_Scene
{
    enum Buttons
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
    }
    enum Sliders
    {
        Timer
    }
    enum Texts
    {
        TimerText,
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

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Slider>(typeof(Sliders));
        Bind<Text>(typeof(Texts));

        

     
      


    }
   
}
