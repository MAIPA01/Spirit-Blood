using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private Rigidbody2D r;
    public float speed = 100f;

    void Awake()
    {
        r = gameObject.AddComponent<Rigidbody2D>();
        r.bodyType = RigidbodyType2D.Dynamic;
        r.gravityScale = 0;
        r.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        Vector2 input = new(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        input.Normalize();

        r.velocity = new Vector2(input.x * speed, input.y * speed);
    }
}