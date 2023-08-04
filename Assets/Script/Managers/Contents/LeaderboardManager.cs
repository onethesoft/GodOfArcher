using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using System.Linq;
using System;
public class LeaderboardManager 
{

    List<PlayerLeaderboardEntry> _rankerList;
    PlayerLeaderboardEntry _playerEntry;

    DateTime? _lastUpdatedTime = null;

    const double UpdateMinutes = 30.0;

    public void Init()
    {
        _rankerList = new List<PlayerLeaderboardEntry>();
    }

    public void AddLeaderboardEntry(PlayerLeaderboardEntry entry)
    {
        if(_rankerList.Any(x=>x.Position == entry.Position))
        {
            _rankerList.RemoveAll(x => x.Position == entry.Position);
            _rankerList.Add(entry);

        }
        else
            _rankerList.Add(entry);
        
    }

    public List<PlayerLeaderboardEntry> GetLeaderboardList()
    {
        _rankerList.Sort(CompareLeaderboardEntry);
        return _rankerList.Where(x => x.Position < 15).ToList();

    }

    private int CompareLeaderboardEntry(PlayerLeaderboardEntry x, PlayerLeaderboardEntry y)
    {
        return x.Position.CompareTo(y.Position);
    }
    public void SetPlayerEntry(PlayerLeaderboardEntry playerEntry)
    {
        _playerEntry = playerEntry;
    }

    public void UpdateLeaderboard()
    {
        _lastUpdatedTime = GlobalTime.Now;
        Managers.Network.GetLeaderboard(PlayerInfo.StatisticsDataKey.MaxClearStage, (result) => {

            List<PlayFab.ClientModels.PlayerLeaderboardEntry> _ranker = GetLeaderboardList().Where(x => x.Position < 10).ToList();
            _ranker.ForEach(x => {
                if (x.PlayFabId != Managers.Game.PlayerId)
                    Managers.Network.GetRankerData(x.PlayFabId);
            });



            
        });
    }
    public void OnUpdate()
    {
        if(_lastUpdatedTime.HasValue)
        {
            if((GlobalTime.Now - _lastUpdatedTime).Value.TotalMinutes >= UpdateMinutes)
            {
                UpdateLeaderboard();
            }
        }
    }
}
