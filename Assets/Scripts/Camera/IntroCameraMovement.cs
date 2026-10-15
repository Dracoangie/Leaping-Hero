using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class IntroCameraMovement : MonoBehaviour
{
    [Tooltip("Maximum rotation in degrees around the local X and Y axes.")]
    [SerializeField] private Vector2 rotationAmplitude = new Vector2(0.5f, 0.5f);
    [Tooltip("Maximum gentle roll in degrees around the local Z axis.")]
    [SerializeField, Min(0f)] private float rollAmplitude = 0.08f;
    [Tooltip("Seconds to complete one circular movement.")]
    [SerializeField, Min(0.1f)] private float cycleDuration = 24f;
    [Tooltip("Seconds to gently introduce the movement when enabled.")]
    [SerializeField, Min(0f)] private float fadeInDuration = 2f;

    private Quaternion initialRotation;
    private float cycleTime;
    private float fadeTime;

    private void OnEnable()
    {
        initialRotation = transform.localRotation;
        cycleTime = 0f;
        fadeTime = 0f;
    }

    private void LateUpdate()
    {
        float duration = Mathf.Max(0.1f, cycleDuration);
        cycleTime = Mathf.Repeat(cycleTime + Time.unscaledDeltaTime, duration);
        fadeTime = Mathf.Min(fadeTime + Time.unscaledDeltaTime, fadeInDuration);
        float blend = fadeInDuration > 0f
            ? Mathf.SmoothStep(0f, 1f, fadeTime / fadeInDuration)
            : 1f;
        float angle = cycleTime / duration * Mathf.PI * 2f;
        float x = Mathf.Sin(angle) * rotationAmplitude.x * blend;
        float y = Mathf.Cos(angle) * rotationAmplitude.y * blend;
        float z = Mathf.Sin(angle + Mathf.PI * 0.25f) * rollAmplitude * blend;

        transform.localRotation = initialRotation * Quaternion.Euler(x, y, z);
    }

    private void OnDisable()
    {
        transform.localRotation = initialRotation;
    }
}
