using UnityEngine;

[RequireComponent(typeof(Collider2D))] // Šíp potøebuje Collider
public class Projectile : MonoBehaviour
{
    // Tyto hodnoty nastavíme z PlayerCombat pøi vystøelení
    [HideInInspector] public int damageToDeal;
    [HideInInspector] public LayerMask enemyLayers;

    [Header("Nastavení Projektilu")]
    [SerializeField] private float lifetime = 5f; // Šíp se po 5s znièí, aby nelétal do nekoneèna

    void Start()
    {
        // Automaticky znièit šíp po uplynutí lifetime
        Destroy(gameObject, lifetime);
    }

    // Tato funkce se zavolá, když Collider šípu narazí do jiného Collideru nastaveného jako Trigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. KONTROLA: Narazili jsme do nepøítele (je ve správné Layer)?
        // Používáme bitový posun k ovìøení LayerMask
        if (((1 << collision.gameObject.layer) & enemyLayers) != 0)
        {
            // 2. KONTROLA: Má nepøítel EnemyHealth nebo BossHealth?
            if (collision.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(damageToDeal);
                HitTarget();
            }
            else if (collision.TryGetComponent(out BossHealth boss))
            {
                boss.TakeDamage(damageToDeal);
                HitTarget();
            }
        }
    }

    // Co se stane, když šíp nìkoho trefí
    void HitTarget()
    {
        // Zde mùžeš pøidat efekt zásahu (èástice, zvuk)

        // Znièit šíp
        Destroy(gameObject);
    }
}