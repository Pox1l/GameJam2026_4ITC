using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public int damageToDeal;
    [HideInInspector] public LayerMask enemyLayers;

    [Header("Nastavení Projektilu")]
    [SerializeField] private float lifetime = 5f;
    // PØIDÁNO: Vrstvy, které šíp znièí (napø. zdi, pøekážky)
    [SerializeField] private LayerMask obstacleLayers;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. KONTROLA: Nepøítel (Dá damage + znièí se)
        if (((1 << collision.gameObject.layer) & enemyLayers) != 0)
        {
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
        // 2. KONTROLA: Pøekážka/Zeï (Jen se znièí bez damage)
        else if (((1 << collision.gameObject.layer) & obstacleLayers) != 0)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        // Tady mùžeš instanciovat èástice nárazu
        Destroy(gameObject);
    }
}