using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestFinder 
{
    private static List<PlayerInfo.UserDataKey> _list = new List<PlayerInfo.UserDataKey> { PlayerInfo.UserDataKey.MainQuest, PlayerInfo.UserDataKey.SubQuest, PlayerInfo.UserDataKey.SeasonpassQuest, PlayerInfo.UserDataKey.GambleQuest , PlayerInfo.UserDataKey.Seasonpass3Quest };
    public static bool TryGetQuestKey(Quest quest, out PlayerInfo.UserDataKey key)
    {
        bool _isValidKey = _list.Any(x => IsContain(quest, x));
        key = _list.Where(x => IsContain(quest, x)).FirstOrDefault();
        return _isValidKey;

    }
    public static List<PlayerInfo.UserDataKey> FindKeys()
    {
        return _list;
    }
    public static bool IsContain(Quest quest, PlayerInfo.UserDataKey key)
    {
        if (key == PlayerInfo.UserDataKey.MainQuest)
        {
            return quest.Category == Define.QuestType.StageClear.ToString() ||
                quest.Category == Define.QuestType.BossMonsterKill.ToString() ||
                quest.Category == Define.QuestType.MonsterKill.ToString() ||
                quest.Category == Define.QuestType.PlayTime.ToString() ||
                quest.Category == Define.QuestType.TrollStageClear.ToString();
        }
        else if (key == PlayerInfo.UserDataKey.SubQuest)
        {
            return quest.Category == Define.QuestType.Daily.ToString() ||
                quest.Category == Define.QuestType.DailyCheckout.ToString() ||
                quest.Category == Define.QuestType.Tutorial.ToString();
        }
        else if (key == PlayerInfo.UserDataKey.SeasonpassQuest)
        {
            return quest.CodeName.Contains("free_") || quest.CodeName.Contains("pass_") ||
                quest.CodeName.Contains("pass2") || quest.CodeName.Contains("pass3");
        }
        else if(key == PlayerInfo.UserDataKey.Seasonpass3Quest)
        {
            return quest.CodeName.Contains("pass4") || quest.CodeName.Contains("pass5");
        }
        else if (key == PlayerInfo.UserDataKey.GambleQuest)
        {
            return quest.Category == Define.QuestType.GambleRune.ToString() ||
                quest.Category == Define.QuestType.GamblePet.ToString() ||
                quest.Category == Define.QuestType.GambleBow.ToString() ||
                quest.Category == Define.QuestType.GambleArmor.ToString() ||
                quest.Category == Define.QuestType.GambleHelmet.ToString() ||
                quest.Category == Define.QuestType.GambleCloak.ToString();
        }
        else
            return false;
    }
   
}
