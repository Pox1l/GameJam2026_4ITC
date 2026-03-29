using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Nutné pro pøepínání scén

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panely")]
    public GameObject weaponSelectionPanel;
    public GameObject upgradeCanvas;
    public GameObject pauseMenuPanel;

    [Header("Vítìzné UI (Finish)")]
    public GameObject finishCanvas;  // Celý objekt Canvasu
    public GameObject victoryPanel;  // Ten vypnutý objekt "Panel" pod Canvasem

    [Header("Boss UI")]
    public GameObject bossHealthPanel;
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
        if (victoryPanel != null) victoryPanel.SetActive(false); // Jistota, že je zavøený
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Pokud svítí vítìzství, upgrade nebo výbìr zbraní, pauzu neøešíme
            if (weaponSelectionPanel.activeSelf || upgradeCanvas.activeSelf || (victoryPanel != null && victoryPanel.activeSelf)) return;

            if (isPaused) ResumeGame();
            else OpenPauseMenu();
        }
    }

    // --- LOGIKA PRO KONEC HRY (VÍTÌZSTVÍ) ---

    public void ShowVictoryScreen()
    {
        HideBossUI(); // Schová HP bar bosse

        if (finishCanvas != null) finishCanvas.SetActive(true);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true); // Aktivuje ten vypnutý panel s tlaèítky
            SetGameState(false);         // Zastaví èas hry (pauza)
        }
    }

    public void ReturnToMainMenu()
    {
        // Pøed naètením scény 0 musíme pustit èas, jinak menu zamrzne!
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // --- PAUSE & MENU LOGIKA ---

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
        Debug.Log("Ukonèuji hru...");
        Application.Quit();
    }

    // --- BOSS UI LOGIKA ---

    public void ShowBossUI(string name, int maxHealth)
    {
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(true);
            if (bossNameText != null) bossNameText.text = name;
            UpdateBossHealth(maxHealth, maxHealth);
        }
    }

    public void UpdateBossHealth(int current, int max)
    {
        if (bossHealthSlider != null) bossHealthSlider.value = (float)current / max;
        if (bossHealthText != null) bossHealthText.text = $"{current} / {max}";
    }

    public void HideBossUI()
    {
        if (bossHealthPanel != null) bossHealthPanel.SetActive(false);
    }

    // --- POMOCNÉ FUNKCE ---

    public void OpenUpgradeMenuOnDeath()
    {
        CloseAllUI();
        upgradeCanvas.SetActive(true);
        if (upgradeUIScript != null) upgradeUIScript.UpdateButtons();
        SetGameState(false);
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
        if (finishCanvas != null) finishCanvas.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
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