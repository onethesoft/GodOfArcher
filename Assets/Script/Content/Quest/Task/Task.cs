using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskState
{
    Inactive,
    Running,
    Complete
}

[CreateAssetMenu(menuName ="Quest/Task/Task" , fileName ="Task_")]
public class Task : ScriptableObject
{
    #region Events
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    #endregion

    [SerializeField]
    private Category _category;
    [SerializeField]
    private string _codeName;

    [SerializeField]
    private string _description;

    [Header("Action")]
    [SerializeField]
    private TaskAction _action;

    [Header("Target")]
    [SerializeField]
    private TaskTarget[] _targets;

    [Header("Setting")]
    [SerializeField]
    private int _needSuccessToComplete;
    [SerializeField]
    private InitialSuccessValue _initialSuccessValue;

  
    // 퀘스트 완료 후에도 계속 보고를 받을것인지
    [SerializeField]
    private bool _canReceiveReportDuringCompletion;

    private int _currentSuccess;
    public int CurrentSuccess
    {
        get => _currentSuccess;
        set
        {
            int prevSuccess = _currentSuccess;
            _currentSuccess = Mathf.Clamp(value, 0, NeedSuccessToComplete);
            if (_currentSuccess != prevSuccess)
            {
                State = _currentSuccess == NeedSuccessToComplete ? TaskState.Complete : TaskState.Running;
                onSuccessChanged?.Invoke(this, _currentSuccess, prevSuccess);
            }
           

        }
    }
    public string CodeName => _codeName;
    public string Description => _description;
    public int NeedSuccessToComplete => _needSuccessToComplete;


    public Category Category => _category;

    private TaskState _state;

    public Quest Owner { get; private set; }

    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;

    public TaskState State
    {
        get => _state;
        set
        {
            var prevState = _state;
            _state = value;
            onStateChanged?.Invoke(this, _state, prevState);
        }
    }


    public bool IsComplete => State == TaskState.Complete;
    public Task(Category Category , string CodeName , string target, int NeedSuccessToComplete)
    {
        _category = Category;
        _codeName = CodeName;
        _targets = new StringTarget[1];
        _targets[0] = new StringTarget(target);


        _action = new PositiveCount();
        _needSuccessToComplete = NeedSuccessToComplete;
        _currentSuccess = 0;


    }
    public void Init(Category Category, TaskTarget [] target, TaskAction action ,  int NeedSuccessToComplete)
    {
        _category = Category;
        _targets = target;
        _action = action;
        _currentSuccess = 0;
        _needSuccessToComplete = NeedSuccessToComplete;

    }
    public void ReceiveReport(int successCount)
    {
        CurrentSuccess = _action.Run(this, CurrentSuccess, successCount);
        
    }

    public bool IsTarget(string category, object target)
    {
        return Category == category && _targets.Any(x => x.IsEqual(target)) && (!IsComplete || (IsComplete && _canReceiveReportDuringCompletion));
    }


    public void SetUp(Quest owner)
    {
        Owner = owner;
    }

    public void Start()
    {
        State = TaskState.Running;
        if (_initialSuccessValue)
            _currentSuccess = _initialSuccessValue.GetValue(this);
    }

    public void End()
    {
        onSuccessChanged = null;
        onStateChanged = null;
    }

    public void Complete()
    {
        CurrentSuccess = NeedSuccessToComplete;
    }
    public void Reset()
    {
        _currentSuccess = 0;
        Start();
    }
}
