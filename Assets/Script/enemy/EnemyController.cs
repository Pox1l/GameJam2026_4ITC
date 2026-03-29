using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private Transform player;
    private Animator animator;
    private NavMeshAgent agent;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 0.1f;

    [Header("Aggro Settings")]
    [SerializeField] private float aggroRange = 5f;
    [SerializeField] private float chaseRange = 15f;

    private float currentDetectionRange;
    private Vector2 homePosition; // Změněno na Vector2

    private bool isChasing = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Nastavení pro 2D NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;

        currentDetectionRange = aggroRange;

        FindPlayer();
    }

    void OnEnable()
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }

        // Kontrola pomocí Vector2
        if (homePosition == Vector2.zero) homePosition = transform.position;
        currentDetectionRange = aggroRange;
        isChasing = false;
    }

    public void OnHitAggro()
    {
        currentDetectionRange = chaseRange;
        isChasing = true;
    }

    public void SetHomePosition(Vector2 position) // Změněno na Vector2
    {
        homePosition = position;
    }

    void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        // Výpočty ve Vector2
        Vector2 currentPos = transform.position;
        Vector2 playerPos = player.position;

        float distanceToPlayer = Vector2.Distance(currentPos, playerPos);
        float distanceToHome = Vector2.Distance(currentPos, homePosition);

        Vector2 targetPosition; // Změněno na Vector2

        // --- Logika Aggra ---
        if (distanceToPlayer <= currentDetectionRange)
        {
            isChasing = true;
            targetPosition = playerPos;
        }
        else
        {
            isChasing = false;
            currentDetectionRange = aggroRange;

            if (distanceToHome <= stopDistance)
            {
                agent.ResetPath();
                SetAnimator(Vector2.zero);
                return;
            }

            targetPosition = homePosition;
        }

        // --- Pohyb ---
        if (Vector2.Distance(currentPos, targetPosition) > stopDistance)
        {
            // NavMeshAgent přijímá Vector3, ale Vector2 se tam automaticky převede (Z bude 0)
            agent.SetDestination(targetPosition);
        }
        else
        {
            agent.ResetPath();
        }

        // Animace
        Vector2 velocity2D = new Vector2(agent.velocity.x, agent.velocity.y);
        SetAnimator(velocity2D);
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void SetAnimator(Vector2 dir)
    {
        if (animator == null) return;
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
        animator.SetFloat("Speed", dir.magnitude);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}