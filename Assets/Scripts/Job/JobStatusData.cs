using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewJobStat", menuName = "Job/Job Stat Data")]
public class JobStatusData : ScriptableObject
{
    public JobManager.JobType jobType;

    [Header("기본 스탯")]
    public float baseHP;
    public float baseMP;
    public float baseAttack;
    public float baseDefense;

    [Header("특수 스탯")]
    public float critical;
    public float critical_Damage;
    public float Someting_Else;


    [Header("레벨 당 증가량")]
    public float hpPerLevel;
    public float mpPerLevel;
    public float attackPerLevel;
    public float defensePerLevel;


   
}