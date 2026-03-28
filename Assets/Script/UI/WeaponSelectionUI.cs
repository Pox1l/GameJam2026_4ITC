using UnityEngine;

public class WeaponSelectionUI : MonoBehaviour
{
    [Header("Propojení")]
    public PlayerCombat playerCombat; // Odkaz na skript hráèe
    public GameObject selectionPanel; // Samotné UI okno, které se po výbìru schová

    // Tuto funkci napojíme na tlaèítka
    public void ChooseWeapon(int weaponIndex)
    {
        if (playerCombat != null)
        {
            // Zavolá funkci z tvého PlayerCombat skriptu
            playerCombat.EquipWeaponByIndex(weaponIndex);
        }

        // Vypne UI panel, aby mohl hráè hrát
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }
}