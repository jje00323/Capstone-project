using UnityEngine;

/// <summary>
/// 몬스터가 드롭하는 아이템 프리팹에 부착되는 스크립트
/// 플레이어가 닿으면 인벤토리에 아이템을 추가하고 프리팹 파괴
/// </summary>
public class ItemPickup : MonoBehaviour
{

    private void Start()
    {
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && itemData != null && itemData.icon != null)
        {
            sr.sprite = itemData.icon;
        }
    }


    public ItemData itemData;
    public int quantity = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(" [ItemPickup] 플레이어와 충돌 감지");

            InventoryManager inventory = InventoryManager.Instance;
            if (inventory != null && itemData != null)
            {
                Debug.Log($" [ItemPickup] {itemData.itemName} x{quantity} 인벤토리에 추가 시도");
                bool added = inventory.AddItem(itemData, quantity);
                if (added)
                {
                    Debug.Log($" [ItemPickup] {itemData.itemName} x{quantity} 추가 성공");
                    var inventoryUI = FindObjectOfType<InventoryUI>();
                    if (inventoryUI != null)
                        inventoryUI.RefreshAllSlots();

                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning($" [ItemPickup] {itemData.itemName} 추가 실패 - 인벤토리 가득?");
                }
            }
            else
            {
                Debug.LogError(" [ItemPickup] InventoryManager 또는 itemData가 null입니다");
            }
        }
    }
}