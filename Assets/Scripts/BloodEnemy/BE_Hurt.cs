using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BE_Hurt : IState
{
    private float timer;
    public void OnEnter(BloodEnemyController sc)
    {
        timer = Time.time;
    }

    public void OnExit(BloodEnemyController sc)
    {

    }

    public void OnHurt(BloodEnemyController sc)
    {

    }

    public void UpdateState(BloodEnemyController sc)
    {
        if (sc.GetHealth() <= 0f)
        {
            sc.target.GetComponent<Player>().score += sc.scoreGained;
            sc.Die();
        }

        if (Time.time >= timer + sc.stunTime)
        {
            sc.ChangeState(sc.chaseState);
        }
    }
}
