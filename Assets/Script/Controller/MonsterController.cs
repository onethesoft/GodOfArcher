using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Numerics;
using Spine.Unity;

using Vector3 = UnityEngine.Vector3;
using Assets.FantasyMonsters.Scripts;
//using Assets.FantasyMonsters.Scripts;

public class MonsterController : BaseController
{
    enum HPBar
    {
        UI_HPBar,
        //MonsterHp
    }
    public delegate void OnHitEventHandler(MonsterController controller);
    public event OnHitEventHandler OnHitEvent;
    public Vector3 CenterPos { 
        get
        {
            //return gameObject.transform.position;
            Vector3 ColliderCenter = gameObject.GetComponent<CapsuleCollider>().center;
            ColliderCenter.x *= gameObject.transform.localScale.x;
            ColliderCenter.y *= gameObject.transform.localScale.y;


            return gameObject.transform.position + ColliderCenter ;
        }
    }


    BigInteger _hp;
    BigInteger _maxHp;
    
    public Define.MonsterType MonsterType { get; set; }
    public void SetHp(Define.MonsterType monsterType , BigInteger hp)
    {
        _maxHp = hp;
        _hp = hp;

        MonsterType = monsterType;
        if (Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar) != null)
        {
            if(MonsterType == Define.MonsterType.Dongeon)   // 골드 던전에서 HP 보이도록
            {
                Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).Setup(hp);
                Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).gameObject.SetActive(true);
            }
            else if(MonsterType == Define.MonsterType.Boss)
            {
                Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).Setup(hp);
                Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).gameObject.SetActive(true);
            }
            else
            {
                Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).gameObject.SetActive(false);
            }
            /*
            if (MonsterType != Define.MonsterType.Boss)
                Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).gameObject.SetActive(false);
            else
            {
                Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).Setup(hp);
                Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).gameObject.SetActive(true);
               
             
            }
            */

        }

     
        /*
        if (Get<MonsterBar>((int)HPBar.MonsterHp) != null)
            Get<MonsterBar>((int)HPBar.MonsterHp).Setup(hp);
        */

    }
    public BigInteger GetHp()
    {
        return _hp;
    }
    public BigInteger GetMaxHp()
    {
        return _maxHp;
    }

    public override void Init()
    {
        base.Init();
        Bind<UI_MonsterHPBar>(typeof(HPBar));

        type = Define.WorldObject.Monster;

        if (MonsterType != Define.MonsterType.Boss)
        {
            if(MonsterType == Define.MonsterType.Normal)
                Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).gameObject.SetActive(false);
        }
        else
        {
            Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).gameObject.SetActive(true);
            Get<UI_MonsterHPBar>((int)HPBar.UI_HPBar).Setup(_maxHp);
        }




        //CenterPos = gameObject.transform.position;// + gameObject.GetComponent<CapsuleCollider>().center;
        Idle();

    }
    public void ShowHitEffect(Vector3 pos)
    {
        Managers.Resource.Instantiate("Skill/Destruction_air_blue").transform.position = pos;
    }

    public void OnHit(BigInteger Damage, Define.DamageType type)
    {
        if (State != Define.State.Idle)
            return;

        if (Managers.Setting.ShowDamage)
        {
            ShowHitAnimation(Damage, gameObject.transform.position, type);
        }
        if(Managers.Setting.EnableShake)
        {
            if(type == Define.DamageType.Critical )
                Managers.Scene.CurrentScene.GetCamera().DoShake( CameraController.Direction.ShakeHorizontal);
            else if(type == Define.DamageType.Skill)
                Managers.Scene.CurrentScene.GetCamera().DoShake( CameraController.Direction.ShakeVertical);
        }
        if (Managers.Setting.ShowEffect && type != Define.DamageType.Skill)
        {
           // Vector3 pos = other.ClosestPointOnBounds(transform.position);
            ShowHitEffect(CenterPos);
        }
        

        _hp -= Damage;
        OnHitEvent?.Invoke(this);

        if (Managers.Scene.CurrentScene is DarkMageScene)
        {

        }
        else
            if (_hp <= 0)
                Death();
        

                
        


    }

    
    public void ShowHitAnimation(BigInteger Damage , Vector3 pos , Define.DamageType type)
    {
        
        UI_DamageText go = Util.GetOrAddComponent<UI_DamageText>(Managers.Resource.Instantiate("UI/SubItem/DamageText", Managers.Scene.CurrentScene.gameObject.transform));

        // go.transform.position = Managers.Scene.CurrentScene.GetCamera().GetComponent<Camera>().WorldToScreenPoint(pos);
        if (type == Define.DamageType.Skill)
            go.transform.position = pos + Vector3.right + (Vector3.up * 0.3f);
        else
            go.transform.position = pos;

        go.SetDamage(Util.GetBigIntegerUnit(Damage), type);

        go.Play();
        

    }
    public void Idle()
    {
        State = Define.State.Idle;
        Monster _monster = gameObject.GetComponent<Monster>();
        SkeletonAnimation anim = gameObject.GetComponent<SkeletonAnimation>();
        if (anim != null)
            anim.AnimationState.SetAnimation(0, "Idle", true);
        else if (_monster != null)
            _monster.SetState(MonsterState.Idle);
        
    }
    public void Death()
    {

        Monster _monster = gameObject.GetComponent<Monster>();
        SkeletonAnimation anim = gameObject.GetComponent<SkeletonAnimation>();
        if(anim != null)
            anim.AnimationState.SetAnimation(0, "Dead", false);
        else if(_monster != null)
        {
            _monster.Die();
        }

        State = Define.State.Death;

    }
    public void OnEnable()
    {
        Idle();
        
    }
    protected override void Release()
    {
        base.Release();
        OnHitEvent = null;
    }



}
