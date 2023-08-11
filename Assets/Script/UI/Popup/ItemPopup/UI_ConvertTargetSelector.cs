using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConvertTargetSelector : MonoBehaviour
{
    public System.Action<BaseItem> OnSelectItem;

    [SerializeField]
    GameObject ItemPrefabToSpawn;

    [SerializeField]
    GameObject SelectorGrid;

    [SerializeField]
    Text TitleText;

    public UI_Convert.ItemType ItemType;

    private List<UI_ConvertTarget> listTarget = new List<UI_ConvertTarget>();

    private void Start()
    {
        if (ItemType == UI_Convert.ItemType.Rune)
        {
            TitleText.text = "∑È º±≈√";
            SelectorGrid.GetComponent<GridLayoutGroup>().constraintCount = 3;
        }
        else
        {
            TitleText.text = "∆Í º±≈√";
            SelectorGrid.GetComponent<GridLayoutGroup>().constraintCount = 2;
        }

        List<EquipableItem> itemList = Managers.Item.Database.ItemList.Where(x => (x.ItemClass == ItemType.ToString()) && x is EquipableItem).Select(x => x as EquipableItem).OrderBy(x => x.Level).ToList();
        itemList.RemoveAll(x => x.Level > (int)EquipableItem.Rank.S || x.Level < (int)EquipableItem.Rank.A);

        foreach (EquipableItem item in itemList)
        {
            UI_ConvertTarget _base = Util.GetOrAddComponent<UI_ConvertTarget>(Managers.Resource.Instantiate(ItemPrefabToSpawn, SelectorGrid.transform));
            _base.SetItem(item);
            _base.OnSelect += OnSelect;

            listTarget.Add(_base);


        }
    }
   

    private void OnSelect(BaseItem item)
    {

        OnSelectItem?.Invoke(item);
    }
    public void UpdateItem(BaseItem item)
    {
        EquipableItem target = item as EquipableItem;
        if (target != null)
        {
            foreach (UI_ConvertTarget convertItem in listTarget)
            {
                EquipableItem _target = convertItem.item as EquipableItem;
                if (target.Level != _target.Level || target.ItemId == _target.ItemId)
                {
                    convertItem.gameObject.SetActive(false);
                }


            }
        }
        else
        {
            foreach (UI_ConvertTarget convertItem in listTarget)
            {
                EquipableItem _target = convertItem.item as EquipableItem;
                convertItem.gameObject.SetActive(true);
                

            }
        }
    }

    private void OnDestroy()
    {
        foreach (UI_ConvertTarget convertItem in listTarget)
        {

            convertItem.OnSelect = null;

        }
        listTarget.Clear();
    }
}
