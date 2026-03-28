using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Nastavení Ruky a Otáèení")]
    // 1. ZDE PØIØAÏ: Pevný støed otáèení (napø. støed tìla hráèe)
    public Transform fixedPoint;
    // Zde se objeví zbraò (musí být Child objektu FixedPoint v Hierarchy)
    public Transform firePoint;

    [Header("Aktuální Výbava (Jen pro ètení)")]
    [SerializeField] private WeaponData currentWeaponData;
    private GameObject spawnedWeaponObject;
    private SpriteRenderer weaponSpriteRenderer;

    private float currentDamage;
    private float nextAttackTime = 0f;

    void Start()
    {
        // Kontrola pøiøazení v Inspektoru
        if (fixedPoint == null || firePoint == null)
        {
            Debug.LogError("PlayerCombat: Chybí FixedPoint nebo FirePoint v Inspektoru!");
            enabled = false; // Vypne skript, pokud nejsou promìnné nastavené
            return;
        }

        if (currentWeaponData != null)
        {
            EquipWeapon(currentWeaponData);
        }
    }

    void Update()
    {
        if (currentWeaponData == null) return;

        HandleWeaponRotation();

        // Útok
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + currentWeaponData.attackCooldown;
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeaponData = newWeapon;
        currentDamage = currentWeaponData.baseDamage;

        if (spawnedWeaponObject != null)
        {
            Destroy(spawnedWeaponObject);
        }

        if (currentWeaponData.visualPrefab != null)
        {
            // Zbraò se vytvoøí UVNITØ firePointu
            spawnedWeaponObject = Instantiate(currentWeaponData.visualPrefab, firePoint);

            // Pevnì se uzamkne na støed FirePointu
            spawnedWeaponObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            weaponSpriteRenderer = spawnedWeaponObject.GetComponent<SpriteRenderer>();
        }
    }

    void HandleWeaponRotation()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ve 2D nás osa Z nezajímá

        // --- ZMÌNA ZDE ---
        // Poèítáme smìr od PEVNÉHO støedu (FixedPoint), ne od FirePointu
        Vector2 aimDirection = mousePos - fixedPoint.position;

        // Vypoèítáme úhel, kam se má FixedPoint otoèit
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Otoèíme CELÝ FixedPoint (rodiè). Protože je FirePoint jeho dítì a je posunutý, 
        // zaène pøirozenì obíhat dokola kolem støedu.
        fixedPoint.rotation = Quaternion.Euler(0, 0, angle);

        // --- ZMÌNA ZDE ---
        // Vizualní oprava, aby zbraò nebyla vzhùru nohama pøi míøení doleva.
        // Otáèíme lokální scale FirePointu, ne celého rodièe, aby se nerozbila matematika orbity.
        if (weaponSpriteRenderer != null)
        {
            // Míøíme vpravo (standardní stav)
            if (angle >= -90f && angle <= 90f)
            {
                firePoint.localScale = Vector3.one;
            }
            // Míøíme vlevo -> pøevrátíme zbraò na ose Y
            else
            {
                firePoint.localScale = new Vector3(1, -1, 1);
            }
        }
    }

    void Attack()
    {
        Debug.Log("Útok zbraní " + currentWeaponData.weaponName + " za " + currentDamage + " ve smìru: " + firePoint.right);

        if (currentWeaponData.isRanged)
        {
            // Logika pro luk (Instantiate šípu)
        }
        else
        {
            // Logika pro meè (OverlapCircle)
        }
    }

    // Funkce pro vylepšení (volat z UI tlaèítka)
    public void UpgradeCurrentWeapon()
    {
        if (currentWeaponData != null)
        {
            currentDamage += 5f;
            Debug.Log("Zbraò vylepšena. Nové poškození: " + currentDamage);
        }
    }
}
