using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using PlayFab.ClientModels;

public class UI_Test : UI_Popup
{
    enum Buttons
    {
        Exit,
        Close,
        StageButton,
        SkillCoolTimeButton,
        SpecialResetTest,
        SPButton,
        QuestCompleteTest,
        GoldTest,
        AllReset,
        LevelTest,
        CPResetTest,
        DailyQuestReset,
        DailyQuestComplete,
        DailyCheckoutTest,
        DailyCheckoutResetTest,
        GiveBeginnerCouponTest,
        AutoTest,
        CharacterReset,
        ShopQuestReset
    }

    enum InputFields
    {
        StageInput,
        SkillCoolTimeInput
    }

    string _stageInput;
    string _skillCoolTimeInput;
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));

#if ENABLE_LOG


        Get<InputField>((int)InputFields.StageInput).onValueChanged.AddListener((text) => {
            _stageInput = text;
        });

        Get<InputField>((int)InputFields.SkillCoolTimeInput).onValueChanged.AddListener((text) => {
            _skillCoolTimeInput = text;
        });

        AddUIEvent(GetButton((int)Buttons.AutoTest).gameObject, (data) => {
            Managers.Game.AutoForTest = !Managers.Game.AutoForTest;
          
        });

        AddUIEvent(GetButton((int)Buttons.StageButton).gameObject, (data) => {
            if (string.IsNullOrEmpty(_stageInput))
                return;

            int _jumpStage = 1;
            if (!Int32.TryParse(_stageInput, out _jumpStage))
                return;
            if (_jumpStage <= 0 || _jumpStage > 500000)
                return;
            Managers.Game.MoveStage(_jumpStage);
            ClosePopupUI();
        });

        AddUIEvent(GetButton((int)Buttons.SkillCoolTimeButton).gameObject, (data) => {
            /*
            if (string.IsNullOrEmpty(_skillCoolTimeInput))
                return;

            float _cooltime = 0.3f;
            if (!float.TryParse(_skillCoolTimeInput, out _cooltime))
                return;
            if (_cooltime < 0.1f || _cooltime >= float.MaxValue)
                return;
            Managers.Game.GetSkillCoolTime = _cooltime;
            */
            ClosePopupUI();
        });

        AddUIEvent(GetButton((int)Buttons.SpecialResetTest).gameObject, (data) => {
            Managers.Game.ResetStat(Define.StatType.Special);
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.SPButton).gameObject, (data) => {

            Managers.Game.AddCurrency(Define.CurrencyID.SP.ToString(), 1000);
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.QuestCompleteTest).gameObject, (data) => {

            if (Managers.Network.IS_ENABLE_NETWORK == false)
                return;

            string[] QuestCodeName = new string[] { "BossMonsterKill_0" , 
                "StageClear_0" , "StageClear_1" , "StageClear_2" , "StageClear_3" , "StageClear_4" , "StageClear_5" ,"StageClear_6" , "StageClear_7" , "StageClear_8" ,"StageClear_9" ,
            "StageClear_10" , "StageClear_11" , "StageClear_12" , "StageClear_13" ,"StageClear_14" ,"StageClear_15",
            "BossMonsterKill_1" ,"BossMonsterKill_2" ,"BossMonsterKill_3","BossMonsterKill_4",
            "MonsterKill_0", "MonsterKill_1", "MonsterKill_2","MonsterKill_3","MonsterKill_4","MonsterKill_5","MonsterKill_6",
            "PlayTime_0","PlayTime_1","PlayTime_2","PlayTime_3"
            };

            List<Quest> _findQuestByCodeName = Managers.Quest.ActiveQuests.Where(x => QuestCodeName.Any(y => y == x.CodeName)).ToList();
            foreach (Quest quest in _findQuestByCodeName)
                quest.Complete();

            
         
           
        });

        AddUIEvent(GetButton((int)Buttons.GoldTest).gameObject, (data) => {
            Managers.Game.AddCurrency(Define.CurrencyID.Gold.ToString(), System.Numerics.BigInteger.Parse("50000000000000000000000000000"), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
            Managers.Game.AddCurrency(Define.CurrencyID.CP.ToString(), System.Numerics.BigInteger.Parse("50000000000000000000000000000"), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
            Managers.Game.AddCurrency(Define.CurrencyID.SP.ToString(), 100, IsUpdate: Managers.Network.IS_ENABLE_NETWORK);

        });

        AddUIEvent(GetButton((int)Buttons.AllReset).gameObject, (data) => {

            Managers.Game.ResetCurrency(Define.CurrencyID.Gold.ToString(), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
            Managers.Game.ResetCurrency(Define.CurrencyID.CP.ToString(), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);


        });

        AddUIEvent(GetButton((int)Buttons.LevelTest).gameObject, (data) => {
            Debug.Log("LevelyupTest");
            Managers.Game.Levelup();

        });
        AddUIEvent(GetButton((int)Buttons.CPResetTest).gameObject, (data) => {

            Managers.Game.ResetStat(Define.StatType.Craft);

        });
        AddUIEvent(GetButton((int)Buttons.DailyQuestReset).gameObject, (data) => {

            Managers.Quest.ResetQuests(new string[] { Define.QuestType.Daily.ToString() });

        });
        AddUIEvent(GetButton((int)Buttons.DailyQuestComplete).gameObject, (data) => {

            string[] QuestCodeName = new string[] { "Daily_0" , "Daily_1" , "Daily_2" , "Daily_3" , "Daily_4" , "Daily_5"

            };

            List<Quest> _findQuestByCodeName = Managers.Quest.ActiveQuests.Where(x => QuestCodeName.Any(y => y == x.CodeName)).ToList();
            foreach (Quest quest in _findQuestByCodeName)
                quest.Complete();


          

        });
        AddUIEvent(GetButton((int)Buttons.DailyCheckoutTest).gameObject, (data) => {
            FindObjectOfType<GameData>().ProcessReportDailyCheckoutQuest();
          
           
               
        });

        AddUIEvent(GetButton((int)Buttons.DailyCheckoutResetTest).gameObject, (data) => {
            FindObjectOfType<GameData>().DailyCheckoutQuestLastClearTime = null;
            Managers.Game.Save(PlayerInfo.UserDataKey.DailyCheckoutQuestLastClearTime);

            Managers.Quest.ResetQuests(new string[] { Define.QuestType.DailyCheckout.ToString()});

        });

        AddUIEvent(GetButton((int)Buttons.GiveBeginnerCouponTest).gameObject, (data) => {
            Managers.Item.GiveCouponToUser("WELCOME" , FindObjectOfType<GameData>());
        });

        AddUIEvent(GetButton((int)Buttons.CharacterReset).gameObject, (data) => {
            Managers.UI.ShowPopupUI<UI_LoadingBlock>();
            FindObjectOfType<GameDataUpdater>().StopUpdate();
            Managers.Game.AutoForTest = false;

            StartCoroutine(AllResetCharacter());


           


        });

        AddUIEvent(GetButton((int)Buttons.ShopQuestReset).gameObject, (data) => {
            Managers.UI.ShowPopupUI<UI_LoadingBlock>();
            Managers.Game.AutoForTest = false;
            FindObjectOfType<GameDataUpdater>().StopUpdate();
         
            StartCoroutine(ShopQuestReset());





        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { ClosePopupUI(); });
#endif
    }

    IEnumerator AllResetCharacter()
    {
        yield return new WaitForSeconds(1);
#if ENABLE_LOG

        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.Level.ToString()))
            Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.Level.ToString()).First().Value = 0;
        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.Stage.ToString()))
            Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.Stage.ToString()).First().Value = 1;
        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.ClearStage.ToString()))
            Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.ClearStage.ToString()).First().Value = 0;
        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.MaxClearStage.ToString()))
            Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.MaxClearStage.ToString()).First().Value = 0;
        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.TrollStage.ToString()))
            Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.TrollStage.ToString()).First().Value = 0;
        if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Any(x => x.StatisticName == PlayerInfo.StatisticsDataKey.ItemEquipment.ToString()))
            Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.ItemEquipment.ToString()).First().Value = (255 << 24) + (255 << 16) + (255 << 8) + 255;


        PlayerInfo.StatisticsDataKey[] _arr = new PlayerInfo.StatisticsDataKey[Enum.GetNames(typeof(PlayerInfo.StatisticsDataKey)).Length];
        int index = 0;
        foreach (PlayerInfo.StatisticsDataKey it in Enum.GetValues(typeof(PlayerInfo.StatisticsDataKey)))
            _arr[index++] = it;

        Managers.Network.UpdateStatisticsData(_arr, Managers.Player.GetPlayer(Managers.Game.PlayerId));

        yield return new WaitForSeconds(2);

        Managers.Game.ResetCurrency(Define.CurrencyID.Ruby.ToString(), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
        Managers.Game.ResetCurrency(Define.CurrencyID.SP.ToString(), IsUpdate: Managers.Network.IS_ENABLE_NETWORK);

        yield return new WaitForSeconds(1);

        List<string> _remove = Enum.GetNames(typeof(PlayerInfo.UserDataKey)).ToList();
        _remove.Remove(PlayerInfo.UserDataKey.SessionInfo.ToString());
        List<List<string>> _RemoveList =  _remove.Select((x, i) => new { index = i, value = x }).GroupBy(x => x.index / 10).Select(x => x.Select(v => v.value).ToList()).ToList();




        foreach (List<string> keys in _RemoveList)
        {
            if(keys == _RemoveList.Last())
                PlayFab.PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest {   KeysToRemove = keys }, (result) => {
                    foreach (BaseItem item in Managers.Game.GetInventory().ToList())
                    {
                        if (item == Managers.Game.GetInventory().ToList().Last())
                            Managers.Network.ConsumeItem(item.ItemInstanceId, item.RemainingUses.GetValueOrDefault(), (result) => {

                                Application.Quit(); 

                            });
                        else
                            Managers.Network.ConsumeItem(item.ItemInstanceId, item.RemainingUses.GetValueOrDefault());

                    }

                    }
               , error => {
                   Debug.Log(error.GenerateErrorReport());
               }
               );
            else
                PlayFab.PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { KeysToRemove = keys }, (result) => {
                }
              , error => {
                  Debug.Log(error.GenerateErrorReport());
              }
              );

            yield return new WaitForSeconds(1);
        }

#endif


    }

    IEnumerator ShopQuestReset()
    {
        yield return new WaitForSeconds(2);
#if ENABLE_LOG


        PlayFab.PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { KeysToRemove = new List<string>() { PlayerInfo.UserDataKey.GambleQuest.ToString() } }, (result) => {

        Application.Quit(); 

          

        }
        , error => {
            Debug.Log(error.GenerateErrorReport());
        }
        );
#endif
    }




}
