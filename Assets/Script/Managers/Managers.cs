using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_Instance = null;

    public static Managers Instance { get { Init(); return s_Instance; } }
    public static string Name { get { return "@Managers"; } }
    GameManagerEx _game = new GameManagerEx();
    QuestManager _quest = new QuestManager();
    ItemManager _item = new ItemManager();
    ShopManager _shop = new ShopManager();
    SettingManager _setting = new SettingManager();
    AdManager _ad = new AdManager();
    NetworkManager _network = new NetworkManager();
    PlayerManager _player = new PlayerManager();
    LeaderboardManager _ranking = new LeaderboardManager();

    public static GameManagerEx Game { get { return Instance._game; } }
    public static QuestManager Quest { get { return Instance._quest; } }
    public static ItemManager Item { get { return Instance._item; } }
    public static ShopManager Shop { get { return Instance._shop; } }
    public static SettingManager Setting { get { return Instance._setting; } }
    public static AdManager Ad { get { return Instance._ad; } }
    public static NetworkManager Network { get { return Instance._network; } }
    public static PlayerManager Player { get { return Instance._player; } }
    public static LeaderboardManager Ranking { get { return Instance._ranking; } }

    #region Core
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    UIManager _ui = new UIManager();
    DataManager _data = new DataManager();
    JobManager _job = new JobManager();
    AnalyticsManager _analytics = new AnalyticsManager();


    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static DataManager Data { get { return Instance._data; } }
    public static JobManager Job { get { return Instance._job; } }
    public static AnalyticsManager Analytics { get { return Instance._analytics; } }


    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    private void Update()
    {
        if(s_Instance != null)
        {
            s_Instance._game.OnUpdate();
            s_Instance._network.OnUpdate();
            s_Instance._input.OnUpdate();
            s_Instance._ranking.OnUpdate();
            //_input.OnUpdate();

            //_ranking.OnUpdate();
        }


    }
    static void Init()
    {
        if (s_Instance == null)
        {
            GameObject go = GameObject.Find(Name);
            if (go == null)
            {
                go = new GameObject { name = Name };
                go.AddComponent<Managers>();

            }
            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<Managers>();


            s_Instance._analytics.Init();
            s_Instance._ui.Init();
            s_Instance._setting.Init();
            s_Instance._data.Init();
            s_Instance._job.Init();
            s_Instance._pool.Init();
            s_Instance._quest.Init();
            s_Instance._item.Init();
            s_Instance._shop.Init();
            s_Instance._game.Init();
            s_Instance._ad.Init();
            s_Instance._network.Init();
            s_Instance._player.Init();
            s_Instance._ranking.Init();






            DOTween.Init();
        }
        
        
    }
    
    public static void Clear()
    {
       
        Input.Clear();
        Pool.Clear();
        Scene.Clear();
        UI.Clear();
        Game.Clear();
        

    }



}
