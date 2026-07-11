using UnityEngine;

// Controla o comportamento de Inteligência Artificial (IA) de um inimigo 2D utilizando uma Máquina de Estados.
public class EnemyAI : MonoBehaviour
{
    // --- MÁQUINA DE ESTADOS ---
    public enum Estado { Patrulha, Perseguicao, Ataque, Morto }
    public Estado estadoAtual = Estado.Patrulha;

    // --- PARÂMETROS DO ANIMATOR (CACHE) ---
    private const string AnimVelocidade = "Velocidade";
    private const string AnimAtaque = "Ataque";
    private const string AnimTakeHit = "TakeHit";
    private const string AnimMorte = "Morte";

    // --- CONFIGURAÇÕES DE DETEÇÃO ---
    [Header("Detecao")]
    public float raioVisao = 6f;
    public float raioAtaque = 1.2f;
    public LayerMask layerJogador;

    // --- CONFIGURAÇÕES DE MOVIMENTO ---
    [Header("Movimento")]
    public float velocidadePatrulha = 1.8f;
    public float velocidadePerseguicao = 3.5f;
    public float toleranciaPontoPatrulha = 0.15f;
    public Transform pontoPatrulhaA;
    public Transform pontoPatrulhaB;

    // --- CONFIGURAÇÕES DE COMBATE ---
    [Header("Combate")]
    public int danoAoJogador = 15;
    public float cooldownAtaque = 1.2f;

    // --- COMPONENTES INTERNOS E REFERÊNCIAS ---
    private Transform jogador;
    private Health healthJogador;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer[] spriteRenderers;
    private EnemyHitResponse hitResponse;

    // --- VARIÁVEIS DE CONTROLO ---
    private Transform destino;
    private float tempoUltimoAtaque = -10f; // Inicializado com valor negativo para permitir ataque imediato no início

    void Start()
    {
        // Inicialização e cache de componentes locais
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        // Procura todos os SpriteRenderers (incluindo em objetos filhos desativados) para poder virar o visual do inimigo uniformemente
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        hitResponse = GetComponent<EnemyHitResponse>();

        // Localiza o jogador na cena através da Tag
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            // Procura o componente de vida na hierarquia do jogador
            healthJogador = EncontrarComponenteNoPlayer<Health>(obj);
            // Define o transform do alvo (prioriza o transform onde está o script de vida, senão usa a raiz do objeto)
            jogador = healthJogador != null ? healthJogador.transform : obj.transform;
        }

