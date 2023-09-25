using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_BaseItem : UI_Base
{
    BaseItem _item;
    public BaseItem Item 
    {
        set
        {
            _item = value;
            Setup(_item);
        }
    }

    Mode _mode;
    public enum Mode
    {
        Equip,
        Leaderboard,
        Shop,
        ShopPopup,
        RandomBox
    }
    enum Images
    {
        IconBackground,
        Icon,
    }
   

    enum TMP_Texts
    {
        DisplayName,
        LevelText
    }

    enum GameObjects
    {
        FX_Circle
    }
    [SerializeField]
    Sprite Rune_G_Bottom;

  

    [SerializeField]
    TextMeshProUGUI DisplayName;

    [SerializeField]
    TextMeshProUGUI LevelText;

    [SerializeField]
    GameObject BottomSprite;

    [SerializeField]
    GameObject IconBackground;

    [SerializeField]
    GameObject Icon;

    [SerializeField]
    GameObject Backround;

    [SerializeField]
    GameObject DisplayNameBackground;
    public void Setup(BaseItem item , Mode mode = Mode.RandomBox)
    {
        _item = item;
        _mode = mode;
        UpdateObject(_item);
    }
    private void UpdateObject(BaseItem item)
    {
        if (item.ItemClass == "Rune")
        {
            Rune rune = item as Rune;

            Backround.GetComponent<Image>().sprite = rune.Background;
            IconBackground.GetComponent<Image>().sprite = rune.IconBackground;
            Icon.GetComponent<Image>().sprite = rune.Icon;

            DisplayName.text = rune.DisplayName;
            BottomSprite.SetActive(false);
            EquipableItem.Rank _itemRank = (EquipableItem.Rank)rune.Level;
            LevelText.text = $"{_itemRank.ToString()}등급";

            if (rune.Level > (int)Rune.Rank.S && rune.Level < (int)Rune.Rank.U)
            {
                Icon.SetActive(false);

                
               

            }
            else if (rune.Level == (int)Rune.Rank.U)
            {
                //DisplayNameBackground.gameObject.SetActive(false);
                //DisplayName.gameObject.SetActive(false);

                Icon.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = -65, y = -75 };
                IconBackground.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = -45, y = -45 };
                gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 165, y = 165 };
                //IconBackground.GetComponent<RectTransform>().siz
                // IconBackground.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 100, y = 100 };
                // Icon.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 120, y = 120 };
            }
            else if (rune.Level == (int)Rune.Rank.G)
            {
                BottomSprite.GetComponent<Image>().sprite = Rune_G_Bottom;
                BottomSprite.SetActive(true);
                //DisplayName.gameObject.SetActive(false);

                Icon.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = -65, y = -75 };
                IconBackground.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = -45, y = -45 };
                gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 165, y = 165 };
                //  IconBackground.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 100, y = 100 };
                //Icon.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 120, y = 120 };
            }


            /*
            GetImage((int)Images.IconBackground).sprite = rune.IconBackground;
            GetImage((int)Images.Icon).sprite = rune.Icon;
            GetComponent<Image>().sprite = rune.Background;

            Get<TextMeshProUGUI>((int)TMP_Texts.DisplayName).text = rune.DisplayName;
            EquipableItem.Rank _itemRank = (EquipableItem.Rank)rune.Level;
            Get<TextMeshProUGUI>((int)TMP_Texts.LevelText).text = $"{_itemRank.ToString()}등급";
            // GetText((int)Texts.LevelText).text = $"{_itemRank.ToString()}등급";
            if (rune.Level > (int)Rune.Rank.S)
            {
                GetImage((int)Images.Icon).gameObject.SetActive(false);
                Get<TextMeshProUGUI>((int)TMP_Texts.LevelText).gameObject.SetActive(false);
                if (Get<GameObject>((int)GameObjects.FX_Circle) != null)
                    Get<GameObject>((int)GameObjects.FX_Circle).SetActive(true);
                //GetText((int)Texts.LevelText).gameObject.SetActive(false);
            }
            */

        }
        else if (item.ItemClass == "Pet")
        {
            Pet pet = item as Pet;


            Backround.GetComponent<Image>().sprite = pet.Background;
            IconBackground.GetComponent<Image>().sprite = pet.IconBackground;
            Icon.GetComponent<Image>().sprite = pet.Icon;

            DisplayName.text = pet.DisplayName;
            EquipableItem.Rank _itemRank = (EquipableItem.Rank)pet.Level;
            LevelText.text = $"{_itemRank.ToString()}등급";
           

            if (pet.Level > (int)Pet.Rank.S && pet.Level < (int)Pet.Rank.U)
            {
                Icon.SetActive(false);
                //LevelText.gameObject.SetActive(false);
            }
            else if (pet.Level == (int)Pet.Rank.U)
            {
                //DisplayNameBackground.gameObject.SetActive(false);
                //DisplayName.gameObject.SetActive(false);

                Icon.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = -65, y = -75 };
                IconBackground.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = -45, y = -45 };
                gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 165, y = 165 };

                //IconBackground.GetComponent<RectTransform>().siz
                // IconBackground.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 100, y = 100 };
                // Icon.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 120, y = 120 };
            }
            else if (pet.Level == (int)Pet.Rank.G)
            {
                BottomSprite.GetComponent<Image>().sprite = Rune_G_Bottom;
                BottomSprite.SetActive(true);
                //DisplayNameBackground.gameObject.SetActive(false);
                //DisplayName.gameObject.SetActive(false);

                Icon.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = -65, y = -75 };
                IconBackground.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = -45, y = -45 };
                gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 165, y = 165 };
                //  IconBackground.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 100, y = 100 };
                //Icon.GetComponent<RectTransform>().sizeDelta = new Vector2 { x = 120, y = 120 };
            }

            /*
            GetImage((int)Images.IconBackground).sprite = pet.IconBackground;
            GetImage((int)Images.Icon).sprite = pet.Icon;
            GetComponent<Image>().sprite = pet.Background;

            Get<TextMeshProUGUI>((int)TMP_Texts.DisplayName).text = pet.DisplayName;
            //GetText((int)Texts.DisplayName).text = pet.DisplayName;
            EquipableItem.Rank _itemRank = (EquipableItem.Rank)pet.Level;
            Get<TextMeshProUGUI>((int)TMP_Texts.LevelText).text = $"{_itemRank.ToString()}등급";

            if (pet.Level > (int)Pet.Rank.S)
            {
                GetImage((int)Images.Icon).gameObject.SetActive(false);
                Get<TextMeshProUGUI>((int)TMP_Texts.LevelText).gameObject.SetActive(false);
                if (Get<GameObject>((int)GameObjects.FX_Circle) != null)
                    Get<GameObject>((int)GameObjects.FX_Circle).SetActive(true);
            }
            */
        }
        else if (item.ItemClass == "Bow")
        {
            Bow bow = item as Bow;

            Backround.GetComponent<Image>().sprite = bow.Background;
           
            IconBackground.SetActive(false);
            Icon.GetComponent<Image>().sprite = bow.Icon;

            DisplayName.text = bow.DisplayName;
            BottomSprite.SetActive(false);
            EquipableItem.Rank _itemRank = (EquipableItem.Rank)bow.Level;
            LevelText.text = $"{_itemRank.ToString()}등급";

            

            if (bow.Level > (int)Bow.Rank.S)
            {
                //LevelText.gameObject.SetActive(false);
            }

            /*
            GetImage((int)Images.IconBackground).gameObject.SetActive(false);
            GetImage((int)Images.Icon).sprite = bow.Icon;
            GetComponent<Image>().sprite = bow.Background;

            //GetText((int)Texts.DisplayName).text = bow.DisplayName;
            Get<TextMeshProUGUI>((int)TMP_Texts.DisplayName).text = bow.DisplayName;
            EquipableItem.Rank _itemRank = (EquipableItem.Rank)bow.Level;
            //GetText((int)Texts.LevelText).text = $"{_itemRank.ToString()}등급";
            Get<TextMeshProUGUI>((int)TMP_Texts.LevelText).text = $"{_itemRank.ToString()}등급";

            if (bow.Level > (int)Bow.Rank.S)
            {
                Get<TextMeshProUGUI>((int)TMP_Texts.LevelText).gameObject.SetActive(false);
                // GetText((int)Texts.LevelText).gameObject.SetActive(false);
            }
            */

        }
        else if (item.ItemClass == "Armor")
        {
            Armor armor = item as Armor;

            Backround.GetComponent<Image>().sprite = armor.Background;

            IconBackground.SetActive(false);
            Icon.GetComponent<Image>().sprite = armor.Icon;

            DisplayName.text = armor.DisplayName;
            BottomSprite.SetActive(false);
            EquipableItem.Rank _itemRank = (EquipableItem.Rank)armor.Level;
            LevelText.text = $"{_itemRank.ToString()}등급";

           

            if (armor.Level > (int)Armor.Rank.S)
            {
                //LevelText.gameObject.SetActive(false);
            }

         
            //GetText((int)Texts.LevelText).gameObject.SetActive(false);
        }
        else if (item.ItemClass == "Helmet")
        {
            Helmet helmet = item as Helmet;

            Backround.GetComponent<Image>().sprite = helmet.Background;

            IconBackground.SetActive(false);
            Icon.GetComponent<Image>().sprite = helmet.Icon;

            DisplayName.text = helmet.DisplayName;
            BottomSprite.SetActive(false);
            EquipableItem.Rank _itemRank = (EquipableItem.Rank)helmet.Level;
            LevelText.text = $"{_itemRank.ToString()}등급";

          
            if (helmet.Level > (int)Armor.Rank.S)
            {
                //LevelText.gameObject.SetActive(false);
            }
        }
        else if (item.ItemClass == "Cloak")
        {
            Cloak cloak = item as Cloak;

            Backround.GetComponent<Image>().sprite = cloak.Background;

            IconBackground.SetActive(false);
            Icon.GetComponent<Image>().sprite = cloak.Icon;

            DisplayName.text = cloak.DisplayName;
            BottomSprite.SetActive(false);
            EquipableItem.Rank _itemRank = (EquipableItem.Rank)cloak.Level;
            LevelText.text = $"{_itemRank.ToString()}등급";

          

            if (cloak.Level > (int)Cloak.Rank.S)
            {
                //LevelText.gameObject.SetActive(false);
            }
            //GetText((int)Texts.LevelText).gameObject.SetActive(false);
        }
        else if (item.ItemClass == "Essence")
        {

            Artifactpiece essence = item as Artifactpiece;

            IconBackground.SetActive(false);
            Icon.GetComponent<Image>().sprite = essence.Icon;

            Backround.GetComponent<Image>().sprite = essence.Background;
            Backround.GetComponent<Image>().color  = essence.BackgroundColor;

            DisplayName.text = essence.DisplayName;
            BottomSprite.SetActive(false);
            LevelText.gameObject.SetActive(false);
          


            //GetText((int)Texts.DisplayName).text = essence.DisplayName;
            //GetText((int)Texts.LevelText).gameObject.SetActive(false);
        }
        else if (item.ItemClass == "Heart")
        {

            Artifactpiece heart = item as Artifactpiece;

            IconBackground.SetActive(false);
            Icon.GetComponent<Image>().sprite = heart.Icon;

            Backround.GetComponent<Image>().sprite = heart.Background;
            Backround.GetComponent<Image>().color = heart.BackgroundColor;

            DisplayName.text = heart.DisplayName;
            DisplayName.fontSize = 17;
            BottomSprite.SetActive(false);
            LevelText.gameObject.SetActive(false);
            
            // GetText((int)Texts.DisplayName).text = heart.DisplayName;
            //GetText((int)Texts.DisplayName).fontSize = 17;
            //GetText((int)Texts.LevelText).gameObject.SetActive(false);
        }
    }
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        //Bind<Text>(typeof(Texts));
        Bind<TextMeshProUGUI>(typeof(TMP_Texts));
        Bind<GameObject>(typeof(GameObjects));


        if (_item != null)
        {
            UpdateObject(_item);
            

        }


    }
    private void Start()
    {
        Init();
    }
}
