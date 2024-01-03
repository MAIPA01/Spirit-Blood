using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteChanger : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprite;
    int i = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        if(i >= sprite.Length * 5)
        {
            i = 0;
        }
        GetComponent<SpriteRenderer>().sprite = sprite[i/5];
        i++;
    }
}
