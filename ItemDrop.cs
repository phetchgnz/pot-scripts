using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    public float dropForce = 9;
    public GameObject groundObject;
    public LayerMask groundLayer;
    public bool isGrounded = false;

    void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.up * dropForce, ForceMode2D.Impulse);

        boxCollider.isTrigger = true;
    }

    void Update() {
        if (IsGrounded()) {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
        } else {
            rb.constraints = RigidbodyConstraints2D.None;
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(groundObject.transform.position,0.1f,groundLayer);
    }
}
