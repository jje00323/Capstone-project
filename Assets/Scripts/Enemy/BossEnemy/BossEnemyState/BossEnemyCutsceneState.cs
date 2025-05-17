using System.Collections;
using UnityEngine;
using Cinemachine;

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

    //public override void Enter()
    //{
    //    Debug.Log("[CutsceneState] 컷씬 시작");

    //    // Priority로 전환
    //    if (mainCam != null) mainCam.Priority = 10;
    //    if (cutsceneCam != null) cutsceneCam.Priority = 100;

    //    // LookAt 연결
    //    Transform lookAt = boss.transform.Find("LookAtTarget");
    //    if (cutsceneCam != null && lookAt != null)
    //    {
    //        cutsceneCam.LookAt = lookAt;
    //    }

    //    // DollyCart 이동 시작
    //    if (dollyCart != null)
    //    {
    //        dollyCart.m_Position = 0f;
    //        dollyCart.m_Speed = 1.5f;
    //    }

    //    boss.StartCoroutine(PlayCutscene());
    //}
    public override void Enter()
    {
        Debug.Log("[CutsceneState] 컷씬 시작");

        // 카메라 참조 유효성 확인
        Debug.Log($"[DEBUG] mainCam: {(mainCam != null ? mainCam.name : "null")}");
        Debug.Log($"[DEBUG] cutsceneCam: {(cutsceneCam != null ? cutsceneCam.name : "null")}");
        Debug.Log($"[DEBUG] dollyCart: {(dollyCart != null ? dollyCart.name : "null")}");

        // 1. Priority 전환 시도
        if (mainCam != null)
        {
            mainCam.Priority = 10;
            Debug.Log($"[DEBUG] mainCam.Priority 설정됨: {mainCam.Priority}");
        }

        if (cutsceneCam != null)
        {
            cutsceneCam.Priority = 100;
            Debug.Log($"[DEBUG] cutsceneCam.Priority 설정됨: {cutsceneCam.Priority}");
        }

        // 2. LookAtTarget 연결
        Transform lookAt = FindDeepChild(boss.transform, "LookAtTarget");
        if (cutsceneCam != null && lookAt != null)
        {
            cutsceneCam.LookAt = lookAt;
            Debug.Log($"[DEBUG] cutsceneCam.LookAt 설정됨 → {lookAt.name}");
        }
        else
        {
            Debug.LogWarning($"[DEBUG] LookAt 설정 실패 → LookAtTarget: {(lookAt != null ? "있음" : "null")}");
        }

        // 3. Dolly 시작
        if (dollyCart != null)
        {
            dollyCart.m_Position = 0f;
            dollyCart.m_Speed = 1.5f;
            Debug.Log($"[DEBUG] dollyCart 이동 시작: speed={dollyCart.m_Speed}");
        }

        boss.StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // 1. 플레이어 및 UI 비활성화
        GameObject player = GameObject.FindWithTag("Player");
        GameObject playerUI = GameObject.Find("Player_UI_Canvas");
        GameObject bossUI = GameObject.Find("Boss_HP_Bar_UI");

        if (player != null) player.SetActive(false);
        if (playerUI != null) playerUI.SetActive(false);
        if (bossUI != null) bossUI.SetActive(false);

        // 2. 애니메이션 트리거 + 속도 설정
        boss.Animator.speed = 0.6f;
        boss.Animator.SetTrigger("CutScene");

        // 3. 애니메이션 상태 진입 대기
        yield return new WaitUntil(() =>
            boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("CutScene"));

        Debug.Log("[CutsceneState] 애니메이션 CutScene 시작됨");

        // 4. 연출 시간 대기
        yield return new WaitForSeconds(4f);

        // 5. 애니메이션 속도 복원
        boss.Animator.speed = 1.0f;

        // 6. Dolly 정지
        if (dollyCart != null)
        {
            dollyCart.m_Speed = 0f;
        }

        // 7. 카메라 Priority 복구
        if (mainCam != null) mainCam.Priority = 100;
        if (cutsceneCam != null) cutsceneCam.Priority = 10;

        // 8. UI 및 플레이어 복구
        if (player != null) player.SetActive(true);
        if (playerUI != null) playerUI.SetActive(true);
        if (bossUI != null) bossUI.SetActive(true);

        Debug.Log("[CutsceneState] 컷씬 종료 → Idle 상태 복귀");
        boss.ChangeState(boss.idleState);
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

    public override void Exit()
    {
        if (dollyCart != null)
        {
            dollyCart.m_Speed = 0f;
        }
    }

    public override void Update() { }
}
