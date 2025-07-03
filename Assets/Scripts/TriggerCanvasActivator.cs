using UnityEngine;

public class TriggerCanvasActivator : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueToPlay;
    [SerializeField] private GameObject panelToShow;
    [HideInInspector] public bool showOnEnter = true;

    private bool onTrigger = false;

    void Update()
    {
        if (onTrigger && Input.GetKeyDown(KeyCode.Space))
        {
            DialogueEvents.TriggerDialogue(dialogueToPlay);
            showOnEnter = false;
            panelToShow.SetActive(false);
            onTrigger = false;
        }
	}

	private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && showOnEnter)
        {
            if(showOnEnter)
                panelToShow.SetActive(true);
            onTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(showOnEnter)
                panelToShow.SetActive(false);
            onTrigger = false;
        }
    }
}
