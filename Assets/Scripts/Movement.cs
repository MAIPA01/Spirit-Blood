using UnityEngine;


public class Movement : MonoBehaviour
{
    [SerializeField] public float speed = 1f;
    [SerializeField] public float jumpForce = 50f;
    [SerializeField] private GroudCheck groundCheck;
    [SerializeField] private bool _isJumping = false;
    [SerializeField] private float rezistance = 0.7f;
    [SerializeField] private ParticleSystem _dust;

    Rigidbody _rb;

    private bool _isFalling = false;

    void Start()
    {
        //_rb = GetComponent<Rigidbody2D>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 vel = _rb.velocity;
        if (groundCheck.GroundContacts == 0)
        {
            vel.x = Input.GetAxis("Horizontal") * speed * rezistance;
        }
        else
        {
            float before = vel.x;
            vel.x = Input.GetAxis("Horizontal") * speed;
            if ((vel.x < 0 && before >= 0) || (vel.x > 0 && before <= 0))
            {
                _dust.Play();
            }
        }
        
        _rb.velocity = vel;

        Jump();

        // NIE USUWAC TEGO JEST TO WAZNE BY DZIALALA GRA
        if (groundCheck.GroundContacts == 0 && !_isJumping)
        {
            // Budzi fizyke gracza gdy stoi na platformie (naprawia blad z spirit Platform)
            _rb.WakeUp();
            GetComponent<Collider>().isTrigger = true;
            GetComponent<Collider>().isTrigger = false;
        }
    }


    public void Jump()
    {
        if (_rb.velocity.y <= 0.0f)
        {
            if (groundCheck.GroundContacts != 0 && _isJumping)
            {
                _dust.Play();
            }
            else if (groundCheck.GroundContacts == 0)
            {
                _isJumping = true;
            }
        }

        if (groundCheck.GroundContacts != 0 && _isJumping)
        {
            _isJumping = false;
        }

        if (Input.GetButtonDown("Jump") && (groundCheck.GroundContacts != 0 && !_isJumping))
        {
            _dust.Play();
            _isJumping = true;
            _rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }

        if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y * 0.7f, _rb.velocity.z);
        }
    }
};
