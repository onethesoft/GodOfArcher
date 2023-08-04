using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class UI_MonsterHPBar : UI_Base
{
    enum Sliders
    {
        Slider
    }
    BigInteger _maxHp;
   
    public BigInteger MaxHp { 
        get
        {
            return _maxHp;
        }
        private set
        {
            _maxHp = value;
        }
    }

    BigInteger _hp;
    public BigInteger Hp
    {
        get
        {
            return _hp;
        }
        private set
        {
            if (value > 0)
                _hp = value;
            else
                _hp = 0;

            if(Get<Slider>((int)Sliders.Slider) != null && _maxHp > 0)
            {
                double ratio = Math.Exp(BigInteger.Log(_hp) - BigInteger.Log(_maxHp));
                if(ratio != Double.NaN)
                    Get<Slider>((int)Sliders.Slider).value = (float)ratio;
              

            }
        }
    }

    public override void Init()
    {
        Bind<Slider>(typeof(Sliders));
        MonsterController _parent = gameObject.GetComponentInParent<MonsterController>();
        _maxHp = _parent.GetMaxHp();
        _parent.OnHitEvent -= OnHit;
        _parent.OnHitEvent += OnHit;


    }
   public void Setup(BigInteger maxhp)
    {

        _maxHp = maxhp;
        Hp = maxhp;


    }
    
    void OnHit(MonsterController parent)
    {
        Hp = parent.GetHp();
    }
    
}
