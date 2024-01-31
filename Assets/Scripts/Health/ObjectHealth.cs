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
        //Debug.Log("Dead");
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
        if(health + value > maxHealth)
        {
            health = maxHealth;
        }
        else
            health += value;
    }

    [Button]
    private void TakeTestDamage()
    {
        TakeDamage(0.1f * GetMaxHealth());
    }
}
