using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public float closeAttackRadius, rangeAttackRadius, detectRadius;
    public float chaseSpeed;
    public Transform closeAttackTransform, rangeAttackTransform, detectArea;
    private Rigidbody2D rb;
    private Animator bossAnim;
    private bool isChasing = false;
    [SerializeField] Transform player; // Reference to the player's transform

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        bossAnim = GetComponent<Animator>();
    }

    private void Update() {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectRadius) {
            isChasing = true;
        }
    

        if (distanceToPlayer <= rangeAttackRadius) {
            
            RangedAttack();
            Debug.Log("Range Attack");
        }
        if (distanceToPlayer <= closeAttackRadius) {
            CloseAttack();
            Debug.Log("Close Attack");
        } 

        if (isChasing == true) {
            MoveToPlayer();
        } else {
            bossAnim.SetBool("isWalking", false);
        }

        
        
    }

    private void MoveToPlayer() {
        if (transform.position.x > player.position.x) {
            transform.localScale = new Vector3(0.8411f,0.8411f,0.8411f);
        }
        if (transform.position.x < player.position.x) {
            transform.localScale = new Vector3(-0.8411f,0.8411f,0.8411f);
        }

        bossAnim.SetBool("isWalking", true);
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position,target, chaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        
    }

    private void StopMoving() {
        rb.velocity = new Vector2(0,0);
    }

    private void CloseAttack() {
        Debug.Log("Close Attack!");
        isChasing = false;

        bossAnim.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;
        bossAnim.SetTrigger("Attack");
    }

    private void RangedAttack() {
        
        Debug.Log("Ranged Attack!");
        isChasing = false;

        bossAnim.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;
        bossAnim.SetTrigger("DashAttack");
        rb.velocity = Vector2.left * 3f;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectArea.transform.position, detectRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(closeAttackTransform.position,closeAttackRadius);


        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(rangeAttackTransform.position,rangeAttackRadius);
    }
}
