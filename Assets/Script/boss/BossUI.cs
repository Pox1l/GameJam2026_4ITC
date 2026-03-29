using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUI : MonoBehaviour
{
    public static BossUI Instance;

    [Header("UI Komponenty")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI bossNameText;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // Na zaèátku schované
    }

    public void ShowBossUI(string name, int maxHealth)
    {
        gameObject.SetActive(true);
        bossNameText.text = name;
        UpdateUI(maxHealth, maxHealth); // Nastavíme plné HP
    }

    public void UpdateUI(int current, int max)
    {
        float pct = (float)current / max;
        healthSlider.value = pct;
        healthText.text = $"{current} / {max}";
    }

    public void HideBossUI()
    {
        gameObject.SetActive(false);
    }
}