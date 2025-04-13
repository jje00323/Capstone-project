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

    public void SetSlot(SkillInfo skill)
    {
        skillData = skill;

        skillIcon.sprite = skill.skillIcon;
        skillNameText.text = skill.skillName;
        featureText.text = skill.Feature;
        currentLevel.text = skill.currentLevel.ToString();

    }

    // 클릭 시 오른쪽 SkillDetailUI에 알림
    //public void OnClickSlot()
    //{
    //    SkillDetailUI.Instance.ShowSkillDetail(skillData);
    //}
}