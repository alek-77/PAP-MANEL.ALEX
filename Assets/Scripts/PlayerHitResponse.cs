using UnityEngine;

public class PlayerHitResponse : MonoBehaviour
{
    public Rigidbody2D rb;
    public float knockbackForceX = 6f;
    public float knockbackForceY = 3f;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) rb = GetComponentInParent<Rigidbody2D>();
            if (rb == null) rb = GetComponentInChildren<Rigidbody2D>();
        }
    }

    public void KnockbackFrom(Vector2 damageSourcePosition)
    {
        if (rb == null) return;

        float direction = transform.position.x < damageSourcePosition.x ? -1f : 1f;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            new Vector2(direction * knockbackForceX, knockbackForceY),
            ForceMode2D.Impulse
        );
    }
}
