using UnityEngine;

public class GroudCheck : MonoBehaviour
{ 
    public int groundContats = 0;

    public int GroundContacts { get => groundContats; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            groundContats++;
            //Debug.Log("Ground contacts: " + groundContats);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            groundContats--;
            //Debug.Log("Ground contacts: " + groundContats);
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            groundContats++;
            //Debug.Log("Ground contacts: " + groundContats);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            groundContats--;
            //Debug.Log("Ground contacts: " + groundContats);
        }
    }
}
