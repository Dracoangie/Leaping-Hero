using UnityEngine;

public class PatrolEnemy : Enemy
{
    public float patrolSpeed = 2f;
    public Transform patrolPointA;
    public Transform patrolPointB;

    private Vector2 targetPos;
    private Vector2 worldPointA;
    private Vector2 worldPointB;

    private enum MovementType { move, noMove }

    [SerializeField]
    private MovementType movementType = MovementType.move;

    protected override void Start()
    {
        base.Start();

        worldPointA = patrolPointA.position;
        worldPointB = patrolPointB.position;

        if (Vector2.Distance(worldPointA, worldPointB) < 0.1f)
        {
            worldPointA = transform.position;
            worldPointB = (Vector2)transform.position + new Vector2(5, 0);
        }
        targetPos = worldPointB;
    }

    protected override void HandleIdle()
    {
        base.HandleIdle();

        switch (movementType)
        {
            case MovementType.move:
                MoveTowardsTarget();
                break;
            case MovementType.noMove:
                break;
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            targetPos = (targetPos == worldPointA) ? worldPointB : worldPointA;
            FlipTowardsTarget();
        }
    }

    private void FlipTowardsTarget()
    {
        float direction = targetPos.x - transform.position.x;
        if (direction > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
}