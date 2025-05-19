using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemObject : MonoBehaviour
{
    [Header("아이템 데이터")]
    public ItemData itemData;
    public int quantity = 1;

    [Header("회전 및 부유 효과")]
    [SerializeField] private bool useFloatingEffect = true;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatFrequency = 1f;

    [Header("등급 이펙트")]
    [SerializeField] private GameObject rarityEffect; // 하위 이펙트 오브젝트

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;

        // 아이콘 설정 (SpriteRenderer용)
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && itemData != null && itemData.icon != null)
        {
            sr.sprite = itemData.icon;
        }

        // 등급 이펙트 색상 설정
        if (rarityEffect != null && itemData != null)
        {
            Color color = GetColorByRarity(itemData.rarity);

            // 1) Particle System
            var ps = rarityEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = color;
            }

            // 2) Renderer (예: Glow Mesh)
            var renderer = rarityEffect.GetComponent<Renderer>();
            if (renderer != null && renderer.material.HasProperty("_Color"))
            {
                renderer.material.color = color;
            }

            // 3) Light
            var light = rarityEffect.GetComponent<Light>();
            if (light != null)
            {
                light.color = color;
            }
        }
    }

    private void Update()
    {
        if (!useFloatingEffect) return;

        // 회전
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // 부유
        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0f, newY, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (itemData == null)
        {
            Debug.LogError("[ItemObject] itemData가 null입니다.");
            return;
        }

        var inventory = InventoryManager.Instance;
        var quickSlots = FindObjectsOfType<QuickSlotUI>();

        // 소비 아이템일 경우, 퀵슬롯에서 먼저 확인
        if (itemData.itemType == ItemType.Consumable)
        {
            foreach (var slot in quickSlots)
            {
                if (slot.GetItem() == itemData)
                {
                    InventoryManager.Instance.AddItem(itemData, quantity);
                    slot.RefreshSlotUI();
                    Debug.Log($"[ItemObject] 퀵슬롯 수량 증가: {itemData.itemName}");
                    Destroy(gameObject);
                    return;
                }
            }
        }

        // 인벤토리에 추가
        if (inventory != null)
        {
            bool added = inventory.AddItem(itemData, quantity);
            if (added)
            {
                Debug.Log($"[ItemObject] 인벤토리에 추가: {itemData.itemName} x{quantity}");
                var inventoryUI = FindObjectOfType<InventoryUI>();
                if (inventoryUI != null)
                    inventoryUI.RefreshAllSlots();

                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("[ItemObject] 인벤토리 추가 실패");
            }
        }
    }

    private Color GetColorByRarity(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Normal: return Color.white;
            case ItemRarity.Rare: return Color.blue;
            case ItemRarity.Epic: return new Color(0.6f, 0f, 0.8f); // 보라색
            default: return Color.gray;
        }
    }
}