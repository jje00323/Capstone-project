using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Attack Pattern Data")]
public class BossAttackData : ScriptableObject
{
    public int attackIndex;

    [Header("히트박스 프리팹")]
    public List<GameObject> hitboxPrefabs;

    [System.Serializable]
    public class TelegraphInfo
    {
        public GameObject telegraphPrefab;
        public ShapeType shapeType;
    }

    [Header("경고 장판 프리팹")]
    public List<TelegraphInfo> telegraphInfos;
}