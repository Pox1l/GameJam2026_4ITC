using UnityEngine;
using UnityEngine.Events; // Nutné pro UnityEvent

public class BossHealth : MonoBehaviour
{
    [Header("Nastavení Zdraví")]
    public int maxHealth = 500;
    private int currentHealth;

    [Header("Události (Events)")]
    // Tohle se ukáže v Inspektoru. Mùžeš sem napojit napø. UI Slider (Health Bar)
    public UnityEvent<float> OnHealthChanged;
    // Mùžeš sem napojit Win Screen, otevøení dveøí atd.
    public UnityEvent OnBossDeath;

    void Start()
    {
        currentHealth = maxHealth;

        // Na zaèátku pošleme do UI hodnotu 1f (100 %)
        OnHealthChanged?.Invoke(1f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"BOSS {gameObject.name} dostal {damage} poškození!");

        // Spoèítáme procento zdraví (0.0 až 1.0) pro Health Bar a pošleme ho ven
        float healthPercent = (float)currentHealth / maxHealth;
        OnHealthChanged?.Invoke(Mathf.Clamp01(healthPercent));

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