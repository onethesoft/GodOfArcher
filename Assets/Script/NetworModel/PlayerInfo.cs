using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;

[Serializable]
public class PlayerInfo 
{
   
    public enum UserDataKey
    {
        SessionInfo, // ����̽� ���� �� ���� �ð� ���
        PlayerStat, // �÷��̾� ����
        Gold,   // �÷��̾� ��ȭ
        CP,     // �÷��̾� ��ȭ
        Rune,       // �� ��������
        Pet,        // �� ��������
        Bow,        // Ȱ ���� ����
        Helmet,     // ���� ���� ����
        Armor,      // ���� ���� ����
        Cloak,      // ���� ���� ����
        MainQuest,        // �������� , ���� óġ , �������� óġ , �÷���Ÿ�� , Ʈ���� ������ ����Ʈ
        SubQuest ,          // ���� , Ʃ�丮�� , �����⼮ ����Ʈ
        SeasonpassQuest,    // �����н� ����Ʈ
        GambleQuest,         // �̱� ����Ʈ
        DailyCheckoutQuestLastClearTime,    // �����⼮���� ����Ʈ Ŭ���� �ð�
        DailyQuestLastClearTime,            // ���� ����Ʈ Ŭ���� �ð�
        IsBeginner,  // ó�� ���� �� �ʺ��� ���� �ο�
        DPS,
        StageInfo,
        ReviveInfo,

    }

    public enum TitleDataKey
    {
        ServiceState, // ���� ���� ���� �� ���� ���� 
        MainQuest,    // �������� , ���� óġ , �������� óġ , �÷���Ÿ�� , Ʈ���� ������ ����Ʈ
        SubQuest,   // ���� , Ʃ�丮�� , �����⼮ ����Ʈ
        SeasonpassQuest,    // �����н� ����Ʈ
        GambleQuest,     // �̱� ����Ʈ
        Rune,       // �� ��������
        Pet,        // �� ��������
        Bow,        // Ȱ ���� ����
        Helmet,     // ���� ���� ����
        Armor,      // ���� ���� ����
        Cloak,      // ���� ���� ����
        IsBeginner  // ó�� ���� �� �ʺ��� ���� �ο�
    }

    public enum StatisticsDataKey
    {
        Level,
        Stage,
        ClearStage,
        MaxClearStage,
        TrollStage,
        PlayTime,
        ItemEquipment,   // ��ŷ�гο��� ������ ���������� ǥ���ϱ� ����
    }

    public enum CurrencyKey
    {
        RB, // ��� ��ȭ�� ��� �� ȯ������Ʈ�� UserData �� ����ȴ�.
        SP
    }

    public string PlayfabId;
    public string DisplayName;
    public SessionInfo Session;
    public SessionInfo LastSession;
    public LoginResult LoginResult;
    public GetPlayerCombinedInfoResultPayload Payload;

    public void AddItemInstance(PlayFab.ClientModels.ItemInstance item)
    {
      
        ItemInstance _findItem = Payload.UserInventory.Where(x => x.ItemInstanceId == item.ItemInstanceId).FirstOrDefault();
        if(_findItem == null)
            Payload.UserInventory.Add(item);
        else
            _findItem = item;
        
        
    }
    public void AddItemInstance(PlayFab.ServerModels.GrantedItemInstance item)
    {
        PlayFab.ClientModels.ItemInstance _findItem = Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserInventory.Where(x => x.ItemInstanceId == item.ItemInstanceId).FirstOrDefault();
        if (_findItem == null)
        {
            if( Managers.Item.Database.ItemList.Where(x=>x.ItemId == item.ItemId).First().IsStackable == true)
            {
                if( Payload.UserInventory.Where(x=>x.ItemId == item.ItemId).FirstOrDefault() != null)
                    Payload.UserInventory.RemoveAll(x => x.ItemId == item.ItemId);

            }

            _findItem = new PlayFab.ClientModels.ItemInstance
            {
                ItemId = item.ItemId,
                ItemInstanceId = item.ItemInstanceId,
                Annotation = item.Annotation,
                DisplayName = item.DisplayName,
                Expiration = item.Expiration,
                CatalogVersion = item.CatalogVersion,
                ItemClass = item.ItemClass,
                UsesIncrementedBy = item.UsesIncrementedBy,
                RemainingUses = item.RemainingUses,
                PurchaseDate = item.PurchaseDate,
                BundleContents = item.BundleContents,
                BundleParent = item.BundleParent,
                CustomData = item.CustomData,
                UnitCurrency = item.UnitCurrency,
                UnitPrice = item.UnitPrice
            };

            Payload.UserInventory.Add(_findItem);


        }
        else
        {
            _findItem.DisplayName = item.DisplayName;
            _findItem.CustomData = item.CustomData;
            _findItem.RemainingUses = item.RemainingUses;
            _findItem.PurchaseDate = item.PurchaseDate;
            _findItem.UsesIncrementedBy = item.UsesIncrementedBy;
            _findItem.Expiration = item.Expiration;
        }
    }


