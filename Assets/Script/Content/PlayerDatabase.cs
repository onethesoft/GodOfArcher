using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "PlayerDatabase", menuName = "�÷��̾�/�����ͺ��̽�")]
public class PlayerDatabase : ScriptableObject
{
    [System.Serializable]
    public class KeyValuePair<K,V>
    {
        public K Key;
        public V Value;
    }

    [System.Serializable]
    public class ReviveBonusData
    {
        public int Stage;
        public int RubyBonusCount;
        public int IncrementReviveLevel;
    }

    [Header("�÷��̾� ���")]
    [SerializeField]
    List<PlayerRank> _listrank;

    [Header("ȭ��")]
    [SerializeField]
    List<Currency> _listCurrency;

    [Header("�÷��̾� ����")]
    [SerializeField]
    List<CharacterStat> _listStat;

    [Header("�÷��̾� ��������")]
    [SerializeField]
    List<CharacterStatUpgradeCondition> _listcondition;

    [Header("���� ����")]
    [SerializeField]
    List<BaseStat> _listArtifactStat;

  
    [Header("�� ����")]
    [SerializeField]
    List<BaseStat> _listruneStat;

    [Header("�� ����")]
    [SerializeField]
    List<BaseStat> _listpetStat;

    [Header("���� ����")]
    [SerializeField]
    List<BaseStat> _listBuffStat;

    [Header("������ ����")]
    [SerializeField]
    List<BaseStat> _listItemStat;

    [Header("��ų")]
    [SerializeField]
    List<SkillData> _listSkill;

    [Header("ȯ�� ��� ���ʽ�")]
    [SerializeField]
    List<ReviveBonusData> _listReviveBonus;

    [Header("ȯ�� ���� Ƚ��")]
    [SerializeField]
    int _reviveCountLimit = 10;

    public CharacterStatUpgradeCondition Get => _listcondition[0];

    public IReadOnlyList<PlayerRank> RankList => _listrank;
    public IReadOnlyDictionary<int, PlayerRank> RankDic => _listrank.ToDictionary(keySelector: m => (int)m.Level, elementSelector: m => m);


    public IReadOnlyList<Currency> CurrencyList => _listCurrency;

    public IReadOnlyDictionary<string , Currency> CurrencyDict => _listCurrency.ToDictionary(keySelector: m => m.CodeName, elementSelector: m => m);

    public IReadOnlyList<CharacterStat> StatList => _listStat;
    public IReadOnlyDictionary<Define.StatID, CharacterStat> StatDic => _listStat.ToDictionary(keySelector: m => (Define.StatID)Enum.Parse(typeof(Define.StatID), m.CodeName), elementSelector: m => m);

    public IReadOnlyList<BaseStat> ArtifactStatList => _listArtifactStat;




    public IReadOnlyList<BaseStat> RuneStatList => _listruneStat;


    public IReadOnlyList<BaseStat> PetStatList => _listpetStat;
    public IReadOnlyList<BaseStat> BuffStatList => _listBuffStat;

    public IReadOnlyList<BaseStat> ItemStatList => _listItemStat;
    public IReadOnlyList<SkillData> SkillList => _listSkill;

    public int GetReviveCoutLimit => _reviveCountLimit;



    public List<ReviveBonusData> ListRubyBonus => _listReviveBonus;
    public int GetReviveRubyBonus(int Stage)
    {
        if (_listReviveBonus.Count == 0)
            return 0;

        List<ReviveBonusData> _orderedList = _listReviveBonus.OrderBy(x => x.Stage).ToList();
        if(_orderedList.FirstOrDefault().Stage > Stage)
        {
            return 0;
        }

        for(int i=0;i<_orderedList.Count;i++)
        {
            if (Stage < _orderedList[i].Stage)
                return _orderedList[i - 1].RubyBonusCount;
        }
        return _orderedList.LastOrDefault().RubyBonusCount;
        
    }

    public int GetReviveLevelBonus(int Stage)
    {
        if (_listReviveBonus.Count == 0)
            return 0;

        List<ReviveBonusData> _orderedList = _listReviveBonus.OrderBy(x => x.Stage).ToList();
      
        if (_orderedList.FirstOrDefault().Stage > Stage)
        {   
            return 0;
        }

        for (int i = 0; i < _orderedList.Count; i++)
        {
            if (Stage < _orderedList[i].Stage)
                return _orderedList[i - 1].IncrementReviveLevel;
            
        }
        return _orderedList.LastOrDefault().IncrementReviveLevel;

    }




#if UNITY_EDITOR
    [ContextMenu("FindCharacterStats")]
    private void FindCharacterStats()
    {
        string [] guids = AssetDatabase.FindAssets($"t:{typeof(CharacterStat)}");
        _listStat.Clear();
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var stat = AssetDatabase.LoadAssetAtPath<CharacterStat>(assetPath);

            _listStat.Add(stat);
            

        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif






}
