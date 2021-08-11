using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_viper : BHIII_character
{
    public new void Start()
    {
        base.Start();
        SetAIFunction(-1, IdleState);
    }

    public override void AttackState()
    {
        if (target != null)
        {
            SetAnimation("move", true);
            Vector2 tar = LookAtTarget(target);
            direction = tar;
            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            base.AttackState();
            if (CheckTargetDistance(target, 135))
            {

                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                SetAnimation("attack", false);
                SetAIFunction(0.5f, BeforeDashState);
            }
        }
        else
        {
            SetAIFunction(-1, IdleState);
        }
    }
    public void ShootState()
    {
        if (target != null)
        {
            SetAnimation("move", true);
            Vector2 tar = LookAtTarget(target);
            direction = tar;
            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;

            if (CheckTargetDistance(target, 265))
            {
                StartCoroutineState(ShootAction());
            }
        }
        else
        {
            SetAIFunction(-1, IdleState);
        }
    }
    IEnumerator ShootAction()
    {
        AnimMove();
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        direction = LookAtTarget(target).normalized;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        SetAnimation("shoot", false);
        yield return new WaitForSeconds(0.4f);
        ShootBullet(1, angle, 20f);
        SetAnimation("idle", false);
        yield return new WaitForSeconds(0.4f);
        SetRandomDirection();
        SetAIFunction(0.85f, RetreatState);
    }

    public override void AfterDash()
    {
        DisableAttack();
        base.AfterDash();
        string str = currentAIFunction.Method.ToString();
        switch (str)
        {
            case "Void BeforeDashState()":
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                SetAIFunction(0.6f, DelayState);
                break;
        }
    }
    public void BeforeDashState()
    {
        if (AI_timerUp)
        {
            EnableAttack();
            Dash(0.2f, 6f);
        }
    }

    public void DelayState()
    {
        if (AI_timerUp)
        {
            SetAIFunction(0.7f, RetreatState);
        }
    }

    public override void RetreatState()
    {
        SetAnimation("move", true) ;
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
                    SetAIFunction(2.5f, RetreatState);
                    break;

                case 2:
                    SetAIFunction(1.5f, ShootState);
                    break;
            }
        }
    }
}
