using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillEquipSlotUI : MonoBehaviour, IDropHandler
{
    [Header("슬롯 키 지정 (예: Q, W, E, R)")]
    [SerializeField] public string slotKey;

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

        foreach (var pair in SkillEquipManager.Instance.GetAllEquippedSkills())
        {
            var equipped = pair.Value;

            bool isSameLine =
                equipped == draggedSkill ||
                equipped.originalSkill == draggedSkill ||
                draggedSkill.originalSkill == equipped;

            if (isSameLine)
            {
                Debug.LogWarning($"[SkillEquipSlotUI] 계열 중복: {draggedSkill.skillName}은 이미 슬롯 {pair.Key}에 장착됨");
                return; // 장착 금지
            }
        }

        // 장착 허용
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