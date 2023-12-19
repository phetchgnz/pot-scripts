using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameFollowMonster : MonoBehaviour
{
    public Transform monsterTransform;
    private MiniBScript miniBScript;
    [SerializeField] Vector2 yOffset;

    void Start() {
        miniBScript = GameObject.FindWithTag("Mini Boss").GetComponent<MiniBScript>();
    }

    private void Update() {
        
        if (monsterTransform != null) {

            Vector2 newPosition = monsterTransform.position;
            newPosition.y += yOffset.y;
            transform.position = newPosition;
        }

        if (miniBScript._currentHealth <= 0) {
            Destroy(gameObject);
        }
    }
}
