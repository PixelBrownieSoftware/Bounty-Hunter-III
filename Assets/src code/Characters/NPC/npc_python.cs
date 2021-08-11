using MagnumFoudation;
using UnityEngine;
using System.Collections;

public class npc_python : BHIII_character
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
        SetAnimation("move", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
        if (CheckTargetDistance(target, 135))
        {
            SetAnimation("attack", false);
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            SetAIFunction(0.5f, BeforeDashState);
        }
    }
    public void ShootState()
    {
        SetAnimation("move", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
        if (CheckTargetDistance(target, 175))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            StartCoroutineState(Shoot());
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
                SetAnimation("attack", false);
                SetAIFunction(0.9f, DelayState);
                break;
        }
    }
    public void BeforeDashState()
    {
        base.RetreatState();
        if (AI_timerUp)
        {
            EnableAttack();
            Dash(0.15f, 6f);
        }
    }

    IEnumerator Shoot() {

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        direction = LookAtTarget(target).normalized;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        rendererObj.color = Color.blue;
        SetAnimation("shoot", false);
        yield return new WaitForSeconds(0.4f);
        ShootBullet(1, angle, 1.25f, 6);
        yield return new WaitForSeconds(0.2f);
        ShootBullet(1, angle, 1.25f, 6);
        yield return new WaitForSeconds(0.2f);
        rendererObj.color = Color.white;
        SetAnimation("idle", false);
        yield return new WaitForSeconds(0.4f);
        SetRandomDirection();
        SetAIFunction(1.5f, RetreatState);
    }

    public void DelayState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAIFunction(0.5f, RetreatState);
        }
    }

    public IEnumerator PreAttack() {

        float tim = 0.6f;
        while (tim > 0)
        {
            tim -= Time.deltaTime;
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            if (CheckIfCornered(direction * 10))
                direction *= -1;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public override void RetreatState()
    {
        SetAnimation("move", true);
        base.RetreatState();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
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
                    SetAIFunction(0.75f, RetreatState);
                    break;

                case 2:
                    angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                    SetAIFunction(3.1f, ShootState);
                    break;
            }
        }
    }
}
