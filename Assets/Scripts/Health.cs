using System; // Permite utilizar eventos (Action)
using UnityEngine; // Biblioteca principal da Unity
using UnityEngine.Events; // Permite utilizar UnityEvent no Inspector

// Script responsável por gerir a vida de qualquer objeto (jogador ou inimigo)
public class Health : MonoBehaviour
{
    // Vida máxima do objeto
    public int vidaMaxima = 100;

    // Vida atual
    public int vidaAtual;

    // Propriedade apenas de leitura que devolve a vida máxima
    public int maxHealth => vidaMaxima;

    // Propriedade apenas de leitura que devolve a vida atual
    public int currentHealth => vidaAtual;

    // Evento que pode ser configurado no Inspector para acontecer quando morrer
    public UnityEvent aoMorrer;

    // Evento chamado sempre que a vida muda
    // Envia dois valores:
    // - vidaAtual
    // - vidaMaxima
    public event Action<int, int> AoAlterarVida;

    // Indica se este objeto já morreu
    public bool isDead { get; private set; }

    // Outra forma de consultar se está morto
    public bool EstaMorto => isDead;

    // Executado antes do Start()
    void Awake()
    {
        // Inicializa a vida atual com a vida máxima
        vidaAtual = vidaMaxima;

        // Garante que o objeto começa vivo
        isDead = false;
    }

    // Executado quando o jogo começa
    void Start()
    {
        // Atualiza imediatamente todos os sistemas que mostram a vida (UI, barras, etc.)
        AoAlterarVida?.Invoke(vidaAtual, vidaMaxima);
    }

    // Função chamada quando este objeto recebe dano
    public void TakeDamage(int quantidade)
    {
        // Se já estiver morto não recebe mais dano
        if (isDead) return;

        // Subtrai o dano recebido à vida atual
        vidaAtual -= quantidade;

        // Se a vida chegou a zero ou menos
        if (vidaAtual <= 0)
        {
            // Impede que a vida fique negativa
            vidaAtual = 0;

            // Atualiza os sistemas que mostram a vida
            AoAlterarVida?.Invoke(vidaAtual, vidaMaxima);

            // Executa a morte
            Morrer();

            // Sai da função
            return;
        }

        // Atualiza os sistemas que mostram a nova vida
        AoAlterarVida?.Invoke(vidaAtual, vidaMaxima);

        // Reproduz a animação de receber dano
        TocarAnimacaoTakeHit();
    }

    // Função responsável pela morte
    void Morrer()
    {
        // Evita executar a morte mais do que uma vez
        if (isDead) return;

        // Marca este objeto como morto
        isDead = true;

        // Procura um EnemyAI neste objeto
        EnemyAI ai = GetComponent<EnemyAI>();

        // Se existir, chama a função de morte do inimigo
        if (ai != null) ai.Morrer();

        // Procura um EnemyVoadorAI neste objeto
        EnemyVoadorAI enemyVoadorAI = GetComponent<EnemyVoadorAI>();

        // Se existir, chama a função de morte do inimigo voador
        if (enemyVoadorAI != null) enemyVoadorAI.Morrer();

        // Procura um PlayerDeath neste objeto
        PlayerDeath playerDeath = GetComponent<PlayerDeath>();

        // Caso não exista, procura no objeto pai
        if (playerDeath == null)
            playerDeath = GetComponentInParent<PlayerDeath>();

        // Caso ainda não exista, procura nos filhos
        if (playerDeath == null)
            playerDeath = GetComponentInChildren<PlayerDeath>();

        // Se encontrou o componente
        if (playerDeath != null)

            // Executa a morte do jogador
            playerDeath.Morrer();

        // Executa todos os eventos configurados no Inspector
        aoMorrer?.Invoke();
    }

    // Função responsável por reproduzir a animação de levar dano
    private void TocarAnimacaoTakeHit()
    {
        // Procura o EnemyAI neste objeto
        EnemyAI enemyAI = GetComponent<EnemyAI>();

        // Caso não exista procura no pai
        if (enemyAI == null)
            enemyAI = GetComponentInParent<EnemyAI>();

        // Caso continue sem existir procura nos filhos
        if (enemyAI == null)
            enemyAI = GetComponentInChildren<EnemyAI>();

        // Se encontrou um EnemyAI
        if (enemyAI != null)
        {
            // Reproduz a animação de levar dano
            enemyAI.TocarAnimacaoTakeHit();

            // Termina a função
            return;
        }

        // Procura um EnemyVoadorAI neste objeto
        EnemyVoadorAI enemyVoadorAI = GetComponent<EnemyVoadorAI>();

        // Caso não exista procura no pai
        if (enemyVoadorAI == null)
            enemyVoadorAI = GetComponentInParent<EnemyVoadorAI>();

        // Caso continue sem existir procura nos filhos
        if (enemyVoadorAI == null)
            enemyVoadorAI = GetComponentInChildren<EnemyVoadorAI>();

        // Se encontrou um inimigo voador
        if (enemyVoadorAI != null)
        {
            // Reproduz a animação de levar dano
            enemyVoadorAI.TocarAnimacaoTakeHit();

            // Termina a função
            return;
        }

        // Se não for o jogador nem fizer parte dele
        // não existe nenhuma animação para reproduzir
        if (!CompareTag("Player") && GetComponentInParent<PlayerDeath>() == null)
            return;

        // Procura o Animator nos filhos
        Animator anim = GetComponentInChildren<Animator>();

        // Caso não exista procura no objeto pai
        if (anim == null)
            anim = GetComponentInParent<Animator>();

        // Se encontrou um Animator válido
        if (anim != null && anim.runtimeAnimatorController != null)

            // Ativa o Trigger "TakeHit"
            anim.SetTrigger("TakeHit");
    }
}
