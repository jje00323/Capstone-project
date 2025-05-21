using System.Collections;
using UnityEngine;
using Cinemachine;

public class BossEnemyIntroState : BossEnemyState
{
    private CinemachineVirtualCamera introCam;
    private CinemachineVirtualCamera mainCam;

    public BossEnemyIntroState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        Debug.Log("[IntroState] 보스 인트로 시작");

        InitializeIntroCameras();

        if (introCam == null || mainCam == null)
        {
            Debug.LogWarning("[IntroState] 인트로/메인 카메라가 없습니다.");
            boss.ChangeState(boss.idleState);
            return;
        }

        // Step 1: Fade Out → UI 숨기고 카메라 전환
        BossEnemyIntroController.Instance.FadeOut(0.5f, () =>
        {
            SetCameraPriority(introCam, 100);
            SetCameraPriority(mainCam, 10);

            BossEnemyIntroController.Instance.HideAll();

            // Step 2: Fade In → 애니메이션 실행
            BossEnemyIntroController.Instance.FadeIn(0.5f, () =>
            {
                boss.Animator.SetTrigger("Intro");

                // Step 3: 연출 끝난 후 상태 전환
                boss.StartCoroutine(WaitAndEnterIdle());
            });
        });
    }

    private void InitializeIntroCameras()
    {
        Transform trackRoot = GameObject.Find("BossIntroTrack(Clone)")?.transform;
        if (trackRoot != null)
            introCam = trackRoot.Find("BossIntroVCam")?.GetComponent<CinemachineVirtualCamera>();

        mainCam = GameObject.Find("MainVCam")?.GetComponent<CinemachineVirtualCamera>();
    }

    private IEnumerator WaitAndEnterIdle()
    {
        yield return new WaitForSeconds(4f); // 인트로 연출 시간

        // Step 4: Fade Out → 메인 카메라 복귀
        BossEnemyIntroController.Instance.FadeOut(0.5f, () =>
        {
            SetCameraPriority(introCam, 10);
            SetCameraPriority(mainCam, 100);

            // Step 5: Fade In → UI 복원 → Idle 상태 진입
            BossEnemyIntroController.Instance.FadeIn(0.5f, () =>
            {
                BossEnemyIntroController.Instance.ShowAll();
                boss.ChangeState(boss.idleState);
            });
        });
    }

    private void SetCameraPriority(CinemachineVirtualCamera cam, int priority)
    {
        if (cam != null) cam.Priority = priority;
    }

    public override void Update() { }

    public override void Exit() { }
}
