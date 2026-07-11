using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // Componente generico de vida usado pelo jogador e pelos inimigos.
    public int vidaMaxima = 100;
    public int vidaAtual;
    public int maxHealth => vidaMaxima;
    public int currentHealth => vidaAtual;
    public UnityEvent aoMorrer;

    public event Action<int, int> AoAlterarVida;
    public bool isDead { get; private set; }
    public bool EstaMorto => isDead;

    void Awake()
    {
        // Cada objeto comeca vivo e com a vida cheia.
        vidaAtual = vidaMaxima;
        isDead = false;
    }

    void Start()
    {
        // Avisa a UI e outros sistemas do valor inicial da vida.
        AoAlterarVida?.Invoke(vidaAtual, vidaMaxima);
    }

    public void TakeDamage(int quantidade)
    {
        if (isDead) return;

        vidaAtual -= quantidade;
        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            AoAlterarVida?.Invoke(vidaAtual, vidaMaxima);
            // A morte e tratada uma unica vez para evitar chamadas repetidas.
            Morrer();
            return;
        }

        AoAlterarVida?.Invoke(vidaAtual, vidaMaxima);
        TocarAnimacaoTakeHit();
    }

    void Morrer()
    {
        if (isDead) return;

        isDead = true;

        // Procura o tipo de controlo presente neste objeto e chama a morte correta.
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null) ai.Morrer();

        EnemyVoadorAI enemyVoadorAI = GetComponent<EnemyVoadorAI>();
        if (enemyVoadorAI != null) enemyVoadorAI.Morrer();

        PlayerDeath playerDeath = GetComponent<PlayerDeath>();
        if (playerDeath == null) playerDeath = GetComponentInParent<PlayerDeath>();
        if (playerDeath == null) playerDeath = GetComponentInChildren<PlayerDeath>();
        if (playerDeath != null) playerDeath.Morrer();

        aoMorrer?.Invoke();
    }

    private void TocarAnimacaoTakeHit()
    {
        // Primeiro tenta animacoes especificas dos inimigos.
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI == null) enemyAI = GetComponentInParent<EnemyAI>();
        if (enemyAI == null) enemyAI = GetComponentInChildren<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.TocarAnimacaoTakeHit();
            return;
        }

        EnemyVoadorAI enemyVoadorAI = GetComponent<EnemyVoadorAI>();
        if (enemyVoadorAI == null) enemyVoadorAI = GetComponentInParent<EnemyVoadorAI>();
        if (enemyVoadorAI == null) enemyVoadorAI = GetComponentInChildren<EnemyVoadorAI>();
        if (enemyVoadorAI != null)
        {
            enemyVoadorAI.TocarAnimacaoTakeHit();
            return;
        }

        if (!CompareTag("Player") && GetComponentInParent<PlayerDeath>() == null)
            return;

        // Se for o jogador, usa diretamente o Animator encontrado na hierarquia.
        Animator anim = GetComponentInChildren<Animator>();
        if (anim == null) anim = GetComponentInParent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
            anim.SetTrigger("TakeHit");
    }
}
