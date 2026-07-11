using System.Collections;
using UnityEngine;

public class EnemyVoadorAI : MonoBehaviour
{
    // Inimigo voador simples: patrulha horizontalmente e causa dano por contacto.
    public enum Estado { Patrulha, Morto }
    public Estado estadoAtual = Estado.Patrulha;

    private const string AnimVelocidade = "Velocidade";
    private const string AnimAtaque = "Ataque";
    private const string AnimTakeHit = "TakeHit";
    private const string AnimMorte = "Morte";

    [Header("Animacoes")]
    public string estadoFly = "EnemyVoador-Fly";
    public float duracaoAnimacaoAtaque = 0.5f;

    [Header("Movimento")]
    public float velocidadePatrulha = 2f;
    public float distanciaPatrulhaAutomatica = 3f;
    public float toleranciaPontoPatrulha = 0.2f;
    public Transform pontoPatrulhaA;
    public Transform pontoPatrulhaB;

    [Header("Combate")]
    public int danoAoJogador = 10;
    public float cooldownDano = 1f;

    [Header("Morte")]
    public float gravidadeAoMorrer = 1f;
    public float tempoParaDestruirDepoisDeMorrer = 2.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer[] spriteRenderers;
    private EnemyHitResponse hitResponse;

    private float tempoUltimoDano = -10f;
    private Vector2 posicaoInicial;
    private float alturaBase;
    private bool irParaPontoB = true;
    private bool playerEncostado;
    private Coroutine rotinaAtaque;

    void Start()
    {
        // Prepara referencias e desativa a gravidade enquanto o inimigo esta vivo.
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        hitResponse = GetComponent<EnemyHitResponse>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        posicaoInicial = transform.position;
        alturaBase = transform.position.y;
    }

    void Update()
    {
        if (estadoAtual == Estado.Morto) return;
        // Durante o knockback, a patrulha fica pausada.
        if (hitResponse != null && hitResponse.EstaEmKnockback()) return;

        FazerPatrulha();
    }

    void FazerPatrulha()
    {
        Vector2 pontoA = ObterPontoPatrulhaA();
        Vector2 pontoB = ObterPontoPatrulhaB();
        Vector2 destino = irParaPontoB ? pontoB : pontoA;
        Vector2 alvo = new Vector2(destino.x, alturaBase);

        MoverPara(alvo, velocidadePatrulha);

        if (Mathf.Abs(transform.position.x - destino.x) <= toleranciaPontoPatrulha)
            // Quando chega ao extremo da patrulha, troca de direcao.
            irParaPontoB = !irParaPontoB;
    }

    void DarDanoAoJogador(GameObject playerObject)
    {
        if (estadoAtual == Estado.Morto) return;
        // Evita dano repetido no mesmo contacto e respeita o cooldown global.
        if (playerEncostado) return;
        if (Time.time < tempoUltimoDano + cooldownDano) return;

        Health healthJogador = EncontrarComponenteNoPlayer<Health>(playerObject);
        if (healthJogador == null || healthJogador.isDead) return;

        playerEncostado = true;
        tempoUltimoDano = Time.time;
        if (AnimatorPronto())
        {
            // Toca o ataque e depois regressa ao estado visual de voo.
            anim.ResetTrigger(AnimAtaque);
            anim.SetTrigger(AnimAtaque);

            if (rotinaAtaque != null)
                StopCoroutine(rotinaAtaque);
            rotinaAtaque = StartCoroutine(VoltarParaFlyDepoisDoAtaque());
        }

        PlayerHitFeedback feedback = EncontrarComponenteNoPlayer<PlayerHitFeedback>(playerObject);
        PlayerHitResponse hitResponsePlayer = EncontrarComponenteNoPlayer<PlayerHitResponse>(playerObject);

        if (!healthJogador.isDead)
            healthJogador.TakeDamage(danoAoJogador);

        if (feedback != null && !healthJogador.isDead)
            feedback.PlayHitFlash();

        if (hitResponsePlayer != null && !healthJogador.isDead)
            hitResponsePlayer.KnockbackFrom(transform.position);
    }

    void MoverPara(Vector2 alvo, float velocidade)
    {
        if (rb == null)
        {
            // Alternativa caso o objeto nao tenha Rigidbody2D.
            transform.position = Vector2.MoveTowards(transform.position, alvo, velocidade * Time.deltaTime);
            if (AnimatorPronto())
                anim.SetFloat(AnimVelocidade, velocidade);
            VirarPara(alvo.x - transform.position.x);
            return;
        }

        Vector2 direcao = (alvo - (Vector2)transform.position).normalized;
        rb.linearVelocity = direcao * velocidade;
        rb.WakeUp();

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, rb.linearVelocity.magnitude);

