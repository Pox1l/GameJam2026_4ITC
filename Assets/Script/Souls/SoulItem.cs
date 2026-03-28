using UnityEngine;

public class SoulItem : MonoBehaviour
{
    public SoulItemSO soulData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kontrola, zda do duše narazil hráè
        if (!collision.CompareTag("Player")) return;
        if (soulData == null) return;

        // 1. Pøiètení duší do globálního manageru (logika)
        if (SoulManager.Instance != null)
        {
            SoulManager.Instance.AddSouls(soulData.soulValue);
        }

        // 2. Zobrazení notifikace (vizuál)
        // Použijeme tvou metodu ShowPickup
        if (PickupNotificationManager.Instance != null)
        {
            PickupNotificationManager.Instance.ShowPickup(
                soulData.icon,      // Ikona ze SO
                soulData.soulName,  // Jméno (napø. "Souls")
                soulData.soulValue  // Množství (napø. 10)
            );
        }

        // 3. Znièení objektu na zemi
        Destroy(gameObject);
    }
}