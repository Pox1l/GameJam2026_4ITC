using UnityEngine;
using UnityEngine.UI; // Nutné pro Slider
using TMPro;         // Nutné pro Texty

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panely")]
    public GameObject weaponSelectionPanel;
    public GameObject upgradeCanvas;
    public GameObject pauseMenuPanel;

    [Header("Boss UI (Nové)")]
    public GameObject bossHealthPanel; // Celý objekt BossCanvas nebo BossBar
    public Slider bossHealthSlider;
    public TextMeshProUGUI bossHealthText;
    public TextMeshProUGUI bossNameText;

    [Header("Skripty pro aktualizaci")]
    public UpgradeUI upgradeUIScript;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        OpenWeaponSelection();
        if (bossHealthPanel != null) bossHealthPanel.SetActive(false); // Schovat pøi startu
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (weaponSelectionPanel.activeSelf || upgradeCanvas.activeSelf) return;
            if (isPaused) ResumeGame();
            else OpenPauseMenu();
        }
    }

    // --- LOGIKA PRO BOSSE (Voláno z BossHealth) ---

    public void ShowBossUI(string name, int maxHealth)
    {
        if (bossHealthPanel == null) return;

        bossHealthPanel.SetActive(true);
        if (bossNameText != null) bossNameText.text = name;
        UpdateBossHealth(maxHealth, maxHealth);
    }

    public void UpdateBossHealth(int current, int max)
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = (float)current / max;
        }
        if (bossHealthText != null)
        {
            bossHealthText.text = $"{current} / {max}";
        }
    }

    public void HideBossUI()
    {
        if (bossHealthPanel != null) bossHealthPanel.SetActive(false);
    }

    // --- PAUSE MENU A OSTATNÍ ---

    public void OpenPauseMenu()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        SetGameState(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        SetGameState(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenUpgradeMenuOnDeath()
    {
        CloseAllUI();
        upgradeCanvas.SetActive(true);
        if (upgradeUIScript != null) upgradeUIScript.UpdateButtons();
        SetGameState(false);
    }

    public void OnRespawnClicked()
    {
        ClearLevel();
        upgradeCanvas.SetActive(false);
        weaponSelectionPanel.SetActive(true);
        WeaponSelectionUI uiScript = weaponSelectionPanel.GetComponent<WeaponSelectionUI>();
        if (uiScript != null) uiScript.UpdateWeaponStatsDisplay();
    }

    public void StartNewRun()
    {
        if (PlayerStats.Instance != null) PlayerStats.Instance.ResetPlayerForNewRun();
        TimeManager tm = FindObjectOfType<TimeManager>();
        if (tm != null) tm.ResetTimer();

        CloseAllUI();
        isPaused = false;
        SetGameState(true);
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
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (bossHealthPanel != null) bossHealthPanel.SetActive(false);
    }

    public void SetGameState(bool isPlaying)
    {
        Time.timeScale = isPlaying ? 1f : 0f;
        Cursor.lockState = isPlaying ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ClearLevel()
    {
        SpawnerZone2D[] spawners = FindObjectsOfType<SpawnerZone2D>();
        foreach (var s in spawners) s.DespawnAllEnemies();
    }
}