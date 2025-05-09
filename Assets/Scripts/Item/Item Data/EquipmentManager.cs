using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    [Header("참조")]
    public PlayerStatus playerStatus;

    private Dictionary<EquipmentType, EquipmentData> equippedItems = new();
    private Dictionary<EquipmentType, List<StatModifier>> appliedModifiers = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[EquipmentManager] 인스턴스 초기화 완료");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EquipItem(EquipmentData equipment)
    {
        if (equipment == null) return;

        var currentJob = JobManager.Instance.GetCurrentJob();
        if (!equipment.allowedJobs.Contains(currentJob))
        {
            Debug.LogWarning($"[{equipment.itemName}]은(는) {currentJob} 직업이 착용할 수 없습니다.");
            return;
        }

        if (equippedItems.TryGetValue(equipment.equipmentType, out var oldEquip))
        {
            UnequipItem(oldEquip);
        }

        equippedItems[equipment.equipmentType] = equipment;

        var modifiers = equipment.GetAllStatModifiers();
        appliedModifiers[equipment.equipmentType] = new List<StatModifier>(modifiers);
        StatusEffectApplier.ApplyStatModifiers(playerStatus, modifiers);

        Debug.Log($"[장비 장착] {equipment.itemName} 장착됨");
        Debug.Log($"[스탯 적용 후] maxHP: {playerStatus.maxHP}, maxMP: {playerStatus.maxMP}, " +
              $"Attack: {playerStatus.attack}, Defense: {playerStatus.defense}");
    }

    public void UnequipItem(EquipmentData item)
    {
        if (item == null || playerStatus == null)
        {
            Debug.LogError("[UnequipItem] item 또는 playerStatus가 null");
            return;
        }

        Debug.Log($"[UnequipItem] 장비 해제: {item.itemName}");

        if (!appliedModifiers.TryGetValue(item.equipmentType, out var modifiers) || modifiers == null)
        {
            Debug.LogWarning("[UnequipItem] 적용된 스탯 정보가 없습니다");
            return;
        }

        var negativeMods = new List<StatModifier>();
        foreach (var mod in modifiers)
        {
            Debug.Log($"[UnequipItem] -{mod.type}: -{mod.value} (flat: {mod.isFlat})");
            negativeMods.Add(new StatModifier(mod.type, -mod.value, mod.isFlat));
        }

        StatusEffectApplier.ApplyStatModifiers(playerStatus, negativeMods);
        appliedModifiers.Remove(item.equipmentType);

        if (equippedItems.ContainsKey(item.equipmentType))
        {
            equippedItems.Remove(item.equipmentType);
            Debug.Log($"[UnequipItem] equippedItems에서 제거됨: {item.equipmentType}");
        }
    }

    public EquipmentData GetEquipped(EquipmentType type)
    {
        return equippedItems.TryGetValue(type, out var equip) ? equip : null;
    }

    public void UnequipAll()
    {
        foreach (var equip in new List<EquipmentData>(equippedItems.Values))
        {
            UnequipItem(equip);
        }
    }
}
