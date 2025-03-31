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

    private bool inputBuffered = false;
    private bool allowBufferedInput = false;
    private bool canExecuteImmediately = false;
    private bool inputLocked = false; // 광클 방지용


    void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();
        movement = GetComponent<PlayerMovement>();
    }


    public void TryComboAttack() // 애니메이션 이벤트로 호출됨
    {
        if (canCombo && inputRegistered)
        {
            comboIndex++;
            animator.SetTrigger("NextCombo");
            movement.RotateToMouse();

            canCombo = false;      // 입력 처리 후 다시 막기
            inputRegistered = false;

            Debug.Log("다음 콤보 공격 실행!");
        }
        else
        {
            Debug.Log("콤보 입력 없음, 다음 공격 실행 안함");
        }
    }

    // InputHandler에서 호출되는 함수
    public void HandleAttackInput()
    {
        if (!stateMachine.CanAttack()) return;
        if (inputLocked) return; // 입력 락 걸렸으면 무시

        if (isAttacking)
        {
            if (allowBufferedInput && !canExecuteImmediately)
            {
                inputBuffered = true;
                Debug.Log("입력 감지됨 (버퍼 저장)");
            }
            else if (canExecuteImmediately)
            {
                ExecuteNextCombo();
            }
            else
            {
                Debug.Log("입력 무시됨 (콤보 타이밍 아님)");
            }
        }
        else
        {
            // 첫 공격
            isAttacking = true;
            comboIndex = 1;
            inputLocked = true; // 잠금 시작

            movement.StopAgent();
            movement.RotateToMouse();

            stateMachine.ChangeState(PlayerState.Attacking);
            animator.SetTrigger("NextCombo");

            Debug.Log("첫 번째 공격 실행");
        }
    }

    private void ExecuteNextCombo()
    {
        if (comboIndex >= 4) return;

        comboIndex++;
        animator.SetTrigger("NextCombo");
        movement.RotateToMouse();

        inputBuffered = false;
        allowBufferedInput = false;
        canExecuteImmediately = false;
        inputLocked = true; // 다시 잠금

        Debug.Log($"콤보 {comboIndex}번째 실행됨");
    }

    // 애니메이션 이벤트로 호출될 함수
    public void EnableComboInput()
    {
        inputBuffered = false;
        allowBufferedInput = true;
        canExecuteImmediately = false;
        inputLocked = false; // 다시 입력 가능해짐
        Debug.Log("콤보 입력 허용 시작");
    }

    public void CanAttack()
    {
        canExecuteImmediately = true;

        if (inputBuffered)
        {
            ExecuteNextCombo();
            Debug.Log("버퍼된 입력으로 콤보 실행");
        }
        else
        {
            Debug.Log("CanAttack 호출됨 - 아직 입력 없음");
        }
    }

    public void DisableComboInput()
    {
        allowBufferedInput = false;
        canExecuteImmediately = false;
        Debug.Log("콤보 입력 종료");
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
            inputBuffered = false;
            allowBufferedInput = false;
            canExecuteImmediately = false;
            inputLocked = false; //  중요: 입력 잠금 해제
            comboIndex = 0;

            animator.SetTrigger("endCombo");
            movement.ResumeAgent();

            
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
