using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Action,
        Death,
        Victory
    }

    protected EnemyState currentState = EnemyState.Idle;
    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator  animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        DeadEvent.OnPlayerDead += PlayerDied;
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("No player");
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;
            case EnemyState.Action:
                HandleAction();
                break;
            case EnemyState.Death:
                break;
            case EnemyState.Victory:
                break;
        }
    }

    protected virtual void HandleIdle()
    {
    }

    protected virtual void HandleAction()
    {
    }

    public virtual void Die()
    {
        if (currentState != EnemyState.Death)
        {
            currentState = EnemyState.Death;
            HandleDeath();
        }
    }

    protected virtual void HandleDeath()
    {
        animator.SetBool("isDead", true);
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
    }

    public virtual void PlayerDied()
    {
        if (currentState != EnemyState.Death)
        {
            currentState = EnemyState.Victory;
            HandleVictory();
        }
    }

    protected virtual void HandleVictory()
    {
        animator.SetBool("Win", true);
        rb.linearVelocity = Vector2.zero;
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }
}