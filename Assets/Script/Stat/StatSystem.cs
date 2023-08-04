using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stats<T> : ScriptableObject  where T : BaseStat  
{
    [SerializeField]
    protected List<T> _stats;

    public IReadOnlyList<T> StatList => _stats;
    public IReadOnlyDictionary<string, T> StatDict => _stats.ToDictionary(x => x.CodeName, x => x);

    public virtual void Add(T item)
    {
        if (_stats == null)
            _stats = new List<T>();
        //item.Parent = this;
        _stats.Add(item);
    }
    public T Get(string CodeName)
    {
        return _stats.Find(x => x.CodeName == CodeName);
    }

    public int GetStatValue(string CodeName)
    {
        foreach(T stat in _stats)
        {
            if (stat.CodeName == CodeName)
            {
                return stat.GetValue;
            }
        }

        
        return 0;
        
        //return StatDict.ContainsKey(CodeName) ? StatDict[CodeName].GetValue : 0;
    }

    public void AddModfier(StatModifier _modifier)
    {
        foreach(T stat in _stats)
            stat.AddModifier(_modifier);
        
    }
    public void RemoveModfier(StatModifier _modifier)
    {
        foreach (T stat in _stats)
        {
            stat.RemoveModifier(_modifier);
        }

    }

    public void AddModfierList(List<StatModifier> _modifierList)
    {
        foreach (StatModifier mod in _modifierList)
            AddModfier(mod);
    

    }
    public void RemoveModfierList(List<StatModifier> _modifierList)
    {
        foreach (StatModifier mod in _modifierList)
            RemoveModfier(mod);

    }
    public void ClearModifier()
    {
        foreach (T stat in _stats)
            stat.ClearModifier();

    }
  



}
[CreateAssetMenu(menuName ="플레이어스텟시스템" ,fileName ="PlayerStatSystem")]
public class CharacterStatSystem : Stats<CharacterStat>
{

    public override void Add(CharacterStat item)
    {
        base.Add(item);
        item.Parent = this;
        item.OnLevelChanged -= OnLevelChanged;
        item.OnLevelChanged += OnLevelChanged;


    }
    public void Reset(Define.StatType type = Define.StatType.Gold)
    {
        foreach (CharacterStat stat in StatList)
        {
            if(type == stat.type)
                stat.Reset();
        }
    }
    public CharacterStatSystemSaveData ToSaveData()
    {
        return new CharacterStatSystemSaveData { StatList = StatList.Select(x => x.ToSaveData()).ToList() };
    }
    public void Load(CharacterStatSystemSaveData data)
    {
        foreach(CharacterStatSaveData savedata in data.StatList)
            StatList.ToList().ForEach(x => x.Load(savedata));
        
        
    }
    public void OnLevelChanged(CharacterStat stat , int LevelChanged)
    {
        foreach(CharacterStat _stat in _stats)
        {
            _stat.Unlock(stat);
        }

        if(stat.CodeName == Define.StatID.SkillMaxLevelLimit.ToString())
        {
            _stats.Where(x => x.CodeName == Define.StatID.SkillAttack.ToString()).FirstOrDefault().OnChangedMaxLevel(stat, LevelChanged);
        }
    }
    public void OnPlayerLevelChanged(int playerRank)
    {
        foreach (CharacterStat _stat in _stats)
        {
            _stat.Unlock(playerRank);
        }
    }

    


}
[CreateAssetMenu(menuName = "스텟시스템", fileName = "StatSystem")]
public class StatSystem : Stats<BaseStat>
{
    

}
