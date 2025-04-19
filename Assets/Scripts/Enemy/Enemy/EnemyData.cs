using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName;
    public int enemyLevel;

    [Header("애니메이션")]
    public RuntimeAnimatorController animatorController;

    [Header("능력치")]
    public float maxHP;
    public float moveSpeed;
    public float attackPower;

    [Header("공격 관련")]
    public float attackCooldown;
    public float attackRange;

    [Header("AI 관련")]
    public float detectRadius;
    public float returnDistance;
    public float returnSpeedMultiplier;

    [Header("보상")]
    public int expDrop;
}
