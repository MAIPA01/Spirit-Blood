using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraForward : MonoBehaviour
{
    public Camera cam;
    public Rigidbody2D target;
    public Vector2 offset;
    public float forwardDistance = 5f;
    [Range(0, 10)] public float smoothFactor = 1.8f;

    private Vector2 lastDir = Vector2.zero;

    private void OnValidate()
    {
        if (target == null)
            Debug.LogError("There is no target. Please provide one. :)");
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        ForwardFollow();
    }

    void ForwardFollow()
    {
        Vector2 targetPos = (Vector2)target.transform.position + offset;
        if (target.velocity != Vector2.zero)
        {
            targetPos += target.velocity.normalized * forwardDistance;
        }
        Follow(targetPos);
    }

    void Follow(Vector2 targetPos)
    {
        Vector2 smoothPosition = Vector2.Lerp(cam.transform.position, targetPos, smoothFactor * Time.fixedDeltaTime);
        cam.transform.position = new Vector3(smoothPosition.x, smoothPosition.y, cam.transform.position.z);
    }
}
