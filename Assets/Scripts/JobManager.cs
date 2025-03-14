using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public enum JobType { Basic, Warrior, Mage, Archer } // 직업 유형 선언
    public JobType currentJob = JobType.Basic; // 기본 직업 설정

    public static JobManager Instance; // 싱글턴 패턴 적용 (한 개만 존재)

    void Awake()
    {
        // 싱글턴 패턴: 다른 스크립트에서 쉽게 접근 가능
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 현재 직업을 변경하는 함수
    public void ChangeJob(JobType newJob)
    {
        currentJob = newJob;
        Debug.Log("직업 변경됨: " + currentJob);
    }

    // 현재 직업을 반환하는 함수
    public JobType GetCurrentJob()
    {
        return currentJob;
    }
}
