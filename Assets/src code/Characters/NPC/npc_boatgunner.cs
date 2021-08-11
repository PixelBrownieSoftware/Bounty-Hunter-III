using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_boatgunner : BHIII_character
{
    float waterGunShootDelay = 0;
    int ammo = 25;
    Vector2 direction2;

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


    public void ShootAction()
    {
        Vector2 tar = LookAtTarget(target);
        ArrowKeyControl();
        direction = Vector3.RotateTowards(direction, tar, 1 * Time.deltaTime, 0.8f);
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

        if (waterGunShootDelay <= 0)
        {
            if(CheckTargetDistance(target, 400))
                ShootBullet(1, tar, 0.92f);
            ammo--;
        }
        else {
            waterGunShootDelay -= Time.deltaTime;
        }
        if (ammo == 0 || AI_timerUp)
        {
            ammo = 25;
            SetAIFunction(0.35f, DelayState);
        }
    }

    public void DelayState()
    {

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            direction2 = GetRandomDirection();
            SetAIFunction(1.5f, RetreatState);
        }
    }

    public override void RetreatState()
    {
        base.RetreatState();
        direction = Vector3.RotateTowards(direction, direction2, 1 * Time.deltaTime, 0.8f);
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

        if (AI_timerUp)
        {

            SetAIFunction(-1, IdleState);
        }
    }

    public override void IdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        target = GetClosestTarget<BHIII_character>(4000);
        if (target != null)
        {
            Vector2 tar = LookAtTarget(target);
            direction = Vector3.RotateTowards(direction, tar, 1 * Time.deltaTime, 0.8f);
            angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

            int random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    SetAIFunction(3.7f, ShootAction);
                    break;

                case 1:
                    direction2 = GetRandomDirection();
                    SetAIFunction(2.5f, RetreatState);
                    break;
            }
        }
    }
}
