using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class npc_komodo : BHIII_character
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
        base.AttackState();
        SetAnimation("move", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;

        if (CheckTargetDistance(target, 135))
        {
            SetAnimation("attack_pre", false);
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            SetAIFunction(0.8f, BeforeDashState);
        }
    }

    public override void AfterDash()
    {
        DisableAttack();
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("attack_end", false);
        base.AfterDash();
        string str = currentAIFunction.Method.ToString();
        switch (str)
        {
            case "Void BeforeDashState()":
                SetAIFunction(0.7f, DelayState);
                break;
        }
    }
    public void BeforeDashState()
    {
        if (AI_timerUp)
        {
            SetAnimation("attack", false);
            EnableAttack();
            Dash(0.2f, 6f);
        }
    }

    public void DelayState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetRandomDirection();
            SetAIFunction(1.3f, RetreatState);
        }
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
