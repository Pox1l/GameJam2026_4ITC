using UnityEngine;

[CreateAssetMenu(fileName = "newWeapon", menuName = "Combat/WeaponData")]
public class WeaponData : ScriptableObject
{
   
    public string weaponName;
    // Sem vložíš Prefab meèe nebo luku (ten, co má SpriteRenderer)
    public GameObject visualPrefab;
    public bool isRanged = false; // Zaškrtni jen u Luku

    [Header("Stats")]
    public float baseDamage;
    public float attackCooldown = 0.5f; // Rychlost útokù
}