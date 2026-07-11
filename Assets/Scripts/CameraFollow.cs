using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
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

        Vector3 desiredPosition = target.position + offset;

        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        Bounds bgBounds = background.bounds;

        float minX = bgBounds.min.x + cameraWidth;
        float maxX = bgBounds.max.x - cameraWidth;
        float minY = bgBounds.min.y + cameraHeight;
        float maxY = bgBounds.max.y - cameraHeight;

        float clampedX;
        float clampedY;

        if (minX > maxX)
        {
            clampedX = bgBounds.center.x;
        }
        else
        {
            clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        if (minY > maxY)
        {
            clampedY = bgBounds.center.y;
        }
        else
        {
            clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        Vector3 finalPosition = new Vector3(clampedX, clampedY, offset.z);

        transform.position = Vector3.Lerp(
            transform.position,
            finalPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}