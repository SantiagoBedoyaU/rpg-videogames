using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private EnemyPathfinding enemyPathfinding;
    private Animator animator;
    public Transform player;

    [Header("Configuración de Combate")]
    public float stopDistance = 0.5f; // Distancia a la que deja de correr

    private void Awake() {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        animator = GetComponent<Animator>(); // Inicializamos el Animator
    }

    private void Start() {
        // Al nacer, el enemigo busca automáticamente en el mapa quién tiene la etiqueta "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null) {
            player = playerObject.transform;
            StartCoroutine(ChasingRoutine()); // Inicia la persecución inmediatamente
        } else {
            Debug.LogWarning("¡Cuidado! No se encontró a nadie con el Tag 'Player'.");
        }
    }

    private IEnumerator ChasingRoutine() {
        // Ciclo infinito: mientras el jugador exista, lo persigue sin descanso
        while (player != null)
        {
            float distancia = Vector2.Distance(transform.position, player.position);

            if (distancia > stopDistance) 
            {
                // ESTADO: CORRIENDO
                enemyPathfinding.MoveTo(player.position);
                animator.SetBool("isRunning", true); // <--- ACTIVAR ANIMACIÓN
            }
            else 
            {
                // ESTADO: IDLE (Llegó al jugador)
                enemyPathfinding.MoveTo(transform.position); // Se queda donde está
                animator.SetBool("isRunning", false); // <--- DESACTIVAR ANIMACIÓN
            }
            
            yield return new WaitForSeconds(0.2f); 
        }
    }
}
