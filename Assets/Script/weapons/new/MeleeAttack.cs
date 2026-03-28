using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Attacks/Melee Attack")]
public class MeleeAttack : AttackBase
{
    [Header("Hitbox Settings")]
    public Vector2 boxSize = new Vector2(1.6f, 0.8f);
    public float boxDistance = 1.0f;

    [Header("Visual Effect")]
    public GameObject slashPrefab;
    public float slashDuration = 0.2f;

    [Header("Audio (Unity Standard)")]
    public AudioClip swingSound; // Zvuk máchnutí (vzduch)
    public AudioClip hitSound;   // Zvuk zásahu (maso/kov)

    public override void PerformAttack(Transform attacker, Camera cam, LayerMask enemyLayers, float damageMultiplier)
    {
        var cameraToUse = cam != null ? cam : Camera.main;
        if (cameraToUse == null) return;

        // 1. Přehrát zvuk máchnutí (vždy)
        if (swingSound != null)
        {
            AudioSource.PlayClipAtPoint(swingSound, attacker.position);
        }

        PlayerAttackSystem attackSystem = attacker.GetComponent<PlayerAttackSystem>();

        // OPRAVA: Změněno z meleePoint na attackPoint
        Transform attackPoint = attackSystem != null ? attackSystem.attackPoint : attacker;

        // --- Výpočet pozice ---
        Vector3 mouseWorld = cameraToUse.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // OPRAVA: Změněno z meleePoint.position na attackPoint.position
        Vector2 meleePos = attackPoint.position;
        Vector2 aimDir = (mouseWorld - (Vector3)meleePos).normalized;
        float angleDeg = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        Vector2 center = meleePos + aimDir * boxDistance;

        // OPRAVA: FLAT DAMAGE (SČÍTÁNÍ)
        int bonusDamage = Mathf.RoundToInt(damageMultiplier);
        int finalDamage = baseDamage + bonusDamage;

        // --- Vizuální efekt ---
        if (slashPrefab)
        {
            var slash = Instantiate(slashPrefab, center, Quaternion.Euler(0, 0, angleDeg - 90));

            var visualDealer = slash.GetComponent<DamageDealer>();
            if (visualDealer != null) Destroy(visualDealer);

            Destroy(slash, slashDuration);
        }

        // --- APLIKACE DAMAGE ---
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, angleDeg, enemyLayers);

        List<MonoBehaviour> alreadyHitTargets = new List<MonoBehaviour>();
        bool didHitSomething = false; // Kontrola pro zvuk zásahu

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == attacker.gameObject) continue;

            bool hitSuccess = false;

            // 1. Zkusíme EnemyHealth
            if (hit.TryGetComponent(out EnemyHealth enemy))
            {
                if (alreadyHitTargets.Contains(enemy)) continue;

                enemy.TakeDamage(finalDamage);
                alreadyHitTargets.Add(enemy);
                hitSuccess = true;
            }
            // 2. Zkusíme BossHealth
            else if (hit.TryGetComponent(out BossHealth boss))
            {
                if (alreadyHitTargets.Contains(boss)) continue;

                boss.TakeDamage(finalDamage);
                alreadyHitTargets.Add(boss);
                hitSuccess = true;
            }

            if (hitSuccess) didHitSomething = true;
        }

        // 2. Přehrát zvuk zásahu
        if (didHitSomething && hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, center);
        }
    }
}