        // Define o primeiro ponto de patrulha
        destino = pontoPatrulhaB;
    }

    void Update()
    {
        // Interrompe qualquer ação se o inimigo estiver morto
        if (estadoAtual == Estado.Morto) return;

        // Impede movimentação/ataque se o inimigo estiver sob efeito de knockback (reação a dano)
        if (hitResponse != null && hitResponse.EstaEmKnockback()) return;

        // Cancela a perseguição/ataque caso o jogador já tenha sido derrotado
        if (JogadorMorto())
        {
            PararAtaqueAoJogador();
            return;
        }

        // Avalia constantemente a distância do jogador para mudar de estado
        AtualizarEstado();

        // Executa a lógica correspondente ao estado atual do inimigo
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

    // Calcula a distância até ao jogador e atualiza o estado da máquina de estados.
    void AtualizarEstado()
    {
        if (jogador == null) return;
        if (JogadorMorto())
        {
            PararAtaqueAoJogador();
            return;
        }

        float distancia = Vector2.Distance(transform.position, jogador.position);

        // Ordem de prioridade: Ataque > Perseguição > Patrulha
        if (distancia <= raioAtaque)
            estadoAtual = Estado.Ataque;
        else if (distancia <= raioVisao)
            estadoAtual = Estado.Perseguicao;
        else
            estadoAtual = Estado.Patrulha;
    }

    // Move o inimigo alternadamente entre dois pontos de patrulha designados.
    void FazerPatrulha()
    {
        // Se os pontos não estiverem configurados no Inspetor, o inimigo fica imóvel
        if (pontoPatrulhaA == null || pontoPatrulhaB == null)
        {
            PararMovimento();
            return;
        }

        if (destino == null)
            destino = pontoPatrulhaB;

        MoverPara(destino.position, velocidadePatrulha);

        // Verifica se o inimigo chegou perto o suficiente do ponto X de destino para inverter a direção
        if (Mathf.Abs(transform.position.x - destino.position.x) <= toleranciaPontoPatrulha)
            destino = (destino == pontoPatrulhaA) ? pontoPatrulhaB : pontoPatrulhaA;
    }

    // Segue a posição do jogador com a velocidade de perseguição acrescida.
    void PerseguirJogador()
    {
        if (jogador == null) return;

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, velocidadePerseguicao);

        MoverPara(jogador.position, velocidadePerseguicao);
    }

    // Executa o ataque ao jogador respeitando o tempo de recarga (cooldown).
    void AtacarJogador()
    {
        // Validações de segurança antes de efetuar o ataque
        if (jogador == null || healthJogador == null || healthJogador.isDead)
        {
            PararAtaqueAoJogador();
            return;
        }

        // Imobiliza o inimigo fisicamente no eixo horizontal ao atacar
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Zera o parâmetro de velocidade da animação (transição para Idle/Ataque)
        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, 0f);

        // Controla o intervalo de tempo entre ataques
        if (Time.time < tempoUltimoAtaque + cooldownAtaque) return;

        tempoUltimoAtaque = Time.time;

        if (AnimatorPronto())
            anim.SetTrigger(AnimAtaque);

        // Procura na hora componentes de feedback no jogador para aplicar os efeitos visuais/físicos
        PlayerHitFeedback feedback = EncontrarComponenteNoPlayer<PlayerHitFeedback>(jogador.gameObject);
        PlayerHitResponse hitResponsePlayer = EncontrarComponenteNoPlayer<PlayerHitResponse>(jogador.gameObject);

        // Aplica o dano efetivo na vida do jogador
        if (healthJogador != null && !healthJogador.isDead)
            healthJogador.TakeDamage(danoAoJogador);

        // Ativa o flash visual de dano no jogador
        if (feedback != null && !JogadorMorto())
            feedback.PlayHitFlash();

        // Aplica uma força de empurrão (knockback) no jogador com base na posição deste inimigo
        if (hitResponsePlayer != null && !JogadorMorto())
            hitResponsePlayer.KnockbackFrom(transform.position);
    }

    // Movimenta o Rigidbody2D em direção a uma coordenada alvo, apenas no eixo X, e ajusta a direção do sprite.
    void MoverPara(Vector2 alvo, float velocidade)
    {
        if (rb == null) return;

        float diferencaX = alvo.x - transform.position.x;

        // Se estiver muito próximo do alvo no eixo X, para de andar (direção = 0), caso contrário usa Mathf.Sign para obter 1 ou -1
        float direcaoX = Mathf.Abs(diferencaX) <= toleranciaPontoPatrulha ? 0f : Mathf.Sign(diferencaX);

        // Aplica a velocidade horizontal mantendo a velocidade vertical atual (ex: gravidade)
        rb.linearVelocity = new Vector2(direcaoX * velocidade, rb.linearVelocity.y);

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, Mathf.Abs(rb.linearVelocity.x));

        // Vira os sprites dependendo da direção do movimento horizontal
        if (direcaoX > 0.1f) VirarVisual(false); // Movendo para a direita, flipX desativado
        else if (direcaoX < -0.1f) VirarVisual(true); // Movendo para a esquerda, flipX ativado
    }

    // Trava a velocidade horizontal do Rigidbody e zera a animação de corrida.
    private void PararMovimento()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, 0f);
    }

    // Método público para ser chamado externamente quando o inimigo recebe um golpe, ativando feedback de animação.
    public void TocarAnimacaoTakeHit()
    {
        if (estadoAtual == Estado.Morto) return;

        if (AnimatorPronto())
        {
            anim.SetFloat(AnimVelocidade, 0f);
            anim.SetTrigger(AnimTakeHit);
        }
    }

    // Método público que lida com a morte do inimigo: desativa colisores, físicas e agenda a destruição do GameObject.
    public void Morrer()
    {
        estadoAtual = Estado.Morto;

        // Desativa por completo a física transformando o Rigidbody in Kinematic e congelando tudo
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

        // Desativa todos os colisores no objeto e nos seus filhos para evitar interações pós-morte
        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        // Remove o inimigo da cena após 1.5 segundos (tempo para a animação de morte terminar)
        Destroy(gameObject, 1.5f);
    }

    // Restaura o estado para Patrulha e interrompe o movimento/ataque.
    public void PararAtaqueAoJogador()
    {
        if (estadoAtual != Estado.Morto)
            estadoAtual = Estado.Patrulha;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (AnimatorPronto())
            anim.SetFloat(AnimVelocidade, 0f);
    }

    // Retorna verdadeiro se o jogador tiver o componente de saúde e estiver marcado como morto.
    private bool JogadorMorto()
    {
        return healthJogador != null && healthJogador.isDead;
    }

    // Validação de segurança para garantir que o Animator existe e possui um Controller válido atribuído.
    private bool AnimatorPronto()
    {
        return anim != null && anim.runtimeAnimatorController != null;
    }

    // Função genérica auxiliar que pesquisa de forma abrangente (no próprio objeto, nos pais ou nos filhos) pelo componente solicitado no Player.
    private T EncontrarComponenteNoPlayer<T>(GameObject playerObject) where T : Component
    {
        T componente = playerObject.GetComponent<T>();
        if (componente != null) return componente;

        componente = playerObject.GetComponentInParent<T>();
        if (componente != null) return componente;

        return playerObject.GetComponentInChildren<T>();
    }

    // Altera a propriedade flipX de todos os componentes SpriteRenderer rastreados para espelhar o visual do inimigo.
    private void VirarVisual(bool paraEsquerda)
    {
        if (spriteRenderers == null) return;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = paraEsquerda;
        }
    }

    // Desenha formas geométricas na Scene do editor da Unity para ajudar a visualizar os raios de deteção e as rotas de patrulha.
    void OnDrawGizmosSelected()
    {
        // Esfera amarela para o alcance visual de perseguição
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioVisao);

        // Esfera vermelha para a distância limite de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioAtaque);

        // Linha ciana ligando os dois pontos de patrulha definidos
        if (pontoPatrulhaA != null && pontoPatrulhaB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pontoPatrulhaA.position, pontoPatrulhaB.position);
            Gizmos.DrawWireSphere(pontoPatrulhaA.position, 0.25f);
            Gizmos.DrawWireSphere(pontoPatrulhaB.position, 0.25f);
        }
    }
}