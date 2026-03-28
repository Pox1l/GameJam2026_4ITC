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
            savePath = Path.Combine(Application.persistentDataPath, "souls_save.json");
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
        SaveSouls();
    }

    public void AddPassiveSouls()
    {
        int gain = Mathf.RoundToInt(passiveAmount * multiplier);
        totalSouls += gain;
        UpdateUI();
        SaveSouls();
    }

    private void UpdateUI()
    {
        if (soulText != null)
        {
            soulText.text = "Souls: " + totalSouls.ToString();
        }
    }

    public void SaveSouls()
    {
        SoulSaveData data = new SoulSaveData { totalSouls = totalSouls };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadSouls()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SoulSaveData data = JsonUtility.FromJson<SoulSaveData>(json);
            totalSouls = data.totalSouls;
        }
        else
        {
            totalSouls = 0;
        }
    }
}

[System.Serializable]
public class SoulSaveData
{
    public int totalSouls;
}