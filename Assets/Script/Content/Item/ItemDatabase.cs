//using HeroEditor.Common;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;

[CreateAssetMenu(fileName = "ItemDatabase",menuName ="¾ÆÀÌÅÛ/µ¥ÀÌÅÍº£ÀÌ½º")]
public class ItemDatabase : ScriptableObject
{
    [Header("Á¤¼ö")]
    [SerializeField]
    List<Artifact> _essenceList;

    [Header("½ÉÀå")]
    [SerializeField]
    List<Artifact> _heartList;

    [Header("È°")]
    [SerializeField]
    List<Bow> _bowList;

    [Header("Åõ±¸")]
    [SerializeField]
    List<Helmet> _helmetList;

    [Header("°©¿Ê")]
    [SerializeField]
    List<Armor> _armorList;

    [Header("¸ÁÅä")]
    [SerializeField]
    List<Cloak> _cloakList;

    [Header("·é")]
    [SerializeField]
    List<Rune> _runeList;


    [Header("ÃÑ ¾ÆÀÌÅÛ ¸®½ºÆ®")]
    [SerializeField]
    List<BaseItem> _itemList;


    [Header("·é ¾÷±×·¹ÀÌµå Å×ÀÌºí")]
    [SerializeField]
    UpgradeSystem _runeUpgradeSystem;

    [Header("Æê ¾÷±×·¹ÀÌµå Å×ÀÌºí")]
    [SerializeField]
    UpgradeSystem _petUpgradeSystem;

    [Header("È° ¾÷±×·¹ÀÌµå Å×ÀÌºí")]
    [SerializeField]
    UpgradeSystem _bowUpgradeSystem;

    [Header("°©¿Ê ¾÷±×·¹ÀÌµå Å×ÀÌºí")]
    [SerializeField]
    UpgradeSystem _armorUpgradeSystem;

    [Header("Çï¸ä ¾÷±×·¹ÀÌµå Å×ÀÌºí")]
    [SerializeField]
    UpgradeSystem _helmetUpgradeSystem;

    [Header("¸ÁÅä ¾÷±×·¹ÀÌµå Å×ÀÌºí")]
    [SerializeField]
    UpgradeSystem _cloakUpgradeSystem;

    [Header("À¯¹° ¾÷±×·¹ÀÌµå Å×ÀÌºí")]
    [SerializeField]
    UpgradeSystem _artifactUpgradeSystem;

    [Header("·é ÀåÂø")]
    [SerializeField]
    EquipmentSystem _runeEquipment;

    [Header("Æê ÀåÂø")]
    [SerializeField]
    EquipmentSystem _petEquipment;

    [Header("È° ÀåÂø")]
    [SerializeField]
    EquipmentSystem _bowEquipment;

    [Header("Åõ±¸ ÀåÂø")]
    [SerializeField]
    EquipmentSystem _helmetEquipment;

    [Header("°©¿Ê ÀåÂø")]
    [SerializeField]
    EquipmentSystem _armorEquipment;

    [Header("¸ÁÅä ÀåÂø")]
    [SerializeField]
    EquipmentSystem _cloakEquipment;

    [Header("Æê")]
    [SerializeField]
    List<Pet> _petList;

    [Header("¿ìÆí")]
    [SerializeField]
    List<Mail> _mailList;

    [Header("¹øµé")]
    [SerializeField]
    List<Bundle> _bundleList;

    [Header("ÄíÆù")]
    [SerializeField]
    List<CouponCatalog> _couponList;



    public IReadOnlyList<Artifact> EssenceList => _essenceList;
    public IReadOnlyList<Artifact> HeartList => _heartList;

    
    public IReadOnlyList<Bow> BowList => _bowList;
    public IReadOnlyList<Helmet> HelmetList => _helmetList;
    public IReadOnlyList<Armor> ArmorList => _armorList;
    public IReadOnlyList<Cloak> CloakList => _cloakList;

    

    public IReadOnlyList<Rune> RuneList => _runeList;
    public IReadOnlyList<Pet> PetList => _petList;

    public UpgradeSystem RuneUpgradeSystem => _runeUpgradeSystem; 
    public UpgradeSystem PetUpgradeSystem=>_petUpgradeSystem;

    public UpgradeSystem BowUpgradeSystem => _bowUpgradeSystem;

