using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System.Linq;
using UnityEngine.Purchasing.Security;

public class ShopManager : IStoreListener
{
    [SerializeField]
    ShopDatabase _dataBase;

    IStoreController m_StoreController = null;
    IExtensionProvider m_StoreExtensionProvider = null;
    
    public ShopDatabase Database => _dataBase;
    public readonly string UnityServiceName = "UnityService";

    

    public void Init()
    {
        _dataBase = Managers.Resource.Load<ShopDatabase>("Database/ShopDatabase");

        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            GameObject go = GameObject.Find(UnityServiceName);
            if (go == null)
            {
                go = new GameObject { name = UnityServiceName };
                go.AddComponent<InitializeUnityServices>().OnCallback += () => {
                    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

                    foreach (IAPData _data in _dataBase.IAPItems)
                        builder = builder.AddProduct(_data.Item.ItemId, ProductType.Consumable);

                    foreach (IAPData _data in _dataBase.RubyPackageItems)
                        builder = builder.AddProduct(_data.Item.ItemId, ProductType.Consumable);

                    builder = builder.AddProduct(_dataBase.Seasonpass.Item.ItemId, ProductType.Consumable);
                    builder = builder.AddProduct(_dataBase.Seasonpass2.Item.ItemId, ProductType.Consumable);
                    builder = builder.AddProduct(_dataBase.Seasonpass3.Item.ItemId, ProductType.Consumable);

                    UnityPurchasing.Initialize(this, builder);
                };
                Object.DontDestroyOnLoad(go);
            }
           
            //builder.AddProduct(_dataBase.Seasonpass.Item.ItemId, ProductType.Consumable);

            
            

            
        }
    }
    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"OnInitializeFailed() error " + error);
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"OnInitializeFailed() error " + message);
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(failureReason);
        return;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {

#if UNITY_ANDROID

        if (IsInitialized() == false)
        {
            return PurchaseProcessingResult.Complete;
        }
        if (purchaseEvent.purchasedProduct == null)
        {
            Debug.LogWarning("Attempted to process purchase with unknown product. Ignoring");
            return PurchaseProcessingResult.Complete;
        }
        // Test edge case where purchase has no receipt
        if (string.IsNullOrEmpty(purchaseEvent.purchasedProduct.receipt))
        {
            Debug.LogWarning("Attempted to process purchase with no receipt: ignoring");
            return PurchaseProcessingResult.Complete;
        }

        Debug.Log("Processing transaction: " + purchaseEvent.purchasedProduct.receipt);

        // Deserialize receipt
        var googleReceipt = GooglePurchase.FromJson(purchaseEvent.purchasedProduct.receipt);

        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        bool validPurchase = true;
        try
        {
            var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
            Debug.Log("Receipt is valid. Contents");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        }
        catch (IAPSecurityException)
        {
            Debug.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }

        Managers.UI.ShowPopupUI<UI_LoadingBlock>();

        if (Managers.Network.IS_ENABLE_NETWORK == true && validPurchase)
            Managers.Network.ValidatePurchase(purchaseEvent.purchasedProduct.metadata.isoCurrencyCode, (uint)(purchaseEvent.purchasedProduct.metadata.localizedPrice * 100), googleReceipt.PayloadData.json, googleReceipt.PayloadData.signature);


#endif
        //ProcessGiveToUser(purchaseEvent.purchasedProduct.definition.id);

        return PurchaseProcessingResult.Complete;
       
      
       
    }

    #region RandomBox
    public void BuyRandomBox(string id)
    {
        ShopDate _getdata = GetShopData(id);
        if (_getdata == null)
            return;
        ShopButtonData _data = _getdata.BtnLists.Where(x => x.PurchaseItem.ItemId == id).FirstOrDefault();
        if (Managers.Game.GetCurrency(_data.Currency) < _data.Price)
            return;



        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            Managers.UI.ShowPopupUI<UI_LoadingBlock>();
            if(_data.PurchaseItem.UnitPrice == 0 && Managers.Game.IsAdSkipped == false)
            {
                //Managers.Ad.ShowRewardVideoIronSource(()=> {
                    Managers.Network.PurchaseItem(_data.PurchaseItem.ItemId, Managers.Game.PlyaerDataBase.CurrencyDict[_data.Currency].ShortCodeName, _data.Price,
                    (result) => OnBuyRandomBox(result as PlayFab.ClientModels.PurchaseItemResult));

                //});
            }
            else
            {
                Managers.Network.PurchaseItem(_data.PurchaseItem.ItemId, Managers.Game.PlyaerDataBase.CurrencyDict[_data.Currency].ShortCodeName, _data.Price,
                (result) => OnBuyRandomBox(result as PlayFab.ClientModels.PurchaseItemResult));
            }
            
        }
        else
        {
            List<BaseItem> _grants = Managers.Item.GrantItemToUser(_data.PurchaseItem.ItemId);
            UI_RandomboxPopup _randomboxPopup = Object.FindObjectOfType<UI_RandomboxPopup>();
            if (_randomboxPopup != null)
            {
                _randomboxPopup.Setup(_grants, _getdata);
                _randomboxPopup.UpdatePopup();
            }
            else
            {
                _randomboxPopup = Managers.UI.ShowPopupUI<UI_RandomboxPopup>();
                _randomboxPopup.Setup(_grants, _getdata);

            }

            GambleQuestCount(_data.PurchaseItem.ItemId);
           
        }


        Managers.Game.SubstractCurrency(_data.Currency, _data.Price);

        
       
    }
    void OnBuyRandomBox(PlayFab.ClientModels.PurchaseItemResult result)
    {
        List<BaseItem> _grantItems = new List<BaseItem>();

        foreach (PlayFab.ClientModels.ItemInstance item in result.Items)
        {
            List<BaseItem> _addedItems = Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.GetValueOrDefault());
            _addedItems.ForEach(x => {
                if (x.ItemId == item.ItemId)
                {
                    x.Setup(item);
                    _grantItems.Add(x);
                }
            });
        }

        string randombox_id = result.Items.Where(x => Managers.Shop.GetShopData(x.ItemId) != null).First().ItemId;
        ShopDate _shopdata = Managers.Shop.GetShopData(randombox_id);
   

        GambleQuestCount(randombox_id);
        // Delete UI_LoadingBlock
        Managers.UI.ClosePopupUI();



        // 아이템 팝업을 띄운다.
        UI_RandomboxPopup _randomPopup = UnityEngine.Object.FindObjectOfType<UI_RandomboxPopup>();
        if (_randomPopup != null)
        {
            _randomPopup.Setup(_grantItems, _shopdata);
            _randomPopup.UpdatePopup();
        }
        else
        {
            _randomPopup = Managers.UI.ShowPopupUI<UI_RandomboxPopup>();
            _randomPopup.Setup(_grantItems, _shopdata);
        }
    }

    public ShopDate GetShopData(string Itemid)
    {
        return _dataBase.RandomBoxItems.Where(x => x.BtnLists.Any(y => y.PurchaseItem.ItemId == Itemid)).FirstOrDefault();
    }

    public string GetPrice(string ItemId)
    {
        if (IsInitialized())
        {
            if (m_StoreController.products.WithID(ItemId) != null)
                return m_StoreController.products.WithID(ItemId).metadata.localizedPriceString;

        }

        return string.Empty;

       
    }
    #endregion
    public void BuyProductId(string id)
    {
        
#if UNITY_EDITOR
        ProcessGiveToUser(id);
#elif UNITY_ANDROID
        
        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            if (IsInitialized() == false)
                throw new System.Exception("IAP Service is not initialized!");
            m_StoreController.InitiatePurchase(id);
        }
        else
            ProcessGiveToUser(id);
