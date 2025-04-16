using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUpgradeUI : MonoBehaviour
{
    public static SkillUpgradeUI Instance;

    [Header("현재 스킬 설명")]
    public Image skillImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI featureText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI manaText;

    [Header("업그레이드 버튼들")]
    public Button originalButton;
    public Button upgrade1Button;
    public Button upgrade2Button;
    public Button upgrade3Button;

    private SkillInfo baseSkill;
    private SkillSlotUI currentSlot;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowSkillDetail(SkillInfo original, SkillSlotUI slot)
    {
        baseSkill = original;
        currentSlot = slot;

        // 설명은 현재 슬롯이 가지고 있는 스킬 기준
        UpdateDescription(currentSlot.GetCurrentSkill());

        // 업그레이드 버튼은 baseSkill 기준
        AssignUpgradeButton(originalButton, baseSkill, isOriginal: true);
        AssignUpgradeButton(upgrade1Button, GetUpgradeOption(0));
        AssignUpgradeButton(upgrade2Button, GetUpgradeOption(1));
        AssignUpgradeButton(upgrade3Button, GetUpgradeOption(2));
    }

    private SkillInfo GetUpgradeOption(int index)
    {
        if (baseSkill.upgradeOptions != null && baseSkill.upgradeOptions.Length > index)
            return baseSkill.upgradeOptions[index];
        return null;
    }

    private void AssignUpgradeButton(Button btn, SkillInfo skill, bool isOriginal = false)
    {
        btn.onClick.RemoveAllListeners();

        if (skill == null)
        {
            btn.interactable = false;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "없음";
        }
        else
        {
            btn.interactable = true;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;

            btn.onClick.AddListener(() =>
            {
                Debug.Log($"[SkillUpgradeUI] {skill.skillName} 선택됨");

                if (currentSlot != null)
                {
                    currentSlot.UpgradeSkill(skill);           // 스킬 적용
                    UpdateDescription(skill);                 // 오른쪽 설명 갱신
                }

                // 업그레이드 버튼은 baseSkill 기준으로 유지
                // original 버튼 클릭 시에도 upgrade UI는 바꾸지 않음
            });
        }
    }

    private void UpdateDescription(SkillInfo skill)
    {
        skillImage.sprite = skill.skillIcon;
        nameText.text = skill.skillName;
        featureText.text = skill.Feature;
        descriptionText.text = skill.description;
        cooldownText.text = $"재사용 대기시간: {skill.cooldown}초";
        manaText.text = $"MP: {skill.manaCost}";
    }
}