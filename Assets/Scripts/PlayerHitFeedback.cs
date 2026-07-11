using System.Collections;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    // Faz o jogador piscar quando leva dano.
    public SpriteRenderer[] spriteRenderers;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public int flashCount = 4;

    private Color[] originalColors;
    private Coroutine flashCoroutine;

    void Start()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            // Se nao forem definidos no Inspector, usa todos os sprites do jogador.
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // Guarda as cores originais para restaurar depois do flash.
        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;
    }

    public void PlayHitFlash()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0) return;

        if (flashCoroutine != null)
            // Reinicia o efeito caso o jogador leve outro hit durante o flash.
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Alterna entre a cor de dano e as cores originais.
        for (int i = 0; i < flashCount; i++)
        {
            SetColor(hitColor);
            yield return new WaitForSeconds(flashDuration);

            RestoreColors();
            yield return new WaitForSeconds(flashDuration);
        }

        flashCoroutine = null;
    }

    private void SetColor(Color color)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = color;
        }
    }

    private void RestoreColors()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].color = originalColors[i];
        }
    }
}
