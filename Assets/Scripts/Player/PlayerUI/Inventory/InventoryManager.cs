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
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSlots();
    }

    private void OnEnable()
    {
        ReconnectUI();
    }

    private void InitializeSlots()
    {
        slots.Clear();
        for (int i = 0; i < maxSlotCount; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public bool AddItem(ItemData item, int amount)
    {
        Debug.Log($"[AddItem] {item.itemName} x{amount} 추가 시도");

        if (InventoryUI.Instance == null)
        {
            Debug.LogWarning("[InventoryManager] InventoryUI.Instance가 null → Reconnect 시도");
            ReconnectUI();
        }

        int totalAdded = 0;

        // 1. 스택 가능한 슬롯에 추가
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item == item && item.isStackable && slot.quantity < item.maxStack)
            {
                int space = item.maxStack - slot.quantity;
                int addAmount = Mathf.Min(space, amount);
                slot.quantity += addAmount;
                amount -= addAmount;
                totalAdded += addAmount;

                Debug.Log($"[AddItem] 기존 슬롯에 {addAmount} 추가됨 → 현재 수량: {slot.quantity}");

                if (amount <= 0)
                {
                    InventoryUI.Instance?.RefreshAllSlots();
                    RefreshQuickSlots();
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

                Debug.Log($"[AddItem] 새 슬롯에 {item.itemName} x{addAmount} 추가됨");

                if (amount <= 0)
                {
                    InventoryUI.Instance?.RefreshAllSlots();
                    RefreshQuickSlots();
                    QuestManager.Instance?.UpdateCondition("CollectItem", item.itemName, totalAdded);
                    return true;
                }
            }
        }

        // 3. 아직 남은 수량이 있다면 실패
        Debug.LogWarning($"[AddItem] 인벤토리 공간 부족 → {item.itemName} x{amount} 남음");

        if (totalAdded > 0)
        {
            QuestManager.Instance?.UpdateCondition("CollectItem", item.itemName, totalAdded);
        }

        return false;
    }

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

                InventoryUI.Instance?.RefreshAllSlots();
                RefreshQuickSlots();
                return;
            }
        }
    }

    private void RefreshQuickSlots()
    {
        QuickSlotUI[] quickSlots = FindObjectsOfType<QuickSlotUI>(true); // 비활성 포함
        foreach (var qs in quickSlots)
        {
            if (qs != null)
                qs.RefreshSlotUI();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[Inventory] 골드 +{amount} → 총 {gold}");
    }

    public int GetGold() => gold;

    private void ReconnectUI()
    {
        var inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI != null)
        {
            InventoryUI.Instance = inventoryUI; // 중요!
            Debug.Log("[InventoryManager] InventoryUI 재연결 성공");
            inventoryUI.RefreshAllSlots();
        }
        else
        {
            Debug.LogWarning("[InventoryManager] InventoryUI를 찾을 수 없습니다.");
        }
    }
}