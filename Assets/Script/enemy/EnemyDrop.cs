using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [Header("Nastavení Dropu")]
    public GameObject essencePrefab;
    public int amount = 1;

    public void DropLoot()
    {
        if (essencePrefab == null) return;

        for (int i = 0; i < amount; i++)
        {
            // Malý náhodný rozptyl, aby nespolu ležely na jednom pixelu
            Vector2 offset = Random.insideUnitCircle * 0.5f;
            Instantiate(essencePrefab, (Vector2)transform.position + offset, Quaternion.identity);
        }
    }
}