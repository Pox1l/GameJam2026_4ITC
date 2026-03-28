using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Seznam Zbraní")]
    public List<WeaponData> weaponList;

    [Header("Nastavení Ruky")]
    public Transform fixedPoint; // Støed otáèení (hráè)
    public Transform firePoint;  // Obíhající bod (zbraò)
    public float orbitDistance = 1f; // Vzdálenost zbranì od støedu

    [Header("Aktuální Výbava (Jen pro ètení)")]
    [SerializeField] private WeaponData currentWeaponData;
    private GameObject spawnedWeaponObject;
    private SpriteRenderer weaponSpriteRenderer;

    private float currentDamage;
    private float nextAttackTime = 0f;

    void Start()
    {
        // ZMÌNÌNO: Hráè na zaèátku nemá zbraò. 
        // Èeká se na výbìr z UI, které zavolá EquipWeaponByIndex.
        currentWeaponData = null;
    }

    void Update()
    {
        // Pokud hráè ještì nemá zbraò, kód se pøeruší (neotáèí se ruka, nelze útoèit)
        if (currentWeaponData == null) return;

        HandleWeaponRotation();

        // Útok
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + currentWeaponData.attackCooldown;
        }
    }

    public void EquipWeaponByIndex(int index)
    {
        if (index < 0 || index >= weaponList.Count) return;

        currentWeaponData = weaponList[index];
        currentDamage = currentWeaponData.baseDamage;

        if (spawnedWeaponObject != null)
        {
            Destroy(spawnedWeaponObject);
        }

        if (currentWeaponData.visualPrefab != null)
        {
            spawnedWeaponObject = Instantiate(currentWeaponData.visualPrefab, firePoint);
            spawnedWeaponObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            weaponSpriteRenderer = spawnedWeaponObject.GetComponent<SpriteRenderer>();
        }
    }

    void HandleWeaponRotation()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // 1. Spoèítání smìru od støedu k myši
        Vector2 aimDirection = (mousePos - fixedPoint.position).normalized;

        // 2. Nastavení pozice firePointu (krouží kolem fixedPointu)
        firePoint.position = (Vector2)fixedPoint.position + aimDirection * orbitDistance;

        // 3. Samotné natoèení firePointu na myš
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        // 4. Pøevrácení zbranì pøi pohledu doleva
        if (weaponSpriteRenderer != null)
        {
            if (angle >= -90f && angle <= 90f)
                firePoint.localScale = Vector3.one;
            else
                firePoint.localScale = new Vector3(1, -1, 1);
        }
    }

    void Attack()
    {
        Debug.Log("Útok zbraní " + currentWeaponData.weaponName + " za " + currentDamage);

        if (currentWeaponData.isRanged) { /* Luk */ }
        else { /* Meè */ }
    }
}