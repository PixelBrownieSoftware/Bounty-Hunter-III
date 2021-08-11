using MagnumFoudation;
using System.Collections;
using UnityEngine;

public class npc_crocodile : BHIII_character
{
    public new void Start()
    {
        SetAttackObject<o_bullet>();
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
    }
    public IEnumerator CharacterAttk()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("attack", false);
        yield return new WaitForSeconds(0.45f);
        EnableAttack(direction);
        Dash(0.3f, 7.85f);
        while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        DisableAttack();
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.85f);
        SetAIFunction(0.9f, RetreatState);
    }

    public override void AttackState()
    {
        base.AttackState();
        SetAnimation("walk", true);
        direction = LookAtTarget(target);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckTargetDistance(target, 55))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            StartCoroutineState(CharacterAttk());
        }
    }

    public override void RetreatState()
    {
        SetAnimation("walk", true);
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
            int random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    Vector2 tar = LookAtTarget(target);
                    direction = tar;
                    angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                    SetAIFunction(1.8f, AttackState);
                    break;

                case 1:
                    SetRandomDirection();
                    SetAIFunction(1.5f, RetreatState);
                    break;
            }
        }
    }
}
