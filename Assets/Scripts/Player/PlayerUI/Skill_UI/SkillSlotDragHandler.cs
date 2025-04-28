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

        //  드래그 시작 시 업그레이드 UI에 현재 슬롯 등록
        SkillUpgradeUI.Instance.SetCurrentSlot(slotUI);

        SkillUpgradeUI.Instance.ShowSkillDetail(draggedSkill.originalSkill ?? draggedSkill, slotUI);

        if (draggedSkill != null)
        {
            DragIconUI.Instance.Show(draggedSkill.skillIcon);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Optional: 아이콘 움직임은 DragIconUI에서 처리 중
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedSkill = null;
        DragIconUI.Instance.Hide();
    }
}