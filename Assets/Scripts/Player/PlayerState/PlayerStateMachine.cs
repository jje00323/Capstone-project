using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Moving,
        Attacking,
        SkillCasting,
        Dodging,
        Damaged,
        Knockback,
        Dead
    }
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    public void ChangeState(PlayerState newState)
    {
        if (CurrentState == newState) return;

        Debug.Log($"[FSM] 상태 변경: {CurrentState} → {newState}");
        CurrentState = newState;
    }

    public bool CanMove()
    {
        return CurrentState == PlayerState.Idle || CurrentState == PlayerState.Moving;
    }

    public bool CanAttack()
    {
        return CurrentState == PlayerState.Idle || CurrentState == PlayerState.Moving || CurrentState == PlayerState.Attacking;
    }

    public bool CanDodge()
    {
        return CurrentState != PlayerState.Dodging && CurrentState != PlayerState.Dead;
    }
    public bool CanSkill()
    {
        return CurrentState == PlayerState.Idle || CurrentState == PlayerState.Moving || CurrentState == PlayerState.Attacking || CurrentState == PlayerState.SkillCasting;
    }
}

