using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DashSettings
{
    public float dashDistance;
    public float dashDuration;
    public float cooldownTime; //  대쉬 쿨타임 추가
    public bool dashTowardMouse;

    private float lastUsedTime;

    public DashSettings(float distance, float duration, float cooldown, bool towardMouse)
    {
        dashDistance = distance;
        dashDuration = duration;
        cooldownTime = cooldown;
        dashTowardMouse = towardMouse;
        lastUsedTime = -cooldown;
    }

    public bool IsReady()
    {
        return Time.time >= lastUsedTime + cooldownTime;
    }

    public void UseDash()
    {
        lastUsedTime = Time.time;
    }

    public float GetRemainingCooldown()
    {
        return Mathf.Max(0, (lastUsedTime + cooldownTime) - Time.time);
    }
}
