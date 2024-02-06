using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralaxa : MonoBehaviour
{
    private float length;
    private Vector2 startpos;
    public GameObject cam;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        startpos = new Vector2(transform.position.x, transform.position.y);
        length = GetComponent<SpriteRenderer>().bounds.size.x*this.gameObject.transform.lossyScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        Vector2 distance = new Vector2(cam.transform.position.x, cam.transform.position.y) * parallaxEffect;

        transform.position = new Vector3(startpos.x + distance.x, startpos.y + distance.y, transform.position.z);

        if (temp > startpos.x + length) startpos.x += length;
        else if (temp < startpos.x - length) startpos.x -= length;
    }
}
