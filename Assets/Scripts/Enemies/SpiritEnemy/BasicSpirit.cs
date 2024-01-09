using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BasicSpirit : ObjectHealth
{
    private Rigidbody2D rigid = null;

    [Header("Must Have Objects:")]
    [SerializeField][Tooltip("Zazwyczaj gracz ;)")] private ObjectHealth target = null;
    [SerializeField][Tooltip("Pocisk przeciwnika")] private Bullet bullet = null;

    [Header("Essential Parameters:")]
    [SerializeField][Tooltip("Jak p³ynnie ma skrêcaæ")] private float changeDirectionSpeed = .5f;
    [SerializeField][Tooltip("Prêdkoœæ poruszania siê")] private float speed = 5f;
    [SerializeField][Tooltip("Odleg³oœæ od gracza w jakiej dusza zacznie atakowaæ")] private float attackRange = 5f;
    [SerializeField][Tooltip("Czas pomiêdzy jednym a drugim atakiem")] private float attackDelay = 1f;
    [SerializeField][Tooltip("Czas bycia w szoku po uderzeniu")] private float stuntTime = 1f;
    [SerializeField][Tooltip("Damage który zadaje przeciwnik")] private float attackDamage = 10f;
    [SerializeField][Tooltip("Prêdkoœæ pocisku")] private float bulletSpeed = 5f;
    [Tooltip("Score gained by killing this enemy")] public float scoreGained = 10;

    private Vector2 targetDirection = Vector2.zero;

    private float attackTimer = 0f;

    private void Start()
    {
        StartHealth();
        rigid = rigid != null ? rigid : GetComponent<Rigidbody2D>();

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<ObjectHealth>();
        }
    }

    void Update()
    {
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
                b.GetComponent<Rigidbody2D>().velocity = targetDirection * bulletSpeed;
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
        target.GetComponent<Player>().score += scoreGained;
        base.OnDead();
        Destroy(this.gameObject);
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
