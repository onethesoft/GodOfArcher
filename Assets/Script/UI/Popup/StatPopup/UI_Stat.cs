using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class UI_Stat : UI_Popup
{
    public delegate void OnLeveupCountChangedHandler(int Count);
    event OnLeveupCountChangedHandler _LevelupCountChanged;
    enum Buttons
    {
        GoldStat ,
        SpecialStat ,
        CPStat ,
        SkillStat ,
        Upgrade_1,
        Upgrade_10 ,
        Upgrade_100,
        Upgrade_1000 ,
        Upgrade_10000,
        Close ,
        Exit
    }

    
    enum GameObjects
    {
        StatItemPanel,
        MiddlePanel
    }
    Dictionary<Define.StatID , UI_StatItem> _Items;
    Dictionary<int, UI_UpgradeButton> _UpgradeCountButtons;
    bool _isUpdate = false;
    
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        _isUpdate = false;
        LoadStatItem();
        LoadUpgradeCountButton();
        SelectStatButton((int)Buttons.GoldStat);

       //Managers.Game.OnCurrencyChanged

        AddUIEvent(GetButton((int)Buttons.GoldStat).gameObject, (data) => {
            SelectStatButton((int)Buttons.GoldStat);
         

            GetButton((int)Buttons.GoldStat).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            Get<GameObject>((int)GameObjects.MiddlePanel).SetActive(true);

        });
        AddUIEvent(GetButton((int)Buttons.SpecialStat).gameObject, (data) => {
            SelectStatButton((int)Buttons.SpecialStat);
          

            GetButton((int)Buttons.SpecialStat).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            Get<GameObject>((int)GameObjects.MiddlePanel).SetActive(true);
        });
        AddUIEvent(GetButton((int)Buttons.CPStat).gameObject, (data) => {
            SelectStatButton((int)Buttons.CPStat);
           

            GetButton((int)Buttons.CPStat).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            Get<GameObject>((int)GameObjects.MiddlePanel).SetActive(true);

        });
        AddUIEvent(GetButton((int)Buttons.SkillStat).gameObject, (data) => {
            SelectStatButton((int)Buttons.SkillStat);
            

            GetButton((int)Buttons.SkillStat).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            Get<GameObject>((int)GameObjects.MiddlePanel).SetActive(false);
            
        });

        AddUIEvent(GetButton((int)Buttons.Upgrade_1).gameObject, (data) => {
            GetButton((int)Buttons.Upgrade_1).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            _LevelupCountChanged?.Invoke(1);
        });
        AddUIEvent(GetButton((int)Buttons.Upgrade_10).gameObject, (data) => {
            GetButton((int)Buttons.Upgrade_10).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            _LevelupCountChanged?.Invoke(10);
        });
        AddUIEvent(GetButton((int)Buttons.Upgrade_100).gameObject, (data) => {
            GetButton((int)Buttons.Upgrade_100).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            _LevelupCountChanged?.Invoke(100);

        });
        AddUIEvent(GetButton((int)Buttons.Upgrade_1000).gameObject, (data) => {
            GetButton((int)Buttons.Upgrade_1000).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            _LevelupCountChanged?.Invoke(1000);

        });
        AddUIEvent(GetButton((int)Buttons.Upgrade_10000).gameObject, (data) => {
            GetButton((int)Buttons.Upgrade_10000).gameObject.GetComponent<DG.Tweening.DOTweenAnimation>().DORestart();
            _LevelupCountChanged?.Invoke(10000);
        });


        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });
    }
    
    public void LoadStatItem()
    {
        _Items = new Dictionary<Define.StatID, UI_StatItem>();
        foreach(CharacterStat stat in Managers.Game.GetPlayerStatForUI())
        {
            Define.StatID _statId = (Define.StatID)Enum.Parse(typeof(Define.StatID), stat.CodeName);
            _Items.Add(_statId, Util.GetOrAddComponent<UI_StatItem>(Managers.Resource.Instantiate($"UI/SubItem/StatPopup/UI_StatItem", Get<GameObject>((int)GameObjects.StatItemPanel).transform)));
            _Items[_statId].Stat = stat;
            _Items[_statId].OnLevelup -= OnLevelup;
            _Items[_statId].OnLevelup += OnLevelup;
            _LevelupCountChanged -= _Items[_statId].ShowLevelUpCost;
            _LevelupCountChanged += _Items[_statId].ShowLevelUpCost;
        }
        

       
    }
    public void LoadUpgradeCountButton()
    {
        _UpgradeCountButtons = new Dictionary<int, UI_UpgradeButton>();
        _UpgradeCountButtons.Add(1, Util.GetOrAddComponent<UI_UpgradeButton>(GetButton((int)Buttons.Upgrade_1).gameObject));
        _UpgradeCountButtons.Add(10, Util.GetOrAddComponent<UI_UpgradeButton>(GetButton((int)Buttons.Upgrade_10).gameObject));
        _UpgradeCountButtons.Add(100, Util.GetOrAddComponent<UI_UpgradeButton>(GetButton((int)Buttons.Upgrade_100).gameObject));
        _UpgradeCountButtons.Add(1000, Util.GetOrAddComponent<UI_UpgradeButton>(GetButton((int)Buttons.Upgrade_1000).gameObject));
        _UpgradeCountButtons.Add(10000, Util.GetOrAddComponent<UI_UpgradeButton>(GetButton((int)Buttons.Upgrade_10000).gameObject));

        _UpgradeCountButtons[1].CountText = string.Format(Managers.Data.TextData[(int)Define.TextData.UpgradeCountText].Kor, 1);
        _UpgradeCountButtons[10].CountText = string.Format(Managers.Data.TextData[(int)Define.TextData.UpgradeCountText].Kor, 10);
        _UpgradeCountButtons[100].CountText = string.Format(Managers.Data.TextData[(int)Define.TextData.UpgradeCountText].Kor, 100);
        _UpgradeCountButtons[1000].CountText = string.Format(Managers.Data.TextData[(int)Define.TextData.UpgradeCountText].Kor, 1000);
        _UpgradeCountButtons[10000].CountText = string.Format(Managers.Data.TextData[(int)Define.TextData.UpgradeCountText].Kor, 10000);

    }
    public void SelectStatButton(int Id)
    {
        foreach(UI_StatItem _item in _Items.Select(x=>x.Value).ToList())
        {
            _item.gameObject.SetActive(false);
        }
        
       
        

        if (Id == (int)Buttons.GoldStat)
        {
            _Items[Define.StatID.Attack].gameObject.SetActive(true);
            _Items[Define.StatID.AttackSpeed].gameObject.SetActive(true);
            _Items[Define.StatID.CriticalHit].gameObject.SetActive(true);
            _Items[Define.StatID.CriticalHitRate].gameObject.SetActive(true);


            Get<GameObject>((int)GameObjects.MiddlePanel).SetActive(true);
            SelectUpgradeCountButton(Id);


        }
        else if(Id == (int)Buttons.CPStat)
        {
            _Items[Define.StatID.CraftAttack].gameObject.SetActive(true);
            _Items[Define.StatID.CraftAttackSpeed].gameObject.SetActive(true);
            _Items[Define.StatID.CraftCriticalHit].gameObject.SetActive(true);
            _Items[Define.StatID.CraftCriticalHitRate].gameObject.SetActive(true);

            Get<GameObject>((int)GameObjects.MiddlePanel).SetActive(true);
            SelectUpgradeCountButton(Id);

        }
        else if(Id == (int)Buttons.SpecialStat)
        {
            _Items[Define.StatID.GoldDropRate].gameObject.SetActive(true);
            _Items[Define.StatID.CraftDropRate].gameObject.SetActive(true);
            _Items[Define.StatID.ExtraHit].gameObject.SetActive(true);
            _Items[Define.StatID.JumpingCount].gameObject.SetActive(true);

            Get<GameObject>((int)GameObjects.MiddlePanel).SetActive(true);
            SelectUpgradeCountButton(Id);
        }
        else if(Id == (int)Buttons.SkillStat)
        {
            _Items[Define.StatID.SkillAttack].gameObject.SetActive(true);
            _Items[Define.StatID.SkillMaxLevelLimit].gameObject.SetActive(true);

            Get<GameObject>((int)GameObjects.MiddlePanel).SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(Get<GameObject>((int)GameObjects.StatItemPanel).GetComponent<RectTransform>());
    }

    void SelectUpgradeCountButton(int Id)
    {
        if (Id == (int)Buttons.GoldStat)
        {
            GetButton((int)Buttons.Upgrade_1000).gameObject.SetActive(true);
            GetButton((int)Buttons.Upgrade_10000).gameObject.SetActive(true);

            _UpgradeCountButtons[1].StatText = Managers.Data.TextData[(int)Define.TextData.GoldSelectButtonText].Kor;
            _UpgradeCountButtons[10].StatText = Managers.Data.TextData[(int)Define.TextData.GoldSelectButtonText].Kor;
            _UpgradeCountButtons[100].StatText = Managers.Data.TextData[(int)Define.TextData.GoldSelectButtonText].Kor;
            _UpgradeCountButtons[1000].StatText = Managers.Data.TextData[(int)Define.TextData.GoldSelectButtonText].Kor;
            _UpgradeCountButtons[10000].StatText = Managers.Data.TextData[(int)Define.TextData.GoldSelectButtonText].Kor;

        }
        else if (Id == (int)Buttons.CPStat)
        {
            GetButton((int)Buttons.Upgrade_1000).gameObject.SetActive(true);
            GetButton((int)Buttons.Upgrade_10000).gameObject.SetActive(true);

            _UpgradeCountButtons[1].StatText = Managers.Data.TextData[(int)Define.TextData.CPSelectButtonText].Kor;
            _UpgradeCountButtons[10].StatText = Managers.Data.TextData[(int)Define.TextData.CPSelectButtonText].Kor;
            _UpgradeCountButtons[100].StatText = Managers.Data.TextData[(int)Define.TextData.CPSelectButtonText].Kor;
            _UpgradeCountButtons[1000].StatText = Managers.Data.TextData[(int)Define.TextData.CPSelectButtonText].Kor;
            _UpgradeCountButtons[10000].StatText = Managers.Data.TextData[(int)Define.TextData.CPSelectButtonText].Kor;

        }
        else if (Id == (int)Buttons.SpecialStat)
        {
            _UpgradeCountButtons[1].StatText = Managers.Data.TextData[(int)Define.TextData.SpecialSelectButtonText].Kor;
            _UpgradeCountButtons[10].StatText = Managers.Data.TextData[(int)Define.TextData.SpecialSelectButtonText].Kor;
            _UpgradeCountButtons[100].StatText = Managers.Data.TextData[(int)Define.TextData.SpecialSelectButtonText].Kor;

         
            GetButton((int)Buttons.Upgrade_1000).gameObject.SetActive(false);
            GetButton((int)Buttons.Upgrade_10000).gameObject.SetActive(false);
        }
        else if (Id == (int)Buttons.SkillStat)
        {
           
        }
    }


    void OnLevelup(Define.StatID Id , bool IsAll)
    {
        _isUpdate = true;
    }
    public override void ClosePopupUI()
    {
        if (_isUpdate == true)
        {
            Managers.Game.Save(PlayerInfo.UserDataKey.PlayerStat);
            Managers.Game.SynchronizeCurrency();
        }

        base.ClosePopupUI();
        _LevelupCountChanged = null;
    }


}
