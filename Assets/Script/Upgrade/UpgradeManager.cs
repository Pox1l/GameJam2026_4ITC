using UnityEngine;
using System.IO;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Save Path")]
    private string savePath;
    public UpgradeSaveData data = new UpgradeSaveData();

    [Header("Nastavení cen")]
    public int baseCost = 50;
    public int costStep = 50;

    [Header("Bonusy za úroveò")]
    public float damageBonusPerLevel = 5f;
    public int hpBonusPerLevel = 20;
    public float multiBonusPerLevel = 0.2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        savePath = Path.Combine(Application.persistentDataPath, "upgrades_save.json");
        LoadUpgrades();
    }

    // --- LOGIKA NÁKUPU ---

    public void UpgradeHP()
    {
        int cost = CalculateCost(data.hpLevel);
        if (SoulManager.Instance.totalSouls >= cost)
        {
            SoulManager.Instance.totalSouls -= cost;
            data.hpLevel++;

            // --- PØIDÁNO: Okamžitá aktualizace hráèe v reálném èase ---
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.UpdateMaxHealth();
                PlayerStats.Instance.currentHealth += hpBonusPerLevel; // Doléèíme ho o ten bonus
            }

            FinishUpgrade();
        }
    }

    public void UpgradeSword()
    {
        BuyUpgrade(ref data.swordLevel);
    }

    public void UpgradeBow()
    {
        BuyUpgrade(ref data.bowLevel);
    }

    public void UpgradeMultiplier()
    {
        int cost = CalculateCost(data.multiplierLevel);
        if (SoulManager.Instance.totalSouls >= cost)
        {
            SoulManager.Instance.totalSouls -= cost;
            data.multiplierLevel++;

            // Aplikujeme do SoulManageru
            SoulManager.Instance.multiplier += multiBonusPerLevel;

            FinishUpgrade();
        }
    }

    // --- POMOCNÉ FUNKCE ---

    private void BuyUpgrade(ref int levelVar)
    {
        int cost = CalculateCost(levelVar);
        if (SoulManager.Instance.totalSouls >= cost)
        {
            SoulManager.Instance.totalSouls -= cost;
            levelVar++;
            FinishUpgrade();
        }
    }

    private void FinishUpgrade()
    {
        SoulManager.Instance.SaveSouls();
        SoulManager.Instance.soulText.text = "Souls: " + SoulManager.Instance.totalSouls;
        SaveUpgrades(); // Tady se zapíše tvùj JSON

        // PØIDÁNO: Okamžitá aktualizace textù v druhém menu
        if (WeaponSelectionUI.Instance != null)
        {
            WeaponSelectionUI.Instance.UpdateWeaponStatsDisplay();
        }
    }

    public int CalculateCost(int currentLevel)
    {
        return baseCost + (currentLevel * costStep);
    }

    // Výpoèet poškození pro zbranì
    public float GetUpgradedDamage(float baseDamage, string type)
    {
        if (type == "Sword") return baseDamage + (data.swordLevel * damageBonusPerLevel);
        if (type == "Bow") return baseDamage + (data.bowLevel * damageBonusPerLevel);
        return baseDamage;
    }

    // --- SAVE / LOAD ---

    public void SaveUpgrades()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadUpgrades()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<UpgradeSaveData>(json);
        }
    }
}