        VirarPara(rb.linearVelocity.x);
    }

    void VoarParado()
    {
        if (rb == null) return;

        rb.linearVelocity = Vector2.zero;

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, 0f);
    }

    Vector2 ObterPontoPatrulhaA()
    {
        // Usa o ponto definido no Inspector; se nao existir, cria um ponto automatico.
        if (pontoPatrulhaA != null)
            return pontoPatrulhaA.position;

        return new Vector2(posicaoInicial.x - distanciaPatrulhaAutomatica, alturaBase);
    }

    Vector2 ObterPontoPatrulhaB()
    {
        // Usa o ponto definido no Inspector; se nao existir, cria um ponto automatico.
        if (pontoPatrulhaB != null)
            return pontoPatrulhaB.position;

        return new Vector2(posicaoInicial.x + distanciaPatrulhaAutomatica, alturaBase);
    }

    void PararMovimento()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, 0f);
    }

    public void TocarAnimacaoTakeHit()
    {
        if (estadoAtual == Estado.Morto) return;

        if (AnimatorPronto())
            anim.SetTrigger(AnimTakeHit);
    }

    public void Morrer()
    {
        estadoAtual = Estado.Morto;

        if (rotinaAtaque != null)
        {
            StopCoroutine(rotinaAtaque);
            rotinaAtaque = null;
        }

        if (rb != null)
        {
            // Ao morrer, volta a sofrer gravidade para cair.
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = gravidadeAoMorrer;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (AnimatorPronto())
        {
            anim.SetFloat(AnimVelocidade, 0f);
            anim.SetTrigger(AnimMorte);
        }

        Destroy(gameObject, tempoParaDestruirDepoisDeMorrer);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (ObjetoDoPlayer(collision.gameObject))
            DarDanoAoJogador(collision.gameObject);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (ObjetoDoPlayer(collision.gameObject))
            playerEncostado = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ObjetoDoPlayer(other.gameObject))
            DarDanoAoJogador(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (ObjetoDoPlayer(other.gameObject))
            playerEncostado = false;
    }

    bool AnimatorPronto()
    {
        return anim != null && anim.runtimeAnimatorController != null;
    }

    IEnumerator VoltarParaFlyDepoisDoAtaque()
    {
        yield return new WaitForSeconds(duracaoAnimacaoAtaque);

        rotinaAtaque = null;
        if (estadoAtual == Estado.Morto || !AnimatorPronto()) yield break;

        anim.ResetTrigger(AnimAtaque);

        if (!string.IsNullOrEmpty(estadoFly))
            // CrossFade evita uma transicao brusca de volta ao voo.
            anim.CrossFade(estadoFly, 0.05f);
    }

    T EncontrarComponenteNoPlayer<T>(GameObject playerObject) where T : Component
    {
        // Procura no objeto, no pai e nos filhos para suportar varias hierarquias do jogador.
        T componente = playerObject.GetComponent<T>();
        if (componente != null) return componente;

        componente = playerObject.GetComponentInParent<T>();
        if (componente != null) return componente;

        return playerObject.GetComponentInChildren<T>();
    }

    bool ObjetoDoPlayer(GameObject obj)
    {
        // Nem sempre o collider atingido e o GameObject principal do jogador.
        return obj.CompareTag("Player")
            || obj.GetComponentInParent<PlayerController>() != null
            || obj.GetComponentInParent<PlayerDeath>() != null;
    }

    void VirarPara(float direcaoX)
    {
        if (spriteRenderers == null) return;
        if (Mathf.Abs(direcaoX) <= 0.1f) return;

        bool paraEsquerda = direcaoX < 0f;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = paraEsquerda;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Desenha a linha de patrulha no editor para facilitar o ajuste do nivel.
        Vector2 centro = Application.isPlaying ? posicaoInicial : (Vector2)transform.position;
        Vector2 pontoA = pontoPatrulhaA != null
            ? (Vector2)pontoPatrulhaA.position
            : new Vector2(centro.x - distanciaPatrulhaAutomatica, centro.y);
        Vector2 pontoB = pontoPatrulhaB != null
            ? (Vector2)pontoPatrulhaB.position
            : new Vector2(centro.x + distanciaPatrulhaAutomatica, centro.y);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pontoA, pontoB);
        Gizmos.DrawWireSphere(pontoA, 0.25f);
        Gizmos.DrawWireSphere(pontoB, 0.25f);
    }
}
