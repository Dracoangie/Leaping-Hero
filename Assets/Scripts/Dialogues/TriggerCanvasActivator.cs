using UnityEngine;

public class TriggerCanvasActivator : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueToPlay;
    [SerializeField] private GameObject panelToShow;
    [SerializeField] private bool dialogueAgain;
    private bool dialogueAgainAux;
    [HideInInspector] public bool showOnEnter = true;

    private bool onTrigger = false;

    private void Start()
    {
        dialogueAgainAux = true;
    }

    void Update()
    {
        if (onTrigger && Input.GetKeyDown(KeyCode.E) && dialogueAgainAux)
        {
            DialogueEvents.TriggerDialogue(dialogueToPlay);
            showOnEnter = false;
            panelToShow.SetActive(false);
            onTrigger = false;
            dialogueAgainAux = dialogueAgain;
        }
	}

	private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
