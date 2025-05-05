using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerSkill : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private PlayerMovement playerMovement;
    private PlayerSkillController skillController;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
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
        if (!stateMachine.CanSkill()) return;

        var skill = SkillEquipManager.Instance.GetEquippedSkill(skillKey);
        if (skill == null) return;

        float cooldown = skillController.GetSkillCooldown(skillKey);
        if (Time.time - skillController.GetSkillCooldown(skillKey) < cooldown) return;

        Vector3? mouseTarget = Hitbox.MouseUtility.GetMouseWorldPosition(LayerMask.GetMask("Ground"));

        // 쿨타임 시간 저장
        skillController.SetSkillCooldown(skillKey, Time.time);

        // 상태 전이
        stateMachine.ChangeState(PlayerStateMachine.PlayerState.SkillCasting);

        // 실제 스킬 실행 위임
        skillController.ExecuteSkill(skillKey, mouseTarget);
    }
}
