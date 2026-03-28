using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MeleeDamageDealer : MonoBehaviour
{
    public int damageToDeal;
    public LayerMask enemyLayers;

    // --- NOVÁ SEKCE PRO NERF ---
    [Header("Nerf Nastavení")]
    [Tooltip("Minimální èas (v sekundách) mezi jednotlivými zásahy meèe. Zabraòuje spamu.")]
    [SerializeField] private float hitCooldown = 1f; // Nastav si podle potøeby (napø. 0.2s = max 5 ran za sekundu)
    private float nextHitTime = 0f; // Èas, kdy meè bude moct znova seknout
    // ----------------------------

    // OnTriggerEnter2D se zavolá POUZE JEDNOU pøi protnutí.
    // Ale teï jsme pøidali èasový zámek.
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. KONTROLA COOLDOWNU: Je už meè "pøipraven" znova zranit?
        if (Time.time < nextHitTime)
        {
            return; // Ještì neubìhl dostatek èasu, ignorujeme tento trigger
        }

        // 2. KONTROLA VRSTVY: Narazili jsme na nepøítele?
        if (((1 << collision.gameObject.layer) & enemyLayers) != 0)
        {
            bool hitRegistered = false;

            // Udìlení poškození
            if (collision.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(damageToDeal);
                hitRegistered = true;
            }
            else if (collision.TryGetComponent(out BossHealth boss))
            {
                boss.TakeDamage(damageToDeal);
                hitRegistered = true;
            }

            // 3. AKTUALIZACE ÈASU: Pokud jsme nìkoho trefili, zamkneme meè na cooldown
            if (hitRegistered)
            {
                // Nastavíme èas, kdy nejdøíve mùže padnout další rána
                nextHitTime = Time.time + hitCooldown;
            }
        }
    }
}