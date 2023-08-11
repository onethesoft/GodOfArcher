using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConvertTarget : MonoBehaviour
{
    public System.Action<BaseItem> OnSelect;
    [SerializeField]
    UI_BaseItem baseItem;
    [HideInInspector]
    public BaseItem item;
    public void SetItem(BaseItem item)
    {
        baseItem.Item = item;
        this.item = item;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {

            OnSelect?.Invoke(item);
        });
    }
}
