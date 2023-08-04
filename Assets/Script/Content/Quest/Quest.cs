using com.onethesoft.GodOfArcher;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


public enum QuestState
{
    Inactive,
    Running,
    Complete,
    Cancel,
    WaitingForCompletion
}

[CreateAssetMenu(menuName = "Quest/Quest", fileName = "Quest_")]
public class Quest : ScriptableObject
{
    #region event
    public delegate void TaskSuccessChangeHandler(Quest quest, Task task, int currentSuccess, int prevSuccess);
    public delegate void CompleteHandler(Quest quest);
    public delegate void CancelHandler(Quest quest);
    public delegate void NewTaskGroupHandler(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup);
    public delegate void ResetHandler(Quest quest);

    #endregion
    [SerializeField]
    private Category _category;
    [SerializeField]
    private Sprite _icon;
    

    [Header("Text")]
    [SerializeField]
    private string _codeName;
    [SerializeField]
    private string _description;
    [SerializeField, TextArea]
    private string _displayName;


    [Header("Task")]
    [SerializeField]
    private TaskGroup[] _taskGroups;

    [Header("Reward")]
    [SerializeField]
    private Reward[] _rewards;

    [Header("Option")]
    [SerializeField]
    private bool _useAutoComplete = false;
    [SerializeField]
    private bool _isCancelable = false;
    [SerializeField]
    private bool _isSavable;
    [SerializeField]
    private bool _isRepeatable = false;
   

    [Header("Condition")]
    [SerializeField]
    private Condition[] _acceptionconditions;
    [SerializeField]
    private Condition[] _cancelconditions;

    [Header("Helper")]
    [SerializeField]
    private QuestHelper _helper;



    private int _currentTaskGroupIndex = 0;

    public Category Category => _category;
    public Sprite Icon => _icon;
    
    public string CodeName => _codeName;
    public string DisplayName => _displayName;
    public string Description => _description;

    public QuestState State { get; private set; }
    public TaskGroup CurrentTaskGroup => _taskGroups[_currentTaskGroupIndex];

    public IReadOnlyList<TaskGroup> TaskGroups => _taskGroups;
    public IReadOnlyList<Reward> Rewards => _rewards;
    public bool IsRegisterd => State != QuestState.Inactive;

    public bool IsCompletable => State == QuestState.WaitingForCompletion;
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;
    public virtual bool IsCancelable => _isCancelable && _cancelconditions.All(x => x.IsPass(this));

    public bool IsAcceptable => _acceptionconditions == null ? true :  _acceptionconditions.All(x => x.IsPass(this));
    public virtual bool IsSavable => _isSavable;
    public bool IsRepeatable => _isRepeatable;
    public bool IsQuestHelper => _helper == null ? false : true;
    public QuestHelper GetQuestHelper => _helper;
   

    public event TaskSuccessChangeHandler onTaskSuccessChanged = null;
    public event CompleteHandler onCompleted = null;
    public event CancelHandler onCanceled = null;
    public event NewTaskGroupHandler onNewTaskGroup = null;
    public event ResetHandler OnReseted = null;

    public void Init(string codeName,Category category, TaskGroup [] groups , Sprite icon ,  string description , Reward [] rewards, string displayName = null)
    {
        _codeName = codeName;
        _category = category;
        _taskGroups = groups;
        _icon = icon;
        _description = description;
        _rewards = rewards;

        if (!string.IsNullOrEmpty(displayName))
            _displayName = displayName;
    }
    
    public void SetAcceptableConditions(Condition [] _conditions)
    {
        _acceptionconditions = _conditions;
    }
    public void SetQuestHelper(QuestHelper helper)
    {
        _helper = helper;
    }


    public void OnRegister()
    {
        Debug.Assert(!IsRegisterd, "This Quest has already been registered");
       
        foreach (var taskGroup in _taskGroups)
        {
            taskGroup.Setup(this);
            foreach (var task in taskGroup.Tasks)
            {
                task.onSuccessChanged += OnSuccessChanged;

            }
        }

        State = QuestState.Running;
        CurrentTaskGroup.Start();
    }

