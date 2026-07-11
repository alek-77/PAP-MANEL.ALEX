using System.Collections.Generic; // Permite utilizar coleções como HashSet
using UnityEngine; // Biblioteca principal da Unity

// Script responsável pelos ataques do jogador
public class PlayerAttack : MonoBehaviour
{
    // Organiza estas variáveis no Inspector com o título "Ataque"
    [Header("Ataque")]

    // Quantidade de dano causada por cada ataque
    public int dano = 25;

    // Distância (raio) do ataque
    public float alcanceAtaque = 1.5f;

    // Tempo mínimo entre ataques consecutivos
    public float cooldown = 0.4f;

    // Organiza estas variáveis no Inspector com o título "Referências"
    [Header("Referências")]

    // Ponto de onde o ataque será efetuado
    public Transform pontoAtaque;

    // Layer onde estão os inimigos
    public LayerMask layerInimigos;

    // Guarda o instante em que foi realizado o último ataque
    private float tempoUltimoAtaque = -10f;

    // Referência ao Animator do jogador
    private Animator anim;

    // Executado uma única vez quando o jogo começa
    void Start()
    {
        // Procura automaticamente o Animator nos filhos do jogador
        anim = GetComponentInChildren<Animator>();
    }

    // Executado todos os frames
    void Update()
    {
        // Obtém o SpriteRenderer para saber para que lado o jogador está virado
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        // Verifica se existe um SpriteRenderer e um ponto de ataque
        if (sr != null && pontoAtaque != null)
        {
            // Distância horizontal entre o jogador e o ponto de ataque
            // Pode ser alterada para aproximar ou afastar o ataque
            float distanciaX = 0.5f;

            // Se o sprite estiver virado para a esquerda
            if (sr.flipX)
            {
                // Coloca o ponto de ataque à esquerda do jogador
                pontoAtaque.localPosition = new Vector3(
                    -distanciaX,
                    pontoAtaque.localPosition.y,
                    pontoAtaque.localPosition.z
                );
            }
            else // Caso esteja virado para a direita
            {
                // Coloca o ponto de ataque à direita do jogador
                pontoAtaque.localPosition = new Vector3(
                    distanciaX,
                    pontoAtaque.localPosition.y,
                    pontoAtaque.localPosition.z
                );
            }
        }

        // Se a tecla J for pressionada e o cooldown já tiver terminado
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= tempoUltimoAtaque + cooldown)
        {
            // Executa o ataque
            Atacar();
        }
    }

    // Função responsável por realizar o ataque
    void Atacar()
    {
        // Se não existir ponto de ataque, termina imediatamente
        if (pontoAtaque == null) return;

        // Guarda o instante deste ataque
        tempoUltimoAtaque = Time.time;

        // Se existir um Animator válido
        if (AnimatorPronto())

            // Ativa o Trigger "Ataque" para iniciar a animação
            anim.SetTrigger("Ataque");

        // Procura todos os inimigos dentro do raio de ataque
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(
            pontoAtaque.position,   // Centro da área de ataque
            alcanceAtaque,          // Raio da área
            layerInimigos           // Layer dos inimigos
        );

        // Cria uma lista para evitar causar dano duas vezes ao mesmo inimigo
        HashSet<Health> inimigosJaAtingidos = new HashSet<Health>();

        // Percorre todos os colliders encontrados
        foreach (Collider2D inimigo in inimigosAtingidos)
        {
            // Procura o componente Health no objeto pai
            Health hp = inimigo.GetComponentInParent<Health>();

            // Ignora este inimigo se:
            // - não tiver Health
            // - já estiver morto
            // - já tiver sido atingido neste ataque
            if (hp == null || hp.isDead || inimigosJaAtingidos.Contains(hp))
                continue;

            // Aplica dano ao inimigo
            hp.TakeDamage(dano);

            // Guarda este inimigo para não voltar a receber dano neste ataque
            inimigosJaAtingidos.Add(hp);

            // Procura o componente responsável pela reação ao impacto
            EnemyHitResponse resposta = hp.GetComponent<EnemyHitResponse>();

            // Caso não exista no Health, procura no objeto pai
            if (resposta == null)
                resposta = inimigo.GetComponentInParent<EnemyHitResponse>();

            // Se encontrou o componente
            if (resposta != null)

                // Faz o inimigo reagir ao golpe indicando a posição do jogador
                resposta.ReagirAoHit(transform.position);
        }
    }

    // Desenha o raio de ataque no Editor quando o objeto está selecionado
    void OnDrawGizmosSelected()
    {
        // Se não existir ponto de ataque termina
        if (pontoAtaque == null) return;

        // Define a cor vermelha
        Gizmos.color = Color.red;

        // Desenha uma esfera apenas com contorno
        Gizmos.DrawWireSphere(
            pontoAtaque.position,
            alcanceAtaque
        );
    }

    // Verifica se existe um Animator válido
    private bool AnimatorPronto()
    {
        // Devolve true apenas se o Animator existir e tiver um Animator Controller
        return anim != null && anim.runtimeAnimatorController != null;
    }
}
