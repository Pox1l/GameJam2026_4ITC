using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Nastavení Zdraví")]
    public int maxHealth = 50;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} dostal {damage} poškození. Zbývá: {currentHealth} HP.");

        // Zde mùžeš pozdìji pøidat napø. bliknutí do èervena (Visual Feedback)

        if (currentHealth <= 0)
        {
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