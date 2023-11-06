using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpForce = 50f;

    Rigidbody2D _rb;
    Vector2 _movement;
    bool _jump;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.W))
        {
            _jump = true;
        }
    }

    void FixedUpdate()
    {
        moveCharacter(_movement);

        if (_jump)
        {
            jump();
            _jump = false;
        }
    }

    void moveCharacter(Vector2 direction)
    {
        _rb.MovePosition((Vector2)transform.position + (direction * speed * Time.deltaTime));
    }

    void jump()
    {
        _rb.velocity += new Vector2(0, jumpForce);
    }
};
