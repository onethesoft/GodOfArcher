
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PlayFab;



public class TitleScene : BaseScene
{
    UI_Title _sceneUI;
    protected override void Init()
    {
        base.Init();

        _sceneUI = FindObjectOfType<UI_Title>();
        Managers.Setting.Play("TitleBGM", Define.Sound.Bgm);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //PlayFab.PlayFabSettings.staticSettings.TitleId = "E576D";

        SceneType = Define.Scene.Unknown ;



        // recommended for debugging:
        //


        // Activate the Google Play Games platform

        


    }
    public override void Clear()
    {
        
    }

    public override UI_Scene GetSceneUI()
    {
        return _sceneUI;
    }

    
}
