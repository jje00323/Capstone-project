using System.Collections;
using UnityEngine;
using Cinemachine;

public class BossEnemyIntroState : BossEnemyState
{
    private CinemachineVirtualCamera introCam;
    private CinemachineVirtualCamera mainCam;
    private Coroutine introRoutine;

    public BossEnemyIntroState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        var rb = boss.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
        }

        boss.Animator.applyRootMotion = false;
        InitializeIntroCameras();

        if (introCam == null || mainCam == null)
        {
            boss.ChangeState(boss.idleState);
            return;
        }
        SetLookAtTarget();

        BossEnemyIntroController.Instance.FadeOut(0.5f, () =>
        {
            SetCameraPriority(introCam, 100);
            SetCameraPriority(mainCam, 10);
            BossEnemyIntroController.Instance.HideAll();

            

            BossEnemyIntroController.Instance.FadeIn(0.5f, () =>
            {
                var bgm = GameObject.FindObjectOfType<BGMManager>();
                if (bgm != null && boss.bossData != null && boss.bossData.bossThemeMusic != null)
                {
                    bgm.PlayBGM(boss.bossData.bossThemeMusic);
                    Debug.Log("[IntroState] 보스 인트로 BGM 재생됨");
                }

                boss.Animator.SetTrigger("Intro");
                introRoutine = boss.StartCoroutine(CheckLandingAndTransition());
            });
        });
    }

    private void InitializeIntroCameras()
    {
        var trackGO = GameObject.Find("BossIntroTrack(Clone)");
        introCam = trackGO?.transform.Find("BossIntroVCam")?.GetComponent<CinemachineVirtualCamera>();
        mainCam = GameObject.Find("MainVCam")?.GetComponent<CinemachineVirtualCamera>();
    }

    private IEnumerator CheckLandingAndTransition()
    {
        var rb = boss.GetComponent<Rigidbody>();

        while (true)
        {
            float y = boss.transform.position.y;
            float vy = rb != null ? rb.velocity.y : 0f;
            if (y <= 0.5f && Mathf.Abs(vy) < 0.05f)
                break;
            yield return null;
        }

        yield return new WaitUntil(() => boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("JumpLoop"));
        boss.Animator.SetBool("isLanding", true);

        yield return new WaitUntil(() => boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("JumpEnd"));
        float jumpEndLength = boss.Animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(jumpEndLength);

        BossEnemyIntroController.Instance.FadeOut(0.5f, () =>
        {
            SetCameraPriority(introCam, 10);
            SetCameraPriority(mainCam, 100);

            BossEnemyIntroController.Instance.FadeIn(0.5f, () =>
            {
                BossEnemyIntroController.Instance.ShowAll();

                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    rb.velocity = Vector3.zero;
                }

                boss.Animator.applyRootMotion = true;
                boss.ChangeState(boss.idleState);
            });
        });
    }

    private void SetCameraPriority(CinemachineVirtualCamera cam, int priority)
    {
        if (cam != null) cam.Priority = priority;
    }
    private void SetLookAtTarget()
    {
        Transform lookAt = FindDeepChild(boss.transform, "LookAtTarget");
        if (introCam != null && lookAt != null)
            introCam.LookAt = lookAt;
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

    public override void Update() { }

    public override void Exit()
    {
        if (introRoutine != null)
        {
            boss.StopCoroutine(introRoutine);
            introRoutine = null;
        }
        boss.Animator.SetBool("isLanding", false);
    }
}