//using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common;
using HeroEditor.Common.Enums;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Google.Play.Review;

using Assets.HeroEditor.Common.CharacterScripts;

struct HitInfo
{
    public MonsterController Target;
    public Define.DamageType DamageType;
    public System.Numerics.BigInteger Damage;
    public void Set(MonsterController target, Define.DamageType type , System.Numerics.BigInteger Damage)
    {
        Target = target; DamageType = type; this.Damage = Damage;
    }
}

public class GameManagerEx
{
    
    const string PlayerObjectName = "Player";
    public const string PlayerHairStringId = "Common.Bonus.Hair.AnimeGirl";
    const string savePath = "/SaveData.json";

    public bool AutoForTest = true;

    static GameObject PlayerObject = null;
    static GameData PlayerData = null;
    
    
    List<GameObject> _monster = new List<GameObject>();
    List<GameObject> _objs = new List<GameObject>();
    List<GameObject> _arrows = new List<GameObject>();

   
    List<HitInfo> _hitList = new List<HitInfo>();
    List<HitInfo> _hitPoolingList = new List<HitInfo>();

    List<Buff> _buffList = new List<Buff>();



    

    public delegate void PlayerLevelChangedHandler();
    public PlayerLevelChangedHandler OnLevelChanged = null;

    public delegate void PlayerReviveEventHandler();
    public PlayerReviveEventHandler OnRevived = null;



    public delegate void ResetStageHander();
    public ResetStageHander OnResetScene = null;

    public delegate void OnChangedCurrencyHandler(Define.CurrencyID id, string Amount);
    public event OnChangedCurrencyHandler OnCurrencyChanged = null;

    public delegate void OnResetCurrencyHandler(Define.CurrencyID id);
    public event OnResetCurrencyHandler OnCurrencyUpdated = null;


    PlayerDatabase _playerdatabase;
    StageDatabase _stagedatabse;


    public string PlayerId = string.Empty;   
    public PlayerDatabase PlyaerDataBase => _playerdatabase;
    public StageDatabase StageDataBase => _stagedatabse;

    public int Level { get { return PlayerData.Level; } }
    public int Stage { get { return PlayerData.Stage; } }
    public void ResetStage()
    {
        PlayerData.Stage = 1;
        PlayerData.ClearStage = 0;
        OnResetScene?.Invoke();
        


    }
    public void MoveStage(int stage)
    {
        PlayerData.Stage = stage;
        PlayerData.ClearStage = stage - 1;
        OnResetScene?.Invoke();
    }
    public int TrollStage { get { return PlayerData.TrollStage; } }
    
    
    public void Init()
    {

      

        _playerdatabase = Managers.Resource.Load<PlayerDatabase>("Database/PlayerDatabase");
        _stagedatabse = Managers.Resource.Load<StageDatabase>("Database/StageDatabase");



        PlayerObject = Managers.Resource.Instantiate($"Player/{PlayerObjectName}");
        Util.GetOrAddComponent<PlayerController>(PlayerObject);
        UnityEngine.Object.DontDestroyOnLoad(PlayerObject);


        PlayerData = Util.GetOrAddComponent<GameData>(PlayerObject);
      

        PlayerData.Init();
       


        foreach (Currency currency in _playerdatabase.CurrencyList)
        {
            Debug.Log(currency.CodeName);
            PlayerData.Currency.Add(currency.CodeName, currency.Clone());

        }
        
        foreach (CharacterStat _stat in _playerdatabase.StatList)
        {
            CharacterStat _copy = _stat.clone();
            _copy.Currency = PlayerData.Currency[_copy.CurrencyCode];
            PlayerData.PlayerStat.Add(_copy);
           

        }

        foreach (Define.CurrencyID _currencyId in System.Enum.GetValues(typeof(Define.CurrencyID)))
        {
            Debug.Log(_currencyId.ToString());
            PlayerData.Currency[_currencyId.ToString()].Reset();
        }

        //PlayerData.PlayerStat.Get(Define.StatID.SkillMaxLevelLimit.ToString()).OnLevelChanged -= PlayerData.PlayerStat.Get(Define.StatID.SkillAttack.ToString()).OnChangedMaxLevel;
        //PlayerData.PlayerStat.Get(Define.StatID.SkillMaxLevelLimit.ToString()).OnLevelChanged += PlayerData.PlayerStat.Get(Define.StatID.SkillAttack.ToString()).OnChangedMaxLevel;

        foreach (BaseStat _stat in _playerdatabase.ItemStatList)
            PlayerData.ArtifactStat.Add(_stat.Clone());
        foreach (BaseStat _stat in _playerdatabase.ItemStatList)
            PlayerData.ItemStat.Add(_stat.Clone());

        foreach (BaseStat _stat in _playerdatabase.ItemStatList)
            PlayerData.RuneStat.Add(_stat.Clone());
        foreach (BaseStat _stat in _playerdatabase.ItemStatList)
            PlayerData.PetStat.Add(_stat.Clone());
        foreach (BaseStat _stat in _playerdatabase.ItemStatList)
            PlayerData.BuffStat.Add(_stat.Clone());


        PlayerData.SkillSet = _playerdatabase.SkillList.Select(x=>x.Clone()).ToList();





        foreach (StatModifier _getmodifier in _playerdatabase.RankDic[PlayerData.Level].StatModifiers)
            PlayerData.PlayerStat.AddModfier(_getmodifier);

        PlayerData.Inventory.OnItemChanged -= OnItemChanged;
        PlayerData.Inventory.OnItemChanged += OnItemChanged;


        if(Managers.Network.IS_ENABLE_NETWORK == false)
            Load();
        
       

        Managers.Item.GetUpgradeSystem("Heart").OnItemUpgraded += (consumes , target) =>{
            if (target.ItemClass == "Heart")
                PlayerData.UpgradeItems(target.ItemClass);
                
        };

    }

