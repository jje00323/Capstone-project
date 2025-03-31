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

    private bool isCombat = false;
    private float combatTimer = 0f;
    private float combatDuration = 4f; // 8초 유지


    void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        CombatSystem();
    }


    public void CombatSystem()
    {
        if (isCombat && !isAttacking)
        {
            combatTimer += Time.deltaTime;

            if (combatTimer >= combatDuration)
            {
                isCombat = false;
                animator.SetBool("IsCombat", false);
                combatTimer = 0f;

                Debug.Log("전투 상태 종료됨: BasicMove 상태로 돌아감");
            }
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
            if (!isCombat)
            {
                isCombat = true;
                animator.SetBool("IsCombat", isCombat);
            }
            combatTimer = 0f; // 타이머 초기화
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

        combatTimer = 0f;
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
        // 현재 상태와 comboIndex가 일치하면 정상 종료
        inputRegistered = false;
        isAttacking = false;
        canCombo = false;
        inputBuffered = false;
        allowBufferedInput = false;
        canExecuteImmediately = false;
        inputLocked = false; //  중요: 입력 잠금 해제
        comboIndex = 0;

        //animator.SetTrigger("endCombo");
        movement.ResumeAgent();


        Debug.Log("공격 초기화");

        stateMachine.ChangeState(PlayerState.Idle);
    }
}
