using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBossEnemyData", menuName = "Enemy/BossEnemyData")]
public class BossEnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string bossName;
    public int bossLevel;
    public Sprite portrait;

    [Header("스탯")]
    public float maxHP;
    public int hpBarCount;
    public float moveSpeed;
    public float attackPower;

    [Header("애니메이션")]
    public RuntimeAnimatorController animatorController;

    [Header("패턴 전환 조건")]
    public float[] phaseThresholds; // ex) [0.7f, 0.4f] → 체력 70%, 40%일 때 컷씬 발생

    [Header("시네마틱 컷씬 데이터")]
    public string[] cutsceneDialogues;
    public AudioClip[] cutsceneVoiceClips;
    public GameObject cutsceneVFXPrefab;
    public float cutsceneDuration = 4f;

    [Header("보상")]
    public int expReward;
    public int goldReward;
    public GameObject[] itemDrops; // 드랍 아이템 프리팹

    [Header("기타")]
    public AudioClip bossThemeMusic;
}
