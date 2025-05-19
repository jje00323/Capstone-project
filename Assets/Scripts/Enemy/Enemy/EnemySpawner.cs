using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("적 프리팹 및 스폰 위치")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("보스 UI 프리팹 및 부모")]
    [SerializeField] private GameObject bossHPUIPrefab;

    private Transform bossUIParent;

    [SerializeField] private bool autoSpawnOnStart = true;

    private void Awake()
    {
        // 컷씬용 BossUIGroup(CanvasGroup)의 Transform을 찾아서 자식으로 넣기
        var bossUIGroup = GameObject.Find("BossUIGroup");
        if (bossUIGroup != null)
        {
            bossUIParent = bossUIGroup.transform;
            Debug.Log("[EnemySpawner] BossUIGroup 자동 연결 완료");
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] BossUIGroup을 찾을 수 없습니다. 보스 체력바 UI가 제대로 표시되지 않을 수 있습니다.");
        }
    }

    private void Start()
    {
        if (autoSpawnOnStart)
            SpawnAll();
    }

    public void SpawnAll()
    {
        foreach (Transform spawn in spawnPoints)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation);

            if (enemy.CompareTag("Boss"))
            {
                var bossFSM = enemy.GetComponent<BossEnemyFSM>();
                if (bossFSM != null && bossFSM.bossData != null)
                {
                    bossFSM.bossStatus.Setup(bossFSM.bossData);
                    Debug.Log("[EnemySpawner] Boss Setup 호출 완료");

                    // 보스 UI 생성 및 연결
                    if (bossHPUIPrefab != null && bossUIParent != null)
                    {
                        GameObject uiGO = Instantiate(bossHPUIPrefab, bossUIParent);
                        var hpUI = uiGO.GetComponent<BossEnemyHealthUI>();

                        if (hpUI != null)
                        {
                            hpUI.SetBoss(bossFSM.bossStatus);
                            Debug.Log("[EnemySpawner] BossHealthUI에 bossStatus 연결 완료");
                        }
                        else
                        {
                            Debug.LogError("[EnemySpawner] BossHPUIPrefab에 BossEnemyHealthUI 컴포넌트가 없습니다.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[EnemySpawner] bossHPUIPrefab 또는 bossUIParent가 비어있습니다.");
                    }
                }
                else
                {
                    Debug.LogError("[EnemySpawner] bossFSM 또는 bossData가 누락되었습니다.");
                }
            }
            else
            {
                SetupNavMeshObstacle(enemy);
            }
        }
    }

    private void SetupNavMeshObstacle(GameObject enemy)
    {
        if (enemy.CompareTag("Boss")) return;

        var obstacle = enemy.GetComponent<NavMeshObstacle>();
        if (obstacle == null)
            obstacle = enemy.AddComponent<NavMeshObstacle>();

        obstacle.carving = true;
        obstacle.carveOnlyStationary = false;
        obstacle.shape = NavMeshObstacleShape.Capsule;
        obstacle.radius = 0.3f;
        obstacle.height = 2.0f;
    }
}
