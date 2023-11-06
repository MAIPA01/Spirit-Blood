using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BasicSpirit : MonoBehaviour
{
    private Rigidbody2D rigid = null;

    [Header("Must Have Objects:")]
    [SerializeField][Tooltip("Zazwyczaj gracz ;)")] private GameObject target = null;

    [Header("Essential Parameters:")]
    [SerializeField][Tooltip("Jak p³ynnie ma skrêcaæ 1 (bardzo) - 0 (w ogóle)")] private float smoothing = .5f;
    [SerializeField][Tooltip("Prêdkoœæ poruszania siê")] private float speed = 5f;
    [SerializeField][Tooltip("Odleg³oœæ od gracza w jakiej dusza zacznie atakowaæ")] private float attackRange = 5f;
    [SerializeField][Tooltip("Czas pomiêdzy jednym a drugim atakiem")] private float attackDelay = 1f;

    private Vector2 targetDirection = Vector2.zero;

    private bool canAttack = true;

    private void Start()
    {
        rigid = rigid != null ? rigid : GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (target != null && rigid != null)
        {
            Vector2 toTargetVector = target.transform.position - transform.position;
            if (toTargetVector.magnitude >= .1f)
            {
                targetDirection = toTargetVector.normalized;
                rigid.velocity += speed * Time.deltaTime * targetDirection;
                rigid.velocity = (1f - smoothing) * speed * targetDirection + rigid.velocity * smoothing;
            }
            else
            {
                rigid.velocity = Vector2.zero;                 
            }

            if (canAttack && toTargetVector.magnitude <= attackRange)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        Debug.Log("Attack");
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
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
