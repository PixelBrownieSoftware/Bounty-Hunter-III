using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_eagle : BHIII_character
{

    public float flapDownGrav;

    private new void Start()
    {
        base.Start();
        SetAIFunction(-1, IdleState);
    }

    new private void Update()
    {
        //JumpWithoutGround(2.5f,5.6f);
        base.Update();
    }


    public override void AttackState() {

        SetAnimation("move", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckTargetDistance(target, 90))
        {

            SetAnimation("attack", false);
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            SetAIFunction(0.6f, BeforeDashState);
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
                SetRandomDirection();
                SetAIFunction(0.9f, DelayState);
                break;
        }
    }
    public void BeforeDashState()
    {
        if (AI_timerUp)
        {
            EnableAttack();
            Dash(0.2f, 4.5f);
        }
    }

    public void DelayState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("idle", true);
        if (AI_timerUp)
        {
            SetAIFunction(0.65f, RetreatState);
        }
    }
    public override void RetreatState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        SetAnimation("move", true);
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

        target = GetClosestTarget<BHIII_character>(700);
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
                    SetAIFunction(2.5f, RetreatState);
                    break;
            }
        }
    }
}
