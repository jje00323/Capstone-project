using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyStatus : CharacterStatus
{
    public Transform target;
    public float moveSpeed;

    public void Setup(BossEnemyData data)
    {
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

    protected override void OnDeath()
    {
        GetComponent<BossEnemyFSM>()?.Die();
    }
}
