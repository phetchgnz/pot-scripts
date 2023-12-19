using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using Spine.Unity;
using TMPro;

public class MonsterScript : MonoBehaviour
{
    [Header("Status")]
    public string _name;
    public float _hp;
    public float _atk; 
    public int _def;  
    public float _currentHealth;

    [Header("CheckStates")]
    public bool wasHitted = false;
    public bool isDead = false;

    [Header("References")]
    private EnemyHealthManager enemyHealthBar;
    public HealthManager _playerHealth;
    public PlayerScript _player;
    private Animator enemyAnimator;
    private SkeletonMecanim skeletonMecanim;
    private Rigidbody2D rb;
    public AudioManager audioManager;
    [SerializeField] PlayerMovement playerMovement;
    private Attack attack;
    private HealthManager healthManager;
    public bool AliveCheck = true;
    public GameObject itemDropGreen;
    public GameObject itemDropRed;

    [SerializeField] private GameObject floatingTextPrefab;

    [Header("Currency")]
    private int currencyAmount;


    //Methods
    private void Start() {
        _currentHealth = _hp;
        skeletonMecanim = GetComponent<SkeletonMecanim>();
        enemyAnimator = GetComponent<Animator>();
        enemyHealthBar = GetComponent<EnemyHealthManager>();
        attack = GetComponentInChildren<Attack>();
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.Find("Player");
        healthManager = player.GetComponent<HealthManager>();
        
        if (enemyHealthBar != null) {
            enemyHealthBar.UpdateHealthBar(_currentHealth,_hp);
        }

        currencyAmount = Random.Range(150,420);

    }


    private void Update() 
    {
        if (_currentHealth <= 0) {
            Die();
            isDead = true;
        } else isDead = false;
    }
    public virtual void TakeDamage(float _damage)
    {
        _currentHealth -= _damage;
        wasHitted = true;

        if (enemyHealthBar != null)
        {
            enemyHealthBar.UpdateHealthBar(_currentHealth, _hp);
        }


        enemyAnimator.SetTrigger("wasHitted");
        rb.velocity += Vector2.zero;

        if (wasHitted && _currentHealth > 50)
        {
            StartCoroutine(Hurt());
            rb.velocity += Vector2.zero;
        }
        else if (wasHitted && _currentHealth <= 50)
        {
            StartCoroutine(MoreHurt());
            rb.velocity += Vector2.zero;
        }

        IEnumerator Hurt()
        {
            Color normalColor = skeletonMecanim.skeleton.GetColor();
            skeletonMecanim.skeleton.SetColor(Color.red);
            yield return new WaitForSeconds(.125f);
            skeletonMecanim.skeleton.SetColor(normalColor);
            yield return new WaitForSeconds(.5f);
            wasHitted = false;
        }

        IEnumerator MoreHurt()
        {
            Color normalColor = skeletonMecanim.skeleton.GetColor();
            skeletonMecanim.skeleton.SetColor(Color.red);
            yield return new WaitForSeconds(.045f);
            skeletonMecanim.skeleton.SetColor(normalColor);
            yield return new WaitForSeconds(.045f);
            skeletonMecanim.skeleton.SetColor(Color.red);
            yield return new WaitForSeconds(.045f);
            skeletonMecanim.skeleton.SetColor(normalColor);
            yield return new WaitForSeconds(.5f);
            wasHitted = false;
        }
    }

    private void ItemDrop() {
        int rand = Random.Range(1, 60);

        if (rand >= 10 && rand <= 20) {
            Instantiate(itemDropGreen, transform.position + new Vector3(0,1,0), Quaternion.identity);
        }
        else if (rand >= 21 && rand <= 40) {
            Instantiate(itemDropRed, transform.position + new Vector3(0,1,0), Quaternion.identity);
        }

        Debug.Log(rand);
    }

    public virtual void Die() 
    {
        StartCoroutine(Die());

        IEnumerator Die()
        {
            AliveCheck = false;
            enemyAnimator.SetBool("isAlive", false);
            this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            this.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            yield return new WaitForSeconds(1f);
            ItemDrop();    
            CurrencyManager.Instance.addCurrency(currencyAmount);
            Destroy(this.gameObject);
        }
    }

    public void ShowDamage(string text)
    {
        if (_currentHealth > 0)
        {
            float minY = 1.5f;
            float maxY = 2f;
            float randomY = minY + UnityEngine.Random.value * (maxY - minY);

            float minX = -1f;
            float maxX = 1.5f;
            float randomX = minX + UnityEngine.Random.value * (maxX - minX);

            Vector3 offset = new Vector3(randomX, randomY, 0f);
            Vector3 spawnPosition = transform.position + offset;
            var prefab = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);

            prefab.transform.SetParent(null);

            prefab.GetComponent<TextMeshPro>().text = text;
        }
    }
}
