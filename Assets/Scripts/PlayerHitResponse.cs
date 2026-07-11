using UnityEngine; // Biblioteca principal da Unity

// Script responsável por aplicar o efeito de knockback (empurrão)
// quando o jogador recebe dano
public class PlayerHitResponse : MonoBehaviour
{
    // Referência ao Rigidbody2D do jogador
    public Rigidbody2D rb;

    // Força horizontal do knockback
    public float knockbackForceX = 6f;

    // Força vertical do knockback
    public float knockbackForceY = 3f;

    // Executado uma única vez quando o jogo começa
    void Start()
    {
        // Se o Rigidbody2D não tiver sido atribuído no Inspector
        if (rb == null)
        {
            // Procura o Rigidbody2D neste objeto
            rb = GetComponent<Rigidbody2D>();

            // Se não encontrar, procura no objeto pai
            if (rb == null)
                rb = GetComponentInParent<Rigidbody2D>();

            // Se ainda não encontrar, procura nos filhos
            if (rb == null)
                rb = GetComponentInChildren<Rigidbody2D>();
        }
    }

    // Função chamada quando o jogador recebe dano
    // Recebe como parâmetro a posição da origem do ataque
    public void KnockbackFrom(Vector2 damageSourcePosition)
    {
        // Se não existir Rigidbody2D termina imediatamente
        if (rb == null) return;

        // Calcula para que lado o jogador deve ser empurrado
        // Se o inimigo estiver à direita, empurra para a esquerda (-1)
        // Caso contrário empurra para a direita (+1)
        float direction = transform.position.x < damageSourcePosition.x ? -1f : 1f;

        // Remove completamente a velocidade atual
        rb.linearVelocity = Vector2.zero;

        // Aplica uma força instantânea (Impulse)
        rb.AddForce(
            // Vetor da força aplicada
            new Vector2(
                direction * knockbackForceX, // força horizontal
                knockbackForceY              // força vertical
            ),

            // Faz com que a força seja aplicada imediatamente
            ForceMode2D.Impulse
        );
    }
}
