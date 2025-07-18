using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public bool switchToFixed = true;
    public Vector3 fixedPosition = new (0, 0, -10f);
    public float fixedSize = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneCamera sceneCam = Camera.main.GetComponent<SceneCamera>();
        if (sceneCam == null) return;

        if (switchToFixed)
        {
            sceneCam.SetCameraFixed(fixedPosition, fixedSize);
        }
        else
        {
            sceneCam.SetCameraFollow();
        }
    }
}
