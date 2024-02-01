using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BasicSpirit : ObjectHealth
{
    //private Rigidbody2D rigid = null;
    private Rigidbody rigid = null;

    public MeshRenderer body;
    public GameObject healthCanvas;
    private bool isDead = false;
    public Material dieMaterial;
    public float dieAnimTime = 2;
    private float currAnimTime = 0;

    [Header("Must Have Objects:")]
    [SerializeField][Tooltip("Zazwyczaj gracz ;)")] private ObjectHealth target = null;
    [SerializeField][Tooltip("Pocisk przeciwnika")] private Bullet bullet = null;

    [Header("Essential Parameters:")]
    [SerializeField][Tooltip("Jak p³ynnie ma skrêcaæ")] private float changeDirectionSpeed = .5f;
    [SerializeField][Tooltip("Prêdkoœæ poruszania siê")] private float speed = 5f;
    [SerializeField][Tooltip("Odleg³oœæ od gracza w jakiej dusza zacznie atakowaæ")] private float attackRange = 5f;
    [SerializeField][Tooltip("Czas pomiêdzy jednym a drugim atakiem")] private float attackDelay = 1f;
    [Tooltip("Czas bycia w szoku po uderzeniu")] public float stuntTime = 1f;
    [SerializeField][Tooltip("Damage który zadaje przeciwnik")] private float attackDamage = 10f;
    [SerializeField][Tooltip("Prêdkoœæ pocisku")] private float bulletSpeed = 5f;
    [Tooltip("Score gained by killing this enemy")] public float scoreGained = 10;

    private Vector2 targetDirection = Vector2.zero;

    private float attackTimer = 0f;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip[] clips;

    private void Start()
    {
        StartHealth();
        //rigid = rigid != null ? rigid : GetComponent<Rigidbody2D>();
        rigid = rigid != null ? rigid : GetComponent<Rigidbody>();

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<ObjectHealth>();
        }
    }

    void Update()
    {
        if (isDead)
        {
            currAnimTime += Time.deltaTime;
            body.material.SetFloat("_AnimationProgress", Mathf.Clamp01(currAnimTime / dieAnimTime));
            return;
        }

        if (target == null || rigid == null)
        {
            return;
        }

        if (this.GetHealth() <= 0f || target.GetHealth() <= 0f)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        Vector2 toTargetVector = target.transform.position - transform.position;
        targetDirection = toTargetVector.normalized;
        Vector2 desiredVelocity = speed * targetDirection;
        Vector2 realVelocity = new(Mathf.Lerp(rigid.velocity.x, desiredVelocity.x, Time.deltaTime * changeDirectionSpeed), Mathf.Lerp(rigid.velocity.y, desiredVelocity.y, Time.deltaTime * changeDirectionSpeed));
        rigid.velocity = realVelocity;

        if (attackTimer <= 0f && toTargetVector.magnitude <= attackRange)
        {
            attackTimer = attackDelay;
            if (bullet != null)
            {
                Bullet b = Instantiate(bullet.gameObject, (Vector2)this.gameObject.transform.position + targetDirection * .5f, Quaternion.identity, null).GetComponent<Bullet>();
                float angle = Vector2Extensions.Angle360(b.transform.forward, targetDirection);
                b.transform.Rotate(new Vector3(0f, 0f, angle));
                b.damage = attackDamage;
                //b.GetComponent<Rigidbody2D>().velocity = targetDirection * bulletSpeed;
                b.GetComponent<Rigidbody>().velocity = targetDirection * bulletSpeed;
            }
            //target.TakeDamage(attackDamage);
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
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

        if (target != null) target.GetComponent<Player>().score += scoreGained;
        base.OnDead();
        Destroy(this.gameObject, dieAnimTime);
    }

    public void SetStunt()
    {
        attackTimer = stuntTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            float radius = (target.transform.position - transform.position).magnitude;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
