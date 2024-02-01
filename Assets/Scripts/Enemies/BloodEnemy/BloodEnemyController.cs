using UnityEngine;
using System;

public class BloodEnemyController : ObjectHealth
{
	[HideInInspector] public float prevHP;
    [SerializeField] public Animator anime;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip[] clips;
    public SkinnedMeshRenderer body;
    public GameObject healthCanvas;
    private bool isDead = false;
    public Material dieMaterial;
    public float dieAnimTime = 2;
    private float currAnimTime = 0;

    public bool isFallen = false;

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
        anime.SetLayerWeight(1, 0);

        ChangeState(chaseState);
    }

    void Update()
    {
        if (isDead)
        {
            currAnimTime += Time.deltaTime;
            body.material.SetFloat("_AnimationProgress", Mathf.Clamp01(currAnimTime / dieAnimTime));
            return;
        }

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
        isDead = true;
        currAnimTime = 0;
        body.material = dieMaterial;
        body.material.SetFloat("_AnimationProgress", 0);
        healthCanvas.SetActive(false);

        if (Vector3.Distance(transform.position, target.transform.position) < 25)
        {
            audioSource.Stop();
            audioSource.clip = clips[0];
            audioSource.volume = 0.5f;
            audioSource.pitch = 1;
            float add = (UnityEngine.Random.Range(0, 20) - 10) / 100.0f;
            audioSource.pitch += add;
            audioSource.Play();
        }
        if (!isFallen) target.GetComponent<Player>().score += scoreGained;
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
