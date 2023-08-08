using PolyAndCode.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_RankingItem : UI_Base, ICell
{
    enum GameObjects
    {
        UI_BowItem,
        UI_HelmetItem,
        UI_ArmorItem,
        UI_CloakItem,
        RankItem,
        Border
    }

    enum Texts
    {
        PositionText,
        DisplayNameText,
        StageText,
        DPSText,
        RankText
    }

    enum Images
    {
        PositionImage,

    }

    enum Mode
    {
        RankerItem,
        PlayerItem
    }

    [SerializeField]
    Image PositionImage;

    [SerializeField]
    Text PositionText;

    [SerializeField]
    GameObject RankItem;

    [SerializeField]
    Text RankText;

    [SerializeField]
    Text DisplayNameText;

    [SerializeField]
    Text StageText;

    [SerializeField]
    Text DPSText;


    [SerializeField]
    UI_BaseItem UI_BowItem;

    [SerializeField]
    UI_BaseItem UI_HelmetItem;

    [SerializeField]
    UI_BaseItem UI_ArmorItem;

    [SerializeField]
    UI_BaseItem UI_CloakItem;

    [SerializeField]
    GameObject Border;




    PlayFab.ClientModels.PlayerLeaderboardEntry _entry;
    PlayerInfo _playerinfo;
    Mode _mode = Mode.RankerItem;

    readonly string[] _ranker = new string[] { "Icon_ImageIcon_Medal_Gold", "Icon_ImageIcon_Medal_Silver", "Icon_ImageIcon_Medal_Bronze" };


    public void Setup(PlayFab.ClientModels.PlayerLeaderboardEntry entry)
    {
        _entry = entry;
        _mode = Mode.RankerItem;
        UpdateRankingItem();
    }
    public void Setup(PlayerInfo player)
    {
        _playerinfo = player;
        _mode = Mode.PlayerItem;
        UpdateRankingItem();
    }

    public void UpdateItemEquipment()
    {
        if (_mode == Mode.RankerItem && _entry.PlayFabId != Managers.Game.PlayerId)
        {
            if (_entry.Profile.Statistics.Any(x => x.Name == PlayerInfo.StatisticsDataKey.ItemEquipment.ToString()) == false)
            {
                UI_BowItem.gameObject.SetActive(false);
                UI_HelmetItem.gameObject.SetActive(false);
                UI_ArmorItem.gameObject.SetActive(false);
                UI_CloakItem.gameObject.SetActive(false);
                /*
                Get<GameObject>((int)GameObjects.UI_BowItem).SetActive(false);
                Get<GameObject>((int)GameObjects.UI_HelmetItem).SetActive(false);
                Get<GameObject>((int)GameObjects.UI_ArmorItem).SetActive(false);
                Get<GameObject>((int)GameObjects.UI_CloakItem).SetActive(false);
                */
            }
            else
            {

                //Debug.Log("ItemEquipment : " + _entry.Profile.Statistics.Count);
                string[] EquippedItemId = Managers.Item.DecodingItemEquipment(_entry.Profile.Statistics.Where(x => x.Name == PlayerInfo.StatisticsDataKey.ItemEquipment.ToString()).First().Value);

                if (string.IsNullOrEmpty(EquippedItemId[0]) == false)
                {
                    UI_BowItem.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemId == EquippedItemId[0]).FirstOrDefault(), UI_BaseItem.Mode.Leaderboard);
                    UI_BowItem.gameObject.SetActive(true);
                    /*
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Get<GameObject>((int)GameObjects.UI_BowItem));
                    _item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemId == EquippedItemId[0]).FirstOrDefault(), UI_BaseItem.Mode.Leaderboard);
                    */
                }
                else
                {
                    UI_BowItem.gameObject.SetActive(false);
                    //Get<GameObject>((int)GameObjects.UI_BowItem).SetActive(false);
                }

                if (string.IsNullOrEmpty(EquippedItemId[1]) == false)
                {
                    UI_HelmetItem.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemId == EquippedItemId[1]).FirstOrDefault(), UI_BaseItem.Mode.Leaderboard);
                    UI_HelmetItem.gameObject.SetActive(true);
                    /*
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Get<GameObject>((int)GameObjects.UI_HelmetItem));
                    _item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemId == EquippedItemId[1]).FirstOrDefault(), UI_BaseItem.Mode.Leaderboard);
                    */
                }
                else
                {
                    UI_HelmetItem.gameObject.SetActive(false);
                    //Get<GameObject>((int)GameObjects.UI_HelmetItem).SetActive(false);
                }

                if (string.IsNullOrEmpty(EquippedItemId[2]) == false)
                {
                    UI_ArmorItem.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemId == EquippedItemId[2]).FirstOrDefault(), UI_BaseItem.Mode.Leaderboard);
                    UI_ArmorItem.gameObject.SetActive(true);
                    /*
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Get<GameObject>((int)GameObjects.UI_ArmorItem));
                    _item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemId == EquippedItemId[2]).FirstOrDefault(), UI_BaseItem.Mode.Leaderboard);
                    */
                }
                else
                {
                    UI_ArmorItem.gameObject.SetActive(false);
                    //Get<GameObject>((int)GameObjects.UI_ArmorItem).SetActive(false);
                }

                if (string.IsNullOrEmpty(EquippedItemId[3]) == false)
                {
                    UI_CloakItem.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemId == EquippedItemId[3]).FirstOrDefault(), UI_BaseItem.Mode.Leaderboard);
                    UI_CloakItem.gameObject.SetActive(true);
                    /*
                    UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Get<GameObject>((int)GameObjects.UI_CloakItem));
                    _item.Setup(Managers.Item.Database.ItemList.Where(x => x.ItemId == EquippedItemId[3]).FirstOrDefault(), UI_BaseItem.Mode.Leaderboard);
                    */
                }
                else
                {
                    UI_CloakItem.gameObject.SetActive(false);
                    // Get<GameObject>((int)GameObjects.UI_CloakItem).SetActive(false);
                }
            }
        }
        else
        {
            if (Managers.Game.GetEquipment("Bow").SlotList[0].IsEquip == false)
            {
                UI_BowItem.gameObject.SetActive(false);
                //Get<GameObject>((int)GameObjects.UI_BowItem).SetActive(false);
            }
            else
            {
                UI_BowItem.Setup(Managers.Game.GetEquipment("Bow").SlotList[0].GetItem, UI_BaseItem.Mode.Leaderboard);
                UI_BowItem.gameObject.SetActive(true);
                /*
                UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Get<GameObject>((int)GameObjects.UI_BowItem));
                _item.Setup(Managers.Game.GetEquipment("Bow").SlotList[0].GetItem, UI_BaseItem.Mode.Leaderboard);
                */
            }


            if (Managers.Game.GetEquipment("Helmet").SlotList[0].IsEquip == false)
            {
                UI_HelmetItem.gameObject.SetActive(false);
                //Get<GameObject>((int)GameObjects.UI_HelmetItem).SetActive(false);
            }
            else
            {
                UI_HelmetItem.Setup(Managers.Game.GetEquipment("Helmet").SlotList[0].GetItem, UI_BaseItem.Mode.Leaderboard);
                UI_HelmetItem.gameObject.SetActive(true);
                /*
                UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Get<GameObject>((int)GameObjects.UI_HelmetItem));
                _item.Setup(Managers.Game.GetEquipment("Helmet").SlotList[0].GetItem, UI_BaseItem.Mode.Leaderboard);
                */
            }

            if (Managers.Game.GetEquipment("Armor").SlotList[0].IsEquip == false)
            {
                UI_ArmorItem.gameObject.SetActive(false);
                //Get<GameObject>((int)GameObjects.UI_ArmorItem).SetActive(false);
            }
            else
            {
                UI_ArmorItem.Setup(Managers.Game.GetEquipment("Armor").SlotList[0].GetItem, UI_BaseItem.Mode.Leaderboard);
                UI_ArmorItem.gameObject.SetActive(true);
                /*
                UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Get<GameObject>((int)GameObjects.UI_ArmorItem));
                _item.Setup(Managers.Game.GetEquipment("Armor").SlotList[0].GetItem, UI_BaseItem.Mode.Leaderboard);
                */
            }


            if (Managers.Game.GetEquipment("Cloak").SlotList[0].IsEquip == false)
            {
                UI_CloakItem.gameObject.SetActive(false);
                //Get<GameObject>((int)GameObjects.UI_CloakItem).SetActive(false);
            }
            else
            {
                UI_CloakItem.Setup(Managers.Game.GetEquipment("Cloak").SlotList[0].GetItem, UI_BaseItem.Mode.Leaderboard);
                UI_CloakItem.gameObject.SetActive(true);
                /*
                UI_BaseItem _item = Util.GetOrAddComponent<UI_BaseItem>(Get<GameObject>((int)GameObjects.UI_CloakItem));
                _item.Setup(Managers.Game.GetEquipment("Cloak").SlotList[0].GetItem, UI_BaseItem.Mode.Leaderboard);
                */
            }


        }
    }

    public void UpdatePosition()
    {
        if (_mode == Mode.RankerItem)
        {
            if (_entry.Position >= 0 && _entry.Position < 3)
            {
                PositionText.gameObject.SetActive(false);
                PositionImage.gameObject.SetActive(true);

                PositionImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/RankingPopup/{_ranker[_entry.Position]}");
                /*
                GetText((int)Texts.PositionText).gameObject.SetActive(false);
                GetImage((int)Images.PositionImage).gameObject.SetActive(true);

                GetImage((int)Images.PositionImage).sprite = Managers.Resource.Load<Sprite>($"Sprites/RankingPopup/{_ranker[_entry.Position]}");
                */
            }
            else
            {

                PositionText.text = (_entry.Position + 1).ToString();
                PositionText.gameObject.SetActive(true);
                PositionImage.gameObject.SetActive(false);

                /*
                GetText((int)Texts.PositionText).gameObject.SetActive(true);
                GetImage((int)Images.PositionImage).gameObject.SetActive(false);

                GetText((int)Texts.PositionText).text = (_entry.Position + 1).ToString();
                */
            }
        }
        else
        {
            PlayFab.ClientModels.PlayerLeaderboardEntry _playerEntry = Managers.Ranking.GetLeaderboardList().Where(x => x.PlayFabId == Managers.Game.PlayerId).FirstOrDefault();
            if (_playerEntry == null)
            {
                PositionText.text = "--";
                PositionText.gameObject.SetActive(true);
                PositionImage.gameObject.SetActive(false);

                /*
                GetText((int)Texts.PositionText).gameObject.SetActive(true);
                GetImage((int)Images.PositionImage).gameObject.SetActive(false);

                GetText((int)Texts.PositionText).text = "--";
                */
            }
            else
            {
                if (_playerEntry.Position >= 0 && _playerEntry.Position < 3)
                {
                    PositionText.gameObject.SetActive(false);
                    PositionImage.gameObject.SetActive(true);
                    PositionImage.sprite = Managers.Resource.Load<Sprite>($"Sprites/RankingPopup/{_ranker[_playerEntry.Position]}");
                    /*

                    GetText((int)Texts.PositionText).gameObject.SetActive(false);
                    GetImage((int)Images.PositionImage).gameObject.SetActive(true);

                    GetImage((int)Images.PositionImage).sprite = Managers.Resource.Load<Sprite>($"Sprites/RankingPopup/{_ranker[_playerEntry.Position]}");
                    */
                }
                else
                {

                    PositionText.text = (_playerEntry.Position + 1).ToString();
                    PositionText.gameObject.SetActive(true);
                    PositionImage.gameObject.SetActive(false);
                    /*
                    GetText((int)Texts.PositionText).gameObject.SetActive(true);
                    GetImage((int)Images.PositionImage).gameObject.SetActive(false);

                    GetText((int)Texts.PositionText).text = (_playerEntry.Position + 1).ToString();
                    */
                }
            }
        }
    }
    public void UpdateLevel()
    {
        if (_mode == Mode.RankerItem)
        {
            if (_entry.Profile.Statistics.Any(x => x.Name == PlayerInfo.StatisticsDataKey.Level.ToString()) == false)
            {
                for (int i = 0; i < RankItem.transform.childCount; i++)
                {
                    Managers.Resource.Destroy(RankItem.transform.GetChild(i).gameObject);
                }
                RankItem.transform.DetachChildren();
                Managers.Resource.Instantiate(Managers.Game.PlyaerDataBase.RankDic[0].Icon, RankItem.transform);
                RankText.text = Managers.Game.PlyaerDataBase.RankDic[0].RankText;
                /*

                Managers.Resource.Instantiate(Managers.Game.PlyaerDataBase.RankDic[0].Icon, Get<GameObject>((int)GameObjects.RankItem).transform);
                GetText((int)Texts.RankText).text = Managers.Game.PlyaerDataBase.RankDic[0].RankText;
                */
            }
            else
            {
                int PlayerRank = _entry.Profile.Statistics.Where(x => x.Name == PlayerInfo.StatisticsDataKey.Level.ToString()).First().Value;
                for (int i = 0; i < RankItem.transform.childCount; i++)
                {
                    Managers.Resource.Destroy(RankItem.transform.GetChild(i).gameObject);
                }
                RankItem.transform.DetachChildren();
                Managers.Resource.Instantiate(Managers.Game.PlyaerDataBase.RankDic[PlayerRank].Icon, RankItem.transform);
                RankText.text = Managers.Game.PlyaerDataBase.RankDic[PlayerRank].RankText;
                /*
                int PlayerRank = _entry.Profile.Statistics.Where(x => x.Name == PlayerInfo.StatisticsDataKey.Level.ToString()).First().Value;
                Managers.Resource.Instantiate(Managers.Game.PlyaerDataBase.RankDic[PlayerRank].Icon, Get<GameObject>((int)GameObjects.RankItem).transform);
                GetText((int)Texts.RankText).text = Managers.Game.PlyaerDataBase.RankDic[PlayerRank].RankText;
                */
            }
        }
        else
        {
            for (int i = 0; i < RankItem.transform.childCount; i++)
            {
                Managers.Resource.Destroy(RankItem.transform.GetChild(i).gameObject);
            }
            RankItem.transform.DetachChildren();

            GameData _player = FindObjectOfType<GameData>();
            Managers.Resource.Instantiate(Managers.Game.PlyaerDataBase.RankDic[_player.Level].Icon, RankItem.transform);
            RankText.text = Managers.Game.PlyaerDataBase.RankDic[_player.Level].RankText;
            /*
            GameData _player = FindObjectOfType<GameData>();
            Managers.Resource.Instantiate(Managers.Game.PlyaerDataBase.RankDic[_player.Level].Icon, Get<GameObject>((int)GameObjects.RankItem).transform);
            GetText((int)Texts.RankText).text = Managers.Game.PlyaerDataBase.RankDic[_player.Level].RankText;
            */
        }
    }

    public void UpdateDPSText()
    {
        if (_mode == Mode.RankerItem)
        {
            System.Numerics.BigInteger _dps;
            if (Managers.Player.GetPlayer(_entry.PlayFabId) != null && Managers.Player.GetPlayer(_entry.PlayFabId).Payload.UserData != null)
            {
                if (Managers.Player.GetPlayer(_entry.PlayFabId).Payload.UserData.ContainsKey(PlayerInfo.UserDataKey.DPS.ToString()))
                {
                    _dps = System.Numerics.BigInteger.Parse(Managers.Player.GetPlayer(_entry.PlayFabId).Payload.UserData[PlayerInfo.UserDataKey.DPS.ToString()].Value);
                    DPSText.text = Util.GetBigIntegerUnit(_dps);
                    //GetText((int)Texts.DPSText).text = Util.GetBigIntegerUnit(_dps);
                }
            }
            else
            {
                if (_entry.PlayFabId == Managers.Game.PlayerId)
                {
                    DPSText.text = Util.GetBigIntegerUnit(Managers.Game.DPS);
                    //GetText((int)Texts.DPSText).text = Util.GetBigIntegerUnit(Managers.Game.DPS);
                }

                else
                {
                    DPSText.text = "";
                    //GetText((int)Texts.DPSText).text = "";
                }


            }



        }
        else
        {
            DPSText.text = Util.GetBigIntegerUnit(Managers.Game.DPS);
            //GetText((int)Texts.DPSText).text = Util.GetBigIntegerUnit(Managers.Game.DPS);
        }
    }
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        UpdateRankingItem();

    }

    private void UpdateRankingItem()
    {
        if (_mode == Mode.RankerItem)
        {
            Debug.Assert(_entry != null, "entry is not setup");

            DisplayNameText.text = _entry.DisplayName;
            //GetText((int)Texts.DisplayNameText).text = _entry.DisplayName;

            if (_entry.PlayFabId == Managers.Game.PlayerId)
            {
                PlayFab.ClientModels.StatisticValue _stage = Managers.Player.GetPlayer(_entry.PlayFabId).Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.MaxClearStage.ToString()).FirstOrDefault();
                if (_stage == null)
                {
                    StageText.text = "0";
                    //GetText((int)Texts.StageText).text = "0";
                }
                else
                {
                    StageText.text = _stage.Value.ToString();
                    //GetText((int)Texts.StageText).text = _stage.Value.ToString();
                }
            }
            else
            {
                StageText.text = _entry.StatValue.ToString();
                //GetText((int)Texts.StageText).text = _entry.StatValue.ToString();
            }

            UpdateDPSText();
            UpdateItemEquipment();
            UpdateLevel();
            UpdatePosition();


            if (_entry.PlayFabId == Managers.Game.PlayerId)
            {
                Border.SetActive(true);
                //Get<GameObject>((int)GameObjects.Border).SetActive(true);
            }
            else
            {
                Border.SetActive(false);
                //Get<GameObject>((int)GameObjects.Border).SetActive(false);
            }
        }
        else
        {
            Debug.Assert(_playerinfo != null, "PlayerInfo is not setup");

            DisplayNameText.text = Managers.Player.GetPlayer(Managers.Game.PlayerId).DisplayName;
            //GetText((int)Texts.DisplayNameText).text = Managers.Player.GetPlayer(Managers.Game.PlayerId).DisplayName;

            PlayFab.ClientModels.StatisticValue _stage = _playerinfo.Payload.PlayerStatistics.Where(x => x.StatisticName == PlayerInfo.StatisticsDataKey.MaxClearStage.ToString()).FirstOrDefault();
            if (_stage == null)
            {
                StageText.text = "0";
                //GetText((int)Texts.StageText).text = "0";
            }
            else
            {
                StageText.text = _stage.Value.ToString();
                //GetText((int)Texts.StageText).text = _stage.Value.ToString();
            }

            UpdateDPSText();
            UpdateItemEquipment();
            UpdateLevel();
            UpdatePosition();

            Border.SetActive(true);

        }
    }




}
