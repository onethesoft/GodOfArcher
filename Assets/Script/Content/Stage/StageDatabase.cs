using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="�������� �����ͺ��̽�")]
public class StageDatabase : ScriptableObject
{
    [SerializeField]
    List<StageTask> _stageList;

    public IReadOnlyList<StageTask> StageList => _stageList;

    public StageDataList CreateStageDataList()
    {
        StageDataList _ret = new StageDataList();
        _ret.List = new List<StageData>();

        foreach (StageTask stage in _stageList)
        {
            if(stage.EnterCondition.Count > 0)
                _ret.List.Add(stage.CreateStageData());
        }
        return _ret;
    }
}
