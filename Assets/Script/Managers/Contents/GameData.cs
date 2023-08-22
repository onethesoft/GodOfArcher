using Assets.HeroEditor.Common.CharacterScripts;

using HeroEditor.Common;
using HeroEditor.Common.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;



public class GameData : MonoBehaviour
{
    #region Quest Reporter Event
    [Serializable]
    public class OnChanged : UnityEvent<int> { }

    [Serializable]
    public class OnChangedStat : UnityEvent<CharacterStat , int> { }

    [Serializable]
    public class OnChangedEquipItem: UnityEvent<string> { }
    [Serializable]
    public class OnKilledMonster : UnityEvent<Define.MonsterType> { }
    [Serializable]
    public class OnChangedOpenPopup : UnityEvent<UI_Popup> { }

    [Serializable]
    public class OnChangedPurchaseItemClass : UnityEvent<string, int ,bool> { }

    [Serializable]
    public class OnChangedPurchaseItemId : UnityEvent<string, int> { }



    [SerializeField]
    public OnChanged OnPlayTime;

    [SerializeField]
    public OnChanged OnClearStage;

    [SerializeField]
    public OnChangedStat OnUpgradeStat;

    [SerializeField]
    public OnChangedEquipItem OnEquipItem;

    [SerializeField]
    public OnKilledMonster OnKill;
    [SerializeField]
    public OnChangedOpenPopup OnOpenPopup;

    [SerializeField]
    public OnChangedPurchaseItemClass OnPurchaseItemClass;

    [SerializeField]
    public OnChangedPurchaseItemId OnPurchaseItemId;

    [SerializeField]
    public OnChanged OnDailyCheckout;

    [SerializeField]
    public OnChangedEquipItem OnUpgradeItem;


    #endregion

    [SerializeField]
    public int Level;

    [SerializeField]
    public int Stage;

    [SerializeField]
    public int ClearStage;

    [SerializeField]
    public int MaxClearStage;

    [SerializeField]
    public int TrollStage;

    [SerializeField]
    public ReviveInfo ReviveInfo;

    public RouletteInfo RouletteInfo;

    public EquipmentSystem Rune;
    public EquipmentSystem Pet;
    public EquipmentSystem Bow;
    public EquipmentSystem Helmet;
    public EquipmentSystem Armor;
    public EquipmentSystem Cloak;

    public StatSystem ItemStat;
    public StatSystem RuneStat;
    public StatSystem PetStat;
   
    public StatSystem BuffStat;
    public StatSystem ArtifactStat;

    public CharacterStatSystem PlayerStat;
    
    public Inventory Inventory;
    public Dictionary<string, Currency> Currency;

    public List<SkillData> SkillSet;

    public int PierceCount;

    

    public int PlayTime;
    public DateTime? DailyCheckoutQuestLastClearTime = null;
    public DateTime? DailyQuestLastClearTime = null;
    public bool IsBeginner = true;

    public StageDataList StageInfoList;

    private GameDataUpdater _updater;

   

    public GameSaveData ToSaveData()
    {
        return new GameSaveData(this);
    }
    
    public void Load(GameSaveData data)
    {
        Level = data.Level;
        
        //Managers.Game.LevelChanged();

        Stage = data.Stage;
        ClearStage = data.ClearStage;
        MaxClearStage = data.MaxClearStage;
        TrollStage = data.TrollStage;
        DailyQuestLastClearTime = data.DailyQuestLastClearTime;
        DailyCheckoutQuestLastClearTime = data.DailyCheckoutQuestLastClearTime;

        PlayerStat.Load(data.PlayerStat);
        Inventory.Load(data.Inventory);

        foreach (CurrencySaveData currency in data.Currency)
            Currency[currency.Key].Load(currency);


        Rune.Load(data.Rune);
        Pet.Load(data.Pet);
        Bow.Load(data.Bow);
        Helmet.Load(data.Helmet);
        Armor.Load(data.Armor);
        Cloak.Load(data.Cloak);
        IsBeginner = data.IsBeginner;
        StageInfoList = data.StageInfoList;

        OnLoaded();


    }

