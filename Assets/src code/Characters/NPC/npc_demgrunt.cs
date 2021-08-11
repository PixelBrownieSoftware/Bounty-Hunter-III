using MagnumFoudation;
using UnityEngine;
using System.Collections;

public class npc_demgrunt : BHIII_character
{
    public new void Start()
    {
        SetAttackObject<o_bullet>(2);
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
    }

    public void ShootState()
    {
        SetAnimation("move", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
        if (CheckTargetDistance(target, 235))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            StartCoroutineState(BreatheFire());
        }
    }
    public override void AttackState()
    {
        SetAnimation("move", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
        if (CheckTargetDistance(target, 135))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            StartCoroutineState(PreAttack());
        }
    }
    public IEnumerator BreatheFire()
    {
        yield return new WaitForSeconds(0.15f);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("attack", false);
        yield return new WaitForSeconds(0.25f);
        direction = LookAtTarget(target);
        angle = ReturnAngle2(direction);
        ShootBullet("fireball", 2, angle + 20f, 0.9f, 6);
        ShootBullet("fireball", 2, angle - 30f, 0.8f, 6);
        ShootBullet("fireball",2, angle - 20f, 0.8f, 6);
        ShootBullet("fireball", 2, angle - 30f, 0.8f, 6);
        ShootBullet("fireball", 2, angle, 0.8f, 6);
        yield return new WaitForSeconds(0.2f);
        ShootBullet("fireball", 2, angle, 0.8f, 6);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.65f);
        SetRandomDirection();
        SetAIFunction(0.3f, RetreatState);
    }
    public IEnumerator PreAttack()
    {
        yield return new WaitForSeconds(0.15f);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("attack", false);
        direction = LookAtTarget(target);
        yield return new WaitForSeconds(0.3f);
        EnableAttack();
        Dash(0.13f, 16.5f);
        while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("idle", false);
        DisableAttack();
        yield return new WaitForSeconds(0.6f);
        SetRandomDirection();
        SetAIFunction(0.4f, RetreatState);
    }

    public override void RetreatState()
    {
        SetAnimation("move", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.RetreatState();
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
                    SetAIFunction(0.3f, RetreatState);
                    break;

                case 2:
                    SetAIFunction(3.1f, ShootState);
                    break;
            }
        }
    }
}
