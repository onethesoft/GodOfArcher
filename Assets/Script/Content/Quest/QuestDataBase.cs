using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Quest/QuestDatabase")]
public class QuestDataBase : ScriptableObject
{
    [SerializeField]
    private List<Quest> _quests = new List<Quest>();

    public IReadOnlyList<Quest> Quests => _quests;


    public Quest FindQuestBy(string codeName) => _quests.FirstOrDefault(x => x.CodeName == codeName);
    public List<Quest> FindQuestByCategory(string category) => _quests.FindAll(x => x.Category == category);

    
#if UNITY_EDITOR
    [ContextMenu("FindQuests")]
    private void FindQuests()
    {
        FindQuestBy<Quest>();
    }

    [ContextMenu("CreateTitleData")]
    private void CreateFile()
    {
        Dictionary<PlayerInfo.UserDataKey, List<Quest>> _dictQuestGroup = new Dictionary<PlayerInfo.UserDataKey, List<Quest>>();
        string _savePath = Application.persistentDataPath;

        foreach (PlayerInfo.UserDataKey key in QuestFinder.FindKeys())
            _dictQuestGroup.Add(key, new List<Quest>());
        
        foreach(var Quest in _quests)
            _dictQuestGroup[_dictQuestGroup.Keys.Where(x => QuestFinder.IsContain(Quest.Category, x)).First()].Add(Quest);
        
        foreach(KeyValuePair<PlayerInfo.UserDataKey, List<Quest>> pair in _dictQuestGroup)
        {
            var root = new JObject();
            var _activeArray = new JArray();

            foreach (Quest quest in pair.Value)
                _activeArray.Add(JObject.FromObject(quest.ToSaveData()));

            root.Add(QuestManager.kActiveQuestsSavePath, _activeArray);
            root.Add(QuestManager.kCompletedQuestsSavePath, new JArray());

            System.IO.File.WriteAllText($"{_savePath}/{pair.Key.ToString()}.json" , root.ToString(), System.Text.Encoding.UTF8);

        }

       
    }


    private void FindQuestBy<T>() where T : Quest
    {
        _quests = new List<Quest>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (quest.GetType() == typeof(T))
                _quests.Add(quest);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }


#endif
}



