using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfExtensions
{
    public static float DegreesToRadians(float angle)
    {
        return angle * Mathf.Deg2Rad;
    }

    public static float RadiansToDegrees(float radians)
    {
        return radians * Mathf.Rad2Deg;
    }
}
