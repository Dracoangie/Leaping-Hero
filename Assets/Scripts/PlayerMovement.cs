using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    public float movingSpeed;
    public float jumpForce;
    public float acceleration = 50f;
    public float deceleration = 50f;

    [HideInInspector]
    public bool canMove = true;

    private bool facingRight = false;
    [HideInInspector]
    public bool deathState = false;
    private bool jumpBuffered = false;

    public LayerMask whatIsGround;
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private bool isJumping = false;
    private bool firstTime = true;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;

    private ParticleSystem runParticles;
    private ParticleSystem runRParticles;
    private ParticleSystem runLParticles;
    private ParticleSystem landParticles;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        runRParticles = transform.Find("Run").GetComponent<ParticleSystem>();
        runLParticles = transform.Find("Run_L").GetComponent<ParticleSystem>();
        runParticles = runRParticles;
        landParticles = transform.Find("Land").GetComponent<ParticleSystem>();

        originalScale = transform.localScale;
    }

    void Update()
    {
        if (canMove)
        {
            move();
            jump();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
            deathState = true;
        else
            deathState = false;
    }

    void move()
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
            float smoothedVelocityX = Mathf.MoveTowards(rigidbody.linearVelocityX, 0f, deceleration * Time.deltaTime);
            rigidbody.linearVelocityX = smoothedVelocityX;

            if (runParticles.isPlaying)
                runParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        if (!facingRight && moveInput > 0)
            Flip();
        else if (facingRight && moveInput < 0)
            Flip();
    }

    void jump()
    {
        if (CheckGround())
            isJumping = false;
        else if (runParticles.isPlaying)
                runParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (CheckGround() && Input.GetKey(KeyCode.Space))
        {
            if (!jumpBuffered && !isJumping)
            {
                jumpBuffered = true;
                isJumping = true;
                StartCoroutine(JumpWithAnticipation());
            }
        }
        else if (!Input.GetKey(KeyCode.Space))
        {
            jumpBuffered = false;
        }
    }

    IEnumerator JumpWithAnticipation()
    {
        animator.Play("Player_StartJump", 0, 0f);
        yield return new WaitForSeconds(0.06f);
        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !facingRight;
        if (runParticles.isPlaying)
        {
            runParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            if (runParticles == runRParticles)
                runParticles = runLParticles;
            else
                runParticles = runRParticles;
            runParticles.Play();
        }
        else
        {
            if (runParticles == runRParticles)
                runParticles = runLParticles;
            else
                runParticles = runRParticles;
        }
    }

    private bool CheckGround()
    {
        Vector2 origin = transform.position;
        Vector2 size = new Vector2(0.65f, 0.1f);
        float distance = 0.5f;
        bool isGrounded;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, distance, whatIsGround);
        isGrounded = hit.collider != null;
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
        return (isGrounded);
    }

    IEnumerator LandingStretch()
    {
        float duration = 0.2f;
        float stretchX = 1.3f;
        float stretchY = 0.7f;

        Vector3 startScale = new Vector3(
            originalScale.x * stretchX,
            originalScale.y * stretchY,
            originalScale.z
        );

        transform.localScale = startScale;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 origin = transform.position;
        Vector2 size = new Vector2(0.65f, 0.1f);
        Vector2 castPoint = origin + Vector2.down * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(castPoint, size);
    }

    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Rock")
        {
        }
    }*/
}
