using System.Collections;
using MagnumFoudation;
using UnityEngine;

public class npc_roboknight : BHIII_character
{
    BoxCollider2D groundAttack;
    int shootTimes = 3;
    public GameObject[] teleportObjects;
    public Vector2[] teleportPositions;
    int teleportNum = 4;

    bool dashComplete = false;

    public npc_drone drone;
    public LineRenderer linerend;
    float lazerTimer;

    /// <summary>
    /// Next to final boss
    /// Teleports to random places and then shoots
    /// Does a 3 string attack
    /// Does a large range ground attack started by a jump
    /// </summary>
    private new void Start()
    {
        healthPhases = new float[2] {0.5f, 0.5f};
        base.Start();
        SetAIFunction(-1, IdleState);
        teleportPositions = TeleportObjectsToPositions(teleportObjects);
    }

    new void Update()
    {
        print(healthPhase);
        base.Update();
        if (collision == null)
            collision = GetComponent<BoxCollider2D>();
    }

    public IEnumerator TeleportShootStateRapid()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAnimation("load_gun", false);
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < 6; i++)
        {
            Vector2 tp = FurtherestAwayPosition(teleportPositions);
            float t = 0;
            LaserGunOn(tp);
            yield return new WaitForSeconds(0.1f);
            LaserGunOff();

            rendererObj.color = Color.blue;
            yield return new WaitForSeconds(0.15f);
            rendererObj.color = Color.clear;
            transform.position = tp;
            SetAnimation("shoot_idle", false);
            yield return new WaitForSeconds(0.15f);
            rendererObj.color = Color.blue;
            yield return new WaitForSeconds(0.15f);
            rendererObj.color = Color.white;

            yield return new WaitForSeconds(0.1f);
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            Vector2 tar = LookAtTarget(target);
            direction = tar;
            yield return new WaitForSeconds(0.05f);
            SetAnimation("shoot", false);
            ShootBullet(1, direction, 9.6f);
            yield return new WaitForSeconds(0.05f);
            if (healthPhase == 1 && i % 2 == 0)
            {
                direction = LookAtTarget(target);
                yield return new WaitForSeconds(0.2f);
                SetAnimation("shoot", false);
                angle = ReturnAngle2(direction);
                ShootBullet(1, angle, 1.4f);
                ShootBullet(1, angle + 20, 1.4f);
                ShootBullet(1, angle - 20, 1.4f);
                yield return new WaitForSeconds(0.1f);

            }
            yield return new WaitForSeconds(0.2f);
        }
        SetAnimation("idle_d", true);
        yield return new WaitForSeconds(0.5f);
        SetAIFunction(1, IdleState);
    }

    public IEnumerator TeleportShootState() {

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.7f);
        SetAnimation("load_gun", false);
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < 3; i++) {

            Vector2 tp = FurtherestAwayPosition(teleportPositions);
            LaserGunOn(tp);
            if (healthPhase == 1)
            {
                yield return new WaitForSeconds(0.25f);
            } else {

                yield return new WaitForSeconds(0.5f);
            }
            LaserGunOff();

            rendererObj.color = Color.blue;
            yield return new WaitForSeconds(0.15f);
            rendererObj.color = Color.clear;
            transform.position = tp;
            SetAnimation("idle", true);
            yield return new WaitForSeconds(0.15f);
            rendererObj.color = Color.blue;
            yield return new WaitForSeconds(0.15f);
            rendererObj.color = Color.white;

            SetAnimation("shoot", true);
            yield return new WaitForSeconds(0.35f);
            for (int i2 = 0; i2 < 8; i2++)
            {
                Vector2 tar = LookAtTarget(target);
                direction = tar;
                yield return new WaitForSeconds(0.08f);
                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                ShootBullet(1, direction, 9.6f);
                yield return new WaitForSeconds(0.08f);
            }
            SetAnimation("shoot_idle", false);
            yield return new WaitForSeconds(0.1f);
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            yield return new WaitForSeconds(0.1f);
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.9f);
        SetAIFunction(1, IdleState);
    }
    public IEnumerator TeleportAttackState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 4; i++)
        {
            Vector2 tp = FurtherestAwayPosition(teleportPositions);
            //Spawn particle effects and playsound
            float t = 0;
            LaserGunOn(tp);
            yield return new WaitForSeconds(0.1f);
            LaserGunOff();

            rendererObj.color = Color.blue;
            yield return new WaitForSeconds(0.15f);
            rendererObj.color = Color.clear;
            transform.position = tp;
            SetAnimation("idle", true);
            yield return new WaitForSeconds(0.15f);
            rendererObj.color = Color.blue;
            yield return new WaitForSeconds(0.15f);
            rendererObj.color = Color.white;

            direction = LookAtTarget(target);
            yield return new WaitForSeconds(0.05f);
            while (rendererObj.color != Color.white)
            {
                rendererObj.color = Color.Lerp(new Color(0.5f, 0, 0, 0), Color.white, t);
                t += Time.deltaTime * 5.5f;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            yield return new WaitForSeconds(0.1f);
            SetAnimation("attack_1", false);
            yield return new WaitForSeconds(0.11f);
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
                yield return new WaitForSeconds(0.11f);
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
            } else
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
        SetAnimation("idle", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.7f);
        SetAIFunction(-1, IdleState);
    }

    public void JumpAttack() {
        Jump(4);
    }

    public override void OnGround()
    {
        base.OnGround();
    }

    public void ShootAction()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        StartCoroutine(ShootGun());
    }

    public IEnumerator ShootGun() {

        SetAnimation("load_gun", false);
        yield return new WaitForSeconds(2f);
        SetAnimation("shoot", false);
        ShootBullet(1, direction, 6f, 15);
        yield return new WaitForSeconds(0.2f);
        ShootBullet(1, direction, 6f, 15);
        yield return new WaitForSeconds(0.1f);
        ShootBullet(1, direction, 6f, 15);
        yield return new WaitForSeconds(0.05f);
        SetRandomDirection();
        SetAIFunction(0.4f, RetreatState);
    }

    public void LaserGunOn(Vector2 target)
    {
        linerend.enabled = true;
        linerend.SetPosition(0, transform.position + new Vector3(0, 0, -2));
        linerend.SetPosition(1, (Vector3)target + new Vector3(0, 0, -2));
    }
    public void LaserGunOn()
    {
        linerend.enabled = true;
        linerend.SetPosition(0, transform.position + new Vector3(0, 0, -2));
        linerend.SetPosition(1, target.transform.position + new Vector3(0, 0, -2));
    }
    public void LaserGunOff()
    {
        linerend.enabled = false;
    }

    public void DashToRandomPoint() {
        SetRandomDirection();
        Dash(0.6f, 0.5f);
    }

    public override void AfterDefeat()
    {
        SetAnimation("idle_d", true);
        SetAIFunction(-1, NothingState);
        StopAllCoroutines();
        KillAllCharacterLinks();
        StartCoroutineState(ExplosionDeath());
    }

    IEnumerator SummonDrones()
    {
        SetAnimation("idle", true);

        rendererObj.color = Color.blue;
        yield return new WaitForSeconds(0.15f);
        rendererObj.color = Color.clear;
        transform.position = teleportPositions[0];
        SetAnimation("idle", true);
        yield return new WaitForSeconds(0.15f);
        rendererObj.color = Color.blue;
        yield return new WaitForSeconds(0.15f);
        rendererObj.color = Color.white;

        yield return new WaitForSeconds(0.6f);
        AddCharacter(drone, transform.position + new Vector3(50, 50));
        AddCharacter(drone, transform.position + new Vector3(-50, 50));
        AddCharacter(drone, transform.position + new Vector3(-50, -50));
        AddCharacter(drone, transform.position + new Vector3(50, -50));
        SetAnimation("idle", true);
        yield return new WaitForSeconds(3.5f);
        SetAIFunction(-1, IdleState);
    }

    public override void IdleState()
    {
        SetAnimation("idle_d", true);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        if (target != null)
        {
            int random = Random.Range(0,4);
            switch (random)
            {

                case 0:
                    StartCoroutineState(TeleportShootState());
                    break;

                case 1:
                    StartCoroutineState(TeleportShootStateRapid());
                    break;

                case 2:
                    StartCoroutineState(TeleportAttackState());
                    break;

                case 3:
                    if (characterLinks.Count < 4)
                        StartCoroutineState(SummonDrones());
                    else
                        StartCoroutineState(TeleportAttackState());
                    break;

            }
        }
        else {

            target = GameObject.Find("Player").GetComponent<o_plcharacter>();
        }
    }
}
