using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerSkill : MonoBehaviour
{
    private JobManager.JobType currentJob;
    private PlayerControls controls;
    private PlayerController playerController;
    private Camera mainCamera;

    private bool isDashing = false;

    public Animator Actionanimator;

    private bool bAttacking = false; // 공격 중인지 여부
    private bool bComboEnable = false; // 콤보 입력이 가능한지 여부
    private bool bComboExist = false; // 다음 공격이 예약되었는지 여부
    private float combonum = 0;
    public static PlayerSkill Instance;
    private Dictionary<string, GameObject> hitboxes = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> effectPrefabs = new Dictionary<string, GameObject>();
    GameObject hitbox;
    private bool isSkillActive = false;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.DashSkill.performed += ctx => TryDash();
        controls.Player.OnLeftClick.performed += ctx => UpdateAttacking();
        controls.Player.QWER.performed += TrySkills;

        playerController = GetComponent<PlayerController>();
        Actionanimator = GetComponent<Animator>();
        InitializeSkillCooldowns();
        mainCamera = Camera.main;

        LoadHitboxes();
        LoadSkillEffects();
    }

    private void Start()
    {
        if (JobManager.Instance == null)
        {
            Debug.LogError("PlayerSkill: JobManager.Instance가 null입니다! JobManager가 씬에 있는지 확인하세요.");
            return;
        }

        currentJob = JobManager.Instance.GetCurrentJob();
    }

    private void OnEnable() { controls.Player.Enable(); }
    private void OnDisable() { controls.Player.Disable(); }

    private void UpdateAttacking()
    {


        if (bAttacking)
        {
            if (bComboEnable) // 콤보 가능 상태일 때만 추가 입력을 허용
            {
                bComboExist = true;
                //Debug.Log("추가 공격 입력 감지");
            }
            return;
        }

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {

        string key = $"{currentJob}_Attack";
        
        if (playerController != null)
        {
            playerController.StopMovement();
        }
        bAttacking = true;
        bComboExist = false;

        LookAtMouse();


        //  애니메이션 실행 (Base Layer → AttackState)
        Actionanimator.SetTrigger("NextCombo");
        //ActivateHitbox(key);
        //Debug.Log("공격 시작");

        //  애니메이션이 30% 진행되면 다음 입력을 받을 수 있도록 설정
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f &&
            !Actionanimator.IsInTransition(0));

        bComboEnable = true; //  다음 공격 입력 가능
        //Debug.Log("콤보 입력 가능");


        //  애니메이션이 70% 진행되면 추가 입력을 감지하여 즉시 실행
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
            !Actionanimator.IsInTransition(0));

        if (bComboExist) //  추가 입력이 들어왔으면 즉시 다음 공격 실행
        {
            combonum++;
            //Debug.Log("추가 공격 실행");
            bComboEnable = false;
            StartCoroutine(PerformAttack());
            
        }
        else if (combonum == 3)
        {
            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                !Actionanimator.IsInTransition(0));

            Actionanimator.SetTrigger("endCombo");

            yield return new WaitForSeconds(0.1f); // <- 약간의 지연을 둠
            EndAttack();
            playerController.StartMovement();
        }
        else
        {
            //  공격이 끝나기 직전에 Idle 상태로 복귀
            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                !Actionanimator.IsInTransition(0));
            playerController.StartMovement();
            Actionanimator.SetTrigger("endCombo");


            EndAttack(); //  공격이 완전히 끝나기 전에 미리 종료 처리
        }
    }

    private void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0; //  Y축 회전 방지 (바닥만 보게 함)

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation; // 즉시 회전
            }
        }
    }

    private void EndAttack()
    {
        bAttacking = false;
        bComboEnable = false;
        bComboExist = false;
        combonum = 0;
        isSkillActive = false;
        isDashing = false;
        //Debug.Log("공격 종료 → 즉시 다시 공격 가능");
    }


    public void Hit()
    {
        //Debug.Log(" Hit 이벤트 발생! (공격 판정 처리)");
        // 여기에서 실제 공격 판정을 구현하면 됨 (예: Raycast, Collider 검사 등)
    }

    private void TryDash()
    {
        DashSettings dashSettings = JobManager.Instance.GetDashSettings();

        if (dashSettings.IsReady())
        {
            dashSettings.UseDash(); //  대쉬 사용 시간 기록
            StartCoroutine(Dash(dashSettings));
        }
        else
        {
            //Debug.Log($"대쉬 사용 불가! 남은 쿨타임: {dashSettings.GetRemainingCooldown():F1}초");
        }
    }
    private IEnumerator Dash(DashSettings dashSettings)
    {
        if (isDashing) yield break;
        isDashing = true;

        if (playerController != null)
        {
            playerController.StopMovement();
        }

        Vector3 dashDirection;
        if (dashSettings.dashTowardMouse)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            dashDirection = (mouseWorldPosition - transform.position).normalized;
        }
        else
        {
            dashDirection = transform.forward;
        }

        if (dashSettings.dashTowardMouse && dashDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dashDirection);
        }

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + dashDirection * dashSettings.dashDistance;

        float elapsedTime = 0f;
        while (elapsedTime < dashSettings.dashDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, (dashSettings.dashDistance / dashSettings.dashDuration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        EndAttack();
        playerController.StartMovement();
    }

    private void TrySkills(InputAction.CallbackContext context)
    {
        if (isSkillActive)
        {
            return;
        }

        string keyPressed = context.control.displayName; // 누른 키의 이름 가져오기

        if (IsSkillOnCooldown(keyPressed))
        {
            Debug.Log($" {keyPressed} 스킬은 아직 쿨타임 중!");
            return; // 스킬 사용 불가
        }
        Debug.Log($" TrySkills 실행됨! 입력된 키: {keyPressed}"); //  실행 로그 추가


        if (playerController != null)
        {
            playerController.StopMovement();
            
        }
        LookAtMouse();

        

        if (keyPressed == "Q")StartCoroutine(UseSkill("Q"));
        else if (keyPressed == "W") StartCoroutine(UseSkill("W"));
        else if (keyPressed == "E") StartCoroutine(UseSkill("E"));
        else if (keyPressed == "R") StartCoroutine(UseSkill("R"));
        skillLastUsedTime[keyPressed] = Time.time;
    }

    private IEnumerator UseSkill(string skillName)
    {

        isSkillActive = true;
        string key = $"{currentJob}_{skillName}";
        string animationStartTrigger = $"Press_{skillName}";
        string animatioEndTrigger = $"end_skill";
        Debug.Log($" UseSkill 실행됨! 스킬 이름: {skillName}, 애니메이션: {animationStartTrigger}"); //  실행 로그 추가


        if (hitboxes.ContainsKey(key))
        {

            Debug.Log($" {skillName} 스킬 사용!");
            

            Actionanimator.SetTrigger(animationStartTrigger);
            //ActivateHitbox(key);

            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f &&
                !Actionanimator.IsInTransition(0));

            yield return new WaitUntil(() =>
            {
                var animState = Actionanimator.GetCurrentAnimatorStateInfo(0);
                return animState.normalizedTime >= 0.8f && !Actionanimator.IsInTransition(0);
            });

            Actionanimator.SetTrigger(animatioEndTrigger);

        }
        else
        {
            Debug.LogError($" {key} 스킬이 없습니다! LoadHitboxes()에서 추가하세요.");
        }

        if (playerController != null)
        {
            bAttacking = false; //  기본 공격 가능하도록 초기화
            bComboEnable = false; //  콤보 초기화
            bComboExist = false;
            combonum = 0;
            isSkillActive = false;
            playerController.StartMovement(); // 스킬 종료 후 이동 다시 활성화
        }
    }



    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return transform.position + transform.forward * 5f;
    }


    private void LoadHitboxes()
    {
        hitboxes["Basic_Attack"] = Resources.Load<GameObject>("Hitboxes/Basic_Attack");
        hitboxes["Basic_Q"] = Resources.Load<GameObject>("Hitboxes/Basic_Q");
        hitboxes["Basic_W"] = Resources.Load<GameObject>("Hitboxes/Basic_W");
        hitboxes["Basic_E"] = Resources.Load<GameObject>("Hitboxes/Basic_E");
        hitboxes["Basic_R"] = Resources.Load<GameObject>("Hitboxes/Basic_R");

        hitboxes["Warrior_Attack"] = Resources.Load<GameObject>("Hitboxes/Warrior_Attack");
        hitboxes["Warrior_Q"] = Resources.Load<GameObject>("Hitboxes/Warrior_Q");
        hitboxes["Warrior_W"] = Resources.Load<GameObject>("Hitboxes/Warrior_W");
        hitboxes["Warrior_E"] = Resources.Load<GameObject>("Hitboxes/Warrior_E");
        hitboxes["Warrior_R"] = Resources.Load<GameObject>("Hitboxes/Warrior_R");

    }

    private void LoadSkillEffects()
    {
        effectPrefabs["Basic_Q"] = Resources.Load<GameObject>("Effects/Basic_Q_Effect");
        effectPrefabs["Basic_W"] = Resources.Load<GameObject>("Effects/Basic_W_Effect");
        effectPrefabs["Basic_E"] = Resources.Load<GameObject>("Effects/Basic_E_Effect");
        effectPrefabs["Basic_R"] = Resources.Load<GameObject>("Effects/Basic_R_Effect");

        effectPrefabs["Warrior_Q"] = Resources.Load<GameObject>("Effects/Warrior_Q_Effect");
        effectPrefabs["Warrior_W"] = Resources.Load<GameObject>("Effects/Warrior_W_Effect");
        effectPrefabs["Warrior_E"] = Resources.Load<GameObject>("Effects/Warrior_E_Effect");
        effectPrefabs["Warrior_R"] = Resources.Load<GameObject>("Effects/Warrior_R_Effect");
    }

    //private void ActivateHitbox(string key)
    //{
    //    // 1. 히트박스 프리팹이 존재하는지 확인
    //    if (!hitboxes.ContainsKey(key))
    //    {
    //        Debug.LogError($"히트박스 {key} 없음!");
    //        return;
    //    }

    //    GameObject hitboxPrefab = hitboxes[key];

    //    // 2. 프리팹에서 Hitbox 컴포넌트를 꺼내 설정값 확인 (isProjectile 등)
    //    Hitbox hitboxData = hitboxPrefab.GetComponent<Hitbox>();
    //    if (hitboxData == null)
    //    {
    //        Debug.LogError($"Hitbox 스크립트가 {key} 프리팹에 없습니다.");
    //        return;
    //    }

    //    // 3. 임시 프리팹 생성 → SpawnPoint 위치 계산용
    //    GameObject temp = Instantiate(hitboxPrefab);
    //    Transform spawnPoint = temp.transform.Find("SpawnPoint");

    //    if (spawnPoint == null)
    //    {
    //        Debug.LogError($"히트박스 {key}에 'SpawnPoint' 오브젝트가 없습니다.");
    //        Destroy(temp);
    //        return;
    //    }

    //    // 4. SpawnPoint 기준 로컬 위치/회전을 월드 위치로 변환
    //    Vector3 localOffset = spawnPoint.localPosition;
    //    Quaternion localRotation = spawnPoint.localRotation;

    //    Vector3 worldPosition = transform.position + transform.TransformDirection(localOffset);

    //    Destroy(temp); // 임시 프리팹 제거 (SpawnPoint 용도 끝)

    //    worldPosition.y = 1.0f;

    //    Quaternion worldRotation = transform.rotation * localRotation;

    //    // 히트박스 생성
    //    GameObject hitbox = Instantiate(hitboxPrefab, worldPosition, worldRotation);
        
    //    // 6. 투사체 설정일 경우 → Rigidbody를 forward 방향으로 날려줌
    //    //if (hitboxData.isProjectile)
    //    //{
    //    //    Rigidbody rb = hitbox.GetComponent<Rigidbody>();
    //    //    if (rb != null)
    //    //    {
    //    //        rb.velocity = transform.forward * hitboxData.projectileSpeed;
    //    //    }
    //    //}

    //    // 7. 히트박스 활성화
    //    hitbox.SetActive(true);

    //    // 8. 이펙트 프리팹이 등록되어 있으면 같이 생성 (위치 동일)
    //    if (effectPrefabs.ContainsKey(key))
    //    {
    //        GameObject effect = Instantiate(effectPrefabs[key], worldPosition, Quaternion.LookRotation(transform.forward));
    //        Destroy(effect, 2f); // 2초 뒤 자동 제거
    //    }
    //}

    // 히트박스 일정 시간 후 비활성화

    private Dictionary<JobManager.JobType, Dictionary<string, float>> jobSkillCooldowns = new Dictionary<JobManager.JobType, Dictionary<string, float>>();

    private Dictionary<string, float> skillLastUsedTime = new Dictionary<string, float>(); // 마지막 사용 시간 저장

    //  직업별 스킬 쿨타임 초기화
    private void InitializeSkillCooldowns()
    {
        jobSkillCooldowns[JobManager.JobType.Basic] = new Dictionary<string, float>
    {
        { "Q", 2.0f },
        { "W", 2.0f },
        { "E", 2.0f },
        { "R", 2.0f }
    };

        jobSkillCooldowns[JobManager.JobType.Warrior] = new Dictionary<string, float>
    {
        { "Q", 1.0f },
        { "W", 1.0f },
        { "E", 1.0f },
        { "R", 1.0f }
    };

        jobSkillCooldowns[JobManager.JobType.Mage] = new Dictionary<string, float>
    {
        { "Q", 4.0f },  
        { "W", 7.0f },
        { "E", 12.0f },
        { "R", 25.0f }   
    };

        jobSkillCooldowns[JobManager.JobType.Archer] = new Dictionary<string, float>
    {
        { "Q", 3.0f },  
        { "W", 6.0f },
        { "E", 8.0f },
        { "R", 15.0f }
    };
    }

    private bool IsSkillOnCooldown(string skillName)
    {
        if (!skillLastUsedTime.ContainsKey(skillName))
            return false; //  스킬을 한 번도 사용하지 않았다면 사용 가능

        float lastUsed = skillLastUsedTime[skillName];
        float cooldown = GetSkillCooldown(skillName); // 현재 직업의 스킬 쿨타임 가져오기

        return (Time.time - lastUsed) < cooldown; //  현재 시간이 마지막 사용 시간 + 쿨타임보다 작으면 아직 사용 불가
    }
    private float GetSkillCooldown(string skillName)
    {
        JobManager.JobType currentJob = JobManager.Instance.GetCurrentJob();

        if (jobSkillCooldowns.ContainsKey(currentJob) && jobSkillCooldowns[currentJob].ContainsKey(skillName))
        {
            return jobSkillCooldowns[currentJob][skillName]; //  현재 직업의 해당 스킬 쿨타임 반환
        }

        return 2.0f; // 기본값 (만약 설정되지 않은 경우)
    }

    public void UpdateCurrentJob(JobManager.JobType newJob)
    {
        currentJob = newJob;
    }

}
