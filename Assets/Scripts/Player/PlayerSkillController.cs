using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerSkillController : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private PlayerMovement playerMovement;
    private PlayerAttack playerattack;
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

        animator.applyRootMotion = true;
        playerMovement.StopAgent();
        playerMovement.RotateToMouse();
        

        // 핵심: 직접 애니메이션 재생
        animator.Play(skill.skillAnimation.name);

        skillLastUsedTime[skillKey] = Time.time;
        isSkillActive = true;

        //PlayerSkillUI.Instance?.StartUICooldown(skillKey, cooldown);


       
        // 전투 상태 진입 및 기본공격 초기화3
        var attackSystem = GetComponent<PlayerAttack>();
        if (attackSystem != null)
        {
            attackSystem.EnterCombatMode(); // 이 메서드를 PlayerAttack에 추가할 거야
        }
        attackSystem.ForceEndCombo();
    }

    public void ActivateHitbox(string skillName)
    {
        SkillInfo skillInfo = null;

        foreach (var s in skillData.skills)
        {
            if (s.skillName == skillName)
            {
                skillInfo = s;
                break;
            }
        }

        if (skillInfo == null || skillInfo.hitboxPrefab == null)
        {
            Debug.LogWarning($"[Hitbox] 이름이 {skillName}인 스킬이 없거나 히트박스 프리팹이 지정되지 않음.");
            return;
        }

        string fullSkillKey = currentJob + "_" + skillInfo.skillKey;
        GameObject prefab = skillInfo.hitboxPrefab;

        Transform spawnPoint = prefab.transform.Find("SpawnPoint");
        if (spawnPoint == null)
        {
            Debug.LogWarning($"[Hitbox] {skillName} 스킬의 프리팹에 'SpawnPoint'가 없음.");
            return;
        }

        Vector3 offset = spawnPoint.localPosition;
        Vector3 worldOffset = transform.position + transform.TransformDirection(offset);
        worldOffset.y = 1.0f;
        Quaternion worldRotations = transform.rotation * spawnPoint.localRotation;

        GameObject instance = Instantiate(prefab, worldOffset, worldRotations);
        Hitbox hitbox = instance.GetComponent<Hitbox>();

        if (hitbox != null)
            hitbox.Initialize(transform, skillInfo.followCaster);
    }

    public void SpawnEffect(string skillName)
    {
        SkillInfo skill = null;

        foreach (var s in skillData.skills)
        {
            if (s.skillName == skillName)
            {
                skill = s;
                break;
            }
        }

        if (skill == null || skill.effectPrefab == null)
        {
            Debug.LogWarning($"[SkillEffect] 이름이 {skillName}인 스킬이 없거나 이펙트가 지정되지 않음.");
            return;
        }

        // 프리팹 내부에 "EffectSpawnPoint" 라는 자식 트랜스폼이 있다고 가정
        Transform spawnTransform = skill.effectPrefab.transform.Find("EffectSpawnPoint");

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (spawnTransform != null)
        {
            // SpawnPoint 위치와 회전을 로컬 기준으로 변환
            spawnPosition = transform.position + transform.TransformDirection(spawnTransform.localPosition);
            spawnRotation = transform.rotation * spawnTransform.localRotation;
        }
        else
        {
            // fallback: 기존 방식
            spawnPosition = transform.position + transform.forward;
            spawnRotation = transform.rotation;
        }

        activeEffect = Instantiate(skill.effectPrefab, spawnPosition, spawnRotation);

        if (skill.effectDuration > 0f)
            Destroy(activeEffect, skill.effectDuration);
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
