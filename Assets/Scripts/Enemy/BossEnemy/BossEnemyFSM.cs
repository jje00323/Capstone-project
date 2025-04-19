using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyFSM : BaseEnemyFSM
{
    public BossEnemyStatus bossStatus;
    public Animator animator;

    public BossEnemyState currentState;

    public BossEnemyIdleState idleState;
    public BossEnemyPatternState patternState;
    public BossEnemyDeadState deadState;

    protected override void Awake()
    {
        base.Awake();
        bossStatus = GetComponent<BossEnemyStatus>();
    }
    private void Start()
    {
        idleState = new BossEnemyIdleState(this);
        patternState = new BossEnemyPatternState(this);
        deadState = new BossEnemyDeadState(this);

        ChangeState(idleState);
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
        ChangeState(deadState);
        Debug.Log("º¸½º »ç¸Á Ã³¸®µÊ");
    }
}
