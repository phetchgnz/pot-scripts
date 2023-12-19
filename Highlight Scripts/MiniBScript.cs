using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using System;

public class MiniBScript : MonoBehaviour
{
    [Header("Status")]
    public string _name;
    public float _hp;
    public float _atk; 
    public int _def;  
    public float _currentHealth;

    [Header("CheckStates")]
    protected bool wasHitted = false;

    [Header("References")]
    [SerializeField] EnemyHealthManager minibossHealthBar;
    public HealthManager _playerHealth;
    public PlayerScript _player;
    private Animator minibossAnimator;
    private SkeletonMecanim skeletonMecanim;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] TextMeshPro minibossName;
    [SerializeField] GameObject floatingTextPrefab;
    private Attack attack;
    private HealthManager healthManager;
    public GameObject ending;
    private Rigidbody2D rb;

    [Header("Gravity")]
    public float miniBoss_baseGravity = 2;
    public float miniBoss_maxFallSpeed = 25f;
    public float miniBoss_fallSpeedMultiplier = 2f;


    private void Start() {
        _currentHealth = _hp;
        skeletonMecanim = GetComponent<SkeletonMecanim>();
        minibossAnimator = GetComponent<Animator>();

        minibossHealthBar.UpdateHealthBar(_currentHealth, _hp);
        minibossName.text = _name;
        

        GameObject player = GameObject.Find("Player");
        healthManager = player.GetComponent<HealthManager>();
        attack = player.GetComponentInChildren<Attack>();
        rb = GetComponent<Rigidbody2D>();


    }

    private void Update() {
        if (_currentHealth <= 0) {
            Die();
        }

        if (Input.GetKeyDown(KeyCode.M)) {
           TakeDamage(200);
        }
        Gravity();
    }

    public void TakeDamage(float damage) 
    {
        

        _currentHealth -= damage;

        wasHitted = true;

        minibossHealthBar.UpdateHealthBar(_currentHealth, _hp);


        if (wasHitted && _currentHealth > (_hp / 2)) {
            StartCoroutine(Hurt());
        } else if (wasHitted && _currentHealth <= (_hp / 2)) {
            StartCoroutine(MoreHurt());
        }

        IEnumerator Hurt() {
            Color normalColor = skeletonMecanim.skeleton.GetColor();
            skeletonMecanim.skeleton.SetColor(Color.red);
            yield return new WaitForSeconds(.125f);
            skeletonMecanim.skeleton.SetColor(normalColor);
            wasHitted = false;
        }

        IEnumerator MoreHurt() {
            Color normalColor = skeletonMecanim.skeleton.GetColor();
            skeletonMecanim.skeleton.SetColor(Color.red);
            yield return new WaitForSeconds(.045f);
            skeletonMecanim.skeleton.SetColor(normalColor);
            yield return new WaitForSeconds(.045f);
            skeletonMecanim.skeleton.SetColor(Color.red);
            yield return new WaitForSeconds(.045f);
            skeletonMecanim.skeleton.SetColor(normalColor);
            wasHitted = false;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (attack.IsAlive && !attack.isInvincible)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                healthManager.TakeDamage(15f);
                playerMovement.StartCoroutine(playerMovement.Knockback(0.01f, 0.001f, playerMovement.transform.position));
            }
        }
        
    }

    public void Die() {

        StartCoroutine(Die());

        IEnumerator Die() 
        {
            this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            minibossAnimator.SetBool("isAlive", false);
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            this.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            ending.SetActive(true);
            yield return new WaitForSeconds(5f);
            Destroy(this.gameObject);

        }
    }

    public void ShowDamage(string text)
    {
        float minY = 1.5f;
        float maxY = 2f;
        float randomY = minY + UnityEngine.Random.value * (maxY - minY);

        float minX = -1f;
        float maxX = 1.5f;
        float randomX = minX + UnityEngine.Random.value * (maxX - minX);

        Vector3 offset = new Vector3(randomX, randomY, 0f);
        Vector3 spawnPosition = transform.position + offset;
        var prefab = Instantiate(floatingTextPrefab,spawnPosition,Quaternion.identity);

        prefab.transform.SetParent(null);

        prefab.GetComponent<TextMeshPro>().text = text;
        Debug.Log("Show Up Floating Text");
    }

    private void Gravity()
    { 
        if (rb.velocity.y < 0) 
        {
            rb.gravityScale = miniBoss_baseGravity * miniBoss_fallSpeedMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, MathF.Max(rb.velocity.y, -miniBoss_maxFallSpeed));
        }
        else
        {
            rb.gravityScale = miniBoss_baseGravity;
        }
            
    }
}
