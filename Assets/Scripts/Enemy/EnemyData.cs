using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHP;
    public float moveSpeed;
    public float attackPower;
    public float attackCooldown;
    public float attackRange;
    public float detectRadius;
    public float returnDistance;
    public float returnSpeedMultiplier;
    public int expDrop;
    public RuntimeAnimatorController animatorController;
}
