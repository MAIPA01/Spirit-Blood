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

public static class PhysicExtension
{
    public static RaycastHit[] SphereSectorCastAll(Vector3 origin, float sphereRadius, float angle, Vector3 lookDirection, float maxDistance, LayerMask layerMask)
    {
        float halfSectorAngle = angle / 2f;
        RaycastHit[] castHits = Physics.SphereCastAll(origin, sphereRadius, Vector3.zero, maxDistance, layerMask);
        List<RaycastHit> sphereSectorCastHits = new();

        Vector2 lookDirXY = new(lookDirection.x, lookDirection.y);
        Vector2 lookDirXZ = new(lookDirection.x, lookDirection.z);

        if (castHits.Length > 0)
        {
            for (uint i = 0; i < castHits.Length; i++)
            {
                Vector3 directionToObj = castHits[i].transform.position - origin;

                // testXY
                Vector2 dirToObjXY = new(directionToObj.x, directionToObj.y);
                float angleToHitXY = Vector2Extensions.Angle360(lookDirXY.normalized, dirToObjXY.normalized);
                // testXZ
                Vector2 dirToObjXZ = new(directionToObj.x, directionToObj.z);
                float angleToHitXZ = Vector2Extensions.Angle360(lookDirXZ.normalized, dirToObjXZ.normalized);

                if ((angleToHitXY <= halfSectorAngle && angleToHitXY >= -halfSectorAngle) && (angleToHitXZ <= halfSectorAngle && angleToHitXZ >= -halfSectorAngle))
                {
                    sphereSectorCastHits.Add(castHits[i]);
                }
            }
        }

        float lookRadiansXY = MathfExtensions.DegreesToRadians(Vector2Extensions.Angle360(Vector2.right, lookDirXY));
        float lookRadiansXZ = MathfExtensions.DegreesToRadians(Vector2Extensions.Angle360(Vector2.right, lookDirXZ));
        float halfSectorRadians = MathfExtensions.DegreesToRadians(halfSectorAngle);

        float startRadians = lookRadiansXY + halfSectorRadians;
        Vector3 startPoint = (new Vector2(Mathf.Cos(startRadians), Mathf.Sin(startRadians))).normalized;
        castHits = Physics.RaycastAll(origin, startPoint, maxDistance, layerMask);
        if (castHits.Length > 0)
        {
            for (uint i = 0; i < castHits.Length; i++)
            {
                if (sphereSectorCastHits.FindAll((p) => p.collider == castHits[i].collider).Count == 0)
                {
                    sphereSectorCastHits.Add(castHits[i]);
                }
            }
        }

        float endRadians = lookRadiansXY - halfSectorRadians;
        Vector3 endPoint = (new Vector2(Mathf.Cos(endRadians), Mathf.Sin(endRadians))).normalized;
        castHits = Physics.RaycastAll(origin, endPoint, maxDistance, layerMask);
        if (castHits.Length > 0)
        {
            for (uint i = 0; i < castHits.Length; i++)
            {
                if (sphereSectorCastHits.FindAll((p) => p.collider == castHits[i].collider).Count == 0)
                {
                    sphereSectorCastHits.Add(castHits[i]);
                }
            }
        }

        // Nie jest to dobre ale dzia³a do naszej gry która jest semi-3D

        return sphereSectorCastHits.ToArray();
    }
}
