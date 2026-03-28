using UnityEngine;

public class WeaponSelectionManager : MonoBehaviour
{
    public static WeaponSelectionManager Instance;

    [Header("UI Reference")]
    public GameObject selectWeaponCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Zobrazí menu hned na zaèátku hry
        ShowWeaponSelect();
    }

    public void ShowWeaponSelect()
    {
        if (selectWeaponCanvas != null)
        {
            selectWeaponCanvas.SetActive(true);

            // Zastavíme èas, aby hráè mohl v klidu vybírat
            Time.timeScale = 0f;

            // Odemkneme myš pro UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CloseWeaponSelect()
    {
        if (selectWeaponCanvas != null)
        {
            selectWeaponCanvas.SetActive(false);

            // Opìt pustíme èas
            Time.timeScale = 1f;

            // Zamkneme myš zpìt do hry (volitelné, podle tvého ovládání)
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
    }

    // Tuto funkci zavoláš ze skriptu PlayerHealth pøi smrti
    public void OnPlayerDeath()
    {
        ShowWeaponSelect();
    }
}