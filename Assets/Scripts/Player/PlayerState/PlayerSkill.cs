using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerSkill : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private PlayerMovement playerMovement;
    private PlayerSkillController skillController;
    private Animator animator;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
        skillController = GetComponent<PlayerSkillController>();
        animator = GetComponent<Animator>();
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
        if (!stateMachine.CanSkill()) return;

        

        var skill = SkillEquipManager.Instance.GetEquippedSkill(skillKey);
        if (skill == null) return;

        if (skill.currentLevel <= 0)
        {
            Debug.Log($"[Skill] {skillKey}는 잠금 상태입니다.");
            return;
        }

        float cooldown = skillController.GetSkillCooldown(skillKey);
        float lastUsedTime = skillController.GetSkillLastUsedTime(skillKey);

        float timeSinceLastUse = Time.time - lastUsedTime;
        if (timeSinceLastUse < cooldown)
        {
            float remaining = cooldown - timeSinceLastUse;
            Debug.Log($"[Skill] {skillKey} 스킬 쿨타임: {remaining:F1}초 남음");
            return;
        }

        Vector3? mouseTarget = Hitbox.MouseUtility.GetMouseWorldPosition(LayerMask.GetMask("Ground"));

        // 정확히 지금 사용한 시점 저장
        skillController.SetSkillLastUsedTime(skillKey, Time.time);

        Debug.Log($"[Skill] {skillKey} 스킬 사용! (쿨타임 {cooldown}초)");

        stateMachine.ChangeState(PlayerStateMachine.PlayerState.SkillCasting);
        skillController.ExecuteSkill(skillKey, mouseTarget);
    }
    public void EndSkill()
    {
        var attack = GetComponent<PlayerAttack>();
        if (attack != null)
            attack.EndCombo(); // 콤보 종료 시 RootMotion은 이쪽에서 꺼짐

        //  공격 중이 아니라면 여기서만 꺼줌
        if (attack == null || !IsInAttackState())
            animator.applyRootMotion = false;

        playerMovement.ResumeAgent();
        animator.SetTrigger("EndSkill");
        stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
    }

    private bool IsInAttackState()
    {
        return stateMachine.CurrentState == PlayerStateMachine.PlayerState.Attacking;
    }
}
