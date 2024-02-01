using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKiller : MonoBehaviour
{
    public List<ObjectHealth> enemys = new();
    public float currDelay = 2;

    // Update is called once per frame
    void Update()
    {
        if (enemys.Count == 0) return;

        currDelay -= Time.deltaTime;
        if (currDelay <= 0)
        {
            currDelay = Random.Range(0.5f, 1f);
            int index = Random.Range(0, enemys.Count);
            enemys[index].TakeDamage(enemys[index].GetMaxHealth());
            enemys.RemoveAt(index);
        }
    }
}
