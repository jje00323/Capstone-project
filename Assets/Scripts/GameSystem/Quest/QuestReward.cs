using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class QuestReward
{
    public int experience;
    public int gold;
    public List<ItemReward> items;
}

[System.Serializable]
public class ItemReward
{
    public ItemData item;
    public int amount;
}