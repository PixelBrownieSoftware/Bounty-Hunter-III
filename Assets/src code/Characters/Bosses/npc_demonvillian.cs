using MagnumFoudation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demon Beth: She gets turned by the villian, but she kills the villian
/// She throws knife like projectiles and teleports to certain places
/// She shoots circle bullets like the demon from BHI
/// She dashes incredibly fast
/// She can cause earthquakes
/// Is fought alongside 4 biodrones
/// </summary>

public class npc_demonvillian : BHIII_character
{
    public GameObject centreObj;
    Vector2 centrePos;
    public int attackCount = 3;
    public int thunderCount = 3;

    float floatSinThing = 0;
    bool sineFloat = true;

    public delegate void func();
    func functions;

    //For the downward fire attacks
    public GameObject[] firePositionsObjs;
    Vector2[] firePositions;
    bool isflying = false;
    public BHIII_character spawnCharacter;

    public List<npc_biodrone> biodrones = new List<npc_biodrone>();

    private new void Start()
    {
        healthPhases = new float[4] { 0.85f, 0.85f, 0.65f, 0.4f };
        centrePos = centreObj.transform.position;
        firePositions = TeleportObjectsToPositions(firePositionsObjs);
        isHover = true;
        base.Start();
        SetAIFunction(-1, IdleState);
    }

    new void Update()
    {
        //print(healthPhase);
        base.Update();
        if (collision == null)
            collision = GetComponent<BoxCollider2D>();
        if (sineFloat)
        {
            floatSinThing += Time.deltaTime;
            _Z_offset = (Mathf.Sin(floatSinThing) * 10) + 40;
        }

        /*
        if (isflying)
        {
            collision.enabled = false;
            JumpWithoutGround(1.3f, 90);
        }
        */
    }

    IEnumerator ShootDownMeteor() {
        sineFloat = false;
        isflying = true;
        isInvicible = true;
        yield return new WaitForSeconds(0.1f);
        float vel = 0;
        while (_Z_offset < 300)
        {
            vel += Time.deltaTime * 4;
            _Z_offset += vel;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(0.9f);
        SetAnimation("shoot", false);
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < 15; i++)
        {
            ShootBulletAtPosition(1, firePositions[Random.Range(0, firePositions.Length)], "air_to_ground", 0.12f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.35f));
        }
        yield return new WaitForSeconds(1f);
        while (_Z_offset < 10)
        {
            vel += Time.deltaTime * 4;
            _Z_offset -= vel;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _Z_offset = 0;
        sineFloat = true;
        isInvicible = false;
        yield return new WaitForSeconds(1.5f);
        SetAIFunction(-1, IdleState);
    }

    void SpawnThunder() {
        b_thunder b = ll_BHIII.LevEd.SpawnObject<b_thunder>("thunder_strike", firePositions[UnityEngine.Random.Range(0, firePositions.Length)], Quaternion.identity);
        b.SetTimer(1.6f);
    }

