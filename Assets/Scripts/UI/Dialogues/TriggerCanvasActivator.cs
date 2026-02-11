using UnityEngine;
using UnityEngine.InputSystem;

public class TriggerCanvasActivator : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueToPlay;
    [SerializeField] private GameObject panelToShow;
    [SerializeField] private bool dialogueAgain;
    private bool dialogueAgainAux;
    [HideInInspector] public bool showOnEnter = true;

    private bool onTrigger = false;
    private bool interPressed = false;

    private void Start()
    {
        dialogueAgainAux = true;
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

    void OnInter(InputValue value)
    {
        interPressed = value.isPressed;
        if (onTrigger && interPressed && dialogueAgainAux)
        {
            DialogueEvents.TriggerDialogue(dialogueToPlay);
            showOnEnter = false;
            panelToShow.SetActive(false);
            onTrigger = false;
            dialogueAgainAux = dialogueAgain;
        }
    }
}
