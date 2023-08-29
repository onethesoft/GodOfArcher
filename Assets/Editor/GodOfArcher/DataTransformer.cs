using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

public class DataTransformer : EditorWindow
{
    const string DataFilePath = "Data/";
    [MenuItem("Tools/Csv를 Json 으로 저장")]
    static void ToJson()
    {
        string MonsterCsvFile = "MonsterStat_Test";
        WriteJsonFile($"{MonsterCsvFile}.json", ParseCsv(MonsterCsvFile));

        string TextDataCsvFile = "TextData";
        WriteJsonFile($"{TextDataCsvFile}.json", ParseCsv(TextDataCsvFile));

        string StatItemDataCsvFile = "StatItemData";
        WriteJsonFile($"{StatItemDataCsvFile}.json", ParseCsv(StatItemDataCsvFile));

        string AttackStatDataCsvFile = "AttackStatData";
        WriteJsonFile($"{AttackStatDataCsvFile}.json", ParseCsv(AttackStatDataCsvFile));

        string QuestCategoryCsvFile = "Quest_Category";
        WriteJsonFile($"{QuestCategoryCsvFile}.json", ParseCsv($"Quest/{QuestCategoryCsvFile}"));

        string QuestTableCsvFile = "QuestTable";
        WriteJsonFile($"{QuestTableCsvFile}.json", ParseCsv($"Quest/{QuestTableCsvFile}"));

        string StatDataCsvFile = "StatData";
        WriteJsonFile($"{StatDataCsvFile}.json", ParseCsv($"Stat/{StatDataCsvFile}"));

        string CurrencyDataCsvFile = "CurrencyData";
        WriteJsonFile($"{CurrencyDataCsvFile}.json", ParseCsv($"{CurrencyDataCsvFile}"));

    }


    static void WriteJsonFile(string filename , List<Dictionary<string,object>> content)
    {
        
        JObject _root = new JObject(); 
        var _list = new JArray();

        foreach (Dictionary<string, object> ite in content)
        {
            JObject c = new JObject();
            foreach (KeyValuePair<string, object> _row in ite)
                c.Add(_row.Key, _row.Value.ToString());

            _list.Add(c);
        }

        _root.Add("data", _list);
        try
        {
            StreamWriter df = new StreamWriter($"{Application.dataPath}/Resources/Data/{filename}", false, System.Text.Encoding.UTF8);
            df.Write(_root.ToString());
            df.Close();
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        AssetDatabase.Refresh();
        
        
        
    }
    static List<Dictionary<string, object>> ParseCsv(string filename)
    {
        const string LINE_SPLIT_RE = @"\n|\n\r|\r\n|\r";
        const string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        char[] TRIM_CHARS = { '\"' };

        string dataAsFile = File.ReadAllText($"{Application.dataPath}/Resources/Data/Csv/{filename}.csv", System.Text.Encoding.UTF8);

        var list = new List<Dictionary<string, object>>();
        var lines = Regex.Split(dataAsFile, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
            
                object finalvalue = value;
                entry[header[j]] = finalvalue;
                //Debug.Log(finalvalue);
            }
            list.Add(entry);
        }
        return list;



    }

