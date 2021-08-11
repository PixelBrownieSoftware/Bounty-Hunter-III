using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts a bit like snake but faster and can do a double attack
/// </summary>
public class npc_wolf : BHIII_character
{
    public new void Start()
    {
        base.Start();
        SetAIFunction(-1, IdleState);
    }

    public new void Update()
    {
        base.Update();
        if (collision == null)
            collision = GetComponent<BoxCollider2D>();
    }

    public override void AttackState()
    {
        SetAnimation("walk", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 185))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            SetAIFunction(0.65f, BeforeDashState);
        }
    }

    public override void AfterDash()
    {
        DisableAttack();
        base.AfterDash();
        string str = currentAIFunction.Method.ToString();
        switch ( str) {
            case "Void BeforeDashState()":
                SetRandomDirection();
                SetAIFunction(1.4f, DelayState);
                break;
        }
    }

    public void BeforeDashState()
    {
        SetAnimation("attack_prep", false);
        base.RetreatState();
        if (AI_timerUp)
        {
            EnableAttack();
            SetAnimation("attack", false);
            Dash(0.2f, 6f);
        }
    }

    public void DelayState()
    {
        SetAnimation("idle", false);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.RetreatState();
        if (AI_timerUp)
        {
            SetAIFunction(1.5f, RetreatState);
        }
    }

    public override void RetreatState()
    {
        SetAnimation("walk", true);
        base.RetreatState();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckIfCornered(direction * 3.6f)) {
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
