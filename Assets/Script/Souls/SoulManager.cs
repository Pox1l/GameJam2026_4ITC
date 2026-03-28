using UnityEngine;
using TMPro;

public class SoulManager : MonoBehaviour
{
    public static SoulManager Instance;

    [Header("Nastavení Duší")]
    public int totalSouls = 0;
    public int passiveAmount = 10;
    public float multiplier = 1f;

    [Header("Reference (Pøiøadí se automaticky)")]
    public TextMeshProUGUI soulText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Najde objekt podle Tagu, pokud ještì není pøiøazen v Inspektoru
        if (soulText == null)
        {
            GameObject soulObj = GameObject.FindWithTag("SoulText");
            if (soulObj != null) soulText = soulObj.GetComponent<TextMeshProUGUI>();
        }

        // Okamžitý update na "Souls: 0" hned po startu
        UpdateUI();
    }

    public void AddSouls(int amount)
    {
        totalSouls += amount;
        UpdateUI();
    }

    public void AddPassiveSouls()
    {
        int gain = Mathf.RoundToInt(passiveAmount * multiplier);
        totalSouls += gain;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (soulText != null)
        {
            // Upraveno na formát "Souls: X"
            soulText.text = "Souls: " + totalSouls.ToString();
        }
    }
}