    public bool BeforeBeginPlay()
    {
        bool _isBeginner = PlayerData.IsBeginner;
        // 초보사용 버프 부여
        if (_isBeginner == true)
        {
            List<Buff> _addedBuff = new List<Buff>();
            _addedBuff.AddRange(Managers.Item.GrantItemToUser(Buff.Id.Buff_Auto_Beginner.ToString()).Select(x=>x as Buff).ToList());
            _addedBuff.AddRange(Managers.Item.GrantItemToUser(Buff.Id.Buff_Attack_Beginner.ToString()).Select(x => x as Buff).ToList());
            _addedBuff.AddRange(Managers.Item.GrantItemToUser(Buff.Id.Buff_AttackSpeed_Beginner.ToString()).Select(x => x as Buff).ToList());
            _addedBuff.AddRange(Managers.Item.GrantItemToUser(Buff.Id.Buff_GoldDropRate_Beginner.ToString()).Select(x => x as Buff).ToList());
            _addedBuff.AddRange(Managers.Item.GrantItemToUser(Buff.Id.Buff_Skill_Beginner.ToString()).Select(x => x as Buff).ToList());

            PlayerData.IsBeginner = false;

            if(Managers.Network.IS_ENABLE_NETWORK == false)
                Save();
            else
            {
                Managers.Player.GetPlayer(PlayerId).Payload.UserData[PlayerInfo.UserDataKey.IsBeginner.ToString()].Value = PlayerData.IsBeginner.ToString();
                
                
                Managers.Network.UpdateUserData(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.IsBeginner }, Managers.Player.GetPlayer(PlayerId));
                Managers.Network.GrantItems(_addedBuff.Select(x => x.ToGrantItem()).ToList() , 
                    (result)=> 
                    {
                        PlayFab.ServerModels.GrantItemsToUsersResult _result = result as PlayFab.ServerModels.GrantItemsToUsersResult;
                        foreach (PlayFab.ServerModels.GrantedItemInstance item in _result.ItemGrantResults)
                        {
                            Managers.Game.GetInventory().Find(x => x.ItemId == item.ItemId).Setup(new PlayFab.ClientModels.ItemInstance
                                {
                                    ItemId = item.ItemId,
                                    Expiration = item.Expiration,
                                    ItemClass = item.ItemClass,
                                    CatalogVersion = item.CatalogVersion,
                                    DisplayName = item.DisplayName,
                                    ItemInstanceId = item.ItemInstanceId,
                                    UnitCurrency = item.UnitCurrency,
                                    UnitPrice = item.UnitPrice,
                                    PurchaseDate = item.PurchaseDate,
                                    RemainingUses = item.RemainingUses,
                                    UsesIncrementedBy = item.UsesIncrementedBy,
                                    CustomData = item.CustomData
                                });
                            
                        }
                    });
                Managers.Item.GiveCouponToUser("WELCOME", PlayerData);
            }
        }

        return _isBeginner;

    }
    
    public void Save()
    {
        string _savePath = Application.persistentDataPath + savePath;
        File.WriteAllText(_savePath, PlayerData.ToSaveData().ToString(),System.Text.Encoding.UTF8);

        Managers.Quest.Save();
        Debug.Log($"Save Game Completed : {_savePath}");
    }
    public void Save(PlayerInfo.UserDataKey SaveDataKey)
    {
        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            if (QuestFinder.FindKeys().Any(x=> x == SaveDataKey))
                Managers.Quest.Save(SaveDataKey);
            else
                PlayerData.UpdateData(SaveDataKey, Managers.Player.GetPlayer(PlayerId));

            Managers.Network.UpdateUserData(new PlayerInfo.UserDataKey[] { SaveDataKey }, Managers.Player.GetPlayer(PlayerId));
        }
    }

    public void Save(PlayerInfo.UserDataKey [] SaveDataKeys)
    {
        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            foreach(PlayerInfo.UserDataKey key in SaveDataKeys)
                PlayerData.UpdateData(key, Managers.Player.GetPlayer(PlayerId));
#if ENABLE_LOG

            PlayerInfo.UserDataKey[] _filteringSaveDataKeys = SaveDataKeys.Where(x => x != PlayerInfo.UserDataKey.DPS).ToArray();
            Managers.Network.UpdateUserData(_filteringSaveDataKeys, Managers.Player.GetPlayer(PlayerId));
#else
            Managers.Network.UpdateUserData(SaveDataKeys, Managers.Player.GetPlayer(PlayerId));
#endif
         
        }
    }

    public void Save(PlayerInfo.StatisticsDataKey[] SaveDataKeys)
    {
        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            foreach (PlayerInfo.StatisticsDataKey key in SaveDataKeys)
                PlayerData.UpdateData(key, Managers.Player.GetPlayer(PlayerId));
#if ENABLE_LOG
            foreach(PlayerInfo.StatisticsDataKey key in SaveDataKeys)
            {
                PlayFab.ClientModels.StatisticValue _findValue = Managers.Player.GetPlayer(PlayerId).Payload.PlayerStatistics.Where(x=>x.StatisticName == key.ToString()).FirstOrDefault();
                if(_findValue != null)
                {
                    PlayerPrefs.SetInt(key.ToString(), _findValue.Value);
                }
            }
            PlayerPrefs.Save();

#else
            Managers.Network.UpdateStatisticsData(SaveDataKeys, Managers.Player.GetPlayer(PlayerId));
#endif

        }
    }

    public void Save(PlayerInfo.StatisticsDataKey SaveDataKey)
    {
        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            
            PlayerData.UpdateData(SaveDataKey, Managers.Player.GetPlayer(PlayerId));
#if ENABLE_LOG
            PlayFab.ClientModels.StatisticValue _findValue = Managers.Player.GetPlayer(PlayerId).Payload.PlayerStatistics.Where(x => x.StatisticName == SaveDataKey.ToString()).FirstOrDefault();
            if (_findValue != null)
            {
                PlayerPrefs.SetInt(SaveDataKey.ToString(), _findValue.Value);
            }
            
            PlayerPrefs.Save();
#else
            Managers.Network.UpdateStatisticsData(new PlayerInfo.StatisticsDataKey[] { SaveDataKey }, Managers.Player.GetPlayer(PlayerId));
#endif
    }
    }
    public void Load()
    {
        string _savePath = Application.persistentDataPath + savePath;
        if (!File.Exists(_savePath))
            return;

        string _loadData = File.ReadAllText(_savePath, System.Text.Encoding.UTF8);
        PlayerData.Load(GameSaveData.Parse(_loadData));
        
    }

    public void Load(PlayerInfo User)
    {
        // Quest 를 먼저 로드해야 SeasonPass 이벤트 수신가능
        Managers.Quest.Load(User);
        PlayerData.Load(User);

        PlayerData.ResetStage();
        PlayerData.ResetQuest();

    }
    
    public GameObject GetRankIcon(Transform parent = null)
    {
        return Managers.Resource.Instantiate(_playerdatabase.RankDic[Level].Icon, parent);
    }
    public void Levelup()
    {
        
        if (PlayerData.Level == 0)
            PlayerObject.GetComponent<Character>().SetBody(PlayerObject.GetComponent<Character>().SpriteCollection.Hair.ToDictionary(i => i.Id, i => i)[PlayerHairStringId], BodyPart.Hair);
        else if (!_playerdatabase.RankDic.ContainsKey(PlayerData.Level))
            return;


        foreach(StatModifier _getmodifier in _playerdatabase.RankDic[PlayerData.Level].StatModifiers)
            PlayerData.PlayerStat.RemoveModfier(_getmodifier);
        
        PlayerData.Level++;
        PlayerData.PierceCount = _playerdatabase.RankDic[PlayerData.Level].PierceCount;

        foreach (StatModifier _getmodifier in _playerdatabase.RankDic[PlayerData.Level].StatModifiers)
            PlayerData.PlayerStat.AddModfier(_getmodifier);

        // 절전모드일 때 끊김현상
        // 절전모드를 먼저 종류한다.
        UI_Shutdown _shutdown = GameObject.FindObjectOfType<UI_Shutdown>();
        if (_shutdown != null)
        {
            _shutdown.ClosePopupUI();
        }
            

        OnLevelChanged?.Invoke();
        
        Managers.Game.Save(new PlayerInfo.StatisticsDataKey[] { PlayerInfo.StatisticsDataKey.Stage, PlayerInfo.StatisticsDataKey.MaxClearStage, PlayerInfo.StatisticsDataKey.ClearStage , PlayerInfo.StatisticsDataKey.Level });
        Managers.UI.ShowPopupUI<UI_RankupNotice>();

        PlayerData.PlayerStat.OnPlayerLevelChanged(PlayerData.Level);
        PlayerData.Rune.OnUpdateSlot(PlayerData.Level);
        PlayerData.Pet.OnUpdateSlot(PlayerData.Level);
        Managers.Game.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.PlayerStat , PlayerInfo.UserDataKey.Rune, PlayerInfo.UserDataKey.Pet });


        if (PlayerData.Level == 1)
        {
            Managers.UI.ShowPopupUI<UI_Review>();
            /* 리뷰 표출
            var reviewManager = new ReviewManager();
            var reviewAsyncOperation = reviewManager.RequestReviewFlow();

            reviewAsyncOperation.Completed += playReviewInfoAsync => {
                if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
                {
                    // display the review prompt
                    var playReviewInfo = playReviewInfoAsync.GetResult();
                    reviewManager.LaunchReviewFlow(playReviewInfo);
                }
                else
                {
                    // handle error when loading review prompt
                }
                


            };
            */
        }


       
    }

    public void LevelChanged()
    {
        if (PlayerData.Level != 0)
            PlayerObject.GetComponent<Character>().SetBody(PlayerObject.GetComponent<Character>().SpriteCollection.Hair.ToDictionary(i => i.Id, i => i)[PlayerHairStringId], BodyPart.Hair);
        else if (!_playerdatabase.RankDic.ContainsKey(PlayerData.Level))
            return;

        PlayerData.PierceCount = _playerdatabase.RankDic[PlayerData.Level].PierceCount;
        foreach (StatModifier _getmodifier in _playerdatabase.RankDic[PlayerData.Level].StatModifiers)
            PlayerData.PlayerStat.AddModfier(_getmodifier);

    }
    public string GetDisplayRank()
    {
        return _playerdatabase.RankDic[PlayerData.Level].DisplayName;
    }
