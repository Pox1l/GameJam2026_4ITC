using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panely")]
    public GameObject weaponSelectionPanel;
    public GameObject upgradeCanvas;

    [Header("Skripty pro aktualizaci")]
    public UpgradeUI upgradeUIScript;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // První spuštìní hry
        OpenWeaponSelection();
    }

    // --- VOLÁNO PØI SMRTI ---
    public void OpenUpgradeMenuOnDeath()
    {
        CloseAllUI();
        upgradeCanvas.SetActive(true);
        if (upgradeUIScript != null) upgradeUIScript.UpdateButtons();
        SetGameState(false); // Pauza
    }

    // --- TLAÈÍTKO RESPAWN (v Upgrade panelu) ---
    public void OnRespawnClicked()
    {
        // Nejdøív vyèistíme svìt, zatímco je ještì pauza
        ClearLevel();

        // Pøepneme na výbìr zbranì
        upgradeCanvas.SetActive(false);
        weaponSelectionPanel.SetActive(true);

        // Aktualizujeme damage v textech, aby hráè vidìl, co si nakoupil
        WeaponSelectionUI uiScript = weaponSelectionPanel.GetComponent<WeaponSelectionUI>();
        if (uiScript != null) uiScript.UpdateWeaponStatsDisplay();
    }

    // --- FINÁLNÍ START (po vybrání zbranì) ---
    public void StartNewRun()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.ResetPlayerForNewRun();
        }

        TimeManager tm = FindObjectOfType<TimeManager>();
        if (tm != null) tm.ResetTimer();

        CloseAllUI();
        SetGameState(true); // Rozbìhnout hru
    }

    public void OpenWeaponSelection()
    {
        CloseAllUI();
        weaponSelectionPanel.SetActive(true);
        SetGameState(false);
    }

    public void CloseAllUI()
    {
        if (weaponSelectionPanel != null) weaponSelectionPanel.SetActive(false);
        if (upgradeCanvas != null) upgradeCanvas.SetActive(false);
    }

    public void SetGameState(bool isPlaying)
    {
        Time.timeScale = isPlaying ? 1f : 0f;
        Cursor.lockState = isPlaying ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ClearLevel()
    {
        // Vymaže enemy ze všech aktivních spawner zón
        SpawnerZone2D[] spawners = FindObjectsOfType<SpawnerZone2D>();
        foreach (var s in spawners)
        {
            s.DespawnAllEnemies();
        }

        Debug.Log("Level vyèištìn od nepøátel.");
    }
}