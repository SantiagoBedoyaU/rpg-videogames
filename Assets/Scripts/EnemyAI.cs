using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private EnemyPathfinding enemyPathfinding;
    private Animator animator;
    public Transform player;

    [Header("Configuración de Combate")]
    public float stopDistance = 1.2f; // Distancia para empezar a atacar
    public float attackCooldown = 1.5f; // Tiempo entre golpes
    private bool isAttacking = false;

    private void Awake() {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>(); 
    }

    private void Start() {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) {
            player = playerObject.transform;
            StartCoroutine(ChasingRoutine()); 
        }
    }

    private IEnumerator ChasingRoutine() {
        while (player != null)
        {
            float distancia = Vector2.Distance(transform.position, player.position);
            
            // Girar siempre hacia el jugador
            GirarHaciaObjetivo(player.position);

            if (distancia > stopDistance) 
            {
                // PERSEGUIR
                enemyPathfinding.MoveTo(player.position);
                animator.SetBool("isRunning", true); 
            }
            else 
            {
                // DETENERSE Y ATACAR
                enemyPathfinding.MoveTo(transform.position); // Se queda quieto
                animator.SetBool("isRunning", false); 

                // Si no está en medio de un ataque, inicia uno nuevo
                if (!isAttacking) {
                    StartCoroutine(AttackRoutine());
                }
            }
            
            yield return new WaitForSeconds(0.1f); 
        }
    }

    private IEnumerator AttackRoutine() {
        isAttacking = true;
        
        // 1. Disparar el trigger en el Animator
        animator.SetTrigger("attack");

        // 2. Esperar el tiempo de enfriamiento (cooldown)
        yield return new WaitForSeconds(attackCooldown);
        
        isAttacking = false;
    }

    private void GirarHaciaObjetivo(Vector3 objetivo) {
        if (objetivo.x > transform.position.x && transform.localScale.x < 0) {
            Flip();
        } else if (objetivo.x < transform.position.x && transform.localScale.x > 0) {
            Flip();
        }
    }

    private void Flip() {
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    // Esta función será llamada por la animación
    public void CausarDano()
    {
        Debug.Log("1. El evento de animación se disparó correctamente"); // <--- AÑADE ESTO

        float distancia = Vector2.Distance(transform.position, player.position);
        
        if (distancia <= stopDistance + 0.5f) 
        {
            Debug.Log("2. El jugador está en rango de daño"); // <--- AÑADE ESTO
            PlayerHealth scriptSalud = player.GetComponent<PlayerHealth>();

            if (scriptSalud != null)
            {
                scriptSalud.TakeDamage(10f);
            }
        }
    }
}