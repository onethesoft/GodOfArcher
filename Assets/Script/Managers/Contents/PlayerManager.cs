using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    Dictionary<string, PlayerInfo> _playerList;
    public void Init()
    {
        _playerList = new Dictionary<string, PlayerInfo>();
    }

    public void AddPlayer(string id , PlayerInfo info)
    {
        if (_playerList.ContainsKey(id))
            _playerList[id] = info;
        else
            _playerList.Add(id, info);
    }
    public PlayerInfo GetPlayer(string id)
    {
        if (_playerList.ContainsKey(id) == false)
            return null;

        return _playerList[id];
    }
    
}
