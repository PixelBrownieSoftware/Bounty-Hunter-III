using MagnumFoudation;
using System.Collections;
using UnityEngine;

public class npc_newt : BHIII_character
{

    public new void Start()
    {
        SetAttackObject<o_bullet>();
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
    }
    public void DoNothing()
    {
        SetAnimation("idle", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
    }
    public void ShootState()
    {
        SetAnimation("move", true);
        direction = LookAtTarget(target);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 355))
        {
            StartCoroutineState(ShootSlime());
        }
    }

    new private void Update()
    {
        //JumpWithoutGround(2.5f,5.6f);
        base.Update();
        if (collision == null)
            collision = GetComponent<BoxCollider2D>();
    }
    public IEnumerator ShootSlime()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        direction = LookAtTarget(target);
        angle = ReturnAngle2(new Vector3(direction.x, direction.y, 0));
        SetAnimation("shoot", true);
        yield return new WaitForSeconds(0.5f);
        ShootBullet(1, angle + 6, 0.9f,6);
        ShootBullet(1, angle - 6, 0.8f, 6);
        ShootBullet(1, angle, 0.8f, 6);
        yield return new WaitForSeconds(0.3f);
        SetAnimation("idle", true);
        yield return new WaitForSeconds(0.2f);
        SetAIFunction(1.5f, RetreatState);
    }



    public override void RetreatState()
    {
        SetAnimation("move", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
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
            int random = Random.Range(0, 3);
            switch (random)
            {
                case 0:
                    SetRandomDirection();
                    SetAIFunction(0.8f, RetreatState);
                    break;

                case 1:
                    SetAIFunction(2.7f, ShootState);
                    break;

                case 2:
                    SetAIFunction(0.7f, DoNothing);
                    break;
            }
        }
    }
}
