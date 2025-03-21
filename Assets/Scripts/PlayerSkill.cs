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

    private bool bAttacking = false; // ���� ������ ����
    private bool bComboEnable = false; // �޺� �Է��� �������� ����
    private bool bComboExist = false; // ���� ������ ����Ǿ����� ����
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
            Debug.LogError("PlayerSkill: JobManager.Instance�� null�Դϴ�! JobManager�� ���� �ִ��� Ȯ���ϼ���.");
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
            if (bComboEnable) // �޺� ���� ������ ���� �߰� �Է��� ���
            {
                bComboExist = true;
                //Debug.Log("�߰� ���� �Է� ����");
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
            playerController.StopAllMovementAnimations();
        }
        bAttacking = true;
        bComboExist = false;

        LookAtMouse();


        //  �ִϸ��̼� ���� (Base Layer �� AttackState)
        Actionanimator.SetTrigger("NextCombo");
        StartCoroutine(ActivateHitboxAfterDelay(key, 0.3f));
        //Debug.Log("���� ����");

        //  �ִϸ��̼��� 30% ����Ǹ� ���� �Է��� ���� �� �ֵ��� ����
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f &&
            !Actionanimator.IsInTransition(0));

        bComboEnable = true; //  ���� ���� �Է� ����
        //Debug.Log("�޺� �Է� ����");


        //  �ִϸ��̼��� 70% ����Ǹ� �߰� �Է��� �����Ͽ� ��� ����
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
            !Actionanimator.IsInTransition(0));

        if (bComboExist) //  �߰� �Է��� �������� ��� ���� ���� ����
        {
            combonum++;
            //Debug.Log("�߰� ���� ����");
            bComboEnable = false;
            StartCoroutine(PerformAttack());
            
        }
        else if(combonum == 3)
        {
            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                !Actionanimator.IsInTransition(0));
            playerController.StartMovement();
            Actionanimator.SetTrigger("endCombo");
        }
        else
        {
            //  ������ ������ ������ Idle ���·� ����
            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                !Actionanimator.IsInTransition(0));
            playerController.StartMovement();
            Actionanimator.SetTrigger("endCombo");


            EndAttack(); //  ������ ������ ������ ���� �̸� ���� ó��
        }
    }

    private void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0; //  Y�� ȸ�� ���� (�ٴڸ� ���� ��)

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation; // ��� ȸ��
            }
        }
    }

    private void EndAttack()
    {
        bAttacking = false;
        bComboEnable = false;
        //Debug.Log("���� ���� �� ��� �ٽ� ���� ����");
        
    }


    public void Hit()
    {
        //Debug.Log(" Hit �̺�Ʈ �߻�! (���� ���� ó��)");
        // ���⿡�� ���� ���� ������ �����ϸ� �� (��: Raycast, Collider �˻� ��)
    }

    private void TryDash()
    {
        DashSettings dashSettings = JobManager.Instance.GetDashSettings();

        if (dashSettings.IsReady())
        {
            dashSettings.UseDash(); //  �뽬 ��� �ð� ���
            StartCoroutine(Dash(dashSettings));
        }
        else
        {
            //Debug.Log($"�뽬 ��� �Ұ�! ���� ��Ÿ��: {dashSettings.GetRemainingCooldown():F1}��");
        }
    }
    private IEnumerator Dash(DashSettings dashSettings)
    {
        if (isDashing) yield break;
        isDashing = true;

        if (playerController != null)
        {
            playerController.StopMovement();
            playerController.StopAllMovementAnimations();
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
        isDashing = false;
        bAttacking = false; //  �⺻ ���� �����ϵ��� �ʱ�ȭ
        bComboEnable = false; //  �޺� �ʱ�ȭ
        bComboExist = false;
        combonum = 0;
        playerController.StartMovement();
    }

    private void TrySkills(InputAction.CallbackContext context)
    {
        if (isSkillActive)
        {
            return;
        }

        string keyPressed = context.control.displayName; // ���� Ű�� �̸� ��������

        if (IsSkillOnCooldown(keyPressed))
        {
            Debug.Log($" {keyPressed} ��ų�� ���� ��Ÿ�� ��!");
            return; // ��ų ��� �Ұ�
        }
        Debug.Log($" TrySkills �����! �Էµ� Ű: {keyPressed}"); //  ���� �α� �߰�


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
        Debug.Log($" UseSkill �����! ��ų �̸�: {skillName}, �ִϸ��̼�: {animationStartTrigger}"); //  ���� �α� �߰�


        if (hitboxes.ContainsKey(key))
        {

            Debug.Log($" {skillName} ��ų ���!");
            

            Actionanimator.SetTrigger(animationStartTrigger);
            StartCoroutine(ActivateHitboxAfterDelay(key, 0.3f));

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
            Debug.LogError($" {key} ��ų�� �����ϴ�! LoadHitboxes()���� �߰��ϼ���.");
        }

        if (playerController != null)
        {
            bAttacking = false; //  �⺻ ���� �����ϵ��� �ʱ�ȭ
            bComboEnable = false; //  �޺� �ʱ�ȭ
            bComboExist = false;
            combonum = 0;
            isSkillActive = false;
            playerController.StartMovement(); // ��ų ���� �� �̵� �ٽ� Ȱ��ȭ
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

    }

    private void LoadSkillEffects()
    {
        effectPrefabs["Basic_Q"] = Resources.Load<GameObject>("Effects/Basic_Q_Effect");
        effectPrefabs["Basic_W"] = Resources.Load<GameObject>("Effects/Basic_W_Effect");
        effectPrefabs["Basic_E"] = Resources.Load<GameObject>("Effects/Basic_E_Effect");
        effectPrefabs["Basic_R"] = Resources.Load<GameObject>("Effects/Basic_R_Effect");
    }

    private IEnumerator ActivateHitboxAfterDelay(string key, float delay)
    {
        yield return new WaitForSeconds(delay); // �ִϸ��̼� Ÿ�̹� ���� ����

        if (!hitboxes.ContainsKey(key))
        {
            Debug.LogError($"��Ʈ�ڽ� {key} ����!");
            yield break;
        }

        GameObject hitboxPrefab = hitboxes[key];

        // 1. �ӽ÷� ������ �����ؼ� SpawnPoint ��ġ�� ȸ�� ������ �����´�
        GameObject temp = Instantiate(hitboxPrefab); // �ӽ÷� ���� (��ġ ����)
        Transform spawnPoint = temp.transform.Find("SpawnPoint");

        if (spawnPoint == null)
        {
            Debug.LogError($"��Ʈ�ڽ� {key}�� 'SpawnPoint' ������Ʈ�� �����ϴ�.");
            Destroy(temp);
            yield break;
        }

        // 2. SpawnPoint�� ���� ��ġ�� ȸ���� �������� ���� ��ġ ���
        Vector3 localOffset = spawnPoint.localPosition;
        Quaternion localRotation = spawnPoint.localRotation;

        // ���� �÷��̾��� ��ġ�� ������ �������� ��ȯ
        Vector3 worldPosition = transform.position + transform.TransformDirection(localOffset);
        Quaternion worldRotation = transform.rotation * localRotation;

        Destroy(temp); // ��ġ ��� �������� �ӽ� ������ ����

        // 3. ��Ʈ�ڽ��� �ùٸ� ��ġ�� �������� ����
        hitbox = Instantiate(hitboxPrefab, worldPosition, worldRotation);
        hitbox.SetActive(true);

        // ����Ʈ ����
        if (effectPrefabs.ContainsKey(key))
        {
            GameObject effect = Instantiate(effectPrefabs[key], spawnPosition, Quaternion.LookRotation(transform.forward));
            Destroy(effect, 2f); // 2�� �� �ڵ� ����
        }
    }

    // ��Ʈ�ڽ� ���� �ð� �� ��Ȱ��ȭ

    private Dictionary<JobManager.JobType, Dictionary<string, float>> jobSkillCooldowns = new Dictionary<JobManager.JobType, Dictionary<string, float>>();

    private Dictionary<string, float> skillLastUsedTime = new Dictionary<string, float>(); // ������ ��� �ð� ����

    //  ������ ��ų ��Ÿ�� �ʱ�ȭ
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
        { "Q", 5.0f },
        { "W", 8.0f },
        { "E", 10.0f },
        { "R", 20.0f }
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
            return false; //  ��ų�� �� ���� ������� �ʾҴٸ� ��� ����

        float lastUsed = skillLastUsedTime[skillName];
        float cooldown = GetSkillCooldown(skillName); // ���� ������ ��ų ��Ÿ�� ��������

        return (Time.time - lastUsed) < cooldown; //  ���� �ð��� ������ ��� �ð� + ��Ÿ�Ӻ��� ������ ���� ��� �Ұ�
    }
    private float GetSkillCooldown(string skillName)
    {
        JobManager.JobType currentJob = JobManager.Instance.GetCurrentJob();

        if (jobSkillCooldowns.ContainsKey(currentJob) && jobSkillCooldowns[currentJob].ContainsKey(skillName))
        {
            return jobSkillCooldowns[currentJob][skillName]; //  ���� ������ �ش� ��ų ��Ÿ�� ��ȯ
        }

        return 2.0f; // �⺻�� (���� �������� ���� ���)
    }

}