    [MenuItem("Tools/아이템 팝업 신데이트 적용")]
    static void ApplyItemPopupData()
    {
        ItemDatabase db = null;
        UI_Item _itempopup = null;
        List<UI_EquipItemData> _updateList = new List<UI_EquipItemData>();
        string [] guid  = AssetDatabase.FindAssets($"t:{typeof(ItemDatabase)}");
        foreach(string path in guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(path);
            Debug.Log(assetPath);
            if (assetPath.Contains("Resources"))
                db = AssetDatabase.LoadAssetAtPath<ItemDatabase>(assetPath);
        }

        GameObject UI_itemPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/UI/Popup/UI_Item.prefab", typeof(GameObject));
        _itempopup = UI_itemPrefab.GetComponent<UI_Item>();


       
        foreach (EquipableItem item in db.ItemList.Where(x => x.ItemClass == "Bow").OrderBy(x => (x as EquipableItem).Level))
        {
            UI_EquipItemData d = new UI_EquipItemData();
            d.ItemId = item.ItemId;
            d.ItemClass = item.ItemClass;
            d.IconSprite = item.Icon;
            d.IconWrapperSprite = item.DescriptionBackground;
            d.DisplayNameOutline = item.DisplayNameOutLine.Copy();

            d.ItemPanelSprite = item.IconBackground;
            d.ItemPanelOutline = item.IconBackgroundOutline.Copy();

            d.BackgroundSprite = item.DescriptionBackgroundTexture;
            d.BackgroundColor = item.DescriptionBackgroundColor;
            d.BackgroundOutline = item.DescriptionBackgroundOutline.Copy();
            _updateList.Add(d);


        }
        foreach (EquipableItem item in db.ItemList.Where(x => x.ItemClass == "Helmet").OrderBy(x => (x as EquipableItem).Level))
        {
            UI_EquipItemData d = new UI_EquipItemData();
            d.ItemId = item.ItemId;
            d.ItemClass = item.ItemClass;
            d.IconSprite = item.Icon;
            d.IconWrapperSprite = item.DescriptionBackground;
            d.DisplayNameOutline = item.DisplayNameOutLine.Copy();

            d.ItemPanelSprite = item.IconBackground;
            d.ItemPanelOutline = item.IconBackgroundOutline.Copy();

            d.BackgroundSprite = item.DescriptionBackgroundTexture;
            d.BackgroundColor = item.DescriptionBackgroundColor;
            d.BackgroundOutline = item.DescriptionBackgroundOutline.Copy();
            _updateList.Add(d);


        }
        foreach (EquipableItem item in db.ItemList.Where(x => x.ItemClass == "Armor").OrderBy(x => (x as EquipableItem).Level))
        {
            UI_EquipItemData d = new UI_EquipItemData();
            d.ItemId = item.ItemId;
            d.ItemClass = item.ItemClass;
            d.IconSprite = item.Icon;
            d.IconWrapperSprite = item.DescriptionBackground;
            d.DisplayNameOutline = item.DisplayNameOutLine.Copy();

            d.ItemPanelSprite = item.IconBackground;
            d.ItemPanelOutline = item.IconBackgroundOutline.Copy();

            d.BackgroundSprite = item.DescriptionBackgroundTexture;
            d.BackgroundColor = item.DescriptionBackgroundColor;
            d.BackgroundOutline = item.DescriptionBackgroundOutline.Copy();
            _updateList.Add(d);


        }
        foreach (EquipableItem item in db.ItemList.Where(x => x.ItemClass == "Cloak").OrderBy(x => (x as EquipableItem).Level))
        {
            UI_EquipItemData d = new UI_EquipItemData();
            d.ItemId = item.ItemId;
            d.ItemClass = item.ItemClass;
            d.IconSprite = item.Icon;
            d.IconWrapperSprite = item.DescriptionBackground;
            d.DisplayNameOutline = item.DisplayNameOutLine.Copy();

            d.ItemPanelSprite = item.IconBackground;
            d.ItemPanelOutline = item.IconBackgroundOutline.Copy();

            d.BackgroundSprite = item.DescriptionBackgroundTexture;
            d.BackgroundColor = item.DescriptionBackgroundColor;
            d.BackgroundOutline = item.DescriptionBackgroundOutline.Copy();
            _updateList.Add(d);


        }
        _itempopup.SetupItemViewData(_updateList);
        EditorUtility.SetDirty(_itempopup);
        


    }

    [MenuItem("Tools/펫 팝업 신데이트 적용")]
    static void ApplyPetPopupData()
    {
        ItemDatabase db = null;
        UI_PetPopup _itempopup = null;
        List<UI_PetItemData> _updateList = new List<UI_PetItemData>();
        string [] guid  = AssetDatabase.FindAssets($"t:{typeof(ItemDatabase)}");
        foreach(string path in guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(path);
            Debug.Log(assetPath);
            if (assetPath.Contains("Resources"))
                db = AssetDatabase.LoadAssetAtPath<ItemDatabase>(assetPath);
        }

        GameObject UI_itemPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/UI/Popup/UI_PetPopup.prefab", typeof(GameObject));
        _itempopup = UI_itemPrefab.GetComponent<UI_PetPopup>();


        foreach (EquipableItem item in db.ItemList.Where(x => x.ItemClass == "Pet").OrderBy(x => (x as EquipableItem).Level))
        {
            UI_PetItemData d = new UI_PetItemData();
            d.ItemId = item.ItemId;
            d.ItemClass = item.ItemClass;
            d.Level = item.Level;
            d.DescriptionBackgroundSprite = item.DescriptionBackground;
            d.EquipBackgroundSprite = item.DescriptionBackground;
            _updateList.Add(d);


        }
       
        _itempopup.SetupItemViewData(_updateList);
        EditorUtility.SetDirty(_itempopup);
    }

