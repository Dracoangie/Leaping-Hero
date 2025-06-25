using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movingSpeed;
    public float jumpForce;
    private float moveInput;

    private bool facingRight = false;
    [HideInInspector]
    public bool deathState = false;

    private bool isGrounded;
    public Transform groundCheck;
    public LayerMask whatIsGround;


    private new Rigidbody2D rigidbody;
    //private Animator animator;
//    private GameManager gameManager;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
//        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    void Update()
    {
        if (Input.GetButton("Horizontal"))
        {
            moveInput = Input.GetAxis("Horizontal");
            Vector3 direction = transform.right * moveInput;
            rigidbody.linearVelocity = new Vector2(moveInput * movingSpeed, rigidbody.linearVelocityY);
            //animator.SetInteger("playerState", 1);
        }
        //else
        //    if (isGrounded) animator.SetInteger("playerState", 0);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        //if (!isGrounded)
        //    animator.SetInteger("playerState", 2);

        if (facingRight == false && moveInput > 0)
            Flip();
        else if (facingRight == true && moveInput < 0)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    private void CheckGround()
    {
        Vector2 origin = transform.position;
        Vector2 size = new Vector2(0.65f, 0.1f);
        float distance = 0.5f;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, distance, whatIsGround);
        isGrounded = hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
            deathState = true;
        else
            deathState = false;
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
