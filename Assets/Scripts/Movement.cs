using System.Diagnostics;
using UnityEngine;


public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private GroudCheck groundCheck;
    [SerializeField] private bool _isJumping = false;
    [SerializeField] private int _jumpCount = 1;
    private int actualJumpCount = 0;

    [Tooltip("Co myslicie to takich filko³kach jak zosta³o to zaprezentowane w grze platformer shooter???")]
    [SerializeField] private bool _enableRotation = false;

    Rigidbody2D _rb;
    

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 vel = _rb.velocity;
        vel.x = Input.GetAxis("Horizontal") * speed;
        _rb.velocity = vel;

        if (Input.GetButtonDown("Jump") && (groundCheck.groundContats != 0 || !_isJumping) && actualJumpCount < _jumpCount)
        {
            _isJumping = true;
            actualJumpCount++;
            UnityEngine.Debug.Log("Jump count: " + actualJumpCount);
            UnityEngine.Debug.Log("Is jumping???: " + _isJumping);
            //_rb.velocity = new Vector2(0, jumpForce);
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        if (groundCheck.groundContats != 0 && actualJumpCount >= _jumpCount)
        {
            actualJumpCount = 0;
            _isJumping = false;
            UnityEngine.Debug.Log("Ij jumping??: " + _isJumping);
        }

    }
    
};
