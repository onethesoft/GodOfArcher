using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainScene : BaseScene
{
    GameObject _background;
    UI_Main _sceneUI;
    StageTask _sceneTask;

    float _ClearTime;

    float _Load = 0.0f;
    float _RunStage = 0.0f;
    float _ProcessReward = 0.0f;
    float _End = 0.0f;

    [SerializeField]
    List<GameObject> _spawnPos;

   

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Main;

        Managers.Setting.Play("MainBGM", Define.Sound.Bgm);
        _sceneUI = Managers.UI.ShowSceneUI<UI_Main>();

        _sceneUI.IsBeginner = Managers.Game.BeforeBeginPlay();

        //Managers.Resource.Instantiate("UI/UI_DebugPopup", _sceneUI.transform);

        Managers.Game.CreatePlayer();

        _sceneTask = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.Main).FirstOrDefault();
        _background = _sceneTask.GetBackground(Managers.Game.Stage);
        Managers.Game.OnResetScene -= OnReset;
        Managers.Game.OnResetScene += OnReset;

        _ClearTime = _sceneTask.GetTimeout;

      
        _sceneUI.SetTimerText("--:--:--");
        _Load = 0.0f;
        _state = GameState.Load;

        if(GameObject.FindObjectOfType<GameData>().IsCompletableDailyCheckoutQuest())
        {
            Managers.UI.ShowPopupUI<UI_DailyCheckout>();
        }
        Managers.Item.GiveDailyRewardToUser();


    }
    public override UI_Scene GetSceneUI()
    {
        return _sceneUI;
    }
    public void OnReset()
    {
        
        
       
        //FindObjectOfType<PlayerController>().
        if (Managers.Game.GetTotalTarget().Count != 0)
        {
            Managers.Game.Despawn(Define.WorldObject.Monster);
        }
           
            
        Managers.Game.Despawn(Define.WorldObject.Arrow);
        Managers.Job.ReserveJob(System.TimeSpan.FromSeconds(0.1f), (args) => {
            _Load = 0;
            _state = GameState.Load;
        });

        _state = GameState.Wait;
    }
    public void LoadStage()
    {
   

        _Load += Time.deltaTime; 
        if (_Load >= 0.2f)
        {
            /*
            for(int i=0; i< _spawnPos.Count;i++)
            {
                GameObject monsterobj;
                if(i < _spawnPos.Count - 1)
                    monsterobj = Managers.Game.SpawnMonster(Define.Scene.Main, Managers.Game.Stage, Define.MonsterType.Normal);
                else
                    monsterobj = Managers.Game.SpawnMonster(Define.Scene.Main, Managers.Game.Stage, Define.MonsterType.Boss);
                monsterobj.transform.position = _spawnPos[i].transform.position;
                monsterobj.GetComponent<MonsterController>().OnStateChanged -= OnStateChangedHandler;
                monsterobj.GetComponent<MonsterController>().OnStateChanged += OnStateChangedHandler;
            }
            */
            if(Managers.Game.GetTotalTarget().Count != 0)
                foreach(GameObject monster in Managers.Game.GetTotalTarget())
                {
                    Managers.Game.Despawn(monster);
                }

            Managers.Game.SpawnMonster(Define.Scene.Main, Managers.Game.Stage, Define.MonsterType.Normal);
            

            _RunStage = 0.0f;
            _state = GameState.Run;
        }
    }
    void OnStateChangedHandler(Define.State changedState, GameObject gameobject)
    {

        if (changedState == Define.State.Death)
            Managers.Game.AddCurrency(Define.CurrencyID.Gold.ToString(), _sceneTask.GetMonsterHP(Managers.Game.Stage));
    }
    public void RunStage()
    {
        _RunStage += Time.deltaTime;

        if (Managers.Game.GetTotalTarget().Count == 0 || _RunStage >= _ClearTime)
        {
            _ProcessReward = 0.0f;
            
            _state = GameState.ProcessReward;
            
            
        }
        
        UpdateRunStageTime(_RunStage);
       
    }
    void ProcessReward()
    {
        //foreach (PierceController arraw in GameObject.FindObjectsOfType<PierceController>())
            //Destroy(arraw.gameObject);

        

        _sceneUI.SetTimerText("--:--:--");
        _ProcessReward += Time.deltaTime;
        if (_ProcessReward >= 0.2f)
        {
            if(Managers.Game.GetTotalTarget().Count == 0)
            {
                Managers.Game.CompleteStage();
            }
            _End = 0.0f;
            _state = GameState.End;

        }
    }

    void End()
    {
        _End += Time.deltaTime;

        while (Managers.Game.GetTotalTarget().Count != 0)
            Managers.Game.Despawn(Define.WorldObject.Monster);
        

        if (_End >= 0.2f)
        {
            _Load = 0.0f;
            _state = GameState.Load;
        }
        
    }
    void Wait()
    {

    }

    void UpdateRunStageTime(float _secs)
    {
    
        float progress = (_ClearTime - _secs) / _ClearTime;
        _sceneUI.SetTimerProgress(progress);
        _sceneUI.SetTimerText(string.Format("00:00:{0:00}", _secs));
    }
    public void Update()
    {
       

        switch (_state)
        {
            case GameState.Load:
                LoadStage();
                break;
            case GameState.Run:
                RunStage();
                break;
            case GameState.ProcessReward:
                ProcessReward();
                break;
            case GameState.End:
                End();
                break;
            case GameState.Wait:
                Wait();
                break;

        }
    }

    public override void Clear()
    {
       
    }
    private void OnDestroy()
    {
        _sceneUI = null;
        Managers.Game.OnResetScene -= OnReset;
    }
}
