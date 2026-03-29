using UnityEngine;
using System.Collections;

public class MantisAttackController : MonoBehaviour
{
    [Header("Statistiky Útoku")]
    public float damage = 10f;
    public float attackRange = 1.5f;   // Vzdálenost, kdy Kudlanka zaène útoèit
    public float attackCooldown = 2f; // Prodleva mezi útoky
    public float hitAreaRadius = 0.6f; // Velikost zóny zásahu

    [Header("Nastavení Detekce")]
    public LayerMask playerLayer;      // Vrstva hráèe
    public string playerTag = "Player";

    private Transform playerTransform;
    private Animator animator;
    private bool canAttack = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Najdeme hráèe ve scénì
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null || !canAttack) return;

        // Kontrola vzdálenosti k hráèi
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(PerformAttackSequence());
        }
    }

    IEnumerator PerformAttackSequence()
    {
        canAttack = false;

        // 1. Výpoèet smìru pro Blend Tree (urèí, která animace se pustí - L,R,Up,Down)
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // 2. Nastavení parametrù v Animatoru (AttackX, AttackY)
        animator.SetFloat("AttackX", direction.x);
        animator.SetFloat("AttackY", direction.y);

        // 3. Spuštìní animace triggerem 'Attack'
        animator.SetTrigger("Attack");

        // Èekání na cooldown, než bude moci kudlanka útoèit znovu
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // --- TUTO FUNKCI VOLÁ ANIMATION EVENT ---
    // Musíš ji pøidat do svých 4 útoèných animací v oknì Animation!
    public void ApplyMeleeDamage()
    {
        // Bezpeènostní kontrola, pokud by hráè mezitím zmizel
        if (playerTransform == null) return;

        // --- ZMÌNA ZDE ---
        // Vypoèítáme aktuální smìr pøímo k hráèi v tento moment,
        // ignorujeme smìr zapsaný v Animatoru.
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Výpoèet støedu kruhu zásahu (v aktuálním smìru k hráèi)
        Vector2 hitPosition = (Vector2)transform.position + directionToPlayer * (attackRange * 0.7f);

        // Kontrola, zda je hráè v kruhu
        Collider2D hitPlayer = Physics2D.OverlapCircle(hitPosition, hitAreaRadius, playerLayer);

        if (hitPlayer != null)
        {
            if (hitPlayer.TryGetComponent(out PlayerStats playerStats))
            {
                playerStats.TakeDamage(damage);
                Debug.Log("<color=green>Mantis úspìšnì zasáhla hráèe ve smìru k nìmu!</color>");
            }
        }
    }

    // Vizualizace zásahové zóny v editoru pro snadnìjší ladìní
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Pokud hra bìží a máme hráèe, Gizmo ukazuje aktuální smìr útoku k hráèi
        if (Application.isPlaying && playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Vector2 hitPosition = (Vector2)transform.position + directionToPlayer * (attackRange * 0.7f);
            Gizmos.DrawWireSphere(hitPosition, hitAreaRadius);
        }
        else
        {
            // Náhled v klidu v editoru (smìrem dolù, nebo výchozí smìr)
            Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.down * (attackRange * 0.7f), hitAreaRadius);
        }
    }
}