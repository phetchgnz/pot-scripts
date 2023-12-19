using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    //======================= Move =======================//
    #region MoveVariables

    [Header("Move")]
    [SerializeField] private bool _isMoving = false;
    public bool isMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            anim.SetBool("isWalking", value);
        }
    }
    public bool CanMove
    {
        get
        {
            return anim.GetBool(AnimationStrings.canMove);
        }
    }

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                return speed;
            }
            else
            {
                return 0;
            }
        }
    }

    public bool LockVelocity
    {
        get
        {
            return anim.GetBool("lockVelocity");
        }
        set
        {
            anim.SetBool("lockVelocity", value);
        }
    }
    public Vector2 moveInput;
    [SerializeField]
    public float speed = 4f;
    public bool isFacingRight = true;
    public float Direction;

    #endregion


    //======================= Jump =======================//
    #region JumpVariables
    [Header("Jumping")]
    [SerializeField]
    public bool _isJumping = false;

    [SerializeField]
    public bool isJumping
    {
        get
        {
            return _isJumping;
        }
        private set
        {
            _isJumping = value;


        }
    }
    [SerializeField]
    private float jumpingPower = 17f;

    private int maxJumps = 2;
    private int jumpsRemaining;
    [SerializeField] private float jumpingAttackPowerDuration = 10f;

    private float jumpCoyoteTime;
    private float lastGroundedTime;
    private float jumpBufferTime;
    private float lastJumpTime;

    private int maxJumpATK = 3;



    #endregion


    //======================= Dash =======================//
    #region DashVariables

    [Header("Dashing")]
    [SerializeField] public float _dashingVelocity = 14f;
    [SerializeField] private float _dashingTime = 0.5f;
    public float lastDash;
    [SerializeField] private float cooldownDash = 3f;
    public bool _isDashing;
    public bool _canDash = true;

    #endregion


    //=====================//
    [Header("Knockback")]
    public float lastATK;
    [SerializeField] public float cooldownATK;
    [SerializeField]
    private int maxAttack = 3;

    //=====================//
    [Header("Animator")]
    private Animator anim;

    //=====================//
    [Header("Declare Other Script & another component")]
    private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private TrailRenderer _tr;

    //=====================//
    [Header("iFrames")] 
    [SerializeField] private float iFramesDuration;

    //=====================//
    [Header("Camera")]
    [SerializeField] private GameObject _cameraFollowGO;
    private CameraFollowObj _cameraFollowObj;
    private float _fallSpeedYDampingChangThreshold;

    //=====================//
    [Header("Audio")] 
    private AudioManager audiomanager;
    private PlayerSound playersound;
    private AudioSource audioSource;

    //=====================//
    [Header("Gravity")] 
    public float baseGravity = 2;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;
    private bool _isJunpingAttack = false;

    [Header("Refereces")]
    private HealthManager healthManager;
    private PlayerInput playerInput;



    //=============================== VOID START ===============================//
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        _cameraFollowObj = _cameraFollowGO.GetComponent<CameraFollowObj>();
        _tr = GetComponent<TrailRenderer>();
        _fallSpeedYDampingChangThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        audioSource = GetComponent<AudioSource>();
        healthManager = GetComponent<HealthManager>();
        playerInput = GetComponent<PlayerInput>();
    }




    //=============================== VOID Awake ===============================//
    private void Awake()
    {
        
    }

    //=============================== VOID UPDATE ===============================//
    void Update()
    {
        //======================= Move =======================//
        if (!LockVelocity)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }
        Gravity();


        //======================= Camera & Flip Direction =======================//

        //========= Flip ===========//
        if (!isFacingRight && moveInput.x > 0)
        {
            Flip();
        }
        else if (isFacingRight && moveInput.x < 0)
        {
            Flip();
        }

        //========= Camera ===========//
        //if we are falling past a certain speed threshold
        if (rb.velocity.y < _fallSpeedYDampingChangThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.lerpYDamping(true);
        }
        //if we are standing still or moving up
        if (rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.lerpYDamping(false);
        }


        //======================= Dashing =======================//
        if (_isDashing)
        {
            StartCoroutine(DelayATKdash());

            if (moveInput.x == -1)
            {
                StartCoroutine(DelayFacing());
                isFacingRight = false;
                rb.velocity = new Vector2(transform.localScale.x * _dashingVelocity, 0f);
                rb.velocity += Vector2.zero;
            }
            else if (moveInput.x == 1)
            {
                StartCoroutine(DelayFacing());
                isFacingRight = true;
                rb.velocity = new Vector2(transform.localScale.x * _dashingVelocity, 0f);
                rb.velocity += Vector2.zero;

            }
            else if (Isgrounded() && moveInput.x != -1 && moveInput.x != 1)
            {
                if (isFacingRight)
                {
                    StartCoroutine(DelayFacing());
                    rb.velocity = new Vector2(transform.localScale.x * _dashingVelocity, 0f);
                    rb.velocity += Vector2.zero;
                }
                else if (!isFacingRight)
                {
                    StartCoroutine(DelayFacing());
                    rb.velocity = new Vector2(transform.localScale.x * _dashingVelocity, 0f);
                    rb.velocity += Vector2.zero;
                }
            }

            if (isJumping)
            {
                if (isFacingRight)
                {
                    StartCoroutine(DelayFacing());
                    rb.velocity = new Vector2(transform.localScale.x * _dashingVelocity, 0f);
                    rb.velocity += Vector2.zero;
                }
                else if (!isFacingRight)
                {
                    StartCoroutine(DelayFacing());
                    rb.velocity = new Vector2(transform.localScale.x * _dashingVelocity, 0f);
                    rb.velocity += Vector2.zero;
                }
            }

            IEnumerator DelayFacing()
            {
                //playerInput.enabled = false;
                yield return new WaitForSeconds(0.257f);
                //playerInput.enabled = true;
            }

            IEnumerator DelayATKdash()
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    yield return new WaitForSeconds(0.11f);
                    if (moveInput.x == -1)
                    {
                        StartCoroutine(DelayFacing());
                        isFacingRight = false;
                        rb.velocity = new Vector2(transform.localScale.x * _dashingVelocity, 0f);

                    }
                    else if (moveInput.x == 1)
                    {
                        StartCoroutine(DelayFacing());
                        isFacingRight = true;
                        rb.velocity = new Vector2(transform.localScale.x * _dashingVelocity, 0f);

                    }
                }
            }
            
        }

        //======================= Grounded Check =======================//
        if (Isgrounded())
        {
            _canDash = true;
            anim.SetBool("isJumping", false);
            isJumping = false;
            jumpsRemaining = maxJumps;
            jumpsRemaining = maxJumpATK;
            lastGroundedTime = jumpCoyoteTime;
        }

        if (rb.velocity.x != 0 && Isgrounded())
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                audioSource.pitch = Random.Range(0.85f, 1f);
            }
        }
        

        lastGroundedTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
    }
    //END LINE UPDATE//




    //=============================== VOID FIXED UPDATE ===============================//
    private void FixedUpdate()
    {
   
        anim.SetFloat("yVelocity", rb.velocity.y);

    }




    //=============================== VOID Move Method ===============================//
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput != Vector2.zero;
    }




    //=============================== VOID & bool Jump Method ===============================//
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0 || lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                jumpsRemaining--;
                isJumping = true;
                anim.SetBool("isJumping", true);
                lastJumpTime = 0;

            }

            if (context.canceled && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                jumpsRemaining--;
                isJumping = true;
                anim.SetBool("isJumping", true);
                
            }
        }
    }

    public bool Isgrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Gravity()
    {
        if (!_isJunpingAttack)
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = baseGravity * fallSpeedMultiplier;
                rb.velocity = new Vector2(rb.velocity.x, MathF.Max(rb.velocity.y, -maxFallSpeed));
            }
            else
            {
                rb.gravityScale = baseGravity;
            }
        }
        
    }




    //=============================== VOID & bool Dash Method ===============================//
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_canDash)
            {
                if (Time.time - lastDash < cooldownDash)
                {
                    return;
                }
                lastDash = Time.time;
                StartCoroutine(_DashDash());
            }
        }
        
    }
    private IEnumerator _StopDashing()
    {
        yield return new WaitForSeconds(_dashingTime);
        _tr.emitting = false;
        _isDashing = false;

    }
    public IEnumerator _DashDash()
    {
        anim.SetTrigger("DashTrig");
        StartCoroutine(Invunerability());
        _isDashing = true;
        LockVelocity = true;
        _canDash = false;
        _tr.emitting = true;
        yield return new WaitForSeconds(_dashingTime);
        _tr.emitting = false;
        _isDashing = false;
        LockVelocity = false;
        _canDash = true;
    }
    public IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        Physics2D.IgnoreLayerCollision(7, 10, true);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreLayerCollision(7, 8, false);
        Physics2D.IgnoreLayerCollision(7, 10, false);
    }



    public IEnumerator JumpATK()
    {

        if (isJumping && maxAttack > 0)
        {
            _isJunpingAttack = true;
            _canDash = false;
            LockVelocity = true;
            rb.gravityScale = 0;
            rb.velocity = new Vector3(0, 0,0);
            //yield return new WaitForSeconds(1.5f);
            yield return new WaitForSeconds(jumpingAttackPowerDuration);
            rb.gravityScale = 3.5f;
            LockVelocity = false;
            maxAttack--;
            yield return new WaitForSeconds(1.5f);
            maxAttack = 3;
            yield return new WaitForSeconds(4f);
            _canDash = true;
            _isJunpingAttack = false;
        }

        else if (Isgrounded())
        {
            maxJumpATK = 3;
            StopCoroutine(JumpATK());
        }
    }



    //=============================== VOID Flip Camera Method ===============================//
    private void Flip()
    {
        if (!healthManager.CheckHealing)
        { 
            isFacingRight = !isFacingRight;

            if (isFacingRight)
            {
                transform.localScale = new Vector3(1, 1, 1);
                _cameraFollowObj.CallTurn();
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
                _cameraFollowObj.CallTurn();
            }
        }
        

    }
    public IEnumerator Knockback(float knockDur, float knockbackPwr, Vector3 knockbackDir)
    {

        float timer = 0;

        while (knockDur > timer)
        {
            timer += Time.deltaTime;
            if (isFacingRight)
            {
                rb.velocity = new Vector3(knockbackDir.x * -0.05f, knockbackDir.y * knockbackPwr, transform.position.z);
            }
            else if (!isFacingRight)
            {
                rb.velocity = new Vector3(knockbackDir.x * 0.05f , knockbackDir.y * knockbackPwr, transform.position.z);
            }
            anim.SetTrigger("hit");
            LockVelocity = true;
            _canDash = false;
            yield return new WaitForSeconds(0.2f);
            _canDash = true;
            LockVelocity = false;
        }

        yield return 0;
    }

    // Interact

    public LayerMask bonfireLayer;
    public void Interact(InputAction.CallbackContext context) {
        if (context.performed) {
            Collider2D colliders = Physics2D.OverlapCircle(transform.position, 1.5f,bonfireLayer);

            if (colliders != null) {
                if (colliders.CompareTag("Mary")) {
                    if (colliders.GetComponentInChildren<Checkpoint>().hasBeenActivated == false) {
                        
                        StartCoroutine(ShowBonfirePopUp());

                        IEnumerator ShowBonfirePopUp() {
                            UIManager.instance.ShowStatueActivated();
                            audiomanager.PlaySFX(audiomanager.StatueActivated);
                            colliders.GetComponentInChildren<Checkpoint>().targetIntensity = 1.25f;
                            colliders.GetComponentInChildren<Checkpoint>().hasBeenActivated = true;
                            PlayerPrefs.SetInt("Bonfire_" + colliders.name, 1);
                            PlayerPrefs.SetFloat("Bonfire_Light2D_" + colliders.name, colliders.GetComponentInChildren<Checkpoint>().targetIntensity = 1.25f);
                            PlayerPrefs.Save();
                            yield return new WaitForSeconds(3f);
                            // colliders.GetComponentInChildren<Checkpoint>().Interact();
                        }
                    } else {
                        colliders.GetComponentInChildren<Checkpoint>().Interact();
                    }
                }

                if (colliders.CompareTag("GreenCrystal")) {
                    Debug.Log("You pick up a green crystal");
                    Destroy(colliders.gameObject);
                    NewWeaponMood.instance.CollectGreenEmotion();
                }

                if (colliders.CompareTag("RedCrystal")) {
                    Debug.Log("You pick up a red crystal");
                    Destroy(colliders.gameObject);
                    NewWeaponMood.instance.CollectRedEmotion();
                }
            }
        }
    }

    // Active WeaponMood

    public NewWeaponMood newWeaponMood;
    public void ActiveWeaponMood(InputAction.CallbackContext context) {
        if (context.performed) {
            newWeaponMood.ActivateMoodSkill();
        }
    }
}