using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_DongeonItem : UI_Base
{
    StageTask _task;
    public StageTask Task { set { _task = value; } }
    enum Buttons
    {
        Button
    }
    enum Images
    {
        Icon ,
        NameBackground,
        SubDescriptionBackground,
        KeyIcon
    }
    enum Texts
    {
        Name,
        Description,
        KeyText,
        DailyText
    }
    enum GameObjects
    {
        Blocker
    }


    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        if (_task != null)
        {
            GetImage((int)Images.Icon).sprite = _task.Icon;
            GetImage((int)Images.NameBackground).sprite = _task.NameBackground;
            GetImage((int)Images.SubDescriptionBackground).sprite = _task.SubDescriptionBackground;

            GetText((int)Texts.Name).text = _task.Name;
            GetText((int)Texts.Description).text = _task.Description;


            

            if (_task.type == Define.Dongeon.Troll)
            {
                GetImage((int)Images.KeyIcon).gameObject.SetActive(false);

                GetText((int)Texts.KeyText).text = $"도전층 : {Managers.Game.TrollStage + 1}";
                GetText((int)Texts.DailyText).text = "무한 도전";

                BlockerUpdate();
            }
            else
            {
                DongeonKey _key = _task.EnterCondition.Item as DongeonKey;
                GetImage((int)Images.KeyIcon).sprite = _key.Icon;

                GameData _gameData = UnityEngine.Object.FindObjectOfType<GameData>();
                GetText((int)Texts.DailyText).text = $"일일 도전 : {_gameData.StageInfoList.List.Where(x => x.type == _task.type.ToString()).FirstOrDefault().FreePassCount}EA";

                if(_gameData.Inventory.IsFindItem(_task.EnterCondition.Item.ItemId))
                {
                    BaseItem key = _gameData.Inventory.Find(x => x.ItemId == _task.EnterCondition.Item.ItemId);
                    GetText((int)Texts.KeyText).text = $"열쇠 수량 : {key.RemainingUses.GetValueOrDefault()}EA";
                }
                else
                    GetText((int)Texts.KeyText).text = $"열쇠 수량 : {0}EA";

                Get<GameObject>((int)GameObjects.Blocker).SetActive(false);
                BlockerUpdate();
            }

            AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
                GameData _gameData = UnityEngine.Object.FindObjectOfType<GameData>();
               
                
                if (_task.CanEnter(_gameData) == false)
                {
                    UI_FadeText _text = UnityEngine.Object.FindObjectOfType<UI_FadeText>();
                    if (_text == null)
                    {
                         _text = Util.GetOrAddComponent<UI_FadeText>(Managers.Resource.Instantiate("UI/SubItem/UI_FadeText", FindObjectOfType<UI_Dongeon>().gameObject.transform));
                        _text.text = "열쇠 수량이 부족합니다";
                        
                    }
                    else
                    {
                        _text.text = "열쇠 수량이 부족합니다";
                        _text.RePlay();
                   
                    }
                      
                }
                else
                {
                   
                   _task.Enter(_gameData);
                }
                    
                

            });
        }

    }

    void BlockerUpdate()
    {
        if (_task.IsMaxStage(FindObjectOfType<GameData>()) == true)
        {
            if(_task.type == Define.Dongeon.Troll)
            {
                GetText((int)Texts.KeyText).text = $"도전층 : 최고층";
            }
            Get<GameObject>((int)GameObjects.Blocker).SetActive(true);
        }
        else
            Get<GameObject>((int)GameObjects.Blocker).SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    
}
