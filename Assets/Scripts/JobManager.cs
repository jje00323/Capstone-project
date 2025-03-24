using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

public class JobManager : MonoBehaviour
{
    public enum JobType { Basic, Warrior, Mage, Archer } // 직업 유형 선언
    public JobType currentJob = JobType.Basic; // 기본 직업 설정

    public static JobManager Instance { get; private set; }// 싱글턴 패턴 적용 (한 개만 존재)

    private Dictionary<JobType, DashSettings> dashSettings;
    public List<JobResources> jobResourcesList;
    private Dictionary<JobType, JobResources> jobResourcesDict;


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
        // 싱글턴 패턴: 다른 스크립트에서 쉽게 접근 가능
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeJobResources();
            Debug.Log("JobManager 인스턴스 생성됨!");
        }
        else
        {
            Debug.LogWarning("JobManager 중복 생성! 기존 인스턴스를 유지합니다.");
            Destroy(gameObject);
        }

        dashSettings = new Dictionary<JobType, DashSettings>
        {
            { JobType.Basic, new DashSettings(5f, 0.2f, 3f, true) },    // 기본 대쉬 (마우스 방향)
            { JobType.Warrior, new DashSettings(3f, 0.15f, 4f, false) }, // 전사는 전방 대쉬
            { JobType.Mage, new DashSettings(7f, 0.3f, 5f, true) },     // 마법사는 긴 거리 대쉬 (마우스 방향)
            { JobType.Archer, new DashSettings(6f, 0.25f, 6f, true) }   // 궁수는 중간 거리 대쉬 (마우스 방향)
        };
    }

    void Start()
    {
        // 시작 직후 현재 직업 적용
        ChangeJob(currentJob);
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

    // 현재 직업을 변경하는 함수
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

        // 2. 기존 모델 제거
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        // 2. 새 모델 Instantiate
        currentModel = Instantiate(res.modelPrefab, modelParent); // 또는 player.transform
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;

        // 3. Animator 설정은 모델이 생성된 후에
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.runtimeAnimatorController = res.animatorController;
            playerAnimator.avatar = res.avatar;
        }

        // 4. 연결 확인 (디버그)
        var hips = playerAnimator.GetBoneTransform(HumanBodyBones.Hips);
        Debug.Log("Hips 찾았는가? → " + (hips != null ? hips.name : "null"));

        Debug.Log($"[직업 변경 완료] {newJob}");
    }

    public DashSettings GetDashSettings()
    {
        return dashSettings[currentJob];
    }

    // 현재 직업을 반환하는 함수
    public JobType GetCurrentJob()
    {
        return currentJob;
    }

    public void ChangeToBasic()
    {
        JobManager.Instance.ChangeJob(JobManager.JobType.Basic);
    }
    public void ChangeToWarrior()
    {
        JobManager.Instance.ChangeJob(JobManager.JobType.Warrior);
    }

    public void ChangeToMage()
    {
        JobManager.Instance.ChangeJob(JobManager.JobType.Mage);
    }

    public void ChangeToArcher()
    {
        JobManager.Instance.ChangeJob(JobManager.JobType.Archer);
    }
}
