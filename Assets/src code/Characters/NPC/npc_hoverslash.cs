using System.Collections;
using System.Collections.Generic;
using MagnumFoudation;
using UnityEngine;

public class npc_hoverslash : BHIII_character
{
    public new void Start()
    {
        base.Start();
        SetAIFunction(-1, IdleState);
        _Z_offset = 4;
        grounded = false;
    }

    public override void IdleState()
    {
        SetAnimation("hover", true);
        base.IdleState();
        if (target != null)
        {
            direction = LookAtTarget(target);
            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            int random = Random.Range(0, 3);
            switch (random)
            {
                case 0:
                    SetAIFunction(1.8f, ShootState);
                    break;

                case 1:
                    SetAIFunction(2.6f, AttackState);
                    break;

                case 2:
                    SetRandomDirection();
                    SetAIFunction(0.65f, RetreatState);
                    break;
            }

        }
        else {
            target = GetClosestTarget<BHIII_character>(450);
        }

    }

    public override void AttackState()
    {
        base.AttackState();
        direction = LookAtTarget(target);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckTargetDistance(target, 115))
        {
            StartCoroutineState(AttackSlash());
        }
        else if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
    }

    public void ShootState() {

        SetAnimation("hover", true);
        direction = LookAtTarget(target);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckTargetDistance(target, 215))
        {
            StartCoroutineState(ShootProj());
        } else if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }

    }

    public new void Update()
    {
        base.Update();
        _Z_offset = 20;
    }

    public IEnumerator AttackSlash() {

        SetAnimation("attk_pre", false);
        yield return new WaitForSeconds(0.6f);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("attk", true);
        EnableAttack();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        float ti = 1.35f;
        angle = ReturnAngle2(direction);
        terminalspd = terminalSpeedOrigin * 1.75f;
        while (ti > 0)
        {
            angle += Mathf.PI * Random.Range(0.0005f, 0.007f);
            direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)).normalized;
            ti -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        terminalspd = terminalSpeedOrigin;
        SetAnimation("hover", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        DisableAttack();
        yield return new WaitForSeconds(1.45f);
        SetRandomDirection();
        SetAIFunction(0.6f, RetreatState);
    }
    public IEnumerator ShootProj() {

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("attk_pre", false);
        yield return new WaitForSeconds(0.7f);
        direction = LookAtTarget(target);
        yield return new WaitForSeconds(0.1f);
        for (int i2 = 0; i2 < 3; i2++)
        {
            yield return new WaitForSeconds(0.08f);
                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                PlaySound("milbert_projectile");
            ShootBullet(1, direction, 6.6f);
            yield return new WaitForSeconds(0.08f);
        }
        yield return new WaitForSeconds(0.8f);
        SetRandomDirection();
        SetAIFunction(0.6f, RetreatState);
    }

    public override void RetreatState()
    {
        SetAnimation("hover", true);
        base.RetreatState();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckIfCornered(direction * 10))
            direction *= -1;
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }

    }
}
