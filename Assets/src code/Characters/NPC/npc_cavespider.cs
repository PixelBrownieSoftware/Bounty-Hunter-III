using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class npc_cavespider : BHIII_character
{
    public new void Start()
    {
        SetAttackObject<o_bullet>();
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
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
                SetAnimation("attack", false);
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                SetAIFunction(0.5f, BeforeDashState);
            }
        }
        else
        {
            SetAIFunction(-1, IdleState);
        }
    }

    public override void AfterDash()
    {
        DisableAttack();
        base.AfterDash();
        string str = currentAIFunction.Method.ToString();
        switch (str)
        {
            case "Void BeforeDashState()":
                if (target != null)
                    SetRandomDirection();
                else
                    direction = direction * -1;
                SetAIFunction(0.75f, DelayState);
                break;
        }
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

    public void DelayState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("idle", true);
        base.RetreatState();
        if (AI_timerUp)
        {
            SetRandomDirection();
            SetAIFunction(0.95f, RetreatState);
        }
    }

    public override void RetreatState()
    {
        SetAnimation("move", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckIfCornered(direction * 3))
        {
            direction *= -1;
        }
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
            int random = Random.Range(0, 1);
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
