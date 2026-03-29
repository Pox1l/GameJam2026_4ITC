using UnityEngine;
using TMPro;

public class BossAltar : MonoBehaviour
{
    [Header("Nastavení Spawnu")]
    public GameObject bossPrefab;
    public int spawnCost = 500;
    public Transform spawnPoint;

    [Header("UI Reference (Pøiøaï v Inspektoru)")]
    public GameObject interactionWorldUI; // Celý panel (bossPanel)
    public GameObject pressEKeyObject;   // Konkrétnì ten objekt "E"
    public TextMeshProUGUI costTextTMP;  // Text s cenou

    [Header("Nastavení Klavesy")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerIsNear = false;
    private bool bossSpawned = false;

    void Start()
    {
        // Skryjeme UI pøi startu
        ShowUI(false);

        if (costTextTMP != null)
            costTextTMP.text = spawnCost + " Souls";

        if (spawnPoint == null) spawnPoint = transform;
    }

    void Update()
    {
        if (playerIsNear && !bossSpawned && Input.GetKeyDown(interactKey))
        {
            TrySpawnBoss();
        }
    }

    private void TrySpawnBoss()
    {
        if (SoulManager.Instance != null && SoulManager.Instance.totalSouls >= spawnCost)
        {
            SoulManager.Instance.totalSouls -= spawnCost;
            SoulManager.Instance.SaveSouls();

            SpawnBoss();
        }
        else
        {
            Debug.Log("Nedostatek duší!");
        }
    }

    private void SpawnBoss()
    {
        bossSpawned = true;
        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);

        ShowUI(false);
        this.enabled = false; // Vypne skript po úspìšném vyvolání
    }

    private void ShowUI(bool state)
    {
        if (interactionWorldUI != null) interactionWorldUI.SetActive(state);
        if (pressEKeyObject != null) pressEKeyObject.SetActive(state);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !bossSpawned)
        {
            playerIsNear = true;
            ShowUI(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = false;
            ShowUI(false);
        }
    }
}