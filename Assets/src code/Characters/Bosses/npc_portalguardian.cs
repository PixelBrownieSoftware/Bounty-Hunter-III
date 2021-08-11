using System;
using System.Collections;
using MagnumFoudation;
using UnityEngine;

/// <summary>
/// This guardian has less health than the core
/// But as long as the core is alive, this guaridan will keep on reviving
/// 
/// This guardian has a very fast dash attack
/// Shoots a whole load of bullets
/// Can create floors that hurt the player if they stand on it
/// </summary>

public class npc_portalguardian : BHIII_character
{
    public GameObject[] TeleportObjects;
    public Vector2[] TeleportPositions;
    public GameObject centrePosObj;
    public Vector2 centrePos;

    public b_floorKiller[] floorKills;

    int firebreathAmount = 26;
    const int firebreathAmountMax = 26;

    int teleportAmount = 5;
    const int teleportAmountMax = 5;

    public npc_portalcore coreBoss;

    private new void Start()
    {
        base.Start();
        shadow = transform.Find("shadow").gameObject;
        Initialize();
        SetAIFunction(0.65f, RetreatState);
        centrePos = centrePosObj.transform.position;
        TeleportPositions = TeleportObjectsToPositions(TeleportObjects);
    }

    new void Update()
    {
        base.Update();

        if (collision == null)
            collision = GetComponent<BoxCollider2D>();
    }


    public override void RetreatState()
    {
        SetAnimation("walk_d", true);
        base.RetreatState();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckIfCornered(direction * 10))
            direction *= -1;

