using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quest/Reward/Mail")]
public class MailReward : Reward
{
    [SerializeField]
    Mail _reward;

   
    public override string GetId()
    {
        return _reward.ItemId;
    }

    public override void Give(Quest quest)
    {
        Managers.Item.GrantItemToUser(_reward.ItemId, new Dictionary<string, string>() { { "CodeName", quest.CodeName } });
    }

    
}
