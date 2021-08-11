using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_slime : BHIII_character
{

    public int spawnAmount = 0;
    const int spawnLimit = 7;

    public new void Start()
    {
        base.Start();
        SetAIFunction(-1, IdleState);
        spawnAmount = 0;
    }

    public new void Update()
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
            EnableAttack();
            Dash(0.2f, 6f);
        }
    }

    public override void AfterDash()
    {
        DisableAttack();
        base.AfterDash();
        string str = currentAIFunction.Method.ToString();
        switch (str)
        {
            case "Void AttackState()":
                SetRandomDirection();
                SetAIFunction(0.7f, RetreatState);
                break;
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
