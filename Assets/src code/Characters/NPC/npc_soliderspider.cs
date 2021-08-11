using System.Collections;
using MagnumFoudation;
using UnityEngine;

public class npc_soliderspider : BHIII_character
{
    public new void Start()
    {
        SetAttackObject<o_bullet>();
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
    }
    public IEnumerator ShootSlime()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        direction = LookAtTarget(target);
        angle = ReturnAngle2(new Vector3(direction.x, direction.y, 0));
        SetAnimation("shoot", true);
        yield return new WaitForSeconds(0.5f);
        ShootBullet(1, angle + 6, 0.9f, 6);
        ShootBullet(1, angle - 6, 0.8f, 6);
        ShootBullet(1, angle, 0.8f, 6);
        yield return new WaitForSeconds(0.3f);
        SetAnimation("idle", true);
        yield return new WaitForSeconds(0.2f);
        SetAIFunction(1.5f, RetreatState);
    }
    public IEnumerator CharacterAttk()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("attack", false);
        yield return new WaitForSeconds(0.45f);
        EnableAttack(direction);
        Dash(0.4f, 4f);
        while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING) {

            yield return new WaitForSeconds(Time.deltaTime);
        }
        DisableAttack();
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.4f);
        SetAIFunction(0.9f, RetreatState);
    }

    public override void AttackState()
    {
        base.AttackState();
        SetAnimation("walk", true);
        direction = LookAtTarget(target);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (AI_timerUp)
            SetAIFunction(-1, IdleState);
        if (CheckTargetDistance(target, 55))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            StartCoroutineState(CharacterAttk());
        }
    }

    public override void RetreatState()
    {
        SetAnimation("walk", true);
        base.RetreatState(); 
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckIfCornered(direction * 3))
        {
            direction *= -1;
        }
        if (AI_timerUp)
            SetAIFunction(-1, IdleState);
    }

    public void ShootState()
    {
        SetAnimation("move", true);
        direction = LookAtTarget(target);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if(AI_timerUp)
            SetAIFunction(-1, IdleState);

        base.AttackState();
        if (CheckTargetDistance(target, 355))
        {
            StartCoroutineState(ShootSlime());
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

                    SetAIFunction(1.8f, AttackState);
                    break;

                case 1:
                    SetRandomDirection();
                    SetAIFunction(1.5f, RetreatState);
                    break;

                case 2:
                    direction = LookAtTarget(target);
                    SetAIFunction(1.8f, ShootState);
                    break;
            }
        }
    }
}
