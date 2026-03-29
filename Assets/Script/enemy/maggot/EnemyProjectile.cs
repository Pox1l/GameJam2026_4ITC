using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float lifetime = 3f;

    [Header("Detekce nárazu")]
    // V Unity Inspektoru tady zaškrtni vrstvy jako Walls, Ground atd.
    public LayerMask obstacleLayers;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. KONTROLA: Narazili jsme do hráèe?
        if (collision.gameObject.TryGetComponent<PlayerStats>(out PlayerStats playerStats))
        {
            playerStats.TakeDamage(damage); // Použijeme tvou metodu pro damage
            Destroy(gameObject);
            return; // Ukonèíme funkci, aby se kód dál nevyhodnocoval
        }

        // 2. KONTROLA: Narazili jsme do pøekážky (zeï/zemì)?
        // Kontrolujeme, jestli layer kolize sedí s maskou obstacleLayers
        if (((1 << collision.gameObject.layer) & obstacleLayers) != 0)
        {
            Destroy(gameObject);
        }
    }
}