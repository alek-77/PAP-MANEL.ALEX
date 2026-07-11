using UnityEngine;
using UnityEngine.UI;

public class HealthBarHUD : MonoBehaviour
{
    [Header("Referencias")]
    public Health playerHealth;
    public Image healthBarImage;

    [Header("Sprites da vida")]
    public Sprite[] healthSprites;

    private void Start()
    {
        // Se a referencia nao estiver ligada no Inspector, tenta encontrar o jogador pela tag.
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
                if (playerHealth == null) playerHealth = player.GetComponentInParent<Health>();
                if (playerHealth == null) playerHealth = player.GetComponentInChildren<Health>();
            }
        }

        if (playerHealth != null)
        {
            // Subscreve o evento para atualizar a barra sempre que a vida mudar.
            playerHealth.AoAlterarVida += AtualizarVida;
            AtualizarVida(playerHealth.vidaAtual, playerHealth.vidaMaxima);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            // Remove a subscricao para evitar chamadas para um objeto destruido.
            playerHealth.AoAlterarVida -= AtualizarVida;
        }
    }

    private void AtualizarVida(int vidaAtual, int vidaMaxima)
    {
        if (healthBarImage == null || healthSprites == null || healthSprites.Length == 0)
        {
            return;
        }

        int hp = Mathf.Clamp(vidaAtual, 0, vidaMaxima);

        // Converte a percentagem de vida num indice do array de sprites.
        float percentagemVida = vidaMaxima > 0 ? hp / (float)vidaMaxima : 0f;
        int index = Mathf.CeilToInt(percentagemVida * (healthSprites.Length - 1));
        index = Mathf.Clamp(index, 0, healthSprites.Length - 1);

        healthBarImage.sprite = healthSprites[index];
    }
}
