using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Objeto que a camara segue, normalmente o jogador.
    public Transform target;
    // Sprite usado como limite da area visivel do nivel.
    public SpriteRenderer background;

    [Header("Follow")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 1, -10);

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null || background == null) return;

        // Calcula a posicao pretendida com o offset configurado no Inspector.
        Vector3 desiredPosition = target.position + offset;

        // Numa camara ortografica, estes valores representam metade da altura/largura visivel.
        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        Bounds bgBounds = background.bounds;

        // Limites que impedem a camara de mostrar zonas fora do background.
        float minX = bgBounds.min.x + cameraWidth;
        float maxX = bgBounds.max.x - cameraWidth;
        float minY = bgBounds.min.y + cameraHeight;
        float maxY = bgBounds.max.y - cameraHeight;

        float clampedX;
        float clampedY;

        if (minX > maxX)
        {
            // Se o background for mais pequeno do que a camara, centra no eixo X.
            clampedX = bgBounds.center.x;
        }
        else
        {
            clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        if (minY > maxY)
        {
            // Se o background for mais pequeno do que a camara, centra no eixo Y.
            clampedY = bgBounds.center.y;
        }
        else
        {
            clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        Vector3 finalPosition = new Vector3(clampedX, clampedY, offset.z);

        // Interpolacao para o seguimento ficar suave em vez de instantaneo.
        transform.position = Vector3.Lerp(
            transform.position,
            finalPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
