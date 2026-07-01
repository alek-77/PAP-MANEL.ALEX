using System.Collections;
using UnityEngine;

public class EnemyHitResponse : MonoBehaviour
{
    [Header("Knockback")]
    public float forcaKnockback = 5f;
    public float duracaoKnockback = 0.15f;

    [Header("Flash")]
    public float duracaoFlash = 0.1f;
    public Color corFlash = Color.white;

    private Rigidbody2D rb;
    private SpriteRenderer[] spriteRenderers;
    private Color[] coresOriginais;
    private bool emKnockback = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        coresOriginais = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            coresOriginais[i] = spriteRenderers[i].color;
    }

    public void ReagirAoHit(Vector2 origemAtaque)
    {
        Vector2 direcao = ((Vector2)transform.position - origemAtaque).normalized;

        if (rb != null)
            StartCoroutine(AplicarKnockback(direcao));

        if (spriteRenderers.Length > 0)
            StartCoroutine(FlashBranco());
    }

    IEnumerator AplicarKnockback(Vector2 direcao)
    {
        emKnockback = true;
        rb.linearVelocity = direcao * forcaKnockback;
        yield return new WaitForSeconds(duracaoKnockback);
        emKnockback = false;
    }

    IEnumerator FlashBranco()
    {
        SetColor(corFlash);
        yield return new WaitForSeconds(duracaoFlash);
        RestaurarCores();
    }

    public bool EstaEmKnockback() => emKnockback;

    private void SetColor(Color color)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = color;
        }
    }

    private void RestaurarCores()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].color = coresOriginais[i];
        }
    }
}
