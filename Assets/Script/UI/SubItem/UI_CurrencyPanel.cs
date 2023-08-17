using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 튜토리얼 퀘스트 완료 후에 보여지는 패널
 */
public class UI_CurrencyPanel : MonoBehaviour
{
    GameData playerData;
    // Start is called before the first frame update
    void Start()
    {
        playerData = FindObjectOfType<GameData>();
        if (playerData.IsAllCompletedQuest(Define.QuestType.Tutorial))
        {
            gameObject.SetActive(true);
        }
        else
        {
            Managers.Quest.onQuestCompleted += OnCompletedQuest;
            gameObject.SetActive(false);
        }


    }

    void OnCompletedQuest(Quest quest)
    {
        if (!gameObject.activeSelf && playerData.IsAllCompletedQuest(Define.QuestType.Tutorial))
            gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        Managers.Quest.onQuestCompleted -= OnCompletedQuest;
    }

}
