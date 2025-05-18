using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlayerStateMachine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement movement;
    private Animator animator;
    private PlayerStateMachine stateMachine;

    private int comboIndex = 0;
    private bool isAttacking = false;

    private bool inputBuffered = false;
    private bool allowBufferedInput = false;
    private bool canExecuteImmediately = false;
    private bool inputLocked = false; // 광클 방지용

    private bool isCombat = false;
    private float combatTimer = 0f;
    private float combatDuration = 6f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        CombatSystem();
        if (EventSystem.current.IsPointerOverGameObject()) return;
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

               // Debug.Log("전투 상태 종료됨: BasicMove 상태로 돌아감");
            }
        }
    }

    public void HandleAttackInput()
    {
        if (!stateMachine.CanAttack()) return;
        if (inputLocked) return;

        if (isAttacking)
        {
            if (allowBufferedInput && !canExecuteImmediately)
            {
                inputBuffered = true;
                //Debug.Log("입력 감지됨 (버퍼 저장)");
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
            combatTimer = 0f;

            isAttacking = true;
            comboIndex = 1;
            inputLocked = true;

            movement.StopAgent();
            movement.RotateToMouse();

            animator.applyRootMotion = true;

            stateMachine.ChangeState(PlayerState.Attacking);
            animator.SetTrigger("NextCombo");

            //Debug.Log("첫 번째 공격 실행");
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
        inputLocked = true;

        //Debug.Log($"콤보 {comboIndex}번째 실행됨");
    }

    public void EnableComboInput()
    {
        inputBuffered = false;
        allowBufferedInput = true;
        canExecuteImmediately = false;
        inputLocked = false;
        //Debug.Log("콤보 입력 허용 시작");
    }

    public void CanAttack()
    {
        canExecuteImmediately = true;

        if (inputBuffered)
        {
            ExecuteNextCombo();
            //Debug.Log("버퍼된 입력으로 콤보 실행");
        }
        else
        {
           // Debug.Log("CanAttack 호출됨 - 아직 입력 없음");
        }
    }

    public void DisableComboInput()
    {
        allowBufferedInput = false;
        canExecuteImmediately = false;
        //Debug.Log("콤보 입력 종료");
    }

    public void EndCombo()
    {
        isAttacking = false;
        inputBuffered = false;
        allowBufferedInput = false;
        canExecuteImmediately = false;
        inputLocked = false;
        comboIndex = 0;

        animator.applyRootMotion = false;

        movement.ResumeAgent();

        stateMachine.ChangeState(PlayerState.Idle);

       // Debug.Log("공격 초기화");
    }

    //public void ForceEndCombo()
    //{
    //    isAttacking = false;
    //    inputBuffered = false;
    //    allowBufferedInput = false;
    //    canExecuteImmediately = false;
    //    inputLocked = false;
    //    comboIndex = 0;

    //    movement.ResumeAgent();

    //    Debug.Log("공격 초기화 (강제 종료)");

    //    stateMachine.ChangeState(PlayerState.Idle);
    //}

    public void ActivateBasicHitbox()
    {
        string key = "기본공격";

        PlayerSkillController skillSystem = GetComponent<PlayerSkillController>();
        if (skillSystem != null)
        {
            skillSystem.ActivateHitbox(key);
            //Debug.Log($"[기본공격] 히트박스 생성 요청: {key}");
        }
        else
        {
            Debug.LogWarning("PlayerSkillSystem 컴포넌트가 없음!");
        }
    }

    public void EnterCombatMode()
    {
        if (!isCombat)
        {
            isCombat = true;
            animator.SetBool("IsCombat", true);
        }
        combatTimer = 0f;
    }
}
