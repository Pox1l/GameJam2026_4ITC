using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Nutné pro List
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    private int _maxHealth;
    public float currentHealth;
    private bool isDead = false;

    [Header("Spawn Nastavení")]
    // Seznam prázdných objektù v mapì, které slouží jako body pro respawn
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("UI Reference")]
    public Slider healthSlider;
    public TextMeshProUGUI hpText;

    public AudioClip hurtEffect;
    public AudioSource player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        isDead = false;
        UpdateMaxHealth();
        currentHealth = _maxHealth;

        // Pokud chceš zaèít na náhodném místì i pøi úplnì prvním startu:
        TeleportToRandomSpawn();
    }

    void Update()
    {
        if (healthSlider != null && _maxHealth > 0)
            healthSlider.value = currentHealth / _maxHealth;

        if (hpText != null)
            hpText.text = Mathf.CeilToInt(currentHealth).ToString() + "/" + _maxHealth.ToString() + " HP";

        if (currentHealth <= 0 && !isDead)
        {
            currentHealth = 0;
            isDead = true;
            Die();
        }

        if (currentHealth > _maxHealth) currentHealth = _maxHealth;

        // Testovací klávesa
        if (Input.GetKeyDown(KeyCode.K)) TakeDamage(999);
    }

    public void UpdateMaxHealth()
    {
        if (UpgradeManager.Instance != null)
            _maxHealth = 100 + (UpgradeManager.Instance.data.hpLevel * UpgradeManager.Instance.hpBonusPerLevel);
        else
            _maxHealth = 100;
    }

    public void ResetPlayerForNewRun()
    {
        isDead = false;
        UpdateMaxHealth();
        currentHealth = _maxHealth;

        // --- ZMÌNA: Náhodný teleport ---
        TeleportToRandomSpawn();

        Debug.Log("Hráè resetován na náhodném bodì.");
    }

    private void TeleportToRandomSpawn()
    {
        if (spawnPoints != null && spawnPoints.Count > 0)
        {
            // Vybereme náhodný index ze seznamu
            int randomIndex = Random.Range(0, spawnPoints.Count);
            // Nastavíme pozici hráèe na pozici vybraného bodu
            transform.position = spawnPoints[randomIndex].position;
        }
        else
        {
            // Pokud jsi zapomnìl nastavit body, hodí tì to na støed
            transform.position = Vector3.zero;
            Debug.LogWarning("Žádné spawn pointy nebyly nastaveny! Teleportuji na (0,0,0).");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        player.clip = hurtEffect;
        player.Play();
    }

    void Die()
    {
        player.clip = hurtEffect;
        player.Play();
        Debug.Log("<color=red>HRÁÈ ZEMØEL!</color>");

        if (TryGetComponent(out Rigidbody2D rb)) rb.velocity = Vector2.zero;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenUpgradeMenuOnDeath();
        }
    }
}