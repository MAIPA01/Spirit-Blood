using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BE_Attack : IState
{
    private float lastAttackTime = -999f;

    public void OnEnter(BloodEnemyController sc)
    {
    }

    public void OnExit(BloodEnemyController sc)
    {

    }

    public void OnHurt(BloodEnemyController sc)
    {
        sc.ChangeState(sc.hurtState);
    }

    public void UpdateState(BloodEnemyController sc)
    {
        if (sc.GetHealth() < sc.prevHP)
        {
            sc.prevHP = sc.GetHealth();
            OnHurt(sc);
        }

        if (!sc.checkRange())
        {
            sc.ChangeState(sc.chaseState);
        }

        if(Time.time > lastAttackTime + sc.enemyAttackDecay)
        {
            //Debug.Log("OUCH! taking dmg: " + sc.enemyAttackDmg);
            sc.target.GetComponent<Player>().TakeDamage(sc.enemyAttackDmg);
            lastAttackTime = Time.time;
        }
    }
}
