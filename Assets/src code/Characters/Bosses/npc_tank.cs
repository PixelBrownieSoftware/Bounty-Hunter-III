using MagnumFoudation;
using UnityEngine;

public class npc_tank : BHIII_character
{
    int shotsUntilCooldown = 6;
    int numOfShots = 20;
    public BHIII_character bioDrone;
    public BoxCollider2D groundAttack;

    /// <summary>
    /// Points towards the player, never moves
    /// Fires a deadly missile
    /// Fires a stream of bullets
    /// Spawns drones
    /// Can only be defeated by shooting later on since it creates a forcefeild
    /// When lower on health, shots take shorter delays
    /// </summary>

    public string[] locations;

    public new void Start()
    {
        SetAttackObject<o_bullet>();
        SetAIFunction(-1, IdleState);
        base.Start();
        Initialize();
    }

    public void ShootAction()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        ShootBullet(1, direction, 6.99f);
        SetRandomDirection();
        SetAIFunction(0.1f, ConstShootDelay);
    }
    public void ConstShootDelay()
    {
        if (AI_timerUp)
        {

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


    public void ShootState() {

        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        direction = LookAtTarget(target);
        ShootBullet(1, direction, 6.99f);
        SetAIFunction(0.9f, ShootStateDelay);
    }

    public void ShootStateDelay()
    {
        if (AI_timerUp)
            SetAIFunction(-1,IdleState);
    }

    public override void AttackState()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));
        base.AttackState();
        if (CheckTargetDistance(target, 135))
        {
        }
    }
    public  void RotAngleShoot()
    {
        Vector2 tar = LookAtTarget(target);
        direction = tar;
        angle += 20;
    }

    public override void OnGround()
    {
        base.OnGround();
        groundAttack.enabled = true;
        SetAIFunction(0.6f, GroundDelay);
    }
    public void GroundDelay()
    {
        base.RetreatState();
        if (AI_timerUp)
        {
            groundAttack.enabled = false;
            SetAIFunction(-1, IdleState);
        }
    }

    public void DelayState()
    {
        base.RetreatState();
        if (AI_timerUp)
        {
            SetAIFunction(1.5f, RetreatState);
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

    public void TripleShoot() {

        direction = LookAtTarget(target);
        ShootBullet(1, direction, 6.99f);
    }

    public void ShootBombs() { 
        
    }

    public override void OnHit(BHIII_bullet b)
    {
        base.OnHit(b);
        SetAIFunction(0.6f, OnHitStateRetalitatePre);
    }

    public void OnHitStateRetalitatePre() {
        if (AI_timerUp)
        {
            SetAIFunction(-1, JumpGround);
        }
    }

    public void JumpGround() {
        Jump(0.5f);
    }

    public override void IdleState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        base.IdleState();

        target = GetClosestTarget<BHIII_character>(990);
        if (target != null)
        {
            int random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    SetAIFunction(-1, ShootState);
                    break;

                case 1:
                    if (characterLinks.Count < 4)
                    {
                        AddCharacter(bioDrone, transform.position);
                    }
                    else
                    {
                        SetAIFunction(-1, ShootState);
                    }
                    break;

                case 2:
                    SetAIFunction(-1, TripleShoot);
                    break;

                case 3:
                    SetAIFunction(-1, ShootBombs);
                    break;
            }
        }
    }
}
