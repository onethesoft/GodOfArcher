using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameReporter : MonoBehaviour
{
    [SerializeField]
    QuestReporter _killMonster;

    [SerializeField]
    QuestReporter _killBossMonster;

    [SerializeField]
    QuestReporter _mainStageClear;

    [SerializeField]
    QuestReporter _playTime;

    [SerializeField]
    QuestReporter _trollStageClear;


    #region DailyQuestReporter
    [SerializeField]
    QuestReporter _dailyKillMonster;

    [SerializeField]
    QuestReporter _dailyPlayTime;

    [SerializeField]
    QuestReporter _dailyPurchaseRune;

    [SerializeField]
    QuestReporter _dailyPurchasePet;

    [SerializeField]
    QuestReporter _dailyPurchaseItem;

    [SerializeField]
    QuestReporter _dailyPurchaseArtifact;

    #endregion

    #region TutorialQuestReporter
    [SerializeField]
    QuestReporter _tutorialAttackStat;

    [SerializeField]
    QuestReporter _tutorialAttackSpeedStat;

    [SerializeField]
    QuestReporter _tutorialCriticalHitStat;

    [SerializeField]
    QuestReporter _tutorialCriticalHitRateStat;

    [SerializeField]
    QuestReporter _tutorialPurchasePet;

    [SerializeField]
    QuestReporter _tutorialPurchaseRune;

    [SerializeField]
    QuestReporter _tutorialPurchaseBow;

    [SerializeField]
    QuestReporter _tutorialPurchaseHelmet;

    [SerializeField]
    QuestReporter _tutorialPurchaseArmor;

    [SerializeField]
    QuestReporter _tutorialPurchaseCloak;

    [SerializeField]
    QuestReporter _tutorialPurchaseArtifact;



    [SerializeField]
    QuestReporter _tutorialEquipPet;
    [SerializeField]
    QuestReporter _tutorialEquipRune;

    [SerializeField]
    QuestReporter _tutorialEquipBow;

    [SerializeField]
    QuestReporter _tutorialEquipHelmet;

    [SerializeField]
    QuestReporter _tutorialEquipArmor;

    [SerializeField]
    QuestReporter _tutorialEquipCloak;

    [SerializeField]
    QuestReporter _tutorialUpgradeArtifact;



    [SerializeField]
    QuestReporter _tutorialOpenPopupCharacterInfo;

    [SerializeField]
    QuestReporter _tutorialOpenPopupRanking;


    [SerializeField]
    QuestReporter _tutorialClearStage;



    #endregion

    #region SeasonPassReporter

    [SerializeField]
    QuestReporter _seasonpassClearStage;

    [SerializeField]
    List<QuestReporter> _seasonpassList;

    
    #endregion

    #region DailyCheckoutReporter
    [SerializeField]
    QuestReporter _dailyCheckout;
    #endregion

    #region GambleReporter
    [SerializeField]
    QuestReporter _gambleRune;

    [SerializeField]
    QuestReporter _gamblePet;

    [SerializeField]
    QuestReporter _gambleBow;

    [SerializeField]
    QuestReporter _gambleArmor;

    [SerializeField]
    QuestReporter _gambleHelmet;

    [SerializeField]
    QuestReporter _gambleCloak;
    #endregion

    public void KillMonster(Define.MonsterType type)
    {
        if (type == Define.MonsterType.Normal)
        {
            _dailyKillMonster.DoReport();
            _killMonster.DoReport();

        }
        else if (type == Define.MonsterType.Boss)
        {
            _killBossMonster.DoReport();

            // 보스몬스터도 일반 몬스터 킬로 취급한다.
            _dailyKillMonster.DoReport();
            _killMonster.DoReport();
        }
    }

    public void ClearStage(int clearstage)
    {
        if (Managers.Scene.CurrentScene is MainScene)
        {
            _mainStageClear.SetSuccessCount(clearstage);
            _mainStageClear.DoReport();

            
            _seasonpassClearStage.SetSuccessCount(clearstage);
            _seasonpassClearStage.DoReport();

            _tutorialClearStage.SetSuccessCount(clearstage);
            _tutorialClearStage.DoReport();

        }
        else if (Managers.Scene.CurrentScene is TrollScene)
        {
            _trollStageClear.SetSuccessCount(clearstage);
            _trollStageClear.DoReport();
        }
    }
    public void AddPlayTime(int addedTime = 1)
    {
        _playTime.SetSuccessCount(addedTime);
        _playTime.DoReport();

        _dailyPlayTime.SetSuccessCount(addedTime);
        _dailyPlayTime.DoReport();
    }

    public void EquipItem(string ItemClass)
    {
        if (ItemClass == "Bow")
        {
            _tutorialEquipBow.DoReport();
        }
        else if (ItemClass == "Helmet")
        {
            _tutorialEquipHelmet.DoReport();
        }
        else if (ItemClass == "Armor")
        {
            _tutorialEquipArmor.DoReport();
        }
        else if (ItemClass == "Cloak")
        {
            _tutorialEquipCloak.DoReport();
        }
        else if (ItemClass == "Rune")
        {
            _tutorialEquipRune.DoReport();
        }
        else if (ItemClass == "Pet")
        {
            _tutorialEquipPet.DoReport();
        }
        
    }
    public void UpgradeItems(string ItemClass)
    {
        if (ItemClass == "Heart")
            _tutorialUpgradeArtifact.DoReport();
    }

    public void UpgradeStat(CharacterStat target, int count)
    {
        if (target.CodeName == Define.StatID.Attack.ToString())
        {
            _tutorialAttackStat.SetSuccessCount(count);
            _tutorialAttackStat.DoReport();
        }
        else if (target.CodeName == Define.StatID.AttackSpeed.ToString())
        {
            _tutorialAttackSpeedStat.SetSuccessCount(count);
            _tutorialAttackSpeedStat.DoReport();
        }
        else if (target.CodeName == Define.StatID.CriticalHit.ToString())
        {
            _tutorialCriticalHitStat.SetSuccessCount(count);
            _tutorialCriticalHitStat.DoReport();
        }
        else if (target.CodeName == Define.StatID.CriticalHitRate.ToString())
        {
            _tutorialCriticalHitRateStat.SetSuccessCount(count);
            _tutorialCriticalHitRateStat.DoReport();
        }
    }

    public void PurchaseItemClass(string ItemClass ,  int count , bool isFree)
    {
        if (ItemClass == "Bow")
        {
            _tutorialPurchaseBow.SetSuccessCount(count);
            _tutorialPurchaseBow.DoReport();

            _dailyPurchaseItem.DoReport();

            if (isFree == false)
            {
                _gambleBow.SetSuccessCount(count);
                _gambleBow.DoReport();
            }
        }
        else if (ItemClass == "Helmet")
        {
            _tutorialPurchaseHelmet.SetSuccessCount(count);
            _tutorialPurchaseHelmet.DoReport();

            _dailyPurchaseItem.DoReport();

            if (isFree == false)
            {
                _gambleHelmet.SetSuccessCount(count);
                _gambleHelmet.DoReport();
            }
        }
        else if (ItemClass == "Armor")
        {
            _tutorialPurchaseArmor.SetSuccessCount(count);
            _tutorialPurchaseArmor.DoReport();

            _dailyPurchaseItem.DoReport();

            if (isFree == false)
            {
                _gambleArmor.SetSuccessCount(count);
                _gambleArmor.DoReport();
            }
        }
        else if (ItemClass == "Cloak")
        {
            _tutorialPurchaseCloak.SetSuccessCount(count);
            _tutorialPurchaseCloak.DoReport();

            _dailyPurchaseItem.DoReport();

            if (isFree == false)
            {
                _gambleCloak.SetSuccessCount(count);
                _gambleCloak.DoReport();
            }
        }
        else if (ItemClass == "Rune")
        {
            _tutorialPurchaseRune.SetSuccessCount(count);
            _tutorialPurchaseRune.DoReport();

            _dailyPurchaseRune.DoReport();

            if (isFree == false)
            {
                _gambleRune.SetSuccessCount(count);
                _gambleRune.DoReport();
            }
        }
        else if (ItemClass == "Pet")
        {
            _tutorialPurchasePet.SetSuccessCount(count);
            _tutorialPurchasePet.DoReport();

            _dailyPurchasePet.DoReport();

            if (isFree == false)
            {
                _gamblePet.SetSuccessCount(count);
                _gamblePet.DoReport();
            }
        }
        else if(ItemClass == "Heart")
        {
            _dailyPurchaseArtifact.SetSuccessCount(count);
            _dailyPurchaseArtifact.DoReport();

            _tutorialPurchaseArtifact.SetSuccessCount(count);
            _tutorialPurchaseArtifact.DoReport();


        }
       
    }
    public void PurchaseItemId(string ItemId, int count)
    {
        
        if (ItemId.Contains("seasonpass"))
        {
            string parseItemId = ItemId.Replace("seasonpass", "");
            if (string.IsNullOrEmpty(parseItemId))
                _seasonpassList[0].DoReport();
            else
            {
                int number;
                if(int.TryParse(parseItemId, out number))
                {
                    if(number > 0)
                        _seasonpassList[number - 1].DoReport();
                }
            }
        }
    }

    public void OpenPopup(UI_Popup popup)
    {
        if (popup is UI_CharacterInfo)
            _tutorialOpenPopupCharacterInfo.DoReport();
        else if (popup is UI_Ranking)
            _tutorialOpenPopupRanking.DoReport();
        
    }

    public void DailyCheckout(int checkoutCount)
    {
        Debug.Log(checkoutCount);
        _dailyCheckout.SetSuccessCount(checkoutCount);
        _dailyCheckout.DoReport();
    }
  
    
    

}