        if (AI_timerUp)
        {
            SetAnimation("idle", true);
            SetAIFunction(-1, IdleState);
        }
    }



    public override void AfterDefeat()
    {
        base.AfterDefeat();
        StopAllCoroutines();
        StartCoroutineState(DefeatStateAnim());
    }
    public IEnumerator DefeatStateAnim()
    {
        isInvicible = true;
        SetAnimation("defeat", false);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        rbody2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.95f);
        coreBoss.SetNonInvincible();
    }


    public void RegenDefeat()
    {
        StartCoroutineState(RegenToLife());
    }

    public IEnumerator RegenToLife() {
        SetAnimation("revive", false);
        yield return new WaitForSeconds(0.95f);
        float t = 0;
        while (rendererObj.color != Color.white)
        {
            rendererObj.color = Color.Lerp(new Color(0, 0, 0, 0.15f), Color.white, t);
            t += Time.deltaTime * 15f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isInvicible = false;
        yield return new WaitForSeconds(0.25f);
        SetAIFunction(-1, IdleState);
    }

    public IEnumerator Teleport(Vector2 pos) {

        isInvicible = true;
        collision.enabled = false;
        direction = LookAtTarget(pos);
        shadow.SetActive(false);
        showHealth = false;
        float t = 0;
        PlaySound("magic_2");
        terminalspd = terminalSpeedOrigin * 2.45f;
        while (!CheckTargetDistance(pos, 20))
        {
            rendererObj.color = Color.Lerp(Color.white, Color.clear, t);
            t += Time.deltaTime * 11.5f;
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        t = 0;
        PlaySound("magic_2");
        while (rendererObj.color != Color.white)
        {
            rendererObj.color = Color.Lerp(Color.clear, Color.white, t);
            t += Time.deltaTime * 15f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        terminalspd = terminalSpeedOrigin;
        showHealth = true;
        shadow.SetActive(true);
        collision.enabled = true;
    }

    public IEnumerator CircleBullets()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 tp = TeleportPositions[UnityEngine.Random.Range(0, TeleportPositions.Length)];

            #region Teleport
            yield return StartCoroutine(Teleport(tp));
            #endregion

            SetAnimation("shoot", false);
            yield return new WaitForSeconds(0.55f);
            float tm = 8.4f;
            float ang = 0;
            for (int i2 = 0; i2 < 20; i2++)
            {
                if (i2 % 2 == 0)
                {
                    ang += Mathf.PI * 0.1f;
                    continue;
                }
                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                PlaySound("milbert_projectile");
                ShootBullet(1, new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)), tm);
                ang += Mathf.PI * 0.1f;
            }
            yield return new WaitForSeconds(0.4f);
            SetAnimation("idle", false);
        }

        yield return new WaitForSeconds(1.6f);
        SetAIFunction(1, IdleState);
    }

    public IEnumerator TeleportShootState()
    {
        isInvicible = true;
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        for (int i = 0; i < 3; i++)
        {
            Vector2 tp = TeleportPositions[UnityEngine.Random.Range(0, TeleportPositions.Length)];

            #region Teleport
            yield return StartCoroutine(Teleport(tp));
            #endregion

            SetAnimation("shoot", false);
            yield return new WaitForSeconds(0.3f);
            for (int i2 = 0; i2 < 6; i2++)
            {
                direction = LookAtTarget(target);
                if (coreBoss.phase > 0 && i2 % 2 == 0)
                    direction = LookAtTarget(target.transform.position + (Vector3)(target.direction * 3));
                SetAnimation("shoot_loop", true);

                yield return new WaitForSeconds(0.1f);
                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                PlaySound("milbert_projectile");
                ShootBullet(1, direction, 9.6f);
                yield return new WaitForSeconds(0.05f);
            }
            SetAnimation("idle", false);
        }
        isInvicible = false;
        yield return new WaitForSeconds(2.5f);
        SetAIFunction(1, IdleState);
    }

    public IEnumerator BreatheFire()
    {
        isInvicible = true;
        SetAnimation("idle", false);
        yield return new WaitForSeconds(0.5f);

        int posFire = 0;
        Vector2 firePos = TeleportPositions[posFire];

        #region Teleport
        isInvicible = true;
        collision.enabled = false;
        direction = LookAtTarget(firePos);
        shadow.SetActive(false);
        showHealth = false;
        float t = 0;
        while (!CheckTargetDistance(firePos, 20))
        {
            terminalspd = terminalSpeedOrigin * 2.3f;
            rendererObj.color = Color.Lerp(Color.white, Color.clear, t);
            t += Time.deltaTime * 11.5f;
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        terminalspd = terminalSpeedOrigin;
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        t = 0;
        while (rendererObj.color != Color.white)
        {
            rendererObj.color = Color.Lerp(Color.clear, Color.white, t);
            t += Time.deltaTime * 15f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        showHealth = true;
        shadow.SetActive(true);
        collision.enabled = true;
        isInvicible = false;
        #endregion

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("shoot", false);
        yield return new WaitForSeconds(0.3f);
        float ang = 0;
        if (posFire == 1)
            ang = ReturnAngle2(new Vector3(1, 0));
        else
            ang = ReturnAngle2(new Vector3(-1, 0));

        float shootSpeed = 0.08f;
        PlaySound("breathe_fire");
        for (int i = 0; i < 10; i++)
        {

            ShootBullet("fireball", 2, ang, 5.4f);
            yield return new WaitForSeconds(shootSpeed);
            ang += 25f;
        }
        yield return new WaitForSeconds(0.25f);
        PlaySound("breathe_fire");
        for (int i = 0; i < 10; i++)
        {

            ShootBullet("fireball", 2, ang, 5.4f);
            yield return new WaitForSeconds(shootSpeed);
            ang -= 25f;
        }
        shootSpeed -= 0.05f;
        yield return new WaitForSeconds(0.6f);

        posFire = 1;
        firePos = TeleportPositions[posFire];

        #region Teleport
        isInvicible = true;
        collision.enabled = false;
        direction = LookAtTarget(firePos);
        shadow.SetActive(false);
        showHealth = false;
        t = 0;
        while (!CheckTargetDistance(firePos, 20))
        {
            terminalspd = terminalSpeedOrigin * 2.3f;
            rendererObj.color = Color.Lerp(Color.white, Color.clear, t);
            t += Time.deltaTime * 11.5f;
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        terminalspd = terminalSpeedOrigin;
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        t = 0;
        while (rendererObj.color != Color.white)
        {
            rendererObj.color = Color.Lerp(Color.clear, Color.white, t);
            t += Time.deltaTime * 15f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        showHealth = true;
        shadow.SetActive(true);
        collision.enabled = true;
        isInvicible = false;
        #endregion

        if (posFire == 1)
            ang = ReturnAngle2(new Vector3(1, 0));
        else
            ang = ReturnAngle2(new Vector3(-1, 0));

        SetAnimation("shoot", false);
        PlaySound("breathe_fire");
        for (int i = 0; i < 10; i++)
        {
            ShootBullet("fireball", 2, ang, 5.4f);
            yield return new WaitForSeconds(shootSpeed);
            ang += 25f;
        }
        yield return new WaitForSeconds(0.25f);
        PlaySound("breathe_fire");
        for (int i = 0; i < 10; i++)
        {

            ShootBullet("fireball", 2, ang, 5.4f);
            yield return new WaitForSeconds(shootSpeed);
            ang -= 25f;
        }
        shootSpeed -= 0.05f;
        yield return new WaitForSeconds(0.6f);

        t = 0;
        //Spawn particle effects and playsound
        while (rendererObj.color != Color.clear)
        {
            rendererObj.color = Color.Lerp(Color.blue, Color.clear, t);
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.position = centrePos;
        t = 0;
        while (rendererObj.color != Color.white)
        {
            rendererObj.color = Color.Lerp(new Color(0.5f, 0, 0, 0), Color.white, t);
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isInvicible = false;
        yield return new WaitForSeconds(1.5f);
        SetAIFunction(-1, IdleState);
    }

    public IEnumerator TeleportAttackState()
    {
        isInvicible = true;
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 4; i++)
        {
            Vector2 tp = FurtherestAwayPosition(TeleportPositions);
            //Spawn particle effects and playsound

            yield return new WaitForSeconds(0.25f);
            SetAnimation("idle", true);
            yield return new WaitForSeconds(0.2f);

            float t = 0;
            yield return StartCoroutine(Teleport(tp));

            yield return new WaitForSeconds(0.1f);
            direction = LookAtTarget(target);
            yield return new WaitForSeconds(0.1f);
            SetAnimation("attack_1", false);
            PlaySound("nyarlothotep_attack2");
            yield return new WaitForSeconds(0.6f);
            EnableAttack();
            Dash(0.3f, 7.85f);
            t = 0;
            while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            DisableAttack();
            dashdelay = 0;
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            if (healthPhase == 1)
            {
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                yield return new WaitForSeconds(0.06f);
                SetAnimation("attack_2", false);
                direction = LookAtTarget(target);
                PlaySound("nyarlothotep_attack");
                yield return new WaitForSeconds(0.5f);
                EnableAttack();
                Dash(0.3f, 7.85f);
                t = 0;
                while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                DisableAttack();
                dashdelay = 0;
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
        isInvicible = false;
        SetAnimation("idle", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(1.7f);
        SetAIFunction(-1, IdleState);
    }

    public IEnumerator LightFastDash() {

        isInvicible = true;
        int dashNum = 3;
        if (healthPhase > 1)
            dashNum = 6;
        for (int i = 0; i < dashNum; i++)
        {

            SetAnimation("attack_1", false);
            direction = LookAtTarget(target);
            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            PlaySound("nyarlothotep_attack2");
            yield return new WaitForSeconds(0.45f);
            Dash(0.35f, 4.85f);
            EnableAttack();

            while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            float tm = 8.4f;
            float ang = 0;
            for (int i2 = 0; i2 < 20; i2++)
            {
                if (i2 % 2 == 0)
                {
                    ang += Mathf.PI * 0.1f;
                    continue;
                }
                ShootBullet(1, new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)), tm);
                ang += Mathf.PI * 0.1f;
            }
            yield return new WaitForSeconds(0.5f);
            dashdelay = 0;
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            DisableAttack();
            SetAnimation("idle", true);
            yield return new WaitForSeconds(0.1f);
        }
        DisableAttack();
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("idle", true);
        isInvicible = false;
        yield return new WaitForSeconds(1.5f);
        SetAIFunction(0.7f, RetreatState);
    }

    public override void IdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();
        SetAnimation("idle", true);

        target = GetClosestTarget<BHIII_character>(300);
        if (target != null)
        {
            int random = 0;
            switch (coreBoss.phase) {
                case 0:
                    random = UnityEngine.Random.Range(0, 3);
                    break;
                case 1:
                    random = UnityEngine.Random.Range(0, 5);
                    break;
                case 2:
                    random = UnityEngine.Random.Range(0, 6);
                    break;

            }
            switch (random)
            {
                case 0:
                    SetAIFunction(1.5f, RetreatState);
                    break;

                case 1:
                    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                    direction = LookAtTarget(target);
                    StartCoroutineState(LightFastDash());
                    break;

                case 2:
                    StartCoroutineState(TeleportShootState());
                    break;

                case 3:
                    StartCoroutineState(TeleportAttackState());
                    break;

                case 4:
                    StartCoroutineState(BreatheFire());
                    break;

                case 5:
                    StartCoroutineState(CircleBullets());
                    break;
            }
        }
    }
}
