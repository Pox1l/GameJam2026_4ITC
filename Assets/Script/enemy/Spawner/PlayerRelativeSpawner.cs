using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class PlayerRelativeSpawner : MonoBehaviour
{
    public static PlayerRelativeSpawner Instance;

    [Header("Reference na hráèe")]
    public Transform playerTransform;

    [Header("Spawn nastavení")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public int maxActive = 10;
    public float spawnInterval = 3f;

    [Tooltip("Minimální vzdálenost od hráèe (aby se nespawnuli pøed oblièejem)")]
    public float minSpawnRadius = 8f;
    [Tooltip("Maximální vzdálenost od hráèe")]
    public float maxSpawnRadius = 15f;

    [Header("Kolize pøi spawnu")]
    public float checkRadius = 1f;
    public LayerMask obstacleLayer;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private Coroutine spawnLoop;
    private bool isSpawning = false;

    void Awake()
    {
        // Singleton, aby se dal snadno volat Despawn z UIManageru nebo PlayerStats
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        StartSpawning();
    }

    public void StartSpawning()
    {
        if (spawnLoop == null)
        {
            isSpawning = true;
            spawnLoop = StartCoroutine(SpawnLoop());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnLoop != null)
        {
            StopCoroutine(spawnLoop);
            spawnLoop = null;
        }
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while (isSpawning)
        {
            // Vyèistit seznam od mrtvých nepøátel
            activeEnemies.RemoveAll(item => item == null);

            if (playerTransform != null && activeEnemies.Count < maxActive)
            {
                SpawnOneEnemyNearPlayer();
            }

            yield return wait;
        }
    }

    void SpawnOneEnemyNearPlayer()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return;

        Vector3 spawnPos = Vector3.zero;
        bool validPosFound = false;

        // Zkusíme 10x najít validní místo v prstenci kolem hráèe
        for (int i = 0; i < 10; i++)
        {
            // Vytvoøíme náhodný smìr a vynásobíme ho vzdáleností mezi min a max
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float randomDist = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector2 potentialPoint = (Vector2)playerTransform.position + (randomDir * randomDist);

            if (!Physics2D.OverlapCircle(potentialPoint, checkRadius, obstacleLayer))
            {
                spawnPos = new Vector3(potentialPoint.x, potentialPoint.y, 0f);
                validPosFound = true;
                break;
            }
        }

        if (!validPosFound) return;

        // Výbìr a spawn
        GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        GameObject enemy = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);

        // --- Logika pro pohyb na hráèe ---

        // Pokud používáš NavMeshAgenta
        var agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(spawnPos);
            // Nastavíme mu cíl okamžitì na hráèe
            agent.SetDestination(playerTransform.position);
        }

        // Pokud používáš tvùj EnemyController, nastavíme mu aggro
        var ctrl = enemy.GetComponent<EnemyController>();
        if (ctrl != null)
        {
            // Nastavíme home pozici, aby vìdìl, kam se vrátit, nebo ho rovnou "naštveme"
            ctrl.SetHomePosition(spawnPos);
            ctrl.OnHitAggro(); // Vynutíme pronásledování hráèe
        }
    }

    // Volat pøi smrti hráèe nebo respawnu z UIManageru/PlayerStats
    public void DespawnAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        activeEnemies.Clear();
    }

    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, minSpawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, maxSpawnRadius);
        }
    }
}