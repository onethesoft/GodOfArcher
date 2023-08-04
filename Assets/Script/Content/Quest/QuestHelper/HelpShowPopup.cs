using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestHelper_", menuName = "Quest/QuestHelper/ShowPopup")]
public class HelpShowPopup : QuestHelper
{
    [SerializeField]
    UIManager.Category _showPopup;

    public void SetPopup(UIManager.Category _category)
    {
        _showPopup = _category;
    }

    public override void DoHelp()
    {
        Managers.UI.ShowPopupUI(_showPopup);
    }

    
}
