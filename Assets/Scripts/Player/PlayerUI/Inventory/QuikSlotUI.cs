using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class QuickSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("슬롯 설정")]
    [SerializeField] private int slotIndex;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("입력 액션 연결")]
    [SerializeField] private InputActionReference useSlotAction;

    private ItemData currentItem;
    private int currentAmount = 0;

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

    public void SetItem(ItemData item, int amount = 1)
    {
        currentItem = item;
        currentAmount = amount;
        RefreshSlotUI();
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentAmount = 0;
        iconImage.enabled = false;
        countText.text = "";
    }

    public void UseItem()
    {
        if (currentItem is ConsumableData consumable)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null && player.TryGetComponent(out PlayerStatus playerStatus))
            {
                consumable.ApplyEffect(playerStatus);
            }

            currentAmount--;
            if (currentAmount <= 0)
                ClearSlot();
            else
                countText.text = currentAmount.ToString();
        }
    }

    public ItemData GetItem() => currentItem;
    public int GetAmount() => currentAmount;

    public void OnDrop(PointerEventData eventData)
    {
        if (InventorySlotUI.draggedItem == null || InventorySlotUI.draggedSlotUI == null) return;
        if (InventorySlotUI.draggedItem.itemType != ItemType.Consumable) return;

        var draggedItem = InventorySlotUI.draggedItem;
        var draggedFrom = InventorySlotUI.draggedSlotUI;

        // 기존 아이템 백업
        var oldItem = currentItem;
        var oldAmount = currentAmount;

        // 현재 슬롯에 드래그한 아이템 등록
        SetItem(draggedItem, draggedFrom is QuickSlotUI qs ? qs.GetAmount() : 1);

        // 드래그 원본에 기존 아이템 되돌려 놓기
        if (draggedFrom is QuickSlotUI fromQuick)
        {
            fromQuick.SetItem(oldItem, oldAmount);
        }
        else if (draggedFrom is InventorySlotUI fromInv)
        {
            if (oldItem != null)
            {
                fromInv.SetItemToSlot(oldItem, oldAmount);
            }
            else
            {
                fromInv.RemoveItemFromSlot();
            }
        }

        // 드래그 상태 초기화
        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        draggedQuickSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        InventorySlotUI.draggedItem = currentItem;
        InventorySlotUI.draggedSlotUI = this;
        draggedQuickSlot = this;
        DragIconUI.Instance.Show(currentItem.icon);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        draggedQuickSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void RefreshSlotUI()
    {
        if (currentItem != null)
        {
            iconImage.sprite = currentItem.icon;
            iconImage.enabled = true;
            countText.text = currentAmount > 1 ? currentAmount.ToString() : "";
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
}