    [MenuItem("Tools/룬 팝업 신데이트 적용")]
    static void ApplyRunePopupData()
    {
        ItemDatabase db = null;
        UI_RunePopup _itempopup = null;
        List<UI_RuneItemData> _updateList = new List<UI_RuneItemData>();
        string[] guid = AssetDatabase.FindAssets($"t:{typeof(ItemDatabase)}");
        foreach (string path in guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(path);
            Debug.Log(assetPath);
            if (assetPath.Contains("Resources"))
                db = AssetDatabase.LoadAssetAtPath<ItemDatabase>(assetPath);
        }

        GameObject UI_itemPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/UI/Popup/UI_RunePopup.prefab", typeof(GameObject));
        _itempopup = UI_itemPrefab.GetComponent<UI_RunePopup>();


        foreach (EquipableItem item in db.ItemList.Where(x => x.ItemClass == "Rune").OrderBy(x => (x as EquipableItem).Level))
        {
            UI_RuneItemData d = new UI_RuneItemData();
            d.ItemId = item.ItemId;
            d.ItemClass = item.ItemClass;
            d.Level = item.Level;
            d.DescriptionBackgroundSprite = item.DescriptionBackground;
            d.EquipBackgroundSprite = item.DescriptionBackground;
            _updateList.Add(d);


        }

        _itempopup.SetupItemViewData(_updateList);
        EditorUtility.SetDirty(_itempopup);
    }
    #region Create CSV File
    [MenuItem("Tools/스테이지 테이블 CSV 생성")]
    static void CreateStageCSVfile()
    {
        StageTask _mainStage = (StageTask)AssetDatabase.LoadAssetAtPath($"Assets/Stage/MainStage/MainStage.asset", typeof(StageTask));
        string filename = "Stage.csv";
        StreamWriter df = new StreamWriter($"{Application.dataPath}/Resources/Data/{filename}", false, System.Text.Encoding.UTF8);
        df.WriteLine($"stage , 일반몬스터 , 보스몬스터");
        try
        {
            for (int stage = 1; stage <= 500000; stage++)
            {
                df.WriteLine($"{stage} , {_mainStage.GetMonsterHP(stage)} , {_mainStage.GetMonsterHP(stage)*5}");
            }
            df.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }

        AssetDatabase.Refresh();

    }

    [MenuItem("Tools/스텟 CSV 생성")]
    static void CreateStatCSVfile()
    {
        List<CharacterStat> _stats = new List<CharacterStat>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(CharacterStat)}", new string[] { "Assets/PlayerStat/" });
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var stat = AssetDatabase.LoadAssetAtPath<CharacterStat>(assetPath);

