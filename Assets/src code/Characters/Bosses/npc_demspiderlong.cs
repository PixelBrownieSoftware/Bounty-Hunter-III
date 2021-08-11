using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

/// <summary>
/// Fast speed and shooting spikey projectiles
/// Runs around and dashes
/// Millbert can stun her
/// Later on she can spawn cave spiders
/// She jumps to several set locations
/// </summary>
public class npc_demspiderlong : BHIII_character
{
    public GameObject[] moveNShootObjects;
    public Vector2[] moveNShootPositions;
    int moveShootPosPoint;

    public GameObject jumpOBject;
    public Vector2 jumpPosition;

    public GameObject MilbertJumpOBJ;
    public Vector2 MilbertJumpPosition;

    public List<npc_cavespider> caveSpiders;
    public BHIII_character[] spawnCharacter;
    public bool PeastSide = true;

    public GameObject[] peastJumpObjects;
    public Vector2[] peastJumpPositions;

    public GameObject[] milbertJumpObjects;
    public Vector2[] milbertJumpPositions;


    public BoxCollider2D JumpAttk;

    bool defeat = false;

    public string JumpLabel;

    public new void Start()
    {
        base.Start();
        if(MilbertJumpOBJ != null)
            MilbertJumpPosition = MilbertJumpOBJ.transform.position;
        SetAIFunction(-1, IdleState);
        if (jumpOBject != null)
            jumpPosition = jumpOBject.transform.position;

        milbertJumpPositions = TeleportObjectsToPositions(milbertJumpObjects);
        peastJumpPositions = TeleportObjectsToPositions(peastJumpObjects);

        healthPhases = new float[2]{ 0.85f, 0.85f };
    }

    public void StopCharging() {

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAIFunction(0.3f, DelayToIdleState);
    }

    public new void Update()
    {
        base.Update();
    }


