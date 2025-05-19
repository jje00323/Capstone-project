using TMPro;
using UnityEngine;
using System.Collections;
public class SkillUIManager : MonoBehaviour
{
    [Header("스킬 데이터")]
    [SerializeField] private JobSkillData[] allJobSkills;

    [Header("UI 연결")]
    [SerializeField] private Transform skillListArea;
    [SerializeField] private GameObject skillSlotPrefab;
    [SerializeField] private TextMeshProUGUI skillPointText;


    public static SkillUIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

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
        LoadSkillListForCurrentJob();  // 슬롯 생성 & 스킬 포인트 텍스트 갱신
        UpdateSkillPointText();
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
            StartCoroutine(SelectFirstSlotAfterUIReady(firstSlot));

        SkillUpgradeManager.Instance.AutoLinkUpgradeToBase(allJobSkills);
    }

    public void UpdateSkillPointText()
    {
        int current = SkillPointManager.Instance.GetPoints();
        Debug.Log($"[UI] 현재 스킬 포인트: {current}");
        skillPointText.text = $"스킬 포인트: {current}";
    }

    private IEnumerator SelectFirstSlotAfterUIReady(SkillSlotUI firstSlot)
    {
        yield return null;
        yield return null;


        if (firstSlot == null)
        {
            Debug.LogWarning("[SkillUIManager] firstSlot이 null입니다.");
            yield break;
        }

        if (SkillUpgradeUI.Instance == null)
        {
            Debug.LogWarning("[SkillUIManager] SkillUpgradeUI.Instance가 null입니다. 첫 스킬 선택 생략.");
            yield break;
        }

        firstSlot.OnClickSlot();
    }

    private IEnumerator DelayedInitializeUI()
    {
        yield return null;
        yield return null;
        LoadSkillListForCurrentJob();
        UpdateSkillPointText();
    }

    public void InitializeSkillDataOnly()
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

        // UI는 생성하지 않고, 데이터만 준비
        SkillUpgradeManager.Instance.AutoLinkUpgradeToBase(allJobSkills);
    }

}