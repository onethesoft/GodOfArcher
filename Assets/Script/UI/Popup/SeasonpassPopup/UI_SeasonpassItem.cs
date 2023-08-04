using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SeasonpassItem : UI_Base
{
    enum Texts
    {
        Stage
    }
    enum GameObjects
    {
        UI_SeasonpassQuestItem_free,
        UI_SeasonpassQuestItem_pass
    }
    Quest _free;
    Quest _pass;
    int _stage;
    public void Setup(int stage , Quest freeQuest , Quest passQuest)
    {
        _stage = stage;
        _free = freeQuest;
        _pass = passQuest;

    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        Debug.Assert(_free != null && _pass != null, "UI_SeasonpassItem Free and Pass Quest item not Setup");

        Util.GetOrAddComponent<UI_SeasonpassQuestItem>(Get<GameObject>((int)GameObjects.UI_SeasonpassQuestItem_free)).Setup(_free);
        Util.GetOrAddComponent<UI_SeasonpassQuestItem>(Get<GameObject>((int)GameObjects.UI_SeasonpassQuestItem_pass)).Setup(_pass);

        GetText((int)Texts.Stage).text = _stage.ToString();



    }
}
