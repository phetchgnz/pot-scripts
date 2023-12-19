using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EnemyBehavior : MonoBehaviour
{
    public Transform playerTransform;
    public Transform sightA, sightB;
    private Rigidbody2D rb;
    public LayerMask playerLayer;
    
    [Header("Chase Properties")]
    public float chaseSpeed = 6;
    public float attackRadius;
    public Transform attackRadiusTransform;
    public Vector3 startPoint;

    [Header("CheckStates")]
    public bool canDetect;
    // private bool isAttacking = false;
    private bool isFacingRight = false;

    [Header("References")]
    private Animator enemyAnim;


    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        startPoint = transform.position;
        enemyAnim = GetComponent<Animator>();
        startPoint = transform.position;
        canDetect = false;
    }
    private void Update() {
        
    }
    
    public virtual void BackToStartPoint() {
        float positionThreshold = 0.1f;

        if (Mathf.Abs(transform.position.x - startPoint.x) > positionThreshold)
        {

            if (transform.position.x < startPoint.x)
            {
                rb.velocity = new Vector2(chaseSpeed, 0f);
                isFacingRight = true;
                transform.localScale = new Vector2(-1f,1f);
                enemyAnim.SetBool("isWalking", true);
            }
            else if (transform.position.x > startPoint.x)
            {
                rb.velocity = new Vector2(-chaseSpeed, 0f);
                isFacingRight = false;
                transform.localScale = new Vector2(1f,1f);
                enemyAnim.SetBool("isWalking", true);
            }
        } else {
            isFacingRight = false;
            transform.localScale = new Vector2(1f,1f);
            enemyAnim.SetBool("isWalking", false);
            rb.velocity = Vector2.zero;

        }
    }
    public virtual void EnemySeeing() {
        
        Collider2D hitCollider = Physics2D.OverlapArea(sightA.position, sightB.position,playerLayer);
        if (hitCollider != null) {
            if (hitCollider.CompareTag("Player")) {
                canDetect = true;
            } 
        }else if (hitCollider == null) {
            canDetect = false;
        }
    }
    public virtual void ChasePlayer() 
    { 
        Vector2 target = new Vector2(playerTransform.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, chaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        enemyAnim.SetBool("isWalking", true);

        if (transform.position.x > playerTransform.position.x && isFacingRight) {
            Flip();
        } else if (transform.position.x < playerTransform.position.x && !isFacingRight) {
            Flip();
        }
    }
    public virtual void AttackPlayer() {
        rb.velocity = Vector2.zero;
        enemyAnim.SetTrigger("Attack");        
    }

    public virtual void OnDrawGizmos() {
        
        // Gizmos.DrawWireCube(LOS.position,new Vector2(6,2));
        CustomDebug.DrawRectange(sightB.position,sightA.position);
        // Gizmos.DrawLine(LOS.position, transform.position);
        Gizmos.DrawLine(sightA.position, sightB.position);
        Gizmos.DrawWireSphere(attackRadiusTransform.position,attackRadius);
    }

    private void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
     }
}
