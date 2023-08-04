using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DamageMeter : UI_Base
{
    enum Texts
    {
        //Text,
        //DamageText
    }
    enum TMP_Texts
    {
        Text,
        DamageText
    }
    MonsterController _monster;
    public override void Init()
    {
        //Bind<Text>(typeof(Texts));
        Bind<TMP_Text>(typeof(TMP_Texts));

        _monster = gameObject.GetComponentInParent<MonsterController>();
    }
    private void Update()
    {
        Get<TMP_Text>((int)TMP_Texts.DamageText).text = Util.GetBigIntegerUnit(System.Numerics.BigInteger.Abs(_monster.GetHp()));

      
    }


}
