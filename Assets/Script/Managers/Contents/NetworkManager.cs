using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using PlayFab.ServerModels;
using System.Linq;
using System.Threading;



/*
 * Playfab Wrapper
 */
public class NetworkManager 
{
    public enum Command
    {
        Login,
        Register,
        UpdateEntityToken,
        UpdateDisplayName,
        UpdateSessionInfo,
        GetPlayerInfo,
        GetTime,
        GetTitleAndProfile,
        GetSessionInfo,
        UpdateUserData,
        UpdateStatisticsData,
        AddCurrency,
        SubtractCurrency,
        GrantItems,
        ValidatePurchase,
        PurchaseItem,
        UnlockContainerItem,
        UnlockContainerItemWithItemId,
        UpgradeItems,
        GetLeaderBoard,
        GetRankerData,
        ConsumeItem,
        UpdateItemCustomData,
        GetCatalogItems,
        DeletePlayer
        

    }

    class CommandInfo
    {
        public int Id;
        public Command Command;
        public PlayFabRequestCommon Argument;
    }
    
   
   
    readonly int? CloudScriptRevision = null;


    public System.Action<PlayFabErrorCode> OnErrorCallback = null;
    public readonly bool IS_ENABLE_NETWORK = true;
    public ServiceStatus ServerState;
    public static readonly string CatalogVersion = "Main";
    public readonly int MaxCallsAPICount = 23;



    private Queue<CommandInfo> _pendingCallsQueue = new Queue<CommandInfo>();
    private Queue<CommandInfo> _waitCallsQueue = new Queue<CommandInfo>();
    private Queue<CommandInfo> _poolingQueue = new Queue<CommandInfo>();


    private int _commandId  = 0;
    private readonly int MaxCallsAPICountFor = 1;
    private object _lock = new object();
    private float _delay = 0.1f;
    private float _time = 0.0f;

    private PlayfabSender _sender;
    private List<PlayfabSender> _senderQueue = new List<PlayfabSender>();
    private List<PlayfabSender> _pooling = new List<PlayfabSender>();
    private PlayfabExecuter _executer;





    private readonly string TestID = "AdminTest_4"; // 룬 슬롯 , 펫 슬롯 테스트 위한 신규 아이디
    //"AdminTest_3"; 기존에 사용하던 테스트 아이디
    private const string TitleId = "E576D";


