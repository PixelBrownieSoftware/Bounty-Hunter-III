using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_soliderlgun : BHIII_character
{

    private new void Start()
    {
        base.Start();
        SetAIFunction(-1, IdleState);
    }

    new void Update()
    {
        base.Update();
        if (collision == null)
            collision = GetComponent<BoxCollider2D>();
    }

    public override void AttackState()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 135))
        {
            AttackGo(0.1f, tar);
            SetRandomDirection();
            SetAIFunction(0.7f, RetreatState);
        }
    }

    public void ShootAction()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        ShootBullet(1, direction, 0.12f);
        SetRandomDirection();
        SetAIFunction(0.35f, DelayState);
    }

    public void DelayState()
    {

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAIFunction(1.5f, RetreatState);
        }
    }

    public void ShootState()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        if (!CheckForDitch())
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        else
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;

        if (CheckTargetDistance(target, 170))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            AttackGoStand(0.25f);
            SetAIFunction(-1, ShootAction);
        }
    }

    public override void RetreatState()
    {
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

        target = GetClosestTarget<BHIII_character>(300);
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
                    SetAIFunction(3.7f, ShootState);
                    break;

                case 2:
                    SetRandomDirection();
                    SetAIFunction(2.5f, RetreatState);
                    break;
            }
        }
    }
}
