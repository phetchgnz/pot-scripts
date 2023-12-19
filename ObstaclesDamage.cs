using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesDamage : MonoBehaviour
{
    [SerializeField] private int obstaclesDamage = 500;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.CompareTag("Player"))
        {
            other.collider.GetComponent<HealthManager>().TakeDamage(obstaclesDamage);
        }
    }
}