    public void Init()
    {
        
        PlayFabSettings.staticSettings.TitleId = TitleId;

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        
        PacketHandler.Init();

        GameObject _PlayfabExecuter = GameObject.Find("PlayfabExecuter");
        if(_PlayfabExecuter == null)
        {
            _PlayfabExecuter = new GameObject { name = "PlayfabExecuter" };
            _executer = Util.GetOrAddComponent<PlayfabExecuter>(_PlayfabExecuter);
            Object.DontDestroyOnLoad(_PlayfabExecuter);
        }

        

    }
    void Request(Command command , PlayFabRequestCommon request)
    {
        if(_poolingQueue.Count == 0)
        {
            lock (_lock)
            {
                _waitCallsQueue.Enqueue(new CommandInfo { Id = Interlocked.Increment(ref _commandId), Command = command, Argument = request });
            }
        }
        else
        {
            CommandInfo _info = _poolingQueue.Dequeue();
            _info.Id = Interlocked.Increment(ref _commandId);
            _info.Command = command;
            _info.Argument = request;
            lock (_lock)
            {
                _waitCallsQueue.Enqueue(_info);
            }
                
        }
        
        /*
        if (_pendingCallsQueue.Count > MaxCallsAPICountFor)
        {
            _waitCallsQueue.Enqueue(new CommandInfo { Id = Interlocked.Increment(ref _commandId), Command = command, Argument = request });
            return;
        }
        else
        {
            _pendingCallsQueue.Enqueue(new CommandInfo { Id = Interlocked.Increment(ref _commandId), Command = command, Argument = request });
        }
        */
        
    }
    void Execute()
    {
        CommandInfo _getCommand = _waitCallsQueue.Dequeue();
        _pendingCallsQueue.Enqueue(_getCommand);

        Command command = _getCommand.Command;
        PlayFabRequestCommon request = _getCommand.Argument;

     

        switch (command)
        {
            case Command.Login:
            case Command.Register:
            case Command.UpdateEntityToken:
#if UNITY_EDITOR
                PlayFabClientAPI.LoginWithCustomID(request as LoginWithCustomIDRequest, (result) => OnHandle(command, result), error => OnHandleError(error));
#elif UNITY_ANDROID
                PlayFabClientAPI.LoginWithGooglePlayGamesServices(request as LoginWithGooglePlayGamesServicesRequest, result => OnHandle(command , result) , error => OnHandleError(error));
#endif
                break;
            case Command.UpdateDisplayName:
                PlayFabClientAPI.UpdateUserTitleDisplayName(request as UpdateUserTitleDisplayNameRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.GetTime:
                PlayFabClientAPI.GetTime(request as PlayFab.ClientModels.GetTimeRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;

            case Command.GetPlayerInfo:
            case Command.GetTitleAndProfile:

                PlayFabClientAPI.GetPlayerCombinedInfo(request as PlayFab.ClientModels.GetPlayerCombinedInfoRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.UpdateSessionInfo:
            case Command.UpdateUserData:
                PlayFabClientAPI.UpdateUserData(request as PlayFab.ClientModels.UpdateUserDataRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.UpdateStatisticsData:
                PlayFabClientAPI.UpdatePlayerStatistics(request as PlayFab.ClientModels.UpdatePlayerStatisticsRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.GrantItems:
                PlayFabServerAPI.GrantItemsToUsers(request as PlayFab.ServerModels.GrantItemsToUsersRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.AddCurrency:
                PlayFabClientAPI.AddUserVirtualCurrency(request as PlayFab.ClientModels.AddUserVirtualCurrencyRequest, result => OnHandle(command, result),
                    error => OnHandleError(error));
                break;
            case Command.SubtractCurrency:
                PlayFabClientAPI.SubtractUserVirtualCurrency(request as PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest, result => OnHandle(command, result),
                    error => OnHandleError(error));
                break;
            case Command.ValidatePurchase:
                PlayFabClientAPI.ValidateGooglePlayPurchase(request as PlayFab.ClientModels.ValidateGooglePlayPurchaseRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.PurchaseItem:
                PlayFabClientAPI.PurchaseItem(request as PlayFab.ClientModels.PurchaseItemRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.UnlockContainerItem:
                Debug.Log(request.ToJson());
                PlayFabClientAPI.UnlockContainerInstance(request as PlayFab.ClientModels.UnlockContainerInstanceRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.UnlockContainerItemWithItemId:
                PlayFabClientAPI.UnlockContainerItem(request as PlayFab.ClientModels.UnlockContainerItemRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.GetLeaderBoard:
                PlayFabClientAPI.GetLeaderboard(request as PlayFab.ClientModels.GetLeaderboardRequest, result => OnHandle(command, result), error => { OnHandleError(error); });
                break;
            case Command.UpgradeItems:
                PlayFabClientAPI.ExecuteCloudScript(request as PlayFab.ClientModels.ExecuteCloudScriptRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.GetSessionInfo:
            case Command.GetRankerData:
                PlayFabClientAPI.GetUserData(request as PlayFab.ClientModels.GetUserDataRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.ConsumeItem:
                PlayFabClientAPI.ConsumeItem(request as PlayFab.ClientModels.ConsumeItemRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.UpdateItemCustomData:
                PlayFabServerAPI.UpdateUserInventoryItemCustomData(request as PlayFab.ServerModels.UpdateUserInventoryItemDataRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case Command.GetCatalogItems:
                PlayFabClientAPI.GetCatalogItems(request as PlayFab.ClientModels.GetCatalogItemsRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;

        }
    }
    void OnHandle(Command command, PlayFabResultCommon result)
    {

        _poolingQueue.Enqueue(_pendingCallsQueue.Dequeue());
       
        PacketHandler.OnHandle(command, result);
    }

    void OnHandleError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
      
        switch (error.Error)
        {
            case PlayFabErrorCode.ConnectionError:
                break;
            case PlayFabErrorCode.InvalidContainerItem:
                Debug.Log(error.ErrorMessage);
                Debug.Log(error.HttpCode);
                Debug.Log(error.HttpStatus);
              


                break;
            case PlayFabErrorCode.InvalidReceipt:
                break;
            case PlayFabErrorCode.ReceiptAlreadyUsed:
                break;
            // 재시도 가능////////////////////////////////////////////////////////////////
            case PlayFabErrorCode.APIClientRequestRateLimitExceeded:        // Indicates too many calls in a short burst.
                break;
            case PlayFabErrorCode.APIConcurrentRequestLimitExceeded:        // Indicates too many simultaneous calls.
                break;
            case PlayFabErrorCode.ConcurrentEditError:                      // Indicates too many simultaneous calls or very rapid sequential calls.
                break;
            case PlayFabErrorCode.DataUpdateRateExceeded:                   // Indicates too many simultaneous calls, or very rapid sequential calls.
                break;
            case PlayFabErrorCode.DownstreamServiceUnavailable:             // Indicates that PlayFab or a third-party service might be having a temporary issue.
                break;
            case PlayFabErrorCode.InvalidAPIEndpoint:                       // Indicates that the URL for this request is not valid for this title.
                break;
            case PlayFabErrorCode.OverLimit:                                 // Indicates that an attempt to perform an operation will cause the service usage to go over the limit as shown in the Game Manager limit pages. Evaluate the returned error details to determine which limit would be exceeded.
                break;
            case PlayFabErrorCode.ServiceUnavailable:                       // Indicates that PlayFab may be having a temporary issue or the client is making too many API calls too quickly.
                break;
            ///////////////////////////////////////////////////////////////////////////////
            case PlayFabErrorCode.AccountNotFound:
                // 최초 로그인 시 실패하므로 ErrorCallback을 호출하여 가입 버튼을 보이게한다.
                OnErrorCallback?.Invoke(error.Error);
                    OnErrorCallback = null;
                    return;
                break;
            

        }

        UI_LogTest _showLog = UnityEngine.Object.FindObjectOfType<UI_LogTest>();
        if (_showLog == null)
            Managers.UI.ShowPopupUI<UI_LogTest>().Setup("Errorcode : "+ error.Error + System.Environment.NewLine + error.GenerateErrorReport());
        else
            _showLog.AddLog(error.GenerateErrorReport());
        
        

       



    }



    void Authenticate(System.Action callback)
    {
      
        if (IS_ENABLE_NETWORK)
        {
            if (PlayGamesPlatform.Instance.IsAuthenticated())
                callback?.Invoke();
            else
            {
                PlayGamesPlatform.Instance.Authenticate(status => {
                    if (status == GooglePlayGames.BasicApi.SignInStatus.Success)
                        callback?.Invoke();
                    else
                        Debug.Log(status.ToString());
                });

               
            }
        }
    }


    public void RequestRegister()
    {
        
        if (IS_ENABLE_NETWORK)
        {
#if UNITY_EDITOR


            PlayfabSender _sender = _executer.Pop();
            _sender.Setup(Command.Register, new LoginWithCustomIDRequest { CreateAccount = true, TitleId = TitleId, CustomId = TestID });
            _executer.Execute(_sender);

            //Request(Command.Register, new LoginWithCustomIDRequest { CreateAccount = true, TitleId = TitleId, CustomId = TestID });

#elif UNITY_ANDROID

            Authenticate(() => {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                     PlayfabSender _sender = _executer.Pop();
                    _sender.Setup(Command.Register, new LoginWithGooglePlayGamesServicesRequest { CreateAccount = true, TitleId = TitleId, ServerAuthCode = code });
                     _executer.Execute(_sender);
                    
                });
            });
            /*
            Authenticate(() => {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Request(Command.Register, new LoginWithGooglePlayGamesServicesRequest { CreateAccount = true, TitleId = TitleId, ServerAuthCode = code });
                    
                });
            });
            */
#endif

        }
        else
        {
           
            
        }
        
    }


    public void RequestLogin()
    {
        if (IS_ENABLE_NETWORK)
        {
#if UNITY_EDITOR
            PlayfabSender _sender = _executer.Pop();
            _sender.Setup(Command.Login, new LoginWithCustomIDRequest { CreateAccount = false, TitleId = TitleId, CustomId = TestID });
            _executer.Execute(_sender);
            //Request(Command.Login, new LoginWithCustomIDRequest { CreateAccount = false, TitleId = TitleId, CustomId = TestID });
#elif UNITY_ANDROID
             Authenticate(() => {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    PlayfabSender _sender = _executer.Pop();
                    _sender.Setup(Command.Login, new LoginWithGooglePlayGamesServicesRequest { TitleId = TitleId, CreateAccount = false, ServerAuthCode = code });
                    _executer.Execute(_sender);
                   
                });
            });
            /*
            Authenticate(() => {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Request(Command.Login, new LoginWithGooglePlayGamesServicesRequest { TitleId = TitleId, CreateAccount = false, ServerAuthCode = code });
                   
                });
            });
            */
#endif

        }
        else
        {
            
        }
          
        

    }

    public void UpdateEntityToken(System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK)
        {


#if UNITY_EDITOR
            PlayfabSender _sender = _executer.Pop();
            _sender.Setup(Command.UpdateEntityToken, new LoginWithCustomIDRequest { CreateAccount = false, TitleId = TitleId, CustomId = TestID }, OnAfterCallback);
            _executer.Execute(_sender);
#elif UNITY_ANDROID
            Authenticate(() => {
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => {
                    PlayfabSender _sender = _executer.Pop();
                    _sender.Setup(Command.UpdateEntityToken, new LoginWithGooglePlayGamesServicesRequest { TitleId = TitleId, CreateAccount = false, ServerAuthCode = code }, OnAfterCallback);
                    _sender.Execute();
                });
                
            });
            /*
            Authenticate(() => {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Request(Command.UpdateEntityToken, new LoginWithGooglePlayGamesServicesRequest { TitleId = TitleId, CreateAccount = false, ServerAuthCode = code });
                   
                });
            });
            */
#endif

        }
    }


    public void UpdateNickName(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
            return;

        if (IS_ENABLE_NETWORK)
        {
            PlayfabSender _sender = _executer.Pop();
            _sender.Setup(Command.UpdateDisplayName, new UpdateUserTitleDisplayNameRequest { DisplayName = nickname });
            _executer.Execute(_sender);
            //Request(Command.UpdateDisplayName, new UpdateUserTitleDisplayNameRequest { DisplayName = nickname });

        }


    }
    public void GetServerTime()
    {
        if (IS_ENABLE_NETWORK)
        {
            PlayfabSender _sender = _executer.Pop();
            _sender.Setup(Command.GetTime, new PlayFab.ClientModels.GetTimeRequest());
            _executer.Execute(_sender);
            //Request(Command.GetTime, new PlayFab.ClientModels.GetTimeRequest());

        }
    }

    public void GetTitleAndProfileData()
    {
        if (IS_ENABLE_NETWORK)
        {
            PlayfabSender _sender = _executer.Pop();
            _sender.Setup(Command.GetTitleAndProfile, new PlayFab.ClientModels.GetPlayerCombinedInfoRequest {
                PlayFabId = Managers.Game.PlayerId,
                InfoRequestParameters = new PlayFab.ClientModels.GetPlayerCombinedInfoRequestParams
                {
                    GetTitleData = true,
                    GetPlayerProfile = true,
                }
            });
            _executer.Execute(_sender);


            /*
            Request(Command.GetTitleAndProfile, new PlayFab.ClientModels.GetPlayerCombinedInfoRequest
            {
                PlayFabId = Managers.Game.PlayerId,
                InfoRequestParameters = new PlayFab.ClientModels.GetPlayerCombinedInfoRequestParams
                {
                    GetTitleData = true,
                    GetPlayerProfile = true,
                }
            });
            */

        }
    }

   
    public void GetPlayerInfo()
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.GetPlayerInfo, new PlayFab.ClientModels.GetPlayerCombinedInfoRequest
        {
            PlayFabId = Managers.Game.PlayerId,
            InfoRequestParameters = new PlayFab.ClientModels.GetPlayerCombinedInfoRequestParams
            {
                GetUserAccountInfo = true,
                GetUserInventory = true,
                GetUserData = true,
                GetPlayerStatistics = true,
                GetUserVirtualCurrency = true,
            }
        });

        _executer.Execute(_sender);

        /*
        Request(Command.GetPlayerInfo, new PlayFab.ClientModels.GetPlayerCombinedInfoRequest
        {
            PlayFabId = Managers.Game.PlayerId,
            InfoRequestParameters = new PlayFab.ClientModels.GetPlayerCombinedInfoRequestParams 
            {
                GetUserAccountInfo = true,
                GetUserInventory = true,
                GetUserData = true,
                GetPlayerStatistics = true,
                GetUserVirtualCurrency = true,
            }
        });
        */
    }
    

    public void UpdateSessionInfo(SessionInfo session)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.UpdateSessionInfo, new PlayFab.ClientModels.UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { PlayerInfo.UserDataKey.SessionInfo.ToString(), session.ToJson() } }
        });

        _executer.Execute(_sender);

        /*
        Request(Command.UpdateSessionInfo, new PlayFab.ClientModels.UpdateUserDataRequest
        { 
            Data = new Dictionary<string, string> { { PlayerInfo.UserDataKey.SessionInfo.ToString(), session.ToJson() } }, 
        });
        */
    }

    public void GetSessionInfo(System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.GetSessionInfo, new PlayFab.ClientModels.GetUserDataRequest{ PlayFabId = Managers.Game.PlayerId, Keys = new List<string>() { PlayerInfo.UserDataKey.SessionInfo.ToString() } }, OnAfterCallback);
        _executer.Execute(_sender);
        /*
        if (OnAfterCallback != null)
            PacketHandler.RegisterHandler(Command.GetSessionInfo , OnAfterCallback);
        Request(Command.GetSessionInfo, new PlayFab.ClientModels.GetUserDataRequest { PlayFabId = Managers.Game.PlayerId , Keys = new List<string>() { PlayerInfo.UserDataKey.SessionInfo.ToString() }});
        */
    }


    public void UpdateUserData(PlayerInfo.UserDataKey [] key, PlayerInfo userdata)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        if (key.ToList().All(x=>userdata.Payload.UserData.ContainsKey(x.ToString()) == false))
                return;

        Dictionary<string, string> privateData = new Dictionary<string, string>();
        Dictionary<string, string> publicData = new Dictionary<string, string>();
        

        foreach (PlayerInfo.UserDataKey dataKey in key)
        {
            if (userdata.Payload.UserData[dataKey.ToString()].Permission == PlayFab.ClientModels.UserDataPermission.Private)
                privateData.Add(dataKey.ToString(), userdata.Payload.UserData[dataKey.ToString()].Value);
            else
                publicData.Add(dataKey.ToString(), userdata.Payload.UserData[dataKey.ToString()].Value);
        }

        if(privateData.Count > 0)
        {
            PlayfabSender _sender = _executer.Pop();
            _sender.Setup(Command.UpdateUserData, new PlayFab.ClientModels.UpdateUserDataRequest { Data = privateData });
            _executer.Execute(_sender);
        }
        if(publicData.Count > 0)
        {
            PlayfabSender _sender = _executer.Pop();
            _sender.Setup(Command.UpdateUserData, new PlayFab.ClientModels.UpdateUserDataRequest { 
                Data = publicData,
                Permission = PlayFab.ClientModels.UserDataPermission.Public
            });
            _executer.Execute(_sender);
        }

        /*
        if (privateData.Count > 0)
            Request(Command.UpdateUserData, new PlayFab.ClientModels.UpdateUserDataRequest
            {
                Data = privateData,
            }) ;

        if(publicData.Count > 0)
            Request(Command.UpdateUserData, new PlayFab.ClientModels.UpdateUserDataRequest
            {
                Data = publicData,
                Permission = PlayFab.ClientModels.UserDataPermission.Public
            });
        */

    }

