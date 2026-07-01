using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum Estado { Patrulha, Perseguicao, Ataque, Morto }
    public Estado estadoAtual = Estado.Patrulha;

    private const string AnimVelocidade = "Velocidade";
    private const string AnimAtaque = "Ataque";
    private const string AnimTakeHit = "TakeHit";
    private const string AnimMorte = "Morte";

    [Header("Detecao")]
    public float raioVisao = 6f;
    public float raioAtaque = 1.2f;
    public LayerMask layerJogador;

    [Header("Movimento")]
    public float velocidadePatrulha = 1.8f;
    public float velocidadePerseguicao = 3.5f;
    public float toleranciaPontoPatrulha = 0.15f;
    public Transform pontoPatrulhaA;
    public Transform pontoPatrulhaB;

    [Header("Combate")]
    public int danoAoJogador = 15;
    public float cooldownAtaque = 1.2f;

    private Transform jogador;
    private Health healthJogador;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer[] spriteRenderers;
    private EnemyHitResponse hitResponse;

    private Transform destino;
    private float tempoUltimoAtaque = -10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        hitResponse = GetComponent<EnemyHitResponse>();

        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            healthJogador = EncontrarComponenteNoPlayer<Health>(obj);
            jogador = healthJogador != null ? healthJogador.transform : obj.transform;
        }

        destino = pontoPatrulhaB;
    }

    void Update()
    {
        if (estadoAtual == Estado.Morto) return;
        if (hitResponse != null && hitResponse.EstaEmKnockback()) return;
        if (JogadorMorto())
        {
            PararAtaqueAoJogador();
            return;
        }

        AtualizarEstado();

        switch (estadoAtual)
        {
            case Estado.Patrulha:
                FazerPatrulha();
                break;
            case Estado.Perseguicao:
                PerseguirJogador();
                break;
            case Estado.Ataque:
                AtacarJogador();
                break;
        }
    }

    void AtualizarEstado()
    {
        if (jogador == null) return;
        if (JogadorMorto())
        {
            PararAtaqueAoJogador();
            return;
        }

        float distancia = Vector2.Distance(transform.position, jogador.position);

        if (distancia <= raioAtaque)
            estadoAtual = Estado.Ataque;
        else if (distancia <= raioVisao)
            estadoAtual = Estado.Perseguicao;
        else
            estadoAtual = Estado.Patrulha;
    }

    void FazerPatrulha()
    {
        if (pontoPatrulhaA == null || pontoPatrulhaB == null)
        {
            PararMovimento();
            return;
        }

        if (destino == null)
            destino = pontoPatrulhaB;

        MoverPara(destino.position, velocidadePatrulha);

        if (Mathf.Abs(transform.position.x - destino.position.x) <= toleranciaPontoPatrulha)
            destino = (destino == pontoPatrulhaA) ? pontoPatrulhaB : pontoPatrulhaA;
    }

    void PerseguirJogador()
    {
        if (jogador == null) return;

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, velocidadePerseguicao);
        MoverPara(jogador.position, velocidadePerseguicao);
    }

    void AtacarJogador()
    {
        if (jogador == null || healthJogador == null || healthJogador.isDead)
        {
            PararAtaqueAoJogador();
            return;
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, 0f);

        if (Time.time < tempoUltimoAtaque + cooldownAtaque) return;

        tempoUltimoAtaque = Time.time;
        if (AnimatorPronto())
            anim.SetTrigger(AnimAtaque);

        PlayerHitFeedback feedback = EncontrarComponenteNoPlayer<PlayerHitFeedback>(jogador.gameObject);
        PlayerHitResponse hitResponsePlayer = EncontrarComponenteNoPlayer<PlayerHitResponse>(jogador.gameObject);

        if (healthJogador != null && !healthJogador.isDead)
            healthJogador.TakeDamage(danoAoJogador);

        if (feedback != null && !JogadorMorto())
            feedback.PlayHitFlash();

        if (hitResponsePlayer != null && !JogadorMorto())
            hitResponsePlayer.KnockbackFrom(transform.position);
    }

    void MoverPara(Vector2 alvo, float velocidade)
    {
        if (rb == null) return;

        float diferencaX = alvo.x - transform.position.x;
        float direcaoX = Mathf.Abs(diferencaX) <= toleranciaPontoPatrulha ? 0f : Mathf.Sign(diferencaX);
        rb.linearVelocity = new Vector2(direcaoX * velocidade, rb.linearVelocity.y);

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, Mathf.Abs(rb.linearVelocity.x));

        if (direcaoX > 0.1f) VirarVisual(false);
        else if (direcaoX < -0.1f) VirarVisual(true);
    }

    private void PararMovimento()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, 0f);
    }

    public void TocarAnimacaoTakeHit()
    {
        if (estadoAtual == Estado.Morto) return;

        if (AnimatorPronto())
        {
            anim.SetFloat(AnimVelocidade, 0f);
            anim.SetTrigger(AnimTakeHit);
        }
    }

    public void Morrer()
    {
        estadoAtual = Estado.Morto;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (AnimatorPronto())
        {
            anim.SetFloat(AnimVelocidade, 0f);
            anim.SetTrigger(AnimMorte);
        }

        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        Destroy(gameObject, 1.5f);
    }

    public void PararAtaqueAoJogador()
    {
        if (estadoAtual != Estado.Morto)
            estadoAtual = Estado.Patrulha;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, 0f);
    }

    private bool JogadorMorto()
    {
        return healthJogador != null && healthJogador.isDead;
    }

    private bool AnimatorPronto()
    {
        return anim != null && anim.runtimeAnimatorController != null;
    }

    private T EncontrarComponenteNoPlayer<T>(GameObject playerObject) where T : Component
    {
        T componente = playerObject.GetComponent<T>();
        if (componente != null) return componente;

        componente = playerObject.GetComponentInParent<T>();
        if (componente != null) return componente;

        return playerObject.GetComponentInChildren<T>();
    }

    private void VirarVisual(bool paraEsquerda)
    {
        if (spriteRenderers == null) return;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = paraEsquerda;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioVisao);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioAtaque);

        if (pontoPatrulhaA != null && pontoPatrulhaB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pontoPatrulhaA.position, pontoPatrulhaB.position);
            Gizmos.DrawWireSphere(pontoPatrulhaA.position, 0.25f);
            Gizmos.DrawWireSphere(pontoPatrulhaB.position, 0.25f);
        }
    }
}