#region Currency
    public System.Numerics.BigInteger GetCurrency(string Id)
    {
        return PlayerData.Currency[Id].Amount;
    }
    public void AddCurrency(string Id , System.Numerics.BigInteger Amount , bool IsApplyDropRate = false , bool IsUpdate = false)
    {
        if (!PlayerData.Currency.ContainsKey(Id))
            return;

        System.Numerics.BigInteger _addedCurrency = IsApplyDropRate ? CalculateDropRateAmount((Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), Id), Amount) : Amount;
        PlayerData.Currency[Id].Add(_addedCurrency);
        Define.CurrencyID _id = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), Id);

        if (Amount > 0)
        {
            OnCurrencyChanged?.Invoke(_id, "+" + _addedCurrency.ToString());
        }

        if(IsUpdate == true)
        {
            if (_id == Define.CurrencyID.Ruby || _id == Define.CurrencyID.SP || _id == Define.CurrencyID.Coin)
            {
                Currency _currency = PlayerData.Currency.ToList().Where(x => x.Key == _id.ToString()).FirstOrDefault().Value;
                PlayerInfo.CurrencyKey _shortName = (PlayerInfo.CurrencyKey)Enum.Parse(typeof(PlayerInfo.CurrencyKey), _currency.ShortCodeName);

                Managers.Network.AddCurrency(_shortName, (int)_addedCurrency, Managers.Player.GetPlayer(PlayerId));
                
            }
            else if (_id == Define.CurrencyID.Gold || _id == Define.CurrencyID.CP)
            {
                PlayerInfo.UserDataKey _currencykey = (PlayerInfo.UserDataKey)Enum.Parse(typeof(PlayerInfo.UserDataKey), _id.ToString());
                Managers.Network.UpdateUserData( new PlayerInfo.UserDataKey[] { _currencykey }, Managers.Player.GetPlayer(PlayerId));
            }
        }
    }
    public void SubstractCurrency(string Id, System.Numerics.BigInteger Amount, bool IsUpdate = false)
    {
        if (!PlayerData.Currency.ContainsKey(Id))
            return;

        PlayerData.Currency[Id].Substract(Amount);
        Define.CurrencyID _id = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), Id);
        if (Amount > 0)
            OnCurrencyChanged?.Invoke(_id, "-" + Amount.ToString());
        

        if (IsUpdate == true)
        {
            if (_id == Define.CurrencyID.Ruby || _id == Define.CurrencyID.SP || _id == Define.CurrencyID.Coin)
            {
                Currency _currency = PlayerData.Currency.ToList().Where(x => x.Key == _id.ToString()).FirstOrDefault().Value;
                PlayerInfo.CurrencyKey _shortName = (PlayerInfo.CurrencyKey)Enum.Parse(typeof(PlayerInfo.CurrencyKey), _currency.ShortCodeName);

                Managers.Network.SubtractCurrency(_shortName, (int)Amount, Managers.Player.GetPlayer(PlayerId));
            }
            else if (_id == Define.CurrencyID.Gold || _id == Define.CurrencyID.CP)
            {
                PlayerInfo.UserDataKey _currencykey = (PlayerInfo.UserDataKey)Enum.Parse(typeof(PlayerInfo.UserDataKey), _id.ToString());
                
                Managers.Network.UpdateUserData(new PlayerInfo.UserDataKey[] { _currencykey }, Managers.Player.GetPlayer(PlayerId));
            }


        }
    }

    
    public void SynchronizeCurrency()
    {
        if (Managers.Network.IS_ENABLE_NETWORK == false)
            return;

        foreach(KeyValuePair<string , Currency> pair in PlayerData.Currency)
        {
            Currency _currency = pair.Value;
            
            if (_currency.CodeName == Define.CurrencyID.Gold.ToString() || _currency.CodeName == Define.CurrencyID.CP.ToString())
            {
                PlayerInfo.UserDataKey _currencykey = (PlayerInfo.UserDataKey)Enum.Parse(typeof(PlayerInfo.UserDataKey), _currency.CodeName);
                PlayerData.UpdateData(_currencykey, Managers.Player.GetPlayer(PlayerId));
                Managers.Network.UpdateUserData(new PlayerInfo.UserDataKey[] { _currencykey }, Managers.Player.GetPlayer(PlayerId));
            }
            
            if (_currency.CodeName == Define.CurrencyID.Ruby.ToString() || _currency.CodeName == Define.CurrencyID.SP.ToString())
            {
                PlayerInfo.CurrencyKey _shortName = (PlayerInfo.CurrencyKey)Enum.Parse(typeof(PlayerInfo.CurrencyKey), _currency.ShortCodeName);
                int _diff = (int)_currency.Amount - (Managers.Player.GetPlayer(PlayerId).Payload.UserVirtualCurrency[_shortName.ToString()]);

                if (_diff == 0)
                    continue;
                
                else if (_diff > 0)
                    Managers.Network.AddCurrency(_shortName, _diff, Managers.Player.GetPlayer(PlayerId));
                else
                    Managers.Network.SubtractCurrency(_shortName, Math.Abs(_diff), Managers.Player.GetPlayer(PlayerId));

                

            }
            
            

        }
    }
    public void ResetCurrency(string Id, bool IsUpdate = false)
    {
        System.Numerics.BigInteger _currentAmount = PlayerData.Currency[Id].Amount;
        if (_currentAmount <= 0)
            return;

        Define.CurrencyID _id = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), Id);
        OnCurrencyUpdated?.Invoke(_id);

        SubstractCurrency(Id, _currentAmount, IsUpdate);
    }

    // 획득 재화량 * ( 1 + 0.xx ) * Buff 로 얻는 획득률
    public System.Numerics.BigInteger CalculateDropRateAmount(Define.CurrencyID Id , System.Numerics.BigInteger Amount)
    {
        System.Numerics.BigInteger temp;
        if (Id == Define.CurrencyID.Gold)
        {
            temp = Amount * (System.Numerics.BigInteger)GetGoldDropRate;
            temp /= 100;
            temp += Amount;
            temp *= System.Numerics.BigInteger.Max(1, (System.Numerics.BigInteger)PlayerData.BuffStat.GetStatValue(Define.StatID.GoldDropRate.ToString()));
            return temp;

        }
        else if(Id == Define.CurrencyID.CP)
        {
            temp = Amount * (System.Numerics.BigInteger)GetCraftDropRate;
            temp /= 100;
            temp += Amount;
            temp *= System.Numerics.BigInteger.Max(1, (System.Numerics.BigInteger)PlayerData.BuffStat.GetStatValue(Define.StatID.CraftDropRate.ToString()));
            return temp;
        }
        else
            return Amount;
    }

    
