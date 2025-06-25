using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float movingSpeed;
    public float jumpForce;
    public float acceleration = 50f;
    public float deceleration = 50f;

    private bool facingRight = false;
    [HideInInspector]
    public bool deathState = false;
    private bool jumpBuffered = false;

    public LayerMask whatIsGround;
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private bool isJumping = false;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        move();
        jump();
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
        }
        else
        {
            animator.SetBool("isMoving", false);
            float smoothedVelocityX = Mathf.MoveTowards(rigidbody.linearVelocityX, 0f, deceleration * Time.deltaTime);
            rigidbody.linearVelocityX = smoothedVelocityX;
        }
        if (!facingRight && moveInput > 0)
            Flip();
        else if (facingRight && moveInput < 0)
            Flip();
    }

    void jump()
    {
        if (CheckGround())
        {
            isJumping = false;
        }
        if (CheckGround() && Input.GetKey(KeyCode.Space))
        {
            if (!jumpBuffered && !isJumping)
            {
                jumpBuffered = true;
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
        isJumping = true;
        animator.Play("Player_StartJump", 0, 0f);
        yield return new WaitForSeconds(0.08f);
        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    private bool CheckGround()
    {
        Vector2 origin = transform.position;
        Vector2 size = new Vector2(0.65f, 0.1f);
        float distance = 0.5f;
        bool isGrounded;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, distance, whatIsGround);
        isGrounded = hit.collider != null;
        animator.SetBool("isGround", isGrounded);
        return (isGrounded);
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
