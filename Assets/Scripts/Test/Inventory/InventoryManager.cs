using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("인벤토리 설정")]
    public int maxSlotCount = 48;
    public List<InventorySlot> slots = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        slots.Clear();
        for (int i = 0; i < maxSlotCount; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// 아이템을 인벤토리에 추가
    /// </summary>
    public bool AddItem(ItemData item, int amount)
    {
        Debug.Log($" [AddItem] {item.itemName} x{amount} 추가 시도");

        // 스택 가능 슬롯
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item == item && item.isStackable && slot.quantity < item.maxStack)
            {
                int space = item.maxStack - slot.quantity;
                int addAmount = Mathf.Min(space, amount);
                slot.quantity += addAmount;
                amount -= addAmount;

                Debug.Log($" [AddItem] 스택된 슬롯에 {addAmount} 추가됨 (남은 수량: {amount})");

                if (amount <= 0)
                    return true;
            }
        }

        // 빈 슬롯
        foreach (var slot in slots)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.quantity = amount;
                Debug.Log($" [AddItem] 빈 슬롯에 {item.itemName} x{amount} 추가됨");
                return true;
            }
        }

        Debug.LogWarning($" [AddItem] 실패: 빈 슬롯 없음. {item.itemName} x{amount} 추가 불가");
        return false;
    }

    /// <summary>
    /// 아이템 제거
    /// </summary>
    public void RemoveItem(ItemData targetItem, int amount = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot.item == targetItem)
            {
                slot.quantity -= amount;

                if (slot.quantity <= 0)
                    slot.Clear();

                return;
            }
        }
    }
}