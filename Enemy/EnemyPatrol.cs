using System.Collections;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class EnemyPatrol : MonoBehaviour
{
    [Header("References")]
    public Transform currentPoint;
    public float waitTime;
    public Transform pointA;
    public Transform pointB;
    // public int currentPatrolPointIndex;
    // public Transform currentPatrolPoint;
    // public Transform[] patrolPoints;
    private Rigidbody2D rb;
    private Animator anim;
    public Transform playerTransform;
    public bool isChasing;
    public float chaseDistance;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPoint = pointB;

        // currentPatrolPointIndex = 0;
        // currentPatrolPoint = patrolPoints[currentPatrolPointIndex];

        // StartCoroutine(EnemyPatroling());
        // Monster walk animation here
    }

    private void Update()
    {

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < chaseDistance)
        {
            ChasePlayer();
        }
        else
        {
            StartCoroutine(EnemyPatroling());
        }

    }

    private IEnumerator EnemyPatroling()
    {
        isChasing = false;
        Vector2 direction = (currentPoint.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * patrolSpeed, 0);

        float distanceToTarget = Vector2.Distance(transform.position, currentPoint.position);
        if (distanceToTarget < .5f && currentPoint == pointB)
        {
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(waitTime);
            currentPoint = pointA;
        }
        else if (distanceToTarget < .5f && currentPoint == pointA)
        {
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(waitTime);
            currentPoint = pointB;
        }
    }

    private void ChasePlayer()
    {

        isChasing = true;
        if (transform.position.x < playerTransform.position.x)
        {
            rb.velocity = new Vector2(chaseSpeed, 0f);
        }
        else
        {
            rb.velocity = new Vector2(-chaseSpeed, 0f);
        }
    }


    #region Old Patrol
    // private IEnumerator EnemyPatroling()
    // {

    //     isChasing = false;
    //     Debug.Log("Can't find Player...");

    //     Vector2 direction = (currentPoint.position - transform.position).normalized;
    //     rb.velocity = new Vector2(direction.x * patrolSpeed, 0);

    //     float distanceToTarget = Vector2.Distance(transform.position, currentPoint.position);

    //     if (distanceToTarget < 0.5f)
    //     {
    //         rb.velocity = Vector2.zero;
    //         yield return new WaitForSeconds(4f);

    //         if (currentPoint == pointA)
    //         {

    //             rb.velocity = new Vector2(patrolSpeed, 0);
    //             currentPoint = pointB;
    //             Debug.Log(currentPoint.name);
    //         }
    //         else
    //         {

    //             rb.velocity = new Vector2(-patrolSpeed, 0);
    //             currentPoint = pointA;
    //             Debug.Log(currentPoint.name);
    //         }
    //     }




    //     // if (isChasing)
    //     // {

    //     //     Debug.Log("Chasing Player!");

    //     //     if (transform.position.x > playerTransform.position.x)
    //     //     {

    //     //         transform.position += Vector3.left * chaseSpeed * Time.deltaTime;
    //     //         // transform.position = new Vector2(transform.position.x * chaseSPeed * Time.deltaTime, 0); /* Same result as a top Vector3 */

    //     //     }
    //     //     else if (transform.position.x < playerTransform.position.x)
    //     //     {

    //     //         transform.position += Vector3.right * chaseSpeed * Time.deltaTime;
    //     //     }
    //     // }
    //     // else
    //     // {

    //     //     if (Vector2.Distance(transform.position, playerTransform.position) < chaseDistance)
    //     //     {

    //     //         isChasing = true;

    //     //     }
    //     //     else
    //     //     {

    //     //         isChasing = false;
    //     //         Debug.Log("Can't find Player...");

    //     //         Vector2 direction = (currentPoint.position - transform.position).normalized;
    //     //         rb.velocity = new Vector2(direction.x * patrolSpeed, 0);

    //     //         float distanceToTarget = Vector2.Distance(transform.position, currentPoint.position);

    //     //         if (distanceToTarget < 0.5f)
    //     //         {
    //     //             rb.velocity = Vector2.zero;
    //     //             yield return new WaitForSeconds(4f);

    //     //             if (currentPoint == pointA)
    //     //             {

    //     //                 rb.velocity = new Vector2(patrolSpeed, 0);
    //     //                 currentPoint = pointB;
    //     //                 Debug.Log(currentPoint.name);
    //     //             }
    //     //             else
    //     //             {

    //     //                 rb.velocity = new Vector2(-patrolSpeed, 0);
    //     //                 currentPoint = pointA;
    //     //                 Debug.Log(currentPoint.name);
    //     //             }
    //     //         }
    //     //     }



    //     //     yield return null;
    //     // }
    // }

    // private void ChasingPlayer()
    // {

    //     // if (Vector2.Distance(transform.position, playerTransform.position) < chaseDistance)
    //     // {

    //     //     isChasing = true;
    //     // }
    //     // else
    //     // {

    //     //     return;
    //     // }

    //     isChasing = true;

    //     Debug.Log("Chasing Player!");

    //     if (transform.position.x > playerTransform.position.x)
    //     {

    //         transform.position += Vector3.left * chaseSpeed * Time.deltaTime;
    //         // transform.position = new Vector2(transform.position.x * chaseSPeed * Time.deltaTime, 0); /* Same result as a top Vector3 */

    //     }
    //     else if (transform.position.x < playerTransform.position.x)
    //     {

    //         transform.position += Vector3.right * chaseSpeed * Time.deltaTime;
    //     }

    // }
    #endregion

    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere(pointA.position, .5f);
        Gizmos.DrawWireSphere(pointB.position, .5f);
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        //================= For Old Patrol Array=============

        // for (int i = 0; i < patrolPoints.Length; i++)
        // {   
        //     Gizmos.DrawWireSphere(patrolPoints[i].position, .5f);

        //     if (i < patrolPoints.Length - 1) {

        //         Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i+1].position);
        //     } else if (i == patrolPoints.Length - 1 && patrolPoints.Length > 1) {

        //         Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
        //     }
        // }
    }
}
