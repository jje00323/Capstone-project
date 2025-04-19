using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI featureText;
    [SerializeField] private TextMeshProUGUI currentLevel;

    private SkillInfo skillData;  // 현재 적용 중인 스킬
    private SkillInfo baseSkill;  // 원본 스킬 (업그레이드 이전)


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
        skillData.currentLevel++;
        UpdateUI();
    }

    public void LevelDown()
    {
        if (skillData == null || skillData.currentLevel <= 1) return;
        skillData.currentLevel--;
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


}