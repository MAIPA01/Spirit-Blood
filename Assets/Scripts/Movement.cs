using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;


public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private GroudCheck groundCheck;
    [SerializeField] private bool _isJumping = false;
    [SerializeField] private int _jumpCount = 1;
    [SerializeField] private float rezistance = 0.7f;
    private int actualJumpCount = 0;

    //Rigidbody2D _rb;

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

        if (Input.GetButtonDown("Jump") && (groundCheck.groundContats != 0 || !_isJumping) && actualJumpCount < _jumpCount)
        {
            _isJumping = true;
            actualJumpCount++;
            //_rb.velocity = new Vector2(0, jumpForce);
            _rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);

        }

        if (groundCheck.groundContats != 0 && actualJumpCount >= _jumpCount)
        {
            actualJumpCount = 0;
            _isJumping = false;
        }
    }

 

};
