using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/*
 * Quest Sequence System
 */

[System.Serializable]
[CreateAssetMenu(menuName ="Quest/QuestGroup")]
public class QuestGroup : ScriptableObject
{
    #region Events
    public delegate void QuestCompletedHandler(List<Quest> quest);
    #endregion

   

    public enum Mode
    {
        Serial ,
        Parallel
    }

   
    [SerializeField]
    List<Quest> _questList;

   
   

    [SerializeField]
    int _progress = 0;

    [SerializeField]
    Mode _groupMode;

    [SerializeField]
    Define.QuestGroupType _type;

    public Define.QuestGroupType GetQuestGroupType => _type;

    [SerializeField]
    DateTime _lastClearTime;

  
    List<Quest> _completableQuests = new List<Quest>();

    

    public bool IsCompleteGroup => _progress >= _questList.Count ? true : false;
    public Quest ActiveQuest => IsCompleteGroup ?  null : _questList[_progress];

    [SerializeField]
    Mode _mode = Mode.Serial;
    public Mode mode => _mode;

    [SerializeField]
    string _codeName;
    public string CodeName => _codeName;

   

  
    public event QuestCompletedHandler onQuestCompleted;

    public QuestGroup(string CodeName, Mode mode)
    {
        _mode = mode;
        _codeName = CodeName;

        _questList = new List<Quest>();
        _completableQuests = new List<Quest>();
    }
    

    public void Setup(int QuestIndex = 0)
    {
        Debug.Assert(QuestIndex <= _questList.Count + 1 && QuestIndex >= 0);

        for(int i=0;i<_questList.Count;i++)
        {
            RegisterQuest(_questList[i]);
            if (i < QuestIndex)
            {
                _questList[i].Complete();
            }

        }

        _progress = QuestIndex;



    }

    public Quest RegisterQuest(Quest quest)
    {

        quest.OnRegister();
        return quest;
    }


    public void ReceiveReport( string category, object target, int successCount)
    {
       
        if(mode == Mode.Serial)
            _questList[_progress].ReceiveReport(category, target, successCount);
        else
        {
            for(int i=_progress;i<_questList.Count;i++)
                _questList[i].ReceiveReport(category, target, successCount);  
        }
    }


    public List<Quest> RequestCompleteQuest()
    {
        List<Quest> _completeQuests = new List<Quest>();
        for (int i = _progress; i < _questList.Count; i++)
        {
            if (_questList[i].IsCompletable)
            {
                _questList[i].Complete();
                _completeQuests.Add(_questList[i]);
            }
            
        }
        _progress += _completeQuests.Count;
        return _completeQuests;


    }
    public int GetCompletableQuest()
    {
        int _ret = 0;
        for (int i = _progress; i < _questList.Count; i++)
        {
            if (_questList[i].IsCompletable)
                _ret++;
        }
        return _ret;
    }
    public void CheckComplete()
    {
        _completableQuests.Clear();

        
        for (int i = _progress; i < _questList.Count; i++)
        {
            _questList[i].Complete();
            if(_questList[i].IsComplete) _completableQuests.Add(_questList[i]);
        }

        if (_completableQuests.Count != 0)
        {
            _progress += _completableQuests.Count;
            onQuestCompleted?.Invoke(_completableQuests);
        }

        


    }
    public QuestGroup Clone()
    {
        QuestGroup _ret = new QuestGroup(_codeName, _mode);
        _ret._type = this._type;
        foreach (Quest _quest in _questList)
            _ret._questList.Add(_quest.Clone());

        return _ret;
    }
   
    

}
