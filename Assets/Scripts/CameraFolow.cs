using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFolow : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    public Vector3 offset;
    [Range(0, 10)] public float smoothFactor = 1.8f;

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

        Follow();
    }

    void Follow()
    {
        Vector2 targetPosition = target.position + offset;
        Vector2 smoothPosition = Vector2.Lerp(cam.transform.position, targetPosition, smoothFactor * Time.fixedDeltaTime);
        cam.transform.position = new Vector3(smoothPosition.x, smoothPosition.y, cam.transform.position.z);
    }
}
