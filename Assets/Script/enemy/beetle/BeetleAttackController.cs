using UnityEngine;
using System.Collections;

public class BeetleAttackController : MonoBehaviour
{
    [Header("Statistiky Útoku")]
    public float damage = 10f;
    public float attackRange = 2.5f;   // Kdy zaène nálet
    public float attackCooldown = 3f;
    public float hitAreaRadius = 0.8f;

    [Header("Nastavení Náletu (Dash)")]
    public float dashDistance = 3f;    // Jak daleko brouk vyletí
    public float dashSpeed = 15f;      // Jakou rychlostí
    public float returnSpeed = 5f;     // Jak rychle se vrací zpìt

    [Header("Nastavení Detekce")]
    public LayerMask playerLayer;
    public string playerTag = "Player";

    private Transform playerTransform;
    private Animator animator;
    private bool canAttack = true;
    private Vector3 originalPosition; // Místo, kam se brouk vrací

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null || !canAttack) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(PerformDashAttack());
        }
    }

    IEnumerator PerformDashAttack()
    {
        canAttack = false;
        originalPosition = transform.position;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        animator.SetFloat("AttackX", direction.x);
        animator.SetFloat("AttackY", direction.y);
        animator.SetTrigger("Attack");

        // --- NOVÉ: Poèkáme pár milisekund, než se brouk v animaci napøáhne ---
        // Nastav si podle toho, jak rychle tvoje animace zaèíná
        yield return new WaitForSeconds(0.15f);

        Vector3 targetDashPos = transform.position + (Vector3)direction * dashDistance;

        // NÁLET
        while (Vector3.Distance(transform.position, targetDashPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetDashPos, dashSpeed * Time.deltaTime);
            yield return null;
        }

        // 5. Cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Volá Animation Event (nechej ho v animaci tam, kde brouk reálnì "narazí")
    public void ApplyMeleeDamage()
    {
        float lookX = animator.GetFloat("AttackX");
        float lookY = animator.GetFloat("AttackY");
        Vector2 lookDirection = new Vector2(lookX, lookY).normalized;

        // Detekce pøímo v místì brouka (protože tam právì doletìl)
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, hitAreaRadius, playerLayer);

        if (hitPlayer != null && hitPlayer.TryGetComponent(out PlayerStats playerStats))
        {
            playerStats.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.down, hitAreaRadius);
    }
}