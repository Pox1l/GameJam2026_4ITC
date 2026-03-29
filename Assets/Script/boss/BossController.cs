using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    public Transform player;
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Hierarchy Reference (Z image_7.png)")]
    [Tooltip("Pivot bod na tìle bosse, kolem kterého zbraò rotuje (Boss -> FixedPoint)")]
    public Transform fixedPoint;
    [Tooltip("Bod výstøelu, který obíhá (Boss -> FixedPoint -> Fire point)")]
    public Transform firePoint;

    [Header("Nastavení pohybu a rotace zbranì")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 5f; // Vzdálenost, kde boss zastaví a støílí
    [Tooltip("Vzdálenost FirePointu od FixedPointu (polomìr orbity)")]
    public float orbitDistance = 1.2f;

    [Header("Útok na dálku")]
    public float attackCooldown = 3f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 7f;

    [Header("Editor Visualization")]
    [SerializeField] private Color rangeColor = new Color(1, 0, 0, 0.3f);
    [SerializeField] private Color orbitColor = new Color(0, 1, 1, 0.5f);
    [SerializeField] private float gizmoSize = 0.3f;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Kontrola referencí
        if (fixedPoint == null || firePoint == null)
        {
            Debug.LogError("Boss nemá pøiøazené FixedPoint nebo FirePoint! Zkontroluj Hierarchy podle image_7.png.");
        }
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 1. OTÁÈENÍ BOSSE (Pro Blend Tree v Animatoru - image_3.png context)
        // Vypoèítáme, zda je hráè vlevo (-1) nebo vpravo (1) pro sprite
        float directionToPlayer = player.position.x > transform.position.x ? 1f : -1f;
        anim.SetFloat("Horizontal", directionToPlayer);

        // 2. POHYB NEBO ÚTOK
        if (distance > stoppingDistance)
        {
            MoveTowardsPlayer();
            anim.SetFloat("Speed", 1f); // Zapne animaci chùze
        }
        else
        {
            rb.velocity = Vector2.zero;
            anim.SetFloat("Speed", 0f); // Idle

            // Pokud je v dosahu a cooldown vypršel, spus sekvenci útoku
            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(AttackSequence());
            }
        }
    }

    // Rotaci a pozici FirePointu øešíme v LateUpdate, aby Cinemachine (pokud je) nezpùsobovala tøas
    void LateUpdate()
    {
        if (player == null || fixedPoint == null || firePoint == null) return;
        HandleWeaponRotation();
    }

    // --- LOGIKA ROTACE ZBRANÌ ---
    void HandleWeaponRotation()
    {
        // Smìr k hráèi
        Vector2 aimDirection = (player.position - fixedPoint.position).normalized;

        // Aktualizace pozice FirePointu (orbita kolem FixedPointu)
        firePoint.position = (Vector2)fixedPoint.position + aimDirection * orbitDistance;

        // Výpoèet úhlu
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Otoèení FirePointu tak, aby jeho 'right' vektor míøil na hráèe
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        // Spustí animaci útoku (v Animatoru nastav trigger "RangedAttack" - image_3.png context)
        // anim.SetTrigger("RangedAttack");
        anim.SetBool("Attack", true); // Pøidáno pro jistotu, pokud máš Bool parametrem

        nextAttackTime = Time.time + attackCooldown;

        // Èekáme, než animace nápøahu dobìhne k Eventu (uprav èas)
        yield return new WaitForSeconds(0.5f);

        //anim.SetBool("Attack", false); // Vypne útoèný stav
        isAttacking = false;
    }

    // --- TUTO FUNKCI ZAVOLÁŠ PØES ANIMATION EVENT ---
    public void ShootProjectile()
    {
        if (player == null || projectilePrefab == null || firePoint == null) return;

        // Vytvoøíme støelu na FirePoint.position s orientací FirePointu
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Nastavíme rychlost støely ve smìru, kam míøí FirePoint
        if (proj.TryGetComponent(out Rigidbody2D projRb))
        {
            // Standardní 2D projektily smìøují 'doprava'. FirePoint.right míøí na hráèe.
            projRb.velocity = firePoint.right * projectileSpeed;

            // Zabezpeèíme, aby sprite støely byl otoèen ve smìru letu (pokud letí šikmo)
            Vector2 moveDir = projRb.velocity;
            if (moveDir != Vector2.zero)
            {
                float projAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                proj.transform.rotation = Quaternion.Euler(0, 0, projAngle);
            }
        }
    }

    // Vizualizace orbity a spawn pointu v Editoru
    void OnDrawGizmosSelected()
    {
        // Vykreslení dosahu støelby (stopping distance)
        Gizmos.color = rangeColor;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        if (fixedPoint != null)
        {
            // Nakreslí støed otáèení
            Gizmos.color = orbitColor;
            Gizmos.DrawWireSphere(fixedPoint.position, gizmoSize);

            // Nakreslí polomìr orbity
            Gizmos.DrawWireSphere(fixedPoint.position, orbitDistance);

            if (firePoint != null)
            {
                // Èára k aktuální pozici FirePointu
                Gizmos.DrawLine(fixedPoint.position, firePoint.position);

                // Nakreslí bod výstøelu
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(firePoint.position, new Vector3(gizmoSize, gizmoSize, gizmoSize));

                // Ukázka smìru výstøelu
                Gizmos.DrawRay(firePoint.position, firePoint.right * 1f);
            }
        }
    }
}