using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using PlayFab.SharedModels;
using System.Linq;

public class PacketHandler 
{
    private static Dictionary<NetworkManager.Command, Action<PlayFabResultCommon>> _handlers;
    private static Dictionary<NetworkManager.Command, Queue<Action<PlayFabResultCommon>>> _customHandlers;

    private static bool IsExistCustomHandler(NetworkManager.Command command)
    {
        if (_customHandlers.ContainsKey(command))
        {
            if (_customHandlers[command].Count > 0)
                return true;
        }

        return false;
    }


    public static void RegisterHandler(NetworkManager.Command command , Action<PlayFabResultCommon> handler)
    {
        if (_customHandlers[command] == null)
            _customHandlers[command] = new Queue<Action<PlayFabResultCommon>>();
        
        _customHandlers[command].Enqueue(handler);
    }
    
    public static void OnRegister(PlayFabResultCommon result)
    {
        LoginResult _loginResult = result as LoginResult;
        
        Managers.Game.PlayerId = _loginResult.PlayFabId;
        Managers.Player.AddPlayer(Managers.Game.PlayerId, new PlayerInfo());
        Managers.Player.GetPlayer(Managers.Game.PlayerId).PlayfabId = Managers.Game.PlayerId;
        Managers.Player.GetPlayer(Managers.Game.PlayerId).LoginResult = _loginResult;

        Managers.Network.GetServerTime();
       
        
    }
    public static void OnLogin(PlayFabResultCommon result)
    {
        LoginResult _loginResult = result as LoginResult;
        Managers.Game.PlayerId = _loginResult.PlayFabId;
        Managers.Player.AddPlayer(Managers.Game.PlayerId, new PlayerInfo());
        Managers.Player.GetPlayer(Managers.Game.PlayerId).PlayfabId = Managers.Game.PlayerId;
        Managers.Player.GetPlayer(Managers.Game.PlayerId).LoginResult = _loginResult;

        Managers.Network.GetServerTime();

    }
    public static void OnUpdateEntityToken(PlayFabResultCommon result)
    {
        LoginResult _loginResult = result as LoginResult;

        if(Managers.Player.GetPlayer(_loginResult.PlayFabId) != null)
            Managers.Player.GetPlayer(_loginResult.PlayFabId).LoginResult = _loginResult;

        if (IsExistCustomHandler(NetworkManager.Command.UpdateEntityToken))
            _customHandlers[NetworkManager.Command.UpdateEntityToken].Dequeue()?.Invoke(result);


    }
    public static void OnUpdateDisplayName(PlayFabResultCommon result)
    {
        UpdateUserTitleDisplayNameResult _result = result as UpdateUserTitleDisplayNameResult;

        Managers.Player.GetPlayer(Managers.Game.PlayerId).DisplayName = _result.DisplayName;

        // 첫 가입이므로 바로 플레이어 정보를 다운로드 한다.
        Managers.Network.GetPlayerInfo();
       

        //Managers.UI.ShowPopupUI<UI_Loading>();

    }
    public static void OnGetTime(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.GetTimeResult _getTimeResult = result as PlayFab.ClientModels.GetTimeResult;


        GlobalTime.SetSyncTime(_getTimeResult.Time);

        Managers.Network.GetCatalogItems();

      

    }

