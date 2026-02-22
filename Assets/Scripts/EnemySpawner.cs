using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración del Spawner")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnRate = 1f; // Tiempo en segundos entre cada aparición
    [SerializeField] private float spawnRadius = 10f; // Distancia a la que aparecen del jugador (fuera de cámara)

    private Transform player;

    private void Start()
    {
        // Buscamos al jugador al inicio, igual que hicimos con la IA del enemigo
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            StartCoroutine(SpawnRoutine()); // Iniciamos la creación de enemigos
        }
        else
        {
            Debug.LogWarning("Spawner: No se encontró al jugador.");
        }
    }

    private IEnumerator SpawnRoutine()
    {
        // Mientras el jugador esté vivo, sigue creando enemigos infinitamente
        while (player != null)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnRate); // Espera antes de crear el siguiente
        }
    }

    private void SpawnEnemy()
    {
        // Si por error la lista está vacía, no hacemos nada para evitar que el juego colapse
        if (enemyPrefabs.Length == 0) return;

        // 2. Elegimos un número al azar entre 0 y la cantidad de enemigos en la lista
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        
        // 3. Seleccionamos el enemigo correspondiente a ese número
        GameObject enemyToSpawn = enemyPrefabs[randomIndex];

        // 4. Calculamos la posición y lo creamos (igual que antes)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPosition = (Vector2)player.position + (randomDirection * spawnRadius);

        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }
}