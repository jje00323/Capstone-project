using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyFSM : MonoBehaviour
{
    public Animator Animator { get; protected set; }
    public Rigidbody Rigidbody { get; protected set; }
    public Collider Collider { get; protected set; }

    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
    }

    public abstract void Die();
}
