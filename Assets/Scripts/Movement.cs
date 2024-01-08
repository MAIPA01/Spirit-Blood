using System.Diagnostics;
using System.Security.Cryptography;
using UnityEditor.Timeline.Actions;
using UnityEngine;


public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private GroudCheck groundCheck;
    [SerializeField] private bool _isJumping = false;
    [SerializeField] private float rezistance = 0.7f;



    Rigidbody _rb;


    void Start()
    {
        //_rb = GetComponent<Rigidbody2D>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 vel = _rb.velocity;
        if (groundCheck.groundContats == 0)
        {
            vel.x = Input.GetAxis("Horizontal") * speed * rezistance;
        }
        else
        {
            vel.x = Input.GetAxis("Horizontal") * speed;
        }
        
        _rb.velocity = vel;

        Jump();   
    }


    public void Jump()
    {
        if (Input.GetButtonDown("Jump") && (groundCheck.groundContats != 0 && !_isJumping))
        {
            _isJumping = true;
            _rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);

        }

        if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y * 0.7f, _rb.velocity.z);
        }

        if (groundCheck.groundContats != 0)
        {
            _isJumping = false;
        }
    }



};
