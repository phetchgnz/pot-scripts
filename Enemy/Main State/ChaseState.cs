using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    [Header("State Reference")]
    [SerializeField] AttackState attackState;
    [SerializeField] IdleState idleState;


    [Header("References")]
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform enemyTranform;
    [SerializeField] Animator enemyAnim;
    private MonsterScript monsterScript;
    private Rigidbody2D rb;


    [Header("ChaseState Properties")]
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseRange;
    public float attackRange;
    public bool isFacingRight;
    public bool isInAttackRange;


    void Start() {
        rb = GetComponentInParent<Rigidbody2D>();

        if (rb == null) {
            Debug.Log("RigidBody2D not found!");
        }

        monsterScript = GetComponentInParent<MonsterScript>();

        if (monsterScript == null) {
            Debug.Log("MonsterScript not found!");
        }
        
        isFacingRight = false;
    }
    
    void Update() {
        if (transform.position.x > playerTransform.position.x && isFacingRight) {
            Flip();
        } else if (transform.position.x < playerTransform.position.x&& !isFacingRight) {
            Flip();
        }
    }

    public override State RunCurrentState()
    {
        #region Handle Chase Method
        Chase();
        #endregion

        #region Handle Back to Idle Logic
        float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);   
        if (distanceToPlayer > chaseRange) {
            enemyAnim.SetBool("isWalking", false);
            return idleState;
        }
        #endregion

        #region Handle Switching To Next State
        if (isInAttackRange) {
            enemyAnim.SetBool("isWalking", false);
            return attackState;
        } else {
            return this;
        }
        #endregion
    }

    private void Chase() {
        if (!monsterScript.wasHitted)
        {
            //Declare new variable that contains target's location
            Vector2 target = new Vector2(playerTransform.position.x, rb.position.y);
            
            //Declare new variable that contains New Position to move
            Vector2 newPosition = Vector2.MoveTowards(rb.position,target, chaseSpeed * Time.fixedDeltaTime);
    
            //Move Object to New Position
            rb.MovePosition(newPosition);
            enemyAnim.SetBool("isWalking", true);
            
    
            float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);
    
            if (distanceToPlayer < attackRange) {
                isInAttackRange = true;
            } else {
                isInAttackRange = false;
            }
        } else {
            StopMoving();
            enemyAnim.SetBool("isWalking", false);
        }
    }

    private void StopMoving() {
        rb.velocity = new Vector2(0,0);
    }

    private void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 localScale = enemyTranform.localScale;
        localScale.x *= -1f;
        enemyTranform.localScale = localScale;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = isInAttackRange ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
