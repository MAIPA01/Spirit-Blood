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
    [Header("Form Values:")]
    [SerializeField] 
    private PlayerForm form;

    [SerializeField]
    private SpriteRenderer body;

    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private float spiritDamage = 2f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private Color spiritColor = Color.black;
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
    private Color bloodColor = Color.white;

    void Start()
    {
        StartHealth();
    }

    void Update()
    {
        if (form == PlayerForm.Spirit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpiritAttack();
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
        if (form == PlayerForm.Spirit)
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
    }
}
