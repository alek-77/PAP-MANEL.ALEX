using UnityEngine;

public class PlayerHitResponse : MonoBehaviour
{
    // Aplica knockback ao jogador quando recebe dano de um inimigo.
    public Rigidbody2D rb;
    public float knockbackForceX = 6f;
    public float knockbackForceY = 3f;

    void Start()
    {
        if (rb == null)
        {
            // Procura o Rigidbody2D de forma flexivel na hierarquia.
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) rb = GetComponentInParent<Rigidbody2D>();
            if (rb == null) rb = GetComponentInChildren<Rigidbody2D>();
        }
    }

    public void KnockbackFrom(Vector2 damageSourcePosition)
    {
        if (rb == null) return;

        // Se a fonte do dano estiver a direita, o jogador e empurrado para a esquerda, e vice-versa.
        float direction = transform.position.x < damageSourcePosition.x ? -1f : 1f;

        // Limpa a velocidade atual para o impulso ser consistente.
        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            new Vector2(direction * knockbackForceX, knockbackForceY),
            ForceMode2D.Impulse
        );
    }
}
