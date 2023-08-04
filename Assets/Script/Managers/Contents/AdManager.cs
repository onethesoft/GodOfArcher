using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GoogleMobileAds.Api;
using System;
using UnityEngine.Advertisements;

public class AdManager 
{
    AdsLoader _adPlayer;
    public void Init()
    {
        GameObject go = GameObject.Find("AdsLoader");
        if (go == null)
        {
            go = new GameObject { name = "AdsLoader" };
            _adPlayer = go.AddComponent<AdsLoader>();

        }
        UnityEngine.Object.DontDestroyOnLoad(go);
      
    }

    public bool ShowAd(string id , Action callback)
    {
        return _adPlayer.ShowRewardVd(callback);
        //return _adPlayer.ShowAd(id, callback);
    }
}

