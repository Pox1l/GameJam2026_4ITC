using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class BossController : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform fixedPoint;
    public Transform firePoint;
    private Animator anim;
    private NavMeshAgent agent;

    [Header("Combat Settings")]
    public float attackRange = 7f;      // Dosah, kdy zaène støílet
    public float stoppingDistance = 5f; // Jak blízko si tì pustí k tìlu
    public float attackCooldown = 2f;
    public float projectileForce = 10f;
    public LayerMask whatIsTarget;      // Musí obsahovat "Player" a "Obstacles"
    public Vector3 aimOffset = new Vector3(0, 0, 0);

    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float orbitDistance = 1.2f;

    private Transform playerTransform;
    private float lastAttackTime = -999f;
    private bool isAttacking = false;

    private float lastShotTime = -1f;
    private float shotThreshold = 0.1f; // Minimální pauza mezi výstøely v sekundách

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Nastavení pro 2D NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
    }

    void Start()
    {
        FindPlayer();

        if (fixedPoint == null || firePoint == null)
            Debug.LogError("Boss nemá pøiøazené body pro støelbu!");
    }

    void Update()
    {
        if (playerTransform == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool hasLineOfSight = CheckLineOfSight();


        // 1. OTÁÈENÍ A ANIMACE SMÌRU
       

        if (!isAttacking)
        {
            float directionToPlayer = playerTransform.position.x > transform.position.x ? 1f : -1f;
            anim.SetFloat("Horizontal", directionToPlayer);
        }

        // 2. LOGIKA ÚTOKU A POHYBU
        if (hasLineOfSight)
        {
            HandleWeaponRotation(); // Otáèí firePoint k hráèi

            if (distanceToPlayer <= attackRange)
            {
                // VIDÍ HRÁÈE A JE V DOSAHU -> STOJÍ A STØÍLÍ
                StopMovement();

                if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
                {
                    StartAttackSequence();
                }
            }
            else if (distanceToPlayer <= stoppingDistance)
            {
                StopMovement();
            }
            else
            {
                MoveToPlayer();
            }
        }
        else
        {
            // NEVIDÍ HRÁÈE -> JDE K NÌMU (Pathfinding kolem zdí)
            MoveToPlayer();
        }

        // Update animace pohybu
        anim.SetFloat("Speed", agent.velocity.magnitude > 0.1f ? 1f : 0f);
    }

    bool CheckLineOfSight()
    {
        Vector3 targetPos = playerTransform.position + aimOffset;
        Vector2 direction = (targetPos - firePoint.position).normalized;
        float distance = Vector2.Distance(firePoint.position, targetPos);

        // Raycast kontroluje pøekážky
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, distance, whatIsTarget);

        Debug.DrawRay(firePoint.position, direction * distance, hit.collider != null && hit.collider.CompareTag("Player") ? Color.green : Color.red);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    void HandleWeaponRotation()
    {
        Vector2 aimDirection = (playerTransform.position + aimOffset - fixedPoint.position).normalized;
        firePoint.position = (Vector2)fixedPoint.position + aimDirection * orbitDistance;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    void StartAttackSequence()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        anim.SetTrigger("Attack");

        // Pojistka pro ukonèení útoku, kdyby vypadl Animation Event
        Invoke("FinishAttack", 2.0f);
    }

    // --- VOLÁNO PØES ANIMATION EVENT ---
    public void Shoot()
    {
        // Pokud od posledního výstøelu uteklo ménì než 0.1s, metodu ukonèíme
        if (Time.time < lastShotTime + shotThreshold) return;

        lastShotTime = Time.time;

        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            if (proj.TryGetComponent(out Rigidbody2D rb))
            {
                Vector2 shootDir = (playerTransform.position + aimOffset - firePoint.position).normalized;
                rb.AddForce(shootDir * projectileForce, ForceMode2D.Impulse);
                FinishAttack();
            }
        }
        FinishAttack();
    }

    public void FinishAttack()
    {
        isAttacking = false;
        CancelInvoke("FinishAttack");
    }

    void MoveToPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(playerTransform.position);
    }

    void StopMovement()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}