using UnityEngine;

public class BE_Hurt : IState
{
    private float timer;
    public void OnEnter(BloodEnemyController sc)
    {
        timer = Time.time;
        Vector2 throwBackDir = sc.transform.position - sc.target.transform.position;
        throwBackDir.Normalize();
        //sc.GetComponent<Rigidbody2D>().velocity += throwBackDir * sc.pushBackFactor;
        sc.GetComponent<Rigidbody>().velocity += (Vector3)(throwBackDir * sc.pushBackFactor);
    }

    public void OnExit(BloodEnemyController sc)
    {

    }

    public void OnHurt(BloodEnemyController sc)
    {

    }

    public void UpdateState(BloodEnemyController sc)
    {
        if (sc.GetHealth() <= 0.0f)
        {
            sc.OnDead();
        }

        if (Time.time >= timer + sc.stunTime)
        {
            sc.ChangeState(sc.chaseState);
        }
    }
}
