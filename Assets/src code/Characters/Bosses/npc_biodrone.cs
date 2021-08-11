using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_biodrone : BHIII_character
{
    BoxCollider2D explosion;

    /// <summary>
    /// Fires a circle of bullets
    /// Heals demon Beth when near her
    /// Creates explosions when commanded by Beth, the explosions happen at their positions
    /// </summary>
    private new void Start()
    {
        base.Start();
        grounded = false;
        SetAIFunction(-1, IdleState);
    }

    public void HealPulseState() {
    }

    new void Update()
    {
        base.Update();
        if (collision == null)
            collision = GetComponent<BoxCollider2D>();
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
            AttackGo(0.1f, tar);
            SetRandomDirection();
            SetAIFunction(0.7f, ShootAction);
        }
    }

    public void ShootAction()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        Vector2 tar = LookAtTarget(target);
        direction = tar;

        SetRandomDirection();
        ShootBullet(1, direction, 0.62f);
        SetRandomDirection();
        ShootBullet(1, direction, 0.62f);
        SetRandomDirection();
        ShootBullet(1, direction, 0.62f);

        SetAIFunction(0.3f, RetreatState);

    }

    public override void RetreatState()
    {
        base.RetreatState();
        if (AI_timerUp)
        {

            SetAIFunction(-1, IdleState);
        }
    }

    public void BlowUp() { 
    
    }

    public override void IdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        target = GetClosestTarget<BHIII_character>(30000);
        if (target != null)
        {
            int random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    Vector2 tar = LookAtTarget(target);
                    direction = tar;
                    angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

                    SetAIFunction(3.1f, AttackState);
                    break;

                case 1:
                    SetAIFunction(2.5f, RetreatState);
                    break;
            }
        }
    }
}
