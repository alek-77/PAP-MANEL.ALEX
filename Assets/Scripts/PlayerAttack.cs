using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Ataque")]
    public int dano = 25;
    public float alcanceAtaque = 1.5f;
    public float cooldown = 0.4f;

    [Header("Referências")]
    public Transform pontoAtaque;
    public LayerMask layerInimigos;

    private float tempoUltimoAtaque = -10f;
    private Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Ataque com J ou botão de ataque
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= tempoUltimoAtaque + cooldown)
        {
            Atacar();
        }
    }

    void Atacar()
    {
        if (pontoAtaque == null) return;

        tempoUltimoAtaque = Time.time;
        if (AnimatorPronto())
            anim.SetTrigger("Ataque");

        // Detetar inimigos no raio de ataque
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(
            pontoAtaque.position, alcanceAtaque, layerInimigos
        );

        HashSet<Health> inimigosJaAtingidos = new HashSet<Health>();

        foreach (Collider2D inimigo in inimigosAtingidos)
        {
            Health hp = inimigo.GetComponentInParent<Health>();
            if (hp == null || hp.isDead || inimigosJaAtingidos.Contains(hp)) continue;

            hp.TakeDamage(dano);
            inimigosJaAtingidos.Add(hp);

            EnemyHitResponse resposta = hp.GetComponent<EnemyHitResponse>();
            if (resposta == null)
                resposta = inimigo.GetComponentInParent<EnemyHitResponse>();

            if (resposta != null)
                resposta.ReagirAoHit(transform.position);
        }
    }

    // Visualizar o raio de ataque no editor
    void OnDrawGizmosSelected()
    {
        if (pontoAtaque == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pontoAtaque.position, alcanceAtaque);
    }

    private bool AnimatorPronto()
    {
        return anim != null && anim.runtimeAnimatorController != null;
    }
}
