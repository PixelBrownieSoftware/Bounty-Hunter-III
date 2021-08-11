using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_turret : BHIII_character
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

    public void ShootAction()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        ShootBullet(1, direction, 0.12f);
        SetRandomDirection();
        SetAIFunction(0.75f, DelayState);
    }

    public void DelayState()
    {

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAIFunction(1.5f, IdleState);
        }
    }

    public void ShootState()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;

        if (CheckTargetDistance(target, 275))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            AttackGoStand(0.25f);
            SetAIFunction(-1, ShootAction);
        }
    }


    public override void IdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        target = GetClosestTarget<BHIII_character>(300);
        if (target != null)
        {
            Vector2 tar = LookAtTarget(target);
            direction = tar;
            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

            SetAIFunction(-1, ShootState);
        }
    }
}
