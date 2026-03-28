using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
    [Header("General Settings")]
    public LayerMask enemyLayers;
    public Camera cam;
    public Transform attackPoint; // Slouèený bod pro meè i luk

    [Header("Active Attack")]
    public AttackBase currentAttack;
    public float nextAttackTime;

    // Hodnota, o kterou se zvedá damage (pokud máš systém vylepšování)
    public float currentBonusDamage = 0f;

    void Update()
    {
        if (currentAttack == null) return;
        if (Time.time < nextAttackTime) return;

        if (Input.GetMouseButtonDown(0))
        {
            currentAttack.PerformAttack(transform, cam, enemyLayers, currentBonusDamage);
            nextAttackTime = Time.time + 1f / currentAttack.attackRate;
        }
    }

    // Tuto funkci volej z UI na zaèátku hry
    public void EquipAttack(AttackBase newAttack)
    {
        currentAttack = newAttack;
        Debug.Log("Zbraò vybrána: " + currentAttack.attackName);
    }
}