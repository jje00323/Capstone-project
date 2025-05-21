using System.Collections;
using UnityEngine;
using Cinemachine;

public class BossEnemyCutsceneState : BossEnemyState
{
    private CinemachineVirtualCamera cutsceneCam;
    private CinemachineDollyCart dollyCart;
    private CinemachineVirtualCamera mainCam;

    public BossEnemyCutsceneState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        Debug.Log("[CutsceneState] 컷씬 시작");

        InitializeCutsceneObjects(); // ▶ 참조 시점 보장

        if (cutsceneCam != null)
            cutsceneCam.gameObject.SetActive(true); // ▶ 비활성화 대비

        BossEnemyCutsceneController.Instance.FadeOut(0.5f, () =>
        {
            TeleportBossToTransitionPoint();
            HideAllUI();

            SetCameraPriority(10, 100);  // MainVCam = 10, CutsceneVCam = 100
            SetLookAtTarget();

            if (dollyCart != null)
            {
                dollyCart.m_Position = 0f;
                dollyCart.m_Speed = 3f; // 필요 시 1.5f로 조정
            }

            BossEnemyCutsceneController.Instance.FadeIn(0.5f, () =>
            {
                boss.Animator.speed = 0.6f;
                boss.Animator.SetTrigger("CutScene");

                boss.StartCoroutine(PlayCutscene());
            });
        });
    }

    private void InitializeCutsceneObjects()
    {
        // 씬에서 동적으로 생성된 오브젝트 이름 기준
        Transform trackRoot = GameObject.Find("BossCutSceneTrack(Clone)")?.transform;
        if (trackRoot != null)
        {
            cutsceneCam = trackRoot.Find("BossCutsceneVCam")?.GetComponent<CinemachineVirtualCamera>();
            dollyCart = trackRoot.Find("Dolly Cart")?.GetComponent<CinemachineDollyCart>();
        }

        mainCam = GameObject.Find("MainVCam")?.GetComponent<CinemachineVirtualCamera>();
    }

    private IEnumerator PlayCutscene()
    {
        // 컷씬 애니메이션 시작까지 대기
        yield return new WaitUntil(() =>
            boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("CutScene"));
        Debug.Log("[CutsceneState] 애니메이션 CutScene 시작됨");

        // 컷씬 애니메이션 재생 시간 대기
        yield return new WaitForSeconds(boss.bossData.cutsceneDuration);

        // 페이드 아웃 → 카메라 전환
        BossEnemyCutsceneController.Instance.FadeOut(0.5f, () =>
        {
            SetCameraPriority(100, 10); // CutsceneCam → MainCam
            if (dollyCart != null) dollyCart.m_Speed = 0f;

            ShowAllUI();

            // 페이드 인 완료 후 1초 대기하고 Idle 상태 진입
            BossEnemyCutsceneController.Instance.FadeIn(0.5f, () =>
            {
                boss.StartCoroutine(WaitAndEnterIdle());
            });
        });
    }
    private IEnumerator WaitAndEnterIdle()
    {
        yield return new WaitForSeconds(1f); // 카메라 전환 완료 후 추가 대기 시간

        boss.cutsceneTriggered = false;
        boss.Animator.speed = 1.0f;
        boss.ChangeState(boss.idleState);
    }

    private void TeleportBossToTransitionPoint()
    {
        Transform trans = GameObject.Find("BossTransition")?.transform;
        if (trans != null)
        {
            boss.transform.position = trans.position;
            boss.transform.rotation = trans.rotation;
            Debug.Log("[CutsceneState] 보스 위치 전환 완료");
        }
        else
        {
            Debug.LogWarning("[CutsceneState] BossTransition 오브젝트를 찾을 수 없습니다.");
        }
    }

    private void SetCameraPriority(int mainPriority, int cutscenePriority)
    {
        if (mainCam != null) mainCam.Priority = mainPriority;
        if (cutsceneCam != null) cutsceneCam.Priority = cutscenePriority;
    }

    private void SetLookAtTarget()
    {
        Transform lookAt = FindDeepChild(boss.transform, "LookAtTarget");
        if (cutsceneCam != null && lookAt != null)
            cutsceneCam.LookAt = lookAt;
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
                return child;
        }
        return null;
    }

    private void HideAllUI()
    {
        BossEnemyCutsceneController.Instance.HideAll();
    }

    private void ShowAllUI()
    {
        BossEnemyCutsceneController.Instance.ShowAll();
    }

    public override void Exit()
    {
        if (dollyCart != null)
            dollyCart.m_Speed = 0f;
    }

    public override void Update() { }
}
