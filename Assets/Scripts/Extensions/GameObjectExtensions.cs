using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static bool IsObjectInAnyLayer(this GameObject gameObject, LayerMask mask)
    {
        return ((1 << gameObject.layer) & mask.value) != 0;
    }
}
