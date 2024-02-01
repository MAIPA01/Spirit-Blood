using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out ObjectHealth oh))
        {
            if (collision.TryGetComponent(out BloodEnemyController bec))
            {
                bec.isFallen = true;
            }
            oh.TakeDamage(oh.GetMaxHealth());
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ObjectHealth oh))
        {
            if (other.TryGetComponent(out BloodEnemyController bec))
            {
                bec.isFallen = true;
            }
            oh.TakeDamage(oh.GetMaxHealth());
        }
    }
}
