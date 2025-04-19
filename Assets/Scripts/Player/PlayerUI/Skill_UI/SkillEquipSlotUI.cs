using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillEquipSlotUI : MonoBehaviour, IDropHandler
{
    [Header("슬롯 키 지정 (예: Q, W, E, R)")]
    [SerializeField] private string slotKey;

    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI keyText;

    private void Start()
    {
        ClearSlot();
        if (keyText != null)
            keyText.text = slotKey;
    }

    public void OnDrop(PointerEventData eventData)
    {
        SkillInfo draggedSkill = SkillSlotDragHandler.draggedSkill;

        if (draggedSkill == null)
        {
            Debug.LogWarning("[SkillEquipSlotUI] draggedSkill이 null입니다.");
            return;
        }

        if (SkillEquipManager.Instance == null)
        {
            Debug.LogError("[SkillEquipSlotUI] SkillEquipManager.Instance가 null입니다.");
            return;
        }

        SkillEquipManager.Instance.EquipSkill(slotKey, draggedSkill);
        SetSkillIcon(draggedSkill);

        Debug.Log($"[SkillEquipSlotUI] [{slotKey}] 슬롯에 {draggedSkill.skillName} 장착 완료");
    }

    public void SetSkillIcon(SkillInfo skill)
    {
        iconImage.sprite = skill.skillIcon;
        iconImage.enabled = true;
    }

    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
    }
}