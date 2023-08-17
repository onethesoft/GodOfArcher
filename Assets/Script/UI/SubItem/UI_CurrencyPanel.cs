using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Ʃ�丮�� ����Ʈ �Ϸ� �Ŀ� �������� �г�
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
