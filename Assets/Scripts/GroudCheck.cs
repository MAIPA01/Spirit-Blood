using UnityEngine;
using System.Collections.Generic;

public class GroudCheck : MonoBehaviour
{ 
    [SerializeField]
    private List<Collider2D> groundColliders = new();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.IsObjectInAnyLayer(groundLayers) && !groundColliders.Contains(collision))
        {
            groundColliders.Add(collision);
            //groundContats++;
            //Debug.Log("Ground contacts: " + groundContats);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.IsObjectInAnyLayer(groundLayers) && groundColliders.Contains(collision))
        {
            groundColliders.Remove(collision);
            //groundContats--;
            //Debug.Log("Ground contacts: " + groundContats);
        }
    }

    private void CheckGround()
    {
        groundColliders.Clear();
        List<Collider2D> colliders = new();
        if (Physics2D.OverlapCollider(this.GetComponent<Collider2D>(), new ContactFilter2D(), colliders) != 0)
        {
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
