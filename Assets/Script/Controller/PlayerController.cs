using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common;
using System;
using System.Linq;


public class PlayerController : BaseController
{
    enum Transfomrs
    {
        ArmL,
        FireTransform
    }
    Transform weapon;
    Animator _anim;
    Character _character;

    // 2.2 ~ 0.2(max)
    public float attakspeed = 2.2f;

    // max = 70, min=20
    public float _arrowspeed = 20.0f;

    [SerializeField]
    QuestReporter _playTimeReporter;

    Vector3 _tarpos = new Vector3(4, -1, 0);

    [SerializeField]
    GameObject Target;

    int _attackState = 0;

    float _attackTime = 0f;
    float _itemTime = 0f;

    GameData _playerData;

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(Transfomrs));

        _anim = gameObject.GetComponent<Character>().Animator;
        _character = gameObject.GetComponent<Character>();
       
        weapon = _character.BowRenderers[3].transform;
    

        _character.Animator.GetComponent<AnimationEvents>().OnCustomEvent += OnHitEvent;

        _character.GetReady();
        if (_character == null)
            Debug.LogError("Player Prefab Error");

        _playerData = GetComponent<GameData>();
    }
   
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    protected override void UpdateIdle() {

       
        if (Managers.Game.AutoForTest && CanAttack()) // isauto
        {
            UpdateState(Define.State.Attack);
        }
        _itemTime += Time.deltaTime;

       

    }
    protected override void UpdateAttack() {
       
        _attackTime += Time.deltaTime;
        if (_attackTime >= Managers.Game.GetAttackSpeed)
        {
            int _temp = _attackState;
        
            _attackTime = 0;
            _attackState = (_attackState + 1) % 3;
            _character.Animator.SetInteger("Charge", _attackState);

            if (_temp == 2)
            {
                UpdateState(Define.State.Idle);
            }



        }


    }
    protected override void UpdateMoving() { }
    protected override void UpdateHurt() { }
    protected override void UpdateDeath() { }
    protected override void OnUpdate() { }

   

    bool CanAttack()
    {

        if (Managers.Scene.CurrentScene.CurrentState != BaseScene.GameState.Run) return false;
        if (State != Define.State.Idle) return false;


        if (Managers.Game.GetNextTarget() == null) return false;

        // 1. 스킬 타격후에도 남은 몬스터가 존재하는지 체크
        // 2. 공격하기전에 스킬이 발동할 수 있는지 체크
        if(Managers.Scene.CurrentScene.SceneType != Define.Scene.DarkMage)

            if (Managers.Game.GetNextTarget().GetComponent<MonsterController>().GetHp() <= Managers.Game.GetSkillDamage &&
                _playerData.SkillSet.First().RemainingTime() <= Managers.Game.GetAttackSpeed * 2 &&
                _playerData.SkillSet.First().Status == Define.SK_Status.Cooldown
                )
            {
                return false;
            }
               

     
        return true;
    }

    void UpdateState(Define.State state)
    {
        switch(state)
        {
            case Define.State.Attack:
                if (!_character.IsReady())
                    _character.SetState(CharacterState.Ready);
                _attackTime = 0;
                _attackState = 0;

                break;
            case Define.State.Idle:
                _itemTime = 0;
               
                //Target = Managers.Game.GetNextTarget() != null ? Managers.Game.GetNextTarget() : null;
                break;
        }

        State = state;
      
    }
    public void ResetAnimation()
    {
        if (gameObject.activeSelf)
        {
            _character.ResetAnimation();
            _character.Animator.SetInteger("Charge", 3);  // Cancel
          

        }
        UpdateState(Define.State.Idle);
       
    }

    public IEnumerator Ready()
    {
        yield return new WaitForSeconds(0.1f);
       
    }
    public IEnumerator Shoot()
    {

        _character.Animator.SetInteger("Charge", 1); // 0 = ready, 1 = charging, 2 = release, 3 = cancel.

        yield return new WaitForSeconds(Managers.Game.GetAttackSpeed);

        _character.Animator.SetInteger("Charge", 2);

        yield return new WaitForSeconds(Managers.Game.GetAttackSpeed);

        _character.Animator.SetInteger("Charge", 0);
        State = Define.State.Idle;

        
    }
    public void OnHitEvent(string name)
    {
        if (name == "BowCharged")
        {
            
        }
        else if(name == "BowReleased")
        {
            if(Target != null)
                CreateArrow();

           
        }
    }
    void UpdateTarget()
    {
        Target = Managers.Game.GetNextTarget();
       

    }
    private void LateUpdate()
    {
       
        UpdateTarget();
        if (Target == null)
        {
            RotateArm(Get<GameObject>((int)Transfomrs.ArmL).transform, weapon, _tarpos, -40, 40);
            return;
        }
        MonsterController _getMonster;
        if (_character.IsReady())
        {
            if (Managers.Game.GetNextTarget().activeSelf)
            {
                _getMonster = Managers.Game.GetNextTarget().GetComponent<MonsterController>();
                RotateArm(Get<GameObject>((int)Transfomrs.ArmL).transform, weapon, _getMonster.CenterPos, -40, 40);
            }
        }
        
            
    }

    public void RotateArm(Transform arm, Transform weapon, Vector2 target, float angleMin, float angleMax) // TODO: Very hard to understand logic.
    {
        target = arm.transform.InverseTransformPoint(target);
        
        var angleToTarget = Vector2.SignedAngle(Vector2.right, target);
        var angleToArm = Vector2.SignedAngle(weapon.right, arm.transform.right) * Math.Sign(weapon.lossyScale.x);
        var fix = weapon.InverseTransformPoint(arm.transform.position).y / target.magnitude;

        
        if (fix < -1) fix = -1;
        else if (fix > 1) fix = 1;

        var angleFix = Mathf.Asin(fix) * Mathf.Rad2Deg;
        var angle = angleToTarget + angleFix + arm.transform.localEulerAngles.z;

        angle = NormalizeAngle(angle);
      

        if (angle > angleMax)
        {
            angle = angleMax;
        }
        else if (angle < angleMin)
        {
            angle = angleMin;
        }
     
        
        if (float.IsNaN(angle))
        {
            Debug.LogWarning(angle);
        }

        arm.transform.localEulerAngles = new Vector3(0, 0, angle + angleToArm);
        Get<GameObject>((int)Transfomrs.ArmL).transform.localEulerAngles = arm.transform.localEulerAngles;

    }

    private static float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;

        return angle;
    }

    private void CreateArrow()
    {
        PierceController _arrscript = Util.GetOrAddComponent<PierceController>(Managers.Game.Spawn(Define.WorldObject.Arrow, "Player/Arrow", Get<GameObject>((int)Transfomrs.FireTransform).transform));

        var arrow = _arrscript.gameObject;
        var sr = arrow.GetComponent<SpriteRenderer>();
        var rb = arrow.GetComponent<Rigidbody>();


        arrow.transform.localPosition = Vector3.zero;
        arrow.transform.localRotation = Quaternion.identity;

        arrow.transform.SetParent(null);
        sr.sprite = _character.Bow.Single(j => j.name == "Arrow");

        
        rb.velocity = (float)30.0f * Get<GameObject>((int)Transfomrs.FireTransform).transform.right * Mathf.Sign(_character.transform.lossyScale.x) * UnityEngine.Random.Range(0.85f, 1.15f);

        var characterCollider = _character.GetComponent<Collider>();

        if (characterCollider != null)
        {
            Physics.IgnoreCollision(arrow.GetComponent<Collider>(), characterCollider);
        }

        arrow.gameObject.layer = 31; // TODO: Create layer in your project and disable collision for it (in physics settings)
        Physics.IgnoreLayerCollision(31, 31, true); // Disable collision with other projectiles.
        Managers.Setting.Play("Arrow");
    }

}
