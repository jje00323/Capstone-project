using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerStateMachine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerSkillSystem : MonoBehaviour
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

    private bool isSkillActive = false;
    private string currentJob = "";

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        LoadSkillsFromData();
    }

    public void LoadSkillsFromData()
    {
        if (skillData == null) return;

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
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame) TryUseSkill("Q");
        if (Keyboard.current.wKey.wasPressedThisFrame) TryUseSkill("W");
        if (Keyboard.current.eKey.wasPressedThisFrame) TryUseSkill("E");
        if (Keyboard.current.rKey.wasPressedThisFrame) TryUseSkill("R");
    }

    private void TryUseSkill(string skillKey)
    {
        if (isSkillActive || !stateMachine.CanSkill()) return;

        string fullSkillKey = currentJob + "_" + skillKey;

        if (!hitboxPrefabs.ContainsKey(fullSkillKey)) return;

        float cooldown = GetSkillCooldown(skillKey);
        float lastUsedTime = skillLastUsedTime.ContainsKey(skillKey) ? skillLastUsedTime[skillKey] : -999f;
        if (Time.time - lastUsedTime < cooldown) return;

        stateMachine.ChangeState(PlayerStateMachine.PlayerState.SkillCasting);
        playerMovement.RotateToMouse();

        string animTrigger = "Press_" + skillKey;
        playerMovement.StopAgent();
        animator.SetTrigger(animTrigger);

        skillLastUsedTime[skillKey] = Time.time;
    }

    // 애니메이션 이벤트로 호출될 함수
    public void ActivateHitbox(string skillKey)
    {
        Debug.Log($"[스킬 시스템] ActivateHitbox 호출됨! skillKey = {skillKey}");

        string fullSkillKey = currentJob + "_" + skillKey;
        if (!hitboxPrefabs.ContainsKey(fullSkillKey))
        {
            Debug.LogWarning(" 히트박스 프리팹을 찾을 수 없음: " + fullSkillKey);
            return;
        }

        GameObject prefab = hitboxPrefabs[fullSkillKey];
        GameObject instance;

        Transform spawnPoint = prefab.transform.Find("SpawnPoint");

        Vector3 offset = spawnPoint.localPosition;
        Vector3 worldOffset = transform.position + transform.TransformDirection(offset);
        Quaternion localRotations = spawnPoint.localRotation;
        worldOffset.y = 1.0f;
        Quaternion worldRotations = transform.rotation * localRotations;
        instance = Instantiate(prefab, worldOffset, worldRotations);

        //  핵심: 생성 후 caster 지정
        Hitbox hitbox = instance.GetComponent<Hitbox>();
        if (hitbox != null)
        {
            hitbox.Initialize(transform); // 이 스크립트를 가진 플레이어 오브젝트
        }
    }

    // 애니메이션 이벤트로 호출될 함수
    public void SpawnEffect(string skillKey)
    {
        string fullSkillKey = currentJob + "_" + skillKey;
        if (!effectPrefabs.ContainsKey(fullSkillKey) || effectPrefabs[fullSkillKey] == null)
            return;

        activeEffect = Instantiate(effectPrefabs[fullSkillKey], transform.position + transform.forward, transform.rotation);

        // 해당 스킬 데이터 가져와서 effectDuration 확인
        SkillInfo skillInfo = GetSkillInfo(skillKey);
        if (skillInfo != null && skillInfo.effectDuration > 0)
        {
            Destroy(activeEffect, skillInfo.effectDuration);
            activeEffect = null;
        }
    }

    // 애니메이션 이벤트로 호출될 함수
    public void DestroyEffect()
    {
        if (activeEffect != null)
        {
            Destroy(activeEffect);
            activeEffect = null;
        }
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

    // 애니메이션 이벤트로 호출될 함수
    public void EndSkill()
    {
        isSkillActive = false;
        animator.SetTrigger("end_skill");
        playerMovement.ResumeAgent();

        stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
    }

    private float GetSkillCooldown(string skillKey)
    {
        return skillCooldowns.ContainsKey(skillKey) ? skillCooldowns[skillKey] : 0f;
    }

    public void UpdateCurrentJob(JobManager.JobType newJob)
    {
        currentJob = newJob.ToString();
        LoadSkillsFromData();
    }
}
