using MagnumFoudation;
using UnityEngine;

public class p_waterBoat : BHIII_character
{
    bool textshow = false;
    o_plcharacter pl;
    public Sprite popSprite;

    new void Start()
    {
        AI = true;
        base.Start();
        pl = GameObject.Find("Player").GetComponent<o_plcharacter>();
        Initialize();
    }

    public override void PlayerControl()
    {
        if (CurrentNode != null)
        {
            switch ((COLLISION_T)CurrentNode.COLTYPE)
            {
                default:
                    break;

                case COLLISION_T.WATER_TILE:
                    break;
            }
        }
        switch (CHARACTER_STATE)
        {
            case CHARACTER_STATES.STATE_IDLE:
                if (ArrowKeyControl())
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                }
                break;

            case CHARACTER_STATES.STATE_MOVING:
                if (!ArrowKeyControl())
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                }
                break;
        }
        base.PlayerControl();
    }


    public override void ArtificialIntelleginceControl()
    {
        base.ArtificialIntelleginceControl();
    }

    new private void FixedUpdate()
    {

        base.FixedUpdate();
    }

    new void Update()
    {
        base.Update();
    }
}
