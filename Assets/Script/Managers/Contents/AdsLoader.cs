using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;



public class AdsLoader : MonoBehaviour //, IUnityAdsInitializationListener, IUnityAdsLoadListener , IUnityAdsShowListener
{
    Action _rewardCallback = null;
    // ironsource
    private void Start()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

        /*
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        */
        
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        


        IronSource.Agent.shouldTrackNetworkState(true);



    }
    private void OnApplicationPause(bool pause)
    {
        IronSource.Agent.onApplicationPause(pause);
    }
    void SdkInitializationCompletedEvent()
    {
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent");

       
    }

    void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
    {
        Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
    }

    void RewardedVideoAdOpenedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
    }

    void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
    {
        Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());

        _rewardCallback?.Invoke();
        _rewardCallback = null;

    }

    void RewardedVideoAdClosedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
    }

    void RewardedVideoAdStartedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
    }

    void RewardedVideoAdEndedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
    }

    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        _rewardCallback = null;
    }

    void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
    {
        Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
    }



    private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {

    }
    private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        _rewardCallback?.Invoke();
        _rewardCallback = null;
    }
    private void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        _rewardCallback = null;
    }

    private void RewardedVideoOnAdUnavailable()
    {

    }
    private void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {

    }
    private void RewardedVideoAdClosedEvent(IronSourceAdInfo adInfo)
    {
        
    }

    private void RewardedVideoAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        
    }

    public bool ShowRewardVd(Action callback)
    {
        bool available = IronSource.Agent.isRewardedVideoAvailable();
        if (available)
        {
           
            _rewardCallback = callback;
            IronSource.Agent.showRewardedVideo();
            return true;
        }
        Debug.Log("Ad Showing is false");
        return false;

    }


    /*
string[] _unityads = { "PurchaseRune", "PurchasePet", "PurchaseBow", "PurchaseArmor", "PurchaseHelmet", "PurchaseCloak", "PurchaseHeart" };
string[] _admob = { "BuffAuto", "BuffAttack", "BuffAttackSpeed", "BuffSkill", "BuffGoldActRate" , "Revive" };

Dictionary<string, bool> _listReadyAd = new Dictionary<string, bool>();
Dictionary<string, RewardedAd> _listAdmob = new Dictionary<string, RewardedAd>();
Dictionary<string, string> _adUnit = new Dictionary<string, string>();

bool _processReward = false;

Action _handlerOnAdRewarded = null;

public event Action OnSdkInitializationCompletedEvent;

public void OnInitializationComplete()
{
   foreach (string id in _unityads)
   {

       _listReadyAd.Add(id, false);
       Advertisement.Load(id,this);
   }
}

public void OnInitializationFailed(UnityAdsInitializationError error, string message)
{
   Debug.LogError("UnityAds InitializationFailed");
}

public void OnUnityAdsAdLoaded(string placementId)
{
   if (_listReadyAd.ContainsKey(placementId))
       _listReadyAd[placementId] = true;
   else
       _listReadyAd.Add(placementId, true);
}

public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
{

   _listReadyAd[placementId] = false;
}

public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
{

}

public void OnUnityAdsShowStart(string placementId)
{

}

public void OnUnityAdsShowClick(string placementId)
{

}

public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
{

   if(showCompletionState == UnityAdsShowCompletionState.SKIPPED)
   {

   }
   else if(showCompletionState == UnityAdsShowCompletionState.UNKNOWN)
   {

   }
   else if(showCompletionState == UnityAdsShowCompletionState.COMPLETED)
   {
       _processReward = true;
   }
   _listReadyAd[placementId] = false;
   Advertisement.Load(placementId,this);

}






// Start is called before the first frame update
void Start()
{
   InitUnityAds();
   InitGoogleAdMob();

   _adUnit.Add("Buff_Auto_AD", "BuffAuto");
   _adUnit.Add("Buff_Attack_AD", "BuffAttack");
   _adUnit.Add("Buff_AttackSpeed_AD", "BuffAttackSpeed");
   _adUnit.Add("Buff_GoldDropRate_AD", "BuffGoldActRate");
   _adUnit.Add("Buff_Skill_AD", "BuffSkill");


   _adUnit.Add("Rune_Randombox_10", "PurchaseRune");
   _adUnit.Add("Pet_Randombox_10", "PurchasePet");
   _adUnit.Add("Bow_Randombox_10", "PurchaseBow");
   _adUnit.Add("Helmet_Randombox_10", "PurchaseHelmet");
   _adUnit.Add("Cloak_Randombox_10", "PurchaseCloak");
   _adUnit.Add("Armor_Randombox_10", "PurchaseArmor");
   _adUnit.Add("Heart_Randombox_10", "PurchaseHeart");

   _adUnit.Add("Revive", "Revive");

}
void InitUnityAds()
{
   Debug.Log("UnityAds Init");
   Advertisement.Initialize("5024263", true , this);



}
void InitGoogleAdMob()
{
   MobileAds.Initialize(initStatus => {

       foreach (string id in _admob)
       {
           _listReadyAd.Add(id, false);
           _listAdmob.Add(id, CreateAndLoadRewardedAd(id));

           AdRequest request = new AdRequest.Builder().Build();
           _listAdmob[id].LoadAd(request);


       }

   });
}
public RewardedAd CreateAndLoadRewardedAd(string AdUnitId)
{
   RewardedAd rewardedAd;
   rewardedAd = new RewardedAd("ca-app-pub-3940256099942544/5224354917");


   rewardedAd.OnAdLoaded += OnLoadedAd;
   rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
   rewardedAd.OnAdFailedToLoad += OnLoadedFailed;
   rewardedAd.OnAdClosed += OnAdClosed;



   return rewardedAd;
}

public void OnLoadedAd(object sender, EventArgs args)
{
   if(_listAdmob.ContainsValue((RewardedAd)sender))
   {
       string key = _listAdmob.Where(x => x.Value == (RewardedAd)sender).First().Key;
       if (_listReadyAd.ContainsKey(key))
           _listReadyAd[key] = true;
       else
           _listReadyAd.Add(key, true);
   }

}
public void OnAdClosed(object sender, EventArgs args)
{

}
public void OnLoadedFailed(object sender, EventArgs args)
{

   if (_listAdmob.ContainsValue((RewardedAd)sender))
   {
       string key = _listAdmob.Where(x => x.Value == (RewardedAd)sender).First().Key;
       if (_listReadyAd.ContainsKey(key))
           _listReadyAd[key] = false;
       else
           _listReadyAd.Add(key, false);


       _listAdmob[key].Destroy();
       _listAdmob.Remove(key);


       _listAdmob.Add(key, CreateAndLoadRewardedAd(key));
       AdRequest request = new AdRequest.Builder().Build();
       _listAdmob[key].LoadAd(request);


   }
}
public void OnAdFailedToShow(object sender, EventArgs args)
{


}
public void OnUserEarnedReward(object sender, EventArgs args)
{
   if (_listAdmob.ContainsValue((RewardedAd)sender))
   {
       string key = _listAdmob.Where(x => x.Value == (RewardedAd)sender).First().Key;

       _listReadyAd[key] = false;

       _listAdmob[key] = CreateAndLoadRewardedAd(key);
       AdRequest request = new AdRequest.Builder().Build();
       _listAdmob[key].LoadAd(request);

       _processReward = true;


   }
}

public bool ShowAd(string AdUnitId , Action callback)
{

   if (_listReadyAd.ContainsKey(_adUnit[AdUnitId]) && _listReadyAd[_adUnit[AdUnitId]])
   {
       _handlerOnAdRewarded = callback;
       if(_unityads.Any(x=>x == _adUnit[AdUnitId]))
       {
           Advertisement.Show(_adUnit[AdUnitId], this);
           return true;
       }
       else if(_admob.Any(x=>x == _adUnit[AdUnitId]))
       {
           _listAdmob[_adUnit[AdUnitId]].Show();
           return true;
       }
   }
   return false;

}

// Update is called once per frame
void Update()
{
   if(_processReward)
   {
       _handlerOnAdRewarded?.Invoke();
       _handlerOnAdRewarded = null;
       _processReward = false;
   }
}
*/


}
