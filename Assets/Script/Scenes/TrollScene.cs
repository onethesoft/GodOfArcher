using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrollScene : BaseScene
{
    GameObject _background;
    UI_Troll _sceneUI;

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
        SceneType = Define.Scene.Troll;
        Managers.Setting.Play("DongeonBGM", Define.Sound.Bgm);
        _sceneUI = Managers.UI.ShowSceneUI<UI_Troll>();
        //Managers.Resource.Instantiate("UI/UI_DebugPopup", _sceneUI.transform);
        Managers.Game.CreatePlayer();

        _background = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.Troll).FirstOrDefault().GetBackground(Managers.Game.TrollStage + 1);

        _ClearTime = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.Troll).FirstOrDefault().GetTimeout;

        _state = GameState.Load;
    }
    public override UI_Scene GetSceneUI()
    {
        return _sceneUI;
    }
    public void LoadStage()
    {

        _Load += Time.deltaTime;
        _sceneUI.SetLevelText(Managers.Game.TrollStage + 1);
       
        if (_Load >= 0.5f)
        {
           
            Managers.Game.SpawnMonster(Define.Scene.Troll, Managers.Game.TrollStage + 1 , Define.MonsterType.Dongeon);
            _sceneUI.SetEnableButton(UI_Troll.Buttons.Giveup, true);



            _RunStage = 0.0f;
            _state = GameState.Run;
        }
    }

    private void RunStage()
    {
        _RunStage += Time.deltaTime;

        if (Managers.Game.GetTotalTarget().Count == 0 || _RunStage >= _ClearTime)
        {
            if (_sceneUI.IsEnableButton(UI_Troll.Buttons.Giveup) == true)
                _sceneUI.SetEnableButton(UI_Troll.Buttons.Giveup, false);

            Managers.UI.CloseAllPopupUI();

            _ProcessReward = 0.0f;
            _state = GameState.ProcessReward;

            return;
        }
        else if(_RunStage < _ClearTime && _RunStage > _ClearTime - 5.0f)
        {
            if(_sceneUI.IsEnableButton(UI_Troll.Buttons.Giveup) == true)
                _sceneUI.SetEnableButton(UI_Troll.Buttons.Giveup, false);
            
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
                    foreach (GameObject monster in Managers.Game.GetTotalTarget())
                        Managers.Game.Despawn(monster);

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
    public override void Clear()
    {

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
}