    public override void AttackState()
    {
        SetAnimation("move", true);
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 315))
        {
            if (PeastSide)
            {
                if (healthPhase == 0)
                {
                    StartCoroutineState(ShootEnumState());
                }
                else
                {
                    StartCoroutineState(ShootEnumState2());
                }
            }
            else
            {
                StartCoroutineState(ShootEnumState2());
            }
        }
    }

    public void BeforeDash()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp) {

            EnableAttack();
            Dash(0.6f, 6f);
        }
    }

    public override void AfterDash()
    {
        DisableAttack();
        base.AfterDash();
        SetAnimation("idle", false);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        switch (currentAIFuncName)
        {
            case "BeforeDash":
                DisableAttack();
                SetRandomDirection();
                SetAIFunction(0.65f, DelayToRetreatState);
                break;
        }
    }

    /*
    public void BeforeCharge()
    {
        SetAnimation("move", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.RetreatState();
        if (AI_timerUp)
        {
            terminalspd = terminalSpeedOrigin * 2;
            SetAIFunction(4.5f, ChargeAttack);
        }
    }
    public void ChargeAttack()
    {
        SetAnimation("move", true,2);
        terminalspd = terminalSpeedOrigin * 2;
        direction = LookAtTarget(target);
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (AI_timerUp)
        {
            terminalspd = terminalSpeedOrigin;
            SetAIFunction(0.5f, DelayToIdleState);
        }
    }
    */

    public void JumpPause()
    {
    }

    public IEnumerator JumpToOtherSide() {

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        s_triggerhandler.trig.JumpToEvent(JumpLabel, false);
        yield return new WaitForSeconds(3f);
        yield return new WaitForSeconds(2f);
        PeastSide = false;
        target = null;
        SetAIFunction(-1, IdleState);
    }

    public IEnumerator ChargeAttack()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        direction = LookAtTarget(target);
        SetAnimation("attack", false);
        PlaySound("demon_spider_attack2");
        yield return new WaitForSeconds(0.45f);
        EnableAttack();
        Dash(0.3f, 6.85f);
        while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        dashdelay = 0;
        DisableAttack();
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (healthPhase > 0)
        {
            yield return new WaitForSeconds(0.15f);

            direction = LookAtTarget(target);
            SetAnimation("attack", false);
            PlaySound("demon_spider_attack2");
            yield return new WaitForSeconds(0.45f);
            EnableAttack();
            Dash(0.3f, 7.85f);
            while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            DisableAttack();
        }
        SetAnimation("idle", true);
        yield return new WaitForSeconds(1.5f);
        SetAIFunction(-1, IdleState);
    }

    public IEnumerator JumpAttack() {

        DisableAttack();
        DisableAttack(JumpAttk);
        for (int i = 0; i < 4; i++) {

            DisableAttack(JumpAttk);
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            PlaySound("demon_spider_attack2");
            yield return new WaitForSeconds(0.1f);
            Jump(3.5f);

            while (gravity > 0)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            if (PeastSide)
                transform.position = target.transform.position;
            else
                transform.position = target.transform.position;

            collision.enabled = false;
            isInvicible = true;
            while (!grounded)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            collision.enabled = true;
            isInvicible = false;
            EnableAttack(JumpAttk);
            s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
                transform.position + new Vector3(50, 50), Quaternion.identity);
            s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
                transform.position + new Vector3(-50, 50), Quaternion.identity);
            s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
                transform.position + new Vector3(50, -50), Quaternion.identity);
            s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
                transform.position + new Vector3(-50, -50), Quaternion.identity);
            s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
                transform.position + new Vector3(-25, -25), Quaternion.identity);
            s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
                transform.position + new Vector3(25, -25), Quaternion.identity);
            s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
                transform.position + new Vector3(-25, 25), Quaternion.identity);
            s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
                transform.position + new Vector3(25, 25), Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
            DisableAttack(JumpAttk);
            yield return new WaitForSeconds(0.3f);
        }
        DisableAttack(JumpAttk);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.1f);
        Jump(3.5f);

        while (gravity > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (PeastSide)
            transform.position = peastJumpPositions[0];
        else
            transform.position = milbertJumpPositions[0];

        collision.enabled = false;
        isInvicible = true;
        while (!grounded)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        collision.enabled = true;
        isInvicible = false;
        EnableAttack(JumpAttk);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(50, 50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-50, 50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(50, -50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-50, -50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-25, -25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(25, -25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-25, 25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(25, 25), Quaternion.identity);

        yield return new WaitForSeconds(0.1f);
        DisableAttack(JumpAttk);
        yield return new WaitForSeconds(0.3f);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(1.6f);
        SetAIFunction(-1, IdleState);
    }

    public IEnumerator ShootEnumState2()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        for (int i = 0; i < 4; i++)
        {
            direction = LookAtTarget(target).normalized;
            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
            angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            SetAnimation("shoot", false);
            PlaySound("demon_spider_attack1");
            yield return new WaitForSeconds(0.8f);
            ShootBullet(1, angle, 20f);
            angle += 15;
            ShootBullet(1, angle, 20f);
            angle -= 35;
            ShootBullet(1, angle, 20f);

            PlaySound("demon_spider_attack1");
            yield return new WaitForSeconds(0.15f);
            ShootBullet(1, angle, 20f);
            angle += 45;
            ShootBullet(1, angle, 20f);
            angle -= 25;
            ShootBullet(1, angle, 20f);
            angle += 15;
            ShootBullet(1, angle, 20f);
            angle -= 35;
            ShootBullet(1, angle, 20f);

            {
                int oldShootPos = moveShootPosPoint;
                moveShootPosPoint = Random.Range(0, 3);
                if (moveShootPosPoint == oldShootPos)
                    moveShootPosPoint = (oldShootPos + 1) % 4;
            }
            SetRandomDirection();
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            SetAnimation("move", true);
            float tim = 0.6f;
            while (tim > 0)
            {
                tim -= Time.deltaTime;
                CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                if (CheckIfCornered(direction * 10))
                    direction *= -1;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            SetAnimation("idle", true);
            yield return new WaitForSeconds(0.1f);
        }
        SetAIFunction(0.5f, RetreatState);
    }
    public IEnumerator ShootEnumState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        direction = LookAtTarget(target).normalized;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        SetAnimation("shoot", false);
        PlaySound("demon_spider_attack1");
        yield return new WaitForSeconds(0.8f);
        ShootBullet(1, angle, 20f);
        angle += 15;
        ShootBullet(1, angle, 20f);
        angle -= 35;
        ShootBullet(1, angle, 20f);

        {
            int oldShootPos = moveShootPosPoint;
            moveShootPosPoint = Random.Range(0, 3);
            if (moveShootPosPoint == oldShootPos)
                moveShootPosPoint = (oldShootPos + 1) % 4;
        }
        SetRandomDirection();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        SetAnimation("move", true);
        float tim = 0.6f;
        while (tim > 0)
        {
            tim -= Time.deltaTime;
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            if (CheckIfCornered(direction * 10))
                direction *= -1;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("idle", true);
        yield return new WaitForSeconds(0.5f);
        SetAIFunction(0.5f, RetreatState);
    }

    public IEnumerator SpawnCaveSpiders()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.2f);

        if (PeastSide)
        {
            if (healthPhase == 0)
            {
                AddCharacter(spawnCharacter[0], jumpPosition, SPAWN_TYPE.JUMP);
            }
            else
            {
                AddCharacter(spawnCharacter[Random.Range(0, 2)], MilbertJumpPosition, SPAWN_TYPE.JUMP);
            }
        }
        else
            AddCharacter(spawnCharacter[Random.Range(0, 2)], MilbertJumpPosition);
        yield return new WaitForSeconds(0.4f);
        SetAIFunction(0.6f, RetreatState);
    }
    public void DelayToRetreatState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAIFunction(0.4f, RetreatState);
        }
    }

    public void DelayToIdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
    }
    public override void AfterDefeat()
    {
        KillAllCharacterLinks();
        StopAllCoroutines();
        PlaySound("demon_spider_defeat");
        s_triggerhandler.trig.JumpToEvent(onDefeatLabel, false);
        base.AfterDefeat();
        defeat = true;
        //Destroy(this);
    }

    public override void RetreatState()
    {
        SetAnimation("move", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.RetreatState();
        if (CheckIfCornered(direction * 10))
            direction *= -1;

        if (AI_timerUp )
        {
            SetAIFunction(-1, IdleState);
        }
    }

    public override void OnHit(BHIII_bullet b)
    {
        base.OnHit(b);
    }

    public override void IdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        if (target != null)
        {
            int random = 0;

            switch (healthPhase) {
                case 0:
                    random = Random.Range(0, 2);
                    break;

                case 1:
                    random = Random.Range(0, 5);
                    break;
            }
            switch (random)
            {
                case 0:
                    StartCoroutineState(ChargeAttack());
                    break;

                case 1:
                    SetRandomDirection();
                    SetAIFunction(0.8f, RetreatState);
                    break;

                case 2:
                    Vector2 tar = LookAtTarget(target);
                    direction = tar;
                    angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                    SetAIFunction(3.1f, AttackState);
                    break;

                case 3:
                    if (characterLinks.Count < 5)
                        StartCoroutineState(SpawnCaveSpiders());
                    else
                    {
                        SetRandomDirection();
                        SetAIFunction(1.5f, RetreatState);
                    }
                    break;

                case 4:
                    StartCoroutineState(JumpAttack());
                    break;
            }
        }
        else {

            if (PeastSide)
                target = GameObject.Find("Player").GetComponent<o_plcharacter>();
        }
    }
}
