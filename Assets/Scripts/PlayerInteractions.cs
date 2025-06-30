using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private bool deathState = false;

    void Start()
    {

    }

    void Update()
    {
        if (deathState)
        {
            
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
            deathState = true;
        else
            deathState = false;
    }
    
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Rock")
        {
        }
    }*/
}
