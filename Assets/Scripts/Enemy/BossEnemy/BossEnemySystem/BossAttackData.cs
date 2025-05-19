using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Attack Pattern Data")]
public class BossAttackData : ScriptableObject
{
    public int attackIndex;

    [System.Serializable]
    public class HitboxInfo
    {
        public GameObject hitboxPrefab;
        public bool followCaster = false;
    }

    [Header("히트박스 정보")]
    public List<HitboxInfo> hitboxInfos;

    [System.Serializable]
    public class TelegraphInfo
    {
        public GameObject telegraphPrefab;
        public BossHitbox.ShapeType shapeType; // 기존 ShapeType → BossHitbox.ShapeType 로 변경
    }

    [Header("경고 장판 프리팹")]
    public List<TelegraphInfo> telegraphInfos;
}