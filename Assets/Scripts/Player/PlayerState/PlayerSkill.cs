using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerSkill : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private Animator animator;
    private PlayerSkillController skillController;

    private bool isSkillActive = false;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
        animator = GetComponent<Animator>();
        skillController = GetComponent<PlayerSkillController>();
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame) TryUseSkill("Q");
        if (Keyboard.current.wKey.wasPressedThisFrame) TryUseSkill("W");
        if (Keyboard.current.eKey.wasPressedThisFrame) TryUseSkill("E");
        if (Keyboard.current.rKey.wasPressedThisFrame) TryUseSkill("R");
    }

    public void TryUseSkill(string skillKey)
    {
        if (isSkillActive || !stateMachine.CanSkill()) return;

        var skill = SkillEquipManager.Instance.GetEquippedSkill(skillKey);
        if (skill == null) return;

        float cooldown = skillController.GetSkillCooldown(skillKey);
        if (Time.time - skillController.GetLastUsedTime(skillKey) < cooldown) return;

        Vector3? mouseTarget = Hitbox.MouseUtility.GetMouseWorldPosition(LayerMask.GetMask("Ground"));

        playerMovement.StopAgent();
        if (mouseTarget.HasValue)
            playerMovement.RotateToPosition(mouseTarget.Value);
        else
            playerMovement.RotateToMouse();

        animator.applyRootMotion = true;
        animator.Play(skill.skillAnimation.name);

        skillController.SaveSkillUseTime(skillKey);
        isSkillActive = true;

        if (playerAttack != null)
        {
            playerAttack.EnterCombatMode();
        }
    }

    public void EndSkill()
    {
        isSkillActive = false;
        animator.applyRootMotion = false;
        playerMovement.ResumeAgent();

        animator.SetTrigger("EndSkill");
        stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
    }
}