    public static void OnUpdateUserData(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.UpdateUserDataResult _userdata = result as PlayFab.ClientModels.UpdateUserDataResult;
        PlayFab.ClientModels.UpdateUserDataRequest _requestData = _userdata.Request as PlayFab.ClientModels.UpdateUserDataRequest;

    
        foreach (KeyValuePair<string,string> pair in _requestData.Data)
        {
            if(pair.Key == PlayerInfo.UserDataKey.Gold.ToString())
            {
                Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserData[PlayerInfo.UserDataKey.Gold.ToString()].Value = _requestData.Data[PlayerInfo.UserDataKey.Gold.ToString()];
            }
            else if(pair.Key == PlayerInfo.UserDataKey.CP.ToString())
            {
                Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserData[PlayerInfo.UserDataKey.CP.ToString()].Value = _requestData.Data[PlayerInfo.UserDataKey.CP.ToString()];
            }
        }

        

    }

    
    public static void OnGetPlayerInfo(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.GetPlayerCombinedInfoResult _getPlayerInfo = result as PlayFab.ClientModels.GetPlayerCombinedInfoResult;

        Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.AccountInfo = _getPlayerInfo.InfoResultPayload.AccountInfo;
        Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserInventory = _getPlayerInfo.InfoResultPayload.UserInventory;
        Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserData = _getPlayerInfo.InfoResultPayload.UserData;
        Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics = _getPlayerInfo.InfoResultPayload.PlayerStatistics;
        Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserVirtualCurrency = _getPlayerInfo.InfoResultPayload.UserVirtualCurrency;

        Managers.Player.GetPlayer(Managers.Game.PlayerId).CreateSession();
        foreach (PlayFab.ClientModels.ItemInstance item in Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserInventory.Where(x=>x.CustomData != null))
        {
            // 광고 보상 버프아이템의 경우 시간 체크
            if (item.CustomData.ContainsKey(Buff.CustomDataKey.Expire.ToString()) && item.ItemClass == "Buff")
            {

                TimeSpan _diff = GlobalTime.Now - Managers.Player.GetPlayer(Managers.Game.PlayerId).LastSession.LastAccessTime;
          
                DateTime _expireTime = DateTime.Parse(item.CustomData[Buff.CustomDataKey.Expire.ToString()] , null , System.Globalization.DateTimeStyles.RoundtripKind) ;
                _expireTime += _diff;
            

                Buff _ad_buff = Managers.Item.Database.ItemList.Where(x => x.ItemId == item.ItemId).First() as Buff;
                DateTime max = GlobalTime.Now.AddMinutes(_ad_buff.Minutes);
          
                if (DateTime.Compare(_expireTime , max) > 0)
                    _expireTime = max;
            

                item.CustomData[Buff.CustomDataKey.Expire.ToString()] = Util.GetTimeString(_expireTime);
          
                Managers.Network.UpdateItemCustomData(item.ItemInstanceId, item.CustomData);
                //Managers.Player.GetPlayer(Managers.Game.PlayerId).LastSession.LastAccessTime
            }
        }


        Managers.Network.UpdateSessionInfo(Managers.Player.GetPlayer(Managers.Game.PlayerId).CreateSession());
        Managers.Ranking.UpdateLeaderboard();

        Managers.Game.Load(Managers.Player.GetPlayer(Managers.Game.PlayerId));

       




    }
    public static void OnGetTitleAndProfile(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.GetPlayerCombinedInfoResult _getPlayerProfile = result as PlayFab.ClientModels.GetPlayerCombinedInfoResult;
        Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload = _getPlayerProfile.InfoResultPayload;

        Managers.Network.ServerState = JsonUtility.FromJson<ServiceStatus>(_getPlayerProfile.InfoResultPayload.TitleData[PlayerInfo.TitleDataKey.ServiceState.ToString()]);

        if(Managers.Network.ServerState.IsAcceptVersion(Define.APPVersion) == false)
        {
            Debug.Log("APP Version is not Accepted");
            UI_Messagebox _messageBox = Managers.UI.ShowPopupUI<UI_Messagebox>();
            _messageBox.mode = UI_Messagebox.Mode.OK;
            _messageBox.Text = $"업데이트가 필요합니다. {System.Environment.NewLine }확인을 누르시면 {System.Environment.NewLine } 스토어로 이동합니다";
            _messageBox.Title = "알림창";
            _messageBox.OK += () => { Application.OpenURL("market://details?id=com.onethesoft.GodOfArcher"); Application.Quit(); };
            return;
        }
#if !ENABLE_LOG
        if(Managers.Network.ServerState.IsNormal() == false)
        {
            Debug.Log("점검중");
            UI_Messagebox _messageBox = Managers.UI.ShowPopupUI<UI_Messagebox>();
            _messageBox.mode = UI_Messagebox.Mode.OK;
            _messageBox.Text = "현재 점검중입니다";
            _messageBox.Title = "알림창";
            _messageBox.OK += () => { Application.Quit(); };
            return;
        }
#endif


        if (string.IsNullOrEmpty(_getPlayerProfile.InfoResultPayload.PlayerProfile.DisplayName))
        {
            Managers.UI.ShowPopupUI<UI_NickName>();
        }
        else
        {
            Managers.Player.GetPlayer(Managers.Game.PlayerId).DisplayName = _getPlayerProfile.InfoResultPayload.PlayerProfile.DisplayName;
            Managers.UI.ShowPopupUI<UI_Loading>();


            Managers.Network.GetSessionInfo((result) => {

                PlayFab.ClientModels.GetUserDataResult _session = result as PlayFab.ClientModels.GetUserDataResult;
                SessionInfo _serverSession = SessionInfo.FromJson(_session.Data[PlayerInfo.UserDataKey.SessionInfo.ToString()].Value);
                Managers.Player.GetPlayer(Managers.Game.PlayerId).LastSession = _serverSession;
                Managers.Network.GetPlayerInfo();

            });
            
        }

        //_getPlayerProfile.PlayerProfile.DisplayName;
    }

