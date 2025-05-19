//using UnityEngine;

//public class ItemPickup : MonoBehaviour
//{
//    public ItemData itemData;
//    public int quantity = 1;

//    private void Start()
//    {
//        var sr = GetComponentInChildren<SpriteRenderer>();
//        if (sr != null && itemData != null && itemData.icon != null)
//        {
//            sr.sprite = itemData.icon;
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        if (itemData == null)
//        {
//            Debug.LogError("[ItemPickup] itemData가 null입니다");
//            return;
//        }

//        var inventory = InventoryManager.Instance;
//        var quickSlots = FindObjectsOfType<QuickSlotUI>();

//        //  퀵슬롯에 동일한 소비 아이템이 있을 경우 → 수량 증가
//        if (itemData.itemType == ItemType.Consumable)
//        {
//            foreach (var slot in quickSlots)
//            {
//                if (slot.GetItem() == itemData)
//                {
//                    //  인벤토리에서 실제 수량도 증가 → QuickSlotUI는 자동 반영됨
//                    InventoryManager.Instance.AddItem(itemData, quantity);

//                    slot.RefreshSlotUI(); // 퀵슬롯도 수량 반영
//                    Debug.Log($"[ItemPickup] 퀵슬롯 수량 증가: {itemData.itemName}");
//                    Destroy(gameObject);
//                    return;
//                }
//            }
//        }

//        //  인벤토리에 아이템 추가 (소비 아이템이든 아니든 기본은 인벤토리)
//        if (inventory != null)
//        {
//            bool added = inventory.AddItem(itemData, quantity);
//            if (added)
//            {
//                Debug.Log($"[ItemPickup] 인벤토리에 추가: {itemData.itemName} x{quantity}");
//                var inventoryUI = FindObjectOfType<InventoryUI>();
//                if (inventoryUI != null)
//                    inventoryUI.RefreshAllSlots();

//                Destroy(gameObject);
//            }
//            else
//            {
//                Debug.LogWarning($"[ItemPickup] 추가 실패 - 인벤토리 가득?");
//            }
//        }
//    }
//}