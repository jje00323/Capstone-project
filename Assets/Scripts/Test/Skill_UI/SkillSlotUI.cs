using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI featureText;
    [SerializeField] private TextMeshProUGUI currentLevel;
   

    private SkillInfo skillData;

    private void UpdateUI()
    {
        if (skillData == null) return;

        skillIcon.sprite = skillData.skillIcon;
        skillNameText.text = skillData.skillName;
        featureText.text = skillData.Feature;
        currentLevel.text = skillData.currentLevel.ToString();
    }

    public void SetSlot(SkillInfo skill)
    {
        skillData = skill;
        UpdateUI();

    }

    public void LevelUp()
    {
        Debug.Log($"Level_Up!");
        if (skillData == null) return;

        if (skillData.currentLevel < skillData.maxLevel)
        {
            skillData.currentLevel++;
            Debug.Log($"[레벨업] {skillData.skillName} → {skillData.currentLevel}");
            UpdateUI();
        }
        else
        {
            Debug.Log($"[레벨업 차단] {skillData.skillName}은 최대레벨입니다.");
        }
    }

    public void LevelDown()
    {
        Debug.Log($"Level_Down!");
        if (skillData == null) return;

        if (skillData.currentLevel > 1)
        {
            skillData.currentLevel--;
            Debug.Log($"[레벨다운] {skillData.skillName} → {skillData.currentLevel}");
            UpdateUI();
        }
        else
        {
            Debug.Log($"[레벨다운 차단] {skillData.skillName}은 최소레벨입니다.");
        }
    }
}