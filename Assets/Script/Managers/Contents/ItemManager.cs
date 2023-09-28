using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using UnityEngine;

[Serializable]
public class ModifyItem
{
    public string ItemId;
    public int UsesToadds;
    public int CallsAPICount = 0;

    // 서버에서 호출되는 API 횟수 계산
    public void CalculateCallsAPICount(Inventory inven , string ItemId)
    {
        if (string.IsNullOrEmpty(ItemId))
        {
            CallsAPICount = 0;
            return;
        }
            

        if (inven.Find(x => x.ItemId == ItemId) == null)
            CallsAPICount += 2; // GrantItemsToUser , ModifyItemUses 2번 호출
        else
            CallsAPICount += 1; //ModifyItemUses 1번 호출

    }
}

[Serializable]
public class ModifyItemResponse
{
    public string ItemInstanceId;
    public int RemainingUses;
    public string ItemId;

}

public class ItemManager 
{
    [SerializeField]
    ItemDatabase _dataBase;

    
    public ItemDatabase Database => _dataBase;

    private Dictionary<string, Type> _itemInfo;
    private List<PlayFab.ClientModels.CatalogItem> _catalogItems;


    public void Init()
    {
        _dataBase = Managers.Resource.Load<ItemDatabase>("Database/ItemDatabase");
        
    }
    public void AddCatalogItems(List<PlayFab.ClientModels.CatalogItem> catalogItems)
    {
        if (_catalogItems == null)
            _catalogItems = catalogItems;
        else
            _catalogItems.AddRange(catalogItems);
    }
    public BaseItem LoadFrom(ItemSaveData itemSaveData)
    {
        BaseItem _loadItem = CreateItemInstance(itemSaveData.ItemId);
        _loadItem.Setup(itemSaveData);
        return _loadItem;
    }

    public BaseItem LoadFrom(ItemInstance itemData)
    {
        BaseItem _loadItem = CreateItemInstance(itemData.ItemId);
        _loadItem.Setup(itemData);
      
        return _loadItem;
    }

