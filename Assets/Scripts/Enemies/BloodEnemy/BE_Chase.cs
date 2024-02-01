using UnityEngine;

public class BE_Chase : IState
{
    private float timer = 0f;
    private bool left = false;
    private bool right = false;

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
        if(sc.GetHealth() < sc.prevHP)
        {
            sc.prevHP = sc.GetHealth();
            OnHurt(sc);
        }

        if(sc.checkRange())
        {
            sc.ChangeState(sc.attackState);
        }

        Vector2 direction = new(0,0);
        if (sc.target != null)
        {
            if (sc.transform.position.x > sc.target.transform.position.x && !left)
            {
                right = false;
                left = true;
                timer = 2f;
            }
            else if (sc.transform.position.x < sc.target.transform.position.x && !right)
            {
                left = false;
                right = true;
                timer = 2f;
            }
        }

        if (left && timer <= 0f)
        {
            direction = Vector2.left;
        }
        else if (right && timer <= 0f)
        {
            direction = Vector2.right;
        }

        Vector2 movement = sc.enemyMoveSpeed * direction;

        if(movement.x > 0)
        {
            movement.x *= 0.5f;
        }

        sc.transform.Translate(movement * Time.fixedDeltaTime );

        if(timer > 0f)
        {
            timer -= Time.fixedDeltaTime;
        }
    }
}
