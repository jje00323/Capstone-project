using UnityEngine;

public class EnemyDeadState : EnemyState
{
    private float timer = 0f;
    private float dissolveAmount = 0f;
    private float dissolveDelay = 1f;   // 사망 후 유지 시간
    private float dissolveSpeed = 1f;
    private float dissolveEnd = 1f;
    private bool isDissolving = false;

    private Material dissolveMat;

    public EnemyDeadState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        timer = 0f;
        dissolveAmount = 0f;
        isDissolving = false;

        // Rigidbody 물리 해제 (땅에 꺼지는 현상 방지)
        if (enemy.Rigidbody != null)
        {
            enemy.Rigidbody.useGravity = false;
            enemy.Rigidbody.velocity = Vector3.zero;
            enemy.Rigidbody.isKinematic = true;
        }

        // 애니메이션 재생
        if (enemy.Animator != null)
        {
            enemy.Animator.SetTrigger("Die");
        }

        // 충돌 유지 시간 확보
        if (enemy.Collider != null)
        {
            enemy.Collider.enabled = true;
        }

        // Dissolve 머티리얼 할당
        Renderer rend = enemy.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            dissolveMat = rend.material; // 반드시 인스턴스 머티리얼 사용
            if (dissolveMat.HasProperty("_DissolveAmount"))
                dissolveMat.SetFloat("_DissolveAmount", 0f);
        }
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        // Delay 후 Dissolve 시작
        if (!isDissolving && timer >= dissolveDelay)
        {
            isDissolving = true;

            // 충돌 제거
            if (enemy.Collider != null)
                enemy.Collider.enabled = false;
        }

        if (isDissolving && dissolveMat != null)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;

            // 여기 디버그 로그 찍기
            Debug.Log($"[Dissolve] amount: {dissolveAmount}");

            if (dissolveMat.HasProperty("_DissolveAmount"))
                dissolveMat.SetFloat("_DissolveAmount", dissolveAmount);

            if (dissolveAmount >= dissolveEnd)
            {
                // 삭제 또는 풀링 반환 처리
                GameObject.Destroy(enemy.gameObject);
            }
        }
    }

    public override void Exit()
    {
        
    }
}
