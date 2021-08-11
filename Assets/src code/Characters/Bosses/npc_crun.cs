using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_crun : BHIII_character
{
    /// <summary>
    /// First boss
    /// Shoots at the player
    /// Jumps to do a ground attack and then spins around in an imperfect circle
    /// You have to fight 3
    /// </summary>
    float spinAngle = 0;
    int shootAmount = 5;
    int attackCount = 2;

    public new void Start()
    {
        healthPhases = new float[3]{
            0.8f,
            0.5f,
            0.4f
        };
        base.Start();
        SetAIFunction(-1, IdleState);
    }

    public IEnumerator ShootRepeat()
    {
        SetAnimation("shoot_prep", false);
        yield return new WaitForSeconds(0.7f);
        SetAnimation("shoot", true);
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        for (int i = 0; i < 15; i++) {

            angle += 20f;
            ShootBullet(1, angle, 1.6f, 8f);
            yield return new WaitForSeconds(0.2f);
        }
        for (int i = 0; i < 15; i++)
        {
            angle -= 20f;
            ShootBullet(1, angle, 20f, 8f);
            yield return new WaitForSeconds(0.2f);
        }
        SetAnimation("idle", true);
        yield return new WaitForSeconds(0.4f);
        SetAIFunction(0.65f, ShortDelay);
    }

    public override void AfterDefeat()
    {
        StartCoroutineState(ExplosionDeath());
    }

    public new void Update()
    {
        base.Update();
        if (collision == null)
            collision = GetComponent<BoxCollider2D>();
    }

    public override void AttackState()
    {
        AnimMove();
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 135))
        {
            SetAnimation("attack_prep", false);
            SetAIFunction(0.5f, BeforeAttk);
        }
    }
    public void BeforeAttk()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAnimation("attack", false);
            EnableAttack();
            Dash(0.2f, 7f);
        }
    }

    public override void AfterDash()
    {
        DisableAttack();
        base.AfterDash();
        if (healthPhase >= 1)
        {
            if (attackCount > 0)
            {
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                attackCount--;
                SetAIFunction(0.4f, BeforeAttk);
            }
            else {
                switch (currentAIFuncName)
                {
                    case "BeforeAttk":
                        SetAIFunction(0.55f, ShortDelay);
                        break;
                }
            }
        }
        else {

            switch (currentAIFuncName)
            {
                case "BeforeAttk":
                    SetAIFunction(0.45f, ShortDelay);
                    break;
            }
        }

    }
    public IEnumerator ShootShort()
    {
        SetAnimation("shoot_prep", false);
        yield return new WaitForSeconds(0.7f);
        SetAnimation("shoot", true);
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        for (int i = 0; i < 5; i++)
        {
            direction = LookAtTarget(target);
            ShootBullet(1, direction, 20f);
            yield return new WaitForSeconds(0.3f);
        }
        SetAnimation("idle", true);
        yield return new WaitForSeconds(0.1f);
        SetAIFunction(0.55f, ShortDelay);
    }

    public void ShortDelay()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAIFunction(0.85f, RetreatState);
        }
    }

    public override void RetreatState()
    {
        AnimMove();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.RetreatState();
        if (CheckIfCornered(direction))
        {
            direction *= -1;
        }
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
    }

    public override void IdleState()
    {
        AnimMove();
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        target = GetClosestTarget<BHIII_character>(999999);
        if (target != null)
        {
            int random = 0; Vector2 tar;
            print(healthPhase);
            switch (healthPhase) {
                case 0:

                    random = Random.Range(0, 2);
                    switch (random)
                    {
                        case 0:
                            tar = LookAtTarget(target);
                            direction = tar;
                            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                            SetAIFunction(3.1f, AttackState);
                            break;

                        case 1:

                            tar = LookAtTarget(target);
                            direction = tar;
                            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                            SetRandomDirection();
                            SetAIFunction(0.65f, RetreatState);
                            break;
                    }
                    break;

                case 1:

                    random = Random.Range(0, 3);
                    switch (random)
                    {
                        case 0:
                            tar = LookAtTarget(target);
                            direction = tar;
                            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
                            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                            StartCoroutineState(ShootShort());
                            break;

                        case 1:
                            tar = LookAtTarget(target);
                            direction = tar;
                            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                            SetAIFunction(3.1f, AttackState);
                            break;

                        case 2:
                            SetRandomDirection();
                            SetAIFunction(0.65f, RetreatState);
                            break;
                    }
                    break;
                case 2:

                    random = Random.Range(0, 4);
                    switch (random)
                    {
                        case 0:
                            tar = LookAtTarget(target);
                            direction = tar;
                            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
                            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                            StartCoroutineState(ShootShort());
                            break;

                        case 1:
                            tar = LookAtTarget(target);
                            direction = tar;
                            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                            SetAIFunction(3.1f, AttackState);
                            break;

                        case 2:
                            SetRandomDirection();
                            SetAIFunction(0.65f, RetreatState);
                            break;

                        case 3:
                            tar = LookAtTarget(target);
                            direction = tar;
                            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
                            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                            StartCoroutineState(ShootRepeat());
                            break;
                    }
                    break;

            }
        }
    }
}
