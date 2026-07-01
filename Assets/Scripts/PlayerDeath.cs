using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private bool morreu;

    public void Morrer()
    {
        if (morreu) return;
        morreu = true;

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null) attack.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
            anim.SetTrigger("Morte");

        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>())
            enemy.PararAtaqueAoJogador();

        PlayerHUD hud = FindAnyObjectByType<PlayerHUD>();
        if (hud != null) hud.MostrarMorte();
    }
}
