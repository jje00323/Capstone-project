using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

public class JobManager : MonoBehaviour
{
    public enum JobType { Basic, Warrior, Mage, Archer }
    public JobType currentJob = JobType.Basic;

    public static JobManager Instance { get; private set; }

    private Dictionary<JobType, DashSettings> dashSettings;
    public List<JobResources> jobResourcesList;
    private Dictionary<JobType, JobResources> jobResourcesDict;

    public JobSkillData[] allJobSkillData;
    private Dictionary<JobType, JobSkillData> skillDataDict;

    [System.Serializable]
    public class JobResources
    {
        public JobManager.JobType jobType;
        public RuntimeAnimatorController animatorController;
        public Avatar avatar;
        public GameObject modelPrefab;
    }

    [Header("모델이 붙을 위치")]
    public Transform modelParent;

    private GameObject currentModel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeJobResources();
            InitializeDashSettings();
            InitializeSkillData();
            Debug.Log("JobManager 인스턴스 생성됨!");
        }
        else
        {
            Debug.LogWarning("JobManager 중복 생성! 기존 인스턴스를 유지합니다.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ChangeJob(currentJob);
    }

    private void InitializeDashSettings()
    {
        dashSettings = new Dictionary<JobType, DashSettings>
        {
            { JobType.Basic, new DashSettings(5f, 0.2f, 3f, true) },
            { JobType.Warrior, new DashSettings(3f, 0.15f, 4f, false) },
            { JobType.Mage, new DashSettings(7f, 0.3f, 5f, true) },
            { JobType.Archer, new DashSettings(6f, 0.25f, 6f, true) }
        };
    }

    private void InitializeSkillData()
    {
        skillDataDict = new Dictionary<JobType, JobSkillData>();
        foreach (var data in allJobSkillData)
        {
            if (!skillDataDict.ContainsKey(data.jobType))
            {
                skillDataDict[data.jobType] = data;
            }
        }
    }

    public JobSkillData GetSkillData(JobType jobType)
    {
        return skillDataDict.TryGetValue(jobType, out var data) ? data : null;
    }

    private void InitializeJobResources()
    {
        jobResourcesDict = new Dictionary<JobType, JobResources>();
        foreach (var resource in jobResourcesList)
        {
            if (!jobResourcesDict.ContainsKey(resource.jobType))
            {
                jobResourcesDict[resource.jobType] = resource;
            }
        }
    }

    public void ChangeJob(JobType newJob)
    {
        currentJob = newJob;

        if (!jobResourcesDict.ContainsKey(newJob))
        {
            Debug.LogError("해당 직업 리소스 없음: " + newJob);
            return;
        }

        var res = jobResourcesDict[newJob];
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다.");
            return;
        }

        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        currentModel = Instantiate(res.modelPrefab, modelParent);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;

        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.runtimeAnimatorController = res.animatorController;
            playerAnimator.avatar = res.avatar;

            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.UpdateAnimatorReference(playerAnimator);
        }


        PlayerSkillController playerSkill = FindObjectOfType<PlayerSkillController>();
        if (playerSkill != null)
        {
            var skillData = GetSkillData(newJob);
            if (skillData != null)
            {
                playerSkill.LoadSkillsFromData(skillData);
            }
        }
        var stateMachine = player.GetComponent<PlayerStateMachine>();
        if (stateMachine != null)
        {
            stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
        }
        var attack = player.GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.ForceEndCombo(); 
        }
        // 2. 스킬 UI 갱신
        PlayerSkillUI skillUI = FindObjectOfType<PlayerSkillUI>();
        if (skillUI != null)
        {
            var skillData = GetSkillData(newJob);
            if (skillData != null)
            {
                skillUI.ReloadUI(skillData);
            }
        }
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.UpdateAnimatorReference(playerAnimator);

        var hips = playerAnimator.GetBoneTransform(HumanBodyBones.Hips);
        //Debug.Log("Hips 찾았는가? → " + (hips != null ? hips.name : "null"));

        
        
        Debug.Log($"[직업 변경 완료] {newJob}");
    }

    public DashSettings GetDashSettings()
    {
        return dashSettings[currentJob];
    }

    public JobType GetCurrentJob()
    {
        return currentJob;
    }

    public void ChangeToBasic() => ChangeJob(JobType.Basic);
    public void ChangeToWarrior() => ChangeJob(JobType.Warrior);
    public void ChangeToMage() => ChangeJob(JobType.Mage);
    public void ChangeToArcher() => ChangeJob(JobType.Archer);
}
