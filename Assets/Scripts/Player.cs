using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using System.Linq.Expressions;

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

    private Sprite spiritSlashMask = null;
    [SerializeField]
    private Sprite spiritSlashSprite = null;
    [SerializeField]
    private int slashMaskSpriteResolution = 64;
    [SerializeField]
    private Vector2 slashMaskPivot = Vector2.zero;

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

        if (spiritSlashMask == null)
        {
            ReloadSpiritSlashMaskSprite();
        }
    }

    void Start()
    {
        StartHealth();
        if (!GameObject.FindWithTag("GameController").TryGetComponent(out gameController))
        {
            Debug.LogError("Could not find Game Manager! Paste prefab on scene please <3");
        }

        CreateSlashMaskSprite();
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

        if (canAttack)
        {
            StartCoroutine(Attack());
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
        else
        {
            SpiritAttack();
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
                GameObject slash = Instantiate(slashObject, slashPosition.position, Quaternion.identity, slashPosition);

                if (slash.TryGetComponent(out Animator animator))
                {
                    animator.Play("SlashAnim", -1, 0.0f);
                    Destroy(slash, attackDelay + 0.5f);
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

    [Button]
    private void ReloadSpiritSlashMaskSprite()
    {
        Texture2D texture2D = new(slashMaskSpriteResolution, slashMaskSpriteResolution);
        for (int y = 0; y < texture2D.height; y++)
        {
            for (int x = 0; x < texture2D.width; x++)
            {
                texture2D.SetPixel(x, y, Color.white);
            }
        }
        texture2D.Apply();
        spiritSlashMask = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero, 64);
    }

    private void CreateSlashMaskSprite()
    {
        if (spiritSlashMask != null)
        {
            List<Vector3> meshVerticies = new();
            List<int> meshTriangles = new();

            // CENTER POINT
            Vector3 origin = new(0f, 0f, 0f);
            meshVerticies.Add(origin);

            float halfSectorRadians = (sectorAngle / 2f) * Mathf.Deg2Rad;
            float startRadians = halfSectorRadians * 2f;
            float endRadians = 0f;

            Vector3 startPoint = (new Vector2(Mathf.Cos(startRadians), Mathf.Sin(startRadians))).normalized;
            Vector3 endPoint = (new Vector2(Mathf.Cos(endRadians), Mathf.Sin(endRadians))).normalized;

            // TOP POINT
            meshVerticies.Add(origin + startPoint * circleRadius);

            uint pointsNum = (uint)Mathf.Abs((int)((endRadians - startRadians) / (Mathf.Deg2Rad * 3f)));
            float radiansDiff = (endRadians - startRadians) / pointsNum;
            for (int i = 1; i < pointsNum - 1; i++)
            {
                // POINTS BETWEEN TOP AND BOTTOM
                Vector3 point = (new Vector3(Mathf.Cos(startRadians + i * radiansDiff), Mathf.Sin(startRadians + i * radiansDiff), 0f)).normalized;

                meshVerticies.Add(origin + point * circleRadius);
            }
            // BOTTOM POINT
            meshVerticies.Add(origin + endPoint * circleRadius);

            for (int i = 1; i < meshVerticies.Count - 1; i++)
            {
                meshTriangles.Add(0);
                meshTriangles.Add(i);
                meshTriangles.Add(i + 1);
            }

            MeshToSprite.ConvertMeshsToSprite(meshVerticies.ToArray(), meshTriangles.ToArray(), ref spiritSlashMask);

            if (sectorAngle > 90f)
            {
                float alpha = (sectorAngle - 90f) * Mathf.Deg2Rad;
                slashMaskPivot = new Vector2(Mathf.Sin(alpha) / (Mathf.Sin(alpha) + Mathf.Cos(alpha)), 0f);
            }
            else if (sectorAngle > 180f)
            {
                float alpha = (sectorAngle - 180f) * Mathf.Deg2Rad;
                slashMaskPivot = new Vector2(.5f, Mathf.Sin(alpha) / 2f);
            }
            else if (sectorAngle > 270f)
            {
                slashMaskPivot = new Vector2(.5f, .5f);
            }
            else
            {
                slashMaskPivot = Vector2.zero;
            }
        }
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

        // całe to rysowanie działa na razie dla kątów mniejszych równych 90 stopni
        GameObject slash = new("Slash");
        slash.transform.position = transform.position;
        float max = slashMaskPivot.x > slashMaskPivot.y ? slashMaskPivot.x : slashMaskPivot.y;
        float radius = circleRadius / (2f * (1f - max));
        slash.transform.localScale = new Vector3(1f, 1f) * radius + Vector3.forward;
        slash.transform.parent = this.transform;
        slash.AddComponent<SpriteMask>().sprite = spiritSlashMask;

        GameObject slashSprite = new("SlashSprite");
        slashSprite.transform.parent = slash.transform;
        slashSprite.transform.position = transform.position;
        slashSprite.transform.localScale = new Vector3(1f, 1f) * 2f * radius * 0.64f + Vector3.forward; // NIE WIEM SKĄD 0.64 nie mam siły teraz tego liczyć i sprawdzać
        SpriteRenderer renderer = slashSprite.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 1;
        renderer.sprite = spiritSlashSprite;
        renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        slash.transform.Rotate(Vector3.forward, Vector2Extensions.Angle360(Vector2.right, lookDir) - sectorAngle / 2f);
        slash.transform.Translate(-slashMaskPivot * radius); // Wstęp do wyższych kątów (na razie nie działa)
        Destroy(slash, .5f);
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
            Vector2 point1 = startPoint;
            for (int i = 0; i < 10 - 1; i++)
            {
                Vector2 point2 = (new Vector2(Mathf.Cos(startRadians + (i + 1) * radiansDiff), Mathf.Sin(startRadians + (i + 1) * radiansDiff))).normalized;

                Gizmos.DrawLine(origin + point1 * circleRadius, origin + point2 * circleRadius);
                point1 = point2;
            }
            Gizmos.DrawLine(origin + point1 * circleRadius, origin + endPoint * circleRadius);
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
