using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RandomboxTable : UI_Popup
{
    public enum Mode
    {
        RuneAndPet,
        Other
    }

    enum GameObjects
    {
        ItemTable
    }


    enum Texts
    {
        TitleText
    }
    enum Buttons
    {
        Close,
        Exit
    }
    Mode _mode;

    string[] Titletexts = new string[] { "·éÆê »Ì±â È®·üÇ¥", "Àåºñ »Ì±â È®·üÇ¥" };

    [SerializeField]
    List<Sprite> _itemSprites;

    public void SetMode(Mode mode)
    {
        _mode = mode;
    }
    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        GetText((int)Texts.TitleText).text = Titletexts[(int)_mode];

        foreach(Rune.Rank _level in System.Enum.GetValues(typeof(Rune.Rank)))
        {
            Util.GetOrAddComponent<UI_RandomboxTableItem>(Managers.Resource.Instantiate($"UI/SubItem/RandomboxPopup/UI_RandomboxTableItem", Get<GameObject>((int)GameObjects.ItemTable).transform)).Set((int)_level,_itemSprites[(int)_level], _mode);

        }

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => {
            ClosePopupUI();
        });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => {
            ClosePopupUI();
        });

    }
}
