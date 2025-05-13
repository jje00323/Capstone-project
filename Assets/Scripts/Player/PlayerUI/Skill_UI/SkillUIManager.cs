using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    [Header("스킬 데이터")]
    [SerializeField] private JobSkillData[] allJobSkills;

    [Header("UI 연결")]
    [SerializeField] private Transform skillListArea;
    [SerializeField] private GameObject skillSlotPrefab;

    private void Awake()
    {
        if (JobManager.Instance != null)
            JobManager.Instance.OnJobChanged += OnJobChanged;
    }

    private void OnDestroy()
    {
        if (JobManager.Instance != null)
            JobManager.Instance.OnJobChanged -= OnJobChanged;
    }

    private void OnEnable()
    {
        LoadSkillListForCurrentJob(); // UI 열릴 때 항상 갱신
    }

    private void OnJobChanged(JobManager.JobType newJob)
    {
        LoadSkillListForCurrentJob();
    }

    public void LoadSkillListForCurrentJob()
    {
        var currentJob = JobManager.Instance.GetCurrentJob();

        JobSkillData jobData = null;
        foreach (var data in allJobSkills)
        {
            if (data.jobType == currentJob)
            {
                jobData = data;
                break;
            }
        }

        if (jobData == null)
        {
            Debug.LogWarning("[SkillUIManager] 해당 직업의 스킬 데이터를 찾을 수 없습니다.");
            return;
        }

        foreach (Transform child in skillListArea)
        {
            Destroy(child.gameObject);
        }

        SkillSlotUI firstSlot = null;

        foreach (var skill in jobData.skills)
        {
            GameObject slotObj = Instantiate(skillSlotPrefab, skillListArea);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();

            if (slotUI == null)
            {
                Debug.LogError("[SkillUIManager] SkillSlotUI 컴포넌트가 없습니다.");
                continue;
            }

            slotUI.SetSlot(skill);

            if (firstSlot == null)
                firstSlot = slotUI;
        }

        if (firstSlot != null)
            firstSlot.OnClickSlot();

        SkillUpgradeManager.Instance.AutoLinkUpgradeToBase(allJobSkills);
    }
}