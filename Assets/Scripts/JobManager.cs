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


    void Awake()
    {
        // 싱글턴 패턴: 다른 스크립트에서 쉽게 접근 가능
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

    // 현재 직업을 변경하는 함수
    public void ChangeJob(JobType newJob)
    {
        currentJob = newJob;
        Debug.Log("직업 변경됨: " + currentJob);
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
}