    public UpgradeSystem ArmorUpgradeSystem => _armorUpgradeSystem;

    public UpgradeSystem HelmetUpgradeSystem => _helmetUpgradeSystem;

    public UpgradeSystem CloakUpgradeSystem => _cloakUpgradeSystem;


    public UpgradeSystem ArtifactUpgradeSystem => _artifactUpgradeSystem;

    public EquipmentSystem RuneEquipment => _runeEquipment;
    public EquipmentSystem PetEquipment => _petEquipment;

    public EquipmentSystem BowEquipment => _bowEquipment;
    public EquipmentSystem HelmetEquipment => _helmetEquipment;

    public EquipmentSystem ArmorEquipment => _armorEquipment;
    public EquipmentSystem CloakEquipment => _cloakEquipment;

    public IReadOnlyList<Mail> MailList => _mailList;

    public IReadOnlyList<Bundle> BundleList => _bundleList;
    public IReadOnlyList<BaseItem> ItemList => _itemList;

    public IReadOnlyDictionary<string, CouponCatalog> CouponList => _couponList.ToDictionary(keySelector: m => m.CouponCode, elementSelector: m => m);

    
    public string FindArtifactByPiece(string ItemId)
    {
        BaseItem _findPiece = _itemList.Where(x => x.ItemId == ItemId).FirstOrDefault();
        if(_findPiece == null )
        {
            Debug.LogWarning($"Find Item {ItemId} not exist");
            return string.Empty;
        }
        if(_findPiece is Artifactpiece == false)
        {
            Debug.LogWarning($"Find ItemClass {_findPiece.ItemClass} is not ArtifactPiece");
            return string.Empty;
        }

        BaseItem _findArtifact = ItemList.Where(x => x is Artifact).ToList().
                 Where(x => (x as Artifact).Type == (_findPiece as Artifactpiece).Type).FirstOrDefault();
        

        return _findArtifact.ItemId;
    }


#if UNITY_EDITOR
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

