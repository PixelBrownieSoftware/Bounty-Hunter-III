using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class npc_lamprey : BHIII_character
{

    public new void Start()
    {
        SetAttackObject<o_bullet>();
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
    }

    public void ShootState()
    {
        SetAnimation("move", true);
        direction = LookAtTarget(target);
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckTargetDistance(target, 185))
        {
            StartCoroutineState(Shoot());
        }
    }

    public override void AttackState()
    {
        SetAnimation("move",true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 135))
        {
            StartCoroutineState(PreAttack());
        }
    }

    IEnumerator Shoot()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        direction = LookAtTarget(target).normalized;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        SetAnimation("shoot_pre", false);
        yield return new WaitForSeconds(0.4f);
        SetAnimation("shoot", true);
        ShootBullet(1, angle, 20f);
        yield return new WaitForSeconds(0.2f);
        ShootBullet(1, angle, 20f);
        yield return new WaitForSeconds(0.2f);
        ShootBullet(1, angle, 20f);
        yield return new WaitForSeconds(0.2f);
        SetAnimation("idle", false);
        yield return new WaitForSeconds(0.4f);
        SetAIFunction(1.5f, RetreatState);
    }

    public IEnumerator PreAttack()
    {
        yield return new WaitForSeconds(0.15f);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("attack", false);
        direction = LookAtTarget(target);
        yield return new WaitForSeconds(0.4f);
        EnableAttack();
        Dash(0.15f, 6f);
        while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("idle", false);
        DisableAttack();
        yield return new WaitForSeconds(0.1f);
        SetAnimation("attack", false);
        direction = LookAtTarget(target);
        yield return new WaitForSeconds(0.4f);
        EnableAttack();
        Dash(0.15f, 6f);
        while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("idle", false);
        DisableAttack();
        yield return new WaitForSeconds(0.2f);
        SetRandomDirection();
        SetAIFunction(0.4f, RetreatState);
    }

    public void DelayState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAIFunction(1.5f, RetreatState);
        }
    }

    public override void RetreatState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        SetAnimation("move", true);
        base.RetreatState();
        if (CheckIfCornered(direction * 10))
            direction *= -1;

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
            int random = Random.Range(0, 3);
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

                case 2:
                    SetAIFunction(2.5f, ShootState);
                    break;
            }
        }
    }
}
