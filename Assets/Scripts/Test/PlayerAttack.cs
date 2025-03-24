using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlayerStateMachine;
[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private PlayerStateMachine stateMachine;

    private int comboIndex = 0;
    private bool isAttacking = false;
    private bool canCombo = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();
    }


    public void HandleAttack()
    {
        if (!stateMachine.CanAttack())
            return;

        if (isAttacking && canCombo)
        {
            // 콤보 타이밍에 다시 클릭한 경우
            canCombo = false;
            comboIndex++;
            animator.SetTrigger("NextCombo");
        }
        else if (!isAttacking)
        {
            // 첫 공격 시작
            isAttacking = true;
            comboIndex = 1;
            stateMachine.ChangeState(PlayerState.Attacking);
            animator.SetTrigger("NextCombo");
        }
    }

    // 애니메이션 이벤트로 호출될 함수
    public void EnableComboInput()
    {
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

        isAttacking = false;
        canCombo = false;
        comboIndex = 0;

        animator.ResetTrigger("NextCombo");  // 혹시라도 잔여 트리거 제거
        animator.SetTrigger("endCombo");     // Animator 트리거 기반으로 상태 전이
        Debug.Log($"공격 초기화");

        stateMachine.ChangeState(PlayerState.Idle);
    }
}
