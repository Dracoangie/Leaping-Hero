using UnityEngine;


public class PatrolEnemy : Enemy
{
    public float patrolSpeed = 2f;
    public Transform patrolPointA;
    public Transform patrolPointB;

    private Vector3 targetPoint;

    private enum MovementType
    {
        move,
        noMove
    }

    [SerializeField]
    private MovementType movementType = MovementType.move;

    protected override void Start()
    {
        base.Start();

        transform.position = patrolPointA.position;
        targetPoint = patrolPointA.position;
        if (patrolPointA.position == Vector3.zero && patrolPointB.position == Vector3.zero)
        {
            patrolPointA.position = transform.position;
            patrolPointB.position = transform.position + new Vector3(5, 0, 0);
            targetPoint = patrolPointB.position;
        }
    }

    protected override void HandleIdle()
    {
        base.HandleIdle();

        switch(movementType)
        {
            case MovementType.move:
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
                    if (targetPoint == patrolPointA.position)
                        targetPoint = patrolPointB.position;
                    else
                        targetPoint = patrolPointA.position;
                }
                break;
            case MovementType.noMove:
                break;
        }
    }

}