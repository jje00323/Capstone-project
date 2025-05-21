using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI featureText;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject lockOverlay;

    public SkillInfo skillData;  // 현재 적용 중인 스킬
    public SkillInfo baseSkill;  // 원본 스킬 (업그레이드 이전)

    public static SkillInfo draggedSkill;
    public void SetSlot(SkillInfo skill)
    {
        skillData = skill;
        baseSkill = skill.originalSkill == null ? skill : skill.originalSkill;

        UpdateUI();

        // 클릭 이벤트 연결 (중복 방지)
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickSlot);
        }
    }

    public SkillInfo GetCurrentSkill() => skillData;

    public void UpdateUI()
    {
        if (skillData == null) return;

        skillIcon.sprite = skillData.skillIcon;
        skillNameText.text = skillData.skillName;
        featureText.text = skillData.Feature;
        currentLevel.text = $"{skillData.currentLevel}";

        //  잠금 여부 시각화
        if (lockOverlay != null)
            lockOverlay.SetActive(skillData.currentLevel <= 0);

        //  비용 표시
        int nextLevel = skillData.currentLevel + 1;
        costText.text = (nextLevel > skillData.maxLevel)
            ? "  최대레벨"
            : $"레벨업 비용:{nextLevel}";
    }

    public void LevelUp()
    {
        if (skillData == null || skillData.currentLevel >= skillData.maxLevel)
            return;

        int cost = skillData.currentLevel + 1;

        if (!SkillPointManager.Instance.TrySpend(cost))
        {
            Debug.LogWarning($"스킬 포인트 부족: {cost} 필요");
            return;
        }

        skillData.SyncLevelRecursive(skillData.currentLevel + 1);
        UpdateUI();
        SkillUIManager.Instance.UpdateSkillPointText();
    }

    public void LevelDown()
    {
        if (skillData == null || skillData.currentLevel <= 1) return;

        int refund = skillData.currentLevel; // 현재레벨 → 감소 후 레벨+1이니까

        skillData.SyncLevelRecursive(skillData.currentLevel - 1);
        SkillPointManager.Instance.Refund(refund);

        UpdateUI();
        SkillUIManager.Instance.UpdateSkillPointText();
    }

    public void UpgradeSkill(SkillInfo upgraded)
    {
        skillData = upgraded;
        UpdateUI();
        SkillUpgradeManager.Instance.SetCurrentUpgrade(baseSkill, upgraded);

        //  SkillEquipManager에서 슬롯 키를 찾고 업그레이드 적용
        string slotKey = SkillEquipManager.Instance.FindEquippedSlotKey(baseSkill);
        if (!string.IsNullOrEmpty(slotKey))
        {
            SkillEquipManager.Instance.EquipSkill(slotKey, upgraded);
            Debug.Log($"[SkillSlotUI] 슬롯 {slotKey}의 스킬을 {upgraded.skillName}으로 업그레이드 반영함");
        }
    }



    public void ResetToOriginal()
    {
        skillData = baseSkill;
        UpdateUI();
    }

    public void OnClickSlot()
    {
        if (SkillUpgradeUI.Instance == null)
        {
            Debug.LogWarning("[SkillSlotUI] SkillUpgradeUI가 아직 초기화되지 않았습니다.");
            return;
        }

        SkillUpgradeUI.Instance.ShowSkillDetail(baseSkill, this);
    }

    //  여기부터 드래그 관련 인터페이스 구현 
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skillData == null || skillData.currentLevel <= 0)
        {
            Debug.Log("[SkillSlotUI] 잠금된 스킬은 드래그할 수 없습니다.");
            return;
        }

        draggedSkill = skillData;
        if (draggedSkill != null)
        {
            DragIconUI.Instance.Show(draggedSkill.skillIcon);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 마우스를 따라오게 하려면 DragIconUI가 Update로 처리 중이어야 함
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedSkill = null;
        DragIconUI.Instance.Hide();
    }
}