using System.Collections;
using Spine.Unity;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Unity.VisualScripting;
using UnityEngine.VFX;

public class HealthManager : MonoBehaviour
{
    #region Health Manager
    [Header("Health Manager")]
    public float healthAmount;
    private float maxHealth;
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    #endregion
    
    private float lerpSpeed = 3;

    #region Healing System
    [Header("Healing System")]
    [SerializeField] private int healAmount;
    public int maxHeals = 3;
    public int remainingHeals = 3;
    public float  healCooldown = 1.5f;
    public Image[] heartIcons;
    #endregion

    [Header("CheckStates")]
    private bool isAlive;
    // private bool wasHitted = false;
    [SerializeField] private bool isHealing = false;
    public bool CheckHealing = false;
    private bool isHurtCoroutineRunning = false;


    #region References
    [Header("Refereces for Test")]
    private Rigidbody2D rb;
    //public WeaponMood weaponMood;
    private Animator playerAnim;
    private PlayerMovement playerMovement;
    private PlayerScript playerScript;
    private SkeletonMecanim skeletonMecanim;
    private AudioManager audioManager;

    private VisualEffect heal;
    // public GameObject deadScene,gameplayScene;

    #endregion

    private Attack attack;


    public void Start () 
    {
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        maxHealth = healthAmount;
        attack = GetComponentInChildren<Attack>();
        playerAnim = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerScript = GetComponent<PlayerScript>();
        skeletonMecanim = GetComponentInChildren<SkeletonMecanim>();
        rb = GetComponent<Rigidbody2D>();


        healthAmount = playerScript._hp;
        lerpSpeed = 3f * Time.deltaTime;
        isAlive = true;

    }
    public void Update()
    {
        UpdateAllHud();

        if (healthAmount <= 0 && isAlive) {
            Die();
            GameManager.instance.deadCounts++;
            rb.velocity = Vector2.zero;
        }
    }

    public void Die() {
        isAlive = false;
        audioManager.DiedVFX(audioManager.playerDied);
        playerAnim.SetBool("isAlive", false);

        UIManager.instance.ShowDeadScene();

        
        PlayerPrefs.SetInt("CurrentCoins",Character.instance.currentCurrency -= Character.instance.currentCurrency / 2);

        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }
    public void TakeDamage(float damageTaken)
    {
        if (!isHurtCoroutineRunning) {

            isHurtCoroutineRunning = true;
            playerMovement._canDash = false;
            healthAmount -= damageTaken;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (healthAmount / 100f),lerpSpeed);
            
            if (healthAmount > 50) {
                StartCoroutine(Hurt());
            } else if (healthAmount <= 50) {
                StartCoroutine(MoreHurt());
            }
        }
        
        IEnumerator Hurt() {
            Color normalColor = skeletonMecanim.skeleton.GetColor();
            skeletonMecanim.skeleton.SetColor(Color.red);
            yield return new WaitForSeconds(.125f);
            skeletonMecanim.skeleton.SetColor(normalColor);
            isHurtCoroutineRunning = false;
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
            isHurtCoroutineRunning = false;
        }
    }


    /*-------------------------| Heal and SP Regen |-------------------------|*/

    public GameObject VFXheal;

    public void Heal(InputAction.CallbackContext context) {

        if (context.performed) {
            if (healthAmount >= 100 || remainingHeals <= 0) {
                return;
            }
            if (remainingHeals > 0 && !isHealing)
            {
                StartCoroutine(HealLockVelocity());
                audioManager.HealingVFX(audioManager.Healing);
                StartCoroutine(Heal());
                
                remainingHeals--;
                GameManager.instance.healCounts ++;
            }
        }
        
        IEnumerator Heal()
        {
            playerAnim.SetTrigger("Heal");
            isHealing = true;
            VFXheal.SetActive(true);
            yield return new WaitForSeconds(1f);
            VFXheal.SetActive(false);
            healthAmount += healAmount;
            healthAmount = Mathf.Clamp(healthAmount, 0, 100);
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (healthAmount / 100f), lerpSpeed);

            yield return new WaitForSeconds(healCooldown);
            isHealing = false;
            
        }

        IEnumerator HealLockVelocity()
        {
            playerMovement._canDash = false;
            CheckHealing = true;
            rb.velocity = Vector2.zero;
            playerMovement.LockVelocity = true;
            yield return new WaitForSeconds(2f);
            playerMovement.LockVelocity = false;
            CheckHealing = false;
            playerMovement._canDash = true;
        }
    }

    /*-------------------------| Text Update (for Dev) |-------------------------|*/

    public void UpdateHPText (int dmg, float health) {
        
        healthText.text = $"HP: {health.ToString()}";
        healthAmount -= dmg;
        healthBar.fillAmount = healthAmount / 100f;
    }

    public void UpdateAllHud() {
        healthText.text = $"HP: {healthAmount.ToString()}";
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (healthAmount / 100f), lerpSpeed);

        for (int i = 0; i < heartIcons.Length; i++) {
            heartIcons[i].color = i < remainingHeals ? Color.white : Color.gray;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(25);
            StartCoroutine(playerMovement.Knockback(0.02f, 0.001f, playerMovement.transform.position));

            attack.isInvincible = true;
        }
    }
}