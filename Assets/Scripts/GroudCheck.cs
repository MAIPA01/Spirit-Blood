using UnityEngine;

public class GroudCheck : MonoBehaviour
{ 
    public int groundContats = 0;

    public int GroundContacts { get => groundContats; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        groundContats++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        groundContats--;
    }
    
}