#endregion
#region Stat

    // ((치명데미지 * 치명확률) + (Damamge * (1- 치명확률)) * 공속 * 0.5
    // 공속 ~ 0.1 = 2 , 1.5 = 0.4         // -1.142 * GetAttackSpeed + 2.2
    // 스킬데미지
    public System.Numerics.BigInteger DPS
    {
        get
        {
            System.Numerics.BigInteger CriticalDPS = CriticalHitDamage;
            System.Numerics.BigInteger _criticalHitRate = System.Numerics.BigInteger.Min(1000, GetCriticalHitRate);
         
                
            CriticalDPS *= _criticalHitRate;


            System.Numerics.BigInteger NormalDPS = Damage;
            NormalDPS *= (System.Numerics.BigInteger)(1000 - _criticalHitRate);

           
            // 확률의 경우 범위가( 0~1000) 이므로 1000을 추가로 나누어준다.
            System.Numerics.BigInteger _DPS = (CriticalDPS + NormalDPS) / 1000;
               

            double _attSpeed = (double)GetAttackSpeed;
            _attSpeed *= -1.142;
            _attSpeed += 2.2;

            _attSpeed *= 10.0;

            System.Numerics.BigInteger _attackSpeedForDPS = (System.Numerics.BigInteger)_attSpeed;

          

            _DPS *= (System.Numerics.BigInteger)_attackSpeedForDPS;
            _DPS /= 10;
            /*
            Debug.Log("CriticalHitRate : " + _criticalHitRate);
            Debug.Log(NormalDPS);
            Debug.Log(CriticalDPS);
            Debug.Log(_DPS);
            */

            return _DPS;
        }
    }
    // 20230710 // 버그 때문에 (PlayerData.PlayerStat.GetStatValue(attackString) + PlayerData.PlayerStat.GetStatValue(attackString)  + PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftAttack.ToString()))
    // 로 수정함.
    public System.Numerics.BigInteger Damage 
    { 
        get 
        {
            string attackString = Define.StatID.Attack.ToString();

            System.Numerics.BigInteger _damage = ((System.Numerics.BigInteger)(PlayerData.PlayerStat.GetStatValue(attackString) + PlayerData.PlayerStat.GetStatValue(attackString) + PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftAttack.ToString())) *
                (System.Numerics.BigInteger)Math.Max(1, PlayerData.RuneStat.GetStatValue(attackString) + PlayerData.ItemStat.GetStatValue(attackString)) * (System.Numerics.BigInteger)Math.Max(1, PlayerData.PetStat.GetStatValue(attackString)) *
               (System.Numerics.BigInteger)Math.Max(1, PlayerData.ArtifactStat.GetStatValue(attackString))) * Math.Max(1, PlayerData.BuffStat.GetStatValue(attackString));


            _damage *= (System.Numerics.BigInteger)(100 +  GetAttackAmp);
            _damage /= 100;

            _damage *= (System.Numerics.BigInteger)(100 + GetAllAttackAmp);
            _damage /= 100;

            return _damage;

        } 
    }
    public int AttackSpeed
    {
        get
        {
            int _generated = PlayerData.PlayerStat.GetStatValue(Define.StatID.AttackSpeed.ToString()) + PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftAttackSpeed.ToString()) + PlayerData.RuneStat.GetStatValue(Define.StatID.AttackSpeed.ToString()) + PlayerData.ItemStat.GetStatValue(Define.StatID.AttackSpeed.ToString());
            int _buff = PlayerData.BuffStat.GetStatValue(Define.StatID.AttackSpeed.ToString());
            return PlayerData.PlayerStat.GetStatValue(Define.StatID.AttackSpeed.ToString()) + PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftAttackSpeed.ToString())+ PlayerData.RuneStat.GetStatValue(Define.StatID.AttackSpeed.ToString()) + PlayerData.ItemStat.GetStatValue(Define.StatID.AttackSpeed.ToString());
        }
    }

    // 0.2 ~ 1.5  
    // 공속 버프 존재 시 / 2를 할것 20230111
    // 20230710 // 버그 수정PlayerData.PlayerStat.GetStatValue(attackSpeedString) + PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftAttackSpeed.ToString())
    public float GetAttackSpeed
    {
        get
        {
            string attackSpeedString = Define.StatID.AttackSpeed.ToString();
            int _generated = PlayerData.PlayerStat.GetStatValue(attackSpeedString) + PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftAttackSpeed.ToString()) + PlayerData.RuneStat.GetStatValue(attackSpeedString) + PlayerData.ItemStat.GetStatValue(attackSpeedString);
            int _buff = PlayerData.BuffStat.GetStatValue(attackSpeedString);
            double _value = 1.5 - (double)_generated * 0.002;
         

            if (_buff != 0) _value *= 0.5;

            _value = Math.Max(0.1, _value);

            return (float)_value;

            /*
            if (_buff != 0) _value += 0.6f;

            _value = 1.7f - _value;
            return Mathf.Max(0.1f, _value);
            */
            // return 1.7f - (_value);
        }
    }
    public float GetArrowSpeed
    {
        get
        {
            double _getArrowSpeed = Managers.Game.GetAttackSpeed;

           
            _getArrowSpeed *= 50.0;
            _getArrowSpeed = 95.0 - _getArrowSpeed;
          

            return (float)_getArrowSpeed;
        }
    }


    // 20230710 // 버그 때문에 (PlayerData.PlayerStat.GetStatValue(criticalHitString) + PlayerData.PlayerStat.GetStatValue(criticalHitString)  + PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftCriticalHit.ToString()))
    // 로 수정함
    public System.Numerics.BigInteger CriticalHitMultiplier
    {
        get
        {
            string criticalHitString = Define.StatID.CriticalHit.ToString();
            System.Numerics.BigInteger _multifier = ((System.Numerics.BigInteger)(PlayerData.PlayerStat.GetStatValue(criticalHitString) + PlayerData.PlayerStat.GetStatValue(criticalHitString) + PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftCriticalHit.ToString())) *
                ((System.Numerics.BigInteger)Math.Max(1, PlayerData.RuneStat.GetStatValue(criticalHitString)) + (System.Numerics.BigInteger)Math.Max(1, PlayerData.ItemStat.GetStatValue(criticalHitString))) * (System.Numerics.BigInteger)Math.Max(1, PlayerData.PetStat.GetStatValue(criticalHitString)) *
               (System.Numerics.BigInteger)Math.Max(1, PlayerData.ArtifactStat.GetStatValue(criticalHitString))) ;


            return _multifier; 
        }
    }
    public System.Numerics.BigInteger CriticalHitDamage
    {
        get
        {

            System.Numerics.BigInteger _multifier = CriticalHitMultiplier;

            _multifier += 100;
            _multifier /= 100; // ( 1+ 0.x)

            _multifier *= (System.Numerics.BigInteger)(100 + GetCriticalHitAmp);
            _multifier /= 100;

            _multifier *= (System.Numerics.BigInteger)(100 + GetAllAttackAmp);
            _multifier /= 100;




            return Damage * _multifier;
        }
    }
    public int GetCriticalHitRate
    {
        get
        {
            
            return PlayerData.PlayerStat.GetStatValue(Define.StatID.CriticalHitRate.ToString()) + PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftCriticalHitRate.ToString())
                + PlayerData.RuneStat.GetStatValue(Define.StatID.CriticalHitRate.ToString()) + PlayerData.PetStat.GetStatValue(Define.StatID.CriticalHitRate.ToString()) +
                 +PlayerData.ArtifactStat.GetStatValue(Define.StatID.CriticalHitRate.ToString()) + PlayerData.ItemStat.GetStatValue(Define.StatID.CriticalHitRate.ToString());
        }
    }
    public int GetGoldDropRate
    {
        get
        {
            return PlayerData.PlayerStat.GetStatValue(Define.StatID.GoldDropRate.ToString())
                + PlayerData.RuneStat.GetStatValue(Define.StatID.GoldDropRate.ToString()) + PlayerData.PetStat.GetStatValue(Define.StatID.GoldDropRate.ToString()) +
                 +PlayerData.ArtifactStat.GetStatValue(Define.StatID.GoldDropRate.ToString()) + PlayerData.ItemStat.GetStatValue(Define.StatID.GoldDropRate.ToString());
            
        }

    }
    public int GetCraftDropRate
    {
        get
        {
            return PlayerData.PlayerStat.GetStatValue(Define.StatID.CraftDropRate.ToString())
                + PlayerData.RuneStat.GetStatValue(Define.StatID.CraftDropRate.ToString()) + PlayerData.PetStat.GetStatValue(Define.StatID.CraftDropRate.ToString()) +
                 +PlayerData.ArtifactStat.GetStatValue(Define.StatID.CraftDropRate.ToString()) + PlayerData.ItemStat.GetStatValue(Define.StatID.CraftDropRate.ToString());
        }
    }
    public int GetJumpingRate
    {
        get
        {
            return PlayerData.PlayerStat.GetStatValue(Define.StatID.JumpingRate.ToString());
        }
    }
    public int GetAttackAmp
    {
        get
        {
            return PlayerData.ItemStat.GetStatValue(Define.StatID.AttackAmp.ToString());
        }
    }
    public int GetCriticalHitAmp
    {
        get
        {
            return PlayerData.ItemStat.GetStatValue(Define.StatID.CriticalHitAmp.ToString());
        }
    }
    public int GetSkillAttackAmp
    {
        get
        {
            return PlayerData.ItemStat.GetStatValue(Define.StatID.SkillAttackAmp.ToString());
        }
    }
    public int GetAllAttackAmp
    {
        get
        {
            return PlayerData.ItemStat.GetStatValue(Define.StatID.AllAttackAmp.ToString()) + PlayerData.RuneStat.GetStatValue(Define.StatID.AllAttackAmp.ToString()) + PlayerData.ReviveLevel;
        }
    }
    public int GetSkillMultipier
    {
        get
        {
            int GetSkillDamageStat = PlayerData.PlayerStat.GetStatValue(Define.StatID.SkillAttack.ToString()) + PlayerData.ItemStat.GetStatValue(Define.StatID.SkillAttack.ToString()) + PlayerData.PetStat.GetStatValue(Define.StatID.SkillAttack.ToString()) + PlayerData.RuneStat.GetStatValue(Define.StatID.SkillAttack.ToString()) + PlayerData.ArtifactStat.GetStatValue(Define.StatID.SkillAttack.ToString());

            return (100 + GetSkillDamageStat) * Math.Max( 1, PlayerData.BuffStat.GetStatValue(Define.StatID.SkillAttack.ToString()));
            

        }
    }
    public System.Numerics.BigInteger GetSkillDamage
    {
        get
        {
            System.Numerics.BigInteger _getDamage = CriticalHitDamage;
            _getDamage *= (System.Numerics.BigInteger)GetSkillMultipier;
            _getDamage /= 100;

            _getDamage *= (System.Numerics.BigInteger)(100 + GetSkillAttackAmp);
            _getDamage /= 100;

            _getDamage *= (System.Numerics.BigInteger)(100 + GetAllAttackAmp);
            _getDamage /= 100;
            return _getDamage;
        }
    }
    public string GetSkillCoolTimeString
    {
        get
        {
            int _stat =  (PlayerData.ItemStat.GetStatValue(Define.StatID.SkillCoolTime.ToString()) + PlayerData.PetStat.GetStatValue(Define.StatID.SkillCoolTime.ToString()) + PlayerData.RuneStat.GetStatValue(Define.StatID.SkillCoolTime.ToString())) * Math.Max(1 , PlayerData.BuffStat.GetStatValue(Define.StatID.SkillCoolTime.ToString()));
            return _stat.ToString();
        }
    }
    // Max 620%
    // Max 는 60초 Min 은 2초이며 버프를 받았을 시 1초
    // 0.0936 * 620% = 58.03
    // 0.0
    public double GetSkillCoolTime
    {
        get
        {
            string _skillCoolTime = Define.StatID.SkillCoolTime.ToString();

            double MaxCoolTime = PlayerData.SkillSet.First().MaxCoolTime;
            double MinCoolTime = PlayerData.SkillSet.First().MinCoolTime;

            double coolTimeFactor = (MaxCoolTime - MinCoolTime) / 620.0;

            int GetSkillCoolTimeStat = PlayerData.ItemStat.GetStatValue(_skillCoolTime) + PlayerData.PetStat.GetStatValue(_skillCoolTime) + PlayerData.RuneStat.GetStatValue(_skillCoolTime);
            double _CoolTimeSub = ((double)GetSkillCoolTimeStat) * coolTimeFactor;
            _CoolTimeSub = Math.Min(_CoolTimeSub, MaxCoolTime - MinCoolTime);


            if (PlayerData.BuffStat.GetStatValue(_skillCoolTime) != 0)
                return (MaxCoolTime - _CoolTimeSub) / 2.0;
            else
                return (MaxCoolTime - _CoolTimeSub);



        }
    }
    
    public bool IsJumping
    {
        get
        {
            int jumpingRate = UnityEngine.Random.Range(0, 100);
            if (jumpingRate < GetJumpingRate)
                return true;
            else
                return false;
        }
    }
    public bool IsCriticalHit
    {
        get
        {
            int calculate= UnityEngine.Random.Range(0, 1000);
            if (calculate < GetCriticalHitRate) return true;
            return false;
        }
    }

    public int GetJumpingCount
    {
        get
        {
            return PlayerData.PlayerStat.GetStatValue(Define.StatID.JumpingCount.ToString()) + PlayerData.PetStat.GetStatValue(Define.StatID.JumpingCount.ToString()) + PlayerData.RuneStat.GetStatValue(Define.StatID.JumpingCount.ToString());
        }
    }
    public int GetPierceCount
    {
        get
        {
            //Debug.Log(PlayerData.PlayerStat.GetStatValue(Define.StatID.ExtraHit.ToString()));
            return PlayerData.PlayerStat.GetStatValue(Define.StatID.ExtraHit.ToString());
        }
    }
    public bool IsPierce
    {
        get
        {
            int _pierceRate = _playerdatabase.RankDic[PlayerData.Level].PierceRate;
            int _generateRate = UnityEngine.Random.Range(0, 100);
            if (_generateRate < _pierceRate)
            {
                return true;
            }
            else
                return false;
            
        }
    }
    public bool IsAuto
    {
        get
        {
            
            if (Managers.Game.GetInventory().IsFindItem(x => x.ItemId == Buff.Id.Buff_Auto_AD.ToString() || x.ItemId == Buff.Id.Buff_Auto_IAP.ToString() ||
             x.ItemId == Buff.Id.Buff_Auto_Beginner.ToString()) && AutoForTest)
                return true;
            else
                return false;
            
            //return false;

        }
    }
    

    public void ResetStat(Define.StatType type = Define.StatType.Gold)
    {
        PlayerData.PlayerStat.Reset(type);
    }
    
