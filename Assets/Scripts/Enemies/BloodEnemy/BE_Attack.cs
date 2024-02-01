using UnityEngine;

public class BE_Attack : IState
{
    private float lastAttackTime = -999f;
    Color startColor;
    Color attackColor = new(248.0f/255.0f , 131.0f / 255.0f, 121.0f / 255.0f);
    public void OnEnter(BloodEnemyController sc)
    {
        sc.anime.SetLayerWeight(1, 1);
        lastAttackTime = -999f;
        //startColor = sc.gameObject.GetComponent<SpriteRenderer>().color;
        startColor = sc.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.color;
    }

    public void OnExit(BloodEnemyController sc)
    {
        sc.anime.SetLayerWeight(1, 0);
    }

    public void OnHurt(BloodEnemyController sc)
    {
        //sc.gameObject.GetComponent<SpriteRenderer>().color = startColor;
        sc.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.color = startColor;
        sc.ChangeState(sc.hurtState);
    }

    public void UpdateState(BloodEnemyController sc)
    {
        float cooldownPercentage = Mathf.Clamp01((Time.time - lastAttackTime) / sc.enemyAttackDecay);
        Color lerpedColor = Color.Lerp(startColor, attackColor, cooldownPercentage);
        //sc.gameObject.GetComponent<SpriteRenderer>().color = lerpedColor;
        sc.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.color = lerpedColor;

        if (sc.GetHealth() < sc.prevHP)
        {
            sc.prevHP = sc.GetHealth();
            OnHurt(sc);
        }

        if (!sc.checkRange())
        {
            //sc.gameObject.GetComponent<SpriteRenderer>().color = startColor;
            sc.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.color = startColor;
            sc.ChangeState(sc.chaseState);
        }

        if(Time.time * GameTimer.TimeMultiplier > lastAttackTime + sc.enemyAttackDecay)
        {
            //Debug.Log("OUCH! taking dmg: " + sc.enemyAttackDmg);
            sc.target.GetComponent<Player>().TakeDamage(sc.enemyAttackDmg);
            sc.audioSource.Stop();
            sc.audioSource.clip = sc.clips[1];
            sc.audioSource.volume = 0.2f;
            sc.audioSource.pitch = 1;
            float add = (Random.Range(0, 20) - 10) / 100.0f;
            sc.audioSource.pitch += add;
            sc.audioSource.Play();
            lastAttackTime = Time.time;
        }
    }
}
