using UnityEngine; // Biblioteca principal da Unity

// Script responsável pela morte do jogador
public class PlayerDeath : MonoBehaviour
{
    // Variável que impede que a morte seja executada mais do que uma vez
    private bool morreu;

    // Função chamada quando o jogador morre
    public void Morrer()
    {
        // Se o jogador já morreu anteriormente, termina imediatamente
        if (morreu) return;

        // Marca o jogador como morto
        morreu = true;

        // Procura o script responsável pelo movimento
        PlayerController controller = GetComponent<PlayerController>();

        // Se existir, desativa-o para impedir que o jogador se mova
        if (controller != null)
            controller.enabled = false;

        // Procura o script responsável pelos ataques
        PlayerAttack attack = GetComponent<PlayerAttack>();

        // Se existir, desativa-o para impedir novos ataques
        if (attack != null)
            attack.enabled = false;

        // Procura o Rigidbody2D do jogador
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Se existir, remove toda a velocidade atual
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Procura o Animator nos filhos do jogador
        Animator anim = GetComponentInChildren<Animator>();

        // Se existir um Animator válido
        if (anim != null && anim.runtimeAnimatorController != null)

            // Ativa a animação de morte através do Trigger "Morte"
            anim.SetTrigger("Morte");

        // Percorre todos os inimigos existentes na cena
        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>())
        {
            // Faz cada inimigo parar de atacar o jogador
            enemy.PararAtaqueAoJogador();
        }

        // Procura o HUD do jogador na cena
        PlayerHUD hud = FindAnyObjectByType<PlayerHUD>();

        // Se existir
        if (hud != null)

            // Mostra o ecrã/interface de morte
            hud.MostrarMorte();
    }
}
