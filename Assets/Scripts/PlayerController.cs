using UnityEngine; // Importa a biblioteca principal da Unity

// Classe responsável por controlar o jogador
public class PlayerController : MonoBehaviour
{
    // Organiza estas variáveis no Inspector da Unity com o título "Movement"
    [Header("Movement")]

    // Velocidade de movimento horizontal do jogador
    public float moveSpeed = 5f;

    // Força aplicada ao salto
    public float jumpForce = 10f;

    // Número máximo de saltos permitidos (2 = Double Jump)
    public int maxSaltos = 2;

    // Guarda quantos saltos ainda podem ser feitos
    private int saltosRestantes;

    // Organiza estas variáveis no Inspector com o título "Visual"
    [Header("Visual")]

    // Referência ao objeto visual (sprite) do jogador
    public Transform visualRoot;

    // Referência ao Rigidbody2D responsável pela física
    private Rigidbody2D rb;

    // Referência ao Animator das animações
    private Animator anim;

    // Lista de todos os SpriteRenderers do jogador
    private SpriteRenderer[] spriteRenderers;

    // Guarda a escala original do objeto visual
    private Vector3 visualOriginalScale = Vector3.one;

    // Indica se o jogador está no chão
    private bool isGrounded;

    // Ponto utilizado para verificar se o jogador está no chão
    [SerializeField] private Transform groundCheck;

    // Layer considerada como chão
    [SerializeField] private LayerMask groundLayer;

    // Executado apenas uma vez quando o jogo começa
    void Start()
    {
        // Procura o Rigidbody2D neste objeto
        rb = GetComponent<Rigidbody2D>();

        // Procura o Animator nos filhos do jogador
        anim = GetComponentInChildren<Animator>();

        // Se o VisualRoot não tiver sido atribuído manualmente
        if (visualRoot == null)
        {
            // Procura automaticamente um SpriteRenderer
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            // Se encontrou um SpriteRenderer
            if (spriteRenderer != null)

                // Guarda o Transform desse SpriteRenderer como visualRoot
                visualRoot = spriteRenderer.transform;
        }

        // Se existir um visualRoot
        spriteRenderers = visualRoot != null

            // Obtém todos os SpriteRenderers filhos do visualRoot
            ? visualRoot.GetComponentsInChildren<SpriteRenderer>(true)

            // Caso contrário procura em todos os filhos deste objeto
            : GetComponentsInChildren<SpriteRenderer>(true);

        // Se existir visualRoot
        if (visualRoot != null)

            // Guarda a escala original para poder inverter depois
            visualOriginalScale = visualRoot.localScale;

        // Começa com todos os saltos disponíveis
        saltosRestantes = maxSaltos;
    }

    // Executado uma vez por frame
    void Update()
    {
        // Se não existir Rigidbody, termina imediatamente
        if (rb == null) return;

        // ----------------------------
        // DETEÇÃO DO CHÃO
        // ----------------------------

        // Verifica se existe um ponto GroundCheck
        if (groundCheck != null)
        {
            // Cria um círculo invisível para verificar se toca no chão
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position, // posição da esfera
                0.15f,                // raio da esfera
                groundLayer           // layer considerada chão
            );

            // Se estiver no chão e praticamente sem velocidade vertical
            if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            {
                // Repõe todos os saltos disponíveis
                saltosRestantes = maxSaltos;
            }
        }

        // ----------------------------
        // MOVIMENTO HORIZONTAL
        // ----------------------------

        // Lê o input horizontal (-1 esquerda, 0 parado, 1 direita)
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Aplica a velocidade horizontal mantendo a velocidade vertical
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        // ----------------------------
        // SALTO
        // ----------------------------

        // Se o botão de salto foi pressionado e ainda existem saltos disponíveis
        if (Input.GetButtonDown("Jump") && saltosRestantes > 0)
        {
            // Define imediatamente a velocidade vertical do salto
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            // Consome um salto
            saltosRestantes--;

            // Marca que já não está no chão
            isGrounded = false;
        }

        // VIRAR O SPRITE

        // Se estiver a mover-se para a direita
        if (moveInput > 0)

            // Vira para a direita
            VirarVisual(false);

        // Se estiver a mover-se para a esquerda
        else if (moveInput < 0)

            // Vira para a esquerda
            VirarVisual(true);

        // ANIMAÇÕES
       
        // Se existir Animator válido
        if (AnimatorPronto())
        {
            // Envia a velocidade horizontal para o Animator
            anim.SetFloat(
                "Velocidade",
                Mathf.Abs(rb.linearVelocity.x)
            );

            // Envia a velocidade vertical
            anim.SetFloat(
                "VelocidadeY",
                rb.linearVelocity.y
            );

            // Diz ao Animator se está no chão
            anim.SetBool(
                "NoChao",
                isGrounded
            );
        }
    }

    // Desenha uma esfera verde no Editor para visualizar o GroundCheck
    void OnDrawGizmos()
    {
        // Apenas desenha se existir GroundCheck
        if (groundCheck != null)
        {
            // Cor verde
            Gizmos.color = Color.green;

            // Desenha uma esfera apenas com contorno
            Gizmos.DrawWireSphere(
                groundCheck.position,
                0.15f
            );
        }
    }

    // Função responsável por virar o sprite
    // paraEsquerda = true -> esquerda
    // paraEsquerda = false -> direita
    private void VirarVisual(bool paraEsquerda)
    {
        // Se existirem SpriteRenderers
        if (spriteRenderers != null && spriteRenderers.Length > 0)
        {
            // Percorre todos os SpriteRenderers
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                // Se existir
                if (spriteRenderer != null)

                    // Inverte horizontalmente o sprite
                    spriteRenderer.flipX = paraEsquerda;
            }

            // Sai da função porque já virou todos os sprites
            return;
        }

        // Caso não existam SpriteRenderers
        if (visualRoot != null)
        {
            // Define a direção da escala
            float direcao = paraEsquerda ? -1f : 1f;

            // Altera apenas a escala em X
            visualRoot.localScale = new Vector3(
                Mathf.Abs(visualOriginalScale.x) * direcao,
                visualOriginalScale.y,
                visualOriginalScale.z
            );
        }
    }

    // Verifica se o Animator existe e possui um Animator Controller
    private bool AnimatorPronto()
    {
        // Devolve true se ambos existirem
        return anim != null && anim.runtimeAnimatorController != null;
    }
}