#endregion
    
    public bool IsAdSkipped
    {
        get
        {
            return PlayerData.IsAdSkip;
        }
    }
    

    public void Revive(bool IsAdReward = false)
    {
        StageTask _mainStageTask = StageDataBase.StageList.Where(x => x.type == Define.Dongeon.Main).FirstOrDefault();

        int CurrentStage = Managers.Game.Stage;
        // 광고 스킵 패키지가 있는지 체크
        if (IsAdSkipped == true)
        {
            // 환생전에 스텟과 스테이지가 초기화되므로 그전에 유저에게 보상을 제공한다
            AddCurrency(Define.CurrencyID.CP.ToString(), _mainStageTask.GetMonsterHP(CurrentStage) , IsApplyDropRate: true);
            AddCurrency(Define.CurrencyID.Ruby.ToString(), PlayerData.GetReviveRubyAmount(CurrentStage), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
            PlayerData.Revive(CurrentStage);

            // 스텟과 스테이지 초기화
            ResetStat();
            ResetStage();
            ResetCurrency(Define.CurrencyID.Gold.ToString());


        }
        else // 없으면 광고 시청 유무 체크 하여 CP 를 준다.
        {
            if(IsAdReward)
                Managers.Item.GrantItemToUser(Buff.Id.Buff_CPDropRate_AD.ToString());

            // 환생전에 스텟과 스테이지가 초기화되므로 그전에 유저에게 보상을 제공한다    
            AddCurrency(Define.CurrencyID.CP.ToString(),_mainStageTask.GetMonsterHP(CurrentStage) , IsApplyDropRate: true);
            AddCurrency(Define.CurrencyID.Ruby.ToString(), PlayerData.GetReviveRubyAmount(CurrentStage), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
            PlayerData.Revive(CurrentStage);


            // 스텟과 스테이지 초기화
            ResetStat();
            ResetStage();
            GetInventory().RemoveItem(Buff.Id.Buff_CPDropRate_AD.ToString());
            ResetCurrency(Define.CurrencyID.Gold.ToString());

            

        }
        PlayerData.Rune.OnUpdateSlot(0);
        PlayerData.Pet.OnUpdateSlot(0);



        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            Save(new PlayerInfo.StatisticsDataKey[] { PlayerInfo.StatisticsDataKey.Stage, PlayerInfo.StatisticsDataKey.ClearStage, PlayerInfo.StatisticsDataKey.MaxClearStage , PlayerInfo.StatisticsDataKey.ReviveLevel });
            Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.PlayerStat, PlayerInfo.UserDataKey.Gold, PlayerInfo.UserDataKey.CP , PlayerInfo.UserDataKey.ReviveInfo });
        }

        OnRevived?.Invoke();
    }

    public StatSystem GetArtifactStat()
    {
        return PlayerData.ArtifactStat;
    }
    public void OnItemChanged()
    {

        PlayerData.ArtifactStat.ClearModifier();

        foreach(BaseItem item in PlayerData.Inventory.ToList().Where(x=>x is Artifact))
        {
            Artifact _artifact = item as Artifact;
            PlayerData.ArtifactStat.AddModfier(_artifact.StatModifier);
        }

        

    }
    

    public GameObject CreatePlayer()
    {

        if (PlayerObject == null)
        {
            PlayerObject = Managers.Resource.Instantiate($"Player/{PlayerObjectName}");
            Util.GetOrAddComponent<PlayerController>(PlayerObject);
          
            UnityEngine.Object.DontDestroyOnLoad(PlayerObject);
        }

        PlayerObject.SetActive(true);
        return PlayerObject;
    }
    public EquipmentSystem GetEquipment(string ItemClass)
    {
        if (ItemClass == "Rune")
            return PlayerData.Rune;
        else if (ItemClass == "Pet")
            return PlayerData.Pet;
        else if (ItemClass == "Bow")
            return PlayerData.Bow;
        else if (ItemClass == "Helmet")
            return PlayerData.Helmet;
        else if (ItemClass == "Armor")
            return PlayerData.Armor;
        else if (ItemClass == "Cloak")
            return PlayerData.Cloak;
        else
            return null;

    }

    public Inventory GetInventory()
    {
        return PlayerData.Inventory;
    }

    public CharacterStat [] GetPlayerStatForUI()
    {
        return PlayerData.PlayerStat.StatList.Where(x => x.type != Define.StatType.None).ToArray();
    }

    public void GiveBuff(Buff buff)
    {

        if(_buffList.Contains(buff))
        {
            //PlayerData.BuffStat.AddModfierList(buff.StatModifier.ToList());
        }
        else
        {
            if(_buffList.Any(x=>x.type == buff.type))
            {
                Buff getbuff = _buffList.Where(x => x.type == buff.type).OrderByDescending(x => x.Level).First();
               
                if(getbuff.Level <= buff.Level)
                {
                    PlayerData.BuffStat.RemoveModfierList(getbuff.StatModifier.ToList());
                    PlayerData.BuffStat.AddModfierList(buff.StatModifier.ToList());
                    _buffList.Remove(getbuff);
                    _buffList.Add(buff);
                }
              
                
            }
            else
            {
                PlayerData.BuffStat.AddModfierList(buff.StatModifier.ToList());
                _buffList.Add(buff);
            }
        }
        /*
        List<BaseItem> _buffList = GetInventory().FindAll(x => x is Buff);
        if (_buffList.Count == 0)
        {
            PlayerData.BuffStat.AddModfierList(buff.StatModifier.ToList());
        }
        else
        {
            if (_buffList.Where(x => (x as Buff).type == buff.type).Count() == 0)
            {
                PlayerData.BuffStat.AddModfierList(buff.StatModifier.ToList());
            }
            else
            {
                Buff _buff = _buffList.Where(x => (x as Buff).type == buff.type).First() as Buff;
                if (_buff.Level <= buff.Level)
                {
                    Debug.Log(_buff.Level);
                    Debug.Log(buff.Level);
                    PlayerData.BuffStat.RemoveModfierList(_buff.StatModifier.ToList());
                    Debug.Log(buff.ItemId + " : " + PlayerData.BuffStat.Get("Attack").Value);
                    PlayerData.BuffStat.AddModfierList(buff.StatModifier.ToList());
                    Debug.Log(buff.ItemId + " : " + PlayerData.BuffStat.Get("Attack").Value);


                }
                
            }
         
        }
        */


        
    }
    public void RemoveBuff(Buff buff)
    {
        if (_buffList.Contains(buff))
        {
            PlayerData.BuffStat.RemoveModfierList(buff.StatModifier.ToList());
            _buffList.Remove(buff);
        }
    }


    public void CompleteStage()
    {
        if(Managers.Scene.CurrentScene is MainScene)
        {
            PlayerData.ClearStage = PlayerData.Stage;

            if (PlayerData.MaxClearStage < PlayerData.ClearStage)
            {
                PlayerData.MaxClearStage = PlayerData.ClearStage;
                PlayerRank _searchRank = _playerdatabase.RankList.Where(x => x.Condition <= PlayerData.MaxClearStage).LastOrDefault();
                if (_searchRank.Level >  (PlayerRank.Rank)PlayerData.Level)
                    Levelup();
            }

            if (IsJumping)
            {
                int gainSPAmount = 0;
                int gainCoinAmount = 0;
                System.Numerics.BigInteger _gainGoldAmount = 0;
                for (int i = PlayerData.Stage; i < PlayerData.Stage + 1 + GetJumpingCount; i++)
                {
                    _gainGoldAmount += _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.Main).First().GetClearGold(i);
                    int SPGainRate = UnityEngine.Random.Range(0, 1000);
                    if (SPGainRate < 1)
                    {
                        gainSPAmount++;
                    }
                    int CoinGainRate = UnityEngine.Random.Range(0, 1000);
                    if (CoinGainRate < 1)
                    {
                        gainCoinAmount++;
                    }
                }
                if (gainSPAmount > 0)
                    AddCurrency(Define.CurrencyID.SP.ToString(), (System.Numerics.BigInteger)(gainSPAmount), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
                if (gainCoinAmount > 0)
                    AddCurrency(Define.CurrencyID.Coin.ToString(), (System.Numerics.BigInteger)(gainCoinAmount), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);

                AddCurrency(Define.CurrencyID.Gold.ToString(), _gainGoldAmount, IsApplyDropRate: true);
                PlayerData.Stage += 1 + GetJumpingCount;


            }
            else
            {
                AddCurrency(Define.CurrencyID.Gold.ToString(), _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.Main).First().GetClearGold(PlayerData.Stage), true);
                int SPGainRate = UnityEngine.Random.Range(0, 1000);
                if (SPGainRate < 1)
                    AddCurrency(Define.CurrencyID.SP.ToString(), 1 , IsUpdate : Managers.Network.IS_ENABLE_NETWORK);

                int CoinGainRate = UnityEngine.Random.Range(0, 1000);
                if (CoinGainRate < 1)
                    AddCurrency(Define.CurrencyID.Coin.ToString(), 1, IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
                PlayerData.Stage += 1;
            }


            PlayerData.CompleteStage();


        }
        else if(Managers.Scene.CurrentScene is TrollScene)
        {
            PlayerData.TrollStage++;
            PlayerData.CompleteStage();
            
        }
    }

    public void OnStateChangedHandler(Define.State changedState, GameObject gameobject)
    {
        if (changedState == Define.State.Death)
        {
            MonsterController _monster = gameobject.GetComponent<MonsterController>();
            Define.MonsterType type = _monster.MonsterType;
            Despawn(gameobject);
            PlayerData.KillMonster(type);

        }
    }
    public GameObject SpawnMonster(Define.Scene Scene , int StageLevel, Define.MonsterType monsterType , Transform parent = null)
    {

        StageTask _stagetask;
        if (Scene == Define.Scene.Main)
        {
            _stagetask = _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.Main).First();
            _stagetask.Spawn();
            return null;
        }
        else if (Scene == Define.Scene.GoldPig)
        {
            _stagetask = _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.GoldPig).First();
            _stagetask.Spawn();
            return null;
        }
        else if (Scene == Define.Scene.DarkMage)
        {
            _stagetask = _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.DarkMage).First();
            _stagetask.Spawn();
            return null;
        }
        else
        {

            _stagetask = _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.Troll).First();
            _stagetask.Spawn();
            return null;
        }

       
    }
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject SpawnObj = Managers.Resource.Instantiate(path, parent);
        switch (type)
        {
            case Define.WorldObject.Monster:
                Util.GetOrAddComponent<MonsterController>(SpawnObj);
                MonsterController _controller = Util.GetOrAddComponent<MonsterController>(SpawnObj);
                _controller.OnStateChanged -= OnStateChangedHandler;
                _controller.OnStateChanged += OnStateChangedHandler;
                _controller.OnDestroyed -= Despawn;
                _controller.OnDestroyed += Despawn;
              
                _monster.Add(SpawnObj);
                break;
            case Define.WorldObject.Pet:
                PetController _petcontroller = Util.GetOrAddComponent<PetController>(SpawnObj);
                _petcontroller.OnDestroyed -= Despawn;
                _petcontroller.OnDestroyed += Despawn;
                _objs.Add(SpawnObj);
                break;
            case Define.WorldObject.Arrow:
                PierceController _arrowcontroller = Util.GetOrAddComponent<PierceController>(SpawnObj);
                _arrowcontroller.OnDestroyed -= Despawn;
                _arrowcontroller.OnDestroyed += Despawn;
                _arrows.Add(SpawnObj);
                break;
            case Define.WorldObject.Unknown:
                _objs.Add(SpawnObj);
                break;
        }
        return SpawnObj;
    }
    public GameObject Spawn(Define.WorldObject type, GameObject obj, Transform parent = null)
    {
        GameObject SpawnObj = Managers.Resource.Instantiate(obj, parent);
        switch (type)
        {
            case Define.WorldObject.Monster:
                    Util.GetOrAddComponent<MonsterController>(SpawnObj);
                    MonsterController _controller = Util.GetOrAddComponent<MonsterController>(SpawnObj);
                    _controller.OnStateChanged -= OnStateChangedHandler;
                    _controller.OnStateChanged += OnStateChangedHandler;
                    _controller.OnDestroyed -= Despawn;
                    _controller.OnDestroyed += Despawn;
               
                    _monster.Add(SpawnObj);
                
                break;
            case Define.WorldObject.Pet:
                PetController _petcontroller = Util.GetOrAddComponent<PetController>(SpawnObj);
                _petcontroller.OnDestroyed -= Despawn;
                _petcontroller.OnDestroyed += Despawn;
                _objs.Add(SpawnObj);
                break;
            case Define.WorldObject.Arrow:
                PierceController _arrowcontroller = Util.GetOrAddComponent<PierceController>(SpawnObj);
                _arrowcontroller.OnDestroyed -= Despawn;
                _arrowcontroller.OnDestroyed += Despawn;
                _arrows.Add(SpawnObj);
                break;
            case Define.WorldObject.Unknown:
                _objs.Add(SpawnObj);
                break;

        }

        return SpawnObj;
    }
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return Define.WorldObject.Unknown;

        return bc.type;
    }
    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.Monster:
                if (_monster.Contains(go))
                {
                    _monster.Remove(go);
                    if (_hitList.Any(x => x.Target == go))
                    {
                        _hitPoolingList.AddRange(_hitList.FindAll(x => x.Target == go));
                        _hitList.RemoveAll(x => x.Target == go);
                    }
                    

                }

                //if(_hitQueue.Any(x=>x.Target.gameObject == go))
                    
                break;
            
            case Define.WorldObject.Pet:
                if (_objs.Contains(go))
                    _objs.Remove(go);
                break;
            case Define.WorldObject.Arrow:
                if (_arrows.Contains(go))
                    _arrows.Remove(go);
                break;
            case Define.WorldObject.Unknown:
                if (_objs.Contains(go))
                    _objs.Remove(go);
                break;
           
        }

        Managers.Resource.Destroy(go);



    }
    public void Despawn(Define.WorldObject type)
    {
        switch (type)
        {
            case Define.WorldObject.Monster:
                foreach (GameObject monster in _monster)
                {
                    if (_hitList.Any(x => x.Target == monster))
                    {
                        _hitPoolingList.AddRange(_hitList.FindAll(x => x.Target == monster));
                        _hitList.RemoveAll(x => x.Target == monster);
                    }
                    Managers.Resource.Destroy(monster);
                }
                _monster.Clear();
                break;
            case Define.WorldObject.Pet:
                break;
            case Define.WorldObject.Arrow:
                foreach (GameObject arrow in _arrows)
                    Managers.Resource.Destroy(arrow);
                
                _arrows.Clear();
                break;
            case Define.WorldObject.Unknown:
                break;
        }

    }

    public GameObject GetNextTarget()
    {
        if (_monster.Count == 0)
        {
            
            return null;
        }
        else
        {
            return _monster.First();
        }
       // return _monster.Count == 0 ? null : _monster.First();
    }
    public List<GameObject> GetTotalTarget()
    {

        return _monster;
    }
    /*
    public GameObject[] GetTotalTarget()
    {

        return _monster.Count == 0 ? null : _monster.ToArray();
    }
    */
    public GameObject GetLastTarget()
    {
        return _monster.Count == 0 ? null : _monster.Last();
    }
    public void Hit(MonsterController hit , Define.DamageType type, System.Numerics.BigInteger Damage)
    {

        if (hit.State == Define.State.Death)
            return;
       
        
        if (_hitPoolingList.Count <= 0)
        {
            _hitList.Add(new HitInfo { Target = hit, DamageType = type, Damage = Damage });
            Debug.Log("HitpollingList Count is 0");
        }
        else
        {
            HitInfo info = _hitPoolingList.First();
            
            info.Set(hit, type, Damage);
            _hitList.Add(info);
            _hitPoolingList.RemoveAt(0);
            //_hitPoolingList.Remove(info);
        }
        

    }

    public UnityEngine.Vector3 GetAutoTargetPos()
    {
        if(Managers.Scene.CurrentScene.SceneType == Define.Scene.Main)
        {
            return _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.Main).First().GetSpawnPosition(0);
        }
        else if(Managers.Scene.CurrentScene.SceneType == Define.Scene.GoldPig)
        {
            return _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.GoldPig).First().GetSpawnPosition(0);
        }
        else if (Managers.Scene.CurrentScene.SceneType == Define.Scene.DarkMage)
        {
            return _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.DarkMage).First().GetSpawnPosition(0);
        }
        else
            return _stagedatabse.StageList.Where(x => x.type == Define.Dongeon.Troll).First().GetSpawnPosition(0);
    }
    

    public void Clear()
    {
        //if (PlayerObject != null)
        // PlayerObject.SetActive(false);

        Managers.Scene.CurrentScene.CurrentState = BaseScene.GameState.Wait;

        _hitList.Clear();
        _hitPoolingList.Clear();

        Despawn(Define.WorldObject.Monster);
        Despawn(Define.WorldObject.Arrow);
       
        if(PlayerObject != null)
            PlayerObject.GetComponent<PlayerController>().ResetAnimation();
        //FindObjectOfType<PlayerController>().Idle();
    }
    public void ClearHitQueue()
    {
        _hitPoolingList.AddRange(_hitList);
        _hitList.Clear();
    }
    public void OnUpdate()
    {
        if(_hitList.Count > 0)
        {


            /*
            if (info.DamageType == Define.DamageType.Normal)
                info.Target.OnHit(info.Damage, Define.DamageType.Normal);
            else if (info.DamageType == Define.DamageType.Critical)
                info.Target.OnHit(info.Damage, Define.DamageType.Critical);
            else
            {
                if (Managers.Game.IsCriticalHit)
                    info.Target.OnHit(Managers.Game.CriticalHitDamage, Define.DamageType.Skill);
                else
                    info.Target.OnHit(Managers.Game.Damage, Define.DamageType.Skill);

            }

            */
            
            HitInfo info = _hitList.First();
            if (info.Target.State == Define.State.Death)
            {
                for (int i = 0; i < _hitList.Count; i++)
                {
                    if (_hitList[i].Target.gameObject == info.Target.gameObject)
                        _hitPoolingList.Add(_hitList[i]);
                }
                _hitList.RemoveAll(x => x.Target.gameObject == info.Target.gameObject);
                
            }
            else
            {
                _hitPoolingList.Add(info);
                _hitList.Remove(info);
                if(_monster.Contains(info.Target.gameObject))
                    info.Target.OnHit(info.Damage, info.DamageType);
            }
            
            




        }
    }
}