    public void AddCurrency(Define.CurrencyID _currencyId , System.Numerics.BigInteger amount)
    {
        if(_currencyId == Define.CurrencyID.Gold)
        {
            UserDataRecord _data;
            if(Payload.UserData.TryGetValue(UserDataKey.Gold.ToString() , out _data))
            {
                System.Numerics.BigInteger currentAmount = System.Numerics.BigInteger.Parse(_data.Value);
                currentAmount += amount;
                _data.Value = currentAmount.ToString();
            }
            else
                Payload.UserData.Add(UserDataKey.Gold.ToString(), new UserDataRecord { Value = amount.ToString() });
            
        }
        else if(_currencyId == Define.CurrencyID.CP)
        {
            UserDataRecord _data;
            if (Payload.UserData.TryGetValue(UserDataKey.CP.ToString(), out _data))
            {
                System.Numerics.BigInteger currentAmount = System.Numerics.BigInteger.Parse(_data.Value);
                currentAmount += amount;
                _data.Value = currentAmount.ToString();
            }
            else
                Payload.UserData.Add(UserDataKey.CP.ToString(), new UserDataRecord { Value = amount.ToString() });
        }
        else if(_currencyId == Define.CurrencyID.Ruby)
        {
            int currentAmount;
            if (Payload.UserVirtualCurrency.TryGetValue(CurrencyKey.RB.ToString(), out currentAmount))
            {
                Payload.UserVirtualCurrency[CurrencyKey.RB.ToString()] += (int)amount;
            }
            else
                Payload.UserVirtualCurrency.Add(CurrencyKey.RB.ToString(), (int)amount);
        }
        else // SP
        {
            int currentAmount;
            if (Payload.UserVirtualCurrency.TryGetValue(CurrencyKey.SP.ToString(), out currentAmount))
            {
                Payload.UserVirtualCurrency[CurrencyKey.SP.ToString()] += (int)amount;
            }
            else
                Payload.UserVirtualCurrency.Add(CurrencyKey.SP.ToString(), (int)amount);
        }
    }
    public void SubstractCurrency(Define.CurrencyID _currencyId, System.Numerics.BigInteger amount)
    {
        if (_currencyId == Define.CurrencyID.Gold)
        {
            UserDataRecord _data;
            if (Payload.UserData.TryGetValue(UserDataKey.Gold.ToString(), out _data))
            {
                System.Numerics.BigInteger currentAmount = System.Numerics.BigInteger.Parse(_data.Value);
                currentAmount -= amount;
                if (currentAmount < 0) 
                    currentAmount = 0;
                _data.Value = currentAmount.ToString();
            }
            

        }
        else if (_currencyId == Define.CurrencyID.CP)
        {
            UserDataRecord _data;
            if (Payload.UserData.TryGetValue(UserDataKey.CP.ToString(), out _data))
            {
                System.Numerics.BigInteger currentAmount = System.Numerics.BigInteger.Parse(_data.Value);
                currentAmount -= amount;
                if (currentAmount < 0)
                    currentAmount = 0;
                _data.Value = currentAmount.ToString();
            }
        }
        else if (_currencyId == Define.CurrencyID.Ruby)
        {
            int currentAmount;
            if (Payload.UserVirtualCurrency.TryGetValue(CurrencyKey.RB.ToString(), out currentAmount))
            {
                Payload.UserVirtualCurrency[CurrencyKey.RB.ToString()] -= (int)amount;
                if (Payload.UserVirtualCurrency[CurrencyKey.RB.ToString()] < 0) 
                    Payload.UserVirtualCurrency[CurrencyKey.RB.ToString()] = 0;
            }
            
        }
        else // SP
        {
            int currentAmount;
            if (Payload.UserVirtualCurrency.TryGetValue(CurrencyKey.SP.ToString(), out currentAmount))
            {
                Payload.UserVirtualCurrency[CurrencyKey.SP.ToString()] -= (int)amount;
                if (Payload.UserVirtualCurrency[CurrencyKey.SP.ToString()] < 0)
                    Payload.UserVirtualCurrency[CurrencyKey.SP.ToString()] = 0;
            }
            
        }
    }
    public SessionInfo CreateSession()
    {
        Session = new SessionInfo { DeviceId = SystemInfo.deviceUniqueIdentifier, LastAccessTime = GlobalTime.Now };
        return Session;
    }
}
