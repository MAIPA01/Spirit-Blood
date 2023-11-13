using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Physic2DExtension
{
    public static RaycastHit2D[] CircleSectorCastAll(Vector2 origin, float circleRadius, float sectorAngle, Vector2 lookDirection, float maxDistance, LayerMask layerMask)
    {
        float halfSectorAngle = sectorAngle / 2f;
        RaycastHit2D[] castHits = Physics2D.CircleCastAll(origin, circleRadius, Vector2.zero, maxDistance, layerMask);
        List<RaycastHit2D> circleSectorCastHits = new();

        if (castHits.Length > 0 )
        {
            for (uint i = 0; i < castHits.Length; i++)
            {
                Vector2 directionToObj = (Vector2)castHits[i].transform.position - origin;

                float angleToHit = Vector2Extensions.Angle360(lookDirection.normalized, directionToObj.normalized);

                if (angleToHit <= halfSectorAngle && angleToHit >= -halfSectorAngle)
                {
                    circleSectorCastHits.Add(castHits[i]);
                }
            }
        }

        float lookRadians = MathfExtensions.DegreesToRadians(Vector2Extensions.Angle360(Vector2.right, lookDirection));
        float halfSectorRadians = MathfExtensions.DegreesToRadians(halfSectorAngle);

        float startRadians = lookRadians + halfSectorRadians;
        Vector2 startPoint = (new Vector2(Mathf.Cos(startRadians), Mathf.Sin(startRadians))).normalized;
        castHits = Physics2D.RaycastAll(origin, startPoint, maxDistance, layerMask);
        if (castHits.Length > 0)
        {
            for (uint i = 0; i < castHits.Length; i++)
            {
                if (!circleSectorCastHits.Find((p) => p.collider == castHits[i].collider))
                {
                    circleSectorCastHits.Add(castHits[i]);
                }
            }
        }

        float endRadians = lookRadians - halfSectorRadians;
        Vector2 endPoint = (new Vector2(Mathf.Cos(endRadians), Mathf.Sin(endRadians))).normalized;
        castHits = Physics2D.RaycastAll(origin, endPoint, maxDistance, layerMask);
        if (castHits.Length > 0)
        {
            for (uint i = 0; i < castHits.Length; i++)
            {
                if (!circleSectorCastHits.Find((p) => p.collider == castHits[i].collider))
                {
                    circleSectorCastHits.Add(castHits[i]);
                }
            }
        }

        return circleSectorCastHits.ToArray();
    }
}
