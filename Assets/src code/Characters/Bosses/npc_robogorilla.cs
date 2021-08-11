using MagnumFoudation;
using System.Collections;
using UnityEngine;

public class npc_robogorilla : BHIII_character
{
    int leapAmount = 2;
    int shootToGroundAmount = 3;
    BHIII_bullet leaAttack;

    public Vector2[] shootPos;
    public GameObject[] shootPosObj;

    public GameObject centreArea;
    Vector2 centreAreaPos;

    public GameObject jumpArea;
    Vector2 jumpAreaPos;
    public BoxCollider2D JumpAttk;
    public npc_solider soliderToSpawn;

    private new void Start()
    {
        jumpAreaPos = jumpArea.transform.position;
        centreAreaPos = centreArea.transform.position;
        shootPos = TeleportObjectsToPositions(shootPosObj);
        SetAttackObject<o_bullet>();
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
        rbody2d = GetComponent<Rigidbody2D>();
    }


    public void ShootAirToGroundPre()
    {
        Vector2 tar = LookAtTarget(centreAreaPos);
        direction = tar;
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckTargetDistance(centreAreaPos, 45))
        {
            SetAIFunction(-1, ShootAirToGround);
        }
    }
    public void ShootAirToGround()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        StartCoroutine(ShootAirToGroundMissile());
    }


    public IEnumerator ShootAirToGroundMissile()
    {
        SetAIFunction(-1, NothingState);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        Jump(2.5f);
        while (gravity > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.position = jumpAreaPos;
        while (!grounded)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(0.4f);
        SetAnimation("shoot", false);
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < 15; i++)
        {
            ShootBulletAtPosition(1, shootPos[Random.Range(0, shootPos.Length)], "air_to_ground", 0.12f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.35f));
        }
        yield return new WaitForSeconds(0.5f);
        Jump(3);
        while (gravity > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.position = centreAreaPos;
        while (!grounded)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        SetAIFunction(0.5f, RetreatState);
    }

    public void ShootAirToGroundDel()
    {
        if (AI_timerUp) {
            shootToGroundAmount--;
            if (shootToGroundAmount > 0)
                SetAIFunction(-1, ShootAirToGround);
            else
            {
                shootToGroundAmount = 3;
                SetAIFunction(-1, IdleState);
            }

        }
    }

    public void ShootAction()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        ShootBullet(1, direction, 0.12f);
        SetRandomDirection();
        SetAIFunction(0.4f, RetreatState);
    }
    public void DelayState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            SetAIFunction(1.5f, RetreatState);
        }
    }

    public override void AfterDash()
    {
        DisableAttack();
        base.AfterDash();
    }
    public IEnumerator SingleDash()
    {
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator MultiDash() {

        for (int i = 0; i < 3; i++) {

            direction = LookAtTarget(target);
            SetAnimation("attack_1", false);
            yield return new WaitForSeconds(0.4f);
            EnableAttack();
            Dash(0.2f, 6f);
            while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            dashdelay = 0;
            DisableAttack();
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            AnimMove();
            yield return new WaitForSeconds(0.15f);
            direction = LookAtTarget(target);
            SetAnimation("attack_2", false);
            yield return new WaitForSeconds(0.4f);
            EnableAttack();
            Dash(0.2f, 8.65f);
            while (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            DisableAttack();
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            AnimMove();
            yield return new WaitForSeconds(0.75f);
            dashdelay = 0;

            SetRandomDirection();
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            AnimMove();
            yield return new WaitForSeconds(0.45f);
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            AnimMove();
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.35f);
        StartCoroutineState(ShootAirToGroundMissile());
    }

    public void BeforeDashState()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

        if (AI_timerUp)
        {
        }
    }

    public override void RetreatState()
    {
        AnimMove();
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

        target = GetClosestTarget<BHIII_character>(99999);
        if (target != null)
        {
            int random = Random.Range(0, 3);
            switch (random)
            {
                case 0:
                    StartCoroutineState(MultiDash());
                    break;

                case 1:
                    if(characterLinks.Count < 3)
                        AddCharacter(soliderToSpawn, centreAreaPos, SPAWN_TYPE.JUMP);
                    else
                        SetAIFunction(0.35f, RetreatState);
                    break;

                case 2:
                    SetRandomDirection();
                    SetAIFunction(0.75f, RetreatState);
                    break;

            }
        }
    }
    public override void AfterDefeat()
    {
        base.AfterDefeat();
        s_triggerhandler.trig.JumpToEvent(onDefeatLabel, false);
    }
}
