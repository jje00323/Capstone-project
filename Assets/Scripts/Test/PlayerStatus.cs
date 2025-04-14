using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : CharacterStatus
{
    public float maxMP = 50f;
    public float currentMP = 50f;

    public float maxEXP = 100f;
    public float currentEXP = 0f;

    public int level = 1;

    public PlayerUI playerUI;

    void Start()
    {
        UpdateAllUI();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        playerUI.UpdateHP(currentHP, maxHP);
        Debug.Log($"player 현재 체력: {currentHP}");
    }

    public override void Heal(float amount)
    {
        base.Heal(amount); // 상위 클래스 기능 사용
        playerUI?.UpdateHP(currentHP, maxHP); // UI 업데이트 등 플레이어 전용 처리
    }

    public void UseMana(float amount)
    {
        currentMP = Mathf.Clamp(currentMP - amount, 0, maxMP);
        playerUI.UpdateMP(currentMP, maxMP);
    }

    public void GainEXP(float amount)
    {
        currentEXP += amount;
        if (currentEXP >= maxEXP)
        {
            currentEXP -= maxEXP;
            level++;
            playerUI.UpdateLevel(level);
        }
        playerUI.UpdateEXP(currentEXP, maxEXP);
    }

    public void UpdateAllUI()
    {
        playerUI.UpdateHP(currentHP, maxHP);
        playerUI.UpdateMP(currentMP, maxMP);
        playerUI.UpdateEXP(currentEXP, maxEXP);
        playerUI.UpdateLevel(level);
    }

    protected override void OnDeath()
    {
        Debug.Log("플레이어 사망 처리");
        // 플레이어 사망 처리 추가
    }
}

