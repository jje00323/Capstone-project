using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage = 10f; // �⺻ ���ݷ�
    public float duration = 0.5f; // ��Ʈ�ڽ� ���� �ð�
    private Collider[] allColliders;

    void Awake()
    {
        allColliders = GetComponents<Collider>();
        if (allColliders != null)
        {
            foreach (var col in allColliders)
            {
                col.isTrigger = true;
            }
        }
    }

    void OnEnable()
    {
        StartCoroutine(DisableAfterDuration());
    }

    IEnumerator DisableAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"���� ���ݴ���! ������: {damage}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // ȸ��/��ġ �ݿ�
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
            // Capsule, Mesh �� �ٸ� Ÿ�Ե� ���ϸ� ���⿡ �߰�
        }

        Gizmos.matrix = oldMatrix; // ���� ���� ����
    }
}