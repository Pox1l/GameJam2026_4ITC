using UnityEngine;
using TMPro;
using System.Collections; // Nutné pro IEnumerator

public class BossAltar : MonoBehaviour
{
    [Header("Nastavení Spawnu")]
    public GameObject bossPrefab;
    public int spawnCost = 500;
    public Transform spawnPoint;

    [Header("UI Reference (Pøiøaï v Inspektoru)")]
    public GameObject interactionWorldUI;
    public GameObject pressEKeyObject;
    public TextMeshProUGUI costTextTMP;

    [Header("Nastavení Klavesy")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerIsNear = false;
    private bool bossSpawned = false;

    void Start()
    {
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

            // Spustíme Coroutinu pro delay
            StartCoroutine(SpawnBossRoutine());
        }
        else
        {
            Debug.Log("Nedostatek duší!");
        }
    }

    // NOVÉ: Coroutina pro zpoždìný spawn
    private IEnumerator SpawnBossRoutine()
    {
        bossSpawned = true; // Hned oznaèíme jako spawnované, aby nešel triggerovat znovu
        ShowUI(false);      // UI zmizí okamžitì po kliknutí

        yield return new WaitForSeconds(0.5f); // Èekání 0.5 sekundy

        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);

        this.enabled = false;
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