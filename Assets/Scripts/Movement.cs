using UnityEngine;


public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private GroudCheck groundCheck;
    [SerializeField] private bool _isJumping = false;
    [SerializeField] private int _jumpCount = 1;
    private int actualJumpCount = 0;
   /* private Vector3 m_rotateTo;
    public float rotationTime = 0.57f;*/

    [Tooltip("Co myslicie to takich filko�kach jak zosta�o to zaprezentowane w grze platformer shooter???")]
    //[SerializeField] private bool _enableRotation = false;

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

        if (Input.GetButtonDown("Jump") && (groundCheck.GroundContacts != 0 || !_isJumping) && actualJumpCount < _jumpCount)
        {
            _isJumping = true;
            actualJumpCount++;
            //_rb.velocity = new Vector2(0, jumpForce);
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        if (groundCheck.GroundContacts != 0 && actualJumpCount >= _jumpCount)
        {
            actualJumpCount = 0;
            _isJumping = false;
        }

        // NIE USUWA� TEGO JEST TO WA�NE BY DZIA�A�A GRA
        if (groundCheck.GroundContacts == 0 && !_isJumping && actualJumpCount == 0)
        {
            // Budzi fizyk� gracza gdy stoi na platformie (naprawia b��d z spirit Platform)
            _rb.WakeUp();
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
};
