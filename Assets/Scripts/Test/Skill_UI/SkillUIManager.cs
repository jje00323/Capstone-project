using TMPro;
using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] private Transform skillListParent;
    [SerializeField] private GameObject skillSlotPrefab;
    [SerializeField] public JobSkillData skillData; // 모든 직업 스킬 데이터들

    private void Start()
    {
        JobManager.JobType currentJob = JobManager.Instance.GetCurrentJob();
        skillData = GetJobSkillData(skillData);

    }

    public void GetJobSkillData(JobSkillData data)
    {
        foreach (SkillInfo skill in data.skills)
        {
            GameObject btnObj = Instantiate(skillButtonPrefab, skillButtonParent);
            btnObj.SetActive(true);

            Image btnImage = btnObj.GetComponent<Image>();
            if (btnImage != null && skill.skillIcon != null)
                btnImage.sprite = skill.skillIcon;

            Image cooldownOverlay = btnObj.transform.Find("CooldownOverlay")?.GetComponent<Image>();
            TextMeshProUGUI cooldownText = btnObj.transform.Find("CooldownText")?.GetComponent<TextMeshProUGUI>();

            if (cooldownOverlay != null) cooldownOverlay.gameObject.SetActive(false);
            if (cooldownText != null) cooldownText.gameObject.SetActive(false);

            uiElements[skill.skillKey] = (cooldownOverlay, cooldownText);

            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                skillController?.TryUseSkill(skill.skillKey);
                StartUICooldown(skill.skillKey, skill.cooldown);
            });
        }
    }

    public void LoadSkillList(JobSkillData data)
    {
        foreach (Transform child in skillListParent)
            Destroy(child.gameObject);

        foreach (SkillInfo skill in data.skills)
        {
            GameObject slotObj = Instantiate(skillSlotPrefab, skillListParent);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
            slotUI.SetSlot(skill);
        }
    }
}