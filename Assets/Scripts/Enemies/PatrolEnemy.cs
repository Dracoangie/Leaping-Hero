using UnityEngine;

public class PatrolEnemy : Enemy
{
    public float patrolSpeed = 2f;
    public Vector3 patrolPointA;
    public Vector3 patrolPointB;

    private Vector3 targetPoint;

    protected override void Start()
    {
        base.Start();

        transform.position = patrolPointA;
        targetPoint = patrolPointA;
        if (patrolPointA == Vector3.zero && patrolPointB == Vector3.zero)
        {
            patrolPointA = transform.position;
            patrolPointB = transform.position + new Vector3(5, 0, 0);
            targetPoint = patrolPointB;
        }
    }

    protected override void HandleIdle()
    {
        base.HandleIdle();
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

}