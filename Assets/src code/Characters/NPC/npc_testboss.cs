using MagnumFoudation;
using UnityEngine;

public class npc_testboss : BHIII_character
{
    BHIII_bullet attack;
    float attkdelay = 0;
    new void Start()
    {
        maxHealth = Random.Range(10, 45);
        health = maxHealth;
        faction = "soliders";
        
        control = false;

        terminalspd = 45f;
        SetAttackObject<o_bullet>();
        DisableAttack();

        base.Start();
        Initialize();
        target = targets.Find(x => x.name == "Player");
        attack = attackobject.GetComponent<BHIII_bullet>();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }
    new void Update()
    {
        if (attkdelay > 0)
        {
            attkdelay -= Time.deltaTime;
        }

        base.Update();
    }

    public override void AttackState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        direction = LookAtTarget(target);

        attack.direction = direction;
        if (CheckTargetDistance(target, 150) && attkdelay > 0)
        {
            print("AI FUNCTION ADDED");
            Vector2 tar = LookAtTarget(target);

            ShootBullet(1, tar, 1.2f);
            attkdelay = 0.5f;
        }
    }
}
