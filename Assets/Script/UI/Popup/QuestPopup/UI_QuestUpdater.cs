using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_QuestUpdater : MonoBehaviour
{
    Queue<Quest> _completeQuests;

    Coroutine _coroutine = null;
    WaitForSeconds _delay = new WaitForSeconds(1f);
    // Start is called before the first frame update
    void Start()
    {
        _completeQuests = new Queue<Quest>();
        Managers.Quest.onQuestCompleted -= AddCompletedQuest;
        Managers.Quest.onQuestCompleted += AddCompletedQuest;

        _coroutine = StartCoroutine(UpdateCoroutine());
    }
    void AddCompletedQuest(Quest quest)
    {
        _completeQuests.Enqueue(quest);
    }
    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            ProcessUpdateQuest();
            yield return _delay;
        }
    }
    public void ProcessUpdateQuest()
    {
        if (Managers.Network.IS_ENABLE_NETWORK == false)
        {
            _completeQuests.Clear();
            return;
        }


        if (_completeQuests.Count <= 0)
            return;

        Dictionary<string, System.Numerics.BigInteger> _currencyAmounts = new Dictionary<string, System.Numerics.BigInteger>();
        List<PlayFab.ServerModels.ItemGrant> _listGrantedItem = new List<PlayFab.ServerModels.ItemGrant>();
        List<PlayerInfo.UserDataKey> _updateKeys = new List<PlayerInfo.UserDataKey>();

        foreach (Quest quest in _completeQuests)
        {
            // 사실상 퀘스트당 보상은 한개라서 적용가능
            // Reward 는 재화인 루비와 우편, 아이템이 있다.
            foreach (Reward reward in quest.Rewards)
            {
                if (reward is CurrencyReward)
                {
                    if (_currencyAmounts.ContainsKey(reward.GetId()))
                        _currencyAmounts[reward.GetId()] += reward.Quantity;
                    else
                        _currencyAmounts.Add(reward.GetId(), reward.Quantity);
                }
                else
                {
                    if (Managers.Item.Database.ItemList.Where(x => x.ItemId == reward.GetId()).FirstOrDefault().IsStackable == true)
                        _listGrantedItem.Add(Managers.Game.GetInventory().Find(x => x.ItemId == reward.GetId()).ToGrantItem());
                    else
                        _listGrantedItem.Add(Managers.Game.GetInventory().Find(x => x.GetCustomData("CodeName") == quest.CodeName).ToGrantItem());

                }

            }


            PlayerInfo.UserDataKey _getQuestKey;
            if (QuestFinder.TryGetQuestKey(quest, out _getQuestKey))
            {
                if (_updateKeys.Any(x => x == _getQuestKey) == false)
                    _updateKeys.Add(_getQuestKey);
            }

        }

        if (Managers.Network.IS_ENABLE_NETWORK == true)
        {
            foreach (KeyValuePair<string, System.Numerics.BigInteger> _pair in _currencyAmounts)
                if (_pair.Key == Define.CurrencyID.Ruby.ToString())
                {
                    Managers.Network.AddCurrency(PlayerInfo.CurrencyKey.RB, (int)_currencyAmounts[_pair.Key], Managers.Player.GetPlayer(Managers.Game.PlayerId));
                }

            if (_listGrantedItem.Count > 0)
            {
                Managers.Network.GrantItems(_listGrantedItem,
                    (result) =>
                    {
                        PlayFab.ServerModels.GrantItemsToUsersResult _result = result as PlayFab.ServerModels.GrantItemsToUsersResult;

                        foreach (PlayFab.ServerModels.GrantedItemInstance itemGrant in _result.ItemGrantResults)
                        {
                            if (Managers.Item.Database.ItemList.Where(x => x.ItemId == itemGrant.ItemId).FirstOrDefault().IsStackable == true)
                                Managers.Game.GetInventory().Find(x => x.ItemId == itemGrant.ItemId).Setup(new PlayFab.ClientModels.ItemInstance
                                {
                                    CatalogVersion = itemGrant.CatalogVersion,
                                    CustomData = itemGrant.CustomData,
                                    ItemClass = itemGrant.ItemClass,
                                    ItemId = itemGrant.ItemId,
                                    ItemInstanceId = itemGrant.ItemInstanceId,
                                    DisplayName = itemGrant.DisplayName,
                                    RemainingUses = itemGrant.RemainingUses,
                                    UsesIncrementedBy = itemGrant.UsesIncrementedBy,
                                    PurchaseDate = itemGrant.PurchaseDate,
                                    Expiration = itemGrant.Expiration
                                });
                            else        // Mail
                                Managers.Game.GetInventory().Find(x => x.GetCustomData("CodeName") == itemGrant.CustomData["CodeName"]).Setup(new PlayFab.ClientModels.ItemInstance
                                {
                                    CatalogVersion = itemGrant.CatalogVersion,
                                    CustomData = itemGrant.CustomData,
                                    ItemClass = itemGrant.ItemClass,
                                    ItemId = itemGrant.ItemId,
                                    ItemInstanceId = itemGrant.ItemInstanceId,
                                    DisplayName = itemGrant.DisplayName,
                                    RemainingUses = itemGrant.RemainingUses,
                                    UsesIncrementedBy = itemGrant.UsesIncrementedBy,
                                    PurchaseDate = itemGrant.PurchaseDate,
                                    Expiration = itemGrant.Expiration
                                });
                        }
                    });


            }

            foreach (PlayerInfo.UserDataKey key in _updateKeys)
            {
                Managers.Quest.Save(key);
                if (key == PlayerInfo.UserDataKey.SubQuest)
                {
                    Managers.Game.Save(PlayerInfo.UserDataKey.DailyCheckoutQuestLastClearTime);
                    Managers.Game.Save(PlayerInfo.UserDataKey.DailyQuestLastClearTime);
                }
            }
        }



        _completeQuests.Clear();
    }
    private void OnDestroy()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        Managers.Quest.onQuestCompleted -= AddCompletedQuest;
        
    }
}
