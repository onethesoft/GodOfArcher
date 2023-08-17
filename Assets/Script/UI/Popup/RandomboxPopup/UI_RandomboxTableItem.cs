using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RandomboxTableItem : UI_Base
{
    enum Texts
    {
        Text,
        RatingText
    }
    enum Images
    {
        Image
    }
    Sprite _sprite;
    string _text;
    int _level;
    UI_RandomboxTable.Mode _mode;
    string[,] _rateTexts = new string[,] { {
            "75%","13%","7%","3%","1.5%","0.4%","0.1%","0.03%","0.01%","0.003%"
        },{
            "90%","5%","2.5%","1.5%","0.9%","0.09%","0.03%","0.01%","0.003%","0.001%"
        } };

    public void Set(int level , Sprite sprite, UI_RandomboxTable.Mode mode )
    {
        _level = level;
        _mode = mode;
        _sprite = sprite;
       

        _text = $"{((Rune.Rank)level).ToString()}±ﬁ {(mode == UI_RandomboxTable.Mode.RuneAndPet ? "∑È,∆Í" :"¿Â∫Ò")}";

    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetText((int)Texts.Text).text = _text;
        GetText((int)Texts.RatingText).text = _rateTexts[(int)_mode, _level];
        GetImage((int)Images.Image).sprite = _sprite;




    }
}
