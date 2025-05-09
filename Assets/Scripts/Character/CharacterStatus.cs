using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public float maxHP;
    public float currentHP;
    public float attack;
    public float defense;
    public virtual void TakeDamage(float damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        if (IsDead)
        {
            OnDeath();
        }
    }

    public virtual void Heal(float amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
    }

    public bool IsDead => currentHP <= 0;

    protected virtual void OnDeath()
    {
        // 자식 클래스에서 구현
    }
}
