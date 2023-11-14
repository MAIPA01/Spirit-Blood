using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEnemyController : ObjectHealth
{
	[HideInInspector] public float prevHP; 
	
    public float enemyMoveSpeed = 0.5f;
    public float enemyRange = 2f;
    public float enemyAttackDmg = 0.00001f;

    [Tooltip("Time in which enemy can't act after taking dmg in SECONDS")]
    public float stunTime = 10f;

    [Tooltip("Delay between next attack in SECONDS")]
    public float enemyAttackDecay = 2f;

    [Tooltip("Score gained by killing this enemy")]
    public float scoreGained = 10;

    public GameObject target;
    IState currentState;

    public BE_Attack attackState = new BE_Attack();
    public BE_Chase chaseState = new BE_Chase();
    public BE_Hurt hurtState = new BE_Hurt();

    private void Start()
    {
        StartHealth();

        if(target == null)
        {
			target = GameObject.FindWithTag("Player");
            //Debug.LogError("Target player not assigned for blood enemy :( Assign me pls");
        }
		prevHP = GetHealth();
        ChangeState(chaseState);
    }

    void Update()
    {
        if(currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public void ChangeState(IState newState)
    {
        if(currentState != null)
        {
            currentState.OnExit(this);
        }

        currentState = newState;
        currentState.OnEnter(this);
    }
	
	public void Die()
    {
        Destroy(gameObject);
    }
}

public interface IState
{
    public void OnEnter(BloodEnemyController sc);
    public void UpdateState(BloodEnemyController sc);
    public void OnExit(BloodEnemyController sc);
    public void OnHurt(BloodEnemyController sc);
}
