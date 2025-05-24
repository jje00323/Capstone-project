using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class JobManager : MonoBehaviour
{
    public enum JobType { Basic, Warrior, Mage, Paladine, Assasine, Babarian }
    public JobType currentJob = JobType.Basic;

    public static JobManager Instance { get; private set; }

    private Dictionary<JobType, DashSettings> dashSettings;
    public List<JobResources> jobResourcesList;
    private Dictionary<JobType, JobResources> jobResourcesDict;

    public JobSkillData[] allJobSkillData;
    private Dictionary<JobType, JobSkillData> skillDataDict;

    public event System.Action<JobType> OnJobChanged;

    [System.Serializable]
    public class JobResources
    {
        public JobManager.JobType jobType;
        public RuntimeAnimatorController animatorController;
        public Avatar avatar;
        public GameObject modelPrefab;
    }

    [Header("모델이 붙을 위치 (Player 하위 ModelRoot를 자동으로 찾습니다)")]
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
            InitializeStatData();

            Debug.Log("JobManager 인스턴스 생성됨!");
        }
        else
        {
            Debug.LogWarning("JobManager 중복 생성! 기존 인스턴스를 유지합니다.");
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        yield return null;

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null && player.TryGetComponent(out PlayerStatus ps))
        {
            if (ps.stateUI == null)
            {
                ps.stateUI = FindObjectOfType<PlayerStateUI>();
                if (ps.stateUI != null)
                {
                    Debug.Log("[JobManager] Start에서 stateUI 강제 연결 완료");
                    ps.UpdateAllUI();
                }
            }
        }

        //ChangeJob(currentJob);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeDashSettings()
    {
        dashSettings = new Dictionary<JobType, DashSettings>
        {
            { JobType.Basic, new DashSettings(5f, 0.2f, 3f, true) },
            { JobType.Warrior, new DashSettings(3f, 0.15f, 4f, false) },
            { JobType.Mage, new DashSettings(7f, 0.3f, 5f, true) },
        };
    }

    private void InitializeSkillData()
    {
        skillDataDict = new Dictionary<JobType, JobSkillData>();
        foreach (var data in allJobSkillData)
        {
            if (!skillDataDict.ContainsKey(data.jobType))
                skillDataDict[data.jobType] = data;
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
                jobResourcesDict[resource.jobType] = resource;
        }
    }

    private void InitializeStatData()
    {
        jobStatDict = new Dictionary<JobType, JobStatusData>();
        foreach (var data in allJobStatData)
        {
            if (!jobStatDict.ContainsKey(data.jobType))
                jobStatDict[data.jobType] = data;
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

        // 모델 부착 위치 자동 설정
        if (modelParent == null)
        {
            var root = player.transform.Find("ModelRoot");
            if (root != null)
            {
                modelParent = root;
                Debug.Log("[JobManager] Player 내부의 'ModelRoot'를 modelParent로 설정함.");
            }
            else
            {
                modelParent = player.transform;
                Debug.LogWarning("[JobManager] 'ModelRoot'가 없어 Player 본체에 모델을 부착함.");
            }
        }

        // 기존 모델 제거
        if (currentModel != null)
            Destroy(currentModel);

        // 모델 생성
        currentModel = Instantiate(res.modelPrefab, modelParent);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;

        // 애니메이터 설정
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.runtimeAnimatorController = res.animatorController;
            playerAnimator.avatar = res.avatar;

            if (player.TryGetComponent(out PlayerMovement movement))
                movement.UpdateAnimatorReference(playerAnimator);
        }

        // 스킬 데이터 적용
        if (FindObjectOfType<PlayerSkillController>() is PlayerSkillController playerSkill)
        {
            if (GetSkillData(newJob) is JobSkillData skillData)
                playerSkill.LoadSkillsFromData(skillData);
        }

        // 상태기계 초기화
        if (player.TryGetComponent(out PlayerStateMachine stateMachine))
            stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);

        // 스탯 적용
        var statData = GetStatData(newJob);
        if (player.TryGetComponent(out PlayerStatus playerStatus) && statData != null)
        {
            if (playerStatus.stateUI == null)
            {
                playerStatus.stateUI = FindObjectOfType<PlayerStateUI>();
                if (playerStatus.stateUI != null)
                    Debug.Log("[JobManager] stateUI를 강제로 연결했습니다.");
                else
                    Debug.LogWarning("[JobManager] PlayerStateUI를 찾을 수 없습니다.");
            }

            playerStatus.ApplyJobStats(statData);
            playerStatus.SetJob(newJob);
            playerStatus.UpdateAllUI();
        }
        else
        {
            Debug.LogWarning("[JobManager] PlayerStatus 또는 스탯 데이터를 찾지 못함: " + newJob);
        }

        // 이벤트 및 애니메이터 재설정
        if (player.TryGetComponent(out PlayerMovement playerMovement))
            playerMovement.UpdateAnimatorReference(playerAnimator);

        Debug.Log($"[직업 변경 완료] {newJob}");
        StartCoroutine(InvokeJobChangedDelayed(newJob));
    }

    private IEnumerator InvokeJobChangedDelayed(JobType newJob)
    {
        yield return null;
        OnJobChanged?.Invoke(newJob);
    }

    public DashSettings GetDashSettings() => dashSettings[currentJob];
    public JobType GetCurrentJob() => currentJob;

    public void ChangeToBasic() => ChangeJob(JobType.Basic);
    public void ChangeToWarrior() => ChangeJob(JobType.Warrior);
    public void ChangeToMage() => ChangeJob(JobType.Mage);
    public void ChangeToArcher() => ChangeJob(JobType.Paladine);

    public JobStatusData[] allJobStatData;
    private Dictionary<JobType, JobStatusData> jobStatDict;

    public JobStatusData GetStatData(JobType jobType)
    {
        return jobStatDict.TryGetValue(jobType, out var data) ? data : null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[JobManager] 씬 로드됨: {scene.name} → 오브젝트 재연결 시도");

        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("[JobManager] Player를 씬에서 찾지 못했습니다.");
            return;
        }

        modelParent = player.transform.Find("ModelRoot") ?? player.transform;

        Debug.Log("[JobManager] modelParent 재연결 완료");

        // PlayerStatus UI 강제 연결
        if (player.TryGetComponent(out PlayerStatus ps))
        {
            if (ps.stateUI == null)
            {
                ps.stateUI = FindObjectOfType<PlayerStateUI>();
                if (ps.stateUI != null)
                {
                    Debug.Log("[JobManager] PlayerStateUI 자동 재연결");
                    ps.UpdateAllUI();
                }
            }
        }

        // 직업 상태 재적용
        ChangeJob(currentJob);
    }
}