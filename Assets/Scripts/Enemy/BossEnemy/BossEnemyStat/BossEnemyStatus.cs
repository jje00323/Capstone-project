using UnityEngine;

public class BossEnemyStatus : CharacterStatus
{
    [Header("전투 상태")]
    public Transform target;

    [Header("보스 능력치")]
    public float moveSpeed;

    [Header("패턴 관리")]
    public int lastAttackIndex = -1; // 패턴별 Idle 딜레이 관리용

    [HideInInspector] public bool hasEnteredCutscene = false; // 컷씬 1회 진입용

    public BossEnemyData bossData { get; private set; }

    public void Setup(BossEnemyData data)
    {
        bossData = data;
        maxHP = data.maxHP;
        currentHP = maxHP;
        moveSpeed = data.moveSpeed;
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
