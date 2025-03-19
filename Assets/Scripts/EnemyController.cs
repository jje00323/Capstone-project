using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxHealth = 50f; // 최대 체력
    private float currentHealth;
    private Renderer enemyRenderer;
    private Color originalColor; // 원래 색상 저장

    void Awake()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>(); // 적의 Renderer 가져오기

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color; // 초기 색상 저장
        }
    }

    //  적이 공격받을 때 호출되는 함수
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($" 적이 {damage}의 피해를 입음! 현재 체력: {currentHealth}");

        StartCoroutine(FlashRed()); //  피격 시 빨간색으로 변경

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //  피격 시 빨간색으로 변경 후 원래 색상으로 복구
    private IEnumerator FlashRed()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red; // 빨간색으로 변경
            yield return new WaitForSeconds(0.2f); // 0.2초 후 복구
            enemyRenderer.material.color = originalColor; // 원래 색상으로 복구
        }
    }

    //  적 사망 처리
    private void Die()
    {
        Debug.Log(" 적이 사망했습니다!");
        Destroy(gameObject); // 즉시 제거
    }
}
