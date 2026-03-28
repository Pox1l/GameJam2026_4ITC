using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponSelectionUI : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public List<Sprite> WeaponSprites = new List<Sprite>();
    public Image WeaponImage;

    public void ChooseWeapon(int weaponIndex)
    {
        if (playerCombat != null)
        {
            playerCombat.EquipWeaponByIndex(weaponIndex);
            if (WeaponImage != null) WeaponImage.sprite = WeaponSprites[weaponIndex];
        }

        // Øekneme manageru, že je vybráno a mùže se hrát
        UIManager.Instance.CloseAllUI();
        UIManager.Instance.SetGameState(true);
    }
}