using UnityEngine;
using static System.TimeZoneInfo;


public class SceneCamera : MonoBehaviour
{
    public enum CameraMode
    {
        FollowTarget,
        Fixed
    }
    public float transitionDuration = 0.5f;
    private float transitionTimer = 0f;

    [HideInInspector]
    public CameraMode mode = CameraMode.Fixed;

    [Header("Follow Settings")]
    public float FollowSpeed = 1.0f;
    public float camSize = 5.0f;
    public float yOffset = 1.0f;
    public Transform target;

    [Header("Zone Settings")]
    public float deathZoneY = -2f;
    public float highSpeedMultiplier = 2f;

    private Vector3 movePosition = new (0, 0, -10f);
    private float fixedSize = 5f;

    private bool isTransicioning = false;
    private float currentFollowSpeed;
    private bool inDeathZone = false;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        currentFollowSpeed = FollowSpeed;
        transform.position = new Vector3(-5, 3.5f, -10f); ;
        cam.orthographicSize = 6.0f;
    }

    void FixedUpdate()
    {
        if (isTransicioning == true)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);
            Vector3 atcPos = transform.position;
            float atcSize = cam.orthographicSize;


            if (mode == CameraMode.FollowTarget)
            {
                transform.position = FollowTargetUpdate();
            }
            else
                transform.position = Vector3.Lerp(atcPos, movePosition, t);
            cam.orthographicSize = Mathf.Lerp(atcSize, fixedSize, t);

            if (t >= 1f)
            {
                isTransicioning = false;
                transitionTimer = 0f;
            }
            return;
        }

        switch (mode)
        {
            case CameraMode.FollowTarget:
                transform.position = FollowTargetUpdate();
                break;

            case CameraMode.Fixed:
                break;
        }
    }

    Vector3 FollowTargetUpdate()
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

        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);

        float newX = Mathf.Lerp(currentPos.x, targetPos.x, FollowSpeed * Time.deltaTime);
        float newY = Mathf.Lerp(currentPos.y, targetPos.y, currentFollowSpeed * Time.deltaTime);

        return new Vector3(newX, newY, targetPos.z);
    }


    public void SetCameraFixed(Vector3 position, float size)
    {
        if (mode == CameraMode.Fixed) return;
        if(isTransicioning == true)
            transitionTimer = 0f;
        isTransicioning = true;
        mode = CameraMode.Fixed;
        movePosition = position;
        fixedSize = size;
    }

    public void SetCameraFollow()
    {
        if (mode == CameraMode.FollowTarget) return;
        if (isTransicioning == true)
            transitionTimer = 0f;
        isTransicioning = true;
        mode = CameraMode.FollowTarget;
        fixedSize = camSize;
    }

}