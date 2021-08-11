using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_portalcore : BHIII_character
{

    public GameObject[] shootOBjects;
    public Vector2[] shootPositions;

    public npc_portalguardian guardSpawn;
    public npc_portalguardian guard;

    public int phase { get { return healthPhase; } }

    public BHIII_character[] enemySpawn;
    bool isdead = false;

    /// <summary>
    /// This boss does not move anywhere
    /// It only fires bullets when it's guardian is inactive
    /// When it's lower on health, it fires downward beams in random locations
    /// WWhen it's lowest on health, it can slowly heal the guardian if it's nearby
    /// </summary>

    private new void Start()
    {
        base.Start();
        Initialize();
        healthPhases = new float[3] { 0.7f, 0.7f, 0.55f };
        SetAIFunction(-1, NothingState);
        isInvicible = true;
        shootPositions = TeleportObjectsToPositions(shootOBjects);
        characterLinks = new List<BHIII_character>();
        SetAnimation("closed", false);
    }

    public override void AfterDefeat()
    {
        base.AfterDefeat();
    }
    public new void Update()
    {
        base.Update();
        if (CHARACTER_STATE == CHARACTER_STATES.STATE_DEFEAT)
        {
            if (!isdead)
            {
                isdead = true;
                StopAllCoroutines();
                KillAllCharacterLinks();
            }
        }
    }

    public void SetNonInvincible() {
        SetAIFunction(-1, IdleState);
    }

    IEnumerator PortalActions()
    {
        SetAnimation("opening", false);
        yield return new WaitForSeconds(0.75f);
        isInvicible = false;
        yield return new WaitForSeconds(1.4f);
        rendererObj.color = Color.white; 
        Vector2 p;
        for (int i =0; i < 2; i++)
        {
            p = shootPositions[Random.Range(0, shootPositions.Length)];
            if (healthPhase > 0)
                AddCharacter(enemySpawn[Random.Range(0, 3)], p, SPAWN_TYPE.APPEAR);
            else
                AddCharacter(enemySpawn[0], p, SPAWN_TYPE.APPEAR);
        }
        p = shootPositions[Random.Range(0, shootPositions.Length)];
        AddCharacter(enemySpawn[2], p, SPAWN_TYPE.APPEAR);
        yield return new WaitForSeconds(5.7f);
        if (healthPhase > 0) {

            for (int i = 0; i < 2; i++)
            {
                p = shootPositions[Random.Range(0, shootPositions.Length)];
                AddCharacter(enemySpawn[Random.Range(0, 3)], p, SPAWN_TYPE.APPEAR);
            }
            yield return new WaitForSeconds(7.85f);
        }
        SetAnimation("closing", false);
        isInvicible = true;
        yield return new WaitForSeconds(0.75f);
        guard.health = guard.maxHealth;
        guard.RegenDefeat();
        SetAIFunction(-1, NothingState);
    }

    public override void OnHit(BHIII_bullet b)
    {
        if (isInvicible)
        {
            if (b.isbullet)
                b.direction = -b.direction;
            else
                b.parent.Pushforce(-b.direction, 150);
        } 
        else
            base.OnHit(b);

    }

    public override void IdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;

        StartCoroutineState(PortalActions());
    }
}
