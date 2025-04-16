using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobAnimations : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    private JobManager.JobType currentJob;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (JobManager.Instance == null)
        {
            Debug.LogError("JobManager.Instance가 null입니다! JobManager가 씬에 있는지 확인하세요.");
            return;
        }

        currentJob = JobManager.Instance.GetCurrentJob(); //  Awake()에서 설정
        Debug.Log($"JobAnimations Awake - 현재 직업: {currentJob}");
    }

    //이동 애니메이션 실행
    public void PlayMovementAnimation(float speed)
    {
        //Debug.Log("PlayMovementAnimation() 호출됨. Speed: " + speed);

        if (JobManager.Instance == null) return;

        if (currentJob == JobManager.JobType.Basic)
        {
            animator.SetFloat("Speed_Basic", speed);
            //Debug.Log($"Basic 직업 - Speed_Basic 값 설정: {speed}");
        }
        else if (currentJob == JobManager.JobType.Warrior)
        {
            animator.SetFloat("Speed_Warrior", speed);
            Debug.Log($"Warrior 직업 - Speed_Warrior 값 설정: {speed}");
        }
        else if (JobManager.Instance.GetCurrentJob() == JobManager.JobType.Mage)
        {
            animator.SetFloat("Speed_Mage", speed);
            Debug.Log($"Mage 직업 - Speed_Mage 값 설정: {speed}");
        }
        else if (JobManager.Instance.GetCurrentJob() == JobManager.JobType.Archer)
        {
            animator.SetFloat("Speed_Archer", speed);
            Debug.Log($"Archer 직업 - Speed_Archer 값 설정: {speed}");
        }
        else
        {
            Debug.LogError($"알 수 없는 직업: {JobManager.Instance.GetCurrentJob()}");
        }
    }

    // Idle 상태로 변경
    public void StopMovementAnimation()
    {
        PlayMovementAnimation(0); // 이동 속도를 0으로 설정하여 Idle 애니메이션 실행
    }
}
