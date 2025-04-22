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
        currentLevel.text = skillData.currentLevel.ToString();
    }

    public void LevelUp()
    {
        if (skillData == null || skillData.currentLevel >= skillData.maxLevel) return;

        skillData.SyncLevelRecursive(skillData.currentLevel + 1); //  핵심
        UpdateUI();
    }

    public void LevelDown()
    {
        if (skillData == null || skillData.currentLevel <= 1) return;

        skillData.SyncLevelRecursive(skillData.currentLevel - 1); //  핵심
        UpdateUI();
    }

    public void UpgradeSkill(SkillInfo upgraded)
    {
        skillData = upgraded;
        UpdateUI();
    }

    public void ResetToOriginal()
    {
        skillData = baseSkill;
        UpdateUI();
    }

    public void OnClickSlot()
    {
        // Upgrade UI에는 항상 baseSkill 기준 전달
        SkillUpgradeUI.Instance.ShowSkillDetail(baseSkill, this);
    }

    //  여기부터 드래그 관련 인터페이스 구현 
    public void OnBeginDrag(PointerEventData eventData)
    {
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