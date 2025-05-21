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

    protected override void Awake()
    {
        base.Awake();
        bossStatus = GetComponent<BossEnemyStatus>();
        patternController = GetComponent<BossEnemyPatternController>();
        attackController = GetComponent<BossEnemyAttackController>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
    }

    private void Start()
    {
        if (bossData == null)
        {
            Debug.LogError("[BossEnemyFSM] bossData가 설정되지 않았습니다.");
            return;
        }

        bossStatus.Setup(bossData);
        Animator.runtimeAnimatorController = bossData.animatorController;

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

    public bool CheckWallNearby()
    {
        float checkDistance = 2.0f;
        Vector3 forward = transform.forward;
        RaycastHit hit;

        return false;
    }

    public void EndBossPattern()
    {
        ChangeState(idleState);
    }
}
