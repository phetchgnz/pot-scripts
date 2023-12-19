using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    [Header("State Referencs")]
    [SerializeField] IdleState idleState;
    [SerializeField] ChaseState chaseState;


    [Header("References")]
    [SerializeField] Transform playerTransform;
    [SerializeField] Animator enemyAnim;
    private Rigidbody2D rb;


    [Header("AttackState Properties")]
    [SerializeField] float attackRange;
    [SerializeField] float waitTime;
    public bool isPlayerDead;
    public bool isPlayerInRange;


    void Start() {
        rb = GetComponentInParent<Rigidbody2D>();

        if (rb == null) {
            Debug.Log("RigidBody2D not found!");
        }

        attackRange = chaseState.attackRange;
    }

    public override State RunCurrentState()
    {
        #region Handle Attack Method
        Attack();
        #endregion
    
        if (isPlayerDead) {
            return idleState;
        } else if (!isPlayerInRange) {
            return chaseState;
        } else {
            return this;
        }
    }

    private void Attack() {
        float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);
        if (distanceToPlayer < attackRange) {
            enemyAnim.SetTrigger("Attack");
            isPlayerInRange = true;
        } else {
            StartCoroutine(Wait());
        }
        
        IEnumerator Wait() {
            yield return new WaitForSeconds(waitTime);
            isPlayerInRange = false;
        }
    }
}
