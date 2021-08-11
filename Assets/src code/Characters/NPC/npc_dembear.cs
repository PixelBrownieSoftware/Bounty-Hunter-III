using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// TODO: GROUND ATTACK
/// </summary>
public class npc_dembear : BHIII_character
{
    BoxCollider2D groundAttack;
    int spikeShoot = 3;

    public new void Start()
    {
        terminalSpeedOrigin = 65f;
        base.Start();
    }

    public void ShootSpikeState() {
        //target.transform.position;
        for(int i =0; i < 15; i++)
            ShootBulletAtPosition(1,transform.position + new Vector3(Random.Range(-30,30), Random.Range(-30, 30)), 23f );
        SetAIFunction(0.6f, ShootSpikeDelayState);
    }
    public void ShootSpikeDelayState()
    {
        spikeShoot--;
        if (AI_timerUp) {
            if(spikeShoot == 0)
                SetAIFunction(0.6f, IdleState);
            else
                SetAIFunction(-1, ShootSpikeState);
        }
    }

    public override void IdleState()
    {

        target = GetClosestTarget<BHIII_character>(300);
        if (target != null) {
            int random = UnityEngine.Random.Range(0,1);
            switch (random) 
            {
                case 0:
                    Jump(12.5f);
                    SetAIFunction(1.5f, AttackState);
                    break;

                case 1:
                    SetAIFunction(1.5f, AttackState);
                    break;

                case 2:
                    SetAIFunction(-1, ShootSpikeState);
                    break;
            }
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
    public void JumpState()
    {
        if (_Z_offset <= 0) {
            //ACTIVATE ATTACK OBJECT
            AttackGoStand(0.5f);
            SetAIFunction(1.7f,JumpDelayState);
        }
    }
    public void JumpDelayState()
    {
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
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

    public override void AttackState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        if (CheckTargetDistance(target, 125))
        {
            EnableAttack();
            Dash(0.7f, 4f);
        }
    }
}
