using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(Collider2D))]
public class SpawnerZone2D : MonoBehaviour
{
    [Header("Spawn nastavení")]
    // Zmìnìno na List pro více druhù nepøátel
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public int maxActive = 6;
    public float spawnInterval = 2f;
    public float spawnRadius = 5f;
    public float firstSpawnDelay = 0.5f;

    [Header("Kolize pøi spawnu")]
    public float checkRadius = 1f;
    public LayerMask obstacleLayer;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool playerInside;
    private Coroutine spawnLoop;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        if (spawnLoop == null)
            spawnLoop = StartCoroutine(SpawnLoop());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        if (spawnLoop != null)
        {
            StopCoroutine(spawnLoop);
            spawnLoop = null;
        }

        DespawnAllEnemies();
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(firstSpawnDelay);
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while (playerInside)
        {
            activeEnemies.RemoveAll(item => item == null);

            if (activeEnemies.Count < maxActive)
            {
                SpawnOneEnemy();
            }

            yield return wait;
        }
    }

    void SpawnOneEnemy()
    {
        // Kontrola, zda máme co spawnovat
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("SpawnerZone2D: Seznam enemyPrefabs je prázdný!");
            return;
        }

        Vector3 spawnPos = Vector3.zero;
        bool validPosFound = false;

        for (int i = 0; i < 10; i++)
        {
            Vector2 randomPoint = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

            if (!Physics2D.OverlapCircle(randomPoint, checkRadius, obstacleLayer))
            {
                spawnPos = new Vector3(randomPoint.x, randomPoint.y, 0f);
                validPosFound = true;
                break;
            }
        }

        if (!validPosFound) return;

        // --- NÁHODNÝ VÝBÌR Z LISTU ---
        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject selectedPrefab = enemyPrefabs[randomIndex];

        // Vytvoøení náhodného nepøítele
        GameObject enemy = Instantiate(selectedPrefab, spawnPos, Quaternion.identity, transform);
        activeEnemies.Add(enemy);

        // Nastavení NavMeshAgenta
        var agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(spawnPos);
        }

        // Nastavení Home pozice
        var ctrl = enemy.GetComponent<EnemyController>();
        if (ctrl != null)
        {
            ctrl.SetHomePosition(spawnPos);
        }
    }

    void DespawnAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
#endif
}