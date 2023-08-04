using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : UI_Base
{

    [SerializeField]
    private Define.State _state = Define.State.Idle;
    private WaitForSeconds _delay;

    public Define.State State
    {
        get
        {
            return _state;
        }
        protected set
        {
            _state = value;
            if(_state == Define.State.Death)
            {
                OnStateChanged?.Invoke(_state, gameObject);
                OnStateChanged = null;
            }
            else
                OnStateChanged?.Invoke(_state, gameObject);
        }
    }

    public Define.WorldObject type = Define.WorldObject.Unknown;

    public delegate void OnStateChangedHandler(Define.State state, GameObject gameobject);
    public event OnStateChangedHandler OnStateChanged;

    public delegate void OnDestroyObject(GameObject gameobject);
    public event OnDestroyObject OnDestroyed;
    public override void Init()
    {
        _delay = new WaitForSeconds(0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Init();
        Init();
    }

    // Update is called once per frame
    void Update()
    {

        switch (_state)
        {
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Attack:
                UpdateAttack();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.Hurt:
                UpdateHurt();
                break;
            case Define.State.Death:
                UpdateDeath();
                break;

        }
        OnUpdate();
    }

    protected virtual void UpdateIdle() {
      
    }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateHurt() {
       
    }
    protected virtual void UpdateDeath() {
       
    }
    protected virtual void OnUpdate() {
        
    }

    private void InvokeState(Define.State state)
    {
        OnStateChanged?.Invoke(state, gameObject);

        if (state == Define.State.Death)
        {
            OnStateChanged = null;

        }
    }
    private IEnumerator InvokeState(Define.State state , float delay)
    {
        yield return _delay;
        OnStateChanged?.Invoke(state, gameObject);

        if (state == Define.State.Death)
        {
            OnStateChanged = null;

        }
    }

    private void OnDestroy()
    {
        Release();
        OnDestroyed?.Invoke(gameObject);
        OnStateChanged = null;
        
    }

    protected virtual void Release()
    {

    }

}
