using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DarkMageScene : BaseScene
{
    GameObject _background;
    UI_DarkMage _sceneUI;

    float _ClearTime;
    float _Load = 0.0f;
    float _RunStage = 0.0f;
    float _ProcessReward = 0.0f;
    float _End = 0.0f;

    [SerializeField]
    List<GameObject> _spawnPos;

    public override UI_Scene GetSceneUI()
    {
        return _sceneUI;
    }
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.DarkMage;

        Managers.Setting.Play("DongeonBGM", Define.Sound.Bgm);
        _sceneUI = Managers.UI.ShowSceneUI<UI_DarkMage>();

        //Managers.Resource.Instantiate("UI/UI_DebugPopup", _sceneUI.transform);

        Managers.Game.CreatePlayer();


        _background = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.DarkMage).FirstOrDefault().GetBackground(Managers.Game.Stage);

        _ClearTime = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.DarkMage).FirstOrDefault().GetTimeout;


        _sceneUI.SetTimerText("--:--:--");
        _Load = 0.0f;
        _state = GameState.Load;


    }
    public void LoadStage()
    {
        _Load += Time.deltaTime;
        if (_Load >= 0.5f)
        {
            Managers.Game.SpawnMonster(Define.Scene.DarkMage, Managers.Game.Stage, Define.MonsterType.Dongeon);
         
            _RunStage = 0.0f;
            _state = GameState.Run;
        }
    }
    public void RunStage()
    {
        _RunStage += Time.deltaTime;

        if (Managers.Game.GetTotalTarget().Count == 0 || _RunStage >= _ClearTime)
        {
            _ProcessReward = 0.0f;
            _state = GameState.ProcessReward;

            return;
        }

        UpdateRunStageTime(_RunStage);

    }
    void ProcessReward()
    {
        _sceneUI.SetTimerText("--:--:--");
        _ProcessReward += Time.deltaTime;
        if (_ProcessReward >= 0.5f)
        {
            UI_DongeonRewardPopup _reward = Managers.UI.ShowPopupUI<UI_DongeonRewardPopup>();
            _reward.OnRetry += () => {

               
                if (Managers.Game.GetNextTarget() != null)
                    Managers.Game.Despawn(Define.WorldObject.Monster);
                
                   

                Managers.UI.ClosePopupUI();


                _Load = 0.0f;
                this.Invoke("OnLoad", 0.1f);


            };

            _End = 0.0f;
            _state = GameState.End;

        }
    }

    void End()
    {
        _End += Time.deltaTime;

      

    }
    private void OnLoad()
    {
        _state = GameState.Load;
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
                // Wait();
                break;

        }
    }

    public override void Clear()
    {

    }
    private void OnDestroy()
    {
        _sceneUI = null;
    }
}