    public void UpdateData(PlayerInfo.StatisticsDataKey key , PlayerInfo data)
    {
        PlayFab.ClientModels.StatisticValue _get = data.Payload.PlayerStatistics.Where(x => x.StatisticName == key.ToString()).FirstOrDefault();

        if(key == PlayerInfo.StatisticsDataKey.Level)
        {
            if (_get == null)
            {
                _get = new PlayFab.ClientModels.StatisticValue { StatisticName = key.ToString(), Value = Level };
                data.Payload.PlayerStatistics.Add(_get);
            }
            else
                _get.Value = Level;
        }
        else if(key == PlayerInfo.StatisticsDataKey.Stage)
        {
            if (_get == null)
            {
                _get = new PlayFab.ClientModels.StatisticValue { StatisticName = key.ToString(), Value = Stage };
                data.Payload.PlayerStatistics.Add(_get);
            }
            else
                _get.Value = Stage;
        }
        else if (key == PlayerInfo.StatisticsDataKey.ClearStage)
        {
            if (_get == null)
            {
                _get = new PlayFab.ClientModels.StatisticValue { StatisticName = key.ToString(), Value = ClearStage };
                data.Payload.PlayerStatistics.Add(_get);
            }
            else
                _get.Value = ClearStage;
        }
        else if (key == PlayerInfo.StatisticsDataKey.MaxClearStage)
        {
            if (_get == null)
            {
                _get = new PlayFab.ClientModels.StatisticValue { StatisticName = key.ToString(), Value = MaxClearStage };
                data.Payload.PlayerStatistics.Add(_get);
            }
            else
                _get.Value = MaxClearStage;
        }
        else if (key == PlayerInfo.StatisticsDataKey.PlayTime)
        {
            if (_get == null)
            {
                _get = new PlayFab.ClientModels.StatisticValue { StatisticName = key.ToString(), Value = PlayTime };
                data.Payload.PlayerStatistics.Add(_get);
            }
            else
                _get.Value = PlayTime;
        }
        else if (key == PlayerInfo.StatisticsDataKey.TrollStage)
        {
            if (_get == null)
            {
                _get = new PlayFab.ClientModels.StatisticValue { StatisticName = key.ToString(), Value = TrollStage };
                data.Payload.PlayerStatistics.Add(_get);
            }
            else
                _get.Value = TrollStage;
        }
        else if(key == PlayerInfo.StatisticsDataKey.ItemEquipment)
        {
            if (_get == null)
            {
                _get = new PlayFab.ClientModels.StatisticValue { StatisticName = key.ToString(), Value = Managers.Item.EncodingItemEquipment() };
                data.Payload.PlayerStatistics.Add(_get);
            }
            else
                _get.Value = Managers.Item.EncodingItemEquipment();
        }


    }