    public BaseItem CreateItemInstance(string ItemId)
    {
        BaseItem _findItem = _dataBase.ItemList.Where(x => x.ItemId == ItemId).FirstOrDefault();
        if (_findItem == null)
            throw new Exception($"ItemManager CreateItemInstance ItemId({ItemId}) not Found");
        
        return _findItem.Clone();
        
    }
    public bool UpgradeOneItem(BaseItem item, bool once = false)
    {
        if (item.ItemClass == "Rune")
        {
            string nexttarget = _dataBase.RuneUpgradeSystem.GetNextLevelItem(item);
            if (string.IsNullOrEmpty(nexttarget))
            {
                return false;
            }
          
            return _dataBase.RuneUpgradeSystem.UpgradeItem(Managers.Game.GetInventory(), nexttarget, once);
        }
        else if (item.ItemClass == "Pet")
        {
            string nexttarget = _dataBase.PetUpgradeSystem.GetNextLevelItem(item);
            Debug.Log("UpgradeOneItem : " + nexttarget);
            if (string.IsNullOrEmpty(nexttarget))
            {
                return false;
            }
           
            return _dataBase.PetUpgradeSystem.UpgradeItem(Managers.Game.GetInventory(), nexttarget, once);
        }
        else if (item.ItemClass == "Bow")
        {
            string nexttarget = _dataBase.BowUpgradeSystem.GetNextLevelItem(item);
            if (string.IsNullOrEmpty(nexttarget))
            {
                return false;
            }
            return _dataBase.BowUpgradeSystem.UpgradeItem(Managers.Game.GetInventory(), nexttarget, once);
        }
        else if (item.ItemClass == "Armor")
        {
            string nexttarget = _dataBase.ArmorUpgradeSystem.GetNextLevelItem(item);
            if (string.IsNullOrEmpty(nexttarget))
            {
                return false;
            }
            return _dataBase.ArmorUpgradeSystem.UpgradeItem(Managers.Game.GetInventory(), nexttarget, once);
        }
        else if (item.ItemClass == "Helmet")
        {
            string nexttarget = _dataBase.HelmetUpgradeSystem.GetNextLevelItem(item);
            if (string.IsNullOrEmpty(nexttarget))
            {
                return false;
            }
            return _dataBase.HelmetUpgradeSystem.UpgradeItem(Managers.Game.GetInventory(), nexttarget, once);
        }
        else if (item.ItemClass == "Cloak")
        {
            string nexttarget = _dataBase.CloakUpgradeSystem.GetNextLevelItem(item);
            if (string.IsNullOrEmpty(nexttarget))
            {
                return false;
            }
            return _dataBase.CloakUpgradeSystem.UpgradeItem(Managers.Game.GetInventory(), nexttarget, once);
        }
        else if (item.ItemClass == "Heart")
        {
            string nexttarget = _dataBase.ArtifactUpgradeSystem.GetNextLevelItem(item);
            if (string.IsNullOrEmpty(nexttarget))
            {
                return false;
            }
            return _dataBase.ArtifactUpgradeSystem.UpgradeItem(Managers.Game.GetInventory(), item.ItemId, once);
        }
        else if (item.ItemClass == "Essence")
        {
            string nexttarget = _dataBase.ArtifactUpgradeSystem.GetNextLevelItem(item);
            if (string.IsNullOrEmpty(nexttarget))
            {
                return false;
            }
            return _dataBase.ArtifactUpgradeSystem.UpgradeItem(Managers.Game.GetInventory(), item.ItemId, once);
        }
        else
            return false;
    }
    public List<BaseItem> GetConsumeItems(BaseItem targetItem)
    {
        if(targetItem.ItemClass == "Heart" || targetItem.ItemClass == "Essence")
        {
            return _dataBase.ArtifactUpgradeSystem.GetConsumeItem(targetItem);
        }
        return null;
    }
    public bool UpgradeAllItems(string ItemClass)
    {
        if (ItemClass == "Rune")
        {
            return _dataBase.RuneUpgradeSystem.UpgradeAllItems(Managers.Game.GetInventory());
        }
        else if (ItemClass == "Pet")
        {
            return _dataBase.PetUpgradeSystem.UpgradeAllItems(Managers.Game.GetInventory());
        }
        else if (ItemClass == "Bow")
        {
            return _dataBase.BowUpgradeSystem.UpgradeAllItems(Managers.Game.GetInventory());
        }
        else if (ItemClass == "Armor")
        {
            return _dataBase.ArmorUpgradeSystem.UpgradeAllItems(Managers.Game.GetInventory());
        }
        else if (ItemClass == "Helmet")
        {
            return _dataBase.HelmetUpgradeSystem.UpgradeAllItems(Managers.Game.GetInventory());
        }
        else if (ItemClass == "Cloak")
        {
            return _dataBase.CloakUpgradeSystem.UpgradeAllItems(Managers.Game.GetInventory());
        }
        else
            return false;
    }

