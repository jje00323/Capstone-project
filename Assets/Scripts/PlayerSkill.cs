using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerSkill : MonoBehaviour
{
    private JobManager.JobType currentJob;
    private PlayerControls controls;
    private PlayerController playerController;
    private Camera mainCamera;

    private bool isDashing = false;

    public Animator Actionanimator;

    private bool bAttacking = false; // 공격 중인지 여부
    private bool bComboEnable = false; // 콤보 입력이 가능한지 여부
    private bool bComboExist = false; // 다음 공격이 예약되었는지 여부
    private float combonum = 0;
    public static PlayerSkill Instance;
    private Dictionary<string, GameObject> hitboxes = new Dictionary<string, GameObject>();
    GameObject hitbox;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.DashSkill.performed += ctx => TryDash();
        controls.Player.OnLeftClick.performed += ctx => UpdateAttacking();
        controls.Player.QWER.performed += TrySkills;

        playerController = GetComponent<PlayerController>();
        Actionanimator = GetComponent<Animator>();

        mainCamera = Camera.main;

        LoadHitboxes();
    }

    private void Start()
    {
        if (JobManager.Instance == null)
        {
            Debug.LogError("PlayerSkill: JobManager.Instance가 null입니다! JobManager가 씬에 있는지 확인하세요.");
            return;
        }

        currentJob = JobManager.Instance.GetCurrentJob();
    }

    private void OnEnable() { controls.Player.Enable(); }
    private void OnDisable() { controls.Player.Disable(); }

    private void UpdateAttacking()
    {


        if (bAttacking)
        {
            if (bComboEnable) // 콤보 가능 상태일 때만 추가 입력을 허용
            {
                bComboExist = true;
                //Debug.Log("추가 공격 입력 감지");
            }
            return;
        }

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {

        string key = $"{currentJob}_Attack";
        
        if (playerController != null)
        {
            playerController.StopMovement();
            playerController.StopAllMovementAnimations();
        }
        bAttacking = true;
        bComboExist = false;

        LookAtMouse();


        //  애니메이션 실행 (Base Layer → AttackState)
        Actionanimator.SetTrigger("NextCombo");
        StartCoroutine(ActivateHitboxAfterDelay(key, 0.3f));
        //Debug.Log("공격 시작");

        //  애니메이션이 30% 진행되면 다음 입력을 받을 수 있도록 설정
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f &&
            !Actionanimator.IsInTransition(0));

        bComboEnable = true; //  다음 공격 입력 가능
        //Debug.Log("콤보 입력 가능");


        //  애니메이션이 70% 진행되면 추가 입력을 감지하여 즉시 실행
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
            !Actionanimator.IsInTransition(0));

        if (bComboExist) //  추가 입력이 들어왔으면 즉시 다음 공격 실행
        {
            combonum++;
            //Debug.Log("추가 공격 실행");
            bComboEnable = false;
            StartCoroutine(PerformAttack());
            
        }
        else if(combonum == 3)
        {
            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                !Actionanimator.IsInTransition(0));
            playerController.StartMovement();
            Actionanimator.SetTrigger("endCombo");
        }
        else
        {
            //  공격이 끝나기 직전에 Idle 상태로 복귀
            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                !Actionanimator.IsInTransition(0));
            playerController.StartMovement();
            Actionanimator.SetTrigger("endCombo");


            EndAttack(); //  공격이 완전히 끝나기 전에 미리 종료 처리
        }
    }

    private void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0; //  Y축 회전 방지 (바닥만 보게 함)

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation; // 즉시 회전
            }
        }
    }

    private void EndAttack()
    {
        bAttacking = false;
        bComboEnable = false;
        //Debug.Log("공격 종료 → 즉시 다시 공격 가능");
        
    }


    public void Hit()
    {
        //Debug.Log(" Hit 이벤트 발생! (공격 판정 처리)");
        // 여기에서 실제 공격 판정을 구현하면 됨 (예: Raycast, Collider 검사 등)
    }

    private void TryDash()
    {
        DashSettings dashSettings = JobManager.Instance.GetDashSettings();

        if (dashSettings.IsReady())
        {
            dashSettings.UseDash(); //  대쉬 사용 시간 기록
            StartCoroutine(Dash(dashSettings));
        }
        else
        {
            //Debug.Log($"대쉬 사용 불가! 남은 쿨타임: {dashSettings.GetRemainingCooldown():F1}초");
        }
    }
    private IEnumerator Dash(DashSettings dashSettings)
    {
        if (isDashing) yield break;
        isDashing = true;

        if (playerController != null)
        {
            playerController.StopMovement();
            playerController.StopAllMovementAnimations();
        }

        Vector3 dashDirection;
        if (dashSettings.dashTowardMouse)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            dashDirection = (mouseWorldPosition - transform.position).normalized;
        }
        else
        {
            dashDirection = transform.forward;
        }

        if (dashSettings.dashTowardMouse && dashDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dashDirection);
        }

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + dashDirection * dashSettings.dashDistance;

        float elapsedTime = 0f;
        while (elapsedTime < dashSettings.dashDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, (dashSettings.dashDistance / dashSettings.dashDuration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isDashing = false;
        playerController.StartMovement();
    }

    private void TrySkills(InputAction.CallbackContext context)
    {
        if (playerController != null)
        {
            playerController.StopMovement();
            
        }
        LookAtMouse();

        string keyPressed = context.control.displayName; // 누른 키의 이름 가져오기
        Debug.Log($" TrySkills 실행됨! 입력된 키: {keyPressed}"); //  실행 로그 추가

        if (keyPressed == "Q")StartCoroutine(UseSkill("Q"));
        else if (keyPressed == "W") StartCoroutine(UseSkill("W"));
        else if (keyPressed == "E") StartCoroutine(UseSkill("E"));
        else if (keyPressed == "R") StartCoroutine(UseSkill("R"));
    }

    private IEnumerator UseSkill(string skillName)
    {
        

        string key = $"{currentJob}_{skillName}";
        string animationStartTrigger = $"Press_{skillName}";
        string animatioEndTrigger = $"end_skill";
        Debug.Log($" UseSkill 실행됨! 스킬 이름: {skillName}, 애니메이션: {animationStartTrigger}"); //  실행 로그 추가


        if (hitboxes.ContainsKey(key))
        {

            Debug.Log($" {skillName} 스킬 사용!");
            

            Actionanimator.SetTrigger(animationStartTrigger);
            StartCoroutine(ActivateHitboxAfterDelay(key, 0.3f));

            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f &&
                !Actionanimator.IsInTransition(0));

            yield return new WaitUntil(() =>
            {
                var animState = Actionanimator.GetCurrentAnimatorStateInfo(0);
                return animState.normalizedTime >= 0.8f && !Actionanimator.IsInTransition(0);
            });

            Actionanimator.SetTrigger(animatioEndTrigger);

        }
        else
        {
            Debug.LogError($" {key} 스킬이 없습니다! LoadHitboxes()에서 추가하세요.");
        }
        if (playerController != null)
        {
            playerController.StartMovement(); // 스킬 종료 후 이동 다시 활성화
        }
    }



    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return transform.position + transform.forward * 5f;
    }


    private void LoadHitboxes()
    {
        hitboxes["Basic_Attack"] = Resources.Load<GameObject>("Hitboxes/Basic_Attack");
        hitboxes["Basic_Q"] = Resources.Load<GameObject>("Hitboxes/Basic_Q");
        
    }

    private IEnumerator ActivateHitboxAfterDelay(string key, float delay)
    {
        yield return new WaitForSeconds(delay); // 애니메이션 공격 타이밍에 맞춰 지연

        Vector3 spawnPosition = transform.position + transform.forward * 1.5f;
        spawnPosition.y += 1.0f;

        hitbox = Instantiate(hitboxes[key], spawnPosition, Quaternion.identity);
        hitbox.SetActive(true);
    }

    // 히트박스 일정 시간 후 비활성화
  
}
