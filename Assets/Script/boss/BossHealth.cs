using UnityEngine;
using UnityEngine.Events; // Nutné pro UnityEvent

public class BossHealth : MonoBehaviour
{
    [Header("Nastavení Zdraví")]
    public int maxHealth = 500;
    private int currentHealth;

    [Header("Vizuální a Fyzické efekty")]
    // Skript se pokusí tyto komponenty najít automaticky pøi startu
    private DamageFlash damageFlash;
    private EnemyKnockback knockback;

    [Header("Události (Events)")]
    // Tohle se ukáže v Inspektoru. Mùžeš sem napojit napø. UI Slider (Health Bar)
    public UnityEvent<float> OnHealthChanged;
    // Mùžeš sem napojit Win Screen, otevøení dveøí atd.
    public UnityEvent OnBossDeath;

    void Awake()
    {
        // Automaticky najde komponenty na stejném objektu
        damageFlash = GetComponent<DamageFlash>();
        knockback = GetComponent<EnemyKnockback>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        // Na zaèátku pošleme do UI hodnotu 1f (100 %)
        OnHealthChanged?.Invoke(1f);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // Pokud už je mrtvý, nic nedìlej

        currentHealth -= damage;
        Debug.Log($"BOSS {gameObject.name} dostal {damage} poškození!");

        // 1. Spuštìní bliknutí (Vizuální efekt)
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

        // 2. Spuštìní odhození (Fyzický efekt)
        // POZNÁMKA: U velkých bossù možná budeš chtít nastavit knockbackForce na 0,
        // aby se nehýbali, ale skript se zavolá, takže agent se vypne a zapne.
        if (knockback != null)
        {
            knockback.PlayKnockback();
        }

        // 3. Aktualizace UI (Health Bar)
        // Spoèítáme procento zdraví (0.0 až 1.0) a pošleme ho ven
        float healthPercent = (float)currentHealth / maxHealth;
        OnHealthChanged?.Invoke(Mathf.Clamp01(healthPercent));

        // 4. Kontrola smrti
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"BOSS {gameObject.name} BYL PORAŽEN!");

        // Spustí všechny funkce zavìšené v Inspektoru (napø. výhra hry)
        OnBossDeath?.Invoke();

        Destroy(gameObject);
    }
}