    public bool CanSavableEquipment(string StrCurrentEquipment , string ItemClass )
    {
        if (ItemClass != "Rune" && ItemClass != "Pet" && 
            ItemClass != "Bow" && ItemClass != "Armor" && ItemClass != "Helmet" && ItemClass != "Cloak" && 
            ItemClass != "Essence" && ItemClass != "Heart")
        {
            return false;
        }
       
        string _getChecksum = JsonUtility.ToJson(Managers.Game.GetEquipment(ItemClass).ToSaveData());
        return StrCurrentEquipment != _getChecksum;
    }
    public UpgradeSystem GetUpgradeSystem(string ItemClass)
    {
        if (ItemClass == "Rune")
        {
            return _dataBase.RuneUpgradeSystem;
        }
        else if (ItemClass == "Pet")
        {
            return _dataBase.PetUpgradeSystem;
        }
        else if (ItemClass == "Bow")
        {
            return _dataBase.BowUpgradeSystem;
        }
        else if (ItemClass == "Armor")
        {
            return _dataBase.ArmorUpgradeSystem;
        }
        else if (ItemClass == "Helmet")
        {
            return _dataBase.HelmetUpgradeSystem;
        }
        else if (ItemClass == "Cloak")
        {
            return _dataBase.CloakUpgradeSystem;
        }
        else if (ItemClass == "Heart" || ItemClass == "Essence")
            return _dataBase.ArtifactUpgradeSystem;

        throw new Exception($"{ItemClass} UpgradeSystem is not founded");
    }
    public void OnItemUpgraded(List<BaseItem> consumes, BaseItem target)
    {

    }
    public List<BaseItem> UnLockItems(Mail mail)
    {
        List<BaseItem> _grantItems = new List<BaseItem>();
        Dictionary<string, int> _addedItem = new Dictionary<string, int>();

        
        if (mail.GrantedCurrencyList != null && mail.GrantedCurrencyList.Count > 0)
        {
            foreach (Collection<Currency> grantedCurrency in mail.GrantedCurrencyList)
            {
                Define.CurrencyID _currencyId = (Define.CurrencyID)grantedCurrency.Item.ID;
                Managers.Game.AddCurrency(_currencyId.ToString(), grantedCurrency.Count);

            }
        }
        if (mail.GrantedItemList != null && mail.GrantedItemList.Count > 0)
        {
            foreach (Collection<BaseItem> grantedItem in mail.GrantedItemList)
            {
                BaseItem item = grantedItem.Item;
                if (item is Table)
                {
                    for (int i = 0; i < grantedItem.Count; i++)
                    {
                        string id = ((Table)item).EvaluteTable();
                        if (!_addedItem.ContainsKey(id))
                            _addedItem.Add(id, 1);
                        else
                            _addedItem[id]++;
                    }
                }
                else
                {
                    for (int i = 0; i < grantedItem.Count; i++)
                    {
                        string id = item.ItemId;
                        if (!_addedItem.ContainsKey(id))
                            _addedItem.Add(id, 1);
                        else
                            _addedItem[id]++;
                    }
                }
            }
        }

       

        //Managers.Game.GetInventory().Find(x)
        List<BaseItem> _findAddedItem = new List<BaseItem>();
        foreach (KeyValuePair<string, int> added in _addedItem)
        {
            Managers.Game.GetInventory().AddItem(added.Key, added.Value);
            _findAddedItem.Add(Managers.Game.GetInventory().Find(x => x.ItemId == added.Key));
        }

        mail.CompleteUnLock();
        mail.Consume(1);

        return _findAddedItem;
     
    }
    public void UnLockItems(Mail mail, Action<List<BaseItem>> callback = null)
    {
     
        if (Managers.Network.IS_ENABLE_NETWORK == false)
            callback?.Invoke(UnLockItems(mail));
        else
        {
            
            // Ruby 를 주는 Mail 의 경우 미리 반영
            if (mail.GrantedCurrencyList != null && mail.GrantedCurrencyList.Count > 0)
            {
                foreach (Collection<Currency> currency in mail.GrantedCurrencyList)
                {
                    Define.CurrencyID _currencyId = (Define.CurrencyID)currency.Item.ID;
                    Managers.Game.AddCurrency(_currencyId.ToString(), currency.Count);
                }
            }
            
           

            Managers.Network.UnlockContainerItem(ItemInstanceId : mail.ItemInstanceId , KeyInstanceId: null ,  (result) => {
                PlayFab.ClientModels.UnlockContainerItemResult _result = result as PlayFab.ClientModels.UnlockContainerItemResult;
              
                List<BaseItem> _grantedItems = new List<BaseItem>();

                foreach (ItemInstance item in _result.GrantedItems)
                {
                    _grantedItems.AddRange(Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.GetValueOrDefault()));
                    _grantedItems.Where(x => x.ItemId == item.ItemId).FirstOrDefault().Setup(item);
                }

                if(_result.GrantedItems.Any(x=> x.ItemClass == "Rune" || x.ItemClass == "Pet" || 
                       x.ItemClass == "Bow" ||  x.ItemClass == "Armor" || x.ItemClass == "Helmet" || x.ItemClass == "Cloak" 
                    || x.ItemClass == "Heart"))
                {
                    if (_grantedItems.Count > 0)
                        Managers.UI.ShowPopupUI<UI_RandomboxPopup>().Setup(_grantedItems);
                }

               
             
                callback?.Invoke(_grantedItems);
            });
        }
    }

    


    public List<BaseItem> GrantItemToUser(string ItemId , Dictionary<string , string> CustomData = null)
    {
        Dictionary<string, int> _addedItem = new Dictionary<string, int>();
        Dictionary<string, System.Numerics.BigInteger> _addedCurrency = new Dictionary<string, System.Numerics.BigInteger>();

        BaseItem _info = Database.ItemList.Where(x => x.ItemId == ItemId).FirstOrDefault();
        if (_info == null)
            throw new Exception("GrantItemToUser ItemId not Found");

        if(_info is Bundle)
        {
            Bundle _bundle = _info as Bundle;
            if(_bundle.Content != null)
            {
                foreach (BundleItemContent content in _bundle.Content)
                {
                    if (content.Item is Table)
                    {
                        for (int i = 0; i < content.Amount; i++)
                        {
                            string id = ((Table)content.Item).EvaluteTable();
                            if (!_addedItem.ContainsKey(id)) 
                                _addedItem.Add(id, 1);
                            else
                                _addedItem[id]++;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < content.Amount; i++)
                        {
                            string id = content.Item.ItemId;
                            if (!_addedItem.ContainsKey(id))
                                _addedItem.Add(id, 1);
                            else
                                _addedItem[id]++;
                        }
                    }
                }
            }

            if (_bundle.Currencies != null && _bundle.Currencies.Count > 0)
            {
                foreach (BundleCurrencyContent content in _bundle.Currencies)
                {
                    if (_addedCurrency.ContainsKey(content.Currency.CodeName))
                        _addedCurrency[content.Currency.CodeName] += (System.Numerics.BigInteger)content.Amount;
                    else
                        _addedCurrency.Add(content.Currency.CodeName, (System.Numerics.BigInteger)content.Amount);
                }
                    
            }


            // 2023-03-15 일일 패키지 추가
            if (_bundle.DailyContents != null && _bundle.DailyContents.Count > 0)
            {
                foreach (BundleItemContent content in _bundle.DailyContents)
                {
                    if (content.Item is Table)
                    {
                        for (int i = 0; i < content.Amount; i++)
                        {
                            string id = ((Table)content.Item).EvaluteTable();
                            if (!_addedItem.ContainsKey(id))
                                _addedItem.Add(id, 1);
                            else
                                _addedItem[id]++;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < content.Amount; i++)
                        {
                            string id = content.Item.ItemId;
                            if (!_addedItem.ContainsKey(id))
                                _addedItem.Add(id, 1);
                            else
                                _addedItem[id]++;
                        }
                    }
                }
            }

            if (_bundle.DailyCurrencies != null && _bundle.DailyCurrencies.Count > 0)
            {
                foreach (BundleCurrencyContent content in _bundle.DailyCurrencies)
                {
                    if (_addedCurrency.ContainsKey(content.Currency.CodeName))
                        _addedCurrency[content.Currency.CodeName] += (System.Numerics.BigInteger)content.Amount;
                    else
                        _addedCurrency.Add(content.Currency.CodeName, (System.Numerics.BigInteger)content.Amount);
                }
             
            }
        }

        foreach(KeyValuePair<string , System.Numerics.BigInteger> pair in _addedCurrency)
            Managers.Game.AddCurrency(pair.Key, pair.Value);
        


        if (!_addedItem.ContainsKey(_info.ItemId))
            _addedItem.Add(_info.ItemId, 1);
        else
            _addedItem[_info.ItemId]++;

        List<BaseItem> _findAddedItem = new List<BaseItem>();
        foreach (KeyValuePair<string, int> added in _addedItem)
        {
            if(added.Key == ItemId)
                _findAddedItem.AddRange(Managers.Game.GetInventory().AddItem(added.Key, added.Value, CustomData));
            else
                _findAddedItem.AddRange(Managers.Game.GetInventory().AddItem(added.Key, added.Value));
        }
        return _findAddedItem;
    }
    
    public void EvaluateTable(Table _tableItem)
    {
        Dictionary<string, int> _genratedItem = new Dictionary<string, int>();

        if(_tableItem.Items.Any(x=>x.GetItem is Artifactpiece))
        {
            string id = _tableItem.EvaluteTable();

            string _itemId = _dataBase.FindArtifactByPiece(id);

            BaseItem _findItem =  Managers.Game.GetInventory().Find(x => x.ItemId == _itemId);
            BaseItem _findPiece = Managers.Game.GetInventory().Find(x => x.ItemId == id);

            int _maxLevel = (_findItem as Artifact).MaxLevel;

            int _currentAritfactLevel = _findItem == null ? 0 : _findItem.RemainingUses.Value;
            int _currentArtifactPieceCount = _findPiece == null ? 0 : _findPiece.RemainingUses.Value;



            // 1.해당 조각에 해당하는 Artifact 를 찾는다.

            // 2. Artifact 의 MaxLevel 을 가져온다.

            // 3. 현재 플레이어가 갖고있는 조각 갯수 + 현재 플레이어가 같고있는 유물레벨의 합 Artifact 의 MaxLevel 을 넘지 않아야 한다.



        }
        else
        {

        }

       
    }
    public void RefundArtifactPiece()
    {

        System.Numerics.BigInteger _amount = 0;
        foreach (Artifact Heart in Managers.Item.Database.HeartList.OrderBy(x => x.Type))
        {
            Artifact _find = Managers.Game.GetInventory().Find(x => x.ItemId == Heart.ItemId) as Artifact;
            if (_find != null)
            {
               
                if (_find.Level >= _find.MaxLevel)
                {
                    Artifactpiece _piece = Managers.Game.GetInventory().FindAll(x => x is Artifactpiece).Where(x => (x as Artifactpiece).Type == _find.Type).Where(x=>x.ItemClass == _find.ItemClass).FirstOrDefault() as Artifactpiece;
                  
                    if (_piece != null)
                        if (_piece.RemainingUses.HasValue == true)
                        {
                            _amount += (System.Numerics.BigInteger)(_piece.RemainingUses.GetValueOrDefault() * 900);

                            if (Managers.Network.IS_ENABLE_NETWORK == true)
                                Managers.Network.ConsumeItem(_piece.ItemInstanceId, _piece.RemainingUses.GetValueOrDefault());

                            _piece.Consume(_piece.RemainingUses.GetValueOrDefault());

                        }
                    //Managers.Item.Database.fp
                }
            }
        }

        if (_amount > 0)
            Managers.Game.AddCurrency(Define.CurrencyID.Ruby.ToString(), _amount , IsUpdate: Managers.Network.IS_ENABLE_NETWORK);


    }
    public int EncodingItemEquipment()
    {
        int _bow , _helmet , _armor , _cloak;
        int EncodedItem;

        EquipmentSystem BowEquipment = Managers.Game.GetEquipment("Bow");
        if (BowEquipment.SlotList[0].IsEquip == false)
            _bow = 255;
        else
        {
            Bow _equippedbow = BowEquipment.SlotList[0].GetItem as Bow;
            _bow = (int)_equippedbow.Level;
        }

        EquipmentSystem HelmetEquipment = Managers.Game.GetEquipment("Helmet");
        if (HelmetEquipment.SlotList[0].IsEquip == false)
            _helmet = 255;
        else
        {
            Helmet _equippedhelmet = HelmetEquipment.SlotList[0].GetItem as Helmet;
            _helmet = (int)_equippedhelmet.Level;
            Debug.Log("_helmet" + _helmet);
        }

        EquipmentSystem ArmorEquipment = Managers.Game.GetEquipment("Armor");
        if (ArmorEquipment.SlotList[0].IsEquip == false)
            _armor = 255;
        else
        {
            Armor _equippedarmor = ArmorEquipment.SlotList[0].GetItem as Armor;
            _armor = (int)_equippedarmor.Level;
        }

        EquipmentSystem CloakEquipment = Managers.Game.GetEquipment("Cloak");
        if (CloakEquipment.SlotList[0].IsEquip == false)
            _cloak = 255;
        else
        {
            Cloak _equippedcloak = CloakEquipment.SlotList[0].GetItem as Cloak;
            _cloak = (int)_equippedcloak.Level;
        }
     
        EncodedItem =  (_bow << 24) | (_helmet << 16) | (_armor << 8) | _cloak;
       
        return EncodedItem;

    }

    public string [] DecodingItemEquipment(int number)
    {
        string[] _itemIds = new string[] { string.Empty, string.Empty, string.Empty, string.Empty };
        if (number == -1)
            return _itemIds;

        int Empty = 255;

        int temp = number;

        int _bow = (temp >> 24);
        if (_bow < 0)
            _bow = Empty;

        int _helmet = (temp >> 16) & Empty;
        if (_helmet < 0)
            _helmet = Empty;

        int _armor = (temp >> 8) & Empty;
        if (_armor < 0)
            _armor = Empty;

        int _cloak = temp & Empty;
        if (_cloak < 0)
            _cloak = Empty;

       



        if (_bow != Empty)
        {
            EquipableItem.Rank _bowRank = (EquipableItem.Rank)_bow;
            _itemIds[0] = "Bow_" + _bowRank.ToString();
        }

        if(_helmet != Empty)
        {
            EquipableItem.Rank _helmetRank = (EquipableItem.Rank)_helmet;
            _itemIds[1] = "Helmet_" + _helmetRank.ToString();
        }

        if (_armor != Empty)
        {
            EquipableItem.Rank _armorRank = (EquipableItem.Rank)_armor;
            _itemIds[2] = "Armor_" + _armorRank.ToString();
        }

        if (_cloak != Empty)
        {
            EquipableItem.Rank _cloakRank = (EquipableItem.Rank)_cloak;
            _itemIds[3] = "Cloak_" + _cloakRank.ToString();
        }

        return _itemIds;


    }
    public void GiveDailyRewardToUser()
    {
        List<BaseItem> _bundles = Managers.Game.GetInventory().FindAll(x => x is Bundle);
        if (_bundles == null || _bundles.Count == 0)
            return;
        else if (_bundles.All(x => (x as Bundle).IsExpireDailyDate() == false))
            return;

        _bundles = _bundles.Where(x => (x as Bundle).IsExpireDailyDate() == true).ToList();

        List<PlayFab.ServerModels.ItemGrant> _grantDailyRewards = new List<PlayFab.ServerModels.ItemGrant>();


        foreach (Bundle bundle in _bundles)
        {
            _grantDailyRewards.AddRange(bundle.ToGrantDailyReward());
            bundle.UpdateDailyDate();
            Managers.Network.UpdateItemCustomData(bundle.ItemInstanceId, bundle.GetCustomData());
        }

        Managers.Network.GrantItems(_grantDailyRewards, (result) => {
            PlayFab.ServerModels.GrantItemsToUsersResult _result = result as PlayFab.ServerModels.GrantItemsToUsersResult;

            foreach (PlayFab.ServerModels.GrantedItemInstance item in _result.ItemGrantResults)
            {
                List<BaseItem> _grantItems = GrantItemToUser(item.ItemId);
                _grantItems.Where(x => x.ItemId == item.ItemId).FirstOrDefault().Setup(new PlayFab.ClientModels.ItemInstance
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

        
    }
    
    public Define.CouponResponse ConsumeCoupon(string CouponCode , GameData playerData)
    {
        if (_dataBase.CouponList.ContainsKey(CouponCode) == false)
            return Define.CouponResponse.NotExist;

        CouponCatalog _couponCatalog = _dataBase.CouponList[CouponCode];

        // 발급된 쿠폰인지를 먼저 확인후에 만료기한을 확인해야함.
       
        if (playerData.Inventory.IsFindItem(_couponCatalog.CouponeReceiveTicketId) == true)
        {
            return Define.CouponResponse.AlreadyIssue;
        }
        else if (playerData.Inventory.IsFindItem(_couponCatalog.CouponKeyId) == false && playerData.Inventory.IsFindItem(_couponCatalog.CouponContainerId) == true)
        {
            return Define.CouponResponse.Expired;
        }
        

        BaseItem _couponContainer = playerData.Inventory.Find(x => x.ItemId == _couponCatalog.CouponContainerId);
        BaseItem _couponKey = playerData.Inventory.Find(x => x.ItemId == _couponCatalog.CouponKeyId);


        List<BaseItem> _coupons = GrantItemToUser(_couponCatalog.CouponeReceiveTicketId);
        Managers.Network.UnlockContainerItem(ItemInstanceId: _couponContainer.ItemInstanceId, KeyInstanceId: _couponKey.ItemInstanceId ,
            (result)=> {
                PlayFab.ClientModels.UnlockContainerItemResult _result = result as PlayFab.ClientModels.UnlockContainerItemResult;
                foreach (ItemInstance item in _result.GrantedItems)
                    if (_coupons.Any(x => x.ItemId == item.ItemId))
                        _coupons.Where(x => x.ItemId == item.ItemId).First().Setup(item);
            } );

        _couponContainer.Consume(1);
        _couponKey.Consume(1);



        return Define.CouponResponse.Success;
    }

    public void GiveCouponToUser(string CouponCode, GameData playerData)
    {
        if (_dataBase.CouponList.ContainsKey(CouponCode) == false)
            return;

        playerData.Inventory.AddItem(_dataBase.CouponList[CouponCode].CouponContainerId, 1);
        List<BaseItem> _key = playerData.Inventory.AddItem(_dataBase.CouponList[CouponCode].CouponKeyId, 1);

        if(_dataBase.CouponList[CouponCode].TimePeriod != null)
        {
            TimeSpan _time = TimeSpan.FromSeconds(_dataBase.CouponList[CouponCode].TimePeriod.Value);
            _key.Where(x => x.ItemId == _dataBase.CouponList[CouponCode].CouponKeyId).First().SetExpiration(GlobalTime.Now + _time);
        }

        if(Managers.Network.IS_ENABLE_NETWORK == true)
        {
            List<PlayFab.ServerModels.ItemGrant> _grantlist = new List<PlayFab.ServerModels.ItemGrant>();

            _grantlist.Add(new PlayFab.ServerModels.ItemGrant { ItemId = _dataBase.CouponList[CouponCode].CouponKeyId, PlayFabId = Managers.Game.PlayerId });
            _grantlist.Add(new PlayFab.ServerModels.ItemGrant { ItemId = _dataBase.CouponList[CouponCode].CouponContainerId, PlayFabId = Managers.Game.PlayerId });

            Managers.Network.GrantItems(_grantlist, (result) => {

                PlayFab.ServerModels.GrantItemsToUsersResult _result = result as PlayFab.ServerModels.GrantItemsToUsersResult;
                foreach(PlayFab.ServerModels.GrantedItemInstance item in _result.ItemGrantResults)
                {
                    playerData.Inventory.Find(x => x.ItemId == item.ItemId).Setup(new PlayFab.ClientModels.ItemInstance
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
        }
       
    }


    public Define.CouponResponse GiveCouponToUser(string CouponCode)
    {
        if (_dataBase.ItemList.Where(x => x.ItemClass == "Coupon").Any(x => x.Description == CouponCode) == false)
            return Define.CouponResponse.NotExist;

        Inventory inven = Managers.Game.GetInventory();
        List<BaseItem> _coupons = inven.FindAll(x => x.ItemClass == "Coupon");

        if(_coupons != null)
        {
            if (_coupons.Where(x => x.Description == CouponCode).FirstOrDefault() != null)
                return Define.CouponResponse.AlreadyIssue;

        }


        BaseItem _getCoupon = _dataBase.ItemList.Where(x => x.ItemClass == "Coupon").Where(x => x.Description == CouponCode).FirstOrDefault();

        GrantItemToUser(_getCoupon.ItemId);
        Managers.Network.GrantItems(new List<PlayFab.ServerModels.ItemGrant>() { _getCoupon.ToGrantItem() });
        




        return Define.CouponResponse.Success;

    }






}
