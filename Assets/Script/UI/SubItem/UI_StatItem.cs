using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.onethesoft.GodOfArcher;
using System.Numerics;

public class UI_StatItem : UI_Base
{
    public Define.StatID StatID;
    public delegate void LevelupHandler(Define.StatID Id , bool IsAll = false);
    public event LevelupHandler OnLevelup = null;



    private StatItemData _StatData;
    CharacterStat _stat;
    public CharacterStat Stat { set { _stat = value; } }
    int UpgradeCount = 1;
    enum Images
    {
        Icon,
        BackIcon ,
        CurrencyIcon
    }
    enum Texts
    {
        LevelText ,
        MaxLevelText ,
        IncrementStatText ,
        StatText ,
        LevelupButtonText ,
        LevelupCostText,
        TotalButtonText,
        BlockerText
    }

    enum Buttons
    {
        TotalButton ,
        Button
    }
    enum GameObjects
    {
        Blocker
    }


    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        if(_stat != null)
        {
            GetImage((int)Images.BackIcon).sprite = _stat.IconBackground;
            GetImage((int)Images.Icon).sprite = _stat.Icon;


            ShowLevelText();
            GetText((int)Texts.MaxLevelText).text = $"최고레벨 {_stat.MaxLevel}";

            ShowIncrementStatText();
            ShowStatText();
            //GetText((int)Texts.StatText).text = $"총"

            GetImage((int)Images.CurrencyIcon).sprite = _stat.Currency.Icon;
            GetText((int)Texts.LevelupButtonText).text = $"{_stat.Currency.Description} 레벨업";
            GetText((int)Texts.TotalButtonText).text = Managers.Data.TextData[(int)Define.TextData.TotalLevelupText].Kor.Replace("\\n", System.Environment.NewLine); ; // 일괄레벨업

            UpgradeCount = 1;
            ShowLevelUpCost(UpgradeCount);

            Managers.Game.OnCurrencyChanged -= UpdateStatText;
            Managers.Game.OnCurrencyChanged += UpdateStatText;

            _stat.OnLevelChanged -= UpdateStatText;
            _stat.OnLevelChanged += UpdateStatText;

            _stat.OnUnLock -= UnLockStat;
            _stat.OnUnLock += UnLockStat;

            _stat.OnMaxLevelChanged -= MaxLevelChanged;
            _stat.OnMaxLevelChanged += MaxLevelChanged;

            if (_stat.IsUnLock)
            {
                Get<GameObject>((int)GameObjects.Blocker).SetActive(false);
            }
            else
            {
                Get<Text>((int)Texts.BlockerText).text = _stat.UnLockString;
            }

            if (_stat.CodeName == Define.StatID.SkillMaxLevelLimit.ToString())
            {
                GetButton((int)Buttons.TotalButton).gameObject.SetActive(false);
            }


        }


        AddUIEvent(GetButton((int)Buttons.TotalButton).gameObject, (data) => {

            Upgrade(_stat.SearchUpgradableMaxLevel(), true);
        });
        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
          
