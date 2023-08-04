using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Assets.HeroEditor.Common.ExampleScripts;
using System.Linq;
using System;

public class PierceController : MonoBehaviour
{
    public delegate void OnDestroyHandler(GameObject go);
    public event OnDestroyHandler OnDestroyed;

    [Header("관통숫자")]
    [SerializeField]
    public int _PierceCount;

    bool _istriggered = false;

    [Header("추가타격이펙트")]
    [SerializeField]
    GameObject _effect;


    double _delay = 0.01;
    TimeSpan _time;

    Define.DamageType _damageType = Define.DamageType.Normal;
    System.Numerics.BigInteger _damage;

   

    public List<Renderer> Renderers;
    public GameObject Trail;
    public GameObject Impact;
    public Rigidbody Rigidbody;
    float _ramainingTime = 0.0f;

    
    public void Start()
    {
       
        //Destroy(gameObject, 5);
        // _PierceCount = Managers.Game.GetPierceCount + 1;
        _PierceCount = 1;
        _istriggered = false;

        if(Managers.Game.IsCriticalHit == true && Managers.Game.CriticalHitDamage != Managers.Game.Damage)
        {
            _damageType =Define.DamageType.Critical;
            _damage = Managers.Game.CriticalHitDamage;
        }
        else
        {
            _damageType = Define.DamageType.Normal;
            _damage = Managers.Game.Damage ;
        }


        _time = TimeSpan.FromSeconds(_delay);
         OnDestroyed -= Managers.Game.Despawn;
        OnDestroyed += Managers.Game.Despawn;

    }
    private void OnEnable()
    {
        _ramainingTime = 0.0f;
        

        if (Managers.Game.IsCriticalHit == true && Managers.Game.CriticalHitDamage != Managers.Game.Damage)
        {
            _damageType = Define.DamageType.Critical;
            _damage = Managers.Game.CriticalHitDamage;
        }
        else
        {
            _damageType = Define.DamageType.Normal;
            _damage = Managers.Game.Damage;
        }

    }

    public void Update()
    {
        if (Rigidbody != null && Rigidbody.useGravity)
        {
            transform.right = Rigidbody.velocity.normalized;
        }
        _ramainingTime += Time.deltaTime;
        if (_ramainingTime >= 5.0f)
            OnDestroyed?.Invoke(gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        _istriggered = false;

    }
    public void OnTriggerEnter(Collider other)
    {
        //Bang(other.gameObject);
      
        if (other.gameObject.GetComponent<MonsterController>() != null && _istriggered == false)
        {
            
            ReplaceImpactSound(other.gameObject);
            //Impact.SetActive(true);

           
            Managers.Game.Hit(other.GetComponent<MonsterController>(), _damageType, _damage);
            if (_PierceCount - 1 <= 0)
            {
                Bang(other.gameObject);
            
                int _damaged = 0;
               
                if(Managers.Game.IsPierce)
                    foreach (GameObject monster in Managers.Game.GetTotalTarget())
                    {
                        if (monster == other.gameObject)
                            continue;
                        else
                        {
                            if( _damaged < Managers.Game.GetPierceCount)
                            {
                                Managers.Game.Hit(monster.GetComponent<MonsterController>(), _damageType, _damage);
                                _damaged++;
                            }   
                        }
                    }
                
            }
            else
            {
                if(other.gameObject.GetComponent<MonsterController>().State != Define.State.Death)
                    _istriggered = true;
            }
            _PierceCount--;
        }
    }

    
  
   

    private void Bang(GameObject other)
    {

        ReplaceImpactSound(other);
        //Impact.SetActive(true);

        //Destroy(GetComponent<SpriteRenderer>());
        //Destroy(GetComponent<Rigidbody>());
        //Destroy(GetComponent<Collider>());
        //Destroy(gameObject, 0.1f);
        /*
        foreach (var ps in Trail.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }

        foreach (var tr in Trail.GetComponentsInChildren<TrailRenderer>())
        {
            tr.enabled = false;
        }
        */
        OnDestroyed?.Invoke(gameObject);
    }

    private void ReplaceImpactSound(GameObject other)
    {
        var sound = other.GetComponent<AudioSource>();

        if (sound != null && sound.clip != null)
        {
            Impact.GetComponent<AudioSource>().clip = sound.clip;
        }
    }
    private void OnDestroy()
    {
        OnDestroyed = null;
    }

}
