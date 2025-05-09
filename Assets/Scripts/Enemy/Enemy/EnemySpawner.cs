using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("적 프리팹 및 스폰 위치")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("보스 UI 관련")]
    [SerializeField] private BossEnemyHealthUI bossHPUI;


    private void Start()
    {
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

                    // UI 연결
                    if (bossHPUI != null)
                    {
                        bossHPUI.SetBoss(bossFSM.bossStatus);
                        Debug.Log("[EnemySpawner] BossHealthUI에 bossStatus 연결 완료");
                    }
                    else
                    {
                        Debug.LogWarning("[EnemySpawner] BossHealthUI가 연결되지 않았습니다.");
                    }
                }
                else
                {
                    Debug.LogError("[EnemySpawner] bossFSM 또는 bossData가 누락되었습니다.");
                }
            }
            else
            {
                SetupNavMeshObstacle(enemy); // 일반 몬스터 처리
            }
        }
    }

    private void SetupNavMeshObstacle(GameObject enemy)
    {
        // 보스는 NavMeshObstacle을 직접 프리팹에서 할당하므로 제외
        if (enemy.CompareTag("Boss")) return;

        var obstacle = enemy.GetComponent<NavMeshObstacle>();
        if (obstacle == null)
        {
            obstacle = enemy.AddComponent<NavMeshObstacle>();
        }

        obstacle.carving = true;
        obstacle.carveOnlyStationary = false;
        obstacle.shape = NavMeshObstacleShape.Capsule;
        obstacle.radius = 0.3f;
        obstacle.height = 2.0f;
    }
}
