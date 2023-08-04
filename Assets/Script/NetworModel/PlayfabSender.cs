using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using PlayFab.ServerModels;

public class PlayfabSender
{
    public enum Status
    {
        InActive,
        Request,
        Exit
    }

    public Status status { get; private set; } = Status.InActive;

    private PlayFabRequestCommon request;
    private NetworkManager.Command command;
    private System.Action<PlayFabResultCommon> customHandler = null;
    private int RetryCount = 0;



    public void Setup(NetworkManager.Command command, PlayFabRequestCommon request, System.Action<PlayFabResultCommon> customHandler = null)
    {
        this.command = command;
        this.request = request;
        this.customHandler = customHandler;

        status = Status.InActive;

    }

    public void Execute()
    {
        RetryCount = 0;
        status = Status.Request;
        Send(command, request, customHandler);
    }


    void Send(NetworkManager.Command command, PlayFabRequestCommon request, System.Action<PlayFabResultCommon> customHandler = null)
    {
      

        switch (command)
        {
            case NetworkManager.Command.Login:
            case NetworkManager.Command.Register:
            case NetworkManager.Command.UpdateEntityToken:
#if UNITY_EDITOR
                PlayFabClientAPI.LoginWithCustomID(request as LoginWithCustomIDRequest, (result) => OnHandle(command, result), error => OnHandleError(error));
#elif UNITY_ANDROID
                PlayFabClientAPI.LoginWithGooglePlayGamesServices(request as LoginWithGooglePlayGamesServicesRequest, result => OnHandle(command , result) , error => OnHandleError(error));
#endif
                break;
            case NetworkManager.Command.UpdateDisplayName:
                PlayFabClientAPI.UpdateUserTitleDisplayName(request as UpdateUserTitleDisplayNameRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.GetTime:
                PlayFabClientAPI.GetTime(request as PlayFab.ClientModels.GetTimeRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;

            case NetworkManager.Command.GetPlayerInfo:
            case NetworkManager.Command.GetTitleAndProfile:

                PlayFabClientAPI.GetPlayerCombinedInfo(request as PlayFab.ClientModels.GetPlayerCombinedInfoRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.UpdateSessionInfo:
            case NetworkManager.Command.UpdateUserData:
                PlayFabClientAPI.UpdateUserData(request as PlayFab.ClientModels.UpdateUserDataRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.UpdateStatisticsData:
                PlayFabClientAPI.UpdatePlayerStatistics(request as PlayFab.ClientModels.UpdatePlayerStatisticsRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.GrantItems:
                PlayFabServerAPI.GrantItemsToUsers(request as PlayFab.ServerModels.GrantItemsToUsersRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.AddCurrency:
                PlayFabClientAPI.AddUserVirtualCurrency(request as PlayFab.ClientModels.AddUserVirtualCurrencyRequest, result => OnHandle(command, result),
                    error => OnHandleError(error));
                break;
            case NetworkManager.Command.SubtractCurrency:
                PlayFabClientAPI.SubtractUserVirtualCurrency(request as PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest, result => OnHandle(command, result),
                    error => OnHandleError(error));
                break;
            case NetworkManager.Command.ValidatePurchase:
                PlayFabClientAPI.ValidateGooglePlayPurchase(request as PlayFab.ClientModels.ValidateGooglePlayPurchaseRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.PurchaseItem:
                PlayFabClientAPI.PurchaseItem(request as PlayFab.ClientModels.PurchaseItemRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.UnlockContainerItem:
                Debug.Log(request.ToJson());
                PlayFabClientAPI.UnlockContainerInstance(request as PlayFab.ClientModels.UnlockContainerInstanceRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.UnlockContainerItemWithItemId:
                PlayFabClientAPI.UnlockContainerItem(request as PlayFab.ClientModels.UnlockContainerItemRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.GetLeaderBoard:
                PlayFabClientAPI.GetLeaderboard(request as PlayFab.ClientModels.GetLeaderboardRequest, result => OnHandle(command, result), error => { OnHandleError(error); });
                break;
            case NetworkManager.Command.UpgradeItems:
                PlayFabClientAPI.ExecuteCloudScript(request as PlayFab.ClientModels.ExecuteCloudScriptRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.GetSessionInfo:
            case NetworkManager.Command.GetRankerData:
                PlayFabClientAPI.GetUserData(request as PlayFab.ClientModels.GetUserDataRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.ConsumeItem:
                PlayFabClientAPI.ConsumeItem(request as PlayFab.ClientModels.ConsumeItemRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.UpdateItemCustomData:
                PlayFabServerAPI.UpdateUserInventoryItemCustomData(request as PlayFab.ServerModels.UpdateUserInventoryItemDataRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.GetCatalogItems:
                PlayFabClientAPI.GetCatalogItems(request as PlayFab.ClientModels.GetCatalogItemsRequest, result => OnHandle(command, result), error => OnHandleError(error));
                break;
            case NetworkManager.Command.DeletePlayer:
                PlayFabClientAPI.ExecuteCloudScript(request as PlayFab.ClientModels.ExecuteCloudScriptRequest, result => OnHandle(command, result), error => OnHandleError(error));
                
                break;

        }

    }

    void OnHandle(NetworkManager.Command command, PlayFabResultCommon result)
    {
        PacketHandler.RegisterHandler(command, customHandler);
        PacketHandler.OnHandle(command, result);
        status = Status.Exit;

    }

    void OnHandleError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
        Debug.Log(error.Error);
        switch (error.Error)
        {
            case PlayFabErrorCode.ConnectionError:
                break;
            case PlayFabErrorCode.InvalidContainerItem:
                Debug.Log(error.ErrorMessage);
                Debug.Log(error.HttpCode);
                Debug.Log(error.HttpStatus);

                if (RetryCount < 3)
                {
                    RetryCount++;
                    Send(command, request, customHandler);
                }
                else
                {
                    status = Status.Exit;
                    Debug.Log("Service Quit");
                    Application.Quit();
                }
                return;

                break;
            case PlayFabErrorCode.InvalidReceipt:
                break;
            case PlayFabErrorCode.ReceiptAlreadyUsed:
                break;
            // 재시도 가능////////////////////////////////////////////////////////////////
            case PlayFabErrorCode.APIClientRequestRateLimitExceeded:        // Indicates too many calls in a short burst.
            case PlayFabErrorCode.APIConcurrentRequestLimitExceeded:        // Indicates too many simultaneous calls.
            case PlayFabErrorCode.ConcurrentEditError:                      // Indicates too many simultaneous calls or very rapid sequential calls.
            case PlayFabErrorCode.DataUpdateRateExceeded:                   // Indicates too many simultaneous calls, or very rapid sequential calls.
            case PlayFabErrorCode.DownstreamServiceUnavailable:             // Indicates that PlayFab or a third-party service might be having a temporary issue.
            case PlayFabErrorCode.InvalidAPIEndpoint:                       // Indicates that the URL for this request is not valid for this title.
            case PlayFabErrorCode.OverLimit:                                 // Indicates that an attempt to perform an operation will cause the service usage to go over the limit as shown in the Game Manager limit pages. Evaluate the returned error details to determine which limit would be exceeded.
            case PlayFabErrorCode.ServiceUnavailable:                       // Indicates that PlayFab may be having a temporary issue or the client is making too many API calls too quickly.
                if (RetryCount < 3)
                {
                    RetryCount++;
                    Send(command, request, customHandler);
                }
                else
                {
                    status = Status.Exit;
                    Debug.Log("Service Quit");
                    Application.Quit();
                }
                return;
                break;
            ///////////////////////////////////////////////////////////////////////////////
            case PlayFabErrorCode.AccountNotFound:
                {

                    UI_Title title = UnityEngine.Object.FindObjectOfType<UI_Title>();
                    if (title != null)
                        title.ShowRegisterButton();

                    return;
                }
                break;
            case PlayFabErrorCode.AccountDeleted:
                {

                    UI_Title title = UnityEngine.Object.FindObjectOfType<UI_Title>();
                    if (title != null)
                        title.ShowRegisterButton();

                    return;
                }
                break;

        }

#if ENABLE_LOG

        UI_LogTest _showLog = UnityEngine.Object.FindObjectOfType<UI_LogTest>();
        if (_showLog == null)
            Managers.UI.ShowPopupUI<UI_LogTest>().Setup("Errorcode : " + error.Error + System.Environment.NewLine + error.GenerateErrorReport());
        else
            _showLog.AddLog(error.GenerateErrorReport());
#endif

    }
}
