using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_animation_movement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private GroudCheck groundCheck;
    [SerializeField] private bool _isJumping = false;

    private Animator _animator;



    Rigidbody _rb;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 vel = _rb.velocity;
        vel.x = Input.GetAxis("Horizontal") * speed ;
        _rb.velocity = vel;
        _animator.SetFloat("speed", vel.x);
    }
}