    public void ReceiveReport(string category, object target, int successCount)
    {
        //Debug.Assert(!IsRegisterd, "This Quest has already been registered");
        Debug.Assert(!IsCancel, "This Quest has been canceled");
        if (IsComplete)
            return;
        if (!IsAcceptable)
            return;

        CurrentTaskGroup.ReceiveReport(category, target, successCount);

        if (CurrentTaskGroup.IsAllTaskComplete)
        {
            if (_currentTaskGroupIndex + 1 == _taskGroups.Length)
            {
                State = QuestState.WaitingForCompletion;
                if (_useAutoComplete)
                    Complete();
            }
            else
            {
                var prevTaskGroup = _taskGroups[_currentTaskGroupIndex++];
                prevTaskGroup.End();
                CurrentTaskGroup.Start();
                onNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
        }
        else
        {
            State = QuestState.Running;
        }

    }

    public void Complete()
    {
        CheckIsRunning();

        foreach (var taskGroup in _taskGroups)
            taskGroup.Complete();

        State = QuestState.Complete;

        foreach (var reward in _rewards)
            reward.Give(this);

        onCompleted?.Invoke(this);

        onTaskSuccessChanged = null;
        onCompleted = null;
        onCanceled = null;
        onNewTaskGroup = null;
    }
    public virtual void Cancel()
    {
        CheckIsRunning();
        Debug.Assert(IsCancelable, "This quest can't be canceled");

        State = QuestState.Cancel;
        onCanceled?.Invoke(this);
    }

    public Quest Clone()
    {
        var clone = Instantiate(this);
        clone._taskGroups = _taskGroups.Select(x => new TaskGroup(x)).ToArray();
        return clone;
    }

    public QuestSaveData ToSaveData()
    {
       
        return new QuestSaveData { CodeName = CodeName, state = State, TaskGroupIndex = _currentTaskGroupIndex, TaskSuccessCount = CurrentTaskGroup.Tasks.Select(x => x.CurrentSuccess).ToArray() };
    }

    public void CopySaveData(QuestSaveData saveData)
    {
        saveData.CodeName = CodeName;
        saveData.state = State;
        saveData.TaskGroupIndex = _currentTaskGroupIndex;
        saveData.TaskSuccessCount = CurrentTaskGroup.Tasks.Select(x => x.CurrentSuccess).ToArray();
       
    }
    public void LoadFrom(QuestSaveData data)
    {
        State = data.state;
        _currentTaskGroupIndex = data.TaskGroupIndex;
        

        for (int i = 0; i < _currentTaskGroupIndex; i++)
        {
            var taskgroup = _taskGroups[i];
            taskgroup.Start();
            taskgroup.Complete();
        }

        CurrentTaskGroup.Start();
        for (int i = 0; i < data.TaskSuccessCount.Length; i++)
            CurrentTaskGroup.Tasks[i].CurrentSuccess = data.TaskSuccessCount[i];


    }

    private void OnSuccessChanged(Task task, int currentSuccess, int prevSuccess)
    {

        onTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);
    }
    public void Reset()
    {
        if(_isRepeatable)
        {
            State = QuestState.Running;
            foreach (var taskGroup in _taskGroups)
            {
                taskGroup.Reset();
                foreach(var task in taskGroup.Tasks)
                {
                    task.onSuccessChanged -= OnSuccessChanged;
                    task.onSuccessChanged += OnSuccessChanged;
                }
            }
            _currentTaskGroupIndex = 0;
            OnReseted?.Invoke(this);

        }
    }
    public void DoHelp()
    {
        if (_helper != null)
            _helper.DoHelp();
    }


    [Conditional("UNITY_EDITOR")]
    private void CheckIsRunning()
    {
        Debug.Assert(!IsRegisterd, "This Quest has already been registered");
        Debug.Assert(!IsCancel, "This Quest has been canceled");
        Debug.Assert(!IsCompletable, "This Quest has already been completed");
    }



    
}
