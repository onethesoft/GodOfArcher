using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public static readonly ServiceVersion APPVersion = new ServiceVersion() 
    { 
        Major = 2 , Minor = 0 , Patch = 0 
    };
   
    public enum UIEvent
    {
        Click,
        Pressed,
        Released,
    }
    public enum MouseEvent
    {
        Pressed,
        Released,
        Click,
    }

    public enum Scene
    {
        Unknown,
        Main,
        GoldPig,
        DarkMage,
        Troll,
    }

    public enum Sound
    {
        Bgm = 0,
        Effect,
        MaxCount
    }
    public enum Option
    {
        ShowDamage,
        ShowEffect,
        ShowShake,
        MaxCount
    }
    public enum MonsterType
    {
        Normal,
        Boss,
        Dongeon
    }
    public enum State
    {
        Idle,
        Attack,
        Moving,
        Hurt,
        Death,
    }

    public enum WorldObject
    {
        Unknown,
        Monster,
        Pet,
        Arrow
    }

    public enum DataFileList
    {
        StatItemData,
        StatData,
        TextData,
        AttackStatData,
        CurrencyData ,
        QuestTable,
        fword_list
    }
    public enum TextData
    {
        AttackStat = 1 ,
        AttackSpeedStat ,
        CriticalHitStat ,
        CriticalHitRateStat ,
        CraftAttackStat ,
        CraftAttackSpeedStat ,
        CraftCriticalHitStat ,
        CraftCriticalHitRateStat ,
        GoldDropRateStat ,
        CraftDropRateStat,
        JumpingRateStat ,
        JumpingCountStat ,
        SkillAttackStat ,
        SkillMaxLevelLimit ,
        IncrementAttackStat ,
        IncrementAttackSpeedStat,
        IncrementCriticalHitStat ,
        IncrementCriticalHitRateStat ,
        IncrementCraftAttackStat,
        IncrementCraftAttackSpeedStat,
        IncrementCraftCriticalHitStat,
        IncrementCraftCriticalHitRateStat,
        IncrementGoldDropRateStat ,
        IncrementCraftDropRateStat ,
        IncrementJumpingRateStat ,
        IncrementJumpingCountStat ,
        IncrementSkillAttackStat ,
        IncrementSkillMaxLevelLimit ,
        StatMaxLevelText ,
        TotalAttack ,
        TotalAttackSpeed ,
        TotalCriticalHit ,
        TotalCriticalHitRate ,
        TotalCraftAttack ,
        TotalCraftAttackSpped ,
        TotalCraftCriticalHit ,
        TotalCraftCriticalHitRate ,
        TotalGoldDropRate ,
        TotalCraftDropRate ,
        TotalJumpingRate ,
        TotalJumpingCount ,
        RemainingSkillPoint,
        StatMaxLevel ,
        LevelupText,
        TotalLevelupText ,
        GoldSelectButtonText,
        CPSelectButtonText,
        SpecialSelectButtonText,
        UpgradeCountText ,
        ClearStageText ,
        KillMonsterText ,
        KillBossMonsterText,
        PlayTimeText,
        ClearDongeonText,
        Currency_Gold,
        Currency_CP,
        Currency_Ruby,
        Currency_SP


    }
    #region Player
    public enum StatType
    {
        Gold ,
        Special ,
        Craft ,
        Skill ,
        None
    }

    public enum StatID
    {
        Attack = 1000,
        AttackSpeed,
        CriticalHit,
        CriticalHitRate,
        CraftAttack ,
        CraftAttackSpeed ,
        CraftCriticalHit ,
        CraftCriticalHitRate ,
        GoldDropRate ,
        CraftDropRate ,
        ExtraHit ,
        JumpingCount ,
        SkillAttack ,
        SkillMaxLevelLimit,
        SkillCoolTime,
        JumpingRate
    }
    #endregion
    public enum CurrencyID
    {
        Gold,
        CP,
        Ruby,
        SP
        
    }
    public enum QuestGroupType
    {
        ClearMainStage ,
        KillMonster ,
        KillBossMonster ,
        GetPlayTime ,
        ClearTrollStage,
        DailyQuest,
        TutorialQuest

    }
    public enum QuestType
    {
        StageClear,
        MonsterKill,
        BossMonsterKill,
        PlayTime,
        TrollStageClear,
        Daily,
        Tutorial,
        DailyCheckout,
        Seasonpass,
        GambleRune,
        GamblePet,
        GambleBow,
        GambleHelmet,
        GambleArmor,
        GambleCloak
    }
    public enum Dongeon
    {
        GoldPig,
        DarkMage,
        Troll,
        Main
    }

    public enum DamageType
    {
        Normal,
        Critical,
        Skill
    }

    public enum CouponResponse
    {
        Success,
        AlreadyIssue, // ¹ß±ÞµÈ ÄíÆù
        Expired,
        NotExist

    }

    public enum SK_Status
    {
        Ready,
        Cooldown,
        GlobalCooldown
    }
   


}
