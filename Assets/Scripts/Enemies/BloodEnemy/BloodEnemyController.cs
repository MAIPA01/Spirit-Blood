using UnityEngine;
using System;

public class BloodEnemyController : ObjectHealth
{
	[HideInInspector] public float prevHP;

    public SkinnedMeshRenderer body;
    public GameObject healthCanvas;
    private bool isDead = false;
    public Material dieMaterial;
    public float dieAnimTime = 2;
    private float currAnimTime = 0;

    public float enemyMoveSpeed = 0.5f;
    public float enemyRange = 2f;
    public float enemyHitboxHeight = 1f;
    public float enemyAttackDmg = 0.00001f;

    [Tooltip("The strenght of this enemy PUSHBACK when struck by player")]
    public float pushBackFactor = 1.5f;

    [Tooltip("Time in which enemy can't act after taking dmg in SECONDS")]
    public float stunTime = 1f;

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
        if (!isDead)
        {
            currentState?.UpdateState(this);
        }
        else
        {
            currAnimTime += Time.deltaTime;
            body.material.SetFloat("_AnimationProgress", Mathf.Clamp01(currAnimTime / dieAnimTime));
        }
    }

    public void ChangeState(IState newState)
    {
        currentState?.OnExit(this);

        currentState = newState;
        currentState.OnEnter(this);
    }
	
	public override void OnDead()
    {
        isDead = true;
        currAnimTime = 0;
        body.material = dieMaterial;
        body.material.SetFloat("_AnimationProgress", 0);
        healthCanvas.SetActive(false);
		//target.GetComponent<Player>().score += scoreGained;
        Destroy(this.gameObject, dieAnimTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(enemyRange * 2f, enemyHitboxHeight * 2f));
    }

    public bool checkRange()
    {
        if (target == null) return false;

        if (Math.Abs(target.transform.position.x - transform.position.x) <= enemyRange && Math.Abs(target.transform.position.y - transform.position.y) <= enemyHitboxHeight)
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