            _stats.Add(stat);
        }

        foreach(CharacterStat stat in _stats)
        {
            string filename;
            if (stat.CodeName == Define.StatID.Attack.ToString())
                filename = "1.골드_공격력.csv";
            else if (stat.CodeName == Define.StatID.AttackSpeed.ToString())
                filename = "2.골드_공속.csv";
            else if (stat.CodeName == Define.StatID.CriticalHit.ToString())
                filename = "3.골드_치명타.csv";
            else if (stat.CodeName == Define.StatID.CriticalHitRate.ToString())
                filename = "4.골드_치명확률.csv";
            else if (stat.CodeName == Define.StatID.CraftAttack.ToString())
                filename = "5.CP_공격력.csv";
            else if (stat.CodeName == Define.StatID.CraftAttackSpeed.ToString())
                filename = "6.CP_공속.csv";
            else if (stat.CodeName == Define.StatID.CraftCriticalHit.ToString())
                filename = "7.CP_치명타.csv";
            else if (stat.CodeName == Define.StatID.CraftCriticalHitRate.ToString())
                filename = "8.CP_치명확률.csv";
            else
                continue;
           
            StreamWriter df = new StreamWriter($"{Application.dataPath}/Resources/Data/{filename}", false, System.Text.Encoding.UTF8);
            df.WriteLine($"스텟레벨 , 스텟 총 증가량 ");
            for (int i=stat.MinLevel;i<= stat.MaxLevel;i++)
            {
                if(stat.CodeName == Define.StatID.CriticalHitRate.ToString() || stat.CodeName == Define.StatID.CraftCriticalHitRate.ToString())
                    df.WriteLine($"{i} , {((double)stat.CalculateValue(i) / 10)}");
                else
                    df.WriteLine($"{i} , {stat.CalculateValue(i)}");
            }
            df.Close();
        }

        AssetDatabase.Refresh();


    }
    #endregion
    [MenuItem("Tools/활로부터 아이템 생성")]
    static void CreateCloakItems()
    {
        
        string[] ItemRating = new string[] { "F", "E", "D", "C", "B", "A", "S", "L", "M", "I" }; 
        foreach(string itemName in ItemRating)
        {
            Bow _blow = (Bow)AssetDatabase.LoadAssetAtPath($"Assets/Item/Item/Bow/활{itemName}.asset", typeof(Bow));
            
            /*
            
            Helmet _helmet = (Helmet)AssetDatabase.LoadAssetAtPath($"Assets/Item/Item/Helmet/헬멧{itemName}.asset", typeof(Helmet));
         
            
            _helmet.DescriptionBackgroundTexture = _blow.DescriptionBackgroundTexture;
            _helmet.DescriptionBackgroundColor = _blow.DescriptionBackgroundColor;
            _helmet.DescriptionBackgroundOutline = _blow.DescriptionBackgroundOutline.Copy();
            _helmet.DisplayNameOutLine = _blow.DisplayNameOutLine.Copy();
            _helmet.IconBackground = _blow.IconBackground;
            _helmet.IconBackgroundOutline = _blow.IconBackgroundOutline.Copy();
           
            AssetDatabase.SaveAssets();
            */
            

        }


        




        AssetDatabase.Refresh();
        
        
    }

    [MenuItem("Tools/시즌패스")]
    static void CreateSeasonpassQuest()
    {
        
        Category _category = (Category)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Category/Seasonpass.asset", typeof(Category));
        TaskAction _taskaction = (TaskAction)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Action/SimpleSet.asset", typeof(TaskAction));
        TaskTarget _tasktarget = (TaskTarget)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Target/StringTarget/Target_MainStage.asset", typeof(TaskTarget));
        Task _seasonpassTask = (Task)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Task/SeasonPass/SeasonPassTask.asset", typeof(Task));
        Task _seasonpassTask2 = (Task)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Task/SeasonPass/SeasonPassTask2.asset", typeof(Task));


        Sprite _questIcon = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Resources/Sprites/Currency/StatusBarIcon_Gem.png", typeof(Sprite));


        int stage = 0;
        while (stage < 300000)
        {
            int _normalReward;
            int _seasonPassReward;
            int _seasonPassReward2;
            int _seasonPassReward3;
            if (stage < 1000)
            {
                stage += 100;
                _normalReward = 1000;
                _seasonPassReward = 3000;
                _seasonPassReward2 = 5000;
                _seasonPassReward3 = 15000;
            }
            else if (stage < 10000)
            {
                stage += 1000;
                _normalReward = 2000;
                _seasonPassReward = 6000;
                _seasonPassReward2 = 10000;
                _seasonPassReward3 = 30000;
            }
            else if (stage < 20000)
            {
                stage += 2000;
                _normalReward = 2000;
                _seasonPassReward = 6000;
                _seasonPassReward2 = 10000;
                _seasonPassReward3 = 30000;
            }
            else if (stage < 35000)
            {
                stage += 3000;
                _normalReward = 3000;
                _seasonPassReward = 10000;
                _seasonPassReward2 = 15000;
                _seasonPassReward3 = 50000;
            }
            else if (stage < 80000)
            {
                stage += 5000;
                _normalReward = 5000;
                _seasonPassReward = 15000;
                _seasonPassReward2 = 25000;
                _seasonPassReward3 = 75000;
            }
            else
            {
                stage += 10000;
                _normalReward = 10000;
                _seasonPassReward = 30000;
                _seasonPassReward2 = 50000;
                _seasonPassReward3 = 150000;
            }


            

            Reward _NormalReward = (Reward)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Reward/Ruby/{_normalReward}.asset", typeof(Reward));
            Reward _SeasonPassReward = (Reward)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Reward/Ruby/{_seasonPassReward}.asset", typeof(Reward));
            Reward _SeasonPassReward2 = (Reward)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Reward/Ruby/{_seasonPassReward2}.asset", typeof(Reward));
            Reward _SeasonPassReward3 = (Reward)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Reward/Ruby/{_seasonPassReward3}.asset", typeof(Reward));

            Task task = CreateInstance<Task>();
            task.Init(_category, new TaskTarget[] { _tasktarget }, _taskaction, stage);

            TaskGroup _NormalTaskGroup = new TaskGroup();
            _NormalTaskGroup.Init(new Task [] { task });
            TaskGroup _SeasonPassTaskGroup = new TaskGroup();
            _SeasonPassTaskGroup.Init(new Task[] { task, _seasonpassTask });

           
            TaskGroup _SeasonPassTaskGroup2 = new TaskGroup();
            _SeasonPassTaskGroup2.Init(new Task[] { task, _seasonpassTask , _seasonpassTask2 });

            Quest _Normalquest = CreateInstance<Quest>();
            _Normalquest.Init($"free_{stage}",_category, new TaskGroup[] { _NormalTaskGroup }, _questIcon , _NormalReward.Description, new Reward [] { _NormalReward });
            
            Quest _SeasonPassquest = CreateInstance<Quest>();
            _SeasonPassquest.Init($"pass_{stage}", _category,new TaskGroup[] { _SeasonPassTaskGroup }, _questIcon, _SeasonPassReward.Description, new Reward[] { _SeasonPassReward });

            Quest _SeasonPassquest2 = CreateInstance<Quest>();
            _SeasonPassquest2.Init($"pass2_{stage}", _category, new TaskGroup[] { _SeasonPassTaskGroup2 }, _questIcon, _SeasonPassReward2.Description, new Reward[] { _SeasonPassReward2 });

            Quest _SeasonPassquest3 = CreateInstance<Quest>();
            _SeasonPassquest3.Init($"pass3_{stage}", _category, new TaskGroup[] { _SeasonPassTaskGroup2 }, _questIcon, _SeasonPassReward3.Description, new Reward[] { _SeasonPassReward3 });

            AssetDatabase.CreateAsset(task, $"Assets/Quests/Task/SeasonPass/{stage}.asset");
            AssetDatabase.CreateAsset(_Normalquest, $"Assets/Quests/Quest/SeasonPass/{stage}_free.asset");
            AssetDatabase.CreateAsset(_SeasonPassquest, $"Assets/Quests/Quest/SeasonPass/{stage}_pass.asset");
            AssetDatabase.CreateAsset(_SeasonPassquest2, $"Assets/Quests/Quest/SeasonPass/{stage}_pass2.asset");
            AssetDatabase.CreateAsset(_SeasonPassquest3, $"Assets/Quests/Quest/SeasonPass/{stage}_pass3.asset");

            EditorUtility.SetDirty(task);
            EditorUtility.SetDirty(_Normalquest);
            EditorUtility.SetDirty(_SeasonPassquest);
            EditorUtility.SetDirty(_SeasonPassquest2);
            EditorUtility.SetDirty(_SeasonPassquest3);


        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem("Tools/일일출석")]
    static void CreateDailyCheckoutQuest()
    {

        Category _category = (Category)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Category/DailyCheckout.asset", typeof(Category));
        TaskAction _taskaction = (TaskAction)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Action/SimpleSet.asset", typeof(TaskAction));
        TaskTarget _tasktarget = (TaskTarget)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Target/StringTarget/Target_Checkout.asset", typeof(TaskTarget));
        Sprite _questIcon = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Resources/Sprites/Currency/StatusBarIcon_Gem.png", typeof(Sprite));

        for(int i=0;i<16;i++)
        {
            Task task = CreateInstance<Task>();
            task.Init(_category, new TaskTarget[] { _tasktarget }, _taskaction, i+1);

            TaskGroup _TaskGroup = new TaskGroup();
            _TaskGroup.Init(new Task[] { task });

            
            int rewardAmount;
            if (i < 7)
                rewardAmount = 10000;
            else if (i < 14)
                rewardAmount = 20000;
            else if( i < 15)
                rewardAmount = 30000;
            else
                rewardAmount = 50000;
            Reward _Reward = (Reward)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Reward/Ruby/{rewardAmount}.asset", typeof(Reward));

            Quest _quest = CreateInstance<Quest>();
            _quest.Init($"Daily_Checkout_{i}", _category, new TaskGroup[] { _TaskGroup }, _questIcon, $"{i+1}일차 보상", new Reward[] { _Reward });

            AssetDatabase.CreateAsset(task, $"Assets/Quests/Task/DailyCheckout/{i}.asset");
            AssetDatabase.CreateAsset(_quest, $"Assets/Quests/Quest/DailyCheckout/DailyCheckout_{i}.asset");

        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/PreQuestCondition")]
    static void CreatePreQuestCondition()
    {
        List<Quest> _quests = new List<Quest>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Quest)}", new string[] { "Assets/Quests/Quest/TutorialQuest/" });
        int _loadIndex = 0;
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Quest quest = AssetDatabase.LoadAssetAtPath<Quest>(assetPath);

            _quests.Add(quest);

            if(_loadIndex > 0)
            {
                PreQuestCondition _condition = CreateInstance<PreQuestCondition>();
                _condition.Init(_quests[_loadIndex - 1]);
                quest.SetAcceptableConditions(new Condition[] { _condition });
                AssetDatabase.CreateAsset(_condition, $"Assets/Quests/Conditions/Tutorial_Condition_{_loadIndex - 1}.asset");
                EditorUtility.SetDirty(_condition);
            }

            _loadIndex++;
            EditorUtility.SetDirty(quest);
        }

        guids = AssetDatabase.FindAssets($"t:{typeof(Quest)}", new string[] { "Assets/Quests/Quest/DailyQuest/" });
        _loadIndex = 0;
        _quests.Clear();
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Quest quest = AssetDatabase.LoadAssetAtPath<Quest>(assetPath);

            _quests.Add(quest);

            if (_loadIndex > 0)
            {
                PreQuestCondition _condition = CreateInstance<PreQuestCondition>();
                _condition.Init(_quests[_loadIndex - 1]);
                quest.SetAcceptableConditions(new Condition[] { _condition });
                AssetDatabase.CreateAsset(_condition, $"Assets/Quests/Conditions/Daily_Condition_{_loadIndex - 1}.asset");
                EditorUtility.SetDirty(_condition);
            }

            _loadIndex++;
            EditorUtility.SetDirty(quest);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


    }

    [MenuItem("Tools/QuestHelperSetup")]
    static void CreateQuestHelper()
    {
        List<Quest> _quests = new List<Quest>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Quest)}", new string[] { "Assets/Quests/Quest/TutorialQuest/" });

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Quest quest = AssetDatabase.LoadAssetAtPath<Quest>(assetPath);

            if(quest.CodeName.Contains("Upgrade") && quest.CodeName.Contains("Stat"))
            {
                HelpShowPopup _condition = CreateInstance<HelpShowPopup>();
                _condition.SetPopup(UIManager.Category.UI_Stat);
                quest.SetQuestHelper(_condition);
                AssetDatabase.CreateAsset(_condition, $"Assets/Quests/QuestHelper/Show_{UIManager.Category.UI_Stat}.asset");
                EditorUtility.SetDirty(quest);
            }
            else if(quest.CodeName.Contains("Purchase"))
            {
                HelpShowPopup _condition = CreateInstance<HelpShowPopup>();
                _condition.SetPopup(UIManager.Category.UI_ShopPopup);
                quest.SetQuestHelper(_condition);
                AssetDatabase.CreateAsset(_condition, $"Assets/Quests/QuestHelper/Show_{UIManager.Category.UI_ShopPopup}.asset");
                EditorUtility.SetDirty(quest);
            }
            else if(quest.CodeName.Contains("Equip"))
            {
                if(quest.CodeName.Contains("Rune"))
                {
                    HelpShowPopup _condition = CreateInstance<HelpShowPopup>();
                    _condition.SetPopup(UIManager.Category.UI_RunePopup);
                    quest.SetQuestHelper(_condition);
                    AssetDatabase.CreateAsset(_condition, $"Assets/Quests/QuestHelper/Show_{UIManager.Category.UI_RunePopup}.asset");
                }
                else if(quest.CodeName.Contains("Pet"))
                {
                    HelpShowPopup _condition = CreateInstance<HelpShowPopup>();
                    _condition.SetPopup(UIManager.Category.UI_PetPopup);
                    quest.SetQuestHelper(_condition);
                    AssetDatabase.CreateAsset(_condition, $"Assets/Quests/QuestHelper/Show_{UIManager.Category.UI_PetPopup}.asset");
                }
                else if(quest.CodeName.Contains("Bow") || quest.CodeName.Contains("Armor") || quest.CodeName.Contains("Helmet") || quest.CodeName.Contains("Cloak"))
                {
                    HelpShowPopup _condition = CreateInstance<HelpShowPopup>();
                    _condition.SetPopup(UIManager.Category.UI_Item);
                    quest.SetQuestHelper(_condition);
                    AssetDatabase.CreateAsset(_condition, $"Assets/Quests/QuestHelper/Show_{UIManager.Category.UI_Item}.asset");
                }
                EditorUtility.SetDirty(quest);
            }


          

            
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();




    }

    [MenuItem("Tools/CreateGambleQuest")]
    static void CreateGambleQuest()
    {
        string[] ItemClasses = new string[] { "Rune", "Pet", "Bow", "Armor", "Helmet", "Cloak" };
        int[] needToSucessComplete = { 50, 100, 250, 500, 1000, 2500, 5000, 10000 };
        string[,] Rewards = new string [,] { { "Ruby/10000", "Ruby/25000", "Ruby/50000", "Ruby/100000", "Ruby/200000" , "Rune/L_Rune" , "Rune/M_Rune" , "Rune/I_Rune" } ,
                                             { "Ruby/10000", "Ruby/25000", "Ruby/50000", "Ruby/100000", "Ruby/200000" , "Pet/L_Pet" , "Pet/M_Pet" , "Pet/I_Pet" },
                                             { "Ruby/10000", "Ruby/25000", "Ruby/50000", "Bow/A_Bow", "Bow/S_Bow" , "Bow/L_Bow" , "Bow/M_Bow" , "Bow/I_Bow" },
                                             { "Ruby/10000", "Ruby/25000", "Ruby/50000", "Armor/A_Armor", "Armor/S_Armor" , "Armor/L_Armor" , "Armor/M_Armor" , "Armor/I_Armor" },
                                            { "Ruby/10000", "Ruby/25000", "Ruby/50000", "Helmet/A_Helmet", "Helmet/S_Helmet" , "Helmet/L_Helmet" , "Helmet/M_Helmet" , "Helmet/I_Helmet" },
                                            { "Ruby/10000", "Ruby/25000", "Ruby/50000", "Cloak/A_Cloak", "Cloak/S_Cloak" , "Cloak/L_Cloak" , "Cloak/M_Cloak" , "Cloak/I_Cloak" }};
        int itemclassIndex = 0;
        foreach(string item in ItemClasses)
        {
            for (int i = 0; i < needToSucessComplete.Length; i++)
            {
                Category _category = (Category)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Category/Gamble{item}.asset", typeof(Category));
                TaskAction _taskaction = (TaskAction)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Action/Positive Count.asset", typeof(TaskAction));
                TaskTarget _tasktarget = (TaskTarget)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Target/StringTarget/Target_{item}.asset", typeof(TaskTarget));
                Sprite _questIcon = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Resources/Sprites/GamblePopup/icon_star_result_n.png", typeof(Sprite));

                Reward _Reward = (Reward)AssetDatabase.LoadAssetAtPath($"Assets/Quests/Reward/{Rewards[itemclassIndex, i]}.asset", typeof(Reward));

                Task task = CreateInstance<Task>();
                task.Init(_category, new TaskTarget[] { _tasktarget }, _taskaction, needToSucessComplete[i]);

                TaskGroup _TaskGroup = new TaskGroup();
                _TaskGroup.Init(new Task[] { task });

                Quest _quest = CreateInstance<Quest>();
                _quest.Init($"Gamble{item}_{i}", _category, new TaskGroup[] { _TaskGroup }, _questIcon, $"{needToSucessComplete[i]} 달성", new Reward[] { _Reward }, "LV "+(i+1).ToString());

                AssetDatabase.CreateAsset(task, $"Assets/Quests/Task/Gamble/Gameble{item}_{i}.asset");
                AssetDatabase.CreateAsset(_quest, $"Assets/Quests/Quest/Gamble/Gameble{item}_{i}.asset");

                EditorUtility.SetDirty(task);
                EditorUtility.SetDirty(_quest);
            }
            itemclassIndex++;


        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

    [MenuItem("Tools/SetGambleQuestAcceptable")]
    static void SetGambleQuestAcceptable()
    {
        string[] ItemClasses = new string[] { "Rune", "Pet", "Bow", "Armor", "Helmet", "Cloak" };
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Quest)}", new string[] { "Assets/Quests/Quest/Gamble/" });
        List<Quest> _gambleQuestList = new List<Quest>();
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Quest quest = AssetDatabase.LoadAssetAtPath<Quest>(assetPath);

            _gambleQuestList.Add(quest);


            int _index = Util.GetIntFromString(quest.CodeName);
            string findItemClass = ItemClasses.Where(x => quest.CodeName.Contains(x)).FirstOrDefault();

            if (_index == 0)
                continue;

            Quest findPreQuest = _gambleQuestList.Where(x => x.CodeName.Contains(findItemClass) && x.CodeName.Contains((_index - 1).ToString())).FirstOrDefault();

            PreQuestCompletableCondition _condition = CreateInstance<PreQuestCompletableCondition>();
            _condition.Init(findPreQuest);
            quest.SetAcceptableConditions(new Condition[] { _condition });

            AssetDatabase.CreateAsset(_condition, $"Assets/Quests/Conditions/Gamble/Gamble{findItemClass}_Condition_{_index - 1}.asset");
            EditorUtility.SetDirty(_condition);
            EditorUtility.SetDirty(quest);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        //_gambleQuestList.Where(x=>x.CodeName.Contains(ItemClasses.All().ToString()))
    }


    [System.Serializable]
    class PlayfabTableNodeData
    {

        public string ResultItemType;
        public string ResultItem;
        public int Weight;

    }

    [System.Serializable]
    class PlayfabTableData
    {
        public string TableId;
        public List<PlayfabTableNodeData> Nodes;

    }
    [MenuItem("Tools/CreateTableJson")]
    static void CreateTableJson()
    {
        const string tablesavePath = "/TableList.json";
        string _savePath = Application.persistentDataPath + tablesavePath;

        var saveDatas = new JArray();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Table)}", new string[] { "Assets/Shop/Table/" });
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string TableName = assetPath.Split('/').Last();

            Table _table = AssetDatabase.LoadAssetAtPath<Table>(assetPath);

            PlayfabTableData tableData = new PlayfabTableData { TableId = _table.ItemId };
            tableData.Nodes = new List<PlayfabTableNodeData>();

            foreach(TableItem _tableItem in _table.Items)
            {
                PlayfabTableNodeData _tableItemData;
                if (_tableItem.GetItem is Table)
                    _tableItemData = new PlayfabTableNodeData { Weight = _tableItem.Count, ResultItemType = "TableId", ResultItem = _tableItem.GetItem.ItemId };
                else
                    _tableItemData = new PlayfabTableNodeData { Weight = _tableItem.Count, ResultItemType = "ItemId", ResultItem = _tableItem.GetItem.ItemId };
                tableData.Nodes.Add(_tableItemData);
            }
            saveDatas.Add(JObject.FromObject(tableData));

        }

        File.WriteAllText(_savePath, saveDatas.ToString(), System.Text.Encoding.UTF8);
    }

    [MenuItem("Tools/AssetDataBaseRefresh")]
    static void AssetDataBaseRefres()
    {
        AssetDatabase.Refresh();
    }

    
    [MenuItem("Tools/Test")]
    static void Test()
    {
       
    }




}
