using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json.Linq;
using com.onethesoft.GodOfArcher;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

public class QuestManager
{
    #region Save Path
    private const string kSaveRootPath = "questSystem";
    public const string kActiveQuestsSavePath = "activeQuests";
    public const string kCompletedQuestsSavePath = "completedQuests";
    #endregion

    #region Save Quest Group

    #endregion
    #region Events
    public delegate void QuestRegisteredHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);

    public delegate void QuestResetHandler(string Category);
    #endregion

    private List<Quest> _activeQuests = new List<Quest>();
    private List<Quest> _completedQuests = new List<Quest>();
    private List<Quest> _saveActiveQuests = new List<Quest>();
    private List<Quest> _saveCompletedQuests = new List<Quest>();
    QuestUpdater _questUpdater;



    public IReadOnlyList<Quest> ActiveQuests => _activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => _completedQuests;



    public event QuestRegisteredHandler onQuestRegistered;
    public event QuestCompletedHandler onQuestCompleted;
    public event QuestCanceledHandler onQuestCanceled;

    public event QuestResetHandler onQuestReset;

    QuestDataBase _questDatabase;

    const string savePath = "/QuestSaveData.json";

    #region UserData QuestGroup
   
    


    #endregion
    public void Init()
    {
        _questDatabase = Resources.Load<QuestDataBase>("Database/QuestDatabase");

        GameObject questUpdater = new GameObject {  name = "QuestUpdater"};
        questUpdater.transform.parent = GameObject.FindObjectOfType<Managers>().gameObject.transform;
        _questUpdater = Util.GetOrAddComponent<QuestUpdater>(questUpdater);

        GameObject.DontDestroyOnLoad(questUpdater);
      
       /*
        foreach(var quest in _questDatabase.Quests)
        {
            Register(quest);
        }
       */

        //foreach (var quest in _questDatabase.Quests)
        //Register(quest);


        if (Managers.Network.IS_ENABLE_NETWORK == false)
            if(!Load())
            {
                foreach (var quest in _questDatabase.Quests)
                    Register(quest);
            }
        

    }

    public Quest Register(Quest quest)
    {
        
        var newQuest = quest.Clone();

        newQuest.onCompleted += OnQuestCompleted;
        newQuest.onCanceled += OnQuestCanceled;

       
        _activeQuests.Add(newQuest);

        newQuest.OnRegister();
        onQuestRegistered?.Invoke(newQuest);
        

        return newQuest;
        

    }

    public List<Quest> FindQuestByCategory(string category)
    {
        List<Quest> _return = new List<Quest>();

        foreach (Quest quest in _completedQuests)
        {
            if (quest.Category == category)
                _return.Add(quest);
        }
        foreach (Quest quest in _activeQuests)
        {
            if (quest.Category == category)
                _return.Add(quest);
        }
        
        _return.Sort(CompareQuestByCodeName);
        return _return;

    }
    private int CompareQuestByCodeName(Quest x, Quest y)
    {
        if (x.CodeName == null)
        {
            if (y.CodeName == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (y.CodeName == null)
                return 1;
            else
            {
                string x_Str = x.CodeName.Substring(x.CodeName.LastIndexOf("_") + 1);
                string y_Str = y.CodeName.Substring(y.CodeName.LastIndexOf("_") + 1);
                //string x_Str = Regex.Replace(x.CodeName, @"\D", "");
                //string y_Str = Regex.Replace(y.CodeName, @"\D", "");

                int x_ = int.Parse(x_Str);
                int y_ = int.Parse(y_Str);

                return x_.CompareTo(y_);
            }
        }
    }

    void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        
        foreach (var quest in quests)
        {
            quest.ReceiveReport(category, target, successCount);
        }
        
        
    }
    void ReceiveReport(List<QuestGroup> questGroups, string category, object target, int successCount)
    {
        foreach (var questGroup in questGroups.ToArray())
        {
            questGroup.ReceiveReport(category, target, successCount);
        }
    }
    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(_activeQuests, category, target, successCount);
        //ReceiveReport(_questGroups, category , target, successCount);
    }

    public void ReceiveReport(Category category, TaskTarget target, int successCount)
    {
        ReceiveReport(category.CodeName, target.Value, successCount);
    }

    
    public void ResetQuests(string [] category)
    {
        if (category == null || category.Length == 0)
            return;

        List<PlayerInfo.UserDataKey> _questKey = new List<PlayerInfo.UserDataKey>();

        // Category 에 해당하는 퀘스트를 모두 Reset 한다.
        foreach(string it in category)
        {
            List<Quest> _resetableQuests = _completedQuests.Where(x => x.Category == it).ToList();
            _resetableQuests.AddRange(_activeQuests.Where(x => x.Category == it).ToList());

            _resetableQuests.Sort(CompareQuestByCodeName);
            _resetableQuests.ForEach(x => x.Reset());

            PlayerInfo.UserDataKey _key;

            QuestFinder.TryGetQuestKey(_resetableQuests.First().Category, out _key);
            if (_questKey.Any(x => x == _key) == false)
                _questKey.Add(_key);
        }
     

        // 서버에 저장

        if (Managers.Network.IS_ENABLE_NETWORK == true)
            foreach (PlayerInfo.UserDataKey update in _questKey)
            {
                Save(update);
            }

        


        // 이벤트 발생
        foreach (string it in category)
        {
            onQuestReset?.Invoke(it);
        }




    }
    
    public void Save()
    {
        var root = new JObject();
        root.Add(kActiveQuestsSavePath, CreateSaveDatas(ActiveQuests));
        root.Add(kCompletedQuestsSavePath, CreateSaveDatas(CompletedQuests));


        string _savePath = Application.persistentDataPath + savePath;
        File.WriteAllText(_savePath, root.ToString(),System.Text.Encoding.UTF8);
       

    }
    public void Save(PlayerInfo.UserDataKey key)
    {
        if(QuestFinder.FindKeys().Any(x => x==key))
        {
            _saveActiveQuests.Clear();
            _saveCompletedQuests.Clear();

            foreach (Quest quest in _activeQuests)
                if (QuestFinder.IsContain(quest.Category, key))
                {
                   
                    _saveActiveQuests.Add(quest);
                }

            foreach (Quest quest in _completedQuests)
                if (QuestFinder.IsContain(quest.Category, key))
                {

                    _saveCompletedQuests.Add(quest);
                }


            var root = new JObject();
            root.Add(kActiveQuestsSavePath, CreateSaveDatas(_saveActiveQuests));
            root.Add(kCompletedQuestsSavePath, CreateSaveDatas(_saveCompletedQuests));

            if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserData.ContainsKey(key.ToString()))
                Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserData[key.ToString()].Value = root.ToString();
            else
                Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserData.Add(key.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = root.ToString() });

         
            Managers.Network.UpdateUserData(new PlayerInfo.UserDataKey[] { key }, Managers.Player.GetPlayer(Managers.Game.PlayerId));
        }
      
    }

    public void Save(PlayerInfo.UserDataKey [] key)
    {
        IEnumerable<PlayerInfo.UserDataKey> questKey = QuestFinder.FindKeys().Intersect(key);
        if (questKey.ToList().Count == 0)
            return;

     

        foreach (PlayerInfo.UserDataKey saveKey in questKey)
        {
            _saveActiveQuests.Clear();
            _saveCompletedQuests.Clear();

            if (QuestFinder.FindKeys().Any(x => x== saveKey))
            {
                foreach (Quest quest in _activeQuests)
                    if (QuestFinder.IsContain(quest.Category, saveKey))
                        _saveActiveQuests.Add(quest);

                foreach (Quest quest in _completedQuests)
                    if (QuestFinder.IsContain(quest.Category, saveKey))
                        _saveCompletedQuests.Add(quest);

                var root = new JObject();
                root.Add(kActiveQuestsSavePath, CreateSaveDatas(_saveActiveQuests));
                root.Add(kCompletedQuestsSavePath, CreateSaveDatas(_saveCompletedQuests));

                if (Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserData.ContainsKey(saveKey.ToString()))
                    Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserData[saveKey.ToString()].Value = root.ToString();
                else
                    Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserData.Add(saveKey.ToString(), new PlayFab.ClientModels.UserDataRecord { Value = root.ToString() });
            }

        }

        Managers.Network.UpdateUserData(questKey.ToArray(), Managers.Player.GetPlayer(Managers.Game.PlayerId));
        

    }
    bool Load()
    {
        string _savePath = Application.persistentDataPath + savePath;
        if (!File.Exists(_savePath))
            return false;
       
        var root = JObject.Parse(File.ReadAllText(_savePath,System.Text.Encoding.UTF8));

        LoadSaveDatas(root[kActiveQuestsSavePath], _questDatabase, LoadActiveQuest);
        LoadSaveDatas(root[kCompletedQuestsSavePath], _questDatabase, LoadCompletedQuest);

        return true;
        
    }

    public void Load(PlayerInfo userdata)
    {
        foreach(PlayerInfo.UserDataKey key in QuestFinder.FindKeys())
        {
            if(userdata.Payload.UserData.ContainsKey(key.ToString()))
            {
                var root = JObject.Parse(userdata.Payload.UserData[key.ToString()].Value);
                LoadSaveDatas(root[kActiveQuestsSavePath], _questDatabase, LoadActiveQuest);
                LoadSaveDatas(root[kCompletedQuestsSavePath], _questDatabase, LoadCompletedQuest);
            }
            else
            {
                PlayerInfo.TitleDataKey findQuestKey;
                if (System.Enum.TryParse(key.ToString(), out findQuestKey))
                {
                    var root = JObject.Parse(userdata.Payload.TitleData[key.ToString()]);
                    LoadSaveDatas(root[kActiveQuestsSavePath], _questDatabase, LoadActiveQuest);
                    LoadSaveDatas(root[kCompletedQuestsSavePath], _questDatabase, LoadCompletedQuest);
                }
                else
                    Debug.LogError("QuestManager.Load : not Find QuestKey from TitleData");
               
               
                
            }
        }
        
    }
    

    private JArray CreateSaveDatas(IReadOnlyList<Quest> quests)
    {
        var saveDatas = new JArray();
        foreach (var quest in quests)
        {
            if (quest.IsSavable)
            {
                QuestSaveData _saveData = Managers.Pool.Pop<QuestSaveData>();
                quest.CopySaveData(_saveData);
                saveDatas.Add(JObject.FromObject(_saveData));
                Managers.Pool.Push(_saveData);
            }
        }

        return saveDatas;

    }

    private void LoadSaveDatas(JToken datasToken, QuestDataBase database, System.Action<QuestSaveData, Quest> onSucess)
    {
        var datas = datasToken as JArray;
        foreach (var data in datas)
        {
            var saveData = data.ToObject<QuestSaveData>();
            var quest = database.FindQuestBy(saveData.CodeName);
            //Debug.Log("LoadSaveDatas : " + saveData.CodeName);
            onSucess?.Invoke(saveData, quest);

        }
    }

    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);
        newQuest.LoadFrom(saveData);

        newQuest.OnReseted -= OnQuestReseted;
        newQuest.OnReseted += OnQuestReseted;
    }

    private void LoadCompletedQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);

        
        _completedQuests.Add(newQuest);

        newQuest.OnReseted -= OnQuestReseted;
        newQuest.OnReseted += OnQuestReseted;
    }

    
    #region Callback

    void OnQuestCompleted(Quest quest)
    {
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);

       
        onQuestCompleted?.Invoke(quest);
    }
    void OnQuestCanceled(Quest quest)
    {
        _activeQuests.Remove(quest);


        onQuestCanceled?.Invoke(quest);

        
    }

    void OnQuestReseted(Quest quest)
    {
        _completedQuests.Remove(quest);

        if(_activeQuests.Contains(quest))
            _activeQuests.Remove(quest);

        _activeQuests.Add(quest);


        quest.onCompleted -= OnQuestCompleted;
        quest.onCanceled -= OnQuestCanceled;

        quest.onCompleted += OnQuestCompleted;
        quest.onCanceled += OnQuestCanceled;


       

        
    }



    #endregion



}
