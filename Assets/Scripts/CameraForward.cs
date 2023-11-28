using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraForward : MonoBehaviour
{
    public Camera cam;
    public Transform target;
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
        Vector2 targetPosition = (Vector2)target.position + offset + lastDir * forwardDistance;
        if (targetPosition - (Vector2)cam.transform.position == Vector2.zero)
        {
            cam.transform.position = new Vector3(target.position.x, target.position.y, cam.transform.position.z);
        }
        else
        {
            lastDir = (targetPosition - (Vector2)cam.transform.position).normalized;
            cam.transform.position = new Vector3(targetPosition.x, targetPosition.y, cam.transform.position.z);
        }
    }
}
