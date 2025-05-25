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
            }
            else if (canExecuteImmediately)
            {
                ExecuteNextCombo();
            }
            else
            {
                Debug.Log("입력 무시됨 (콤보 타이밍 아니면)");
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
    }

    public void EnableComboInput()
    {
        inputBuffered = false;
        allowBufferedInput = true;
        canExecuteImmediately = false;
        inputLocked = false;
    }

    public void CanAttack()
    {
        canExecuteImmediately = true;

        if (inputBuffered)
        {
            ExecuteNextCombo();
        }
    }

    public void DisableComboInput()
    {
        allowBufferedInput = false;
        canExecuteImmediately = false;
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
    }

    public void ActivateBasicHitbox()
    {
        string key = "기본공격";

        PlayerSkillController skillSystem = GetComponent<PlayerSkillController>();
        if (skillSystem != null)
        {
            skillSystem.ActivateHitbox(key);
            skillSystem.SpawnEffect(key);
            // 사운드는 애니메이션 이벤트에서 PlaySkillSound1 / 2 등으로 호출
        }
        else
        {
            Debug.LogWarning("PlayerSkillController 컨포넌트가 없습니다!");
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
