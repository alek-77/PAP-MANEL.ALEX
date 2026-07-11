using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    // Impede que a rotina de morte seja executada mais do que uma vez.
    private bool morreu;

    public void Morrer()
    {
        if (morreu) return;
        morreu = true;

        // Desativa controlos para o jogador deixar de se mexer/atacar depois da morte.
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null) attack.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Toca a animacao de morte, se existir um Animator valido.
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
            anim.SetTrigger("Morte");

        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>())
            // Evita que os inimigos continuem a atacar um jogador morto.
            enemy.PararAtaqueAoJogador();

        PlayerHUD hud = FindAnyObjectByType<PlayerHUD>();
        if (hud != null) hud.MostrarMorte();
    }
}
