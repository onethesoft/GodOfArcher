using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_DongeonRewardPopup : UI_Popup
{
    public delegate void OnRetryHandler();
    public OnRetryHandler OnRetry;




    enum Buttons
    {
        OkButton,
        RetryButton
    }
    enum Texts
    {
        TitleText,
        RewardText,
        InfoText
    }
    enum GameObjects
    {
        RewardPanel,
        Loading_Rotate,
        BottomPanel
    }
    enum Images
    {
        GoldPigRewardImage
    }
    enum Sliders
    {
        ScrollingSlider
    }
    StageTask _rewardTask;


    void OnRetryButton()
    {
        GameData _playerData = FindObjectOfType<GameData>();
        if (_rewardTask.CanEnter(_playerData))
        {
            _rewardTask.CurrentMode = StageTask.Mode.AutoRetry;
            FindObjectOfType<GameData>().SkillSet.First().Reset();
            _rewardTask.Enter(_playerData , false);

            OnRetry?.Invoke();
        }
    }

    void OnOKButton()
    {
        _rewardTask.CurrentMode = StageTask.Mode.Select;
        Managers.Scene.LoadScene(Define.Scene.Main);
    }
    

    void CheckRetry()
    {
        // 최고층 도달 시 재도전 불가이거나 입장 불가시
        if (_rewardTask.IsMaxStage(FindObjectOfType<GameData>()) == true || _rewardTask.CanEnter(FindObjectOfType<GameData>()) == false)
        {
            GetButton((int)Buttons.RetryButton).gameObject.SetActive(false);
            Get<UI_ScrollingSlider>((int)Sliders.ScrollingSlider).gameObject.SetActive(false);

        }
        else if(_rewardTask.CurrentMode == StageTask.Mode.AutoRetry)
        {
            GetButton((int)Buttons.RetryButton).gameObject.SetActive(false);
            Get<UI_ScrollingSlider>((int)Sliders.ScrollingSlider).gameObject.SetActive(true);
            Get<UI_ScrollingSlider>((int)Sliders.ScrollingSlider).OnComplete(() => { OnRetryButton(); });
            Get<UI_ScrollingSlider>((int)Sliders.ScrollingSlider).SetDuration(4.0f);
            Get<UI_ScrollingSlider>((int)Sliders.ScrollingSlider).Play();
        }
        else
        {
            GetButton((int)Buttons.RetryButton).gameObject.SetActive(true);
            Get<UI_ScrollingSlider>((int)Sliders.ScrollingSlider).gameObject.SetActive(false);
        }
       
        
    }
    void HideRetryButton()
    {
        if (GetButton((int)Buttons.RetryButton) != null)
            GetButton((int)Buttons.RetryButton).gameObject.SetActive(false);
    }
    void ShowRetryButton()
    {
        if (GetButton((int)Buttons.RetryButton) != null)
            GetButton((int)Buttons.RetryButton).gameObject.SetActive(true);
    }

    
    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<UI_ScrollingSlider>(typeof(Sliders));

        GetImage((int)Images.GoldPigRewardImage).gameObject.SetActive(false);
        GetText((int)Texts.RewardText).gameObject.SetActive(false);
        Get<GameObject>((int)GameObjects.Loading_Rotate).SetActive(false);
        GetText((int)Texts.InfoText).gameObject.SetActive(false);
        Get<UI_ScrollingSlider>((int)Sliders.ScrollingSlider).gameObject.SetActive(false);


        if (Managers.Scene.CurrentScene is GoldPigScene)
        {
            _rewardTask = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.GoldPig).First();

            StageTask _mainTask = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.Main).First();

            GetText((int)Texts.TitleText).text = _rewardTask.Name+ " 클리어";

            
            if(Managers.Game.GetTotalTarget().Count == 0)
            {
                // 보상 수여
                _rewardTask.ProcessReward((reward) => {

                    GetImage((int)Images.GoldPigRewardImage).gameObject.SetActive(true);
                    GetText((int)Texts.RewardText).gameObject.SetActive(true);
                    GetText((int)Texts.RewardText).text = $"{Util.GetBigIntegerUnit(reward.Gold)} 골드 획득";

                    CheckRetry();

                });
            }
            else
            {
                GetText((int)Texts.RewardText).gameObject.SetActive(true);
                GetText((int)Texts.RewardText).text = "획득 골드 없음";

                _rewardTask.CurrentMode = StageTask.Mode.Select;
                CheckRetry();
            }


        }
        else if(Managers.Scene.CurrentScene is DarkMageScene)
        {
            _rewardTask = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.DarkMage).First();
            GetText((int)Texts.TitleText).text = _rewardTask.Name +" 클리어";

            MonsterController _monster = Managers.Game.GetNextTarget().GetComponent<MonsterController>();
            Get<GameObject>((int)GameObjects.Loading_Rotate).SetActive(true);
            Get<GameObject>((int)GameObjects.BottomPanel).SetActive(false);



            _rewardTask.ProcessReward((reward)=> {
                Get<GameObject>((int)GameObjects.Loading_Rotate).SetActive(false);
                Get<GameObject>((int)GameObjects.BottomPanel).SetActive(true);
                foreach (BaseItem _item in reward.rewards)
                {
                    for (int i = 0; i < _item.UsesIncrementedBy; i++)
                        Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_BaseItem", Get<GameObject>((int)GameObjects.RewardPanel).transform)).Item = _item;
                }


                GetText((int)Texts.InfoText).gameObject.SetActive(true);
                GameObject monster = Managers.Game.GetNextTarget();
                MonsterController _controller = monster.GetComponent<MonsterController>();
                System.Numerics.BigInteger _damageMeter = System.Numerics.BigInteger.Abs(_controller.GetHp());

                GetText((int)Texts.InfoText).text = $"현재 기록 : {Util.GetBigIntegerUnit(_damageMeter)}";

                CheckRetry();
            });
           
           
        
        }
        else // Troll
        {

            _rewardTask = Managers.Game.StageDataBase.StageList.Where(x => x.type == Define.Dongeon.Troll).First();
            GetText((int)Texts.TitleText).text = _rewardTask.Name + " 클리어";
          

            if (Managers.Game.GetTotalTarget().Count == 0)
            {
                Managers.Game.CompleteStage();
                // 보상 수여
                Get<GameObject>((int)GameObjects.Loading_Rotate).SetActive(true);
                Get<GameObject>((int)GameObjects.BottomPanel).SetActive(false);

                _rewardTask.ProcessReward(
                    (reward) => 
                    {
                        Get<GameObject>((int)GameObjects.Loading_Rotate).SetActive(false);
                        Get<GameObject>((int)GameObjects.BottomPanel).SetActive(true);
                        foreach (BaseItem _item in reward.rewards)
                        {
                            Util.GetOrAddComponent<UI_BaseItem>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_BaseItem", Get<GameObject>((int)GameObjects.RewardPanel).transform)).Item = _item;
                        }

                        GetText((int)Texts.InfoText).gameObject.SetActive(true);

                        GameData _playerData = FindObjectOfType<GameData>();
                        // 도전층의 경우 +1
                        GetText((int)Texts.InfoText).text = $"현재 클리어 층 : {_playerData.TrollStage}층";

                        CheckRetry();
                });

               

            }
            else
            {
                GetText((int)Texts.RewardText).gameObject.SetActive(true);
                GetText((int)Texts.RewardText).text = "획득 보상 없음";

                _rewardTask.CurrentMode = StageTask.Mode.Select;
                CheckRetry();

            }

          
        }

        AddUIEvent(GetButton((int)Buttons.OkButton).gameObject, (data) => {
            OnOKButton();
        });
        AddUIEvent(GetButton((int)Buttons.RetryButton).gameObject, (data) => {
            _rewardTask.CurrentMode = StageTask.Mode.AutoRetry;
            OnRetryButton();
        });


    }
}
