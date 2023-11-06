using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class BloodEnemyController : MonoBehaviour
{
    public float enemyMoveSpeed = 0.5f;
    public float enemyRange = 2f;
    public float enemyAttackDmg = 0.00001f;

    // attacks per second
    public float enemyAttackSpeed = 0.5f; 


    public Transform transform;
    public GameObject target;
    IState currentState;

    public BE_Attack attackState = new BE_Attack();
    public BE_Chase chaseState = new BE_Chase();
    public BE_Hurt hurtState = new BE_Hurt();

    private void Start()
    {
        if(target == null)
        {
            Debug.LogError("Target player not assigned for blood enemy :( Assign me pls");
        }

        transform = GetComponent<Transform>();
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
}

public interface IState
{
    public void OnEnter(BloodEnemyController sc);
    public void UpdateState(BloodEnemyController sc);
    public void OnExit(BloodEnemyController sc);
    public void OnHurt(BloodEnemyController sc);
}
