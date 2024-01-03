using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float liveTime = 10f;
    public float damage = 0f;

    private void Start()
    {
        Destroy(this.gameObject, liveTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<ObjectHealth>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
