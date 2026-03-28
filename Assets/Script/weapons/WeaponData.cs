using UnityEngine;

[CreateAssetMenu(fileName = "newWeapon", menuName = "Combat/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject visualPrefab;
    public bool isRanged = false;

    [Header("Stats")]
    public float baseDamage;
    public float attackCooldown = 0.5f;

    [Header("Melee Nastavení")]
    public float attackRange = 1f; // Jak velký kruh zraòuje

    [Header("Ranged Nastavení")]
    public GameObject projectilePrefab; // Prefab šípu, co poletí
    public float projectileSpeed = 15f;
}