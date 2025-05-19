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
        if (JobManager.Instance != null && JobManager.Instance.GetSkillData(JobManager.Instance.GetCurrentJob()) != null)
        {
            StartCoroutine(WaitUntilSkillDataReadyThenLoad());
        }
        else
        {
            Debug.LogWarning("[SkillUIManager] JobManager가 초기화되지 않았거나 skillData가 없음. 로딩 생략");
        } // 슬롯 생성 & 스킬 포인트 텍스트 갱신
        
    }

    private void OnJobChanged(JobManager.JobType newJob)
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[SkillUIManager] GameObject가 꺼져 있어 스킬 UI 로딩 생략됨");
            return;
        }

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

            SkillInfo displaySkill = SkillUpgradeManager.Instance.GetCurrentVersionOf(skill);

            GameObject slotObj = Instantiate(skillSlotPrefab, skillListArea);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
            
            if (slotUI == null)
            {
                Debug.LogError("[SkillUIManager] SkillSlotUI 컴포넌트가 없습니다.");
                continue;
            }

            slotUI.SetSlot(displaySkill);

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

    private IEnumerator WaitUntilSkillDataReadyThenLoad()
    {
        // 최대 2~3 프레임까지 대기 (필요시 시간 제한도 추가 가능)
        for (int i = 0; i < 5; i++)
        {
            if (JobManager.Instance != null &&
                SkillUpgradeManager.Instance != null &&
                JobManager.Instance.GetSkillData(JobManager.Instance.GetCurrentJob()) != null)
            {
                LoadSkillListForCurrentJob();
                UpdateSkillPointText();
                yield break;
            }

            yield return null; // 한 프레임 대기
            yield return null; // 한 프레임 대기
            yield return null; // 한 프레임 대기
            yield return null; // 한 프레임 대기

        }

        Debug.LogWarning("[SkillUIManager] 스킬 데이터가 준비되지 않아 초기화를 건너뜁니다.");
    }

}