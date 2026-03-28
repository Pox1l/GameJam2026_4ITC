using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class WeaponSelectionUI : MonoBehaviour
{
    [Header("Propojení")]
    public PlayerCombat playerCombat; // Odkaz na skript hráèe
    public GameObject selectionPanel; // Samotné UI okno, které se po výbìru schová

    public List<Sprite> WeaponSprites = new List<Sprite>();
    public Image WeaponImage;
    // Tuto funkci napojíme na tlaèítka
    public void ChooseWeapon(int weaponIndex)
    {
        if (playerCombat != null)
        {
            // Zavolá funkci z tvého PlayerCombat skriptu
            playerCombat.EquipWeaponByIndex(weaponIndex);

            WeaponImage.sprite = WeaponSprites[weaponIndex];
        }

        // Vypne UI panel, aby mohl hráè hrát
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }
}