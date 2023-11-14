using UnityEngine;

public static class Vector2Extensions
{
    public static float Angle360(Vector2 from, Vector2 to)
    {
        float angleRight = Vector2.Angle(Vector2.right, to);
        float angleUp = Vector2.Angle(Vector2.up, to);
        float angleTo = angleRight;
        if (angleUp > 90f && angleRight > 90f)
        {
            angleTo = 360f - angleTo;
        }
        else if (angleUp > 90f && angleRight < 90f)
        {
            angleTo = -angleTo;
        }
        
        angleRight = Vector2.Angle(Vector2.right, from);
        angleUp = Vector2.Angle(Vector2.up, from);
        float angleFrom = angleRight;
        if (angleUp > 90f && angleRight > 90f)
        {
            angleFrom = 360f - angleFrom;
        }
        else if (angleUp > 90f && angleRight < 90f)
        {
            angleFrom = -angleFrom;
        }

        return (angleTo - angleFrom) % 360f;
    }
}
