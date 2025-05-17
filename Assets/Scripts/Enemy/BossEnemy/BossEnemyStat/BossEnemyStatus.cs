using UnityEngine;

public class BossEnemyStatus : CharacterStatus
{
    [Header("전투 상태")]
    public Transform target;
    [Header("보스 능력치")]
    public float moveSpeed;

    [Header("패턴 관리")]
    public int lastAttackIndex = -1; // 패턴별 Idle 딜레이 관리용

    private bool gimmickTriggered = false;


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

        if (!gimmickTriggered && currentBarIndex < (totalBars / 2))
        {
            gimmickTriggered = true;

            Debug.Log("[BossEnemyStatus] 체력 50% 이하 → 컷씬 전환");
            GetComponent<BossEnemyFSM>().ChangeState(GetComponent<BossEnemyFSM>().cutsceneState);
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