    const string ItemsavePath= "/ItemList.json";
    [ContextMenu("CreateItemJson")]
    private void CreateJsonFile()
    {
        string _savePath = Application.persistentDataPath + ItemsavePath;
        var root = new JObject();
        var saveDatas = new JArray();
        
        foreach (var item in _itemList)
        {
            CatalogItem _addedItem;
            if (item is Bundle)
            {
                Bundle _bundleItem = item as Bundle;

                if (_bundleItem.Content != null || _bundleItem.Currencies != null)
                {
                    CatalogItemBundleInfo _bundleContentInfo = new CatalogItemBundleInfo();

                    if (_bundleItem.Content != null)
                    {
                        foreach (BundleItemContent content in _bundleItem.Content)
                        {
                            for (int i = 0; i < content.Amount; i++)
                            {
                                if (content.Item is Table)
                                {
                                    if (_bundleContentInfo.BundledResultTables == null)
                                        _bundleContentInfo.BundledResultTables = new List<string>();

                                    _bundleContentInfo.BundledResultTables.Add(content.Item.ItemId);
                                }
                                else
                                {
                                    if (_bundleContentInfo.BundledItems == null)
                                        _bundleContentInfo.BundledItems = new List<string>();
                                    _bundleContentInfo.BundledItems.Add(content.Item.ItemId);
                                }
                            }
                        }


                    }

                    if(_bundleItem.Currencies != null)
                    {
                        foreach (BundleCurrencyContent content in _bundleItem.Currencies)
                        {
                            if (_bundleContentInfo.BundledVirtualCurrencies == null)
                                _bundleContentInfo.BundledVirtualCurrencies = new Dictionary<string, uint>();
                            _bundleContentInfo.BundledVirtualCurrencies.Add(content.Currency.ShortCodeName, (uint)content.Amount);
                        }
                    }



                    if (_bundleItem.ItemClass != "Randombox")
                        _addedItem = new CatalogItem { ItemId = item.ItemId, ItemClass = item.ItemClass, CatalogVersion = "Main", Description = item.Description, DisplayName = item.DisplayName, Bundle = _bundleContentInfo, IsStackable = true, Consumable = new CatalogItemConsumableInfo { UsageCount = 1 } };
                    else
                    {
                        if(item.UnitPrice == 0)
                            _addedItem = new CatalogItem { ItemId = item.ItemId, ItemClass = item.ItemClass, CatalogVersion = "Main", Description = item.Description, DisplayName = item.DisplayName, Bundle = _bundleContentInfo, IsStackable = true, Consumable = new CatalogItemConsumableInfo { UsageCount = 1 }, VirtualCurrencyPrices = new Dictionary<string, uint>() { { "RB", item.UnitPrice } } };
                        else
                            _addedItem = new CatalogItem { ItemId = item.ItemId, ItemClass = item.ItemClass, CatalogVersion = "Main", Description = item.Description, DisplayName = item.DisplayName, Bundle = _bundleContentInfo, IsStackable = true, Consumable = new CatalogItemConsumableInfo { UsageCount = 1 }, VirtualCurrencyPrices = new Dictionary<string, uint>() { { "RB", item.UnitPrice } } };

                    }
                }
                else
                    _addedItem = new CatalogItem { ItemId = item.ItemId, ItemClass = item.ItemClass, CatalogVersion = "Main", Description = item.Description, DisplayName = item.DisplayName , IsStackable = true, Consumable = new CatalogItemConsumableInfo { UsageCount = 1 } };
            }
            else if(item is Mail)
            {
                Mail _mailItem = item as Mail;
                CatalogItemContainerInfo _consuminfo = new CatalogItemContainerInfo();
                _consuminfo.ResultTableContents = new List<string>();
                if ( _mailItem.Currencies.Item != null)
                {
                    Dictionary<string, uint> _cur = new Dictionary<string, uint>();
                    if(_mailItem.Currencies.Item.CodeName == Define.CurrencyID.Ruby.ToString())
                        _cur.Add(_mailItem.Currencies.Item.ShortCodeName, (uint)_mailItem.Currencies.Count);

                    _consuminfo.VirtualCurrencyContents = _cur;
                }

                if(_mailItem.Items.Item != null)
                {
                   
                    if(_mailItem.Items.Item is Table)
                    {
                        for (int i = 0; i < _mailItem.Items.Count; i++)
                            _consuminfo.ResultTableContents.Add(_mailItem.Items.Item.ItemId);
                    }
                    else
                    {
                        for (int i = 0; i < _mailItem.Items.Count; i++)
                            _consuminfo.ResultTableContents.Add(_mailItem.Items.Item.ItemId);
                    }

                }
                
                _addedItem = new CatalogItem { ItemId = _mailItem.ItemId, ItemClass = _mailItem.ItemClass, CatalogVersion = "Main", Description = _mailItem.Description, DisplayName = _mailItem.DisplayName , Consumable = new CatalogItemConsumableInfo { UsageCount = 1 } , Container = _consuminfo };
            }
            else
                _addedItem = new CatalogItem { ItemId = item.ItemId, ItemClass = item.ItemClass , CatalogVersion = "Main", Description = item.Description, DisplayName = item.DisplayName, IsStackable = true , Consumable = new CatalogItemConsumableInfo { UsageCount = 1 } };
           
            //Conainer Catalog
            //CatalogItem _addedItemd = new CatalogItem { Consumable = new CatalogItemConsumableInfo { UsageCount = 1 }, Container = new CatalogItemContainerInfo { ItemContents = { "ItemId0", "ItemId1" } } };
          

            saveDatas.Add(JObject.FromObject(_addedItem));
        }
       
        root.Add("CatalogVersion", "Main");
        root.Add("Catalog", saveDatas);
        var DropTables = new JArray();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Table)}", new string[] { "Assets/Shop/Table/" });
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string TableName = assetPath.Split('/').Last();

            Table _table = AssetDatabase.LoadAssetAtPath<Table>(assetPath);

            PlayfabTableData tableData = new PlayfabTableData { TableId = _table.ItemId };
            tableData.Nodes = new List<PlayfabTableNodeData>();

            foreach (TableItem _tableItem in _table.Items)
            {
                PlayfabTableNodeData _tableItemData;
                if (_tableItem.GetItem is Table)
                    _tableItemData = new PlayfabTableNodeData { Weight = _tableItem.Count, ResultItemType = "TableId", ResultItem = _tableItem.GetItem.ItemId };
                else
                    _tableItemData = new PlayfabTableNodeData { Weight = _tableItem.Count, ResultItemType = "ItemId", ResultItem = _tableItem.GetItem.ItemId };
                tableData.Nodes.Add(_tableItemData);
            }
            DropTables.Add(JObject.FromObject(tableData));

        }

        root.Add("DropTables", DropTables);
        File.WriteAllText(_savePath, root.ToString(), System.Text.Encoding.UTF8);

        //CatalogItem item = new CatalogItem { ItemId = "d"};

    }

   


    [ContextMenu("FindEssence")]
    private void FindEssences()
    {
        FindEssence();
    }
    void FindEssence()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Artifact)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Artifact>(assetPath);

            if (item.GetType() == typeof(Artifact))
            {
                if(item.ItemClass == "Essence")
                _essenceList.Add(item);
                
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
    [ContextMenu("FindHeart")]
    private void FindHearts()
    {
        FindHeart();
    }
    void FindHeart()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Artifact)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Artifact>(assetPath);

            if (item.GetType() == typeof(Artifact))
            {
                if (item.ItemClass == "Heart")
                    _heartList.Add(item);

            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    [ContextMenu("FindRune")]
    private void FindRuness()
    {
        FindRune();
    }
    void FindRune()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Rune)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Rune>(assetPath);

            if (item.GetType() == typeof(Rune))
            {
                _runeList.Add(item);
                _runeList = _runeList.OrderBy(x => ((Rune)x).Level).ThenByDescending(x=>(int)x.type).ToList();
            }
                

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
    

    [ContextMenu("FindBow")]
    private void FindBows()
    {
        FindBow();
    }
    void FindBow()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Bow)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Bow>(assetPath);

            if (item.GetType() == typeof(Bow))
            {
                _bowList.Add(item);
                _bowList = _bowList.OrderBy(x => ((Bow)x).Level).ToList();
            }
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
    [ContextMenu("FindArmor")]
    private void FindArmors()
    {
        FindArmor();
    }
    void FindArmor()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Armor)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Armor>(assetPath);

            if (item.GetType() == typeof(Armor))
            {
                _armorList.Add(item);
                _armorList = _armorList.OrderBy(x => ((Armor)x).Level).ToList();
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    [ContextMenu("FindHelmet")]
    private void FindHelmets()
    {
        FindHelmet();
    }
    void FindHelmet()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Helmet)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Helmet>(assetPath);

            if (item.GetType() == typeof(Helmet))
            {
                _helmetList.Add(item);
                _helmetList = _helmetList.OrderBy(x => ((Helmet)x).Level).ToList();
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
    [ContextMenu("FindCloak")]
    private void FindCloaks()
    {
        FindCloak();
    }
    void FindCloak()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Cloak)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Cloak>(assetPath);

            if (item.GetType() == typeof(Cloak))
            {
                _cloakList.Add(item);
                _cloakList = _cloakList.OrderBy(x => ((Cloak)x).Level).ToList();
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
    [ContextMenu("FindPet")]
    private void FindPets()
    {
        FindPet();
    }
    void FindPet()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Pet)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Pet>(assetPath);

            if (item.GetType() == typeof(Pet))
            {
                _petList.Add(item);
                _petList = _petList.OrderBy(x => ((Pet)x).Level).ToList();
            }
              

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
    
    [ContextMenu("FindMail")]
    private void FindMails()
    {
        FindMail();
    }

    void FindMail()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Mail)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Mail>(assetPath);

            if (item.GetType() == typeof(Mail))
                _mailList.Add(item);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    [ContextMenu("Find Bundle")]
    private void FindBundles()
    {
        FindBundle();
    }

    private void FindBundle()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(Bundle)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<Bundle>(assetPath);

            if (item.GetType() == typeof(Bundle))
                _bundleList.Add(item);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    [ContextMenu("FindTotalItems")]
    private void FindTotalItems()
    {
        FindItems<Bundle>();
        FindItems<Buff>();
        FindItems<Mail>();
        FindItems<Rune>();
        FindItems<Pet>();
       
        FindItems<Artifact>();
        FindItems<Artifactpiece>();
        FindItems<Bow>();
        FindItems<Helmet>();
        FindItems<Cloak>();
        FindItems<Armor>();
        FindItems<EquipmentSlot>();
    }

    void FindItems<T>() where T : BaseItem 
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (item.GetType() == typeof(T))
                _itemList.Add(item);

            EditorUtility.SetDirty(this);
            
        }
        AssetDatabase.SaveAssets();
    }

#endif

}
