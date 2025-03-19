using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxHealth = 50f; // �ִ� ü��
    private float currentHealth;
    private Renderer enemyRenderer;
    private Color originalColor; // ���� ���� ����

    void Awake()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>(); // ���� Renderer ��������

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color; // �ʱ� ���� ����
        }
    }

    //  ���� ���ݹ��� �� ȣ��Ǵ� �Լ�
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($" ���� {damage}�� ���ظ� ����! ���� ü��: {currentHealth}");

        StartCoroutine(FlashRed()); //  �ǰ� �� ���������� ����

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //  �ǰ� �� ���������� ���� �� ���� �������� ����
    private IEnumerator FlashRed()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red; // ���������� ����
            yield return new WaitForSeconds(0.2f); // 0.2�� �� ����
            enemyRenderer.material.color = originalColor; // ���� �������� ����
        }
    }

    //  �� ��� ó��
    private void Die()
    {
        Debug.Log(" ���� ����߽��ϴ�!");
        Destroy(gameObject); // ��� ����
    }
}