    public void UpdateData(PlayerInfo.UserDataKey key , PlayerInfo data)
    {
        if(key == PlayerInfo.UserDataKey.DailyCheckoutQuestLastClearTime )
        {
            DateTime? time = DailyCheckoutQuestLastClearTime;
            
            if (data.Payload.UserData.ContainsKey(key.ToString()) == false)
                data.Payload.UserData.Add(key.ToString(),
                    new PlayFab.ClientModels.UserDataRecord { Value = Util.GetTimeString(time) });
            else
                data.Payload.UserData[key.ToString()].Value = Util.GetTimeString(time);
            
        }
        else if(key == PlayerInfo.UserDataKey.DailyQuestLastClearTime)
        {
            DateTime? time = DailyQuestLastClearTime;
            
            if (data.Payload.UserData.ContainsKey(key.ToString()) == false)
                data.Payload.UserData.Add(key.ToString(),
                    new PlayFab.ClientModels.UserDataRecord { Value = Util.GetTimeString(time) });
            else
                data.Payload.UserData[key.ToString()].Value = Util.GetTimeString(time);
            
        }
        else if(key == PlayerInfo.UserDataKey.Gold || key == PlayerInfo.UserDataKey.CP)
        {
            if (data.Payload.UserData.ContainsKey(key.ToString()) == false)
                data.Payload.UserData.Add(key.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = Currency[key.ToString()].Amount.ToString() });
            else
                data.Payload.UserData[key.ToString()].Value = Currency[key.ToString()].Amount.ToString();
        }
        else if(key == PlayerInfo.UserDataKey.Rune || key == PlayerInfo.UserDataKey.Pet || key == PlayerInfo.UserDataKey.Bow || key == PlayerInfo.UserDataKey.Helmet || key == PlayerInfo.UserDataKey.Armor || key == PlayerInfo.UserDataKey.Cloak)
        {
            if (data.Payload.UserData.ContainsKey(key.ToString()) == false)
                data.Payload.UserData.Add(key.ToString(), new PlayFab.ClientModels.UserDataRecord
                {
                    Value = JsonUtility.ToJson(
                        key == PlayerInfo.UserDataKey.Rune ? Rune.ToSaveData() :
                        key == PlayerInfo.UserDataKey.Pet ? Pet.ToSaveData() :
                        key == PlayerInfo.UserDataKey.Bow ? Bow.ToSaveData() :
                        key == PlayerInfo.UserDataKey.Helmet ? Helmet.ToSaveData() :
                        key == PlayerInfo.UserDataKey.Armor ? Armor.ToSaveData() :
                        Cloak.ToSaveData()),
                    Permission = PlayFab.ClientModels.UserDataPermission.Public
                });
            else
                data.Payload.UserData[key.ToString()].Value = JsonUtility.ToJson(
                        key == PlayerInfo.UserDataKey.Rune ? Rune.ToSaveData() :
                        key == PlayerInfo.UserDataKey.Pet ? Pet.ToSaveData() :
                        key == PlayerInfo.UserDataKey.Bow ? Bow.ToSaveData() :
                        key == PlayerInfo.UserDataKey.Helmet ? Helmet.ToSaveData() :
                        key == PlayerInfo.UserDataKey.Armor ? Armor.ToSaveData() :
                        Cloak.ToSaveData());
        }
        else if( key == PlayerInfo.UserDataKey.PlayerStat)
        {
            if (data.Payload.UserData.ContainsKey(key.ToString()) == false)
                data.Payload.UserData.Add(key.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = JsonUtility.ToJson(PlayerStat.ToSaveData()) });
            else
                data.Payload.UserData[key.ToString()].Value = JsonUtility.ToJson(PlayerStat.ToSaveData());
        }
        else if( key == PlayerInfo.UserDataKey.DPS)
        {
            if (data.Payload.UserData.ContainsKey(key.ToString()) == false)
                data.Payload.UserData.Add(key.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = Managers.Game.DPS.ToString(), Permission = PlayFab.ClientModels.UserDataPermission.Public });
            else
            {
                data.Payload.UserData[key.ToString()].Value = Managers.Game.DPS.ToString();
                if (data.Payload.UserData[key.ToString()].Permission != PlayFab.ClientModels.UserDataPermission.Public)
                    data.Payload.UserData[key.ToString()].Permission = PlayFab.ClientModels.UserDataPermission.Public;
            }
                
        }
        else if( key == PlayerInfo.UserDataKey.StageInfo)
        {
            if (data.Payload.UserData.ContainsKey(key.ToString()) == false)
                data.Payload.UserData.Add(key.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = JsonUtility.ToJson(StageInfoList) });
            else
                data.Payload.UserData[key.ToString()].Value = JsonUtility.ToJson(StageInfoList);
        }
        else if( key == PlayerInfo.UserDataKey.ReviveInfo)
        {
            if (data.Payload.UserData.ContainsKey(key.ToString()) == false)
                data.Payload.UserData.Add(key.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = ReviveInfo.ToJson() });
            else
                data.Payload.UserData[key.ToString()].Value = ReviveInfo.ToJson();
        }
        else if (key == PlayerInfo.UserDataKey.RouletteInfo)
        {
            if (data.Payload.UserData.ContainsKey(key.ToString()) == false)
                data.Payload.UserData.Add(key.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = RouletteInfo.ToJson() });
            else
                data.Payload.UserData[key.ToString()].Value = RouletteInfo.ToJson();
        }




    }
    public void UpdateData(PlayerInfo.CurrencyKey key , PlayerInfo data)
    {
        if(data.Payload.UserVirtualCurrency.ContainsKey(key.ToString()))
        {
            Currency currency = Currency.ToList().Where(x => x.Value.ShortCodeName == key.ToString()).FirstOrDefault().Value;
            data.Payload.UserVirtualCurrency[key.ToString()] = (int)(currency.Amount);
        }
        else
        {
            Currency currency = Currency.ToList().Where(x => x.Value.ShortCodeName == key.ToString()).FirstOrDefault().Value;
            data.Payload.UserVirtualCurrency.Add(key.ToString(), (int)(currency.Amount));
        }
    }
    public void Load(PlayerInfo data)
    {
        if (data.Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.Level.ToString()))
            Level = data.Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.Level.ToString()).First().Value;

        if (data.Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.Stage.ToString()))
            Stage = data.Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.Stage.ToString()).First().Value;

        if (data.Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.ClearStage.ToString()))
            ClearStage = data.Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.ClearStage.ToString()).First().Value;

        if (data.Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.MaxClearStage.ToString()))
            MaxClearStage = data.Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.MaxClearStage.ToString()).First().Value;

        if (data.Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.TrollStage.ToString()))
            TrollStage = data.Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.TrollStage.ToString()).First().Value;

        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.DailyCheckoutQuestLastClearTime.ToString()))
            DailyCheckoutQuestLastClearTime = Util.TryParseDateTime(data.Payload.UserData[PlayerInfo.UserDataKey.DailyCheckoutQuestLastClearTime.ToString()].Value);
        

        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.DailyQuestLastClearTime.ToString()))
            DailyQuestLastClearTime = Util.TryParseDateTime(data.Payload.UserData[PlayerInfo.UserDataKey.DailyQuestLastClearTime.ToString()].Value);
            
        
           
        

        

        // Currency Load 작업
        // Gold , CP 는 UserData 영역에
        // Ruby 및 SP 는 VirtualCurrency 에 저장된다.

        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.Gold.ToString()))
            Currency[PlayerInfo.UserDataKey.Gold.ToString()].SetAmount(System.Numerics.BigInteger.Parse(data.Payload.UserData[PlayerInfo.UserDataKey.Gold.ToString()].Value));
        
        if(data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.CP.ToString()))
            Currency[PlayerInfo.UserDataKey.CP.ToString()].SetAmount(System.Numerics.BigInteger.Parse(data.Payload.UserData[PlayerInfo.UserDataKey.CP.ToString()].Value));

        // foreach(CurrencySaveData saveCurrency in GameSaveData.ParseCurrency(data.Payload.UserData[PlayerInfo.UserDataKey.Currency.ToString()].Value))
        // Currency[saveCurrency.Key].Load(saveCurrency);

        foreach (KeyValuePair<string,int> currency in data.Payload.UserVirtualCurrency)
            if(Currency.Any(x=>x.Value.ShortCodeName == currency.Key))
                Currency.Where(x => x.Value.ShortCodeName == currency.Key).First().Value.SetAmount((System.Numerics.BigInteger)currency.Value);
            
        ////////////////////////////////////


        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.PlayerStat.ToString()))
            PlayerStat.Load(CharacterStatSystemSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.PlayerStat.ToString()].Value));

        if (data.Payload.UserInventory.Count > 0)
            Inventory.Load(data.Payload.UserInventory);


        if(data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.Rune.ToString()))
            Rune.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Rune.ToString()].Value));
        else
        {
            data.Payload.UserData.Add(PlayerInfo.UserDataKey.Rune.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = data.Payload.TitleData[PlayerInfo.TitleDataKey.Rune.ToString()] });
            Rune.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Rune.ToString()].Value));
        }


        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.Pet.ToString()))
            Pet.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Pet.ToString()].Value));
        else
        {
            data.Payload.UserData.Add(PlayerInfo.UserDataKey.Pet.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = data.Payload.TitleData[PlayerInfo.TitleDataKey.Pet.ToString()] });
            Pet.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Pet.ToString()].Value));
        }

        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.Bow.ToString()))
            Bow.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Bow.ToString()].Value));
        else
        {
            data.Payload.UserData.Add(PlayerInfo.UserDataKey.Bow.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = data.Payload.TitleData[PlayerInfo.TitleDataKey.Bow.ToString()] });
            Bow.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Bow.ToString()].Value));
        }

        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.Helmet.ToString()))
            Helmet.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Helmet.ToString()].Value));
        else
        {
            data.Payload.UserData.Add(PlayerInfo.UserDataKey.Helmet.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = data.Payload.TitleData[PlayerInfo.TitleDataKey.Helmet.ToString()] });
            Helmet.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Helmet.ToString()].Value));
        }

        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.Armor.ToString()))
            Armor.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Armor.ToString()].Value));
        else
        {
            data.Payload.UserData.Add(PlayerInfo.UserDataKey.Armor.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = data.Payload.TitleData[PlayerInfo.TitleDataKey.Armor.ToString()] });
            Armor.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Armor.ToString()].Value));
        }

        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.Cloak.ToString()))
            Cloak.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Cloak.ToString()].Value));
        else
        {
            data.Payload.UserData.Add(PlayerInfo.UserDataKey.Cloak.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = data.Payload.TitleData[PlayerInfo.TitleDataKey.Cloak.ToString()] });
            Cloak.Load(EquipmentSaveData.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.Cloak.ToString()].Value));
        }

        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.IsBeginner.ToString()))
            IsBeginner = bool.Parse(data.Payload.UserData[PlayerInfo.UserDataKey.IsBeginner.ToString()].Value);
        else
        {
            data.Payload.UserData.Add(PlayerInfo.UserDataKey.IsBeginner.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = data.Payload.TitleData[PlayerInfo.TitleDataKey.IsBeginner.ToString()] });
            IsBeginner = bool.Parse(data.Payload.UserData[PlayerInfo.UserDataKey.IsBeginner.ToString()].Value);
        }

        if(data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.StageInfo.ToString()))
        {
            StageInfoList = StageDataList.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.StageInfo.ToString()].Value);
            foreach(StageData _data in StageInfoList.List)
            {
                Define.Dongeon _type;
                if(System.Enum.TryParse(_data.type, out _type))
                    _data.ResetStageData(Managers.Game.StageDataBase.StageList.Where(x => x.type == _type).FirstOrDefault());
            }
        }
        else
        {
            StageInfoList = Managers.Game.StageDataBase.CreateStageDataList();
            data.Payload.UserData.Add(PlayerInfo.UserDataKey.StageInfo.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = JsonUtility.ToJson(StageInfoList) });
        }

        if(data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.ReviveInfo.ToString()))
        {
            ReviveInfo = ReviveInfo.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.ReviveInfo.ToString()].Value);
            ReviveInfo.DoReset();
        }
        else
        {
            ReviveInfo = ReviveInfo.Create();
        }

        if (data.Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.RouletteInfo.ToString()))
        {
            RouletteInfo = RouletteInfo.FromJson(data.Payload.UserData[PlayerInfo.UserDataKey.RouletteInfo.ToString()].Value);
        }
        else
        {
            RouletteInfo = new RouletteInfo { PlayRouletteCount = 0 };
        }

        OnLoaded();
       

    }

    public bool IsAdSkip
    {
        get
        {
            return Inventory.IsFindItem(x => x.ItemId == "iap_infinite");
        }
    }

    public void ResetStage()
    {
        foreach (StageData _data in StageInfoList.List)
        {
            Define.Dongeon _type;
            if (System.Enum.TryParse(_data.type, out _type))
                _data.ResetStageData(Managers.Game.StageDataBase.StageList.Where(x => x.type == _type).FirstOrDefault());
        }
        //UpdateData(PlayerInfo.UserDataKey.StageInfo , )
    }
    public void ResetQuest()
    {
        List<Define.QuestType> _resetQuestTypes = new List<Define.QuestType>();

        if (DailyQuestLastClearTime.HasValue)
            if (DailyQuestLastClearTime.GetValueOrDefault().ToLocalTime().Date < GlobalTime.Now.ToLocalTime().Date)
            {
                
                // 일일 퀘스트의 경우 Reset 시 시간을 기록한다.
                DailyQuestLastClearTime = GlobalTime.Now;
                Managers.Game.Save(PlayerInfo.UserDataKey.DailyQuestLastClearTime);

                _resetQuestTypes.Add(Define.QuestType.Daily);


            }

        // 일일 출석 퀘스트의 경우 퀘스트 보상을 수령했을 때만 시간을 기록한다.
        if (DailyCheckoutQuestLastClearTime.HasValue)
            if(DailyCheckoutQuestLastClearTime.GetValueOrDefault().ToLocalTime().Date < GlobalTime.Now.ToLocalTime().Date)
            {
                if(IsAllCompletedQuest(Define.QuestType.DailyCheckout))
                {
                    DailyCheckoutQuestLastClearTime = null;
                    Managers.Game.Save(PlayerInfo.UserDataKey.DailyCheckoutQuestLastClearTime);

                    _resetQuestTypes.Add(Define.QuestType.DailyCheckout);
                }
            }

        
        Managers.Quest.ResetQuests(_resetQuestTypes.Select(x=>x.ToString()).ToArray());


        _resetQuestTypes.Clear();



    }
    
    public void Init()
    {
        Level = 0;
        Stage = 1;
        ClearStage = 0;
        MaxClearStage = 0;
        TrollStage = 0;


        Inventory = new Inventory();

        Currency = new Dictionary<string, Currency>();
        PlayerStat = new CharacterStatSystem();
        ArtifactStat = new StatSystem();

        RuneStat = new StatSystem();
        PetStat = new StatSystem();
       
        ItemStat = new StatSystem();
        BuffStat = new StatSystem();


        PlayTime = 0;
        PierceCount = 0;


        Rune = Managers.Item.Database.RuneEquipment.Clone();
        Rune.SetOwner(this);
        Pet = Managers.Item.Database.PetEquipment.Clone();
        Pet.SetOwner(this);



        Bow = Managers.Item.Database.BowEquipment.Clone();
        Bow.SetOwner(this);
        Helmet = Managers.Item.Database.HelmetEquipment.Clone();
        Helmet.SetOwner(this);
        Armor = Managers.Item.Database.ArmorEquipment.Clone();
        Armor.SetOwner(this);
        Cloak = Managers.Item.Database.CloakEquipment.Clone();
        Cloak.SetOwner(this);


        Pet.Equip += EquipItems;
        Rune.Equip += EquipItems;
        Bow.Equip += EquipItems;
        Helmet.Equip += EquipItems;
        Armor.Equip += EquipItems;
        Cloak.Equip += EquipItems;

        Pet.UnEquip += UnEquipItems;
        Rune.UnEquip += UnEquipItems;
        Bow.UnEquip += UnEquipItems;
        Helmet.UnEquip += UnEquipItems;
        Armor.UnEquip += UnEquipItems;
        Cloak.UnEquip += UnEquipItems;

        _updater = Util.GetOrAddComponent<GameDataUpdater>(gameObject);
        _updater.Setup(this);


    }

    private void OnDestroy()
    {
        Pet.Equip -= EquipItems;
        Rune.Equip -= EquipItems;
        Bow.Equip -= EquipItems;
        Helmet.Equip -= EquipItems;
        Armor.Equip -= EquipItems;
        Cloak.Equip -= EquipItems;

        Pet.UnEquip -= UnEquipItems;
        Rune.UnEquip -= UnEquipItems;
        Bow.UnEquip -= UnEquipItems;
        Helmet.UnEquip -= UnEquipItems;
        Armor.UnEquip -= UnEquipItems;
        Cloak.UnEquip -= UnEquipItems;
    }

    private void Awake()
    {
        StartCoroutine(AddPlayTime());
    }

    private void OnEnable()
    {
        PlayerStat.Get(Define.StatID.Attack.ToString()).OnLevelChanged += UpgradeStatReport;
        PlayerStat.Get(Define.StatID.AttackSpeed.ToString()).OnLevelChanged += UpgradeStatReport;
        PlayerStat.Get(Define.StatID.CriticalHit.ToString()).OnLevelChanged += UpgradeStatReport;
        PlayerStat.Get(Define.StatID.CriticalHitRate.ToString()).OnLevelChanged += UpgradeStatReport;

        

      
    }
    private void OnDisable()
    {
        PlayerStat.Get(Define.StatID.Attack.ToString()).OnLevelChanged -= UpgradeStatReport;
        PlayerStat.Get(Define.StatID.AttackSpeed.ToString()).OnLevelChanged -= UpgradeStatReport;
        PlayerStat.Get(Define.StatID.CriticalHit.ToString()).OnLevelChanged -= UpgradeStatReport;
        PlayerStat.Get(Define.StatID.CriticalHitRate.ToString()).OnLevelChanged -= UpgradeStatReport;

        
    }


    

    #region Event
    
    public bool IsCompletableDailyCheckoutQuest()
    {

        if (DailyCheckoutQuestLastClearTime.HasValue == false)
            return true;

        int totalsize = Managers.Quest.FindQuestByCategory(Define.QuestType.DailyCheckout.ToString()).Count();
        int progress = Managers.Quest.FindQuestByCategory(Define.QuestType.DailyCheckout.ToString()).Where(x => x.IsComplete).Count();
        if (totalsize == progress)
            return false;



        if (DailyCheckoutQuestLastClearTime.GetValueOrDefault().ToLocalTime().Date < GlobalTime.Now.ToLocalTime().Date)
            return true;
        else
            return false;

    }
   
    public bool IsAllCompletedQuest(Define.QuestType type)
    {

        return Managers.Quest.ActiveQuests.Where(x => x.Category == type.ToString()).Count() == 0 ? true : false;

    }
    public bool IsDayPassedQuest(Define.QuestType type)
    {
        if(type == Define.QuestType.Daily)
        {
            if (DailyQuestLastClearTime.HasValue == false)
                return true;

            if (DailyQuestLastClearTime.GetValueOrDefault().ToLocalTime().Date < GlobalTime.Now.ToLocalTime().Date)
                return true;
            else
                return false;

        }
        else if(type == Define.QuestType.DailyCheckout)
        {
            if (DailyCheckoutQuestLastClearTime.HasValue == false)
                return true;

            if (DailyCheckoutQuestLastClearTime.GetValueOrDefault().ToLocalTime().Date < GlobalTime.Now.ToLocalTime().Date)
                return true;
            else
                return false;

        }

        return false;
    }


    public void ProcessReportDailyCheckoutQuest()
    {
        
        if (IsAllCompletedQuest(Define.QuestType.DailyCheckout))
            return;


        int complete = Managers.Quest.CompletedQuests.Where(x => x.Category == Define.QuestType.DailyCheckout.ToString()).Count();
        int total = Managers.Quest.ActiveQuests.Where(x => x.Category == Define.QuestType.DailyCheckout.ToString()).Count();
        total += complete;

        int setSuccessCount = (complete + 1) % total;
        if(setSuccessCount == 0)
            OnDailyCheckout?.Invoke(total);
        else
            OnDailyCheckout?.Invoke(setSuccessCount);


        foreach (Quest quest in Managers.Quest.ActiveQuests.Where(x => x.Category == Define.QuestType.DailyCheckout.ToString()).ToList())
        {
            if (quest.IsCompletable)
                quest.Complete();
        }

        DailyCheckoutQuestLastClearTime = GlobalTime.Now;
    }

    
    IEnumerator AddPlayTime()
    {
        while(true)
        {
            yield return new WaitForSeconds(60.0f);
            PlayTime++;
            OnPlayTime?.Invoke(1);
        }
    }
    public void CompleteStage()
    {
        if(Managers.Scene.CurrentScene is MainScene)
        {
            OnClearStage?.Invoke(MaxClearStage);
        }
        else if(Managers.Scene.CurrentScene is TrollScene)
        {
            OnClearStage?.Invoke(TrollStage);
        }
       
    }
    

    public void UpgradeStatReport(CharacterStat target, int count)
    {
        OnUpgradeStat?.Invoke(target, count);
    }

    public void OnLoaded()
    {
        Dictionary<string, SpriteGroupEntry> spriteCollection;

        foreach (EquipmentSlot bowSlot in Bow.SlotList )
            if(bowSlot.IsEquip)
            {
                spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Bow.ToDictionary(i => i.Id, i => i);
                gameObject.GetComponent<Character>().Equip(spriteCollection[bowSlot.GetItem.SpriteCollectionId], EquipmentPart.Bow);

                Bow _bow = bowSlot.GetItem as Bow;

                ItemStat.AddModfierList(_bow.StatModifiers.ToList());
            }
        foreach (EquipmentSlot helmetSlot in Helmet.SlotList)
            if (helmetSlot.IsEquip)
            {
                spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Helmet.ToDictionary(i => i.Id, i => i);
                gameObject.GetComponent<Character>().Equip(spriteCollection[helmetSlot.GetItem.SpriteCollectionId], EquipmentPart.Helmet);

                Helmet _helmet = helmetSlot.GetItem as Helmet;
                ItemStat.AddModfierList(_helmet.StatModifiers.ToList());
            }
        foreach (EquipmentSlot armorSlot in Armor.SlotList)
            if (armorSlot.IsEquip)
            {
                spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Armor.ToDictionary(i => i.Id, i => i);
                gameObject.GetComponent<Character>().Equip(spriteCollection[armorSlot.GetItem.SpriteCollectionId], EquipmentPart.Armor);

                Armor _armor = armorSlot.GetItem as Armor;
                ItemStat.AddModfierList(_armor.StatModifiers.ToList());
            }
        foreach (EquipmentSlot cloakSlot in Cloak.SlotList)
            if (cloakSlot.IsEquip)
            {
                spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Cape.ToDictionary(i => i.Id, i => i);
                gameObject.GetComponent<Character>().Equip(spriteCollection[cloakSlot.GetItem.SpriteCollectionId], EquipmentPart.Cape);

                Cloak _armor = cloakSlot.GetItem as Cloak;
                ItemStat.AddModfierList(_armor.StatModifiers.ToList());
            }
        foreach (EquipmentSlot runeSlot in Rune.SlotList)
            if (runeSlot.IsEquip)
            {
                Rune _rune = runeSlot.GetItem as Rune;
                RuneStat.AddModfierList(_rune.StatModifiers.ToList());
            }

        foreach (EquipmentSlot petSlot in Pet.SlotList)
            if (petSlot.IsEquip)
            {
                Pet _pet = petSlot.GetItem as Pet;
                PetStat.AddModfierList(_pet.StatModifiers.ToList());
            }


        foreach (Buff buff in Inventory.FindAll(x => x is Buff))
        {
            Debug.Log("Load Buff");
            Managers.Game.GiveBuff(buff);
        }

        
        if (Level != 0)
            GetComponent<Character>().SetBody(GetComponent<Character>().SpriteCollection.Hair.ToDictionary(i => i.Id, i => i)[GameManagerEx.PlayerHairStringId], BodyPart.Hair);
        else if (!Managers.Game.PlyaerDataBase.RankDic.ContainsKey(Level))
            return;
        
        PierceCount = Managers.Game.PlyaerDataBase.RankDic[Level].PierceCount;
        foreach (StatModifier _getmodifier in Managers.Game.PlyaerDataBase.RankDic[Level].StatModifiers)
            PlayerStat.AddModfier(_getmodifier);

        if (Inventory.IsFindItem(Managers.Shop.Database.Seasonpass.Item.ItemId))
            PurchaseItemId(Managers.Shop.Database.Seasonpass.Item.ItemId, 1);
        
        if(Inventory.IsFindItem(Managers.Shop.Database.Seasonpass2.Item.ItemId))
            PurchaseItemId(Managers.Shop.Database.Seasonpass2.Item.ItemId, 1);


        Managers.Game.OnItemChanged();
    }
    void EquipItems(EquipableItem item)
    {
        Dictionary<string, SpriteGroupEntry> spriteCollection;

        if (item.ItemClass == "Bow")
        {
            spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Bow.ToDictionary(i => i.Id, i => i);
            gameObject.GetComponent<Character>().Equip(spriteCollection[item.SpriteCollectionId], EquipmentPart.Bow);

            Bow _bow = item as Bow;

            ItemStat.AddModfierList(_bow.StatModifiers.ToList());

          
        }
        else if (item.ItemClass == "Helmet")
        {
            spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Helmet.ToDictionary(i => i.Id, i => i);
            gameObject.GetComponent<Character>().Equip(spriteCollection[item.SpriteCollectionId], EquipmentPart.Helmet);

            Helmet _helmet = item as Helmet;
            ItemStat.AddModfierList(_helmet.StatModifiers.ToList());

          
        }
        else if (item.ItemClass == "Armor")
        {
            spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Armor.ToDictionary(i => i.Id, i => i);
            gameObject.GetComponent<Character>().Equip(spriteCollection[item.SpriteCollectionId], EquipmentPart.Armor);


            Armor _armor = item as Armor;
            ItemStat.AddModfierList(_armor.StatModifiers.ToList());


        }
        else if (item.ItemClass == "Cloak")
        {
            spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Cape.ToDictionary(i => i.Id, i => i);
            gameObject.GetComponent<Character>().Equip(spriteCollection[item.SpriteCollectionId], EquipmentPart.Cape);

            Cloak _cloak = item as Cloak;
            ItemStat.AddModfierList(_cloak.StatModifiers.ToList());

           
        }
        else if (item.ItemClass == "Rune")
        {
            Rune _rune = item as Rune;
            RuneStat.AddModfierList(_rune.StatModifiers.ToList());

        }
        else if (item.ItemClass == "Pet")
        {
            Pet _pet = item as Pet;
            PetStat.AddModfierList(_pet.StatModifiers.ToList());

        }
        OnEquipItem?.Invoke(item.ItemClass);

    }

    void UnEquipItems(EquipableItem item)
    {
        Dictionary<string, SpriteGroupEntry> spriteCollection;

        if (item.ItemClass == "Bow")
        {
            spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Bow.ToDictionary(i => i.Id, i => i);
            gameObject.GetComponent<Character>().Equip(spriteCollection["FantasyHeroes.Basic.Bow.PathFinderBow"], EquipmentPart.Bow);
            //PlayerObject.GetComponent<Character>().SetBody(BodyPart.Hair)

            Bow _bow = item as Bow;

            ItemStat.RemoveModfierList(_bow.StatModifiers.ToList());
           
        }
        else if (item.ItemClass == "Helmet")
        {
            spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Helmet.ToDictionary(i => i.Id, i => i);
            //gameObject.GetComponent<Character>().Equip(spriteCollection[item.SpriteCollectionId], EquipmentPart.Helmet);
            gameObject.GetComponent<Character>().UnEquip(EquipmentPart.Helmet);
            Helmet _helmet = item as Helmet;
            ItemStat.RemoveModfierList(_helmet.StatModifiers.ToList());
        }
        else if (item.ItemClass == "Armor")
        {
            spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Armor.ToDictionary(i => i.Id, i => i);
            gameObject.GetComponent<Character>().Equip(spriteCollection["FantasyHeroes.Basic.Armor.Rags3"], EquipmentPart.Armor);

            Armor _armor = item as Armor;
            ItemStat.RemoveModfierList(_armor.StatModifiers.ToList());
        }
        else if (item.ItemClass == "Cloak")
        {
            spriteCollection = gameObject.GetComponent<Character>().SpriteCollection.Cape.ToDictionary(i => i.Id, i => i);
            //gameObject.GetComponent<Character>().Equip(spriteCollection[item.SpriteCollectionId], EquipmentPart.Cape);
            gameObject.GetComponent<Character>().UnEquip(EquipmentPart.Cape);
            Cloak _cloak = item as Cloak;
            ItemStat.RemoveModfierList(_cloak.StatModifiers.ToList());
        }
        else if (item.ItemClass == "Rune")
        {
            Rune _rune = item as Rune;
            RuneStat.RemoveModfierList(_rune.StatModifiers.ToList());
        }
        else if (item.ItemClass == "Pet")
        {
            Pet _pet = item as Pet;
            PetStat.RemoveModfierList(_pet.StatModifiers.ToList());

        }



    }
    public void UpgradeItems(string ItemClass)
    {
        if (ItemClass == "Heart")
            OnUpgradeItem?.Invoke(ItemClass);

    }
    public void PurchaseItemClass(string ItemClass , int count , bool isFree = false)
    {
        OnPurchaseItemClass?.Invoke(ItemClass, count , isFree);
    }
    public void PurchaseItemId(string ItemId, int count)
    {
        OnPurchaseItemId?.Invoke(ItemId, count);
    }
    public void OpenPopup(UI_Popup popup)
    {
        OnOpenPopup?.Invoke(popup);
    }
    public void KillMonster(Define.MonsterType type)
    {
        OnKill?.Invoke(type);
        
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
            Managers.Game.Save(new PlayerInfo.StatisticsDataKey[] { PlayerInfo.StatisticsDataKey.Level, PlayerInfo.StatisticsDataKey.Stage, PlayerInfo.StatisticsDataKey.ClearStage, PlayerInfo.StatisticsDataKey.MaxClearStage });
    }
    #endregion



}
