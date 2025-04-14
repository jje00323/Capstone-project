using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveState : EnemyState
{
    private Vector3 destination;
    private float stoppingDistance = 0.2f;
    private bool isReturning = false;
    public bool IsReturning => isReturning;

    public EnemyMoveState(EnemyFSM enemy) : base(enemy) { }

    public void SetDestination(Vector3 targetPos, bool returning = false)
    {
        destination = targetPos;
        isReturning = returning;
    }

    public override void Enter()
    {
        enemy.animator.SetBool("IsMoving", true);

        if (isReturning)
        {
            enemy.gameObject.layer = LayerMask.NameToLayer("EnemyIgnorePlayer");

            var obstacle = enemy.GetComponent<NavMeshObstacle>();
            if (obstacle != null)
            {
                obstacle.carving = false; // 복귀 중엔 NavMesh 방해 안 함
            }
        }
        else
        {
            enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
    }

    public override void Update()
    {
        Transform target = enemy.enemyStatus.target;
        if (!isReturning && target != null)
        {
            float detectDistance = Vector3.Distance(enemy.transform.position, target.position);
            if (detectDistance <= enemy.enemyData.detectRadius)
            {
                enemy.ChangeState(enemy.detectState);
                return;
            }
        }

        Vector3 dir = (destination - enemy.transform.position).normalized;

        float speed = isReturning
        ? enemy.enemyData.moveSpeed * enemy.enemyData.returnSpeedMultiplier
        : enemy.enemyData.moveSpeed;

        enemy.transform.position += dir * speed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            targetRotation,
            360f * Time.deltaTime
        );

        float distance = Vector3.Distance(enemy.transform.position, destination);
        if (distance <= stoppingDistance)
        {
            enemy.animator.SetBool("IsMoving", false);
            enemy.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsMoving", false);
    }
}

