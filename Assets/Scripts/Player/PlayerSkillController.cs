using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerSkillController : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private PlayerMovement playerMovement;
    private Animator animator;

    [Header("스킬 데이터 (ScriptableObject)")]
    public JobSkillData skillData;

    private Dictionary<string, GameObject> hitboxPrefabs = new();
    private Dictionary<string, GameObject> effectPrefabs = new();
    private Dictionary<string, float> skillLastUsedTime = new();
    private Dictionary<string, float> skillCooldowns = new();

    private GameObject activeEffect;
    private GameObject player;
    private bool isSkillActive = false;
    private string currentJob = "";

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        LoadSkillsFromData(skillData);
    }

    public void LoadSkillsFromData(JobSkillData newData)
    {
        if (newData == null)
        {
            Debug.LogWarning("스킬 데이터가 없습니다.");
            return;
        }

        skillData = newData;
        hitboxPrefabs.Clear();
        effectPrefabs.Clear();
        skillLastUsedTime.Clear();
        skillCooldowns.Clear();

        currentJob = skillData.jobType.ToString();

        foreach (var skill in skillData.skills)
        {
            string key = currentJob + "_" + skill.skillKey;

            if (skill.hitboxPrefab != null)
                hitboxPrefabs[key] = skill.hitboxPrefab;

            if (skill.effectPrefab != null)
                effectPrefabs[key] = skill.effectPrefab;

            skillLastUsedTime[skill.skillKey] = -999f;
            skillCooldowns[skill.skillKey] = skill.cooldown;
        }

        Debug.Log($"[SkillController] {currentJob} 스킬 데이터 로드 완료");
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

        SkillInfo skill = SkillEquipManager.Instance.GetEquippedSkill(skillKey);
        if (skill == null || skill.skillAnimation == null) return;

        float cooldown = GetSkillCooldown(skillKey);
        if (Time.time - GetLastUsedTime(skillKey) < cooldown) return;

        stateMachine.ChangeState(PlayerStateMachine.PlayerState.SkillCasting);
        playerMovement.RotateToMouse();
        playerMovement.StopAgent();
        animator.applyRootMotion = true;

        // 핵심: 직접 애니메이션 재생
        animator.Play(skill.skillAnimation.name);

        skillLastUsedTime[skillKey] = Time.time;
        isSkillActive = true;

        PlayerSkillUI.Instance?.StartUICooldown(skillKey, cooldown);

       

        // 전투 상태 진입
        var attackSystem = GetComponent<PlayerAttack>();
        if (attackSystem != null)
        {
            attackSystem.EnterCombatMode(); // 이 메서드를 PlayerAttack에 추가할 거야
        }
    }

    public void ActivateHitbox(string skillKey)
    {
        string fullSkillKey = currentJob + "_" + skillKey;
        if (!hitboxPrefabs.ContainsKey(fullSkillKey)) return;

        GameObject prefab = hitboxPrefabs[fullSkillKey];
        Transform spawnPoint = prefab.transform.Find("SpawnPoint");

        Vector3 offset = spawnPoint.localPosition;
        Vector3 worldOffset = transform.position + transform.TransformDirection(offset);
        worldOffset.y = 1.0f;
        Quaternion worldRotations = transform.rotation * spawnPoint.localRotation;

        GameObject instance = Instantiate(prefab, worldOffset, worldRotations);
        Hitbox hitbox = instance.GetComponent<Hitbox>();

        SkillInfo skillInfo = GetSkillInfo(skillKey);
        bool shouldFollow = skillInfo != null && skillInfo.followCaster;

        if (hitbox != null)
            hitbox.Initialize(transform, shouldFollow);
    }

    public void SpawnEffect(string skillKey)
    {
        string fullSkillKey = currentJob + "_" + skillKey;
        if (!effectPrefabs.ContainsKey(fullSkillKey)) return;

        activeEffect = Instantiate(effectPrefabs[fullSkillKey], transform.position + transform.forward, transform.rotation);

        SkillInfo skillInfo = GetSkillInfo(skillKey);
        if (skillInfo != null && skillInfo.effectDuration > 0)
        {
            Destroy(activeEffect, skillInfo.effectDuration);
            activeEffect = null;
        }
    }

    public void DestroyEffect()
    {
        if (activeEffect != null)
        {
            Destroy(activeEffect);
            activeEffect = null;
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

    private float GetSkillCooldown(string skillKey)
    {
        return skillCooldowns.TryGetValue(skillKey, out float cooldown) ? cooldown : 0f;
    }

    private float GetLastUsedTime(string skillKey)
    {
        return skillLastUsedTime.TryGetValue(skillKey, out float lastTime) ? lastTime : -999f;
    }

    public void UpdateCurrentJob(JobManager.JobType newJob)
    {
        currentJob = newJob.ToString();
        LoadSkillsFromData(skillData);
    }

    private SkillInfo GetSkillInfo(string skillKey)
    {
        foreach (var skill in skillData.skills)
        {
            if (skill.skillKey == skillKey)
                return skill;
        }
        return null;
    }
}
