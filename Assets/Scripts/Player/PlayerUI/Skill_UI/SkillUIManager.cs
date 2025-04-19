using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    [Header("스킬 데이터")]
    [SerializeField] private JobSkillData[] allJobSkills; // 직업별 스킬데이터들

    [Header("UI 연결")]
    [SerializeField] private Transform skillListArea;       // Skill_List_Area 오브젝트
    [SerializeField] private GameObject skillSlotPrefab;    // Skill_Slot_Area 프리팹

    private void Start()
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

        // 기존 스킬 슬롯 제거
        foreach (Transform child in skillListArea)
        {
            Destroy(child.gameObject);
        }

        SkillSlotUI firstSlot = null; // 첫 스킬 슬롯 추적

        // 슬롯 생성
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
                firstSlot = slotUI; // 가장 첫 번째 슬롯 저장
        }

        // 게임 시작 시 첫 스킬 자동 선택
        if (firstSlot != null)
            firstSlot.OnClickSlot();
    }
}