using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName;
    public int enemyLevel;

    [Header("퀘스트 조건용 태그")] //  추가
    public string enemyTag;

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

    
    [Header("드롭 아이템")]
    public List<DropItemData> dropItems = new List<DropItemData>();
}


[System.Serializable]
public class DropItemData
{
    public GameObject itemPrefab;                  // 아이템 프리팹 (ItemObject 붙은)
    [Range(0f, 100f)] public float dropChance = 100f; // 확률 %
    public GameObject dropEffectPrefab;            // 드롭 이펙트 (선택)
}