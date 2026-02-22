using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private EnemyPathfinding enemyPathfinding;
    public Transform player;
    public float chaseDistance = 5f;

    private void Awake() {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
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
            enemyPathfinding.MoveTo(player.position);
            
            // Actualiza la ruta cada 0.2 segundos. 
            // Esto es súper importante en juegos de oleadas para ahorrar memoria 
            // cuando tengas 50 o 100 enemigos en pantalla al mismo tiempo.
            yield return new WaitForSeconds(0.2f); 
        }
    }
}
