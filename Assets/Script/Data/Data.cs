using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.onethesoft.GodOfArcher
{
    [System.Serializable]
    public class QuestSaveData
    {
        public string CodeName;
        public QuestState state;
        public int TaskGroupIndex;
        public int[] TaskSuccessCount;

        public QuestSaveData()
        {
            CodeName = string.Empty;
            state = QuestState.Inactive;
            TaskGroupIndex = 0;
        }

    }

    [System.Serializable]
    public class StageData
    {
        public int Level;
        public string Normal_Hp;
        public string Boss_Hp;
        public string Total_Gold;
        public string Total_Gold_Sum;
        public string Splash_Point;
        public string CP;
        public string Total_Hp;
        public string Total_Summanry;
    }

    [System.Serializable]
    public class TextData
    {
        public int ID;
        public string Kor;
        public string En;
        
    }

    [System.Serializable]
    public class TextDataArray : ILoader<int, TextData>
    {
        public List<TextData> data;

        public Dictionary<int, TextData> MakeDict()
        {
            Dictionary<int, TextData> dict = new Dictionary<int, TextData>();
            foreach (TextData item in data)
            {
                dict.Add(item.ID, item);
            }
            return dict;
        }

    }

    #region PopupItem

    [System.Serializable]
    public class StatItemDataArray : ILoader<int, StatItemData>
    {
        public List<StatItemData> data;

        public Dictionary<int , StatItemData> MakeDict()
        {
            Dictionary<int, StatItemData> dict = new Dictionary<int, StatItemData>();
            foreach(StatItemData item in data)
            {
                dict.Add(item.ID, item);
            }
            return dict;
        }

    }

    [System.Serializable]
    public class StatItemData
    {
        public int ID;
        public string CodeName;
        public int LevelText;
        public int IncrementStatText;
        public int MaxLevelText;
        public int StatText;

        public string Icon;
        public string BackIcon;
        public string CurrencyIcon;
        public int LevelupButtonText;
        public int LevelupCostText;
        public int TotalLevelupButtonText;

    }

    #endregion



    

    [System.Serializable]
    public class StatData
    {
        public int Level;
        public int Increment;
        public int TotalIncrement;
        public int Cost;
        public int TotalCost;


    }
}