            Upgrade(_stat.Level + UpgradeCount, false);
           
        });



    }
    void Upgrade(int targetLevel , bool isAll = false)
    {
        int temp = targetLevel;
        if (temp > _stat.MaxLevel)
            temp = _stat.MaxLevel;
        _stat.Upgrade(temp);
        ShowStatText();
        ShowLevelText();
        OnLevelup?.Invoke(StatID, isAll);

    }
    void UpdateStatText(CharacterStat stat , int Count)
    {
        ShowLevelText();
        ShowIncrementStatText();
        ShowStatText();
        ShowLevelUpCost(UpgradeCount);
    }
    void ShowLevelText()
    {
        if (_stat.CodeName == Define.StatID.SkillAttack.ToString())
            GetText((int)Texts.LevelText).text = $"{_stat.Description} 증가 : 레벨 {_stat.Level}";
        else if (_stat.CodeName == Define.StatID.SkillMaxLevelLimit.ToString())
        {
            GetText((int)Texts.LevelText).text = $"{_stat.Description} 해제 : 레벨 {_stat.Level}";
            GetText((int)Texts.LevelText).fontSize = 22;
        }
        else
            GetText((int)Texts.LevelText).text = $"{_stat.Description} : 레벨 {_stat.Level}";
    }
    public void ShowLevelUpCost(int UpgradeCount)
    {
        if (!gameObject.activeSelf)
            return;

        this.UpgradeCount = UpgradeCount;
        int targetLevel = _stat.Level + UpgradeCount > _stat.MaxLevel ? _stat.MaxLevel : _stat.Level + UpgradeCount;

        BigInteger _targetLevelCost = _stat.GetUpgradeCost(targetLevel);
        //Debug.Log(_stat.CodeName + " , " + _stat.Level + " , " + _stat.GetUpgradeCost(_stat.Level + 1));
        //Debug.Log(_stat.CodeName + " , " + _stat.MaxLevel + " , " + _stat.GetUpgradeCost(_stat.MaxLevel));

        GetText((int)Texts.LevelupCostText).text = $"{Util.GetBigIntegerUnit(_targetLevelCost)}";
    }
    void ShowIncrementStatText()
    {
        if(_stat.CodeName == Define.StatID.Attack.ToString() || _stat.CodeName == Define.StatID.CraftAttack.ToString() || _stat.CodeName == Define.StatID.ExtraHit.ToString())
            GetText((int)Texts.IncrementStatText).text = $"{_stat.Description}량 : {_stat.IncrementValuePerLevel} 증가";
        else if (_stat.CodeName == Define.StatID.JumpingCount.ToString())
            GetText((int)Texts.IncrementStatText).text = $"{_stat.Description}량 : {_stat.IncrementValuePerLevel}층 증가";
        else if (_stat.CodeName == Define.StatID.SkillAttack.ToString())
            GetText((int)Texts.IncrementStatText).text = $"{_stat.Description} : {_stat.IncrementValuePerLevel}% 증가";
        else if (_stat.CodeName == Define.StatID.SkillMaxLevelLimit.ToString())
            GetText((int)Texts.IncrementStatText).text = $"{_stat.Description} : {_stat.IncrementValuePerLevel}%";
        else if (_stat.CodeName == Define.StatID.CriticalHitRate.ToString() || _stat.CodeName == Define.StatID.CraftCriticalHitRate.ToString() )
            GetText((int)Texts.IncrementStatText).text = $"{_stat.Description}량 : {((float)_stat.IncrementValuePerLevel)*0.1f}% 증가";
        else 
            GetText((int)Texts.IncrementStatText).text = $"{_stat.Description}량 : {_stat.IncrementValuePerLevel}% 증가";
    }

    void ShowStatText()
    {
        if (_stat.CodeName == Define.StatID.Attack.ToString() || _stat.CodeName == Define.StatID.CraftAttack.ToString() )
            GetText((int)Texts.StatText).text = $"총 {_stat.Description}량 : {_stat.GetValue} 증가";
        else if (_stat.CodeName == Define.StatID.JumpingCount.ToString())
            GetText((int)Texts.StatText).text = $"총 {_stat.Description}량 : {_stat.Value}층 증가";
        else if (_stat.CodeName == Define.StatID.ExtraHit.ToString())
            GetText((int)Texts.StatText).text = $"총 {_stat.Description}량 : {_stat.Value} 증가";
        else if (_stat.CodeName == Define.StatID.SkillAttack.ToString())
            GetText((int)Texts.StatText).text = $"스킬포인트 잔여수량 : {_stat.Currency.Amount}";
        else if (_stat.CodeName == Define.StatID.SkillMaxLevelLimit.ToString())
        {
            if(_stat.Level == _stat.MaxLevel)
                GetText((int)Texts.StatText).text = $"최고레벨{_stat.GetValue}%";
            else
                GetText((int)Texts.StatText).text = $"최고레벨{_stat.GetValue}% ->{_stat.GetValue + _stat.IncrementValuePerLevel}%";
        }
        else if (_stat.CodeName == Define.StatID.CriticalHitRate.ToString() || _stat.CodeName == Define.StatID.CraftCriticalHitRate.ToString())
            GetText((int)Texts.StatText).text = $"총 {_stat.Description}량 : {((float)_stat.GetValue) * 0.1f}% 증가";
        else
            GetText((int)Texts.StatText).text = $"총 {_stat.Description}량 : {_stat.GetValue}% 증가";
    }
    void UpdateStatText(Define.CurrencyID id, string Amount)
    {
        if(_stat.Currency.CodeName == id.ToString() && _stat.CodeName == Define.StatID.SkillAttack.ToString())
        {
            GetText((int)Texts.StatText).text = $"스킬포인트 잔여수량 : {_stat.Currency.Amount}";
        }

    }
    void UnLockStat(bool isUnlock)
    {
        if (isUnlock == true)
            Get<GameObject>((int)GameObjects.Blocker).SetActive(false);
    }
    public void SetLevelText(string text)
    {
        GetText((int)Texts.LevelText).text = text;
    }


    void MaxLevelChanged(CharacterStat stat , int Count)
    {
        if(stat == _stat && stat.CodeName == Define.StatID.SkillAttack.ToString())
        {
            ShowLevelUpCost(1);
        }
    }


    private void OnDestroy()
    {
        Managers.Game.OnCurrencyChanged -= UpdateStatText;
        if (_stat != null)
        {
            _stat.OnLevelChanged -= UpdateStatText;
            _stat.OnUnLock -= UnLockStat;
            _stat.OnMaxLevelChanged -= MaxLevelChanged;
        }
    }
}
