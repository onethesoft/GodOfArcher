using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(menuName = "스킬/기본스킬", fileName = "기본스킬")]
public class SkillData : ScriptableObject
{
    [SerializeField]
    string _name;
    public string Name => _name;


    [SerializeField]
    double _currentCoolTime = 0.0;
    public double CurrentCoolTime => _currentCoolTime;


    public double CoolTime => Managers.Game.GetSkillCoolTime;

    [SerializeField]
    double _maxCooltime;
    public double MaxCoolTime => _maxCooltime;


    // 버프 부여 제외한 최소 쿨타임
    [SerializeField]
    double _minCcooltime;
    public double MinCoolTime => _minCcooltime;


    [SerializeField]
    Define.SK_Status _status;

    public Define.SK_Status Status
    {
        get
        {
            return _status;
        }
        private set
        {
            _status = value;
            if (_status == Define.SK_Status.Cooldown)
                _currentCoolTime = 0.0;
        }
    }

    [SerializeField]
    GameObject _effect;
    public GameObject Effect => _effect;

    [SerializeField]
    Activator _activator;

    public void Activate()
    {
    
        if (_activator != null && Status == Define.SK_Status.Ready)
            _activator.DoActivate(this);

        Status = Define.SK_Status.Cooldown;
    }

    public void Reset()
    {
        Status = Define.SK_Status.Cooldown;
    }

    public void AddCoolDown(double deltaTime)
    {
        if(Status == Define.SK_Status.Cooldown)
        {
            _currentCoolTime += deltaTime;
            if (_currentCoolTime >= CoolTime)
                Status = Define.SK_Status.GlobalCooldown;
        }
        else if(Status == Define.SK_Status.GlobalCooldown)
        {
            Status = Define.SK_Status.Ready;
        }
    }


    public double RemainingTime()
    {
        return CoolTime - _currentCoolTime;
    }



    public SkillData Clone()
    {
        SkillData _ret = CreateInstance<SkillData>();

        _ret._name = Name;
        _ret._status = Status;
        _ret._minCcooltime = _minCcooltime;
        _ret._maxCooltime = _maxCooltime;
        _ret._effect = Effect;
        _ret._activator = _activator;
        return _ret;

    }






}
