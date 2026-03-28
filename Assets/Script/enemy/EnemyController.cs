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
    private Vector3 homePosition;

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

        if (homePosition == Vector3.zero) homePosition = transform.position;
        currentDetectionRange = aggroRange;
        isChasing = false;
    }

    public void OnHitAggro()
    {
        // Pokud nepřítel dostane ránu, okamžitě přepne na chase range a začne lovit
        currentDetectionRange = chaseRange;
        isChasing = true;
    }

    public void SetHomePosition(Vector3 position)
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

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceToHome = Vector2.Distance(transform.position, homePosition);

        Vector3 targetPosition;

        // --- Logika Aggra ---
        if (distanceToPlayer <= currentDetectionRange)
        {
            isChasing = true;
            targetPosition = player.position;
        }
        else
        {
            // Hráč utekl z dosahu
            isChasing = false;
            currentDetectionRange = aggroRange;

            // Pokud jsme doma, stojíme
            if (distanceToHome <= stopDistance)
            {
                agent.ResetPath();
                SetAnimator(Vector2.zero);
                return;
            }

            targetPosition = homePosition;
        }

        // --- Pohyb ---
        if (Vector2.Distance(transform.position, targetPosition) > stopDistance)
        {
            agent.SetDestination(targetPosition);
        }
        else
        {
            agent.ResetPath();
        }

        // Animace podle rychlosti agenta
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