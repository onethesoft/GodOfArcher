using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI_Artifact : UI_Popup
{
    

    enum Buttons
    {
        Close,
        Exit,
        EssenceButton,
        HeartButton,
        RefundButton
    }
    enum GameObjects
    {
        ItemPanel,
        HeartSelector,
        EssenceSelector
    }
    List<UI_Essence> _essenceList;
    List<UI_Heart> _heartList;
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        _essenceList = new List<UI_Essence>();
        _heartList = new List<UI_Heart>();

        foreach (Artifact essence in Managers.Item.Database.EssenceList.OrderBy(x=>x.Type))
        {
            UI_Essence ui_essence = Util.GetOrAddComponent<UI_Essence>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_Essence" , Get<GameObject>((int)GameObjects.ItemPanel).transform));
            ui_essence.Essence = essence;

            _essenceList.Add(ui_essence);
        }

        foreach (Artifact heart in Managers.Item.Database.HeartList.OrderBy(x => x.Type))
        {
            UI_Heart ui_heart = Util.GetOrAddComponent<UI_Heart>(Managers.Resource.Instantiate("UI/SubItem/ItemPopup/UI_Heart", Get<GameObject>((int)GameObjects.ItemPanel).transform));
            ui_heart.Heart = heart;

            ui_heart.gameObject.SetActive(false);
            _heartList.Add(ui_heart);
        }
        Get<GameObject>((int)GameObjects.EssenceSelector).SetActive(true);
        Get<GameObject>((int)GameObjects.HeartSelector).SetActive(false);

        AddUIEvent(GetButton((int)Buttons.EssenceButton).gameObject, (data) => {
            Get<GameObject>((int)GameObjects.EssenceSelector).SetActive(true);
            Get<GameObject>((int)GameObjects.HeartSelector).SetActive(false);
            foreach (UI_Heart heart in _heartList)
            {
                heart.gameObject.SetActive(false);
            }
            foreach (UI_Essence essence in _essenceList)
            {
                essence.gameObject.SetActive(true);
            }
        });
        AddUIEvent(GetButton((int)Buttons.HeartButton).gameObject, (data) => {
            Get<GameObject>((int)GameObjects.EssenceSelector).SetActive(false);
            Get<GameObject>((int)GameObjects.HeartSelector).SetActive(true);
            foreach (UI_Heart heart in _heartList)
            {
                heart.gameObject.SetActive(true);
            }
            foreach (UI_Essence essence in _essenceList)
            {
                essence.gameObject.SetActive(false);
            }
        });

        AddUIEvent(GetButton((int)Buttons.RefundButton).gameObject, (data) => {
            if(Get<GameObject>((int)GameObjects.HeartSelector).activeSelf)
                Managers.Item.RefundArtifactPiece();
        });

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            ClosePopupUI();
        });

    }
    private void Start()
    {
        Init();
    }
}
