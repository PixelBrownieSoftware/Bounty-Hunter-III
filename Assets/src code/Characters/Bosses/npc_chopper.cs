using System.Collections;
using MagnumFoudation;
using UnityEngine;

public class npc_chopper : BHIII_character
{
    int numOfShots = 20;

    public BHIII_character boatPrefab;
    public float maxBombRangeX;
    public float minBombRangeX;
    public float maxBombRangeY;
    public float minBombRangeY;

    public GameObject shootlineStartOBJ;
    public GameObject shootlineEndOBJ;
    public GameObject shootSpreadOBJ;

    public Vector2 shootlineStartPOS;
    public Vector2 shootlineEndPOS;
    public Vector2 shootSpreadPOS;

    float sfxTimer = 2;

    Vector2 bombPoint;
    public enum SHOOT_DIR {
    DOWN,
    LEFT,
    RIGHT
    };
    public SHOOT_DIR dirShoot;

    /// <summary>
    /// Sprays bombs randomly
    /// fires a huge stream of bullets at the player
    /// Summons a few solider boaters
    /// </summary>
    private new void Start()
    {
        healthPhases = new float[2]{
            0.75f,
            0.3f
        };
        base.Start();
        grounded = false;
        SetAIFunction(-1, IdleState);
    }

    new void Update()
    {
        base.Update();
        if (collision == null)
            collision = GetComponent<BoxCollider2D>();
        SetAnimation("move", true);
        if (health > 0) {
            if (sfxTimer <= 0)
            {
                PlaySound("chopper_sound");
                sfxTimer = 0.65f;
            }
            else
                sfxTimer -= Time.deltaTime;
        }
    }

    public override void AfterDefeat()
    {
        StopAllCoroutines();
        StartCoroutineState(ExplosionDeath());
    }

    public override void AttackState()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 235))
        {
            int ran = Random.Range(0,3);
            switch (ran) {
                case 0:

                    StartCoroutineState(ShootGunSpread());
                    break;

                case 1:

                    StartCoroutineState(ShootGunLine());
                    break;

                case 2:
                    StartCoroutineState(ShootGunSpin());
                    break;
            }

        }
    }

    public IEnumerator ShootGunSpin()
    {
        direction = Vector2.up;

        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.1f);
            s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
            angle += 15f;
            ShootBullet(1, angle, 8,5.5f);
        }
        yield return new WaitForSeconds(0.65f);
        SetAIFunction(-1, IdleState);

    }
    public IEnumerator ShootGunLine()
    {
        direction = LookAtTarget(target);
        rendererObj.color = Color.green;
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        yield return new WaitForSeconds(0.5f);
        rendererObj.color = Color.white;
        for (int i = 0; i < 3; i++)
        {
            direction = LookAtTarget(target);
            yield return new WaitForSeconds(0.1f);
            s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
            ShootBullet(1, direction, 8);
        }
        yield return new WaitForSeconds(0.5f);
        SetAIFunction(-1, IdleState);

    }
    public IEnumerator ShootGunSpread()
    {
        direction = Vector2.up;

        yield return new WaitForSeconds(0.5f);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.4f);
            s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
            angle = ReturnAngle(Vector2.up);
            angle += 20;
            ShootBullet(1, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
            angle += 20;
            ShootBullet(1, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
            angle += 20;
            ShootBullet(1, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
            angle += 20;
            ShootBullet(1, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
            angle += 20;
            ShootBullet(1, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
            angle += 20;
            ShootBullet(1, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
            angle += 20;
            ShootBullet(1, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
            angle += 20;
            ShootBullet(1, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
            angle += 20;
            ShootBullet(1, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
        }
        yield return new WaitForSeconds(0.3f);
        SetAIFunction(-1, IdleState);

    }


    public void SpawnBoatCharacter()
    {
        AddCharacter(boatPrefab, transform.position);
        SetAIFunction(0.5f, DelayState);
    }
    public void DelayState()
    {
        if (AI_timerUp) {
            SetAIFunction(-1, IdleState);
        }
    }

    public void ShootAction()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        ShootBullet(1, direction, 0.62f);
        SetRandomDirection();
        SetAIFunction(0.1f, ConstShootDelay);
    }
    public void ConstShootDelay()
    {
        if (AI_timerUp) {

            numOfShots--;

            if (numOfShots > 0)
                SetAIFunction(-1, ShootAction);
            else
            {
                numOfShots = 20;
                SetAIFunction(-1, IdleState);
            }
        }
    }

    public override void RetreatState()
    {
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

        target = GetClosestTarget<BHIII_character>(30000);
        if (target != null)
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            direction = LookAtTarget(target);
            int random = 0;
            random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                    SetAIFunction(3.1f, AttackState);
                    break;

                case 1:
                    SetRandomDirection();
                    SetAIFunction(1.5f, RetreatState);
                    break;

            }
        }
    }
}
