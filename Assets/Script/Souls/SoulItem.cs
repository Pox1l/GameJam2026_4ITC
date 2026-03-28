using UnityEditor.EditorTools;
using UnityEngine;

public class SoulItem : MonoBehaviour
{
    public SoulItemSO soulData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kontrola, zda do duše narazil hráè
        if (!collision.CompareTag("Player")) return;
        if (soulData == null) return;

        // Pøiètení duší do manageru
        if (SoulManager.Instance != null)
        {
            SoulManager.Instance.AddSouls(soulData.soulValue);
        }

        // Znièení objektu
        Destroy(gameObject);
    }
}