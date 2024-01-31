using UnityEngine;
using System.Collections.Generic;

public class GroudCheck : MonoBehaviour
{ 
    [SerializeField]
    private List<Collider> groundColliders = new();
    [SerializeField]
    private LayerMask groundLayers;

    public int GroundContacts { 
        get
        {
            Debug.Log("Parent: " + this.gameObject.tag);
            return groundColliders.Count; 
        }
    }
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
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.IsObjectInAnyLayer(groundLayers) && groundColliders.Contains(collision))
        {
            groundColliders.Remove(collision);
        }
    }

    private void CheckGround()
    {
        //groundColliders.Clear();
        // Clear only which have invalid layer
        int deleted = 0;
        for (int i = 0; i < groundColliders.Count - deleted; i++)
        {
            if (!groundColliders[i].gameObject.IsObjectInAnyLayer(groundLayers))
            {
                groundColliders.RemoveAt(i);
                deleted++;
            }
        }

        //CapsuleCollider attachedCollider = GetComponent<CapsuleCollider>();
        //Vector3 up = attachedCollider.center + Vector3.up * (attachedCollider.height / 2 - attachedCollider.radius);
        //Vector3 down = attachedCollider.center + Vector3.down * (attachedCollider.height / 2 - attachedCollider.radius);
        SphereCollider attachedCollider = GetComponent<SphereCollider>();

        Collider[] collidersArray = new Collider[0];

        //if (Physics.OverlapCapsuleNonAlloc(down, up, GetComponent<CapsuleCollider>().radius, collidersArray) != 0)
        if (Physics.OverlapSphereNonAlloc(attachedCollider.center, attachedCollider.radius, collidersArray, groundLayers.value) != 0)
        {
            /*foreach (var col in collidersArray)
            {
                if (col.gameObject.IsObjectInAnyLayer(groundLayers) && !groundColliders.Contains(col))
                {
                    groundColliders.Add(col);
                }
            }*/
            foreach (var col in collidersArray)
            {
                if (groundColliders.Contains(col))
                {
                    groundColliders.Add(col);
                }
            }
        }
    }
}