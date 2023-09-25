using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Roulette : UI_Popup
{
    [SerializeField]
    Button ExitButton;

    [SerializeField]
    Button CloseButton;

    [SerializeField]
    Button button;

    [SerializeField]
    Roulette roulette;

    [SerializeField]
    Text PlayRouletteCountText;

    [SerializeField]
    Text CoinToPlayRouletteText;

    [SerializeField]
    float _updateDelay = 10.0f;

    [HideInInspector]
    float _time = 0.0f;

    [HideInInspector]
    GameData playerData;

    [HideInInspector]
    RoulettePieceData selected = null;

    
    public override void Init()
    {
        base.Init();

        playerData = FindObjectOfType<GameData>();

        UpdateText();
        _time = 0.0f;

        button.onClick.AddListener(() => {

          
            if (!IsPlayableRoulette())
                return;

            PlayRoulette();


        });

        AddUIEvent(ExitButton.gameObject, (data) => {

            // �귿�� ������ ���϶� �����ư�� ������ ������ �����Ѵ�.
            if (roulette.IsSpinning)
                GiveToUser(selected);

            UpdateToServer();
            ClosePopupUI();
        });

        AddUIEvent(CloseButton.gameObject, (data) => {

            
        });

    }
    private void Update()
    {
        _time += Time.deltaTime;
        if(_time >= _updateDelay)
        {
            _time = 0.0f;
            UpdateToServer();
        }
    }
    bool IsPlayableRoulette()
    {
        // ���� �귿�� ������ ������
        if (roulette.IsSpinning)
            return false;

        // �귿������ �ִ���
        if (playerData.Currency[Define.CurrencyID.Coin.ToString()].Amount >= playerData.RouletteInfo.GetCoinToPlayRoulette)
            return true;
        else
            return false;
    }
    void UpdateText()
    {
        PlayRouletteCountText.text = $"�귿 ���� ��� Ƚ�� : {playerData.RouletteInfo.PlayRouletteCount}��";
        CoinToPlayRouletteText.text = $"1ȸ�� �귿 ���� ��� ��ȭ : {playerData.RouletteInfo.GetCoinToPlayRoulette}��";
    }
    void UpdateToServer()
    {
        playerData.UpdateData(PlayerInfo.UserDataKey.RouletteInfo, Managers.Player.GetPlayer(Managers.Game.PlayerId));
        Managers.Network.UpdateUserData(new PlayerInfo.UserDataKey[] { PlayerInfo.UserDataKey.RouletteInfo }, Managers.Player.GetPlayer(Managers.Game.PlayerId));

    }
    void PlayRoulette()
    {

        Managers.Game.SubstractCurrency(Define.CurrencyID.Coin.ToString(), playerData.RouletteInfo.GetCoinToPlayRoulette, IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
        playerData.RouletteInfo.PlayRouletteCount++;
        UpdateText();
        selected = roulette.Spin(GiveToUser);

    }

    void GiveToUser(RoulettePieceData selectedItem)
    {
        if(selectedItem.Rewardtype == RoulettePieceData.RewardType.Currency)
        {
            Managers.Game.AddCurrency(selectedItem.Id, selectedItem.Amount, IsUpdate: Managers.Network.IS_ENABLE_NETWORK);
        }
        else if(selectedItem.Rewardtype == RoulettePieceData.RewardType.Item)
        {
            BaseItem item = Managers.Item.Database.ItemList.Where(x => x.ItemId == selectedItem.Id).FirstOrDefault();
            if(item)
            {
                Managers.Network.GrantItems(new List<PlayFab.ServerModels.ItemGrant> { item.ToGrantItem() },
                    (result) => {

                        PlayFab.ServerModels.GrantItemsToUsersResult _result = result as PlayFab.ServerModels.GrantItemsToUsersResult;
                        foreach (PlayFab.ServerModels.GrantedItemInstance item in _result.ItemGrantResults)
                        {
                            Managers.Item.GrantItemToUser(item.ItemId).First().Setup(new PlayFab.ClientModels.ItemInstance
                            {
                                ItemId = item.ItemId,
                                Expiration = item.Expiration,
                                ItemClass = item.ItemClass,
                                CatalogVersion = item.CatalogVersion,
                                DisplayName = item.DisplayName,
                                ItemInstanceId = item.ItemInstanceId,
                                UnitCurrency = item.UnitCurrency,
                                UnitPrice = item.UnitPrice,
                                PurchaseDate = item.PurchaseDate,
                                RemainingUses = item.RemainingUses,
                                UsesIncrementedBy = item.UsesIncrementedBy,
                                CustomData = item.CustomData
                            });
                        }
                    });
            }
        }
    }

}
