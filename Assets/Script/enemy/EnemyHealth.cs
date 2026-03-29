using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Nastavení Zdraví")]
    public int maxHealth = 50;
    private int currentHealth;

    // Reference na tvé další skripty
    private DamageFlash damageFlash;
    private EnemyKnockback knockback;
    private EnemyDrop drop;

    public AudioSource player;
    public AudioClip hurtClip;
    void Awake()
    {
        // Pøi startu si skript automaticky najde potøebné komponenty na stejném objektu
        damageFlash = GetComponent<DamageFlash>();
        knockback = GetComponent<EnemyKnockback>();
        drop = GetComponent<EnemyDrop>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        player.clip = hurtClip;
        player.Play();

        if (currentHealth <= 0) return; // Pokud je už mrtvý, nedìlej nic

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} dostal {damage} poškození. Zbývá: {currentHealth} HP.");

        // 1. Vizuální efekt (Flash)
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

        // 2. Fyzický efekt (Knockback)
        if (knockback != null)
        {
            knockback.PlayKnockback();
        }

        // 3. Kontrola smrti
        if (currentHealth <= 0)
        {
            if (drop != null) drop.DropLoot();
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} zemøel!"); 

        // Zde mùžeš pozdìji pøidat spawn mincí/zkušeností nebo èásticový efekt
        Destroy(gameObject);
    }
}