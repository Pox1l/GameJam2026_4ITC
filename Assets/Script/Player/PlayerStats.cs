using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; // Aby na nìj mohl volat UpgradeManager

    private int _maxHealth;
    public float currentHealth;

    [Header("UI Reference")]
    public Slider healthSlider;
    public TextMeshProUGUI hpText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Naèteme životy podle upgradù pøi startu
        UpdateMaxHealth();
        currentHealth = _maxHealth;
    }

    void Update()
    {
        // OPRAVA: Slider teï poèítá s aktuálním max zdravím, ne se stovkou
        if (healthSlider != null)
            healthSlider.value = currentHealth / _maxHealth;

        if (hpText != null)
            hpText.text = Mathf.CeilToInt(currentHealth).ToString() + "/" + _maxHealth.ToString() + " HP";

        // Limity zdraví
        if (currentHealth > _maxHealth) currentHealth = _maxHealth;
        if (currentHealth < 0) currentHealth = 0;
    }

    // Tuto funkci zavolá UpgradeManager, když si koupíš HP
    public void UpdateMaxHealth()
    {
        if (UpgradeManager.Instance != null)
        {
            // Základ 100 + bonus za každý level z UpgradeManageru
            _maxHealth = 100 + (UpgradeManager.Instance.data.hpLevel * UpgradeManager.Instance.hpBonusPerLevel);
        }
        else
        {
            _maxHealth = 100; // Záloha, kdyby manager chybìl
        }
    }

    // Metoda pro nepøátele (napø. Maggot projectile)
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Hráè zemøel!");
        // Tady zavoláme tvoje menu pro výbìr zbranì, co jsme dìlali minule
        if (WeaponSelectionManager.Instance != null)
        {
            WeaponSelectionManager.Instance.OnPlayerDeath();
        }

        // Resetujeme životy pro "nový pokus"
        currentHealth = _maxHealth;
    }
}