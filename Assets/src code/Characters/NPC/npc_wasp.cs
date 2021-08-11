using MagnumFoudation;
using UnityEngine;

public class npc_wasp : BHIII_character
{
    public new void Start()
    {
        SetAttackObject<o_bullet>();
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
    }

    new private void Update()
    {
        base.Update();
        JumpWithoutGround(0.1f, 2);
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
            SetAIFunction(0.75f, GoFastPre);
        }
    }
    public void GoFastPre() {

        spriteOffset = new Vector2(Random.Range(-4, 4), Random.Range(-4, 4));
        if (AI_timerUp)
        {
            SetAIFunction(0.55f, GoFast);
        }
    }

    public void GoFast()
    {
        spriteOffset = new Vector2(Random.Range(-7, 7), Random.Range(-7, 7));
        terminalspd = terminalSpeedOrigin * 1.7f;
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        EnableAttack(direction);
        if (AI_timerUp)
        {
            terminalspd = terminalSpeedOrigin;
            DisableAttack();
            SetRandomDirection(); 
            spriteOffset = new Vector2(0, 0);
             SetAIFunction(1.3f, DelayState);
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
                break;
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
