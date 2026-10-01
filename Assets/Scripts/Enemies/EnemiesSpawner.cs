using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemiesSpawner : MonoBehaviour
{
    private const int MaxSpawnPositionAttempts = 30;

    [Header("EnemiesSpawner")]
    public float spawnCooldown = 5;
    [SerializeField] private float minSpawnCooldown = 1f;
    [SerializeField] private float spawnCooldownDecayPerSecond = 0.01f;
    [FormerlySerializedAs("enemyPrefab")]
    public GameObject[] enemyPrefabs;
    public Transform player;
    public float spawnDistance = 10f;
    [SerializeField] private float spawnAreaHalfSize = 60f;
    [SerializeField] private float navMeshSearchRadius = 5f;
    [SerializeField] private int maxAliveEnemies = 40;

    public event Action EnemyKilled;

    private PlayerHealth playerHealth;
    private Coroutine spawnRoutine;
    private int aliveEnemyCount;

    void Start()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.Died += StopSpawning;
        spawnRoutine = StartCoroutine(SpawnPeriodically());
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.Died -= StopSpawning;
        }
    }

    void Update()
    {
        if (spawnCooldown > minSpawnCooldown)
        {
            spawnCooldown -= Time.deltaTime * spawnCooldownDecayPerSecond;
        }
    }

    private IEnumerator SpawnPeriodically()
    {
        while (true)
        {
            if (aliveEnemyCount < maxAliveEnemies)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    private void StopSpawning()
    {
        StopCoroutine(spawnRoutine);
    }

    private void SpawnEnemy()
    {
        if (!TryFindSpawnPosition(out Vector3 spawnPosition))
        {
            Debug.LogWarning("No valid NavMesh spawn position found; skipping this spawn.", this);
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        EnemyAI enemy = Instantiate(prefab, spawnPosition, Quaternion.identity, transform).GetComponent<EnemyAI>();
        enemy.Initialize(player);
        enemy.Died += HandleEnemyDied;
        aliveEnemyCount++;
    }

    private bool TryFindSpawnPosition(out Vector3 spawnPosition)
    {
        for (int attempt = 0; attempt < MaxSpawnPositionAttempts; attempt++)
        {
            Vector3 candidate = new Vector3(
                Random.Range(-spawnAreaHalfSize, spawnAreaHalfSize),
                0f,
                Random.Range(-spawnAreaHalfSize, spawnAreaHalfSize));

            bool isOnNavMesh = NavMesh.SamplePosition(candidate, out NavMeshHit navMeshHit, navMeshSearchRadius, NavMesh.AllAreas);
            if (isOnNavMesh && Vector3.Distance(navMeshHit.position, player.position) > spawnDistance)
            {
                spawnPosition = navMeshHit.position;
                return true;
            }
        }

        spawnPosition = default;
        return false;
    }

    private void HandleEnemyDied(EnemyAI enemy)
    {
        enemy.Died -= HandleEnemyDied;
        aliveEnemyCount--;
        EnemyKilled?.Invoke();
    }
}
