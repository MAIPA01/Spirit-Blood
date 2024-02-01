using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;
using UnityEngine.VFX;
using UnityEditor.Rendering.LookDev;

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
    private readonly UnityEvent formChangedEvent = new();

    [SerializeField]
    //private SpriteRenderer body; // 2D
    private SkinnedMeshRenderer body; // 3D

    [SerializeField]
    private bool m_FacingRight = true;

    [SerializeField]
    private GameObject slashObject;

    [SerializeField]
    private Transform slashPosition; 
    
    [SerializeField]
    private Transform bloodSuperAttPosition;

    [SerializeField]
    private Transform spiritSlashPosition;
	
	[SerializeField] private GameManager gameController;

    // counting score for score display
    [HideInInspector] public float score = 0;

    [Header("Forms:")]
    [SerializeField]
    public float formChangeCooldown = 2f;
    private float formCooldownTime = 0f;

    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private Color spiritColor = Color.black;

    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private Color bloodColor = Color.white;

    [Header("Attack:")]
    [SerializeField]
    public float attackDelay = 1f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    public float spiritDamage = 2f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    public float circleRadius;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private float sectorAngle;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private LayerMask spiritLayers;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    public float bloodDamage = 1f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private LayerMask bloodLayers;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private Transform bloodWeaponTransform;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    public float bloodAttackRange = 0.5f;

    private bool canAttack = true;

    private Sprite spiritSlashMask = null;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private int slashMaskSpriteResolution = 64;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private Vector2 slashMaskPivot = Vector2.zero;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private GameObject spiritSlashPrefab = null;

    [Header("Ground:")]
    [SerializeField]
    private GroudCheck groundCheck = null;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private LayerMask bloodGroundLayers;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private LayerMask spiritGroundLayers;

    [SerializeField]
    private GameObject cooldownEndParticles;
    private float superAttackDurTimer = 0f;
    [SerializeField]
    private float superAttackWindUp = 1.10f;
    [SerializeField]
    private float superAttackDuration = 0.75f;
    [SerializeField]
    public float superAttackCooldown = 5.0f;
    private float superCooldownTimer;
    [SerializeField]
    private float superAttackRadius = 20.0f;
    [SerializeField]
    private GameObject bloodSuperAttackObject = null;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clips;
    public CamShake cameraShake;

    private bool right = false;
    private float superBloodAttackDmg; // kills all in 1 go, so should be BIG
    private int superAttackPhase = 0; // 0 - none, 1 - windup, 1 - superAttack

    [HideInInspector]
    public float lifeSteal = 0.0f; // Works only if you pick warrior class
    public float skillBonusFactor = 0.0f; // Works only if you pick unity class, it makes stun and knockback effects longer

    private void OnValidate()
    {
        if (slashObject == null)
        {
            Debug.LogWarning("Slash Object wasn't provided!!!");
        }

        if (bloodSuperAttackObject == null)
        {
            Debug.LogWarning("bloodSuperAttackObject wasn't provided!!!");
        }

        if (!IsSpirit() && bloodWeaponTransform == null)
        {
            Debug.LogError("Blood Weapon Transform is needed for attack to work!!!");
        }

        if (body != null)
        {
            //body.color = IsSpirit() ? spiritColor : bloodColor; // 2D
            body.sharedMaterial.color = IsSpirit() ? spiritColor : bloodColor;
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
        UpdateGround();

        superBloodAttackDmg = 9999.0f;
        superCooldownTimer = superAttackCooldown;
    }

    void Update()
    {
        formCooldownTime -= Time.deltaTime;
		if (gameController == null)
        {
            Debug.Log("Pls Set gameController");
        }
        else
        {
            if (gameController.score < score)
            {
                gameController.UpdateScore(score);
            }
        }
        
        if (((Input.GetMouseButtonDown((int)MouseButton.Right) && superAttackPhase != 2) || superAttackPhase == 1) && GameTimer.TimeMultiplier == GameTimer.PLAYING)
        {
            if(!IsSpirit() && superCooldownTimer <= .0f)
            {
                audioSource.Stop();
                audioSource.clip = clips[0];
                audioSource.volume = 0.33f;
                audioSource.pitch = 1;
                float add = (UnityEngine.Random.Range(0, 20) - 10) / 100.0f;
                audioSource.pitch += add + add + add;
                audioSource.Play();
                StartCoroutine(cameraShake.Shake(superAttackWindUp+superAttackDuration, 0.3f));
                //BloodSuperAttack(superAttackWindUp / 2.0f, true);
                if (superAttackPhase == 0)
                {
                    superAttackDurTimer = Time.time;
                    superAttackPhase = 1;
                }
            }

            if(IsSpirit() && superCooldownTimer <= .0f)
            {
                audioSource.Stop();
                audioSource.clip = clips[1];
                audioSource.pitch = 1;
                audioSource.volume = 0.6f;
                float add = (UnityEngine.Random.Range(0, 20) - 10) / 100.0f;
                audioSource.pitch += add;
                audioSource.Play();
                StartCoroutine(cameraShake.Shake(superAttackWindUp + superAttackDuration/2.0f, 0.3f));
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, superAttackRadius, Vector2.right, 0.01f, spiritLayers.value);

                for (int i = 0; i < hits.Length; i++)
                {
                    GameObject targetHit = hits[i].collider.gameObject;
                    targetHit.GetComponent<BasicSpirit>().TakeDamage(superBloodAttackDmg);
                    GameObject b00mEffect = Instantiate(cooldownEndParticles, targetHit.transform.position, Quaternion.identity);
                    b00mEffect.GetComponent<ParticleSystem>().startColor = Color.cyan;
                    Destroy(b00mEffect, 1.0f);
                    this.AddHealth(lifeSteal);
                }
                superCooldownTimer = superAttackCooldown;
            }
        }
        
        if (superAttackPhase == 0)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePos.x >= transform.position.x && !m_FacingRight && GameTimer.TimeMultiplier == GameTimer.PLAYING)
            {
                Flip();
                right = true;
            }
            else if (mousePos.x < transform.position.x && m_FacingRight && GameTimer.TimeMultiplier == GameTimer.PLAYING)
            {
                Flip();
                right = false;
            }
            
            /*
            if (Input.GetKeyDown(KeyCode.E) && formCooldownTime <= 0f)
            {
                formCooldownTime = formChangeCooldown;
                ChangeForm();
            }
            */
            
            if (Input.GetMouseButtonDown((int)MouseButton.Left) && formCooldownTime <= 0f && GameTimer.TimeMultiplier == GameTimer.PLAYING)
            {
                formCooldownTime = formChangeCooldown;
                ChangeForm();
            }

            if (canAttack && GameTimer.TimeMultiplier == GameTimer.PLAYING)
            {
                StartCoroutine(Attack());
            }
        }
        else if (Time.time >= superAttackDurTimer + superAttackDuration + superAttackWindUp)
        {
            superAttackPhase = 0;
            superCooldownTimer = superAttackCooldown;
        }
        else if (Time.time > superAttackDurTimer + superAttackWindUp && superAttackPhase == 1)
        {
            superAttackPhase = 2;
            BloodSuperAttack(superAttackDuration, false);
        }

        if(superCooldownTimer > 0 && GameTimer.TimeMultiplier == GameTimer.PLAYING)
        {
            superCooldownTimer -= Time.deltaTime;
            if(superCooldownTimer <= 0)
            {
                GameObject cooldownEndEffect = Instantiate(cooldownEndParticles, gameObject.transform.position, Quaternion.identity, gameObject.transform);
                Destroy(cooldownEndEffect, 1.0f);
            }
        }

    }

    public bool IsSpirit() { return form == PlayerForm.Spirit; }

    public void ChangeForm() 
    { 
        // Mowi o starej formie
        bool isSpirit = IsSpirit();
        form = isSpirit ? PlayerForm.Blood : PlayerForm.Spirit;
        if (body != null )
        {
            //body.color = isSpirit ? bloodColor : spiritColor; // 2D
            body.sharedMaterial.color = isSpirit ? bloodColor : spiritColor;
        }
        formChangedEvent.Invoke();
        
        UpdateGround();
    }

    public void AddChangeFormCallback(UnityAction action)
    {
        formChangedEvent.AddListener(action);
    }

    void UpdateGround()
    {
        /*LayerMask playerCollision = Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Player"));
        playerCollision &= ~(IsSpirit() ? bloodGroundLayers : spiritGroundLayers);
        playerCollision |= IsSpirit() ? spiritGroundLayers : bloodGroundLayers;
        Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer("Player"), playerCollision);*/

        LayerMask collDiff = (bloodGroundLayers | spiritGroundLayers) & (~bloodGroundLayers | ~spiritGroundLayers);
        LayerMask prevLayer = IsSpirit() ? bloodGroundLayers : spiritGroundLayers;

        for (int i = 0; i < 32; i++)
        {
            if (((collDiff & (1 << i)) >> i) == 1)
            {
                // było włączone więc ignorować
                if (((prevLayer & (1 << i)) >> i) == 1)
                {
                    Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), i, true);
                }
                // było wyłączone więc nie ignorować
                else
                {
                    Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), i, false);
                }
            }
        }

        if (groundCheck == null)
        {
            Debug.LogError("Ground Check not provided");
        }
        else
        {
            groundCheck.GroundLayers = IsSpirit() ? spiritGroundLayers : bloodGroundLayers;
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

    private void BloodSuperAttack(float killTime, bool check)
    {
        // PUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUNCH!
        Vector2 lookDir = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - bloodSuperAttPosition.position);
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        Vector2 origin;
        float circleRad = 1.10f; 

        GameObject punch;
        punch = Instantiate(bloodSuperAttackObject, bloodSuperAttPosition.position, Quaternion.identity, bloodSuperAttPosition);
        punch.transform.Translate(0.5f * this.transform.localScale.x, 0, 0);
        punch.transform.Rotate(Vector3.forward, Vector2Extensions.Angle360(-Vector2.right * this.transform.localScale.x, lookDir));
        origin = punch.transform.position;

        if (check)
        {
            punch.GetComponent<SpriteRenderer>().sprite = null;
            punch.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.05f);
        }
        else
        {
            RaycastHit[] hits = Physics.SphereCastAll(origin, circleRad, lookDir, float.PositiveInfinity, bloodLayers.value);
            for (int i = 0; i < hits.Length; i++)
            {
                GameObject targetHit = hits[i].collider.gameObject;
                targetHit.GetComponent<BloodEnemyController>().TakeDamage(superBloodAttackDmg);
                this.AddHealth(lifeSteal);
            }
        }

        Destroy(punch, killTime);
    }
    private void BloodAttack()
    {
        if (bloodWeaponTransform != null)
        {
            if (slashObject != null)
            {
                GameObject slash = Instantiate(slashObject, slashPosition.position, Quaternion.identity, slashPosition);
                ///slash.transform.localScale = new Vector3(this.transform.localScale.z * slash.transform.localScale.x, slash.transform.localScale.y, slash.transform.localScale.z);

                /*slash.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                slash.transform.Rotate(new Vector3(0.0f, 0.0f, -60f));*/

                if (slash.TryGetComponent(out VisualEffect ve))
                {
                    ve.Play();
                    Destroy(slash, attackDelay + 0.5f);
                }
                else
                {
                    Destroy(slash, 0.5f);
                }

                /*if (slash.TryGetComponent(out Animator animator))
                {
                    animator.Play("SlashAnim", -1, 0.0f);
                    Destroy(slash, attackDelay + 0.5f);
                }
                else
                {
                    Destroy(slash, 0.5f);
                }*/

            }

            //Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(bloodWeaponTransform.position, bloodAttackRange, bloodLayers);

            //RaycastHit2D[] hitEnemies;
            Vector2 lookDir = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            lookDir.y = 0f;
            lookDir.Normalize();
            //RaycastHit[] hitEnemies = PhysicExtension.SphereSectorCastAll(bloodWeaponTransform.position, bloodAttackRange, 180f, lookDir, float.PositiveInfinity, bloodLayers);
            RaycastHit[] hitEnemies = PhysicExtension.ConeCastAll(bloodWeaponTransform.position, bloodAttackRange, lookDir, 0f, 180f, bloodLayers);


            /*if (m_FacingRight)
            {
                hitEnemies = PhysicExtension.SphereSectorCastAll(bloodWeaponTransform.position, bloodAttackRange, 180, Vector2.right, float.PositiveInfinity, bloodLayers.value);
            }
            else
            {
                hitEnemies = PhysicExtension.SphereSectorCastAll(bloodWeaponTransform.position, bloodAttackRange, 180, Vector2.left, float.PositiveInfinity, bloodLayers.value);
            }*/

            foreach (RaycastHit enemy in hitEnemies)
            {
                if (enemy.collider.TryGetComponent(out ObjectHealth obj))
                {
                    this.AddHealth(lifeSteal);
                    obj.GetComponent<BloodEnemyController>().stunTime *= (1.0f + skillBonusFactor);
                    obj.GetComponent<BloodEnemyController>().pushBackFactor *= (1.0f + skillBonusFactor);
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

        //GetComponent<SpriteRenderer>().flipX = m_FacingRight;
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
        if (spiritSlashMask == null)
        {
            ReloadSpiritSlashMaskSprite();
        }

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

    private void SpiritAttack()
    {
        // Dodac Delay
        Vector2 origin = spiritSlashPosition.position;
        Vector2 lookDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - origin).normalized;
        //RaycastHit[] hits = PhysicExtension.SphereSectorCastAll(origin, circleRadius, sectorAngle, lookDir, float.PositiveInfinity, spiritLayers);
        RaycastHit[] hits = PhysicExtension.ConeCastAll(origin, circleRadius, lookDir, 0, sectorAngle, spiritLayers);
        for (uint i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject == gameObject)
            {
                continue;
            }
            if (((1 << hits[i].collider.gameObject.layer) & spiritLayers.value) == spiritLayers.value)
            {
                GameObject spiritEnemy = hits[i].collider.gameObject;

                this.AddHealth(lifeSteal);
                spiritEnemy.GetComponent<BasicSpirit>().stuntTime *= (1.0f + skillBonusFactor);

                spiritEnemy.GetComponent<BasicSpirit>().TakeDamage(spiritDamage);
                spiritEnemy.GetComponent<BasicSpirit>().SetStunt();

                Vector2 throwBackDir = spiritEnemy.transform.position - body.transform.position;
                throwBackDir.Normalize();
                //spiritEnemy.GetComponent<Rigidbody2D>().velocity += throwBackDir * spiritDamage * (1.0f + skillBonusFactor);
                spiritEnemy.GetComponent<Rigidbody>().velocity += (Vector3)(throwBackDir * spiritDamage * (1.0f + skillBonusFactor));
            }
        }

        // całe to rysowanie działa na razie dla kątów mniejszych równych 90 stopni
        GameObject slash = Instantiate(spiritSlashPrefab);
        slash.transform.position = spiritSlashPosition.position;
        float max = slashMaskPivot.x > slashMaskPivot.y ? slashMaskPivot.x : slashMaskPivot.y;
        float radius = circleRadius / (2f * (1f - max));
        slash.transform.localScale = new Vector3(1f, 1f) * radius + Vector3.forward;
        slash.transform.parent = spiritSlashPosition;
        slash.GetComponent<SpriteMask>().sprite = spiritSlashMask;
        SpriteRenderer renderer = slash.GetComponentInChildren<SpriteRenderer>();
        renderer.transform.localScale = 0.32f * 2f * radius * new Vector3(1f, 1f) + Vector3.forward; // NIE WIEM SKĄD 0.32 nie mam siły teraz tego liczyć i sprawdzać

        slash.transform.Rotate(Vector3.forward * this.transform.localScale.z, Vector2Extensions.Angle360(Vector2.right, lookDir) - sectorAngle / 2f);
        slash.transform.Translate(-slashMaskPivot * radius); // Wstęp do wyższych kątów (na razie nie działa)

        slash.GetComponentInChildren<Animator>().Play("SpiritSlashAnim", -1, 0f);
        Destroy(slash, .5f);
    }

    private void OnDrawGizmos()
    {
        if (IsSpirit())
        {
            if (spiritSlashPosition != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(transform.position, superAttackRadius);

                Vector2 origin = spiritSlashPosition.position;
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
        }
        else
        {
            if (bloodWeaponTransform != null)
            {
                //Gizmos.color = Color.red;
                //Gizmos.DrawWireSphere(bloodWeaponTransform.position, bloodAttackRange);

                Gizmos.color = Color.yellow;
                Vector2 origin = bloodWeaponTransform.position;

                float lookRadians = MathfExtensions.DegreesToRadians(Vector2Extensions.Angle360(Vector2.right, Vector2.right));
                float halfSectorRadians = MathfExtensions.DegreesToRadians(180 / 2f);
                float startRadians = lookRadians + halfSectorRadians;
                float endRadians = lookRadians - halfSectorRadians;

                Vector2 startPoint = (new Vector2(Mathf.Cos(startRadians), Mathf.Sin(startRadians))).normalized;
                Vector2 endPoint = (new Vector2(Mathf.Cos(endRadians), Mathf.Sin(endRadians))).normalized;

                Gizmos.color = Color.yellow;
                if (m_FacingRight)
                {
                    Gizmos.DrawLine(origin, origin + Vector2.right * bloodAttackRange);
                }
                else
                {
                    Gizmos.DrawLine(origin, origin + Vector2.left * bloodAttackRange);
                }

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(origin, origin + startPoint * bloodAttackRange);
                Gizmos.DrawLine(origin, origin + endPoint * bloodAttackRange);

                float radiansDiff = (endRadians - startRadians) / 10;
                Vector2 point1 = startPoint;
                for (int i = 0; i < 10 - 1; i++)
                {
                    Vector2 point2;
                    if (m_FacingRight)
                    {
                        point2 = (new Vector2(Mathf.Cos(startRadians + (i + 1) * radiansDiff), Mathf.Sin(startRadians + (i + 1) * radiansDiff))).normalized;                        
                    }
                    else
                    {
                        point2 = (new Vector2(Mathf.Cos(startRadians - (i + 1) * radiansDiff), Mathf.Sin(startRadians - (i + 1) * radiansDiff))).normalized;
                    }

                    Gizmos.DrawLine(origin + point1 * bloodAttackRange, origin + point2 * bloodAttackRange);

                    point1 = point2;
                }
                Gizmos.DrawLine(origin + point1 * bloodAttackRange, origin + endPoint * bloodAttackRange);
            }
        }
	}
	
    public override void OnDead()
    {
        gameController.DeadScreen();
    }
}
