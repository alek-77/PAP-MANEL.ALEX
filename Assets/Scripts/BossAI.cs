using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Movimento")]
    public float forcaSalto = 12f;
    public float velocidadeHorizontal = 6f;
    public float tempoEntreSaltos = 1.5f;

    [Header("Arena")]
    public Transform limiteEsquerdo;
    public Transform limiteDireito;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Combate")]
    public int danoAoJogador = 25;

    private Rigidbody2D rb;
    private SpriteRenderer[] sprites;
    private Transform jogador;

    private bool estaNoChao;
    private float temporizador;
    private bool podeDarDano = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>(true);

        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            jogador = obj.transform;
    }

    void Update()
    {
        estaNoChao = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer);

        if (!estaNoChao)
            return;

        temporizador += Time.deltaTime;

        if (temporizador >= tempoEntreSaltos)
        {
            Saltar();
            temporizador = 0;
        }
    }

    void Saltar()
    {
        if (jogador == null)
            return;

        int direcao;

        // Está encostado ao limite esquerdo?
        if (transform.position.x <= limiteEsquerdo.position.x)
        {
            direcao = 1;
        }
        // Está encostado ao limite direito?
        else if (transform.position.x >= limiteDireito.position.x)
        {
            direcao = -1;
        }
        else
        {
            // Salta na direção do jogador
            direcao = jogador.position.x > transform.position.x ? 1 : -1;
        }

        rb.linearVelocity = new Vector2(
            direcao * velocidadeHorizontal,
            forcaSalto);

        VirarVisual(direcao < 0);

        podeDarDano = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (!podeDarDano)
            return;

        // Só dá dano se estiver a cair
        if (rb.linearVelocity.y >= 0)
            return;

        Health hp = collision.gameObject.GetComponentInChildren<Health>();

        if (hp == null || hp.isDead)
            return;

        hp.TakeDamage(danoAoJogador);

        PlayerHitFeedback feedback =
            collision.gameObject.GetComponentInChildren<PlayerHitFeedback>();

        if (feedback != null)
            feedback.PlayHitFlash();

        PlayerHitResponse hit =
            collision.gameObject.GetComponentInChildren<PlayerHitResponse>();

        if (hit != null)
            hit.KnockbackFrom(transform.position);

        podeDarDano = false;
    }

    void VirarVisual(bool esquerda)
    {
        foreach (SpriteRenderer sr in sprites)
        {
            if (sr != null)
                sr.flipX = esquerda;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        if (limiteEsquerdo != null && limiteDireito != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                new Vector3(limiteEsquerdo.position.x, -20),
                new Vector3(limiteEsquerdo.position.x, 20));

            Gizmos.DrawLine(
                new Vector3(limiteDireito.position.x, -20),
                new Vector3(limiteDireito.position.x, 20));
        }
    }
}