using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="등급데이터")]
public class PlayerRank : ScriptableObject
{
    public enum Rank
    {
        F = 0,
        E,
        D,
        C,
        B,
        A,
        S,
        L,
        M,
        I
    }
    [SerializeField]
    Rank _rank;
    public Rank Level => _rank;

    
    [SerializeField]
    int _conditionStage;

    public int Condition => _conditionStage;


    [SerializeField]
    GameObject _iconObject;
    public GameObject Icon => _iconObject;

    [SerializeField]
    List<StatModifier> _statModifiers;
    public IReadOnlyList<StatModifier> StatModifiers => _statModifiers;

    [SerializeField]
    int _pierceCount;

    public int PierceCount => _pierceCount;

    [SerializeField]
    string _ranktext;

    public string RankText => _ranktext;

    public string DisplayName => $"{_ranktext}({Level.ToString()})";

    [SerializeField]
    int _pierceRate;

    public int PierceRate => _pierceRate;

    [SerializeField]
    int _petSlotCount;

    public int PetSlotCount => _petSlotCount;

    [SerializeField]
    int _runeSlotCount;

    public int RuneSlotCount => _runeSlotCount;



}
