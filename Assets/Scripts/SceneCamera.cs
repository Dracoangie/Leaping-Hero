using UnityEngine;

public class SceneCamera : MonoBehaviour
{
    public float FollowSpeed = 1.0f;
    public float yOffset = 1.0f;
    public Transform target;

    [Header("Zone Settings")]
    public float deathZoneY = -2f;
    public float highSpeedMultiplier = 2f;

    private float currentFollowSpeed;
    private bool inDeathZone = false;

    private void Awake()
    {
        transform.position = new(target.position.x, target.position.y + yOffset, -10f);
        currentFollowSpeed = FollowSpeed;
    }

    void FixedUpdate()
    {
        if (!inDeathZone && target.position.y < transform.position.y + deathZoneY)
        {
            currentFollowSpeed = FollowSpeed * highSpeedMultiplier;
            inDeathZone = true;
        }
        else if (inDeathZone && target.position.y > transform.position.y + deathZoneY)
        {
            currentFollowSpeed = FollowSpeed;
            inDeathZone = false;
        }

        Vector3 newPos = new(target.position.x, target.position.y + yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, currentFollowSpeed * Time.deltaTime);
    }
}
