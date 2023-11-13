using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Assertions.Must;
using static UnityEngine.UI.Image;

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
        Vector2 origin = transform.position;
        Vector2 lookDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - origin).normalized;
        RaycastHit2D[] hits = Physic2DExtension.CircleSectorCastAll(origin, 5f, 45f, lookDir, 10f, int.MaxValue);
        for (uint i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject == gameObject)
            {
                continue;
            }
            if ((hits[i].collider.gameObject.layer & LayerMask.NameToLayer("Spirit")) == LayerMask.NameToLayer("Spirit"))
            {
                hits[i].collider.gameObject.GetComponent<BasicSpirit>().TakeDamage(10);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (form == PlayerForm.Spirit)
        {
            Vector2 origin = transform.position;
            Vector2 lookDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - origin).normalized;
            float radius = 5f;
            float maxDistance = 10f;
            float sectorAngle = 45f;

            float lookRadians = MathfExtensions.DegreesToRadians(Vector2Extensions.Angle360(Vector2.right, lookDir));

            float halfSectorRadians = MathfExtensions.DegreesToRadians(sectorAngle / 2f);
            float startRadians = lookRadians + halfSectorRadians;
            float endRadians = lookRadians - halfSectorRadians;

            float startAngle = MathfExtensions.RadiansToDegrees(startRadians);
            float endAngle = MathfExtensions.RadiansToDegrees(endRadians);
            endAngle -= startAngle;


            Vector2 startPoint = (new Vector2(Mathf.Cos(startRadians), Mathf.Sin(startRadians))).normalized;
            Vector2 endPoint = (new Vector2(Mathf.Cos(endRadians), Mathf.Sin(endRadians))).normalized;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, origin + lookDir * maxDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, origin + startPoint * maxDistance);
            Gizmos.DrawLine(origin, origin + endPoint * maxDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + startPoint * radius);
            Gizmos.DrawLine(origin, origin + endPoint * radius);

            float radiansDiff = (endRadians - startRadians) / 10;
            for (int i = 1; i < 11; i++)
            {
                Vector2 point1 = (new Vector2(Mathf.Cos(startRadians + (i - 1) * radiansDiff), Mathf.Sin(startRadians + (i - 1) * radiansDiff))).normalized;
                Vector2 point2 = (new Vector2(Mathf.Cos(startRadians + i * radiansDiff), Mathf.Sin(startRadians + i * radiansDiff))).normalized;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(origin + point1 * maxDistance, origin + point2 * maxDistance);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(origin + point1 * radius, origin + point2 * radius);
            }
        }
    }
}
