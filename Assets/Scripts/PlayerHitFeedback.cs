using System.Collections;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public int flashCount = 4;

    private Color[] originalColors;
    private Coroutine flashCoroutine;

    void Start()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;
    }

    public void PlayHitFlash()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
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
