using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraForward : MonoBehaviour
{
    public Camera cam;
    public Rigidbody2D target;
    public Vector2 offset;
    public float forwardDistance = 5f;
    public float changeDirSpeed = 1.8f;

    private void OnValidate()
    {
        if (target == null)
            Debug.LogError("There is no target. Please provide one. :)");
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
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
        float x = Mathf.Lerp(cam.transform.position.x, targetPos.x, Time.deltaTime * changeDirSpeed);
        float y = Mathf.Lerp(cam.transform.position.y, targetPos.y, Time.deltaTime * changeDirSpeed);
        cam.transform.position = new Vector3(x, y, cam.transform.position.z);
    }
}
