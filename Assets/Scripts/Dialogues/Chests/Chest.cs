using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            onTrigger = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            onTrigger = false;
    }

    void OnInter(InputValue value)
    {
        if (onTrigger && dialogueAgainAux)
        {
            onTrigger = false;
            dialogueAgainAux = false;
            animator.Play("ChestOpen");
        }
    }
}
