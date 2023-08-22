using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataUpdater : MonoBehaviour
{
    Coroutine _dataCoroutine = null;
    Coroutine _itemCoroutine;
    Coroutine _sessionCoroutine;
    Coroutine _updateCoroutine = null;

    WaitForSeconds _waitForData = new WaitForSeconds(240.0f);   // 4분
    WaitForSecondsRealtime _waitForItem = new WaitForSecondsRealtime(1.0f);    // 1초
    WaitForSecondsRealtime _waitForSession = new WaitForSecondsRealtime(1800.0f);    // 30분
    GameData _data = null;

    bool isDestroyed = false;
    bool isUpdateDailyQuest = false;
    bool isUpdatingEntityToken = false;

    DateTime _sessiontime;
    DateTime _currentDate;
    const double sessionCheckDelay = 4.0;

    List<BaseItem> _removeItems = new List<BaseItem>();


    public void Setup(GameData gameData)
    {
        _data = gameData;
     
    }
    private void OnEnable()
    {
       
        if (_updateCoroutine == null)
            _updateCoroutine = StartCoroutine(UpdateCoroutine());
    }

    IEnumerator UpdateData()
    {
        while (!isDestroyed)
        {
            yield return _waitForData;
            if (Managers.Network.IS_ENABLE_NETWORK == true)
                Managers.Network.GetSessionInfo(
                    (result) =>
                    {
                        PlayFab.ClientModels.GetUserDataResult _session = result as PlayFab.ClientModels.GetUserDataResult;

                        SessionInfo _serverSession = SessionInfo.FromJson(_session.Data[PlayerInfo.UserDataKey.SessionInfo.ToString()].Value);
                        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Session.DeviceId != _serverSession.DeviceId)
                        {
                            Debug.Log("다중 접속 금지");
                            StopCoroutine(_dataCoroutine);
                            StopCoroutine(_itemCoroutine);
                            UI_Messagebox _messagebox = Managers.UI.ShowPopupUI<UI_Messagebox>();
                            _messagebox.mode = UI_Messagebox.Mode.OK;
                            _messagebox.Text = "다중 접속 금지";
                            _messagebox.OK += () => {
                                Application.Quit();
                            };

                            Managers.Game.AutoForTest = false;


                            /*
                            UI_LogTest _log = FindObjectOfType<UI_LogTest>();
                            if (_log == null)
                            {
                                _log = Managers.UI.ShowPopupUI<UI_LogTest>();
                                _log.Setup("다중 접속 금지");
                            }
                            else
                            {
                                _log.AddLog("다중 접속 금지");
                            }
                            */
                            return;
                        }

                        // 자정 12시 체크해서 업데이트
                        if(_serverSession.LastAccessTime.ToLocalTime().Date < GlobalTime.Now.ToLocalTime().Date)
                        {
                            _data.ResetQuest();
                            _data.ResetStage();
                        }

                        Managers.Player.GetPlayer(Managers.Game.PlayerId).Session.LastAccessTime = GlobalTime.Now;
                        Managers.Network.UpdateSessionInfo(Managers.Player.GetPlayer(Managers.Game.PlayerId).Session);

                        Managers.Game.Save(new PlayerInfo.StatisticsDataKey[] { PlayerInfo.StatisticsDataKey.Level, PlayerInfo.StatisticsDataKey.Stage, PlayerInfo.StatisticsDataKey.ClearStage, PlayerInfo.StatisticsDataKey.MaxClearStage });
                        Managers.Game.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.Gold , PlayerInfo.UserDataKey.DPS});
                        Managers.Quest.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.MainQuest, PlayerInfo.UserDataKey.SubQuest });

                    });
        }

    }
    IEnumerator UpdateCoroutine()
    {
        _sessiontime = GlobalTime.Now;
        while (!isDestroyed)
        {
            _currentDate = GlobalTime.Now;

            // UpdateDaily  와 UpdateUserData 가 한 프레임 내에 실행될 때 SubQuest 를 두번 Update하지 않기 위함.
            isUpdateDailyQuest = false;

            yield return _waitForItem;

            UpdateInventory();
            UpdateDaily();
            UpdateUserData();
            UpdateEntityExpiration();
        }
    }

    
    IEnumerator UpdateItem()
    {
        List<BaseItem> _removeItems = new List<BaseItem>();
        while (!isDestroyed)
        {
            yield return _waitForItem;

            foreach (BaseItem item in _data.Inventory.FindAll(x => x.Expiration.HasValue))
            {
                if (item.Expiration.Value.ToLocalTime() <= GlobalTime.Now.ToLocalTime())
                {
                    Debug.Log("GameUpdater : " + item.ItemId + " , " + item.Expiration.Value);
                    _removeItems.Add(item);
                }
            }

            if (_removeItems.Count > 0)
            {
                foreach (BaseItem item in _removeItems)
                {
                    _data.Inventory.RemoveItem(item.ItemId);
                    if (Managers.Network.IS_ENABLE_NETWORK == true && item is Buff)     // 광고 보상 버프일때만 수동으로 제거한다.
                    {
                        Debug.Log("removeITem : " + item.ItemId);
                        Managers.Network.ConsumeItem(item.ItemInstanceId, item.RemainingUses.GetValueOrDefault());
                    }
                }
                _removeItems.Clear();


            }



        }
    }

    void UpdateEntityExpiration()
    {
        

        if ((Managers.Player.GetPlayer(Managers.Game.PlayerId).LoginResult.EntityToken.TokenExpiration.GetValueOrDefault().ToLocalTime() - GlobalTime.Now.ToLocalTime()).TotalMinutes <= 60)
        {
            if (isUpdatingEntityToken == true)
                return;

            Debug.Log("UpdateEntityToken");

            Managers.Network.UpdateEntityToken((result)=> {
                isUpdatingEntityToken = false;
            });

            isUpdatingEntityToken = true;
        }
    }
    void UpdateInventory()
    {
       
       foreach(BaseItem item in _data.Inventory.ToList())
        {
            if(item.Expiration.HasValue)
            {
                if (item.Expiration.Value.ToLocalTime() <= GlobalTime.Now.ToLocalTime())
                {
                    Debug.Log("GameUpdater : " + item.ItemId + " , " + item.Expiration.Value);
                    _removeItems.Add(item);
                }
            }
        }
        if (_removeItems.Count > 0)
        {
            foreach (BaseItem item in _removeItems)
            {
                _data.Inventory.RemoveItem(item.ItemId);
                if (Managers.Network.IS_ENABLE_NETWORK == true && item is Buff)     // 광고 보상 버프일때만 수동으로 제거한다.
                {
                    Debug.Log("removeITem : " + item.ItemId);
                    Managers.Network.ConsumeItem(item.ItemInstanceId, item.RemainingUses.GetValueOrDefault());
                }
            }
            _removeItems.Clear();


        }
        

        
    }

    

    void UpdateUserData()
    {
      
        if ((GlobalTime.Now - _sessiontime).TotalMinutes > sessionCheckDelay)
        {
            _sessiontime = GlobalTime.Now;

            if (Managers.Network.IS_ENABLE_NETWORK == true)
                Managers.Network.GetSessionInfo(
                    (result) =>
                    {
                        PlayFab.ClientModels.GetUserDataResult _session = result as PlayFab.ClientModels.GetUserDataResult;

                        SessionInfo _serverSession = SessionInfo.FromJson(_session.Data[PlayerInfo.UserDataKey.SessionInfo.ToString()].Value);
                        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Session.DeviceId != _serverSession.DeviceId)
                        {
                            isDestroyed = true;

                            Debug.Log("다중 접속 금지");
                     
                            UI_LogTest _log = FindObjectOfType<UI_LogTest>();
                            if (_log == null)
                            {
                                _log = Managers.UI.ShowPopupUI<UI_LogTest>();
                                _log.Setup("다중 접속 금지");
                            }
                            else
                            {
                                _log.AddLog("다중 접속 금지");
                            }
                            return;
                        }

                        

                        Managers.Player.GetPlayer(Managers.Game.PlayerId).Session.LastAccessTime = GlobalTime.Now;
                        Managers.Network.UpdateSessionInfo(Managers.Player.GetPlayer(Managers.Game.PlayerId).Session);

                        Managers.Game.Save(new PlayerInfo.StatisticsDataKey[] { PlayerInfo.StatisticsDataKey.Level, PlayerInfo.StatisticsDataKey.Stage, PlayerInfo.StatisticsDataKey.ClearStage, PlayerInfo.StatisticsDataKey.MaxClearStage });

#if !ENABLE_LOG
                        if(_data.ReviveInfo.DoReset())
                            Managers.Game.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.Gold, PlayerInfo.UserDataKey.DPS , PlayerInfo.UserDataKey.ReviveInfo });
                        else
                            Managers.Game.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.Gold, PlayerInfo.UserDataKey.DPS });
