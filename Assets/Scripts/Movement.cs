using UnityEngine;


public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private GroudCheck groundCheck;
    [SerializeField] private bool _isJumping = false;

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

        if (groundCheck.groundContats > 0)
        {
            _isJumping = false;
        }

    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("Jump") && (groundCheck.groundContats > 0 || !_isJumping))
        {
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

            if (groundCheck.groundContats == 0)
            {
                _isJumping = true;
            }
        }
    }
    
};
