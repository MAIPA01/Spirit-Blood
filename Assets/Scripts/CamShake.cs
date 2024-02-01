using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    IEnumerator Shake (float duration, float magnitude)
    {
        Vector3 orginalPos = transform.localPosition;

        float elapsed = 0.0f;

        while(elapsed < duration) 
        { 
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, orginalPos.z);
        
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = orginalPos;
    }
}
