using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

[System.Serializable]
class IntervalUnit<T>
{
    public int min;
    public int max;
    public T Unit;
    public bool IsContains(int n)
    {
        if (n >= min && n <= max)
            return true;
        else
            return false;
    }
    
}

[System.Serializable]
public class Collection<T>
{
    public int Count;
    public T Item;
   

}

[CreateAssetMenu(menuName =("스테이지 테스크"))]
public class StageTask : ScriptableObject
{
    public class StageReward
    {
        public BigInteger Gold;
        public List<BaseItem> rewards;
    }

    public enum Mode
    {
        Select,
        AutoRetry
    }
   

    [SerializeField]
    Define.Dongeon _type;
    public Define.Dongeon type => _type;

    [SerializeField]
    Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField]
    Sprite _NameBackground;
    public Sprite NameBackground => _NameBackground;

    [SerializeField]
    Sprite _subDescriptionBackground;

    public Sprite SubDescriptionBackground => _subDescriptionBackground;


    [SerializeField]
    string _name;
    public string Name => _name;

    [SerializeField]
    string _description;
    public string Description => _description;

    [SerializeField]
    Collection<BaseItem> _EnterCondition;
    public Collection<BaseItem> EnterCondition => _EnterCondition;

    [Header("일일무료입장 횟수")]
    [SerializeField]
    int _Count_Daily_Free_Pass; // 일일 무료 입장횟수   -1 무한
    public int DailyFreePassCount => _Count_Daily_Free_Pass;

    [Header("최고스테이지")]
    [SerializeField]
    int _maxStage;
    public int MaxStage => _maxStage;

    public bool IsMaxStage(GameData player)
    {
        if (type == Define.Dongeon.DarkMage)
            return false;
        else if(type == Define.Dongeon.GoldPig)
        {
            return false;
        }
        else if(type == Define.Dongeon.Troll)
        {
            return (player.TrollStage + 1) > _maxStage ? true : false;
        }
        else 
        {
            return false;
        }
       
    }



    [SerializeField]
    MonsterStatCalculator _monsterHPGenerator;

    [SerializeField]
    int _Timeout;
    public int GetTimeout => _Timeout;

    [SerializeField]
    List<IntervalUnit<GameObject>> _mosterPrefabs;

    [SerializeField]
    List<IntervalUnit<GameObject>> _backgroundPrefabs;


    [SerializeField]
    List<BaseItem> _rewardList;

    [SerializeField]
    Spawner _spawner;

    [SerializeField]
    public Mode CurrentMode { get; set; } = Mode.Select;

   
   


    public bool CanEnter(GameData playerData)
    {
        if(_EnterCondition.Count > 0)
        {
            // 일일 무료 입장 횟수 체크 후
            StageData _findData = playerData.StageInfoList.List.Where(x => x.type == type.ToString()).FirstOrDefault();
        
            if (_findData.FreePassCount > 0)
                return true;
            else
            {
                BaseItem _key = playerData.Inventory.Find(x => x.ItemId == _EnterCondition.Item.ItemId);
                if (_key != null)
                    return _key.RemainingUses.GetValueOrDefault() > 0 ? true : false;
                else
                    return false;
            }
        }
        else
        {   // 무한도전 가능
            return true;
        }

        
    }
    public void Enter(GameData playerData, bool loadScene = true)
    {
        if(CanEnter(playerData))
        {
            if(_EnterCondition.Count > 0)
            {
                StageData _findData = playerData.StageInfoList.List.Where(x => x.type == type.ToString()).FirstOrDefault();
                if (_findData.FreePassCount > 0)
                {
                    _findData.FreePassCount--;
                    _findData.time = Util.GetTimeString(GlobalTime.Now);
                    Managers.Game.Save(PlayerInfo.UserDataKey.StageInfo);
                }
                else
                {
                    BaseItem _key = playerData.Inventory.Find(x => x.ItemId == _EnterCondition.Item.ItemId);
                    string KeyInstanceId = _key.ItemInstanceId;

                    _key.Consume(1);
                    Managers.Network.ConsumeItem(KeyInstanceId);
                }
            }

            if (loadScene)
            {
                Define.Scene _findScene = (Define.Scene)System.Enum.Parse(typeof(Define.Scene), _type.ToString());
                CurrentMode = Mode.Select;
                Managers.Scene.LoadScene(_findScene);
            }
           


        }
        
    }
    public StageData CreateStageData()
    {
        StageData stage = new StageData { type = type.ToString(), FreePassCount = _Count_Daily_Free_Pass , time = Util.GetTimeString(GlobalTime.Now)};
        return stage;
    }

    public BigInteger GetMonsterHP(int targetStage)
    {
        return _spawner.GetMonsterHP(targetStage);
    }

    public GameObject GetMonsterObject(int targetStage , Transform parent = null)
    {
        if (Managers.Scene.CurrentScene is DarkMageScene)
        {
            int index = UnityEngine.Random.Range(0, _mosterPrefabs.Count);
            return _mosterPrefabs[index].Unit;
        }
        else if(Managers.Scene.CurrentScene is TrollScene)
        {
            int prefabIndex = targetStage - 1;
            prefabIndex %= _mosterPrefabs.Count;
            return _mosterPrefabs[prefabIndex].Unit;
        }
        else
        {
            foreach (IntervalUnit<GameObject> _inverval in _mosterPrefabs)
            {
                if (_inverval.IsContains(targetStage))
                {
                    return _inverval.Unit;
                }
            }
            return _mosterPrefabs[0].Unit;
        }
    }
    public GameObject GetBackground(int targetStage, Transform parent = null)
    {
        return Managers.Resource.Instantiate(_backgroundPrefabs[0].Unit, parent);
    }
    public virtual void ClearStage()
    {

    }
    public void Spawn()
    {
        _spawner.DoSpawn();
    }
    public BigInteger GetClearGold(int stage)
    {
        return _spawner.GetClearGold(stage);
    }
    public UnityEngine.Vector3 GetSpawnPosition(int index)
    {
        return _spawner.GetSpawnPosition(index);
    }
    public void ProcessReward(System.Action<StageReward> OnProcessComplete)
    {
        if(Managers.Scene.CurrentScene is DarkMageScene)
        {
            StageReward _reward = new StageReward();

            GameObject monster = Managers.Game.GetNextTarget();
            MonsterController _controller = monster.GetComponent<MonsterController>();
            BigInteger _damageMeter = BigInteger.Abs(_controller.GetHp());

            if(_damageMeter >= BigInteger.Parse("5000000000000000000000000000"))  // S   // 5000자 
            {
                if (Managers.Network.IS_ENABLE_NETWORK == true)
                {
                    Managers.Network.PurchaseItem(_rewardList[0].ItemId, Managers.Game.PlyaerDataBase.CurrencyDict[Define.CurrencyID.Ruby.ToString()].ShortCodeName, 0, (result) =>
                    {
                        PlayFab.ClientModels.PurchaseItemResult _getResult = result as PlayFab.ClientModels.PurchaseItemResult;
                        List<BaseItem> _grantedItems = new List<BaseItem>();
                        foreach (PlayFab.ClientModels.ItemInstance item in _getResult.Items)
                        {
                            _grantedItems.AddRange(Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.Value));
                            _grantedItems.Last().Setup(item);
                        }

                        _reward.rewards = _grantedItems;
                        _reward.rewards.RemoveAll(x => x is Bundle);
                        OnProcessComplete?.Invoke(_reward);
                    });
                }
                else
                {
                    _reward.rewards = Managers.Item.GrantItemToUser(_rewardList[0].ItemId);
                    _reward.rewards.RemoveAll(x => x is Bundle);
                    OnProcessComplete?.Invoke(_reward);
                }
            }
            else if(_damageMeter >= BigInteger.Parse("500000000000000000000000")) // A   5000해 500000000000000000000
            {
                if (Managers.Network.IS_ENABLE_NETWORK == true)
                {
                    Managers.Network.PurchaseItem(_rewardList[1].ItemId, Managers.Game.PlyaerDataBase.CurrencyDict[Define.CurrencyID.Ruby.ToString()].ShortCodeName, 0, (result) =>
                    {
                        PlayFab.ClientModels.PurchaseItemResult _getResult = result as PlayFab.ClientModels.PurchaseItemResult;
                        List<BaseItem> _grantedItems = new List<BaseItem>();
                        foreach (PlayFab.ClientModels.ItemInstance item in _getResult.Items)
                        {
                            _grantedItems.AddRange(Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.Value));
                            _grantedItems.Last().Setup(item);
                        }

                        _reward.rewards = _grantedItems;
                        _reward.rewards.RemoveAll(x => x is Bundle);
                        OnProcessComplete?.Invoke(_reward);
                    });
                }
                else
                {
                    _reward.rewards.RemoveAll(x => x is Bundle);
                    OnProcessComplete?.Invoke(_reward);
                    _reward.rewards = Managers.Item.GrantItemToUser(_rewardList[1].ItemId);
                }
            }
            else if(_damageMeter >= BigInteger.Parse("50000000000000000000")) // B     5000경
            {
                if (Managers.Network.IS_ENABLE_NETWORK == true)
                {
                    Managers.Network.PurchaseItem(_rewardList[2].ItemId, Managers.Game.PlyaerDataBase.CurrencyDict[Define.CurrencyID.Ruby.ToString()].ShortCodeName, 0, (result) =>
                    {
                        PlayFab.ClientModels.PurchaseItemResult _getResult = result as PlayFab.ClientModels.PurchaseItemResult;
                        List<BaseItem> _grantedItems = new List<BaseItem>();
                        foreach (PlayFab.ClientModels.ItemInstance item in _getResult.Items)
                        {
                            _grantedItems.AddRange(Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.Value));
                            _grantedItems.Last().Setup(item);
                        }

                        _reward.rewards = _grantedItems;
                        _reward.rewards.RemoveAll(x => x is Bundle);
                        OnProcessComplete?.Invoke(_reward);
                    });
                }
                else
                {
                    _reward.rewards.RemoveAll(x => x is Bundle);
                    OnProcessComplete?.Invoke(_reward);
                    _reward.rewards = Managers.Item.GrantItemToUser(_rewardList[2].ItemId);
                }
            }
            else if (_damageMeter >= BigInteger.Parse("5000000000000000"))  // C   5000조
            {
                if (Managers.Network.IS_ENABLE_NETWORK == true)
                {
                    Managers.Network.PurchaseItem(_rewardList[3].ItemId, Managers.Game.PlyaerDataBase.CurrencyDict[Define.CurrencyID.Ruby.ToString()].ShortCodeName, 0, (result) =>
                    {
                        PlayFab.ClientModels.PurchaseItemResult _getResult = result as PlayFab.ClientModels.PurchaseItemResult;
                        List<BaseItem> _grantedItems = new List<BaseItem>();
                        foreach (PlayFab.ClientModels.ItemInstance item in _getResult.Items)
                        {
                            _grantedItems.AddRange(Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.Value));
                            _grantedItems.Last().Setup(item);
                        }

                        _reward.rewards = _grantedItems;
                        _reward.rewards.RemoveAll(x => x is Bundle);
                        OnProcessComplete?.Invoke(_reward);
                    });
                }
                else
                {
                    _reward.rewards.RemoveAll(x => x is Bundle);
                    OnProcessComplete?.Invoke(_reward);
                    _reward.rewards = Managers.Item.GrantItemToUser(_rewardList[3].ItemId);
                }

            }
            else if (_damageMeter >= BigInteger.Parse("50000000"))  // D     5천만
            {
                if (Managers.Network.IS_ENABLE_NETWORK == true)
                {
                    Managers.Network.PurchaseItem(_rewardList[4].ItemId, Managers.Game.PlyaerDataBase.CurrencyDict[Define.CurrencyID.Ruby.ToString()].ShortCodeName, 0, (result) =>
                    {
                        PlayFab.ClientModels.PurchaseItemResult _getResult = result as PlayFab.ClientModels.PurchaseItemResult;
                        List<BaseItem> _grantedItems = new List<BaseItem>();
                        foreach (PlayFab.ClientModels.ItemInstance item in _getResult.Items)
                        {
                            _grantedItems.AddRange(Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.Value));
                            _grantedItems.Last().Setup(item);
                        }

                        _reward.rewards = _grantedItems;
                        _reward.rewards.RemoveAll(x => x is Bundle);
                        OnProcessComplete?.Invoke(_reward);
                    });
                }
                else
                {
                    _reward.rewards = Managers.Item.GrantItemToUser(_rewardList[4].ItemId);
                    _reward.rewards.RemoveAll(x => x is Bundle);
                    OnProcessComplete?.Invoke(_reward);
                }
            }
            else if (_damageMeter >= BigInteger.Parse("5000000"))  // E
            {
                if (Managers.Network.IS_ENABLE_NETWORK == true)
                {
                    Managers.Network.PurchaseItem(_rewardList[5].ItemId, Managers.Game.PlyaerDataBase.CurrencyDict[Define.CurrencyID.Ruby.ToString()].ShortCodeName, 0, (result) =>
                    {
                        PlayFab.ClientModels.PurchaseItemResult _getResult = result as PlayFab.ClientModels.PurchaseItemResult;
                        List<BaseItem> _grantedItems = new List<BaseItem>();
                        foreach (PlayFab.ClientModels.ItemInstance item in _getResult.Items)
                        {
                            _grantedItems.AddRange(Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.Value));
                            _grantedItems.Last().Setup(item);
                        }

                        _reward.rewards = _grantedItems;
                        _reward.rewards.RemoveAll(x => x is Bundle);
                        OnProcessComplete?.Invoke(_reward);
                    });
                }
                else
                {
                    _reward.rewards = Managers.Item.GrantItemToUser(_rewardList[5].ItemId);
                    _reward.rewards.RemoveAll(x => x is Bundle);
                    OnProcessComplete?.Invoke(_reward);
                }
            }
            else // 500 만 미만 F급 수령
            {
                if (Managers.Network.IS_ENABLE_NETWORK == true)
                {
                    Managers.Network.PurchaseItem(_rewardList[6].ItemId, Managers.Game.PlyaerDataBase.CurrencyDict[Define.CurrencyID.Ruby.ToString()].ShortCodeName, 0, (result) =>
                    {
                        PlayFab.ClientModels.PurchaseItemResult _getResult = result as PlayFab.ClientModels.PurchaseItemResult;
                        List<BaseItem> _grantedItems = new List<BaseItem>();
                        foreach (PlayFab.ClientModels.ItemInstance item in _getResult.Items)
                        {
                            _grantedItems.AddRange(Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.Value));
                            _grantedItems.Last().Setup(item);
                        }

                        _reward.rewards = _grantedItems;
                        _reward.rewards.RemoveAll(x => x is Bundle);
                        OnProcessComplete?.Invoke(_reward);
                    });
                }
                else
                {
                    _reward.rewards = Managers.Item.GrantItemToUser(_rewardList[6].ItemId);
                    _reward.rewards.RemoveAll(x => x is Bundle);
                    OnProcessComplete?.Invoke(_reward);
                }
            }
            

            
        }
        else if(Managers.Scene.CurrentScene is TrollScene)
        {
            StageReward _reward = new StageReward();

            int ClearStage = Managers.Game.TrollStage;  // 방금 클리어한 스테이지 
            Debug.Log(ClearStage);
            int itemindex = (ClearStage - 1) % _rewardList.Count;

            _reward.rewards = Managers.Item.GrantItemToUser(_rewardList[itemindex].ItemId);

            if(Managers.Network.IS_ENABLE_NETWORK == true)
            {
                Managers.Network.GrantItems(_reward.rewards.Select(x => x.ToGrantItem()).ToList() , (grantResult)=> {
                    OnProcessComplete?.Invoke(_reward);
                });
                Managers.Game.Save(PlayerInfo.StatisticsDataKey.TrollStage);
                Managers.Game.Save(PlayerInfo.UserDataKey.MainQuest);
            }
            else
                OnProcessComplete?.Invoke(_reward);

           
        }
        else if(Managers.Scene.CurrentScene is GoldPigScene)
        {
            StageReward _reward = new StageReward();
            BigInteger _clearCount = 120 ;

            BigInteger _baseMoney;
        
            if(Managers.Game.GetJumpingRate > 0)
            {
                for (int i = 0; i < 1 + Managers.Game.GetJumpingCount; i++)
                    _baseMoney += GetMonsterHP(Managers.Game.Stage);

                _baseMoney *= (BigInteger)Managers.Game.GetJumpingRate;
                _baseMoney /= 100;
            }
            else
                _baseMoney = GetMonsterHP(Managers.Game.Stage);
            

            _baseMoney *= _clearCount;

            _reward.Gold = Managers.Game.CalculateDropRateAmount(Define.CurrencyID.Gold , _baseMoney);
            _reward.rewards = null;

          
            Managers.Game.AddCurrency(Define.CurrencyID.Gold.ToString(), _reward.Gold , IsApplyDropRate: false, IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
            OnProcessComplete?.Invoke(_reward);
            

        }
       
    }
}
