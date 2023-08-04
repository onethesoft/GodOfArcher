using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "아이템/장착/장착시스템", fileName = "장착시스템_")]
public class EquipmentSystem : ScriptableObject
{
    public delegate void OnEquipHandler(EquipableItem item);
    public delegate void OnUnEquipHandler(EquipableItem item);

    public event OnEquipHandler Equip;
    public event OnUnEquipHandler UnEquip;

    [SerializeField]
    private List<EquipmentSlot> _slots;
    public IReadOnlyList<EquipmentSlot> SlotList => _slots;

    GameData _owner;
    public GameData Owner => _owner;
    public void SetOwner(GameData owner)
    {
        _owner = owner;
    }
    

    public EquipmentSystem Clone()
    {
        // return Instan
        EquipmentSystem clone = new EquipmentSystem();
        clone._slots = new List<EquipmentSlot>();
        foreach (EquipmentSlot slot in _slots)
        {
            EquipmentSlot copy = (EquipmentSlot)slot.Clone();
            copy.EquipItem = null;
            copy.EquipItem += clone.OnEquip;
            copy.UnEquipItem = null;
            copy.UnEquipItem += clone.OnUnEquip;
            copy.SetParent(clone);
            clone._slots.Add(copy);
        }
        return clone;
        
    }

    public void OnEquip( EquipableItem item)
    {
        Debug.Log("Equiopment Equip");
        Equip?.Invoke(item);
    }
    public void OnUnEquip(EquipableItem item)
    {
        UnEquip?.Invoke(item);
    }

    public EquipmentSaveData ToSaveData()
    {
        return new EquipmentSaveData { SlotList = _slots.Select(x => x.ToSaveSlotData()).ToList() };
    }
    public void Load(EquipmentSaveData data)
    {
        foreach (EquipmentSlotSaveData slotdata in data.SlotList)
            _slots.ForEach(x => x.Load(slotdata));
    }

    public void OnPlayerLevelChanged(int playerRank)
    {
        foreach (EquipmentSlot slot in _slots)
        {
            slot.UpdateUnlock();
        }
    }

    // Player 

}
