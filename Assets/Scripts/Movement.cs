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
        
        // TODO: ZROB NA 3D
        /*
        // NIE USUWAÆ TEGO JEST TO WA¯NE BY DZIA£A£A GRA
        if (groundCheck.GroundContacts == 0 && !_isJumping && actualJumpCount == 0)
        {
            // Budzi fizykê gracza gdy stoi na platformie (naprawia b³¹d z spirit Platform)
            _rb.WakeUp();
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<Collider2D>().isTrigger = false;
        }
        */
    }
};
