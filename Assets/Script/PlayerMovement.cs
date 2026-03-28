using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null) animator = GetComponent<Animator>();

        // Vypnutí gravitace pro top-down
        rb.gravityScale = 0f;
    }

    void Update()
    {
        // Vstup od hráèe (WASD / šipky)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (animator != null)
        {
            animator.SetFloat("Speed", movement.sqrMagnitude);
            // PØIDÁNO: Odeslání hodnot pro Blend Tree
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
    }

    void FixedUpdate()
    {
        // Samotný pohyb hráèe (normalizovaný pro plynulý diagonální pohyb)
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}