    public static void OnUpdateSessionInfo(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.UpdateUserDataResult _session = result as PlayFab.ClientModels.UpdateUserDataResult;

        Debug.Log("UpdateSessionInfo Response");
        
    }

    public static void OnGetSessionInfo(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.GetUserDataResult _session = result as PlayFab.ClientModels.GetUserDataResult;


        SessionInfo _serverSession = SessionInfo.FromJson(_session.Data[PlayerInfo.UserDataKey.SessionInfo.ToString()].Value);
        /*
        if(Managers.Player.GetPlayer(Managers.Game.PlayerId).Session.DeviceId != _serverSession.DeviceId)
        {
            Debug.Log("다중 디바이스 접속금지");
        }
        else
        {
            
                
            

        }
        */

        if (IsExistCustomHandler(NetworkManager.Command.GetSessionInfo))
            _customHandlers[NetworkManager.Command.GetSessionInfo].Dequeue()?.Invoke(result);
       
    }

    public static void OnUpdateStatisticsData(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.UpdatePlayerStatisticsResult _result = result as PlayFab.ClientModels.UpdatePlayerStatisticsResult;
        PlayFab.ClientModels.UpdatePlayerStatisticsRequest _requestData = PlayFab.Json.PlayFabSimpleJson.DeserializeObject<PlayFab.ClientModels.UpdatePlayerStatisticsRequest>(_result.Request.ToJson());

        /*
        foreach (PlayFab.ClientModels.StatisticUpdate update in _requestData.Statistics)
        {
            if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Any(x => x.StatisticName == update.StatisticName))
            {
                Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Find(x => x.StatisticName == update.StatisticName).Value = update.Value;
            }
        }
        */

        
    }

