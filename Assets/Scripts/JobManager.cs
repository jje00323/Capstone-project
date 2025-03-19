using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

public class JobManager : MonoBehaviour
{
    public enum JobType { Basic, Warrior, Mage, Archer } // ���� ���� ����
    public JobType currentJob = JobType.Basic; // �⺻ ���� ����

    public static JobManager Instance { get; private set; }// �̱��� ���� ���� (�� ���� ����)

    private Dictionary<JobType, DashSettings> dashSettings;


    void Awake()
    {
        // �̱��� ����: �ٸ� ��ũ��Ʈ���� ���� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("JobManager �ν��Ͻ� ������!");
        }
        else
        {
            Debug.LogWarning("JobManager �ߺ� ����! ���� �ν��Ͻ��� �����մϴ�.");
            Destroy(gameObject);
        }

        dashSettings = new Dictionary<JobType, DashSettings>
        {
            { JobType.Basic, new DashSettings(5f, 0.2f, 3f, true) },    // �⺻ �뽬 (���콺 ����)
            { JobType.Warrior, new DashSettings(3f, 0.15f, 4f, false) }, // ����� ���� �뽬
            { JobType.Mage, new DashSettings(7f, 0.3f, 5f, true) },     // ������� �� �Ÿ� �뽬 (���콺 ����)
            { JobType.Archer, new DashSettings(6f, 0.25f, 6f, true) }   // �ü��� �߰� �Ÿ� �뽬 (���콺 ����)
        };
    }

    // ���� ������ �����ϴ� �Լ�
    public void ChangeJob(JobType newJob)
    {
        currentJob = newJob;
        Debug.Log("���� �����: " + currentJob);
    }

    public DashSettings GetDashSettings()
    {
        return dashSettings[currentJob];
    }

    // ���� ������ ��ȯ�ϴ� �Լ�
    public JobType GetCurrentJob()
    {
        return currentJob;
    }
}
