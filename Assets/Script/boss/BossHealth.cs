using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    [Header("Nastavení Zdraví")]
    public int maxHealth = 500;
    private int currentHealth;

    private DamageFlash damageFlash;
    private EnemyKnockback knockback;
    private EnemyDrop drop; // Pøidáno

    [Header("Události (Events)")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnBossDeath;

    void Awake()
    {
        damageFlash = GetComponent<DamageFlash>();
        knockback = GetComponent<EnemyKnockback>();
        drop = GetComponent<EnemyDrop>(); // Najde skript pro drop
    }

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(1f);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (damageFlash != null) damageFlash.Flash();
        if (knockback != null) knockback.PlayKnockback();

        float healthPercent = (float)currentHealth / maxHealth;
        OnHealthChanged?.Invoke(Mathf.Clamp01(healthPercent));

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        // Pøed znièením bosse vyhodíme loot
        if (drop != null) drop.DropLoot();

        OnBossDeath?.Invoke();
        Destroy(gameObject);
    }
}