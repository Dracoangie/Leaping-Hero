using UnityEngine;

public class RangedEnemy : Enemy
{
    public float detectionRange = 8f;
    public float attackRange = 6f;
    public float attackCooldown = 2f;
    public GameObject projectilePrefab;

    private float timeSinceLastAttack = 0f;

    protected override void Update()
    {

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange && currentState == EnemyState.Idle)
                ChangeState(EnemyState.Action);
            else if (distanceToPlayer > detectionRange && currentState == EnemyState.Action)
                ChangeState(EnemyState.Idle);
        }
        
        base.Update();

        timeSinceLastAttack += Time.deltaTime;
    }

    protected override void HandleIdle()
    {
        // Solo espera, o puedes añadir un ligero movimiento/animación de patrulla visual
        // La detección está en Update
    }

    protected override void HandleAction()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (Vector2.Distance(transform.position, player.position) <= attackRange && timeSinceLastAttack >= attackCooldown)
        {
            Attack();
            timeSinceLastAttack = 0f;
        }
    }

    private void Attack()
    {
        Debug.Log(gameObject.name + " dispara un proyectil.");
        // Instanciar el proyectil y darle una velocidad/dirección
        // **Asegúrate de que el 'projectilePrefab' está asignado en el Inspector.**
        
        // Calcular la dirección de disparo
        Vector3 fireDirection = (player.position - transform.position).normalized;
        
        // Crear el proyectil ligeramente por delante del enemigo
        GameObject projectile = Instantiate(projectilePrefab, transform.position + fireDirection * 0.5f, Quaternion.identity);
        
        // Asume que el proyectil tiene un componente 'Rigidbody2D' y un script 'Projectile'
        // Puedes añadir aquí la lógica para darle velocidad al proyectil.
        if (projectile.GetComponent<Rigidbody2D>() != null)
        {
            float projectileSpeed = 10f; 
            projectile.GetComponent<Rigidbody2D>().linearVelocity = fireDirection * projectileSpeed;
        }
    }
}