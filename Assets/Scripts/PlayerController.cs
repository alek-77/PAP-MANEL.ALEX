using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Visual")]
    public Transform visualRoot;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer[] spriteRenderers;
    private Vector3 visualOriginalScale = Vector3.one;
    private bool isGrounded;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        if (visualRoot == null)
        {
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null) visualRoot = spriteRenderer.transform;
        }

        spriteRenderers = visualRoot != null
            ? visualRoot.GetComponentsInChildren<SpriteRenderer>(true)
            : GetComponentsInChildren<SpriteRenderer>(true);

        if (visualRoot != null)
            visualOriginalScale = visualRoot.localScale;
    }

    void Update()
    {
        if (rb == null) return;

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        if (moveInput > 0) VirarVisual(false);
        else if (moveInput < 0) VirarVisual(true);

        if (AnimatorPronto())
        {
            anim.SetFloat("Velocidade", Mathf.Abs(rb.linearVelocity.x));
            anim.SetFloat("VelocidadeY", rb.linearVelocity.y);
            anim.SetBool("NoChao", isGrounded);
        }
    }

    private void VirarVisual(bool paraEsquerda)
    {
        if (spriteRenderers != null && spriteRenderers.Length > 0)
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer != null)
                    spriteRenderer.flipX = paraEsquerda;
            }

            return;
        }

        if (visualRoot != null)
        {
            float direcao = paraEsquerda ? -1f : 1f;
            visualRoot.localScale = new Vector3(
                Mathf.Abs(visualOriginalScale.x) * direcao,
                visualOriginalScale.y,
                visualOriginalScale.z
            );
        }
    }

    private bool AnimatorPronto()
    {
        return anim != null && anim.runtimeAnimatorController != null;
    }
}
