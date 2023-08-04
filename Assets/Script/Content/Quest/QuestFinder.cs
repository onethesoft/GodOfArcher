using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestFinder 
{
    private static List<PlayerInfo.UserDataKey> _list = new List<PlayerInfo.UserDataKey> { PlayerInfo.UserDataKey.MainQuest, PlayerInfo.UserDataKey.SubQuest, PlayerInfo.UserDataKey.SeasonpassQuest, PlayerInfo.UserDataKey.GambleQuest };
    public static bool TryGetQuestKey(Category category, out PlayerInfo.UserDataKey key)
    {
        bool _isValidKey = _list.Any(x => IsContain(category, x));
        key = _list.Where(x => IsContain(category, x)).FirstOrDefault();
        return _isValidKey;

    }
    public static List<PlayerInfo.UserDataKey> FindKeys()
    {
        return _list;
    }
    public static bool IsContain(Category questCategory, PlayerInfo.UserDataKey key)
    {
        if (key == PlayerInfo.UserDataKey.MainQuest)
        {
            return questCategory == Define.QuestType.StageClear.ToString() ||
                questCategory == Define.QuestType.BossMonsterKill.ToString() ||
                questCategory == Define.QuestType.MonsterKill.ToString() ||
                questCategory == Define.QuestType.PlayTime.ToString() ||
                questCategory == Define.QuestType.TrollStageClear.ToString();
        }
        else if (key == PlayerInfo.UserDataKey.SubQuest)
        {
            return questCategory == Define.QuestType.Daily.ToString() ||
                questCategory == Define.QuestType.DailyCheckout.ToString() ||
                questCategory == Define.QuestType.Tutorial.ToString();
        }
        else if (key == PlayerInfo.UserDataKey.SeasonpassQuest)
        {
            return questCategory == Define.QuestType.Seasonpass.ToString();
        }
        else if (key == PlayerInfo.UserDataKey.GambleQuest)
        {
            return questCategory == Define.QuestType.GambleRune.ToString() ||
                questCategory == Define.QuestType.GamblePet.ToString() ||
                questCategory == Define.QuestType.GambleBow.ToString() ||
                questCategory == Define.QuestType.GambleArmor.ToString() ||
                questCategory == Define.QuestType.GambleHelmet.ToString() ||
                questCategory == Define.QuestType.GambleCloak.ToString();
        }
        else
            return false;
    }

}
