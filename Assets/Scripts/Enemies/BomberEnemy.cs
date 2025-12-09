using UnityEngine;
using System.Collections;

public class BomberEnemy : Enemy
{
    public float detectionRange = 6f;
    public float followSpeed = 3f;
    public float proximityRange = 1.5f;
    public float countdownTime = 1.5f;

    private bool isCountingDown = false;

    protected override void Update()
    {
        if (player != null && currentState != EnemyState.Death && currentState != EnemyState.Victory)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange && currentState == EnemyState.Idle)
            {
                ChangeState(EnemyState.Action);
            }
        }

        base.Update();
    }

    protected override void HandleIdle()
    {
        rb.linearVelocity = Vector2.zero;
    }

    protected override void HandleAction()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > proximityRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * followSpeed;
            if (isCountingDown) 
            {
                StopAllCoroutines();
                isCountingDown = false;
                // **Añadir aquí lógica visual/sonora para indicar que la cuenta paró**
            }
        }
        else // Está lo suficientemente cerca
        {
            // 2. Detenerse y comenzar la cuenta atrás (si no lo ha hecho ya)
            rb.linearVelocity = Vector2.zero;

            if (!isCountingDown)
            {
                isCountingDown = true;
                StartCoroutine(CountdownToExplosion());
                // **Añadir aquí lógica visual/sonora para indicar el inicio de la cuenta (parpadeo, sonido)**
            }
        }
    }

    private IEnumerator CountdownToExplosion()
    {
        Debug.Log(gameObject.name + " ¡Empezando cuenta atrás para explotar!");

        yield return new WaitForSeconds(countdownTime);

        // Si sigue en estado Action (no lo han matado)
        if (currentState == EnemyState.Action)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // **Lógica de la explosión**
        Debug.Log(gameObject.name + " ¡BOOM!");
        
        // 1. Daño al jugador (necesitarás el script de vida del jugador)
        // Ejemplo: Comprobar colisión radial para afectar al jugador si está cerca

        // 2. Efectos visuales y sonoros (partículas, sonido)

        // 3. Destruir al enemigo llamando a Die()
        Die();
    }
}