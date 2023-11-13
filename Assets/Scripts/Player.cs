using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

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
    private LayerMask enemyLayers;

    [SerializeField]
    private bool m_FacingRight = true;

    [SerializeField]
    private Animator slashAnimator;

    [Header("Forms:")]
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private float spiritDamage = 2f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private Color spiritColor = Color.black;

    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private float bloodDamage = 1f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private Color bloodColor = Color.white;

    [Header("Attack")]
    [SerializeField]
    private float attackDelay = 1f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private Transform bloodWeaponTransform;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private float bloodAttackRange = 0.5f;

    private bool canAttack = true;

    private void OnValidate()
    {
        if (slashAnimator != null)
        {
            Debug.LogWarning("Slash Animator wasn't provided!!!");
        }

        if (!IsSpirit() && bloodWeaponTransform == null)
        {
            Debug.LogError("Blood Weapon Transform is needed for attack to work!!!");
        }
    }

    void Start()
    {
        StartHealth();
    }

    void Update()
    {
        if (Input.mousePosition.x >= Screen.width / 2 && !m_FacingRight)
        {
            Flip();
        }
        else if (Input.mousePosition.x < Screen.width / 2 && m_FacingRight)
        {
            Flip();
        }

        if (Input.GetMouseButtonDown(0) && canAttack)
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
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    private void BloodAttack()
    {
        if (bloodWeaponTransform != null)
        {
            if (slashAnimator != null)
            {
                slashAnimator.Play("SlashAnim", -1, 0.0f);
            }

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(bloodWeaponTransform.position, bloodAttackRange, enemyLayers);

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

    private void OnDrawGizmosSelected()
    {
        if (bloodWeaponTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bloodWeaponTransform.position, bloodAttackRange);
        }
    }

    [Button]
    private void ChangeFormTest()
    {
        ChangeForm();
    }
}
