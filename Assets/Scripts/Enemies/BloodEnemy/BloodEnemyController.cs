using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class BloodEnemyController : ObjectHealth
{
	[HideInInspector] public float prevHP; 

    public float enemyMoveSpeed = 0.5f;
    public float enemyRange = 2f;
    public float enemyAttackDmg = 0.00001f;

    [Tooltip("The strenght of this enemy PUSHBACK when struck by player")]
    public float pushBackFactor = 3.0f;

    [Tooltip("Time in which enemy can't act after taking dmg in SECONDS")]
    public float stunTime = 10f;

    [Tooltip("Delay between next attack in SECONDS")]
    public float enemyAttackDecay = 2f;

    [Tooltip("Score gained by killing this enemy")]
    public float scoreGained = 10;

    public GameObject target;
    private IState currentState;

    public BE_Attack attackState;
    public BE_Chase chaseState;
    public BE_Hurt hurtState;

    /*private void OnValidate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target player not assigned for blood enemy :( Assign me pls");
        }
    }*/

    private void Awake()
    {
        attackState = new();
        chaseState = new();
        hurtState = new();
    }

    private void Start()
    {
        StartHealth();

        if (target == null)
        {
			target = GameObject.FindGameObjectWithTag("Player");
            //Debug.LogError("Target player not assigned for blood enemy :( Assign me pls");
        }
		prevHP = GetHealth();
        ChangeState(chaseState);
    }

    void Update()
    {
        currentState?.UpdateState(this);
    }

    public void ChangeState(IState newState)
    {
        currentState?.OnExit(this);

        currentState = newState;
        currentState.OnEnter(this);
    }
	
	public override void OnDead()
    {
		target.GetComponent<Player>().score += scoreGained;
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(enemyRange * 2.0f, 1.0f));
    }

    public bool checkRange()
    {
       if (Math.Abs(target.transform.position.x - transform.position.x) <= enemyRange && Math.Abs(target.transform.position.y - transform.position.y) <= enemyRange)
            return true;
        return false;
    } 

}

public interface IState
{
    public void OnEnter(BloodEnemyController sc);
    public void UpdateState(BloodEnemyController sc);
    public void OnExit(BloodEnemyController sc);
    public void OnHurt(BloodEnemyController sc);
}
