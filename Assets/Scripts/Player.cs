using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor.Animations;

enum PlayerForm
{
    Blood = 0,
    Spirit = 1
}

public class Player : ObjectHealth
{
    [Header("Player Global Settings:")]
    [SerializeField] 
    private PlayerForm form;

    [SerializeField]
    private SpriteRenderer body;

    [SerializeField]
    private bool m_FacingRight = true;

    [SerializeField]
    private GameObject slashObject;

    [SerializeField]
    private Transform slashPosition;
	
	[SerializeField] private GameManager gameController;

    // counting score for score display
    [HideInInspector] public float score = 0;

    [Header("Forms:")]
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private Color spiritColor = Color.black;

    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private Color bloodColor = Color.white;

    [Header("Attack:")]
    [SerializeField]
    private float attackDelay = 1f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private float spiritDamage = 2f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private float circleRadius;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private float sectorAngle;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private LayerMask spiritLayers;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private float bloodDamage = 1f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private LayerMask bloodLayers;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private Transform bloodWeaponTransform;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private float bloodAttackRange = 0.5f;

    private bool canAttack = true;

    private void OnValidate()
    {
        if (slashObject == null)
        {
            Debug.LogWarning("Slash Object wasn't provided!!!");
        }

        if (!IsSpirit() && bloodWeaponTransform == null)
        {
            Debug.LogError("Blood Weapon Transform is needed for attack to work!!!");
        }

        if (body != null)
        {
            body.color = IsSpirit() ? spiritColor : bloodColor;
        }
    }

    void Start()
    {
        StartHealth();
        if (!GameObject.FindWithTag("GameController").TryGetComponent(out gameController))
        {
            Debug.LogError("Could not find Game Manager! Paste prefab on scene please <3");
        }
    }

    void Update()
    {
		if (gameController.score < score)
        {
            gameController.UpdateScore(score);
        }
		
        if (Input.mousePosition.x >= Screen.width / 2 && !m_FacingRight)
        {
            Flip();
        }
        else if (Input.mousePosition.x < Screen.width / 2 && m_FacingRight)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeForm();
        }

        if (IsSpirit())
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpiritAttack();
            }
        }
        else 
        {
          if (Input.GetMouseButtonDown(0) && canAttack)
          {
              StartCoroutine(Attack());
          }
        }
    }

    public bool IsSpirit() { return form == PlayerForm.Spirit; }

    public void ChangeForm() 
    { 
        bool isSpirit = IsSpirit();
        form = isSpirit ? PlayerForm.Blood : PlayerForm.Spirit;
        if (body != null )
        {
            body.color = isSpirit ? bloodColor : spiritColor;
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        if (!IsSpirit())
        {
            BloodAttack();
        }
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    private void BloodAttack()
    {
        if (bloodWeaponTransform != null)
        {
            if (slashObject != null)
            {
                GameObject slash = Instantiate(slashObject, slashPosition.position, Quaternion.identity);
                Vector3 theScale = slash.transform.localScale;
                theScale.x *= m_FacingRight ? 1 : -1;
                slash.transform.localScale = theScale;

                if (slash.TryGetComponent(out Animator animator))
                {
                    animator.Play("SlashAnim", -1, 0.0f);
                    Destroy(slash, 2.5f);
                }
                else
                {
                    Destroy(slash, 0.5f);
                }
            }

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(bloodWeaponTransform.position, bloodAttackRange, bloodLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.TryGetComponent(out ObjectHealth obj))
                {
                    obj.AddHealth(-bloodDamage);
                }
            }
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    [Button]
    private void ChangeFormTest()
    {
        ChangeForm();
    }

    private void SpiritAttack()
    {
        // Dodaæ Delay
        Vector2 origin = body.transform.position;
        Vector2 lookDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - origin).normalized;
        RaycastHit2D[] hits = Physic2DExtension.CircleSectorCastAll(origin, circleRadius, sectorAngle, lookDir, float.PositiveInfinity, spiritLayers.value);
        for (uint i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject == gameObject)
            {
                continue;
            }
            if (((1 << hits[i].collider.gameObject.layer) & spiritLayers.value) == spiritLayers.value)
            {
                GameObject spiritEnemy = hits[i].collider.gameObject;
                spiritEnemy.GetComponent<BasicSpirit>().TakeDamage(spiritDamage);
                spiritEnemy.GetComponent<BasicSpirit>().SetStunt();

                Vector2 throwBackDir = spiritEnemy.transform.position - body.transform.position;
                throwBackDir.Normalize();
                spiritEnemy.GetComponent<Rigidbody2D>().velocity += throwBackDir * spiritDamage;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (IsSpirit())
        {
            Vector2 origin = transform.position;
            Vector2 lookDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - origin).normalized;

            float lookRadians = MathfExtensions.DegreesToRadians(Vector2Extensions.Angle360(Vector2.right, lookDir));

            float halfSectorRadians = MathfExtensions.DegreesToRadians(sectorAngle / 2f);
            float startRadians = lookRadians + halfSectorRadians;
            float endRadians = lookRadians - halfSectorRadians;

            Vector2 startPoint = (new Vector2(Mathf.Cos(startRadians), Mathf.Sin(startRadians))).normalized;
            Vector2 endPoint = (new Vector2(Mathf.Cos(endRadians), Mathf.Sin(endRadians))).normalized;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, origin + lookDir * circleRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + startPoint * circleRadius);
            Gizmos.DrawLine(origin, origin + endPoint * circleRadius);

            float radiansDiff = (endRadians - startRadians) / 10;
            for (int i = 1; i < 11; i++)
            {
                Vector2 point1 = (new Vector2(Mathf.Cos(startRadians + (i - 1) * radiansDiff), Mathf.Sin(startRadians + (i - 1) * radiansDiff))).normalized;
                Vector2 point2 = (new Vector2(Mathf.Cos(startRadians + i * radiansDiff), Mathf.Sin(startRadians + i * radiansDiff))).normalized;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(origin + point1 * circleRadius, origin + point2 * circleRadius);
            }
        }
        else
        {
            if (bloodWeaponTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(bloodWeaponTransform.position, bloodAttackRange);
            }
        }
	}
	
    public override void OnDead()
    {
        Time.timeScale = 0;
        gameController.DeadScreen();
    }
}
