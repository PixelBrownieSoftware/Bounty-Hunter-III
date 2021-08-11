using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class npc_snake : BHIII_character {

    public new void Start() {

        SetAttackObject<o_bullet>();
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
    }

    private new void Update()
    {
        AnimDir1D();
        base.Update();
    }

    public override void AttackState()
    {
        SetAnimation("walk_d", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 135))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            SetAnimation("attack_c", false);
            SetAIFunction(0.9f, BeforeDashState);
        }
    }

    public override void AfterDash()
    {
        DisableAttack();
        base.AfterDash();
        switch (currentAIFuncName)
        {
            case "BeforeDashState":
                SetRandomDirection();
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                SetAIFunction(0.7f, DelayState);
                break;
        }
    }
    public void BeforeDashState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.RetreatState();
        if (AI_timerUp)
        {
            EnableAttack();
            Dash(0.16f, 5f);
        }
    }

    public void DelayState()
    {
        SetAnimation("idle_d", true);
        if (AI_timerUp)
        {
            SetAIFunction(1.5f, RetreatState);
        }
    }

    public override void RetreatState()
    {
        SetAnimation("walk_d", true);
        base.RetreatState();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckIfCornered(direction * 3)) {
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

/*
public IEnumerator del()
{
    isHurt = true;
    SetAIFunction(-1, RetreatState);
    yield return new WaitForSeconds(0.7f);
    SetAIFunction(-1, AttackState);
    isHurt = false;
}

public IEnumerator Attack()
{
    angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

    direction = LookAtTarget(target);
    EnableAttack(direction);
    Dash(1.3f, 90f);//ShootBullet(1);

    yield return new WaitForSeconds(1.4f);
    DisableAttack();
    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
    StartCoroutine(del());
}

public void Attack()
{
    angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

    direction = LookAtTarget(target);
    EnableAttack(direction);
    Dash(0.5f, 150);//ShootBullet(1);
}

public override void AttackState()
{
    if (CHARACTER_STATE != CHARACTER_STATES.STATE_DASHING) {

        direction = LookAtTarget(target);

        if (CheckTargetDistance(target, 200))
        {
            if(attack_delay <= 0)
                Attack();
        }
        //MoveMotor();
        if (!CheckTargetDistance(target, 400))
        {
            print("AI FUNCTION ADDED");
            SetAIFunction(-1, IdleState);
        }

    }
}

public override void AfterDash()
{
    base.AfterDash();
    attack_delay = 1.85f;
    DisableAttack();
}

public override void RetreatState()
{
    direction = -LookAtTarget(target);

    if (!CheckTargetDistance(target, 400))
    {
        SetAIFunction(-1, AttackState);
    }

}

public override void IdleState()
{
    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
    if (target == null)
        target = GetClosestTarget<BHIII_character>(400);
    else
    if (CheckTargetDistance(target, 400))
    {
        print("AI FUNCTION ADDED");
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        SetAIFunction(-1, AttackState);
    }
}
*/
