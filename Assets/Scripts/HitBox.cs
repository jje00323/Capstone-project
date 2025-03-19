using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage = 10f; // 기본 공격력
    public float duration = 0.5f; // 히트박스 지속 시간
    private Collider hitboxCollider;

    void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider != null)
            hitboxCollider.isTrigger = true;
    }

    void OnEnable()
    {
        StartCoroutine(DisableAfterDuration());
    }

    IEnumerator DisableAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"적이 공격당함! 데미지: {damage}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (hitboxCollider != null)
        {
            Gizmos.color = Color.red;
            if (hitboxCollider is BoxCollider)
            {
                BoxCollider box = (BoxCollider)hitboxCollider;
                Gizmos.DrawWireCube(transform.position + box.center, box.size);
            }
            else if (hitboxCollider is SphereCollider)
            {
                SphereCollider sphere = (SphereCollider)hitboxCollider;
                Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
            }
        }
    }
}