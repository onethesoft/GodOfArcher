using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopupQuestReporter : MonoBehaviour
{
    [SerializeField]
    QuestReporter _reporter;

    public void DoReport()
    {
        _reporter.DoReport();
    }
}
