using UnityEngine;
using UnityEngine.AI; // NavMeshObstacle 사용을 위한 네임스페이스 추가

public class BossEnemyFSM : BaseEnemyFSM
{
    [Header("보스 데이터")]
    public BossEnemyData bossData;

    [Header("연결 컴포넌트")]
    public BossEnemyStatus bossStatus;
    public BossEnemyPatternController patternController;
    [Header("공격 컨트롤러")]
    public BossEnemyAttackController attackController;
    public NavMeshObstacle navMeshObstacle; // 보스 충돌 관통 처리를 위한 필드 추가

    [Header("상태 제어")]
    public bool lockRotation = false;

    [Header("상태")]
    public BossEnemyState currentState;
    public BossEnemyIdleState idleState;
    public BossEnemyMoveState moveState;
    public BossEnemyPatternState patternState;
    public BossEnemyDeadState deadState;
    public BossEnemyCutsceneState cutsceneState;
    public BossEnemyIntroState introState;

    [Header("디버그용 테스트 패턴")]
    public bool useTestPattern = false;
    [Range(0, 7)] public int testPatternIndex = 0;

    [HideInInspector] public bool cutsceneTriggered = false;
    [HideInInspector] public bool cutsceneReserved = false;

    protected override void Awake()
    {
        base.Awake();
        bossStatus = GetComponent<BossEnemyStatus>();
        patternController = GetComponent<BossEnemyPatternController>();
        attackController = GetComponent<BossEnemyAttackController>();
        navMeshObstacle = GetComponent<NavMeshObstacle>(); // NavMeshObstacle 컴포넌트 참조 초기화
    }

    private void Start()
    {
        if (bossData == null)
        {
            Debug.LogError("[BossEnemyFSM] bossData가 설정되지 않았습니다.");
            return;
        }

        // 데이터 적용
        bossStatus.Setup(bossData);
        Animator.runtimeAnimatorController = bossData.animatorController;

        // 상태 초기화
        idleState = new BossEnemyIdleState(this);
        moveState = new BossEnemyMoveState(this);
        patternState = new BossEnemyPatternState(this);
        deadState = new BossEnemyDeadState(this);
        cutsceneState = new BossEnemyCutsceneState(this);
        introState = new BossEnemyIntroState(this);

        ChangeState(introState);
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(BossEnemyState newState)
    {
        // 컷씬 중에는 절대 다른 상태로 전환 불가
        if (currentState == cutsceneState && newState != cutsceneState)
        {
            Debug.LogWarning($"[FSM] 컷씬 중 상태 전환 차단됨: {newState.GetType().Name}");
            return;
        }

        if (currentState == newState) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public override void Die()
    {
        Debug.Log("[BossEnemyFSM] Die() 호출됨 → DeadState 전환");
        ChangeState(deadState);
    }

    /// <summary>
    /// 전방 벽 체크: 보스가 바라보는 방향에 벽 또는 장애물이 가까이 있는지 확인
    /// </summary>
    public bool CheckWallNearby()
    {
        float checkDistance = 2.0f;
        Vector3 forward = transform.forward;
        RaycastHit hit;

        //if (Physics.Raycast(transform.position, forward, out hit, checkDistance))
        //{
        //    return hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Obstacle");
        //}

        return false;
    }

    public void EndBossPattern()
    {
        ChangeState(idleState);
    }
}
