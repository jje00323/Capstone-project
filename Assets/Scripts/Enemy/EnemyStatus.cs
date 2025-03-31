using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : CharacterStatus
{
    public EnemyData enemyData;
    public EnemyUI enemyUI;

    public Transform target;

    public float attackPower;

    public void Setup(EnemyData data)
    {
        enemyData = data;
        maxHP = data.maxHP;
        currentHP = maxHP;

        attackPower = data.attackPower;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        enemyUI?.ShowHP(currentHP, maxHP);
    }

    protected override void OnDeath()
    {
        GetComponent<EnemyFSM>()?.Die();
    }
}
