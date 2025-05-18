using UnityEngine;

public class BossEnemyStatus : CharacterStatus
{
    [Header("전투 상태")]
    public Transform target;
    [Header("보스 능력치")]
    public float moveSpeed;

    [Header("패턴 관리")]
    public int lastAttackIndex = -1; // 패턴별 Idle 딜레이 관리용

    //private bool cutsceneTriggered = false;


    public BossEnemyData bossData { get; private set; }

    public void Setup(BossEnemyData data)
    {
        bossData = data;
        maxHP = data.maxHP;
        currentHP = maxHP;
        moveSpeed = data.moveSpeed;
        // attackPower 등 추가 필요시 가져오기
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    public override void TakeDamage(float damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        Debug.Log($"[BossEnemyStatus] {damage} 데미지 → 현재 체력: {currentHP}");

        int totalBars = bossData.hpBarCount;
        float segmentHP = maxHP / totalBars;
        int currentBarIndex = Mathf.FloorToInt(currentHP / segmentHP);

        // 컷씬 조건 (체력 50% 이하 + 미발동)
        if (!GetComponent<BossEnemyFSM>().cutsceneTriggered &&
            currentHP / maxHP <= 0.5f)
        {
            var fsm = GetComponent<BossEnemyFSM>();

            if (fsm.currentState == fsm.patternState &&
                fsm.Animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                // 애니메이션 실행 중이면 예약
                fsm.cutsceneReserved = true;
                Debug.Log("[BossEnemyStatus] 컷씬 예약됨 (공격 중)");
            }
            else if (!fsm.Animator.IsInTransition(0))
            {
                // 즉시 진입 가능
                fsm.cutsceneTriggered = true;
                fsm.ChangeState(fsm.cutsceneState);
                Debug.Log("[BossEnemyStatus] 컷씬 상태 즉시 진입");
            }

            return;
        }

        if (IsDead)
        {
            OnDeath();
            return;
        }
    }

    protected override void OnDeath()
    {
        GetComponent<BossEnemyFSM>()?.Die();
    }

}