using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    [Header("State Refernce")]
    [SerializeField] ChaseState chaseState;


    [Header("References")]
    [SerializeField] Transform playerTransform;
    private Rigidbody2D rb;


    [Header("IdleState Properties")]
    [SerializeField] float detectRadius;
    public bool canDetectPlayer;


    void Start() {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public override State RunCurrentState()
    {

        float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer < detectRadius) {
            canDetectPlayer = true;
        } else {
            canDetectPlayer = false;
        }

        if (canDetectPlayer) {
            return chaseState;
        } else {
            return this;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = canDetectPlayer ? Color.yellow : Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
