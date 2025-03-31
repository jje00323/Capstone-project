using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlayerStateMachine;
[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement movement;

    private Animator animator;
    private PlayerStateMachine stateMachine;

    private int comboIndex = 0;
    private bool isAttacking = false;
    private bool canCombo = false;
    private bool inputRegistered = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();
        movement = GetComponent<PlayerMovement>();
    }


    public void HandleAttack()
    {
        if (!stateMachine.CanAttack())
            return;

        if (isAttacking && canCombo && !inputRegistered)
        {
            // 콤보 타이밍에 다시 클릭한 경우
            inputRegistered = true;
            canCombo = false;

            movement.RotateToMouse();
            comboIndex++;

            //animator.ResetTrigger("NextCombo");
            animator.SetTrigger("NextCombo");
        }
        else if (!isAttacking)
        {
            movement.StopAgent();
            movement.RotateToMouse();
            // 첫 공격 시작
            isAttacking = true;
            comboIndex = 1;
            stateMachine.ChangeState(PlayerState.Attacking);
            //animator.ResetTrigger("NextCombo");
            animator.SetTrigger("NextCombo");
        }
    }

    // 애니메이션 이벤트로 호출될 함수
    public void EnableComboInput()
    {
        inputRegistered = false;
        canCombo = true;
        Debug.Log($"콤보 가능 -> 현재 콤보: {comboIndex}");
    }

    public void DisableComboInput()
    {
        canCombo = false;
        Debug.Log("추가 콤보 입력 차단");
    }

    public void EndCombo()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("BasicAttack_1") && comboIndex == 1 ||
            stateInfo.IsName("BasicAttack_2") && comboIndex == 2 ||
            stateInfo.IsName("BasicAttack_3") && comboIndex == 3 ||
            comboIndex >= 4)
        {
            // 현재 상태와 comboIndex가 일치하면 정상 종료
            inputRegistered = false;
            isAttacking = false;
            canCombo = false;
            comboIndex = 0;

            movement.ResumeAgent();

            animator.SetTrigger("endCombo");
            Debug.Log("공격 초기화");

            stateMachine.ChangeState(PlayerState.Idle);
        }
        else
        {
            // 다음 콤보로 이미 넘어간 경우 EndCombo 무시
            Debug.Log("EndCombo 무시됨 - 이미 다음 콤보 진행 중");
        }
    }
}