#endif



    }

    public void ProcessGiveToUser(string ItemId)
    {
        IAPData _iapData;
        // 루비 최초 구매 시 더블 보너스 적용
        if (_dataBase.RubyPackageItems.Any(x => x.Item.ItemId == ItemId))
        {
            _iapData = _dataBase.RubyPackageItems.Where(x => x.Item.ItemId == ItemId).First();
            int Amount = Util.GetIntFromString(ItemId);


#if UNITY_ANDROID    // Editor 의 경우 GrantItemsToUser 함수에서 Currency 가 반영되지만 안드로이드의 경우 iap 를 호출하므로 따로 Currency 가 반영되지 않는다.
            if (Managers.Game.GetInventory().Find(x => x.ItemId == ItemId).RemainingUses > 1)
                foreach (BundleCurrencyContent currency in _iapData.Item.Currencies)
                {
                    Managers.Game.AddCurrency(currency.Currency.CodeName, (System.Numerics.BigInteger)currency.Amount, IsUpdate: false);
                }
            else
            {
                foreach (BundleCurrencyContent currency in _iapData.Item.Currencies)
                {
                    string currencyCodeName = currency.Currency.CodeName;
                    System.Numerics.BigInteger currencyAmount = (System.Numerics.BigInteger)currency.Amount;

                    Managers.Game.AddCurrency(currencyCodeName, currencyAmount, IsUpdate: false);

                    Managers.Job.ReserveJob(System.TimeSpan.FromSeconds(0.2), () => {
                        Managers.Game.AddCurrency(currencyCodeName, currencyAmount, IsUpdate: Managers.Network.IS_ENABLE_NETWORK);

                    } );
                   
                }
            }
               
#elif UNITY_EDITOR
            if (Managers.Game.GetInventory().IsFindItem(ItemId) == false)
                Managers.Game.AddCurrency(Define.CurrencyID.Ruby.ToString(), (System.Numerics.BigInteger)Amount, IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
#endif



        }
        else if (_dataBase.IAPItems.Any(x => x.Item.ItemId == ItemId))
        {
            _iapData = _dataBase.IAPItems.Where(x => x.Item.ItemId == ItemId).First();
#if UNITY_EDITOR
#elif UNITY_ANDROID   // Editor 의 경우 GrantItemsToUser 함수에서 Currency 가 반영되지만 안드로이드의 경우 iap 를 호출하므로 따로 Currency 가 반영되지 않는다.
            foreach (BundleCurrencyContent currency in _iapData.Item.Currencies)
                Managers.Game.AddCurrency(currency.Currency.CodeName, (System.Numerics.BigInteger)currency.Amount, IsUpdate: false);
#endif
            // 장비 , 룬 , 펫 패키지 구매 시 확인창
            if (_iapData.type == IAPData.Type.Package && _iapData.ItemIcons.Count != 0)
            {
                if(_iapData.Item.Content.Any(x=>x.Item.ItemClass == "Rune" || x.Item.ItemClass == "Pet"))
                {
                    UI_IAPConfirm _popup = Managers.UI.ShowPopupUI<UI_IAPConfirm>();
                    _popup.TitleText = _iapData.DisplayName + " 구매 완료";
                    _popup.DescriptionText = "룬,펫창으로 이동하여 룬,펫을 장착해 주세요";
                    _popup.SetIAPData(_iapData);
                }
                else
                {
                    UI_IAPConfirm _popup = Managers.UI.ShowPopupUI<UI_IAPConfirm>();
                    _popup.TitleText = _iapData.DisplayName + " 구매 완료";
                    _popup.DescriptionText = "장비창으로 이동하여 장비를 장착해 주세요";
                    _popup.SetIAPData(_iapData);
                }
               
            }
        }
        else if (_dataBase.Seasonpass.Item.ItemId == ItemId)
        {
            _iapData = _dataBase.Seasonpass;
            UnityEngine.Object.FindObjectOfType<GameData>().PurchaseItemId(ItemId, 1);

            UI_Seasonpass _seasonpass = Object.FindObjectOfType<UI_Seasonpass>();
            if(_seasonpass != null)
            {
                _seasonpass.SetActivePurchasePanel(false);
            }
           
        }
        else if (_dataBase.Seasonpass2.Item.ItemId == ItemId)
        {
            _iapData = _dataBase.Seasonpass2;
            UnityEngine.Object.FindObjectOfType<GameData>().PurchaseItemId(ItemId, 1);

            UI_Seasonpass _seasonpass = Object.FindObjectOfType<UI_Seasonpass>();
            if (_seasonpass != null)
            {
                _seasonpass.SetActivePurchasePanel(false);
            }
        }
        else if (_dataBase.Seasonpass3.Item.ItemId == ItemId)
        {
            _iapData = _dataBase.Seasonpass3;
            UnityEngine.Object.FindObjectOfType<GameData>().PurchaseItemId(ItemId, 1);

            UI_Seasonpass _seasonpass = Object.FindObjectOfType<UI_Seasonpass>();
            if (_seasonpass != null)
            {
                _seasonpass.SetActivePurchasePanel(false);
            }
        }
        else
            throw new System.Exception("Find not ItemId : " + ItemId);



        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
#if UNITY_EDITOR
            Managers.Network.GrantItems(new List<PlayFab.ServerModels.ItemGrant>() { _iapData.Item.ToGrantItem() },
                (result) =>
                {
                    List<BaseItem> _giveItems = Managers.Item.GrantItemToUser(ItemId);

                    PlayFab.ServerModels.GrantItemsToUsersResult _result = result as PlayFab.ServerModels.GrantItemsToUsersResult;
                    foreach (PlayFab.ServerModels.GrantedItemInstance item in _result.ItemGrantResults)
                    {
                       
                        _giveItems.Where(x => x.ItemId == item.ItemId).FirstOrDefault().Setup(new PlayFab.ClientModels.ItemInstance
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

                        /*
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
                        */

                    }
                });
#elif UNITY_ANDROID
            if(_iapData.Item.DailyContents.Count > 0)
            {
                Bundle _getDailyBundle = Managers.Game.GetInventory().Find(x => x.ItemId == _iapData.Item.ItemId) as Bundle;
                _getDailyBundle.UpdateDailyDate();
                Managers.Network.UpdateItemCustomData(_getDailyBundle.ItemInstanceId, _getDailyBundle.GetCustomData());
            }
#endif
        }


    }

    // Gamble Quest 를 위함. 아직 깨지 않은 다음 레벨의 퀘스트가 Receive 하지 않도록 1씩 Count 한다.
    private void GambleQuestCount(string ItemId)
    {
        ShopDate _getdata = GetShopData(ItemId);
        if (_getdata == null)
            return;
        ShopButtonData _data = _getdata.BtnLists.Where(x => x.PurchaseItem.ItemId == ItemId).FirstOrDefault();
        if (_data == null)
            return;

        int Count = Util.GetIntFromString(_data.PurchaseItem.ItemId);
        GameData _playerData = Object.FindObjectOfType<GameData>();
        for (int i = 0; i < Count; i++)
            _playerData.PurchaseItemClass(_getdata.ItemClass, 1, _data.Price == 0 ? true : false);

    }

    
}





#region UnityService

public class InitializeUnityServices : MonoBehaviour
{
    public string environment = "production";
    public System.Action OnCallback = null;

    async void Awake()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
            OnCallback?.Invoke();
        }
        catch (System.Exception exception)
        {

        }
    }
}
#endregion

#region IAP Service Json Data
public class JsonData
{
    // JSON Fields, ! Case-sensitive

    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public string purchaseToken;
}

public class PayloadData
{
    public JsonData JsonData;

    // JSON Fields, ! Case-sensitive
    public string signature;
    public string json;

    public static PayloadData FromJson(string json)
    {
        var payload = JsonUtility.FromJson<PayloadData>(json);
        payload.JsonData = JsonUtility.FromJson<JsonData>(payload.json);
        return payload;
    }
}


public class GooglePurchase
{
    public PayloadData PayloadData;

    // JSON Fields, ! Case-sensitive
    public string Store;
    public string TransactionID;
    public string Payload;

    public static GooglePurchase FromJson(string json)
    {
        var purchase = JsonUtility.FromJson<GooglePurchase>(json);
        purchase.PayloadData = PayloadData.FromJson(purchase.Payload);
        return purchase;
    }
}
#endregion
