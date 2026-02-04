using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    private bool deathState = false;
    private bool isInChest;
    private int chestsOpened;
    private bool interPressed;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Chest":
                isInChest = true;
                break;
            case "Enemy":
                interDead(other);
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Chest":
                isInChest = false;
                break;
            default:
                break;
        }
    }

    private void ChestPowrUp()
    {
        chestsOpened++;
        switch (chestsOpened)
        {
            case 1:
                playerMovement.canDoubleJump = true;
                break;
            case 2:
                playerMovement.canDash = true;
                break;
            default:
                break;
        }
    }

    void interDead(Collider2D other)
    {
        if(!deathState && playerMovement.isDashing == false)
        {
            DeadEvent.PlayerDead();
            deathState = true;
            playerMovement.dead();
        }
        else if(!deathState && playerMovement.isDashing)
            other.GetComponent<Enemy>().Die();

    }

    void OnInter(InputValue value)
    {
        interPressed = value.isPressed;
        if (isInChest && interPressed)
            ChestPowrUp();
    }
}
