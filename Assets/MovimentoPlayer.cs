using UnityEngine;

public class MovimentoPlayer : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 6f;
    public float forcaPulo = 12f;

    [Header("Detecção de Chão")]
    public LayerMask layerChao;

    private Rigidbody2D rb;
    private Collider2D meuCollider;
    private bool estaNoChao;
    private float movimentoH;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Pega automaticamente o Box Collider 2D do Player
        meuCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        movimentoH = Input.GetAxisRaw("Horizontal");

        // NOVO MÉTODO: Pergunta ao próprio collider se ele está a tocar na layer do chão
        estaNoChao = meuCollider.IsTouchingLayers(layerChao);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && estaNoChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movimentoH * velocidade, rb.linearVelocity.y);
    }
}