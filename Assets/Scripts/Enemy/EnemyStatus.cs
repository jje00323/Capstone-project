using System.Collections;
using UnityEngine;

public class EnemyStatus : CharacterStatus
{
    public EnemyData enemyData;
    public EnemyUI enemyUI;

    public Transform target;
    public float attackPower;

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] public Transform damageTextParent;

    private FloatingText currentFloatingText;

    public void Setup(EnemyData data)
    {
        enemyData = data;
        maxHP = data.maxHP;
        currentHP = maxHP;
        attackPower = data.attackPower;
    }

    private void Awake()
    {
        if (damageTextParent == null)
        {
            damageTextParent = transform.Find("WorldCanvas");
        }
    }

    public override void TakeDamage(float damage)
    {
        EnemyFSM fsm = GetComponent<EnemyFSM>();
        if (fsm == null || fsm.currentState == fsm.deadState)
            return;

        if (fsm.currentState == fsm.moveState && fsm.moveState.IsReturning)
        {
            ShowDamageText("무적", Color.gray);
            return;
        }

        base.TakeDamage(damage);
        enemyUI?.ShowHP(currentHP, maxHP);
        ShowDamageText((int)damage, Color.red);

        Debug.Log($"Enemy 현재 체력: {currentHP}");

        if (currentHP <= 0)
        {
            OnDeath();
            return;
        }

        if (fsm.currentState == fsm.stunnedState)
        {
            fsm.ChangeState(fsm.idleState); // 1프레임 idle로 보내고
            fsm.ChangeState(fsm.stunnedState); // 다시 경직 진입
        }
        else
        {
            fsm.ChangeState(fsm.stunnedState);
        }
    }

    protected override void OnDeath()
    {
        GetComponent<EnemyFSM>()?.Die();
    }

    private void ShowDamageText(int damage, Color color)
    {
        if (damageTextPrefab == null || damageTextParent == null)
            return;

        if (currentFloatingText != null)
        {
            currentFloatingText.AddDamage(damage);
        }
        else
        {
            GameObject obj = Instantiate(damageTextPrefab, damageTextParent);
            obj.transform.localPosition = Vector3.zero;

            currentFloatingText = obj.GetComponent<FloatingText>();
            currentFloatingText.SetDamage(damage, color);

            // 파괴 시 EnemyStatus에게 알려줘서 null로 초기화
            currentFloatingText.onDestroyed = () =>
            {
                if (currentFloatingText != null)
                {
                    currentFloatingText = null;
                }
            };
        }
    }

    private void ShowDamageText(string text, Color color)
    {
        if (damageTextPrefab == null || damageTextParent == null)
            return;

        GameObject obj = Instantiate(damageTextPrefab, damageTextParent);
        obj.transform.localPosition = Vector3.zero;

        var floating = obj.GetComponent<FloatingText>();
        floating.SetText(text, color);
    }

    private IEnumerator ClearTextAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentFloatingText = null;
    }

}
