using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHP;
    public float attackPower;
    public float moveSpeed;
    public RuntimeAnimatorController animatorController;
}