    public override void AttackState()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        EnableAttack();
        Dash(0.3f, 3.85f);
    }


    public void BioDroneDelay()
    {

        if (AI_timerUp) {

        }
    }

    public IEnumerator MultipleDash() {
        int dashNum = 3;
        if (healthPhase > 1)
            dashNum = 6;
        for (int i = 0; i < dashNum; i++) {

            SetAnimation("dash_pre", false);
            direction = LookAtTarget(target);
            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            yield return new WaitForSeconds(0.25f);
            SetAnimation("dash", false);
            EnableAttack();
            Dash(0.35f, 4.85f);

            while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            if (healthPhase > 1) {

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
            }
                dashdelay = 0;
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            DisableAttack();
            SetAnimation("hover", true);
            yield return new WaitForSeconds(0.1f);
        }
        DisableAttack();
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("hover", true);
        yield return new WaitForSeconds(1.5f);
        SetAIFunction(0.7f, RetreatState);

    }

    public IEnumerator ShootRingBullets()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("breathe", false);
        yield return new WaitForSeconds(0.5f);
        float tm = 8.4f;
        float ang = 0;
        for (int i = 0; i < 20; i++)
        {
            if (i % 2 == 0) {
                ang += Mathf.PI * 0.1f;
                continue;
            }
            ShootBullet(1, new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)), tm);
            ang += Mathf.PI * 0.1f;
        }
        SetAnimation("breathe", false);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 20; i++)
        {
            if (i % 3 == 0)
            {
                ang += Mathf.PI * 0.1f;
                continue;
            }
            ShootBullet(1, new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)), tm);
            ang += Mathf.PI * 0.1f;
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("hover", true);
        yield return new WaitForSeconds(1.5f);
        SetRandomDirection();
        SetAIFunction(0.9f, RetreatState);
    }

    public void ShootAction()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        ShootBullet(1, direction, 0.12f);
        SetRandomDirection();
        SetAIFunction(0.9f, RetreatState);
    }

    public IEnumerator BreatheFire() {
        yield return new WaitForSeconds(0.5f);

        int posFire = UnityEngine.Random.Range(0, 2);
        Vector2 firePos = firePositions[posFire];
        while (!CheckTargetDistance(firePos, 20))
        {
            direction = LookAtTarget(firePos);
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(0.2f);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("breathe", false);
        yield return new WaitForSeconds(0.5f);
        float ang = 0;
        if (posFire == 1)
            ang = ReturnAngle2(new Vector3(1, 0));
        else
            ang = ReturnAngle2(new Vector3(-1, 0));

        float shootSpeed = 0.15f;
        for (int times = 0; times < 5; times++) {

            for (int i = 0; i < 10; i++)
            {

                ShootBullet("fireball", 2, ang, 5.4f);
                yield return new WaitForSeconds(shootSpeed);
                ang += 25f;
            }
            SetAnimation("breathe", false);
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 10; i++)
            {

                ShootBullet("fireball", 2, ang, 5.4f);
                yield return new WaitForSeconds(shootSpeed);
                ang -= 25f;
            }
            shootSpeed -= 0.05f;
            yield return new WaitForSeconds(0.6f);
        }


        while (!CheckTargetDistance(centrePos, 20))
        {
            direction = LookAtTarget(centrePos);
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        SetAnimation("hover", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(2.25f);
        SetAIFunction(-1, IdleState);
    }

    public void GoToCentre()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        direction = LookAtTarget(centrePos);
        if (CheckTargetDistance(centrePos, 120))
            StartCoroutineState(ShootRingBullets());
    }

    public override void RetreatState()
    {
        SetAnimation("hover", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.RetreatState();
        if (CheckIfCornered(direction * 10))
            SetRandomDirection();
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
    }
    IEnumerator SummonHoverslashes()
    {
        SetAnimation("hover", true);

        rendererObj.color = Color.blue;
        yield return new WaitForSeconds(0.15f);
        while (!CheckTargetDistance(centrePos, 20))
        {
            direction = LookAtTarget(centrePos);
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;

        yield return new WaitForSeconds(0.1f);
        sineFloat = false;
        isflying = true;
        collision.enabled = false;
        SetAnimation("hover", true);
        yield return new WaitForSeconds(0.1f);
        isInvicible = true;
        float vel = 0;
        while (_Z_offset < 300)
        {
            vel += Time.deltaTime * 4;
            _Z_offset += vel;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        vel = 0;
        yield return new WaitForSeconds(0.9f);
        AddCharacter(spawnCharacter, transform.position + new Vector3(50, 0));
        AddCharacter(spawnCharacter, transform.position + new Vector3(-50, 0));
        yield return new WaitForSeconds(11.5f);
        yield return new WaitForSeconds(0.1f);
        while (_Z_offset < 10)
        {
            vel += Time.deltaTime * 4;
            _Z_offset -= vel;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isInvicible = false;
        _Z_offset = 0;
        sineFloat = true;
        collision.enabled = true;
        yield return new WaitForSeconds(0.6f);
        SetAnimation("hover", true);
        SetAIFunction(-1, IdleState);
    }
    public override void AfterDefeat()
    {
        StopAllCoroutines();
        base.AfterDefeat();
    }

    public override void IdleState()
    {
        SetAnimation("hover", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        if (target != null)
        {
            int random = 0;
            switch (healthPhase) {
                case 0:
                    random = UnityEngine.Random.Range(0, 2);
                    break;

                case 1:
                    random = UnityEngine.Random.Range(0, 3);
                    break;

                case 2:
                    random = UnityEngine.Random.Range(0, 5);
                    break;

                case 3:
                    random = UnityEngine.Random.Range(0, 6);
                    break;
            }

            switch (random)
            {
                case 0:
                    StartCoroutineState(MultipleDash());
                    break;

                case 1:
                    SetAIFunction(1.4f, RetreatState);
                    break;

                case 2:
                    SetAIFunction(-1, GoToCentre);
                    break;

                case 3:
                    StartCoroutineState(ShootDownMeteor());
                    break;

                case 4:
                    StartCoroutineState(SummonHoverslashes());
                    break;

                case 5:
                    StartCoroutineState(BreatheFire());
                    break;
            }
        }
    }
}
