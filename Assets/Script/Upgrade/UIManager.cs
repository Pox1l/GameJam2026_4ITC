using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panely")]
    public GameObject weaponSelectionPanel;
    public GameObject upgradeCanvas;

    [Header("Skripty pro aktualizaci")]
    public UpgradeUI upgradeUIScript;

    private bool isAnyUIOpen = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Na zaèátku hry vždy otevøeme výbìr zbranì
        OpenWeaponSelection();
    }

    private void Update()
    {
        // Otevírání upgradù pøes TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Pokud je otevøený výbìr zbranì, TAB nic nedìlá
            if (weaponSelectionPanel.activeSelf) return;

            ToggleUpgradeMenu();
        }
    }

    // --- VÝBÌR ZBRANÌ ---
    public void OpenWeaponSelection()
    {
        CloseAllUI();
        weaponSelectionPanel.SetActive(true);
        SetGameState(false); // Pauza
    }

    // --- UPGRADE MENU ---
    public void ToggleUpgradeMenu()
    {
        bool newState = !upgradeCanvas.activeSelf;

        if (newState)
        {
            CloseAllUI();
            upgradeCanvas.SetActive(true);
            if (upgradeUIScript != null) upgradeUIScript.UpdateButtons();
            SetGameState(false); // Pauza
        }
        else
        {
            CloseAllUI();
            SetGameState(true); // Hra bìží
        }
    }

    // --- POMOCNÉ FUNKCE ---
    public void CloseAllUI()
    {
        weaponSelectionPanel.SetActive(false);
        upgradeCanvas.SetActive(false);
        // Sem pøidáš další okna (Inventáø, Nastavení atd.)
    }

    public void SetGameState(bool isPlaying)
    {
        isAnyUIOpen = !isPlaying;
        Time.timeScale = isPlaying ? 1f : 0f;

        if (isPlaying)
        {
            // HRÁÈ HRAJE: Myš je vidìt a mùže se volnì hýbat po oknì
            Cursor.lockState = CursorLockMode.Confined; // Confined zajistí, že myš nevyjede z okna hry
            Cursor.visible = true;
        }
        else
        {
            // MENU JE OTEVØENÉ: Myš je úplnì volná pro UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}