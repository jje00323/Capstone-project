using System.Collections;
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

    private bool isSkillActive = false;
    private string currentJob = "Basic";

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        LoadSkillsFromData();

        if (skillData == null)
        {
            Debug.LogError(" skillData가 연결되지 않았습니다!");
            return;
        }
    }

    public void LoadSkillsFromData()
    {
        if (skillData == null) return;

        hitboxPrefabs.Clear();
        effectPrefabs.Clear();
        skillLastUsedTime.Clear();

        currentJob = skillData.jobType.ToString();

        foreach (var skill in skillData.skills)
        {
            string key = currentJob + "_" + skill.skillKey;

            if (skill.hitboxPrefab != null)
                hitboxPrefabs[key] = skill.hitboxPrefab;

            if (skill.effectPrefab != null)
                effectPrefabs[key] = skill.effectPrefab;

            skillLastUsedTime[skill.skillKey] = -999f;
        }
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            TryUseSkill("Q");
        }
    }

    private void TryUseSkill(string skillKey)
    {
        Debug.Log("TryUseSkill 진입");

        if (isSkillActive)
        {
            Debug.Log("스킬 이미 사용 중 (isSkillActive)");
            return;
        }

        if (!stateMachine.CanSkill())
        {
            Debug.Log("스킬 사용 불가 상태 (FSM)");
            return;
        }

        string fullSkillKey = currentJob + "_" + skillKey;

        if (!hitboxPrefabs.ContainsKey(fullSkillKey))
        {
            Debug.Log("히트박스 없음: " + fullSkillKey);
            return;
        }

        float cooldown = GetSkillCooldown(skillKey);
        float lastUsedTime = skillLastUsedTime.ContainsKey(skillKey) ? skillLastUsedTime[skillKey] : -999f;
        if (Time.time - lastUsedTime < cooldown)
        {
            Debug.Log("쿨타임 적용 중");
            return;
        }

        Debug.Log("▶ 애니메이션 트리거 Press_" + skillKey + " 실행됨");

        stateMachine.ChangeState(PlayerStateMachine.PlayerState.SkillCasting);
        playerMovement.RotateToMouse();

        string animTrigger = "Press_" + skillKey;
        animator.SetTrigger(animTrigger);

        skillLastUsedTime[skillKey] = Time.time;
    }
    // 애니메이션 이벤트로 호출될 함수
    public void ActivateHitbox(string skillKey)
    {
        string fullSkillKey = currentJob + "_" + skillKey;
        if (!hitboxPrefabs.ContainsKey(fullSkillKey)) return;

        GameObject prefab = hitboxPrefabs[fullSkillKey];
        GameObject instance = Instantiate(prefab);

        Transform spawnPoint = prefab.transform.Find("SpawnPoint");
        if (spawnPoint != null)
        {
            Vector3 offset = spawnPoint.localPosition;
            Vector3 worldOffset = transform.TransformDirection(offset);
            instance.transform.position = transform.position + worldOffset;
        }
        else
        {
            instance.transform.position = transform.position + transform.forward;
        }

        instance.transform.rotation = transform.rotation;
    }

    // 애니메이션 이벤트로 호출될 함수
    public void SpawnEffect(string skillKey)
    {
        string fullSkillKey = currentJob + "_" + skillKey;
        if (effectPrefabs.ContainsKey(fullSkillKey) && effectPrefabs[fullSkillKey] != null)
        {
            Instantiate(effectPrefabs[fullSkillKey], transform.position + transform.forward, transform.rotation);
        }
    }

    // 애니메이션 이벤트로 호출될 함수
    public void EndSkill()
    {
        isSkillActive = false;
        animator.SetTrigger("end_skill");

        playerMovement.ResumeAgent();
    }

    private float GetSkillCooldown(string skillKey)
    {
        foreach (var skill in skillData.skills)
        {
            if (skill.skillKey == skillKey)
                return skill.cooldown;
        }
        return 0f;
    }

    public void UpdateCurrentJob(JobManager.JobType newJob)
    {
        currentJob = newJob.ToString();
    }
}
