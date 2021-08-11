using System.Collections;
using System.Collections.Generic;
using MagnumFoudation;
using UnityEngine;

public class npc_solider : BHIII_character
{
    int shootamount = 6;
    const int ammoPerRound = 6;
    public enum SOLIDER_TYPE {
        REGULAR,
        ELITE,
        CORNEL
    }
    public SOLIDER_TYPE soldType;

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
        AnimMove(); if (target != null)
        {
            Vector2 tar = LookAtTarget(target);
            direction = tar;
            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            base.AttackState();
            if (CheckTargetDistance(target, 75))
            {
                StartCoroutineState(PreAttack());
            }
        }
        else
        {
            SetAIFunction(-1, IdleState);
        }
    }

    public void ShootAction()
    {
    }

    public void DelayState() {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetRandomDirection();
            SetAIFunction(1.5f, RetreatState);
        }
    }

    public void ShootState()
    {
        AnimMove(); if (target != null)
        {
            Vector2 tar = LookAtTarget(target);
            direction = tar;
            if (!CheckForDitch())
                CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            else
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;

            if (CheckTargetDistance(target, 155))
            {
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                direction = LookAtTarget(target);
                StartCoroutineState(SoliderShoot());
            }
        }
        else
        {
            SetAIFunction(-1, IdleState);
        }
    }
    public IEnumerator PreAttack()
    {
        yield return new WaitForSeconds(0.15f);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("attack", false);
        yield return new WaitForSeconds(0.4f);
        AttackGo(0.5f, direction);
        SetAIFunction(0.7f, RetreatState);
    }

    public IEnumerator SoliderShoot() {

        SetAnimation("shoot_idle", false);
        direction = LookAtTarget(target);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.5f);
        SetAnimation("shoot", true);
        switch (soldType) { 
            case SOLIDER_TYPE.REGULAR:
                for (int i = 0; i < 4; i++)
                {
                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                PlaySound("milbert_projectile");
                    ShootBullet(1, direction, 1.4f);
                    yield return new WaitForSeconds(0.08f);
                }
                break;
            case SOLIDER_TYPE.ELITE:
                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                PlaySound("milbert_projectile");
                angle = ReturnAngle2(direction);
                ShootBullet(1, angle, 1.4f);
                ShootBullet(1, angle + 20, 1.4f);
                ShootBullet(1, angle - 20, 1.4f);
                yield return new WaitForSeconds(0.2f);
                break;
            case SOLIDER_TYPE.CORNEL:
                angle = ReturnAngle2(direction);
                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                PlaySound("milbert_projectile");
                ShootBullet(1, angle, 1.4f);
                ShootBullet(1, angle + 20, 1.4f);
                ShootBullet(1, angle - 20, 1.4f);
                yield return new WaitForSeconds(0.1f);
                angle = ReturnAngle2(direction);
                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                PlaySound("milbert_projectile");
                ShootBullet(1, angle, 1.4f);
                ShootBullet(1, angle + 20, 1.4f);
                ShootBullet(1, angle - 20, 1.4f);
                direction = LookAtTarget(target);
                yield return new WaitForSeconds(0.1f);
                angle = ReturnAngle2(direction);
                for (int i = 0; i < 6; i++)
                {
                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                PlaySound("milbert_projectile");
                    ShootBullet(1, direction, 1.4f);
                    yield return new WaitForSeconds(0.08f);
                }
                break;
        }
        SetAnimation("reload", false);
        yield return new WaitForSeconds(1.5f);
        SetRandomDirection();
        SetAIFunction(0.5f,RetreatState);
    }


    public override void RetreatState()
    {
        AnimMove();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (AI_timerUp) {

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
