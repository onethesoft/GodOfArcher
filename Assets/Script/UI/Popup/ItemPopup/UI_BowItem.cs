using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BowItem : UI_Base
{
    public delegate void SelectHandler(UI_BowItem item);
    public event SelectHandler onSelect = null;
   
    [SerializeField]
    Bow _bow;
    enum GameObjects
    {
        Selected , 
        SelectedIcon
    }
    enum Texts
    {
        DisplayName
    }

    enum Images
    {
        Icon,
        IconBackground,
    }
    public void Setup(Bow bow)
    {
        _bow = bow;
    }
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        if (_bow != null)
        {
            GetText((int)Texts.DisplayName).text = _bow.DisplayName;
            _bow.DisplayNameOutLine.AddOutline(GetText((int)Texts.DisplayName).gameObject);

            //gameObject.GetComponent<RawImage>().texture = _bow.DescriptionBackgroundTexture;
            //gameObject.GetComponent<RawImage>().color = _bow.DescriptionBackgroundColor;

            gameObject.GetComponent<Image>().sprite = _bow.DescriptionBackgroundTexture;
            gameObject.GetComponent<Image>().color = _bow.DescriptionBackgroundColor;
            _bow.DescriptionBackgroundOutline.AddOutline(gameObject);

            GetImage((int)Images.Icon).sprite = _bow.Icon;
            GetImage((int)Images.IconBackground).sprite = _bow.IconBackground;
            Debug.Log(_bow.IconBackground.name);
            _bow.IconBackgroundOutline.AddOutline(GetImage((int)Images.IconBackground).gameObject);

            UnSelect();

            AddUIEvent(gameObject, (data) => { Select(); });

        }

    }

    public void Select()
    {
        Get<GameObject>((int)GameObjects.Selected).SetActive(true);
        Get<GameObject>((int)GameObjects.SelectedIcon).SetActive(true);
        onSelect?.Invoke(this);
    }
    public void UnSelect()
    {
        Get<GameObject>((int)GameObjects.Selected).SetActive(false);
        Get<GameObject>((int)GameObjects.SelectedIcon).SetActive(false);
    }
    private void Start()
    {
        Init();
    }


}
