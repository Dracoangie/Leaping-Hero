using UnityEngine;

public class PatrolEnemy : Enemy
{
    public float patrolSpeed = 2f;
    public Vector3 patrolPointA; 
    public Vector3 patrolPointB;
    public float detectionRange = 5f;

    private Vector3 targetPoint;

    protected override void Start()
    {
        base.Start();

        targetPoint = patrolPointA; 
        if (patrolPointA == Vector3.zero && patrolPointB == Vector3.zero)
        {
             patrolPointA = transform.position;
             patrolPointB = transform.position + new Vector3(5, 0, 0);
             targetPoint = patrolPointB;
        }
    }

    protected override void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            // **Solo camina**, no hace nada al detectar al player
            // Debug.Log(gameObject.name + " ha detectado al jugador, pero continúa patrullando.");
        }
        
        base.Update();
    }

    protected override void HandleIdle()
    {
        Vector3 direction = (targetPoint - transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        if (direction.x > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Vector2.Distance(transform.position, targetPoint) < 0.2f)
        {
            if (targetPoint == patrolPointA)
                targetPoint = patrolPointB;
            else
                targetPoint = patrolPointA;
        }
    }

    // Sobreescribe HandleAction para que no haga nada si se le llama accidentalmente
    protected override void HandleAction()
    {
        // Intencionalmente vacío. Si por alguna razón un cambio de estado externo lo pone en Action,
        // simplemente seguirá haciendo lo que hace en Idle. Podríamos hacer: ChangeState(EnemyState.Idle);
    }
}