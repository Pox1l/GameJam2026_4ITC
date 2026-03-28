using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class WeaponSelectionUI : MonoBehaviour
{

    public static WeaponSelectionUI Instance; // PØIDÁNO

    private void Awake()
    {
        Instance = this; // Nastavení instance
    }

    [Header("Propojení")]
    public PlayerCombat playerCombat;
    public List<Sprite> WeaponSprites = new List<Sprite>();
    public Image WeaponImage;

    [Header("Texty pro poškození")]
    public TextMeshProUGUI swordDamageText;
    public TextMeshProUGUI bowDamageText;

    // TAHLE FUNKCE TO OPRAVÍ: Zavolá se pokaždé, když se okno ukáže
    private void OnEnable()
    {
        UpdateWeaponStatsDisplay();
    }

    public void UpdateWeaponStatsDisplay()
    {
        // Kontrola, jestli už Manager existuje a data jsou naètená
        if (playerCombat == null || UpgradeManager.Instance == null) return;

        // 1. MEÈ (Index 0)
        if (playerCombat.weaponList.Count > 0 && playerCombat.weaponList[0] != null)
        {
            float baseDmg = playerCombat.weaponList[0].baseDamage;
            float finalDmg = UpgradeManager.Instance.GetUpgradedDamage(baseDmg, "Sword");
            if (swordDamageText != null) swordDamageText.text = $"damage: {finalDmg}";
        }

        // 2. LUK (Index 1)
        if (playerCombat.weaponList.Count > 1 && playerCombat.weaponList[1] != null)
        {
            float baseDmg = playerCombat.weaponList[1].baseDamage;
            float finalDmg = UpgradeManager.Instance.GetUpgradedDamage(baseDmg, "Bow");
            if (bowDamageText != null) bowDamageText.text = $"damage: {finalDmg}";
        }
    }

    public void ChooseWeapon(int weaponIndex)
    {
        if (playerCombat != null)
        {
            playerCombat.EquipWeaponByIndex(weaponIndex);
            if (WeaponImage != null) WeaponImage.sprite = WeaponSprites[weaponIndex];
        }

        UIManager.Instance.StartNewRun();
    }
}