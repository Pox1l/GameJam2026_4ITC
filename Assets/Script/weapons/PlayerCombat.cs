using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Detekce Nepøátel")]
    public LayerMask enemyLayers;

    [Header("Seznam Zbraní")]
    public List<WeaponData> weaponList;

    [Header("Nastavení Ruky")]
    public Transform fixedPoint;
    public Transform firePoint;
    public float orbitDistance = 1f;

    [Header("Aktuální Výbava (Jen pro ètení)")]
    [SerializeField] private WeaponData currentWeaponData;
    private GameObject spawnedWeaponObject;
    private SpriteRenderer weaponSpriteRenderer;

    private float currentDamage;
    private float nextAttackTime = 0f;

    void Start()
    {
        currentWeaponData = null;
    }

    void Update()
    {
        if (currentWeaponData == null) return;

        // Útok necháme v Update kvùli pøesné odezvì na kliknutí
        if (Input.GetButton("Fire1") && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + currentWeaponData.attackCooldown;
        }
    }

    // ZMÌNA: Rotaci zbranì poèítáme až POTÉ, co Cinemachine pohne kamerou
    void LateUpdate()
    {
        if (currentWeaponData == null) return;
        HandleWeaponRotation();
    }

    void HandleWeaponRotation()
    {
        // Výpoèet pøes Screen Space je nejstabilnìjší pro pixel art
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(fixedPoint.position);
        Vector2 mousePos = Input.mousePosition;
        
        Vector2 aimDirection = (mousePos - (Vector2)playerScreenPos).normalized;

        firePoint.position = (Vector2)fixedPoint.position + aimDirection * orbitDistance;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        if (weaponSpriteRenderer != null)
        {
            if (angle >= -90f && angle <= 90f)
                firePoint.localScale = Vector3.one;
            else
                firePoint.localScale = new Vector3(1, -1, 1);
        }
    }

    public void EquipWeaponByIndex(int index)
    {
        if (index < 0 || index >= weaponList.Count) return;

        currentWeaponData = weaponList[index];

        string weaponType = currentWeaponData.isRanged ? "Bow" : "Sword";
        currentDamage = UpgradeManager.Instance.GetUpgradedDamage(currentWeaponData.baseDamage, weaponType);

        if (spawnedWeaponObject != null)
        {
            Destroy(spawnedWeaponObject);
        }

        if (currentWeaponData.visualPrefab != null)
        {
            spawnedWeaponObject = Instantiate(currentWeaponData.visualPrefab, firePoint);
            spawnedWeaponObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            weaponSpriteRenderer = spawnedWeaponObject.GetComponent<SpriteRenderer>();

            if (!currentWeaponData.isRanged)
            {
                if (spawnedWeaponObject.TryGetComponent(out MeleeDamageDealer meleeDealer))
                {
                    meleeDealer.damageToDeal = (int)currentDamage;
                    meleeDealer.enemyLayers = enemyLayers;
                }
            }
        }
    }

    void Attack()
    {
        string weaponType = currentWeaponData.isRanged ? "Bow" : "Sword";
        currentDamage = UpgradeManager.Instance.GetUpgradedDamage(currentWeaponData.baseDamage, weaponType);

        if (currentWeaponData.isRanged)
        {
            if (currentWeaponData.projectilePrefab != null)
            {
                GameObject proj = Instantiate(currentWeaponData.projectilePrefab, firePoint.position, firePoint.rotation);

                if (proj.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.velocity = firePoint.right * currentWeaponData.projectileSpeed;
                }

                if (proj.TryGetComponent(out Projectile dealer))
                {
                    dealer.damageToDeal = (int)currentDamage;
                    dealer.enemyLayers = enemyLayers;
                }
            }
        }
        else
        {
            if (spawnedWeaponObject != null && spawnedWeaponObject.TryGetComponent(out MeleeDamageDealer meleeDealer))
            {
                meleeDealer.damageToDeal = (int)currentDamage;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (firePoint == null || currentWeaponData == null) return;
        if (!currentWeaponData.isRanged)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, currentWeaponData.attackRange);
        }
    }
}