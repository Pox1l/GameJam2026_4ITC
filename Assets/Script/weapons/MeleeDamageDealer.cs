using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MeleeDamageDealer : MonoBehaviour
{
     public int damageToDeal;
     public LayerMask enemyLayers;

    // OnTriggerEnter2D se zavolá POUZE JEDNOU pøi protnutí.
    // Pro další hit musí jít meè ven a znovu dovnitø.
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Kontrola, zda je zasažený objekt ve správné vrstvì (Enemy)
        if (((1 << collision.gameObject.layer) & enemyLayers) != 0)
        {
            // Udìlení poškození
            if (collision.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(damageToDeal);
            }
            else if (collision.TryGetComponent(out BossHealth boss))
            {
                boss.TakeDamage(damageToDeal);
            }
        }
    }
}