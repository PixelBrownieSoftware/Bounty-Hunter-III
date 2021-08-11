using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_demsalamander : BHIII_character
{
    ///Shoots at the player
    ///Spawns Newts
    ///Charges towards the player
    ///Leaves an icky trail that damages the player
    ///You fight 2 of them

    public GameObject[] waterObj;
    Vector2[] waterPos;

    public GameObject gotowaterOBJ;
    Vector2 GotoWater;
    
    public BoxCollider2D SpinAttack;

    public new void Start()
    {
        base.Start();
        GotoWater = gotowaterOBJ.transform.position;
        waterPos = TeleportObjectsToPositions(waterObj);
        SetAIFunction(-1, IdleState);
    }

    public new void Update()
    {
        base.Update();
        if (collision == null)
            collision = GetComponent<BoxCollider2D>();

    }

    public void SpawnNewts() { 
    
    }

    public void PreSpinWalk()
    {
        direction = LookAtTarget(GotoWater);
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;

        if (CheckTargetDistance(GotoWater, 135))
        {
            terminalspd = terminalSpeedOrigin;
            SetAIFunction(6.3f, SpinState);
        }
    }
    public void SpinState()
    {
        //direction = Vector3.RotateTowards(direction, tar, 1 * Time.deltaTime, 0.3f);
        //angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        angle += Mathf.PI * 0.001f;
        terminalspd = terminalSpeedOrigin * 4;
        Vector2 tar = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        direction = tar;
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (AI_timerUp)
        {
            DisableAttack();
            terminalspd = terminalSpeedOrigin;

            if (CheckTargetDistance(GotoWater, 250))
                SetAIFunction(-1, GotoCentre);
            else
                SetAIFunction(-1, IdleState);
        }
    }
    public void GotoCentre()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckTargetDistance(GotoWater, 25))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            tar = LookAtTarget(target);
            direction = tar;
            SetAIFunction(-1, IdleState);
        }
    }
    public override void AttackState()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.AttackState();
        if (CheckTargetDistance(target, 135))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            tar = LookAtTarget(target);
            direction = tar;
            ShootBullet(1, direction, 6.99f);
            SetRandomDirection();
            SetAIFunction(0.4f, ShootDelay);
        }
    }

    void ShootDelay() {
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
    }
    void SubmergeWater()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        if (AI_timerUp)
        {
            direction = LookAtTarget(target);
            ShootBullet(1, direction, 90f);
            SetAIFunction(0.6f, WaterDelay);
        }
    }
    void WaterDelay()
    {
        if (AI_timerUp)
        {
            SetAIFunction(0.6f, TeleportWater);
        }
    }
    void TeleportWater()
    {
        if (AI_timerUp)
        {
            transform.position = waterPos[UnityEngine.Random.Range(0, waterPos.Length)];
            SetAIFunction(0.2f, SubmergeWater);
        }
    }
    void WalkToWater()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        direction = LookAtTarget(GotoWater);
        if (CheckTargetDistance(GotoWater, 60))
        {
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            SetAIFunction(0.3f, TeleportWater);
        }
    }

    public override void AfterDash()
    {
        DisableAttack();
        base.AfterDash();
        string str = currentAIFunction.Method.ToString();
        switch (str)
        {
            case "Void AttackState()":
                SetRandomDirection();
                SetAIFunction(0.7f, RetreatState);
                break;
        }
    }

    public override void RetreatState()
    {
        base.RetreatState();
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }
    }

    public override void IdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        target = GetClosestTarget<BHIII_character>(460);
        if (target != null)
        {
            int random = UnityEngine.Random.Range(0, 4);
            switch (random)
            {
                case 0:
                    Vector2 tar = LookAtTarget(target);
                    direction = tar;
                    angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                    SetAIFunction(3.1f, AttackState);
                    break;

                case 1:
                    SetRandomDirection();
                    SetAIFunction(2.5f, RetreatState);
                    break;

                case 2:
                    SetRandomDirection();
                    EnableAttack();
                    //SetAIFunction(-1, PreSpinWalk);
                    break;

                case 3:
                    SetAIFunction(-1, WalkToWater);
                    break;
            }
        }
    }
}
