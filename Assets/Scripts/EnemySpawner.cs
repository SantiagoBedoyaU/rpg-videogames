using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración del Spawner")]
    [SerializeField] private GameObject enemyPrefab; // El enemigo que vamos a crear
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
        // 1. Obtenemos una dirección aleatoria (un punto en un círculo)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        
        // 2. Multiplicamos esa dirección por el radio y se la sumamos a la posición del jugador
        Vector2 spawnPosition = (Vector2)player.position + (randomDirection * spawnRadius);

        // 3. Creamos al enemigo en esa posición calculada
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}