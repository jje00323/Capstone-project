using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class QuickSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("슬롯 설정")]
    [SerializeField] private int slotIndex;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("입력 액션 연결")]
    [SerializeField] private InputActionReference useSlotAction;

    private InventorySlot referencedSlot;  // 인벤토리 슬롯 참조

    private System.Action<InputAction.CallbackContext> cachedCallback;
    public static QuickSlotUI draggedQuickSlot;

    private void OnEnable()
    {
        if (useSlotAction != null)
        {
            cachedCallback = ctx => UseItem();
            useSlotAction.action.performed += cachedCallback;
            useSlotAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (useSlotAction != null && cachedCallback != null)
        {
            useSlotAction.action.performed -= cachedCallback;
            useSlotAction.action.Disable();
        }
    }

    public void SetReference(InventorySlot slot)
    {
        referencedSlot = slot;
        RefreshSlotUI();
    }

    public void ClearSlot()
    {
        referencedSlot = null;
        iconImage.enabled = false;
        countText.text = "";
    }

    public void UseItem()
    {
        if (referencedSlot == null || referencedSlot.item == null) return;

        if (referencedSlot.item is ConsumableData consumable)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null && player.TryGetComponent(out PlayerStatus playerStatus))
            {
                consumable.ApplyEffect(playerStatus);
            }

            InventoryManager.Instance.RemoveItem(referencedSlot.item, 1);

            QuestManager.Instance?.UpdateCondition("UseItem", referencedSlot.item.itemName);


            RefreshSlotUI();

            if (referencedSlot.quantity <= 0 || referencedSlot.item == null)
            {
                ClearSlot();
            }
        }
    }

    public ItemData GetItem() => referencedSlot?.item;
    public int GetAmount() => referencedSlot?.quantity ?? 0;

    public void OnDrop(PointerEventData eventData)
    {
        if (InventorySlotUI.draggedItem == null || InventorySlotUI.draggedSlotUI == null) return;
        if (InventorySlotUI.draggedItem.itemType != ItemType.Consumable) return;

        if (InventorySlotUI.draggedSlotUI is InventorySlotUI invSlot)
        {
            var slot = invSlot.GetSlotData();
            SetReference(slot);
        }
        else if (InventorySlotUI.draggedSlotUI is QuickSlotUI quickSlot)
        {
            var oldSlot = referencedSlot;
            referencedSlot = quickSlot.referencedSlot;
            quickSlot.referencedSlot = oldSlot;

            quickSlot.RefreshSlotUI();
        }

        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        draggedQuickSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (referencedSlot == null || referencedSlot.item == null) return;

        InventorySlotUI.draggedItem = referencedSlot.item;
        InventorySlotUI.draggedSlotUI = this;
        draggedQuickSlot = this;
        DragIconUI.Instance.Show(referencedSlot.item.icon);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventorySlotUI.draggedSlotUI == this)
        {
            Debug.Log("[QuickSlotUI] 슬롯 밖으로 드래그됨 → 슬롯 해제됨");
            ClearSlot();
        }

        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        draggedQuickSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void RefreshSlotUI()
    {
        if (referencedSlot != null && referencedSlot.item != null)
        {
            iconImage.sprite = referencedSlot.item.icon;
            iconImage.enabled = true;
            countText.text = referencedSlot.quantity > 1 ? referencedSlot.quantity.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void RemoveItemFromSlot()
    {
        ClearSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (referencedSlot == null || referencedSlot.item == null) return;

        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Show(referencedSlot.item.itemName, referencedSlot.item.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Hide();
    }
}
