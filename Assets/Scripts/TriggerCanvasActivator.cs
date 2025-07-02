using UnityEngine;

public class TriggerCanvasActivator : MonoBehaviour
{
    [SerializeField] private GameObject panelToShow;
    [SerializeField] private bool showOnEnter = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && showOnEnter)
            panelToShow.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            panelToShow.SetActive(false);
    }
}
