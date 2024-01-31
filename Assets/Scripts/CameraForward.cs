using NaughtyAttributes;
using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraForward : MonoBehaviour
{
    public Camera cam;
    [Header("Camera Forward Follow")]
    //public Rigidbody2D target; // 2D
    public Rigidbody target;
    public Vector3 offset;
    public float forwardDistance = 5f;
    public float changeDirSpeed = 1.8f;

    // test
    [Header("Edge Snapping")]
    public bool enableSnapping = true;
    [ShowIf("enableSnapping")]
    public float minX, maxX, minY, maxY;

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

        if (enableSnapping)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = cam.aspect * halfHeight;

            Vector2 transform = Vector2.zero;
            transform.x = Mathf.Min(Mathf.Max(cam.transform.position.x, minX + halfWidth), maxX - halfWidth);
            transform.y = Mathf.Min(Mathf.Max(cam.transform.position.y, minY + halfHeight), maxY - halfHeight);
            cam.transform.position = new Vector3(transform.x, transform.y, cam.transform.position.z);
        }
    }

    void ForwardFollow()
    {
        Vector3 targetPos = target.transform.position + offset;
        if (target.velocity != Vector3.zero)
        {
            targetPos += target.velocity.normalized * forwardDistance;
        }
        Follow(targetPos);
    }

    void Follow(Vector3 targetPos)
    {
        float x = Mathf.Lerp(cam.transform.position.x, targetPos.x, Time.deltaTime * changeDirSpeed);
        float y = Mathf.Lerp(cam.transform.position.y, targetPos.y, Time.deltaTime * changeDirSpeed);
        cam.transform.position = new Vector3(x, y, cam.transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        if (enableSnapping)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawLine(new Vector3(minX, minY, cam.transform.position.z), new Vector3(minX, maxY, cam.transform.position.z));
            Gizmos.DrawLine(new Vector3(maxX, minY, cam.transform.position.z), new Vector3(maxX, maxY, cam.transform.position.z));
            Gizmos.DrawLine(new Vector3(minX, minY, cam.transform.position.z), new Vector3(maxX, minY, cam.transform.position.z));
            Gizmos.DrawLine(new Vector3(minX, maxY, cam.transform.position.z), new Vector3(maxX, maxY, cam.transform.position.z));

            Gizmos.color = Color.red;

            float halfHeight = cam.orthographicSize;
            float halfWidth = cam.aspect * halfHeight;

            Gizmos.DrawLine(new Vector3(minX + halfWidth, minY + halfHeight, cam.transform.position.z), new Vector3(minX + halfWidth, maxY - halfHeight, cam.transform.position.z));
            Gizmos.DrawLine(new Vector3(maxX - halfWidth, minY + halfHeight, cam.transform.position.z), new Vector3(maxX - halfWidth, maxY - halfHeight, cam.transform.position.z));
            Gizmos.DrawLine(new Vector3(minX + halfWidth, minY + halfHeight, cam.transform.position.z), new Vector3(maxX - halfWidth, minY + halfHeight, cam.transform.position.z));
            Gizmos.DrawLine(new Vector3(minX + halfWidth, maxY - halfHeight, cam.transform.position.z), new Vector3(maxX - halfWidth, maxY - halfHeight, cam.transform.position.z));
        }
    }
}
