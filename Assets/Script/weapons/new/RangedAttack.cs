using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Ranged Attack")]
public class RangedAttack : AttackBase
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;

    [Header("Audio (Unity Standard)")]
    public AudioClip shootSound;

    public override void PerformAttack(Transform attacker, Camera cam, LayerMask enemyLayers, float damageMultiplier)
    {
        var cameraToUse = cam != null ? cam : Camera.main;
        if (cameraToUse == null || projectilePrefab == null) return;

        // Pøehrání zvuku pøes standardní Unity Audio
        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, attacker.position);
        }

        PlayerAttackSystem attackSystem = attacker.GetComponent<PlayerAttackSystem>();
        Transform attackPoint = attackSystem != null ? attackSystem.attackPoint : attacker;

        // Výpoèet smìru k myši
        Vector3 mouseWorld = cameraToUse.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2 aimDir = (mouseWorld - attackPoint.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // Vytvoøení šípu
        GameObject proj = Instantiate(projectilePrefab, attackPoint.position, Quaternion.Euler(0, 0, angle));

        // Pøedání poškození do tvého existujícího DamageDealeru
        if (proj.TryGetComponent(out DamageDealer dealer))
        {
            int bonusDamage = Mathf.RoundToInt(damageMultiplier);
            dealer.damage = baseDamage + bonusDamage;
            dealer.enemyLayers = enemyLayers;
        }

        // Fyzický let šípu
        if (proj.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = aimDir * projectileSpeed; // ZMÌNÌNO z linearVelocity na velocity
        }
    }
}