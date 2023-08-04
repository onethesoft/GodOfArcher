using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "PlayerDatabase", menuName = "«√∑π¿ÃæÓ/µ•¿Ã≈Õ∫£¿ÃΩ∫")]
public class PlayerDatabase : ScriptableObject
{
    [Header("«√∑π¿ÃæÓ µÓ±ﬁ")]
    [SerializeField]
    List<PlayerRank> _listrank;

    [Header("»≠∆Û")]
    [SerializeField]
    List<Currency> _listCurrency;

    [Header("«√∑π¿ÃæÓ Ω∫≈›")]
    [SerializeField]
    List<CharacterStat> _listStat;

    [Header("«√∑π¿ÃæÓ Ω∫≈›¡∂∞«")]
    [SerializeField]
    List<CharacterStatUpgradeCondition> _listcondition;

    [Header("¿Øπ∞ Ω∫≈›")]
    [SerializeField]
    List<BaseStat> _listArtifactStat;

    [Header("»∞ Ω∫≈›")]
    [SerializeField]
    List<BaseStat> _listbowStat;

    [Header("≈ı±∏ Ω∫≈›")]
    [SerializeField]
    List<BaseStat> _listhelmetStat;

    [Header("∞©ø  Ω∫≈›")]
    [SerializeField]
    List<BaseStat> _listarmorStat;

    [Header("∏¡≈‰ Ω∫≈›")]
    [SerializeField]
    List<BaseStat> _listcloakStat;

    [Header("∑È Ω∫≈›")]
    [SerializeField]
    List<BaseStat> _listruneStat;

    [Header("∆Í Ω∫≈›")]
    [SerializeField]
    List<BaseStat> _listpetStat;

    [Header("πˆ«¡ Ω∫≈›")]
    [SerializeField]
    List<BaseStat> _listBuffStat;

    [Header("æ∆¿Ã≈€ Ω∫≈›")]
    [SerializeField]
    List<BaseStat> _listItemStat;

    [Header("Ω∫≈≥")]
    [SerializeField]
    List<SkillData> _listSkill;

    public CharacterStatUpgradeCondition Get => _listcondition[0];

    public IReadOnlyList<PlayerRank> RankList => _listrank;
    public IReadOnlyDictionary<int, PlayerRank> RankDic => _listrank.ToDictionary(keySelector: m => (int)m.Level, elementSelector: m => m);


    public IReadOnlyList<Currency> CurrencyList => _listCurrency;

    public IReadOnlyDictionary<string , Currency> CurrencyDict => _listCurrency.ToDictionary(keySelector: m => m.CodeName, elementSelector: m => m);

    public IReadOnlyList<CharacterStat> StatList => _listStat;
    public IReadOnlyDictionary<Define.StatID, CharacterStat> StatDic => _listStat.ToDictionary(keySelector: m => (Define.StatID)Enum.Parse(typeof(Define.StatID), m.CodeName), elementSelector: m => m);

    public IReadOnlyList<BaseStat> ArtifactStatList => _listArtifactStat;

    public IReadOnlyList<BaseStat> BowStatList => _listbowStat;

    public IReadOnlyList<BaseStat> HelmetStatList => _listhelmetStat;

    public IReadOnlyList<BaseStat> ArmorStatList => _listarmorStat;

    public IReadOnlyList<BaseStat> CloakStatList => _listcloakStat;


    public IReadOnlyList<BaseStat> RuneStatList => _listruneStat;


    public IReadOnlyList<BaseStat> PetStatList => _listpetStat;
    public IReadOnlyList<BaseStat> BuffStatList => _listBuffStat;

    public IReadOnlyList<BaseStat> ItemStatList => _listItemStat;
    public IReadOnlyList<SkillData> SkillList => _listSkill;

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
