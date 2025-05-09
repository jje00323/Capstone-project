using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Helmet,
    Chest,
    Legs,
    Gloves,
    Boots,
    Rune
}

public class EquipmentData : ItemData
{
    [Header("공통 장비 정보")]
    public EquipmentType equipmentType;

    [Header("스탯 변화")]
    public List<StatModifier> statModifiers = new();
}

public class EquipmentManager : MonoBehaviour
{
    [Header("참조")]
    public PlayerStatus playerStatus;

    // 현재 착용 중인 장비 (부위별로 관리)
    private Dictionary<EquipmentType, EquipmentData> equippedItems = new();

    /// <summary>
    /// 장비 착용
    /// </summary>
    public void EquipItem(EquipmentData equipment)
    {
        if (equipment == null) return;

        // 기존 장비가 있다면 먼저 해제
        if (equippedItems.TryGetValue(equipment.equipmentType, out var oldEquip))
        {
            UnequipItem(oldEquip);
        }

        // 새 장비 착용
        equippedItems[equipment.equipmentType] = equipment;
        StatusEffectApplier.ApplyStatModifiers(playerStatus, equipment.statModifiers);

        Debug.Log($"[장비 장착] {equipment.itemName} 장착됨");
    }

    /// <summary>
    /// 장비 해제
    /// </summary>
    public void UnequipItem(EquipmentData equipment)
    {
        if (equipment == null) return;

        var inverseMods = new List<StatModifier>();
        foreach (var mod in equipment.statModifiers)
        {
            inverseMods.Add(new StatModifier(mod.type, -mod.value, mod.isFlat));
        }

        StatusEffectApplier.ApplyStatModifiers(playerStatus, inverseMods);
        equippedItems.Remove(equipment.equipmentType);

        Debug.Log($"[장비 해제] {equipment.itemName} 해제됨");
    }

    /// <summary>
    /// 특정 부위 장비 확인
    /// </summary>
    public EquipmentData GetEquipped(EquipmentType type)
    {
        return equippedItems.TryGetValue(type, out var equip) ? equip : null;
    }

    /// <summary>
    /// 모든 장비 해제
    /// </summary>
    public void UnequipAll()
    {
        foreach (var equip in new List<EquipmentData>(equippedItems.Values))
        {
            UnequipItem(equip);
        }
    }
}
