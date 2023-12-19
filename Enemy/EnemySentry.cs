using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySentry : EnemyBehavior
{
    private void Update() {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        if (canDetect) {
            // Debug.Log("Player Detected!");

            if (distanceToPlayer > attackRadius) 
            {
                ChasePlayer();

            } else if (distanceToPlayer <= attackRadius) {
                AttackPlayer();
            }
        } else {
            BackToStartPoint();
        }
        // Debug.Log("Player Undetected");
        EnemySeeing();
    }

}