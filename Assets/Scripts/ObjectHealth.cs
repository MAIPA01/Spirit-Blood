using UnityEngine;
using NaughtyAttributes;

public class ObjectHealth : MonoBehaviour, IDamageTaker
{   
    [Header("Health:")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 0f;

    public void StartHealth()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(float value)
    {
        health -= value;
        if (health <= 0f)
        {
            health = 0f;
            OnDead();
        }
    }

    public virtual void OnDead()
    {
        Debug.Log("Dead");
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
    }

    public void AddHealth(float value)
    {
        health += value;
    }

    /*
    static Vector2 ComputeTotalImpulse(Collision2D collision)
    {
        Vector2 impulse = Vector2.zero;
        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            var contact = collision.GetContact(i);
            impulse += contact.normal * contact.normalImpulse;
            impulse.x += contact.tangentImpulse * contact.normal.y;
            impulse.y -= contact.tangentImpulse * contact.normal.x;
        }
        return impulse;
    }
    public static Vector2 ComputeIncidentVelocity(Collision2D collision)
    {
        Vector2 impulse = ComputeTotalImpulse(collision);
        var myBody = collision.otherRigidbody;
        return myBody.velocity - impulse / myBody.mass;
    }
    */

    [Button]
    private void TakeTestDamage()
    {
        TakeDamage(0.1f * GetMaxHealth());
    }
}
