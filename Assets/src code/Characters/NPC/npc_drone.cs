using MagnumFoudation;
using UnityEngine;
using System.Collections;

public class npc_drone : BHIII_character
{
    public new void Start()
    {
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
    }

    public override void AttackState()
    {
        SetAnimation("hover", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 195))
        {
            StartCoroutineState(ShootGun());
        }
    }
    public IEnumerator ShootGun()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("gun_out", false);
        direction = LookAtTarget(target);
        yield return new WaitForSeconds(0.9f);
        SetAnimation("gun_fire", true);
        for (int i = 0; i < 15; i++) {
            angle = ReturnAngle2(new Vector3(direction.x, direction.y, 0));
            angle += Random.Range(8f,-8f);
            ShootBullet(1, angle, 20f, 8f);
            yield return new WaitForSeconds(0.1f);
        }
        SetAnimation("gun_in", false);
        yield return new WaitForSeconds(0.9f);
        SetAnimation("hover", true);
        SetRandomDirection();
        SetAIFunction(0.4f, RetreatState);
    }

    public void BeforeDashState()
    {
        base.RetreatState();
        if (AI_timerUp)
        {
            EnableAttack();
            Dash(0.2f, 6f);
        }
    }

    public override void AfterDefeat()
    {
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-25, -25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(25, -25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-25, 25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(25, 25), Quaternion.identity);
        PlaySound("explode_sound");
        base.AfterDefeat();
    }
    public void DelayState()
    {
        base.RetreatState();
        if (AI_timerUp)
        {
            SetAIFunction(1.5f, RetreatState);
        }
    }

    public override void RetreatState()
    {
        SetAnimation("hover", true);
        base.RetreatState();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
    }

    public override void IdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        target = GetClosestTarget<BHIII_character>(460);
        if (target != null)
        {
            int random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    Vector2 tar = LookAtTarget(target);
                    direction = tar;
                    angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                    SetAIFunction(3.1f, AttackState);
                    break;

                case 1:
                    SetRandomDirection();
                    SetAIFunction(2.5f, RetreatState);
                    break;
            }
        }
    }
}
