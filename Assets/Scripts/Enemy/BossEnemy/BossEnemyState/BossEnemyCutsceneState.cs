using System.Collections;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class BossEnemyCutsceneState : BossEnemyState
{
    private CinemachineVirtualCamera cutsceneCam;
    private CinemachineDollyCart dollyCart;
    private CinemachineVirtualCamera mainCam;



    public BossEnemyCutsceneState(BossEnemyFSM boss) : base(boss)
    {
        // 상위 오브젝트 기준으로 자식 오브젝트 접근
        Transform trackRoot = GameObject.Find("BossCutSceneTrack")?.transform;

        if (trackRoot != null)
        {
            cutsceneCam = trackRoot.Find("BossCutsceneVCam")?.GetComponent<CinemachineVirtualCamera>();
            dollyCart = trackRoot.Find("Dolly Cart")?.GetComponent<CinemachineDollyCart>();
        }

        mainCam = GameObject.Find("MainVCam")?.GetComponent<CinemachineVirtualCamera>();
    }

    public override void Enter()
    {
        Debug.Log("[CutsceneState] 컷씬 시작");

        BossEnemyCutsceneController.Instance.FadeOut(0.5f, () =>
        {
            TeleportBossToTransitionPoint();
            HideAllUI();

            SetCameraPriority(10, 100);  // MainCam, CutsceneCam
            SetLookAtTarget();

            if (dollyCart != null)
            {
                dollyCart.m_Position = 0f;
                dollyCart.m_Speed = 1.5f;
            }

            BossEnemyCutsceneController.Instance.FadeIn(0.5f, () =>
            {
                boss.Animator.speed = 0.6f;
                boss.Animator.SetTrigger("CutScene");

                boss.StartCoroutine(PlayCutscene());
            });
        });
    }

    private IEnumerator PlayCutscene()
    {
        yield return new WaitUntil(() => boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("CutScene"));

        Debug.Log("[CutsceneState] 애니메이션 CutScene 시작됨");
        yield return new WaitForSeconds(boss.bossData.cutsceneDuration);

        BossEnemyCutsceneController.Instance.FadeOut(0.5f, () =>
        {
            SetCameraPriority(100, 10); // 복구
            if (dollyCart != null) dollyCart.m_Speed = 0f;

            ShowAllUI();

            BossEnemyCutsceneController.Instance.FadeIn(0.5f, () =>
            {
                boss.StartCoroutine(EndAfterDelay());
            });
        });
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
    private IEnumerator EndAfterDelay()
    {
        yield return new WaitForSeconds(1f);
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
        {
            cutsceneCam.LookAt = lookAt;
        }
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
        {
            dollyCart.m_Speed = 0f;
        }
    }

    public override void Update() { }
}
