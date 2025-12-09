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

    protected virtual void Start()
    {
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
                HandleDeath();
                break;
            case EnemyState.Victory:
                break;
        }
    }

    protected virtual void HandleIdle()
    {
        // **Acción principal que hará siempre que no esté en otro estado**
        // Por defecto, podría ser solo esperar o una ligera animación.
        // Las clases hijas implementarán el movimiento, patrulla, etc.
        // Debug.Log(gameObject.name + " está en estado Idle.");
    }

    protected virtual void HandleAction()
    {
        // **Acción cuando el player está cerca**
        // Las clases hijas implementarán el ataque a distancia, la persecución, etc.
        // Debug.Log(gameObject.name + " está en estado Action.");
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
        Debug.Log(gameObject.name + " ha muerto.");
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
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
        
        rb.linearVelocity = Vector2.zero;
        Debug.Log(gameObject.name + " celebra la victoria.");
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }
}