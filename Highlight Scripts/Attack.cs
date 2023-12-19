using Spine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using System.Threading;

public class Attack : MonoBehaviour
{
    public float attackDamage;
    public Vector2 knockback = Vector2.zero;
    public AudioManager audiomanager;
    private PlayerScript playerscript;
    private Collider2D coll;
    private HealthManager _playerHealth;
    private Animator animator;
    public PlayerScript playerScript;
    private Rigidbody2D rb2d;
    public bool isHit = false;

    [Header("Ref From Other GameOBJ")] 
    public CameraShake cameraShake;

    [SerializeField]
    public bool _isAlive = true;
    [SerializeField]
    public bool isInvincible = false;

    public bool Hitting = false;

    private float timeSinceHit = 0;
    public float invincibilityTimer = 0.35f;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool("isAlive", value);
            Debug.Log("isAlive set" + value);
        }
    }

    public bool LockVelocity
    {
        get
        {
            return animator.GetBool("lockVelocity");
        }
        set
        {
            animator.SetBool("lockVelocity", value);
        }
    }

    private BoundingBoxFollower bounding;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        playerscript = GetComponent<PlayerScript>();
        animator = GetComponent<Animator>();
        _playerHealth = GetComponent<HealthManager>();
        bounding = GetComponent<BoundingBoxFollower>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTimer)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }
    


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsAlive && !isInvincible)
        {
            MonsterScript monster = collider.GetComponent<MonsterScript>();
            if (collider.gameObject.CompareTag("Enemy"))
            {
                monster.TakeDamage(attackDamage - monster._def);
                monster.ShowDamage((attackDamage - monster._def).ToString("0"));
                StartCoroutine(CheckHitting());

                
                audiomanager.PlaySFX(audiomanager.GotHit);
                isInvincible = true;
                
                if (monster._currentHealth <= 0) 
                {
                    GameManager.instance.killCounts++;
                    audiomanager.PlaySFX(audiomanager.MonsterDead);
                }
            }

            if (collider.gameObject.CompareTag("Dummy"))
            {
                monster.TakeDamage(attackDamage - monster._def);
                monster.ShowDamage((attackDamage - monster._def).ToString("0"));
                StartCoroutine(CheckHitting());


                audiomanager.PlaySFX(audiomanager.DummyHit);
                isInvincible = true;
            }


            if (collider.gameObject.CompareTag("Mini Boss"))
            {
                MiniBScript miniBoss = collider.GetComponent<MiniBScript>();
                miniBoss.TakeDamage(attackDamage - miniBoss._def);
                miniBoss.ShowDamage((attackDamage - miniBoss._def).ToString("0"));
                StartCoroutine(CheckHitting()); 

                
                audiomanager.PlaySFX(audiomanager.GotHit);

                isInvincible = true;
                
                if (miniBoss._currentHealth <= 0) 
                {
                    GameManager.instance.killCounts++;
                    audiomanager.PlaySFX(audiomanager.MonsterDead);
                }
            }
            

            if (collider.gameObject.CompareTag("Player"))
            {
                isHit = true;
                cameraShake.ShakeCamera();
                PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                collider.GetComponent<HealthManager>().TakeDamage(attackDamage - playerScript._def);
                StartCoroutine(playerMovement.Knockback(0.01f, 0.001f, playerMovement.transform.position));
                
                isInvincible = true;
                }
            }

    }

    private IEnumerator CheckHitting()
    {
        Hitting = true;
        yield return new WaitForSeconds(0.4f);
        Hitting = false;

    }

}
