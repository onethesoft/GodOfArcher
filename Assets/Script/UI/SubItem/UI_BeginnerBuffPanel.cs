using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BeginnerBuffPanel : MonoBehaviour
{
    GameData _playerData;
    private void Start()
    {
        _playerData = FindObjectOfType<GameData>();

    }
    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (!IsShow())
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (IsShow())
            {
                gameObject.SetActive(true);
            }
        }
            
    }

    private bool IsShow()
    {
        return 
            _playerData.Inventory.IsFindItem(Buff.Id.Buff_AttackSpeed_Beginner.ToString()) &&
            _playerData.Inventory.IsFindItem(Buff.Id.Buff_Auto_Beginner.ToString()) &&
            _playerData.Inventory.IsFindItem(Buff.Id.Buff_Attack_Beginner.ToString()) &&
            _playerData.Inventory.IsFindItem(Buff.Id.Buff_GoldDropRate_Beginner.ToString()) &&
            _playerData.Inventory.IsFindItem(Buff.Id.Buff_Skill_Beginner.ToString()) ? true : false;
    }
}
