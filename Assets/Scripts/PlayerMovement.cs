using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    #region Movement Variables
    public float movingSpeed;
    public float jumpForce;
    public float acceleration = 50f;
    public float deceleration = 50f;
    #endregion

    #region Dash Variables
    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float dashTrailEmitTimer = 0f;

    [SerializeField]
    private float emitInterval = 0.02f;

    private bool isDashing = false;
    private bool canDash = true;
    #endregion

    #region States
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canDoubleJump = true;
    private bool hasDoubleJumped = false;
    private bool facingRight = false;
    private bool jumpBuffered = false;
    private bool jumpBufferedDuringDash = false;
    private bool isJumping = false;
    private bool firstTime = true;
    #endregion

    #region References
    public LayerMask whatIsGround;
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

    #region Unity Methods
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        visualTransform = transform.Find("Visual");
        originalScale = visualTransform.localScale;

        runRParticles = transform.Find("Run").GetComponent<ParticleSystem>();
        runLParticles = transform.Find("Run_L").GetComponent<ParticleSystem>();
        runParticles = runRParticles;
        landParticles = transform.Find("Land").GetComponent<ParticleSystem>();
        dashTrail = transform.Find("Trail").GetComponent<TrailRenderer>();
        dashParticles = transform.Find("Dash").GetComponent<ParticleSystem>();
        trailEndParticles = transform.Find("TrailEnd").GetComponent<ParticleSystem>();
        dashTrail.emitting = false;
        canDoubleJump = true;
    }

    void Update()
    {
        if (!canMove) return;

        if (!isDashing)
        {
            Move();
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferedDuringDash = true;

        HandleDashInput();
    }
    #endregion

    #region Movement
    void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

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
    #endregion

    #region Jump & Double Jump
    void Jump()
    {
        bool grounded = CheckGround();

        if (grounded)
        {
            isJumping = false;
            hasDoubleJumped = false;
        }
        else if (runParticles.isPlaying)
        {
            runParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded && !jumpBuffered && !isJumping)
            {
                jumpBuffered = true;
                isJumping = true;
                StartCoroutine(JumpWithAnticipation());
            }
            else if (!grounded && canDoubleJump && !hasDoubleJumped)
            {
                hasDoubleJumped = true;
                isJumping = true;
                StartCoroutine(DoubleJump());
            }
        }
        else if (!Input.GetKey(KeyCode.Space))
        {
            jumpBufferedDuringDash = false;
            jumpBuffered = false;
            if (!grounded && !isJumping)
                animator.Play("Player_jump", 0, 0f);
        }
    }

    IEnumerator JumpWithAnticipation()
    {
        animator.Play("Player_StartJump", 0, 0f);
        yield return new WaitForSeconds(0.06f);
        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

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

        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 1);

        float originalGravity = rigidbody.gravityScale;
        rigidbody.gravityScale = 0f;

        dashTrail.emitting = true;
        dashParticles.transform.position = transform.position;
        dashParticles.Play();

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

            trailEndParticles.transform.position = lastPoint;
            trailEndParticles.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            trailEndParticles.Emit(1);
        }
    }
    #endregion

    #region Ground Check
    private bool CheckGround()
    {
        Vector2 origin = transform.position;
        Vector2 size = new Vector2(0.8f, 0.1f);
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
        }

        firstTime = false;
        animator.SetBool("isGround", isGrounded);
        return isGrounded;
    }
    #endregion

    #region Visual Stretch Effect
    IEnumerator LandingStretch()
    {
        float duration = 0.2f;
        float stretchX = 1.3f;
        float stretchY = 0.7f;

        Vector3 startScale = new Vector3(originalScale.x * stretchX, originalScale.y * stretchY, originalScale.z);
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

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = transform.position;
        Vector2 size = new Vector2(0.8f, 0.1f);
        Vector2 castPoint = origin + Vector2.down * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(castPoint, size);
    }
    #endregion
}