    public void UpdateStatisticsData(PlayerInfo.StatisticsDataKey [] key , PlayerInfo userdata)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

#if UNITY_EDITOR
        return;
#endif

        if (key.ToList().All(x => userdata.Payload.PlayerStatistics.Any(y=>y.StatisticName == x.ToString()) == false))
            return;

        
      
        List<PlayFab.ClientModels.StatisticUpdate> _updateData = new List<PlayFab.ClientModels.StatisticUpdate>();

        foreach (PlayerInfo.StatisticsDataKey dataKey in key)
        {
            if (userdata.Payload.PlayerStatistics.Any(x=>x.StatisticName == dataKey.ToString()) == false)
                continue;

            PlayFab.ClientModels.StatisticValue _value = userdata.Payload.PlayerStatistics.Where(x => x.StatisticName == dataKey.ToString()).FirstOrDefault();
            _updateData.Add(new PlayFab.ClientModels.StatisticUpdate { StatisticName = dataKey.ToString(), Value = _value.Value  });
        }

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.UpdateStatisticsData, new PlayFab.ClientModels.UpdatePlayerStatisticsRequest { Statistics = _updateData });
        _executer.Execute(_sender);

        /*
        Request(Command.UpdateStatisticsData, new PlayFab.ClientModels.UpdatePlayerStatisticsRequest 
        { 
            Statistics = _updateData 
        });
        */
    }

    public void AddCurrency(PlayerInfo.CurrencyKey key, int amount , PlayerInfo userdata)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        if (userdata.Payload.UserVirtualCurrency.ContainsKey(key.ToString()) == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.AddCurrency, new PlayFab.ClientModels.AddUserVirtualCurrencyRequest { Amount = amount, VirtualCurrency = key.ToString() });
        _executer.Execute(_sender);

        // Request(Command.AddCurrency, new PlayFab.ClientModels.AddUserVirtualCurrencyRequest{ Amount = amount, VirtualCurrency = key.ToString() }); 
    }
    public void SubtractCurrency(PlayerInfo.CurrencyKey key, int amount, PlayerInfo userdata)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        if (userdata.Payload.UserVirtualCurrency.ContainsKey(key.ToString()) == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.SubtractCurrency, new PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest { Amount = amount, VirtualCurrency = key.ToString() });
        _executer.Execute(_sender);
        //Request(Command.SubtractCurrency, new PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest { Amount = amount, VirtualCurrency = key.ToString() });
    }

    public void GrantItems(List<ItemGrant> grantItems, System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.GrantItems, new PlayFab.ServerModels.GrantItemsToUsersRequest { CatalogVersion = CatalogVersion, ItemGrants = grantItems }, OnAfterCallback);
        _executer.Execute(_sender);

        /*
        PacketHandler.RegisterHandler(Command.GrantItems, OnAfterCallback);
        Request(Command.GrantItems, new PlayFab.ServerModels.GrantItemsToUsersRequest { CatalogVersion = CatalogVersion, ItemGrants = grantItems });
        */
    }

    public void ValidatePurchase(string CurrencyCode , uint PurchasePrice , string ReceiptJson , string Signature)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.ValidatePurchase, new PlayFab.ClientModels.ValidateGooglePlayPurchaseRequest { CatalogVersion = CatalogVersion, CurrencyCode = CurrencyCode, PurchasePrice = PurchasePrice, ReceiptJson = ReceiptJson, Signature = Signature });
        _executer.Execute(_sender);
        //Request(Command.ValidatePurchase, new PlayFab.ClientModels.ValidateGooglePlayPurchaseRequest { CatalogVersion = CatalogVersion, CurrencyCode = CurrencyCode, PurchasePrice = PurchasePrice , ReceiptJson = ReceiptJson , Signature = Signature });
    }

    public void PurchaseItem(string ItemId , string Currency , int Price , System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.PurchaseItem, new PlayFab.ClientModels.PurchaseItemRequest { ItemId = ItemId, CatalogVersion = CatalogVersion, VirtualCurrency = Currency, Price = Price }, OnAfterCallback);
        _executer.Execute(_sender);
        /*
        PacketHandler.RegisterHandler(Command.PurchaseItem , OnAfterCallback);
        Request(Command.PurchaseItem, new PlayFab.ClientModels.PurchaseItemRequest { ItemId = ItemId, CatalogVersion = CatalogVersion, VirtualCurrency = Currency, Price = Price });
        */
    }

    public void UpgradeItems(List<ModifyItem> ModifyList ,  System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.UpgradeItems, new PlayFab.ClientModels.ExecuteCloudScriptRequest { FunctionName = Command.UpgradeItems.ToString(), FunctionParameter = new { ItemList = ModifyList.ToArray() }, GeneratePlayStreamEvent = true, SpecificRevision = CloudScriptRevision }, OnAfterCallback);
        _executer.Execute(_sender);

        /*
        PacketHandler.RegisterHandler(Command.UpgradeItems, OnAfterCallback);
        Request(Command.UpgradeItems, new PlayFab.ClientModels.ExecuteCloudScriptRequest { FunctionName = Command.UpgradeItems.ToString() , FunctionParameter = new { ItemList = ModifyList.ToArray() }, GeneratePlayStreamEvent = true , SpecificRevision = CloudScriptRevision  });
        */
    }
    public void UnlockContainerItem(string ItemInstanceId , string KeyInstanceId = null, System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.UnlockContainerItem, new PlayFab.ClientModels.UnlockContainerInstanceRequest { CatalogVersion = CatalogVersion, ContainerItemInstanceId = ItemInstanceId, KeyItemInstanceId = KeyInstanceId != null ? KeyInstanceId : null }, OnAfterCallback);
        _executer.Execute(_sender);
        /*
        PacketHandler.RegisterHandler(Command.UnlockContainerItem, OnAfterCallback);
        Request(Command.UnlockContainerItem, new PlayFab.ClientModels.UnlockContainerInstanceRequest { CatalogVersion = CatalogVersion , ContainerItemInstanceId = ItemInstanceId , KeyItemInstanceId = KeyInstanceId != null ? KeyInstanceId : null});
        */
    }

    public void UnlockContainerItemsWithItemId(string ItemId, System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.UnlockContainerItemWithItemId, new PlayFab.ClientModels.UnlockContainerItemRequest { ContainerItemId = ItemId, CatalogVersion = CatalogVersion }, OnAfterCallback);
        _executer.Execute(_sender);
        /*
        PacketHandler.RegisterHandler(Command.UnlockContainerItemWithItemId, OnAfterCallback);
        Request(Command.UnlockContainerItemWithItemId, new PlayFab.ClientModels.UnlockContainerItemRequest { ContainerItemId = ItemId, CatalogVersion = CatalogVersion });
        */
    }

    public void GetLeaderboard(PlayerInfo.StatisticsDataKey statisticsName , System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.GetLeaderBoard, new PlayFab.ClientModels.GetLeaderboardRequest { MaxResultsCount = 100, StartPosition = 0, StatisticName = statisticsName.ToString(), ProfileConstraints = new PlayFab.ClientModels.PlayerProfileViewConstraints { ShowStatistics = true, ShowDisplayName = true } }, OnAfterCallback);
        _executer.Execute(_sender);

        /*
        PacketHandler.RegisterHandler(Command.GetLeaderBoard, OnAfterCallback);
        Request(Command.GetLeaderBoard, new PlayFab.ClientModels.GetLeaderboardRequest { MaxResultsCount = 100, StartPosition = 0, StatisticName = statisticsName.ToString(), ProfileConstraints = new PlayFab.ClientModels.PlayerProfileViewConstraints { ShowStatistics = true, ShowDisplayName = true } });
        */
    }

    public void GetRankerData(string PlayerId, System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.GetRankerData, new PlayFab.ClientModels.GetUserDataRequest {
            PlayFabId = PlayerId,
            Keys = new List<string>() { PlayerInfo.UserDataKey.DPS.ToString() }}, OnAfterCallback);
        _executer.Execute(_sender);

        /*
        PacketHandler.RegisterHandler(Command.GetRankerData, OnAfterCallback);
        Request(Command.GetRankerData, new PlayFab.ClientModels.GetUserDataRequest
        {
            PlayFabId = PlayerId,
            Keys = new List<string>() { PlayerInfo.UserDataKey.DPS.ToString() }
          
        });
        */
        //Request(Command.GetRankerData, new PlayFab.ClientModels.GetLeaderboardRequest { MaxResultsCount = 100, StartPosition = 0, StatisticName = statisticsName.ToString() });
    }

    public void ConsumeItem(string ItemInstanceId , int ConsumeCount = 1 , System.Action<PlayFabResultCommon> OnAfterCallback = null)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.ConsumeItem, new PlayFab.ClientModels.ConsumeItemRequest
        {
            ItemInstanceId = ItemInstanceId,
            ConsumeCount = ConsumeCount
        }, OnAfterCallback);
        _executer.Execute(_sender);

        /*
        PacketHandler.RegisterHandler(Command.ConsumeItem, OnAfterCallback);
        Request(Command.ConsumeItem, new PlayFab.ClientModels.ConsumeItemRequest
        {
           ItemInstanceId = ItemInstanceId , 
            ConsumeCount = ConsumeCount
        });
        */
    }

    public void UpdateItemCustomData(string ItemInstanceId, Dictionary<string,string> Data)
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.UpdateItemCustomData, new PlayFab.ServerModels.UpdateUserInventoryItemDataRequest
        {
            PlayFabId = Managers.Game.PlayerId,
            ItemInstanceId = ItemInstanceId,
            Data = Data
        });
        _executer.Execute(_sender);

        /*
        Request(Command.UpdateItemCustomData, new PlayFab.ServerModels.UpdateUserInventoryItemDataRequest
        {
            PlayFabId = Managers.Game.PlayerId,
            ItemInstanceId = ItemInstanceId,
             Data = Data
        });
        */
    }

    public void GetCatalogItems()
    {
        if (IS_ENABLE_NETWORK == false)
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.GetCatalogItems, new PlayFab.ClientModels.GetCatalogItemsRequest());
        _executer.Execute(_sender);

        //Request(Command.GetCatalogItems, new PlayFab.ClientModels.GetCatalogItemsRequest());
    }

    public void DeletePlayerFromTitle(string PlayfabId)
    {
        if (IS_ENABLE_NETWORK == false)
            return;
        if (string.IsNullOrEmpty(PlayfabId))
            return;

        PlayfabSender _sender = _executer.Pop();
        _sender.Setup(Command.DeletePlayer, new PlayFab.ClientModels.ExecuteCloudScriptRequest { FunctionName = Command.DeletePlayer.ToString() , GeneratePlayStreamEvent = true, SpecificRevision = CloudScriptRevision });

        _executer.Execute(_sender);
    }
    public int GetWaitCommandQueueCount()
    {
        return _executer.GetWaitCommandQueueCount();
    }
    public void OnUpdate()
    {
        /*
        _time += Time.deltaTime;
        if (_time > _delay)
        {
            _time = 0.0f;
            if (_pendingCallsQueue.Count <= MaxCallsAPICountFor && _waitCallsQueue.Count > 0)
            {
                Execute();
            }
           
        }
        */

      
        
    }



}
