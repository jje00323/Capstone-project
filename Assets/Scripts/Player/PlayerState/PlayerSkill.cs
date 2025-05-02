using UnityEngine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerSkill : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private PlayerMovement playerMovement;
    private Animator animator;

    private bool isSkillActive = false;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    public bool CanUseSkill()
    {
        return !isSkillActive && stateMachine.CanSkill();
    }

    public void StartSkill(string animationName)
    {
        if (!CanUseSkill()) return;

        stateMachine.ChangeState(PlayerStateMachine.PlayerState.SkillCasting);

        playerMovement.StopAgent();
        playerMovement.RotateToMouse();

        animator.Play(animationName);
        isSkillActive = true;

        var attack = GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.EnterCombatMode();
            attack.ForceEndCombo();
        }
    }

    public void EndSkill()
    {
        isSkillActive = false;
        playerMovement.ResumeAgent();

        animator.SetTrigger("EndSkill");
        stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
    }

    public bool IsSkillActive()
    {
        return isSkillActive;
    }
}