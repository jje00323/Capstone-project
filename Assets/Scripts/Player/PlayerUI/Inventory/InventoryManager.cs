using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("인벤토리 설정")]
    public int maxSlotCount = 48;
    public List<InventorySlot> slots = new();
    private int gold;

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
        Debug.Log($"[AddItem] {item.itemName} x{amount} 추가 시도");

        int totalAdded = 0;

        // 1. 스택 가능한 슬롯에 우선 추가
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item == item && item.isStackable && slot.quantity < item.maxStack)
            {
                int space = item.maxStack - slot.quantity;
                int addAmount = Mathf.Min(space, amount);
                slot.quantity += addAmount;
                amount -= addAmount;
                totalAdded += addAmount;

                Debug.Log($"[AddItem] 기존 스택 슬롯에 {addAmount} 추가됨 → 현재: {slot.quantity}");

                if (amount <= 0)
                {
                    InventoryUI.Instance?.RefreshAllSlots(); //  인벤토리 UI 갱신
                    RefreshQuickSlots();

                    // 퀘스트 조건 반영
                    QuestManager.Instance?.UpdateCondition("CollectItem", item.itemName, totalAdded);
                    return true;
                }
            }
        }

        // 2. 빈 슬롯에 새로 추가
        foreach (var slot in slots)
        {
            if (slot.item == null)
            {
                int addAmount = item.isStackable ? Mathf.Min(item.maxStack, amount) : 1;

                slot.item = item;
                slot.quantity = addAmount;
                amount -= addAmount;
                totalAdded += addAmount;

                Debug.Log($"[AddItem] 빈 슬롯에 {item.itemName} x{addAmount} 추가됨");

                if (amount <= 0)
                {
                    InventoryUI.Instance?.RefreshAllSlots(); //  인벤토리 UI 갱신
                    RefreshQuickSlots();

                    // 퀘스트 조건 반영
                    QuestManager.Instance?.UpdateCondition("CollectItem", item.itemName, totalAdded);
                    return true;
                }
            }
        }

        // 3. 아직도 남은 수량이 있다면 실패
        Debug.LogWarning($"[AddItem] 실패: {item.itemName} x{amount} 남음 → 인벤토리에 추가 불가");

        // 그래도 일부 추가된 경우 조건 반영
        if (totalAdded > 0)
        {
            QuestManager.Instance?.UpdateCondition("CollectItem", item.itemName, totalAdded);
        }

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

                InventoryUI.Instance?.RefreshAllSlots(); //  아이템 제거 후에도 UI 갱신
                RefreshQuickSlots();
                return;
            }
        }
    }

    private void RefreshQuickSlots()
    {
        QuickSlotUI[] quickSlots = GameObject.FindObjectsOfType<QuickSlotUI>();
        foreach (var qs in quickSlots)
        {
            qs.RefreshSlotUI();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[Inventory] 골드 +{amount} → 총 {gold}");
        // 필요 시 UI 갱신 로직 추가
    }

    public int GetGold() => gold;
}