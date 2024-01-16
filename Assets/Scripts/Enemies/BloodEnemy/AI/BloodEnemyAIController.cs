using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BloodEnemyAIController : ObjectHealth
{
    /*
    private AIState currentState;

    public BEAI_Attack attackState = new();
    public BEAI_Chase chaseState = new();
    public BEAI_Hurt hurtState = new();
    */

    private float prevHP;

    [Header("Pathfinding")]
    [SerializeField] private GameObject target;
    [SerializeField] private float pathUpdateSeconds = 0.5f;
    [SerializeField] private float nextWaypointDistance = 3f;

    private Path path;
    private Seeker seeker;
    private int currentWaypoint;

    [Header("Movement")]
    [SerializeField] private float enemyMoveSpeed = 0.5f;
    [SerializeField] private float enemyRange = 2f;

    [SerializeField] private float jumpNodeHeightRequirement = 0.8f;
    [SerializeField] private float jumpModifier = 0.3f;
    [SerializeField] private float jumpCheckOffset = 0.1f;

    [SerializeField] private bool followEnabled = true;
    [SerializeField] private bool jumpEnabled = true;
    [SerializeField] private bool directionLookEnabled = true;

    private bool isGrounded = false;

    private Rigidbody2D rb;

    [Header("When Attacked")]
    [Tooltip("The strenght of this enemy PUSHBACK when struck by player")]
    [SerializeField] private float pushBackFactor = 3.0f;

    [Tooltip("Time in which enemy can't act after taking dmg in SECONDS")]
    [SerializeField] private float stunTime = 10f;

    [Header("Attack")]
    [Tooltip("Delay between next attack in SECONDS")]
    [SerializeField] private float enemyAttackDecay = 2f;

    [SerializeField] private float enemyAttackDmg = 1f;

    [Header("Death")]
    [Tooltip("Score gained by killing this enemy")]
    [SerializeField] private float scoreGained = 10;

    private void Awake()
    {

    }

    private void Start()
    {
        StartHealth();

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        if (TryGetComponent(out seeker))
        {
            Debug.Log("BloodEnemyAIController: Couldn't find Component Seeker!");
        }

        if (TryGetComponent(out rb))
        {
            Debug.Log("BloodEnemyAIController: Couldn't find Component Rigidbody2D!");
        }

        prevHP = GetHealth();

        InvokeRepeating(nameof(UpdatePath), 0f, pathUpdateSeconds);
        
        //ChangeState(chaseState);
    }

    private void FixedUpdate()
    {
        if (followEnabled)
        {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.transform.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // See if colliding with anything
        isGrounded = Physics2D.Raycast(transform.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);

        // Direction Calculation
        Vector2 direction = ((Vector2)(path.vectorPath[currentWaypoint] - transform.position)).normalized;
        Vector2 force = enemyMoveSpeed * direction;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                rb.velocity = new Vector2(rb.velocity.x, enemyMoveSpeed * jumpModifier);
            }
        }

        // Movement
        rb.velocity = new Vector2(force.x, rb.velocity.y);

        // Next Waypoint
        float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    public bool TargetInRange()
    {
        return Vector2.Distance(target.transform.position, transform.position) <= enemyRange;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    /*
    void Update()
    {
        //currentState?.UpdateState(this);
    }
    */

    /*
    public void ChangeState(AIState newState)
    {
        currentState?.OnExit(this);

        currentState = newState;
        currentState.OnEnter(this);
    }
    */

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
}