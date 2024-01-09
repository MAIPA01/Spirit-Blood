using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BE_Attack : IState
{
    private float lastAttackTime = -999f;
    Color startColor;
    Color attackColor = new Color(248.0f/255.0f , 131.0f / 255.0f, 121.0f / 255.0f);
    public void OnEnter(BloodEnemyController sc)
    {
        lastAttackTime = -999f;
        startColor = sc.gameObject.GetComponent<SpriteRenderer>().color;
    }

    public void OnExit(BloodEnemyController sc)
    {

    }

    public void OnHurt(BloodEnemyController sc)
    {
        sc.gameObject.GetComponent<SpriteRenderer>().color = startColor;
        sc.ChangeState(sc.hurtState);
    }

    public void UpdateState(BloodEnemyController sc)
    {
        float cooldownPercentage = Mathf.Clamp01((Time.time - lastAttackTime) / sc.enemyAttackDecay);
        Color lerpedColor = Color.Lerp(startColor, attackColor, cooldownPercentage);
        sc.gameObject.GetComponent<SpriteRenderer>().color = lerpedColor;

        if (sc.GetHealth() < sc.prevHP)
        {
            sc.prevHP = sc.GetHealth();
            OnHurt(sc);
        }

        if (!sc.checkRange())
        {
            sc.gameObject.GetComponent<SpriteRenderer>().color = startColor;
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
