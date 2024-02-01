using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = Time.time;
        Destroy(gameObject, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<MeshRenderer>().material.SetFloat("_Progress", Mathf.Clamp01(Time.time - timer) );
    }
}