    public static void OnGrantItems(PlayFabResultCommon result)
    {

        PlayFab.ServerModels.GrantItemsToUsersResult _result = result as PlayFab.ServerModels.GrantItemsToUsersResult;
        Debug.Log(_result.ToJson());
        if (Managers.Player.GetPlayer(Managers.Game.PlayerId) == null)
            return;

        // 버프를 줄 때 CustomData 가 적용안될 때가 있다. 따라서 Request 와 비교하여 누락된 부분이 있으면 업데이트를 요청한다.
        // 2023-03-02 우편함 수령부분에서 에러 발생.퀘스트 보상으로 메일 수령시 CodeName 이 CustomData 로 들어가는데 Dictionary 에서 중복키 오류 발생함.
        // 따라서 Stackable 아이템에서만 해당 루틴을 넣어둔다.

        PlayFab.ServerModels.GrantItemsToUsersRequest _request = _result.Request as PlayFab.ServerModels.GrantItemsToUsersRequest;
        Dictionary<string, Dictionary<string, string>> _updateCustomData = null;
        foreach(ItemGrant grant in _request.ItemGrants)
        {
            if(grant.Data != null && Managers.Item.Database.ItemList.Where(x=>x.ItemId == grant.ItemId).FirstOrDefault().IsStackable == true)
            {
                GrantedItemInstance getItem = _result.ItemGrantResults.Where(x => x.ItemId == grant.ItemId).FirstOrDefault();
             
                foreach(KeyValuePair<string,string> pair in getItem.CustomData)
                {
                    if (grant.Data.ToList().Any(x => x.Key == pair.Key && x.Value != pair.Value))
                    {
                        if (_updateCustomData == null)
                            _updateCustomData = new Dictionary<string, Dictionary<string, string>>();
                        _updateCustomData.Add(getItem.ItemInstanceId, grant.Data);
                        getItem.CustomData = grant.Data;
                    }
                }
            }
        }

        if(_updateCustomData != null)
            if(_updateCustomData.Count > 0)
            {
                float _totalDelay = 0.0f;
                foreach(KeyValuePair<string , Dictionary<string,string>> pair in _updateCustomData)
                {
                    Managers.Job.ReserveJob(TimeSpan.FromSeconds(_totalDelay), (args) =>
                    {
                        string _instanceId = args[0] as string;
                        Dictionary<string, string> _customData = args[1] as Dictionary<string, string>;
                        Managers.Network.UpdateItemCustomData(_instanceId, _customData);
                    }, new object[] { pair.Key ,pair.Value });
                    _totalDelay += 0.2f;
                }
            }

        

        foreach (GrantedItemInstance grantedItem in _result.ItemGrantResults)
        {
            Managers.Player.GetPlayer(Managers.Game.PlayerId).AddItemInstance(grantedItem);
            IAPData _data = Managers.Shop.Database.IAPItems.Where(x => x.Item.ItemId == grantedItem.ItemId).FirstOrDefault();
            if(_data != null)
                foreach (BundleCurrencyContent currency in _data.Item.Currencies)
                {
                    Define.CurrencyID _codeName = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), currency.Currency.CodeName);
                    Managers.Player.GetPlayer(Managers.Game.PlayerId).AddCurrency(_codeName, (System.Numerics.BigInteger)currency.Amount);
                }
        }

        if (IsExistCustomHandler(NetworkManager.Command.GrantItems))
            _customHandlers[NetworkManager.Command.GrantItems].Dequeue()?.Invoke(result);
        
       
        
    }
    public static void OnAddCurrency(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.ModifyUserVirtualCurrencyResult _result = result as PlayFab.ClientModels.ModifyUserVirtualCurrencyResult;
        Debug.Log($"PacketHandler : OnAddCurrency Key {_result.VirtualCurrency} , ChangeAmount : {_result.BalanceChange} , AfterChangedBalance" + _result.Balance);

        Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserVirtualCurrency[_result.VirtualCurrency] = _result.Balance;
    }
    public static void OnSubtractCurrency(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.ModifyUserVirtualCurrencyResult _result = result as PlayFab.ClientModels.ModifyUserVirtualCurrencyResult;
        Debug.Log($"PacketHandler : OnSubtractCurrency Key {_result.VirtualCurrency} , ChangeAmount : {_result.BalanceChange} , AfterChangedBalance" + _result.Balance);

        Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserVirtualCurrency[_result.VirtualCurrency] = _result.Balance;
    }

    public static void OnValidatePurchase(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.ValidateGooglePlayPurchaseResult _result = result as PlayFab.ClientModels.ValidateGooglePlayPurchaseResult;
        Debug.Log("OnValidatePurchase");

        // 아이템의 정보를 업데이트 한다.
        foreach (PlayFab.ClientModels.PurchaseReceiptFulfillment fullfill in _result.Fulfillments)
            foreach (PlayFab.ClientModels.ItemInstance item in fullfill.FulfilledItems)
            {
                Debug.Log(item.ToJson());
                if (Managers.Game.GetInventory().Find(x => x.ItemInstanceId == item.ItemInstanceId) == null)
                {
                    if(Managers.Item.Database.ItemList.Where(x=>x.ItemId == item.ItemId).FirstOrDefault().IsStackable)
                    {
                        List<BaseItem> _added = Managers.Game.GetInventory().AddItem(item.ItemId, item.UsesIncrementedBy.GetValueOrDefault());
                        _added.ForEach(x =>
                        {
                            if (x.ItemId == item.ItemId)
                                x.Setup(item);
                        });
                    }
                    else
                    {
                        List<BaseItem> _added = Managers.Game.GetInventory().AddItem(item.ItemId, 1);
                        _added.ForEach(x =>
                        {
                            if (x.ItemId == item.ItemId)
                                x.Setup(item);
                        });
                    }
                    
                }
                else
                    Managers.Game.GetInventory().Find(x => x.ItemInstanceId == item.ItemInstanceId).Setup(item);

            }

        if (UnityEngine.Object.FindObjectOfType<UI_LoadingBlock>() != null)
            Managers.UI.ClosePopupUI();


        // ProcessGiveToUser 에서 유저 인벤토리에게 아이템을 생성 후 부여한다.
        foreach (PlayFab.ClientModels.PurchaseReceiptFulfillment fullfill in _result.Fulfillments)
        {
            foreach (PlayFab.ClientModels.ItemInstance grantedItem in fullfill.FulfilledItems)
            {
                Managers.Player.GetPlayer(Managers.Game.PlayerId).AddItemInstance(grantedItem);

                if (Managers.Shop.Database.IAPItems.Any(x => x.Item.ItemId == grantedItem.ItemId) || 
                    Managers.Shop.Database.RubyPackageItems.Any(x => x.Item.ItemId == grantedItem.ItemId))
                {
                    IAPData _data = Managers.Shop.Database.IAPItems.Any(x => x.Item.ItemId == grantedItem.ItemId) ?
                        Managers.Shop.Database.IAPItems.Where(x => x.Item.ItemId == grantedItem.ItemId).FirstOrDefault() :
                        Managers.Shop.Database.RubyPackageItems.Where(x => x.Item.ItemId == grantedItem.ItemId).FirstOrDefault();


                    foreach (BundleCurrencyContent currency in _data.Item.Currencies)
                    {
                        Define.CurrencyID _codeName = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID) , currency.Currency.CodeName);
                        Managers.Player.GetPlayer(Managers.Game.PlayerId).AddCurrency(_codeName, (System.Numerics.BigInteger)currency.Amount);

                    }
                     
                    Managers.Shop.ProcessGiveToUser(grantedItem.ItemId);
                }
                else if (Managers.Shop.Database.Seasonpass.Item.ItemId == grantedItem.ItemId 
                    || Managers.Shop.Database.Seasonpass2.Item.ItemId == grantedItem.ItemId
                     || Managers.Shop.Database.Seasonpass3.Item.ItemId == grantedItem.ItemId)
                    Managers.Shop.ProcessGiveToUser(grantedItem.ItemId);
            }
        }

       
              





        //_result.Fulfillments[0].

    }

    public static void OnPurchaseItem(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.PurchaseItemResult _result = result as PlayFab.ClientModels.PurchaseItemResult;

        foreach(PlayFab.ClientModels.ItemInstance item in _result.Items)
        {
            ShopDate _getdata = Managers.Shop.GetShopData(item.ItemId);
            if (_getdata == null)
                continue;

            ShopButtonData _data = _getdata.BtnLists.Where(x => x.PurchaseItem.ItemId == item.ItemId).FirstOrDefault();
            Define.CurrencyID _codeName = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), _data.Currency);
            Managers.Player.GetPlayer(Managers.Game.PlayerId).SubstractCurrency(_codeName, (System.Numerics.BigInteger)_data.Price);
        }



        if (IsExistCustomHandler(NetworkManager.Command.PurchaseItem))
            _customHandlers[NetworkManager.Command.PurchaseItem].Dequeue()?.Invoke(result);
     
       


    }
    public static void OnUpgradeItems(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.ExecuteCloudScriptResult _result = result as PlayFab.ClientModels.ExecuteCloudScriptResult;

        PlayFab.Json.JsonObject _ret = (PlayFab.Json.JsonObject)_result.FunctionResult;

        if (_ret == null)
            return;

        object _functionResult;
        object _modifyItemList;


        _ret.TryGetValue("result" , out _functionResult);
        _ret.TryGetValue("ModifyItemList", out _modifyItemList);
       

        IEnumerable arrayModifyItems = _modifyItemList as IEnumerable;

        foreach(object modifyItem in arrayModifyItems)
        {
            PlayFab.Json.JsonObject modifyObj = modifyItem as PlayFab.Json.JsonObject;
            ModifyItemResponse _modifyresult = JsonUtility.FromJson<ModifyItemResponse>(modifyObj.ToString());
            if(_modifyresult.RemainingUses > 0)
            {
                BaseItem _find = Managers.Game.GetInventory().Find(x => x.ItemId == _modifyresult.ItemId);
                if (_find != null && string.IsNullOrEmpty(_find.ItemInstanceId))
                    _find.SetItemInstanceId(_modifyresult.ItemInstanceId);

            }
            
          
            //PlayFab.Json.PlayFabSimpleJson.DeserializeObject<ItemInstance>(item.ToString());
        }
        /*
        IEnumerable arrayGrantItems = _grantItemList as IEnumerable;
        foreach (object grantItem in arrayGrantItems)
        {
            PlayFab.Json.JsonObject grantItemobj = grantItem as PlayFab.Json.JsonObject;
            GrantedItemInstance _modifyresult = PlayFab.Json.PlayFabSimpleJson.DeserializeObject<GrantedItemInstance>(grantItemobj.ToString());
            BaseItem _findItem = Managers.Game.GetInventory().Find(x => x.ItemId == _modifyresult.ItemId);

            if (_findItem != null)
            {
                _findItem.Setup(new PlayFab.ClientModels.ItemInstance
                {
                    ItemId = _modifyresult.ItemId,
                    ItemInstanceId = _modifyresult.ItemInstanceId,
                    Annotation = _modifyresult.Annotation,
                    DisplayName = _modifyresult.DisplayName,
                    Expiration = _modifyresult.Expiration,
                    CatalogVersion = _modifyresult.CatalogVersion,
                    ItemClass = _modifyresult.ItemClass,
                    UsesIncrementedBy = _modifyresult.UsesIncrementedBy,
                    RemainingUses = _modifyresult.RemainingUses,
                    PurchaseDate = _modifyresult.PurchaseDate,
                    BundleContents = _modifyresult.BundleContents,
                    BundleParent = _modifyresult.BundleParent,
                    CustomData = _modifyresult.CustomData,
                    UnitCurrency = _modifyresult.UnitCurrency,
                    UnitPrice = _modifyresult.UnitPrice
                });
            }


            //PlayFab.Json.PlayFabSimpleJson.DeserializeObject<ItemInstance>(item.ToString());
        }
        */

    }
    public static void OnUnlockContainerItemInstance(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.UnlockContainerItemResult _result = result as PlayFab.ClientModels.UnlockContainerItemResult;

        Debug.Log(_result.ToJson());
        if(_result.VirtualCurrency != null)
            foreach(KeyValuePair<string,uint> pair in _result.VirtualCurrency)
            {
                Currency _findCurrency = Managers.Game.PlyaerDataBase.CurrencyList.Where(x => x.ShortCodeName == pair.Key).FirstOrDefault();

                Define.CurrencyID _id = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), _findCurrency.CodeName);
                Managers.Player.GetPlayer(Managers.Game.PlayerId).AddCurrency(_id, (System.Numerics.BigInteger)pair.Value);
            }

        // 2023 03 28 
        // Bundle 로 주어지는 재화가 존재할 때 UnlockContainerItemResult 에는 반영되지 않는다.
        // 따라서 수동으로 반영

        if (_result.GrantedItems != null)
            foreach(PlayFab.ClientModels.ItemInstance instance in _result.GrantedItems)
            {
                BaseItem _item = Managers.Item.Database.ItemList.Where(x => x.ItemId == instance.ItemId).First();
                if (_item is Bundle )
                {
                    Bundle _bundle = _item as Bundle;
                    if(_bundle.Currencies != null && _bundle.Currencies.Count > 0)
                    {
                        foreach(BundleCurrencyContent content in _bundle.Currencies)
                        {
                            Define.CurrencyID _id = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), content.Currency.CodeName);
                            Managers.Player.GetPlayer(Managers.Game.PlayerId).AddCurrency(_id, content.Amount);
                        }
                    }
                }
            }

        if (IsExistCustomHandler(NetworkManager.Command.UnlockContainerItem))
            _customHandlers[NetworkManager.Command.UnlockContainerItem].Dequeue()?.Invoke(result);
        //Managers.Player.GetPlayer(Managers.Game.PlayerId).ad
        
        //if(_result.)
    }
    public static void OnUnlockContainerItemWithItemId(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.UnlockContainerItemResult _result = result as PlayFab.ClientModels.UnlockContainerItemResult;

        Debug.Log(_result.ToJson());
        if (_result.VirtualCurrency != null)
            foreach (KeyValuePair<string, uint> pair in _result.VirtualCurrency)
            {
                Currency _findCurrency = Managers.Game.PlyaerDataBase.CurrencyList.Where(x => x.ShortCodeName == pair.Key).FirstOrDefault();

                Define.CurrencyID _id = (Define.CurrencyID)Enum.Parse(typeof(Define.CurrencyID), _findCurrency.CodeName);
                Managers.Player.GetPlayer(Managers.Game.PlayerId).AddCurrency(_id, (System.Numerics.BigInteger)pair.Value);
            }

        if (IsExistCustomHandler(NetworkManager.Command.UnlockContainerItem))
            _customHandlers[NetworkManager.Command.UnlockContainerItem].Dequeue()?.Invoke(result);
    }
    public static void OnGetLeaderBoard(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.GetLeaderboardResult _result = result as PlayFab.ClientModels.GetLeaderboardResult;
        Debug.Log(_result.ToJson());

        foreach(PlayFab.ClientModels.PlayerLeaderboardEntry entry in _result.Leaderboard)
            Managers.Ranking.AddLeaderboardEntry(entry);

        if (IsExistCustomHandler(NetworkManager.Command.GetLeaderBoard))
            _customHandlers[NetworkManager.Command.GetLeaderBoard].Dequeue()?.Invoke(result);


    }

    public static void OnGetRankerData(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.GetUserDataResult _result = result as PlayFab.ClientModels.GetUserDataResult;
        Debug.Log(_result.ToJson());
        PlayFab.ClientModels.GetUserDataRequest _request = PlayFab.Json.PlayFabSimpleJson.DeserializeObject<PlayFab.ClientModels.GetUserDataRequest>(_result.Request.ToJson());

        if (_result.Data == null)
            _result.Data = new Dictionary<string, PlayFab.ClientModels.UserDataRecord> { { PlayerInfo.UserDataKey.DPS.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = "2" } } };
        else if(_result.Data.ContainsKey(PlayerInfo.UserDataKey.DPS.ToString()) == false)
        {
            _result.Data.Add(PlayerInfo.UserDataKey.DPS.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = "2" });
        }
        

        if (_request.PlayFabId != Managers.Game.PlayerId)
        {
            if (Managers.Player.GetPlayer(_request.PlayFabId) == null)
                Managers.Player.AddPlayer(_request.PlayFabId, new PlayerInfo { PlayfabId = _request.PlayFabId, Payload = new PlayFab.ClientModels.GetPlayerCombinedInfoResultPayload { UserData = _result.Data } });
            else
                Managers.Player.GetPlayer(_request.PlayFabId).Payload.UserData = _result.Data;
        }

        

        //Managers.Player.AddPlayer(_result.PlayFabId , _result.)

    }

    public static void OnConsumeItem(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.ConsumeItemResult _result = result as PlayFab.ClientModels.ConsumeItemResult;
        Debug.Log(_result.ToJson());
        if (IsExistCustomHandler(NetworkManager.Command.ConsumeItem))
            _customHandlers[NetworkManager.Command.ConsumeItem].Dequeue()?.Invoke(result);
    }

    public static void OnUpdateItemCustomData(PlayFabResultCommon result)
    {
        PlayFab.ServerModels.EmptyResponse _result = result as PlayFab.ServerModels.EmptyResponse;
        Debug.Log(_result.ToJson());
    }

    public static void OnGetCatalogItems(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.GetCatalogItemsResult _result = result as PlayFab.ClientModels.GetCatalogItemsResult;
        Debug.Log(_result.ToJson());

        Managers.Item.AddCatalogItems(_result.Catalog);
        Managers.Network.GetTitleAndProfileData();


    }

    public static void OnDeletePlayer(PlayFabResultCommon result)
    {
        PlayFab.ClientModels.ExecuteCloudScriptResult _result = result as PlayFab.ClientModels.ExecuteCloudScriptResult;
        Debug.Log(_result.ToJson());

        PlayFab.Json.JsonObject _ret = (PlayFab.Json.JsonObject)_result.FunctionResult;

        object _functionResult;
    

        _ret.TryGetValue("result", out _functionResult);


        Application.Quit();
        



    }

    
    public static void Init()
    {
        _handlers = new Dictionary<NetworkManager.Command, Action<PlayFabResultCommon>>();
        _customHandlers = new Dictionary<NetworkManager.Command, Queue<Action<PlayFabResultCommon>>>();

        foreach (NetworkManager.Command command in Enum.GetValues(typeof(NetworkManager.Command)))
            _customHandlers.Add(command, new Queue<Action<PlayFabResultCommon>>());

        _handlers.Add(NetworkManager.Command.Login, OnLogin);
        _handlers.Add(NetworkManager.Command.Register, OnRegister);
        _handlers.Add(NetworkManager.Command.UpdateEntityToken, OnUpdateEntityToken);
        _handlers.Add(NetworkManager.Command.UpdateDisplayName, OnUpdateDisplayName);
        _handlers.Add(NetworkManager.Command.GetTime, OnGetTime);
        _handlers.Add(NetworkManager.Command.GetPlayerInfo, OnGetPlayerInfo);
        _handlers.Add(NetworkManager.Command.GetTitleAndProfile, OnGetTitleAndProfile);

        _handlers.Add(NetworkManager.Command.UpdateSessionInfo, OnUpdateSessionInfo);
        _handlers.Add(NetworkManager.Command.GetSessionInfo, OnGetSessionInfo);

        _handlers.Add(NetworkManager.Command.UpdateUserData, OnUpdateUserData);
        _handlers.Add(NetworkManager.Command.UpdateStatisticsData, OnUpdateStatisticsData);
        _handlers.Add(NetworkManager.Command.AddCurrency, OnAddCurrency);
        _handlers.Add(NetworkManager.Command.SubtractCurrency, OnSubtractCurrency);
        _handlers.Add(NetworkManager.Command.GrantItems, OnGrantItems);
        _handlers.Add(NetworkManager.Command.ValidatePurchase, OnValidatePurchase);
        _handlers.Add(NetworkManager.Command.PurchaseItem, OnPurchaseItem);
        _handlers.Add(NetworkManager.Command.UnlockContainerItem, OnUnlockContainerItemInstance);
        _handlers.Add(NetworkManager.Command.UnlockContainerItemWithItemId, OnUnlockContainerItemWithItemId);
        _handlers.Add(NetworkManager.Command.UpgradeItems, OnUpgradeItems);
        _handlers.Add(NetworkManager.Command.GetLeaderBoard, OnGetLeaderBoard);
        _handlers.Add(NetworkManager.Command.GetRankerData, OnGetRankerData);
        _handlers.Add(NetworkManager.Command.ConsumeItem, OnConsumeItem);
        _handlers.Add(NetworkManager.Command.UpdateItemCustomData, OnUpdateItemCustomData);
        _handlers.Add(NetworkManager.Command.GetCatalogItems, OnGetCatalogItems);
        _handlers.Add(NetworkManager.Command.DeletePlayer, OnDeletePlayer);
    }

    public static void OnHandle(NetworkManager.Command id , PlayFabResultCommon result)
    {
        if (_handlers.ContainsKey(id) == false)
            Debug.LogError($"PacketHandler : _handlers not find");
            
        
        _handlers[id].Invoke(result);
    }

    public static void OnHandleError(PlayFabError error)
    {
        if(error.Error == PlayFabErrorCode.AccountNotFound)
        {
            UI_Title title = UnityEngine.Object.FindObjectOfType<UI_Title>();
            if (title != null)
                title.ShowRegisterButton();
        }
       
    }

}