#else

                        if (_data.ReviveInfo.DoReset())
                            Managers.Game.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.Gold,  PlayerInfo.UserDataKey.ReviveInfo });
                        else
                            Managers.Game.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.Gold });
#endif


                        Managers.Quest.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.MainQuest , PlayerInfo.UserDataKey.SubQuest });


                    });
        }


    }

    public void ManualUpdateUser()
    {
        if (PlayerPrefs.HasKey("UpdateTime"))
        {
            DateTime? value = Util.TryParseDateTime(PlayerPrefs.GetString("UpdateToServer"));
            if ((GlobalTime.Now - value.GetValueOrDefault()).TotalSeconds <= 30.0)
                return;
        }

        Managers.Game.Save(new PlayerInfo.StatisticsDataKey[] { PlayerInfo.StatisticsDataKey.Level, PlayerInfo.StatisticsDataKey.Stage, PlayerInfo.StatisticsDataKey.ClearStage, PlayerInfo.StatisticsDataKey.MaxClearStage });
        Managers.Game.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.Gold, PlayerInfo.UserDataKey.DPS });
        Managers.Quest.Save(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.MainQuest, PlayerInfo.UserDataKey.SubQuest });

        PlayerPrefs.SetString("UpdateTime", Util.GetTimeString(GlobalTime.Now));
        PlayerPrefs.Save();

        // 혹시나 모를 UpdateUserData 와의 충돌을 막기 위함.
        _sessiontime.AddSeconds(30.0);
    }
    void UpdateDaily()
    {
        if (_currentDate.ToLocalTime().Date < GlobalTime.Now.ToLocalTime().Date)
        {
            _data.ResetQuest();
            _data.ResetStage();
            isUpdateDailyQuest = true;
        }
    }

    public void StopUpdate()
    {
        if (_updateCoroutine != null)
            StopCoroutine(_updateCoroutine);
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }
}
