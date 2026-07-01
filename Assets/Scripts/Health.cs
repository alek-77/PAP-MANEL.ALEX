using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
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
        vidaAtual = vidaMaxima;
        isDead = false;
    }

    void Start()
    {
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

        Animator anim = GetComponentInChildren<Animator>();
        if (anim == null) anim = GetComponentInParent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
            anim.SetTrigger("TakeHit");
    }
}
