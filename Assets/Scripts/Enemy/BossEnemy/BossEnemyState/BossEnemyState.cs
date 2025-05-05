using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossEnemyState
{
    protected BossEnemyFSM boss;


    public BossEnemyState(BossEnemyFSM boss)
    {
        this.boss = boss;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}