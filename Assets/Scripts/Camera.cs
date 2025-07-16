using UnityEngine;

public class Camera : MonoBehaviour
{
    public float FollowSpeed = 1.0f;
    public float yOffset = 1.0f;
    public Transform target;

    private void Awake()
    {
        transform.position = new(target.position.x, target.position.y + yOffset, -10f);
    }

    void FixedUpdate()
    {
        Vector3 newPos = new (target.position.x, target.position.y + yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed*Time.deltaTime);
    }
}
