using UnityEngine;

public class Chest : MonoBehaviour
{

    private bool dialogueAgainAux;
    private bool onTrigger = false;
    private Animator animator;

    private void Start()
    {
        dialogueAgainAux = true;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (onTrigger && Input.GetKeyDown(KeyCode.E) && dialogueAgainAux)
        {
            onTrigger = false;
            dialogueAgainAux = false;
            animator.Play("ChestOpen");
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            onTrigger = true;
    }
}
