using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    #region Movement Variables;
    private float moveInput;

    public float movingSpeed;
    public float jumpForce;
    public float acceleration = 50f;
    public float deceleration = 50f;
    private bool jumpHeld = false;
    private bool grounded = true;
    #endregion

    #region Dash Variables
    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float dashTrailEmitTimer = 0f;

    [SerializeField]
    private float emitInterval = 0.02f;

    [HideInInspector]
    public bool isDashing = false;
    #endregion

    #region States
    [HideInInspector] private bool canMove = true;
    [HideInInspector] public bool canDoubleJump = false;
    [HideInInspector] public bool canDash = false;
    private bool hasDoubleJumped = false;
    private bool facingRight = false;
    private bool jumpBuffered = false;
    private bool jumpBufferedDuringDash = false;
    private bool isJumping = false;
    private bool isbufferJumping = false;
    private bool firstTime = true;
    #endregion

    #region References
    public LayerMask whatIsGround;
    [SerializeField] PlayerInfo playerInfo;
    private new Rigidbody2D rigidbody;
    private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Transform visualTransform;
    private Vector3 originalScale;
    
    #endregion

    #region Particles & Effects
    private ParticleSystem runParticles;
    private ParticleSystem runRParticles;
    private ParticleSystem runLParticles;
    private ParticleSystem landParticles;
    private TrailRenderer dashTrail;
    private ParticleSystem trailEndParticles;
    private ParticleSystem dashParticles;
    #endregion

    [HideInInspector]
    public Vector3 spawnPoint;

    #region Unity Methods
    void Start()
    {
        if (spawnPoint != null)
            transform.position = spawnPoint;

        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        visualTransform = transform.Find("Visual");
        originalScale = visualTransform.localScale;


        DialogueEvents.OnDialogueTriggered += StartDialogue;
        DialogueEvents.OnDialogueEnded += EndDialogue;

        runRParticles = transform.Find("Run").GetComponent<ParticleSystem>();
        runLParticles = transform.Find("Run_L").GetComponent<ParticleSystem>();
        runParticles = runRParticles;
        landParticles = transform.Find("Land").GetComponent<ParticleSystem>();
        dashTrail = transform.Find("Trail").GetComponent<TrailRenderer>();
        dashParticles = transform.Find("Dash").GetComponent<ParticleSystem>();
        trailEndParticles = transform.Find("TrailEnd").GetComponent<ParticleSystem>();
        dashTrail.emitting = false;
        canDoubleJump = false;

        if (playerInfo.spawnPoint != Vector3.zero)
            transform.position = playerInfo.spawnPoint;
    }

    void Update()
    {
        if (!canMove) return;

        if (!isDashing)
        {
            Move();
            Jump();
        }
        else if (jumpHeld)
        {
            jumpBufferedDuringDash = true;
        }

        HandleDashInput();
    }
    #endregion

    #region Movement
    void Move()
    {

        if (moveInput != 0)
        {
            float targetVelocityX = moveInput * movingSpeed;
            float smoothedVelocityX = Mathf.Lerp(rigidbody.linearVelocityX, targetVelocityX, acceleration * Time.deltaTime);
            rigidbody.linearVelocityX = smoothedVelocityX;

            animator.SetBool("isMoving", true);

            if (!runParticles.isPlaying && CheckGround())
                runParticles.Play();
        }
        else
        {
            animator.SetBool("isMoving", false);

            if (CheckGround())
            {
                float smoothedVelocityX = Mathf.MoveTowards(rigidbody.linearVelocityX, 0f, deceleration * Time.deltaTime);
                rigidbody.linearVelocityX = smoothedVelocityX;
            }

            if (runParticles.isPlaying)
                runParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (!facingRight && moveInput > 0 || facingRight && moveInput < 0)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !facingRight;

        if (runParticles.isPlaying)
        {
            runParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            runParticles = (runParticles == runRParticles) ? runLParticles : runRParticles;
            runParticles.Play();
        }
        else
        {
            runParticles = (runParticles == runRParticles) ? runLParticles : runRParticles;
        }
    }

    public bool GetCanMove()
    { return canMove; }
    public void SetCanMove(bool move)
    {
        canMove = move;
        if(!move){
            rigidbody.linearVelocity = Vector2.zero;
            if (runParticles.isPlaying)
                runParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            animator.SetBool("isMoving", false);
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {SetCanMove(false);}
    public void EndDialogue()
    { SetCanMove(true); }
    #endregion

    #region Jump & Double Jump
    void Jump()
    {
        grounded = CheckGround();
        if (grounded)
        {
            hasDoubleJumped = false;
            isJumping = false;
        }
        else if (runParticles.isPlaying)
        {
            runParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (jumpHeld)
        {
            if (grounded && !jumpBuffered && !isJumping && !isbufferJumping && !canDoubleJump && canMove)
            {
                jumpBuffered = true;
                isJumping = true;
                isbufferJumping = true;
                StartCoroutine(JumpWithAnticipation());
            }
        }
    }

    IEnumerator JumpWithAnticipation()
    {
        animator.Play("Player_StartJump", 0, 0f);
        yield return new WaitForSeconds(0.06f);
        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        isbufferJumping = false;
    }

    IEnumerator DoubleJump()
    {
        animator.Play("Player_DoubleJump", 0, 0f);
        yield return new WaitForSeconds(0.05f);
        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, 0f);
        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    #endregion

    #region Dash
    void HandleDashInput()
    {
        if (isDashing)
        {
            EmitTrailEndParticles();
            dashTrailEmitTimer += Time.deltaTime;

            if (dashTrailEmitTimer >= emitInterval)
            {
                dashTrailEmitTimer = 0f;
            }
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        canMove = false;

        float originalGravity = rigidbody.gravityScale;
        rigidbody.gravityScale = 0f;
        rigidbody.linearVelocity = new Vector2(0f, 0f);

        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 1);
        animator.Play("Player_Dash", 1, 0);
        yield return new WaitForSeconds(0.15f);

        canMove = true;
        dashTrail.emitting = true;

        float dashDirection = facingRight ? 1f : -1f;
        rigidbody.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        dashTrail.emitting = false;
        rigidbody.gravityScale = originalGravity;
        isDashing = false;
        rigidbody.linearVelocity = Vector2.zero;

        animator.SetLayerWeight(0, 1);
        animator.SetLayerWeight(1, 0);

        if (jumpBufferedDuringDash)
        {
            bool grounded = CheckGround();

            if (grounded)
            {
                isJumping = true;
                StartCoroutine(JumpWithAnticipation());
            }
            else if (!hasDoubleJumped && canDoubleJump)
            {
                hasDoubleJumped = true;
                isJumping = true;
                StartCoroutine(DoubleJump());
            }

            jumpBufferedDuringDash = false;
        }

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    void EmitTrailEndParticles()
    {
        if (dashTrail != null && trailEndParticles != null && dashTrail.positionCount >= 2)
        {
            Vector3 lastPoint = dashTrail.GetPosition(dashTrail.positionCount - 1);
            Vector3 secondLast = dashTrail.GetPosition(dashTrail.positionCount - 2);
            Vector3 direction = (lastPoint - secondLast).normalized;

            trailEndParticles.transform.SetPositionAndRotation(lastPoint, Quaternion.LookRotation(Vector3.forward, direction));
            trailEndParticles.Emit(1);
        }
    }
    #endregion

    #region Ground Check
    private bool CheckGround()
    {
        Vector2 origin = transform.position;
        Vector2 size = new (0.8f, 0.1f);
        float distance = 0.5f;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, distance, whatIsGround);
        bool isGrounded = hit.collider != null;

        if (isGrounded && !animator.GetBool("isGround") && !firstTime)
        {
            StartCoroutine(LandingStretch());

            if (landParticles != null)
            {
                landParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                landParticles.Play();
                landParticles.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            }
            if(moveInput == 0)
                rigidbody.linearVelocity = rigidbody.linearVelocity/2;
        }

        firstTime = false;
        animator.SetBool("isGround", isGrounded);
        return isGrounded;
    }
    #endregion

    #region Input System

    void OnMovement(InputValue inputValue)
    {
        moveInput = inputValue.Get<float>();
    }

    void OnJump(InputValue inputValue)
    {
        jumpHeld = inputValue.isPressed;

        if (jumpHeld)
        {
            if (grounded && !jumpBuffered && !isJumping && !isbufferJumping && canMove)
            {
                jumpBuffered = true;
                isJumping = true;
                StartCoroutine(JumpWithAnticipation());
            }
            else if (!grounded && canDoubleJump && !hasDoubleJumped && canMove)
            {
                hasDoubleJumped = true;
                isJumping = true;
                StartCoroutine(DoubleJump());
            }
        }
        else
        {
            jumpBufferedDuringDash = false;
            jumpBuffered = false;
            if (!grounded && !isJumping)
                animator.Play("Player_jump", 0, 0f);
        }
    }

    void OnDash(InputValue value)
    {
        if (canDash)
        {
            dashParticles.transform.position = transform.position;
            dashParticles.Play();
            StartCoroutine(Dash());
        }
    }

    #endregion

    #region Visual Stretch Effect
    IEnumerator LandingStretch()
    {
        float duration = 0.2f;
        float stretchX = 1.3f;
        float stretchY = 0.7f;

        Vector3 startScale = new (originalScale.x * stretchX, originalScale.y * stretchY, originalScale.z);
        float offsetY = (originalScale.y - startScale.y) / 2f;

        Vector3 originalPosition = visualTransform.localPosition;
        Vector3 startPosition = originalPosition + new Vector3(0f, -offsetY, 0f);

        visualTransform.localScale = startScale;
        visualTransform.localPosition = startPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            visualTransform.localScale = Vector3.Lerp(startScale, originalScale, t);
            visualTransform.localPosition = Vector3.Lerp(startPosition, originalPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        visualTransform.localScale = originalScale;
        visualTransform.localPosition = originalPosition;
    }
    #endregion

    public void dead()
    
    {
        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(2, 1);
        animator.Play("Player_Dead", 0, 0f);
        SetCanMove(false);
    }

    public void setPlayerInfo(PlayerInfo playerInfo)
        { this.playerInfo = playerInfo; }

    public PlayerInfo getPlayerInfo()
        { return playerInfo; }

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = transform.position;
        Vector2 size = new (0.8f, 0.1f);
        Vector2 castPoint = origin + Vector2.down * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(castPoint, size);
        
    }
    #endregion
}
