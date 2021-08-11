using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class o_collidableobject : s_object
{

    public Vector2 uppercollisionsize { get; set; }
    public Vector2 uppercollision { get; set; }
    public GameObject graphic;
    bool issolid = true;
    public LayerMask layuer; Collider2D cha;
    public List<Vector2> movepositions = new List<Vector2>();
    public int position_index = 0;
    bool forwardDir = true;
    public float debugpo;
    public bool isEnabled = true;

    public Color32 fallingColour = new Color32(0,150, 10, 145);
    public Color32 climbingColour = new Color32(153, 10, 10, 145);
    public Color32 defaultColour = new Color32(255, 0, 255, 145);
    public Color32 fallingOnGroundColour = new Color32(0, 90, 20, 145);
    public Color32 ditchColour = new Color32(30, 20, 40, 145);

    public enum COLLISION_T
    {
        WALL,
        FALLING,
        FALLING_ON_LAND,
        CLIMBING,
        DITCH,
        DAMAGE,
        STAIRS,
        MOVING_PLATFORM,
        NONE
    }
    public COLLISION_T collision_type;

    public COLLISION_T GetCollisionType()
    {
        return collision_type;
    }

    new private void Start()
    {
        positioninworld = transform.position;
        movepositions.Add(positioninworld);
        base.Start();
    }

    private void OnDrawGizmos()
    {
        if (collision != null)
        {
            Gizmos.DrawWireCube(transform.position, collision.size);
        }
        else {
            collision = GetComponent<BoxCollider2D>();
        }
    }

    public void Intialize() {

        SpriteRenderer srp = GetComponent<SpriteRenderer>();
        collision = GetComponent<BoxCollider2D>();
        switch (collision_type)
        {
            case COLLISION_T.CLIMBING:
                collision.isTrigger = true;
                srp.color = climbingColour;
                break;
            case COLLISION_T.DAMAGE:
                collision.isTrigger = true;
                break;
            case COLLISION_T.WALL:
                collision.isTrigger = false;
                srp.color = new Color32(255, 0, 255, 150);
                break;
            case COLLISION_T.MOVING_PLATFORM:
                collision.isTrigger = true;
                srp.color = Color.blue;
                break;
            case COLLISION_T.FALLING:
                collision.isTrigger = true;
                srp.color = fallingColour;
                break;
            case COLLISION_T.DITCH:
                collision.isTrigger = true;
                srp.color = ditchColour;
                break;
            case COLLISION_T.FALLING_ON_LAND:
                collision.isTrigger = true;
                srp.color = fallingOnGroundColour;
                break;
        }
    }

    new void Update ()
    {
        base.Update();
        
        switch (collision_type)
        {
            case COLLISION_T.STAIRS:
                cha = GetCharacter(collision, layuer, "PlayerHitbox");
                if (cha != null)
                {
                    o_character pl = cha.transform.parent.GetComponent<o_character>();
                    if (Input.GetAxisRaw("Horizontal") > 0)
                    {
                        pl.positioninworld.y+= 0.2f;
                    }
                    if (Input.GetAxisRaw("Horizontal") < 0)
                    {
                        pl.positioninworld.y -= 0.2f;
                    }
                }
                break;

            case COLLISION_T.FALLING:

                cha = GetCharacter(collision, layuer);
                if (cha != null)
                {

                    o_character pl = cha.GetComponent<o_character>();
                    pl.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;

                    if (pl.grounded == true)
                        pl.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_FALLING;
                }
                break;
            case COLLISION_T.FALLING_ON_LAND:
                cha = IfTouchingGetCol(collision, typeof(o_character));
                if (cha != null)
                {
                    o_character pl = cha.GetComponent<o_character>();
                    if(pl != null)
                    if (pl.grounded)
                    {
                        pl.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_FALLING;
                    }
                }
                break;

            case COLLISION_T.CLIMBING:
                cha = GetCharacter(collision, layuer, "Player");
                if (cha != null)
                {
                    o_character pl = cha.GetComponent<o_character>();
                    if (pl.grounded)
                    {
                        pl.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_CLIMBING;
                    }
                }
                break;

            case COLLISION_T.DITCH:

                cha = GetCharacter(collision, layuer, "Player");
                if (cha != null)
                {
                    o_character pl = cha.GetComponent<o_character>();
                    if (pl.grounded)
                    {
                        pl.GetComponent<o_plcharacter>().TeleportAfterFall();
                    }
                }
                break;

            case COLLISION_T.MOVING_PLATFORM:
                cha = IfTouchingGetCol(collision, typeof(o_character));
                if (cha != null)
                {
                    o_character pl = cha.GetComponent<o_character>();
                    if (pl != null)
                        if (pl.grounded)
                        {
                            pl.SetTransformPar(this);
                        }
                        else
                        {
                            pl.SetTransformPar(null);
                        }
                }
                debugpo = Vector2.Distance(movepositions[position_index], positioninworld);
                if (debugpo > 2.5f)
                {
                    Vector2 forward = (movepositions[position_index] - (Vector2)positioninworld).normalized;
                    positioninworld += (Vector3)forward;
                    //positioninworld += ;
                    transform.position  = positioninworld;

                }
                else
                {
                    position_index += forwardDir ? 1 : -1;
                    position_index = Mathf.Clamp(position_index, 0, movepositions.Count - 1);
                    
                }


                break;
        }

        //transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = (int)(positioninworld.y/height) - 1;
        //Collider2D collision = transform.GetChild(1).GetComponent<BoxCollider2D>();
        //Bounds bound = collision.bounds;

        //bound.center = new Vector3(positioninworld.x, positioninworld.y, 0) ;

    }

}
*/