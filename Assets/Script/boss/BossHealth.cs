using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    [Header("Nastavení Zdraví")]
    public string bossName = "Golemus";
    public int maxHealth = 500;
    private int currentHealth;

    private DamageFlash damageFlash;
    private EnemyKnockback knockback;
    private EnemyDrop drop;

    public UnityEvent OnBossDeath;

    public AudioClip hurtEffect;
    public AudioSource player;

    void Awake()
    {
        damageFlash = GetComponent<DamageFlash>();
        knockback = GetComponent<EnemyKnockback>();
        drop = GetComponent<EnemyDrop>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        // ZOBRAZENÍ UI PØES MANAGER
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowBossUI(bossName, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        player.clip = hurtEffect;
        player.Play();
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (damageFlash != null) damageFlash.Flash();
        if (knockback != null) knockback.PlayKnockback();

        // AKTUALIZACE PØES MANAGER
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateBossHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        player.clip = hurtEffect;
        player.Play();
        if (drop != null) drop.DropLoot();

        // Zobrazení vítìzného screenu pøes UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowVictoryScreen();
        }

        OnBossDeath?.Invoke();

        // Znièíme objekt bosse
        Destroy(gameObject);
    }
}