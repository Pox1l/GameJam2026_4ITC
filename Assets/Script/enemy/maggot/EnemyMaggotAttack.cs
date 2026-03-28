using UnityEngine;
using UnityEngine.AI;

public class EnemyMaggotAttack : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform fixedPoint;
    public Transform firePoint;
    public Animator animator;

    [Header("Combat Settings")]
    public float attackRange = 7f;
    public float stoppingDistance = 5f;
    public float attackCooldown = 2f;
    public float force = 5f;
    public LayerMask whatIsTarget; // Musí obsahovat "Player" a "Obstacles"

    private Transform playerTransform;
    private float lastAttackTime = -999f;
    private NavMeshAgent agent;
    private bool isAttacking = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null || agent == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool hasLineOfSight = CheckLineOfSight();

        // 1. OTOÈENÍ K HRÁÈI (vždy když ho vidí)
        if (hasLineOfSight)
        {
            RotateToPlayer();
        }

        // 2. LOGIKA ÚTOKU A POHYBU
        if (distanceToPlayer <= attackRange && hasLineOfSight)
        {
            // VIDÍ HRÁÈE A JE V DOSAHU -> STOJÍ A STØÍLÍ
            StopMovement();

            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
            {
                StartAttackSequence();
            }
        }
        if (distanceToPlayer <= stoppingDistance && hasLineOfSight)
        {
            StopMovement();
        }
        else if (!isAttacking)
        {
            // NEVIDÍ NEBO JE DALEKO -> JDE PO NÌM
            MoveToPlayer();
        }
    }

    bool CheckLineOfSight()
    {
        Vector2 direction = (playerTransform.position - firePoint.position).normalized;
        float distance = Vector2.Distance(firePoint.position, playerTransform.position);

        // Raycast kontroluje, jestli v cestì nestojí zeï (Obstacle) døív než hráè
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, attackRange, whatIsTarget);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false;
    }

    void StartAttackSequence()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger("Attack");

        // Pojistka, kdyby se neaktivoval Animation Event "Shoot"
        Invoke("FinishAttack", 2.0f);
    }

    // Voláno pøes Animation Event
    public void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject spit = Instantiate(projectilePrefab, firePoint.position, fixedPoint.rotation);
            spit.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidBody);
            rigidBody.AddForce((playerTransform.position - firePoint.position)*force, ForceMode2D.Impulse);
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

    void RotateToPlayer()
    {
        Vector2 dir = playerTransform.position - fixedPoint.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        fixedPoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}