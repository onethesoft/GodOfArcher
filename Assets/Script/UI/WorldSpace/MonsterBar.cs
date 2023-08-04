using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MonsterBar : UI_Base
{
    enum GameObjects
    {
        Bar
    }

    BigInteger _maxHp;
    public BigInteger MaxHp
    {
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

            if (Get<GameObject>((int)GameObjects.Bar) != null && _maxHp > 0)
            {
                double ratio = Math.Exp(BigInteger.Log(_hp) - BigInteger.Log(_maxHp));
                if (ratio != Double.NaN)
                {
                    Material _mat = Get<GameObject>((int)GameObjects.Bar).GetComponent<Renderer>().material;
                    _mat.SetFloat("_ClipUvRight",1.0f - (float)ratio);
                }

            }
        }
    }

    

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));


        Get<GameObject>((int)GameObjects.Bar).GetComponent<Renderer>().material = Managers.Resource.Load<Material>("Prefabs/Material/Bar");
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
