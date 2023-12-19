using System.Numerics;
using System.Net.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Spine;
using UnityEngine.VFX;

public class PlayerScript : MonoBehaviour
{
    #region Status
    [Header("Status")]
    public string _name;
    public int _hp;
    public float _atk;
    public int _def;
    #endregion

    #region Attack System
    [Header("Attack System")]
    public float lastATK;
    public float cooldownATK;
    [SerializeField] float littleDash;
    public float baseAttack;
    #endregion

    #region References
    [Header("References")]
    private HealthManager healthManager;
    private Animator playerAnim;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private AudioManager audiomanager;
    private Checkpoint checkpoint;
    private PlayerInput playerInput;
    #endregion

    #region InputActions
    public PlayerController playerController;
    private InputAction attack;
    #endregion

    #region CheckStates
    private bool isPause;
    #endregion

    //Atack Animation Test
    public List<AttackSO> combo;
    private float lastclickedTime;
    private float lastComboEnd;
    public int comboCounter;
    [SerializeField] private bool attackingDuration;
    private float attackTime = 0.4f;
    private float lastAttackTime = 0f;
    [SerializeField] Attack attackScript;
    [SerializeField] LayerMask interactableLayer;

    [Header("Upgrade Points (ห้ามแก้ไข)")]
    public float attackUpgradePoint;
    public float defenseUpgradePoint;
    public float attackUpgradedPointPlus;
    public float defenseUpgradedPointPlus;
    public bool isAttackPointUpgraded = false;
    public bool isDefensePointUpgraded = false;
    

    private void Awake() {

        playerController = new PlayerController();

        //Set bool for Upgrade variables
        if (PlayerPrefs.HasKey("IsAttackUpgraded") && isAttackPointUpgraded == false) {
            isAttackPointUpgraded = true;
        } else if (!PlayerPrefs.HasKey("IsAttackUpgraded") && isAttackPointUpgraded == true) {
            isAttackPointUpgraded = false;
        }

        if (PlayerPrefs.HasKey("IsDefenseUpgraded") && isDefensePointUpgraded == false) {
            isDefensePointUpgraded = true;
        } else if (!PlayerPrefs.HasKey("IsDefenseUpgraded") && isDefensePointUpgraded == false) {
            isDefensePointUpgraded = false; 
        }

        //Get Attack Upgraded Point form Upgrade Scene
        if (isAttackPointUpgraded) {
            if (PlayerPrefs.HasKey("UpgradedAttackPoint")) {
                attackUpgradePoint = PlayerPrefs.GetFloat("UpgradedAttackPoint");
                GameManager.instance.attackUpgradePoint += attackUpgradePoint;
                Debug.Log("Your Attack Upgrade Point is: " + GameManager.instance.attackUpgradePoint);
            } else {
                Debug.Log("No Attack Upgraded Point");
            }
        }

        //Get Defense Upgraded Point form Upgrade Scene
        if (isDefensePointUpgraded) {
            if (PlayerPrefs.HasKey("UpgradedDefensePoint")) {
                defenseUpgradePoint = PlayerPrefs.GetFloat("UpgradedDefensePoint");
                GameManager.instance.defenseUpgradePoint += defenseUpgradePoint;
                Debug.Log("Your Defense Upgrade Point is: " + GameManager.instance.defenseUpgradePoint);
            } else {
                Debug.Log("No Defense Upgraded Point");
            }
        }
    }

    private void Start() 
    {
        GameObject statemood = GameObject.Find("Weapon Mood");

        playerAnim = GetComponentInChildren<Animator>();
        transform.position = GameManager.instance._spawnPoint;
        playerMovement = GetComponent<PlayerMovement>();
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        rb = GetComponent<Rigidbody2D>();

        checkpoint = GameObject.FindGameObjectWithTag("Mary").GetComponent<Checkpoint>();

        baseAttack = 30;

        _def += (int)defenseUpgradedPointPlus;

        playerInput = GetComponent<PlayerInput>();
    }

    private void Update() 
    {
        ExitAttack();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        lastAttackTime = Time.time;

        if (!playerMovement._isDashing)
        {

            if (context.started)
            {
                StartCoroutine(playerMovement.JumpATK());
                if (Time.time - lastATK < cooldownATK)
                {
                    return;
                }
                lastATK = Time.time;
                Attacking();
            }
        }
    }

    void Attacking()
    {
        if (Time.time - lastComboEnd >= 0.05f && comboCounter <= combo.Count)
        {
            CancelInvoke("EndComBo");

            if (Time.time - lastclickedTime >= 0.08f)
            {
                StartCoroutine(LittleDash());
                StartCoroutine(LockVelocityTime());
                playerAnim.runtimeAnimatorController = combo[comboCounter].animatorOV;
                playerAnim.Play("Attack", 0, 0.1f);
                audiomanager.PlaySFX(audiomanager.Attack2);
                attackScript.attackDamage = combo[comboCounter].Damage + GameManager.instance.attackUpgradePoint + NewWeaponMood.instance.bonusAttack;
                baseAttack = combo[0].Damage;
                comboCounter++;
                lastclickedTime = Time.time;
                if (comboCounter > combo.Count)
                {
                    comboCounter = 0;
                }
                StartCoroutine(EndcomBoBo());
            }
        }

    }
    IEnumerator LockVelocityTime()
    {
        if (playerMovement.Isgrounded())
        {
            playerMovement.LockVelocity = true;
            yield return new WaitForSeconds(0.485f);
            playerMovement.LockVelocity = false;
        }
    }

    void ExitAttack()
    {
        if (comboCounter == 1)
        {
            StartCoroutine(LockVelocityTime());
            if (Time.time - lastAttackTime > attackTime)
            {
                EndCombo();
            }
        }
        else if (comboCounter == 2)
        {
            StartCoroutine(LockVelocityTime());
            if (Time.time - lastAttackTime > attackTime)
            {
                EndCombo();
            }
        }
        else if (comboCounter == 3)
        {
            StartCoroutine(LockVelocityTime());
            StartCoroutine(Combo3CD());
            if (Time.time - lastAttackTime > attackTime)
            {
                EndCombo();
            }

            IEnumerator Combo3CD()
            {
                playerInput.enabled = false;
                yield return new WaitForSeconds(0.75f);
                playerInput.enabled = true;
            }
        }
    }

    IEnumerator EndcomBoBo()
    {
        if (comboCounter >= 3)
        {
            yield return new WaitForSeconds(0.3f);
            EndCombo();
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    IEnumerator LittleDash()
    {
        if (!playerMovement._isDashing && !playerMovement.isJumping)
        {
            yield return new WaitForSeconds(0.12f);
            playerMovement.LockVelocity = false;
            if (playerMovement.isFacingRight && playerMovement.moveInput.x == 1)
            {
                rb.velocity = new Vector2(littleDash, rb.velocity.y);
            }
            else if (!playerMovement.isFacingRight && playerMovement.moveInput.x == -1)
            {
                rb.velocity = new Vector2(-littleDash, rb.velocity.y);

            }
            yield return new WaitForSeconds(0.15f);
            rb.velocity = Vector2.zero;
            playerMovement.LockVelocity = true;
            StopCoroutine(LittleDash());
        }
    }

    IEnumerator stopInputForABit()
    {
        playerInput.actions.FindAction("Move").Disable();
        yield return new WaitForSeconds(0.2f);
        playerInput.actions.FindAction("Move").Enable();
    }
}
