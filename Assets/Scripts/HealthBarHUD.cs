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
            playerHealth.AoAlterarVida += AtualizarVida;
            AtualizarVida(playerHealth.vidaAtual, playerHealth.vidaMaxima);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
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

        float percentagemVida = vidaMaxima > 0 ? hp / (float)vidaMaxima : 0f;
        int index = Mathf.CeilToInt(percentagemVida * (healthSprites.Length - 1));
        index = Mathf.Clamp(index, 0, healthSprites.Length - 1);

        healthBarImage.sprite = healthSprites[index];
    }
}
