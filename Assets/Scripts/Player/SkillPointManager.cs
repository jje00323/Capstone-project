using UnityEngine;

public class SkillPointManager : MonoBehaviour
{
    public static SkillPointManager Instance;

    [SerializeField] private int currentPoints = 0;
    [SerializeField] private int maxPoints = 8;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int GetPoints() => currentPoints;

    public void GainPointByLevel(int previousLevel)
    {
        currentPoints = Mathf.Min(currentPoints + previousLevel, maxPoints);
    }

    public bool CanSpendPoint() => currentPoints > 0;

    //public bool SpendPoint()
    //{
    //    if (currentPoints <= 0) return false;
    //    currentPoints--;
    //    return true;
    //}

    //public void RefundPoint()
    //{
    //    currentPoints = Mathf.Min(maxPoints, currentPoints + 1);
    //}

    public bool TrySpend(int cost)
    {
        if (currentPoints < cost) return false;

        currentPoints -= cost;
        return true;
    }

    public void Refund(int amount)
    {
        currentPoints = Mathf.Min(currentPoints + amount, maxPoints);
    }


}