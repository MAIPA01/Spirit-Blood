using UnityEngine;
using System.Collections.Generic;

public class GroudCheck : MonoBehaviour
{ 
    [SerializeField]
    private List<Collider> groundColliders = new();
    [SerializeField]
    private LayerMask groundLayers;

    public int GroundContacts { get => groundColliders.Count; }
    public LayerMask GroundLayers
    {
        get
        {
            return groundLayers;
        }
        set
        {
            if (value != groundLayers)
            {
                groundLayers = value;
                CheckGround();
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.IsObjectInAnyLayer(groundLayers) && !groundColliders.Contains(collision))
        {
            groundColliders.Add(collision);
            //groundContats++;
            //Debug.Log("Ground contacts: " + groundContats);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.IsObjectInAnyLayer(groundLayers) && groundColliders.Contains(collision))
        {
            groundColliders.Remove(collision);
            //groundContats--;
            //Debug.Log("Ground contacts: " + groundContats);
        }
    }
    
    // TODO: TEST IT
    private void CheckGround()
    {
        groundColliders.Clear();
        List<Collider> colliders = new();
        CapsuleCollider attachedCollider = GetComponent<CapsuleCollider>();
        Vector3 up = attachedCollider.center + Vector3.up * (attachedCollider.height / 2 - attachedCollider.radius);
        Vector3 down = attachedCollider.center + Vector3.down * (attachedCollider.height / 2 - attachedCollider.radius);

        Collider[] collidersArray = colliders.ToArray();

        if (Physics.OverlapCapsuleNonAlloc(down, up, GetComponent<CapsuleCollider>().radius, collidersArray) != 0)
        {
            colliders.AddRange(collidersArray);

            foreach (var col in colliders)
            {
                if (col.gameObject.IsObjectInAnyLayer(groundLayers) && !groundColliders.Contains(col))
                {
                    groundColliders.Add(col);
                    //groundContats++;
                    //Debug.Log("Ground contacts: " + groundContats);
                }
            }
        }
    }
}