using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerSkill : MonoBehaviour
{

    private PlayerControls controls;
    private PlayerController playerController;
    private Camera mainCamera;

    private bool isDashing = false;

    public Animator Actionanimator;

    private bool bAttacking = false; // 공격 중인지 여부
    private bool bComboEnable = false; // 콤보 입력이 가능한지 여부
    private bool bComboExist = false; // 다음 공격이 예약되었는지 여부


    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.DashSkill.performed += ctx => TryDash();
        controls.Player.OnLeftClick.performed += ctx => UpdateAttacking();

        playerController = GetComponent<PlayerController>();
        Actionanimator = GetComponent<Animator>();

        mainCamera = Camera.main;
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
                Debug.Log("추가 공격 입력 감지");
            }
            return;
        }

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
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
        Debug.Log("공격 시작");

        //  애니메이션이 30% 진행되면 다음 입력을 받을 수 있도록 설정
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f &&
            !Actionanimator.IsInTransition(0));

        bComboEnable = true; //  다음 공격 입력 가능
        Debug.Log("콤보 입력 가능");

        //  애니메이션이 70% 진행되면 추가 입력을 감지하여 즉시 실행
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
            !Actionanimator.IsInTransition(0));

        if (bComboExist) //  추가 입력이 들어왔으면 즉시 다음 공격 실행
        {
            Debug.Log("추가 공격 실행");
            bComboEnable = false;
            StartCoroutine(PerformAttack());
        }
        else
        {
            //  공격이 끝나기 직전에 Idle 상태로 복귀
            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
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
        Debug.Log("공격 종료 → 즉시 다시 공격 가능");
        
    }


    public void Hit()
    {
        Debug.Log(" Hit 이벤트 발생! (공격 판정 처리)");
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
            Debug.Log($"대쉬 사용 불가! 남은 쿨타임: {dashSettings.GetRemainingCooldown():F1}초");
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
}
