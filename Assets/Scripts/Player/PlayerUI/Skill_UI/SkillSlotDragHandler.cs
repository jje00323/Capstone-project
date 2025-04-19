using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static SkillInfo draggedSkill;

    public void OnBeginDrag(PointerEventData eventData)
    {
        SkillSlotUI slotUI = GetComponent<SkillSlotUI>();
        if (slotUI == null) return;

        draggedSkill = slotUI.GetCurrentSkill();

        if (draggedSkill != null && DragIconUI.Instance != null)
        {
            DragIconUI.Instance.Show(draggedSkill.skillIcon);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // DragIconUI는 자체적으로 Update에서 마우스를 따라가기 때문에
        // 여기에 따로 호출할 함수는 없음.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedSkill = null;
        if (DragIconUI.Instance != null)
        {
            DragIconUI.Instance.Hide();
        }
    }
}