using UnityEngine;
using TMPro;
using System.IO;

public class SoulManager : MonoBehaviour
{
    public static SoulManager Instance;

    [Header("Nastavení Duší")]
    public int totalSouls = 0;
    public int passiveAmount = 10;
    public float multiplier = 1f;

    [Header("UI Reference")]
    public TextMeshProUGUI soulText;

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Podobné nastavení jako u tvého PlayerDataManageru
            savePath = ProfileManager.GetSavePath("souls_save.json");
            LoadSouls();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (soulText == null)
        {
            GameObject soulObj = GameObject.FindWithTag("SoulText");
            if (soulObj != null) soulText = soulObj.GetComponent<TextMeshProUGUI>();
        }
        UpdateUI();
    }

    public void AddSouls(int amount)
    {
        totalSouls += amount;
        UpdateUI();
        SaveSouls(); // Uložit pøi zmìnì
    }

    public void AddPassiveSouls()
    {
        int gain = Mathf.RoundToInt(passiveAmount * multiplier);
        totalSouls += gain;
        UpdateUI();
        SaveSouls(); // Uložit pøi zmìnì

        Debug.Log($"<color=cyan>SoulManager:</color> Pøièteno {gain} pasivních duší. Celkem: {totalSouls}");
    }

    private void UpdateUI()
    {
        if (soulText != null)
        {
            soulText.text = "Souls: " + totalSouls.ToString();
        }
    }

    // --- LOGIKA UKLÁDÁNÍ ---

    public void SaveSouls()
    {
        SoulSaveData data = new SoulSaveData();
        data.totalSouls = totalSouls;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        // Volitelné: Bliknutí ikony uložení (pokud ji používáš i pro duše)
        if (SaveVisual.instance != null) SaveVisual.ReportSave();
    }

    public void LoadSouls()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                SoulSaveData data = JsonUtility.FromJson<SoulSaveData>(json);
                totalSouls = data.totalSouls;
            }
            catch
            {
                Debug.LogWarning("Soul save file corrupted.");
                totalSouls = 0;
            }
        }
        else
        {
            totalSouls = 0;
        }
    }
}

// Pomocná tøída pro JSON
[System.Serializable]
public class SoulSaveData
{
    public int totalSouls;
}