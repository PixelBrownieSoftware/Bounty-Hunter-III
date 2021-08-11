using MagnumFoudation;
using UnityEngine;

public class npc_demboar : BHIII_character
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
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 135))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            SetAIFunction(0.5f, BeforeDashState);
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
                SetAIFunction(1.3f, DelayState);
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
        base.RetreatState();
        if (AI_timerUp)
        {
            SetAIFunction(1.5f, RetreatState);
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
