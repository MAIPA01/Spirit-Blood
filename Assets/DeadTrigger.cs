using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out ObjectHealth oh))
        {
            oh.TakeDamage(oh.GetMaxHealth());
        }
        
    }
}
