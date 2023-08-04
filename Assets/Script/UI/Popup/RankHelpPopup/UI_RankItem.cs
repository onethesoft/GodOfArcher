using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RankItem : UI_Base
{
    enum Texts
    {
        Rank,
        Condition,
        Rune,
        Pet,
        PierceCount,
        PierceRate,
        JumpingCount,
        JumpingRate
    }
    PlayerRank _rank;
    public void Setup(PlayerRank rankdata)
    {
        _rank = rankdata;
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));

        Debug.Assert(_rank != null, "UI_RankItem : _rank is null");

        GetText((int)Texts.Rank).text = _rank.DisplayName;
        GetText((int)Texts.Condition).text = _rank.Condition.ToString();
        GetText((int)Texts.Rune).text = $"{_rank.RuneSlotCount}";
        GetText((int)Texts.Pet).text = $"{_rank.PetSlotCount}";
        GetText((int)Texts.PierceCount).text = $"1 + {_rank.PierceCount}";
        GetText((int)Texts.PierceRate).text = $"{_rank.PierceRate}%";
        GetText((int)Texts.JumpingCount).text = $"1 + 0";
        GetText((int)Texts.JumpingRate).text = $"0%";


        foreach (StatModifier stat in _rank.StatModifiers)
        {
            if(stat.CodeName == Define.StatID.JumpingCount.ToString())
                GetText((int)Texts.JumpingCount).text = $"1 + {stat.Value}";
            else if(stat.CodeName == Define.StatID.JumpingRate.ToString())
                GetText((int)Texts.JumpingRate).text = $"{stat.Value }%";
        }




    }
}
