using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración del Spawner")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnRadius = 10f;

    [Header("Configuración de Oleadas")]
    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private int enemyIncreasePerWave = 2;
    [SerializeField] private float spawnDelayBetweenEnemies = 0.3f;

    [Header("Escalado de Dificultad")]
    [SerializeField] private float damageIncreasePerWave = 2f; // Daño extra por oleada
    [SerializeField] private float attackCooldownReductionPerWave = 0.1f; // Reducción de cooldown por oleada

    [Header("Configuración del Jefe Final")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private int bossFirstWave = 5;
    [SerializeField] private int bossWaveInterval = 5;

    private Transform player;
    private int currentWave = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            StartCoroutine(WaveRoutine());
        }
        else
        {
            Debug.LogWarning("Spawner: No se encontró al jugador.");
        }
    }

    private IEnumerator WaveRoutine()
    {
        while (player != null)
        {
            currentWave++;
            int enemiesToSpawn = baseEnemiesPerWave + (currentWave - 1) * enemyIncreasePerWave;

            bool isBossWave = bossPrefab != null && currentWave >= bossFirstWave && currentWave % bossWaveInterval == 0;

            if (isBossWave)
            {
                Debug.Log($"¡OLEADA {currentWave} - JEFE! ¡Ha aparecido el Dark Knight!");
                SpawnBoss();
                int bossWaveEnemies = Mathf.Max(1, enemiesToSpawn / 2);
                for (int i = 0; i < bossWaveEnemies; i++)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(spawnDelayBetweenEnemies);
                }
            }
            else
            {
                Debug.Log($"¡Oleada {currentWave}! Enemigos: {enemiesToSpawn}");
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(spawnDelayBetweenEnemies);
                }
            }

            yield return new WaitUntil(() => AreAllEnemiesDead());

            Debug.Log($"¡Oleada {currentWave} completada!");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || player == null) return;

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyToSpawn = enemyPrefabs[randomIndex];

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPosition = (Vector2)player.position + (randomDirection * spawnRadius);

        GameObject enemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        
        // Aplicar escalado de dificultad basado en la oleada actual
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.attackDamage += (currentWave - 1) * damageIncreasePerWave;
            enemyAI.attackCooldown = Mathf.Max(0.3f, enemyAI.attackCooldown - (currentWave - 1) * attackCooldownReductionPerWave);
        }
        
        aliveEnemies.Add(enemy);
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null || player == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPosition = (Vector2)player.position + (randomDirection * spawnRadius);

        GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        aliveEnemies.Add(boss);
    }

    private bool AreAllEnemiesDead()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
        return aliveEnemies.Count == 0;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(20, 20, 300, 50), $"Oleada: {currentWave}", style);
    }
}