#region CHARACTER CODE
/*
ARCHIVED ON 15/11/2020
public void JumpToAILabel(string lab)
{
    int ptr = actionAI.FindIndex(x => x.string0 == lab && x.action == s_AIAction.ACTION_TYPE.LABEL);
    AIPointer = AIPointerStart = ptr;
}
void CheckNextAIAction()
{
    if (AIPointer + 1 < actionAI.Count)
    {
        s_AIAction ai = actionAI[AIPointer + 1];
        if (target != null)
        {
            switch (ai.action)
            {
                case s_AIAction.ACTION_TYPE.DASH:
                case s_AIAction.ACTION_TYPE.SHOOT:
                    if (!ai.boolean0)
                    {
                        if (CheckTargetDistance(target, ai.float1))
                        {
                            AIPointer++;
                            ai_timer = ai.float0;
                        }
                    }
                    else
                    {
                        AIPointer++;
                        //ai_timer = ai.float0;
                    }
                    break;

                case s_AIAction.ACTION_TYPE.WAIT:
                    ai_timer = ai.float0;
                    AIPointer++;
                    break;

                case s_AIAction.ACTION_TYPE.MOVE_BACK:
                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                    offsetDirection = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                    direction = -(LookAtTarget(target) + offsetDirection).normalized;
                    ai_timer = ai.float0;
                    AIPointer++;
                    break;

                default:
                    AIPointer++;
                    break;
            }
        }
        else
        {
            target = GetClosestTarget<BHIII_character>(ai.float0);
        }
    }
}
*/
/*

public void RunAIFunction(s_AIAction act)
{
    //AIPointer++;
    switch (act.action)
    {
        case s_AIAction.ACTION_TYPE.LABEL:
            //AIPointer++;
            CheckNextAIAction();
            break;

        case s_AIAction.ACTION_TYPE.END:
            AIPointer = AIPointerStart;
            break;

        case s_AIAction.ACTION_TYPE.JUMP:
            JumpToAILabel(act.string0);
            break;

        case s_AIAction.ACTION_TYPE.MOVE_FORWARD:
            if (target == null)
                target = GetClosestTarget<BHIII_character>(act.float0);
            else
            {
                direction = LookAtTarget(target);

                if (CheckTargetDistance(target, act.float0))
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                }
                else
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                }
                CheckNextAIAction();
            }
            break;

        case s_AIAction.ACTION_TYPE.WAIT:
            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            ai_timer = ai_timer - Time.deltaTime;
            if (ai_timer <= 0)
                CheckNextAIAction();
            break;

        case s_AIAction.ACTION_TYPE.MOVE_BACK:
            if (target == null)
            {
                target = GetClosestTarget<BHIII_character>(act.float0);
                CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            }
            else
            {
                if (!act.boolean0)
                {
                    if (CheckTargetDistance(target, act.float0))
                    {
                        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                    }
                    else
                    {
                        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                    }
                }
                else
                {
                    ai_timer = ai_timer - Time.deltaTime;

                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                    if (CheckIfCornered(direction))
                        direction = direction * -1;

                    if (ai_timer <= 0)
                    {
                        CheckNextAIAction();
                    }
                }
            }
            break;

        case s_AIAction.ACTION_TYPE.SHOOT:

            if (target == null)
            {
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                ai_timer = act.float0;
                target = GetClosestTarget<BHIII_character>(act.float1);
            }
            else
            {
                direction = LookAtTarget(target);

                ai_timer = ai_timer - Time.deltaTime;

                if (ai_timer <= 0)
                {
                    ShootBullet(1, direction, 0.5f);
                    CheckNextAIAction();
                }
            }
            break;

        case s_AIAction.ACTION_TYPE.DASH:
            ///TODO:
            ///IF THIS DASH IS DONE OR CANCELLED MOVE TO THE NEXT STATE
            EnableAttack();
            Dash(act.float0, 3.5f);
            break;

        case s_AIAction.ACTION_TYPE.WHEN:
            switch (act.int0)
            {
                case 0://If local variable is a certain number (can set a random range)
                    if (localV1 == act.int1)
                        JumpToAILabel(act.string0);
                    else
                        AIPointer++;
                    break;

                case 1://If health is a certain amount (can set a random range)
                    if (health / maxHealth < act.float0)
                        JumpToAILabel(act.string0);
                    break;
                case 2://If target is present
                    target = GetClosestTarget<BHIII_character>(act.float0);
                    if (target != null)
                        JumpToAILabel(act.string0);
                    break;
            }
            break;

        case s_AIAction.ACTION_TYPE.RANDOM_NUM:
            localV1 = Random.Range(act.int0, act.int1 + 1);
            AIPointer++;
            break;

        case s_AIAction.ACTION_TYPE.CUSTOM_FUNCTION:
            System.Type ty = GetType();
            object[] ohj = new object[3];
            MethodInfo mi = ty.GetMethod(act.string0);
            mi.Invoke(this, ohj);
            break;

    }
}
void DrawRaysLitHor(ref Bounds bound)
{
    float rayspacing = bound.size.y / raycount;
    int i = 0;
    for (i = 0; i < raycount; i++)
    {
        float signedvec = Mathf.Sign(velocity.x);
        float rayleng = Mathf.Abs(signedvec );

        Vector2 raypos = (signedvec != -1 ? new Vector2(bound.max.x, bound.min.y) : new Vector2(bound.min.x, bound.min.y ));
        raypos += Vector2.up * (rayspacing * i );
        RaycastHit2D hit = Physics2D.Raycast(raypos, Vector2.right * signedvec, rayleng, 256);
        if (hit.transform == null)
            continue;
        o_collidableobject col = hit.transform.GetComponent<o_collidableobject>();
        Debug.DrawRay(raypos, Vector2.right * signedvec, Color.blue);

        if (hit)
        {
            if (col.collision_type == o_collidableobject.COLLISION_T.WALL)
            {
                if (!col.isEnabled)
                    continue;
                velocity.x = -signedvec;
            }
        }
    }
}
void DrawRaysLitVer(ref Bounds bound)
{
    float rayspacing = bound.size.x / raycount;
    int i = 0;
    for (i = 0; i < raycount; i++)
    {
        float signedvec = Mathf.Sign(velocity.y);
        float rayleng = Mathf.Abs(signedvec );

        Vector2 raypos = (signedvec != -1 ? new Vector2(bound.min.x, bound.max.y) : new Vector2(bound.min.x, bound.min.y));
        raypos += Vector2.right * (rayspacing * i);
        RaycastHit2D hit = Physics2D.Raycast(raypos, Vector2.up * signedvec, rayleng, 256);
        if (hit.transform == null)
            continue;
        o_collidableobject col = hit.transform.GetComponent<o_collidableobject>();
        Debug.DrawRay(raypos, Vector2.up * signedvec, Color.blue);

        if (hit)
        {
            if (col.collision_type == o_collidableobject.COLLISION_T.WALL)
            {
                if (!col.isEnabled)
                    continue;
                velocity.y = -signedvec;
            }
        }
    }
}
*/
/*
void ParseTextFile() {
    string AItxt = aiTextFile.text;
    string[] parse = AItxt.Split('\n');
    foreach (string str in parse) {
        string parsedTxt = "";
        for (int i = 0; i < str.Length; i++) {
            parsedTxt += str[i];
            switch (str)
            {
                case "":
                    break;


            }
        }

    }

}

void Move()
{
    /*
    switch (CHARACTER_STATE)
    {
        case CHARACTER_STATES.STATE_NOTHING:
            if (crashTimer > 0)
                crashTimer -= Time.deltaTime;
            if (crashTimer != -1)
            {
                if (crashTimer <= 0)
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                }
            }
            break;

        case CHARACTER_STATES.STATE_IDLE:
            if (rbody != null)
                rbody.velocity *= 0.85f;
            break;

        case CHARACTER_STATES.STATE_MOVING:
            if (rbody != null)
                rbody.velocity = direction * 25;
            //transform.Translate(velocity * Time.deltaTime);
            break;

        case CHARACTER_STATES.STATE_FALLING:
            transform.position -= new Vector3(0, 1, 0);
            if (transform.position.y <= fallposy)
            {
                CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            }
            break;

        case CHARACTER_STATES.STATE_HURT:
            if (damage_timer <= 0)
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            break;

        case CHARACTER_STATES.STATE_DASHING:
            if (dashdelay <= 0)
            {
                AfterDash();
            }
            else
            {
                Collider2D c = Physics2D.OverlapBox((Vector2)transform.position + (direction * 10), collision.size, 0);
                if (c != null)
                {
                    if (c.name == "Collision") {
                        CrashOnDash();
                        dashdelay = 0;
                    }
                }
                if (dashdelay < dashdelayStart / 2)
                    if (!AI)
                        if (!Input.GetKey(KeyCode.LeftShift))
                            dashdelay = 0;
                dashdelay -= Time.deltaTime;
            }
            break;

        case CHARACTER_STATES.STATE_DEFEAT:

            rendererObj.color = Color.black;
            break;
    }
    if (rbody == null)
    {
        positioninworld += (Vector3)new Vector2(velocity.x, velocity.y) * Time.deltaTime;
        if (IS_KINEMATIC)
        {
            collision.isTrigger = true;
            COLLISIONDET();
        }
        else {
            collision.isTrigger = false;
        }

        Vector2 composite_pos = positioninworld;

        if (parentTrans != null)
            composite_pos = parentTrans.positioninworld;
        positioninworld = composite_pos;
        transform.position = new Vector2(composite_pos.x, composite_pos.y + Z_offset);
    }
    else {
        transform.Translate((Vector3)rbody.velocity * Time.deltaTime);

        if (SpriteObj != null)
        {
            SpriteObj.transform.position = new Vector3(transform.position.x, transform.position.y + Z_offset);
        }
    }
    if (IS_KINEMATIC)
    {
        COLLISIONDET();
    }
}
*/
/*
 * Process AI stuff
if (actionAI != null)
{
    if (actionAI.Count > 0)
    {
        switch (CHARACTER_STATE)
        {
            default:
                if (AIPointer > actionAI.Count)
                    AIPointer = actionAI.Count;
                RunAIFunction(actionAI[AIPointer]);
                break;

            case CHARACTER_STATES.STATE_DASHING:
            case CHARACTER_STATES.STATE_HURT:
            case CHARACTER_STATES.STATE_NOTHING:
                break;

        }
    }
}
*/
/*
similar code to BHII
c = IfTouchingGetCol(collision, typeof(o_maptransition));
if (c != null)
{
    o_maptransition tr = c.GetComponent<o_maptransition>();
    if (tr != null)
        tr.Transition();
}
*/
#endregion

#region LEVEL_EDIT_CODE
/*
 
ADDED ON 09/09/2018
 
    public override void OnInspectorGUI()
    {
        s_leveledit lev = (s_leveledit)target;

        if (lev != null)
        {
            if (lev.maps != null)
            {
                if (lev.maps.Count >= 2)
                {
                    levelsel = (int)GUILayout.HorizontalSlider(lev.current_area, 0, lev.maps.Count - 1);

                    if (GUILayout.Button("Select level"))
                        lev.current_area = levelsel;

                    //if (lev.maps[lev.current_area] != null)
                        //EditorGUILayout.LabelField(lev.maps[lev.current_area].name);
                }
            }
        }
        foreach (s_map ma in lev.maps)
        {
            EditorGUILayout.LabelField(ma.name);

        }

        
        lev.nam = GUILayout.TextArea(lev.nam);


        if (GUILayout.Button("Load Level"))
        {
            lev.current_map = lev.maps[lev.current_area];
            //lev.JsonToObj();
        }
        if (GUILayout.Button("New Level"))
        {
            lev.NewMap();
        }
        if (GUILayout.Button("Save Level"))
        {
            lev.SaveMap("4");
        }

        base.OnInspectorGUI();
        Repaint();
    }
    
    private void OnSceneGUI()
    {
        s_leveledit lev = (s_leveledit)target;
        if (Event.current.isKey)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.A:

                    lev.SpawnObj(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    break;
            }
        }
        
    }
    
    public void AssignParent(string na, GameObject ga)
    {
        ga.transform.SetParent(GameObject.Find(na).transform);
    }

    Vector3 Snap(Vector3 mousepos)
    {
        return new Vector3(Mathf.Floor(mousepos.x / 20) * 20, Mathf.Floor(mousepos.y / 20) * 20);
    }
 
 */
#endregion

#region REMOVED_AABB_COLLISION
/*
ADDED ON 10/08/2018

This was in the update function

 Bounds thing = new Bounds();
        if (Physics2D.OverlapBox(transform.position, collision.size, 0, 256) != null)
        {
            thing = Physics2D.OverlapBox(transform.position, collision.size, 0, 256).bounds;
            thing.Expand(skinwdth * -2);
        }

        if (thing.min != (Vector3)new Vector2(0, 0) && thing.max != (Vector3)new Vector2(0, 0))
        {
            AABB_COLLISION(thing);
            
            bool collisioncheckXy = this.collision.bounds.min.x * 1.1f > thing.max.x && this.collision.bounds.max.x / 1.1f < thing.min.x;
            if (!collisioncheckXy) 
            {
                if (velocity.x != 0)
                    AABB_CollisionX(thing);
                print(collisioncheckXy + "Y");
            }
            bool collisioncheckYx = this.collision.bounds.min.y * 1.1f > thing.max.y && this.collision.bounds.max.y / 1.1f < thing.min.y;
            if (!collisioncheckYx)
            {
                if (velocity.y != 0) 
                    AABB_CollisionY(thing);
            }




void AABB_CollisionX(Bounds collision)
{

    float directionVecX = Mathf.Sign(velocity.x);
    bool collisioncheckYx = this.collision.bounds.min.y * 1.1f > collision.max.y && this.collision.bounds.max.y / 1.1f < collision.min.y;
    print(collisioncheckYx + "X");
    bool collisioncheckX = directionVecX != -1 ? this.collision.bounds.min.x < collision.max.x : this.collision.bounds.max.x > collision.min.x;

    if (collisioncheckX)
    {
        if (directionVecX == -1)
            velocity.x = Mathf.Sign(this.collision.bounds.min.x - collision.max.x) * directionVecX;

        if (directionVecX == 1)
            velocity.x = Mathf.Sign(collision.min.x - this.collision.bounds.max.x) * directionVecX;

    }

}

void AABB_CollisionY(Bounds collision)
{
    float directionVecY = Mathf.Sign(velocity.y);

    bool collisioncheckY = directionVecY != -1 ? this.collision.bounds.min.y < collision.max.y : this.collision.bounds.max.y > collision.min.y;

    if (collisioncheckY)
    {
        if (directionVecY == -1)
            velocity.y = Mathf.Sign(collision.max.y - this.collision.bounds.min.y) * -directionVecY;

        if (directionVecY == 1)
            velocity.y = Mathf.Sign(this.collision.bounds.max.y - collision.min.y) * -directionVecY;
    }
}
*/

#endregion

#region OLD_COLLISION_SYSTEM
/*
ADDED ON 10/08/2018
This was meant to be the collision system



                if (objectcol != null)
                {
                    foreach (RaycastHit2D sur in objectcol)
                    {
                        //print(sur.collider.gameObject.name);

                        if (sur.collider.transform.parent.GetComponent<o_collidableobject>())
                        {
                            o_collidableobject collidable = sur.collider.transform.parent.GetComponent<o_collidableobject>();


                            if (grounded && zpos < collidable.zpos )
                            {
                                gravity = 1.2f;
                            }
                        }
                    }

                }


    public class ray_z
    {
        private o_character character;
        public float zpos;
        Vector2 position;
        bool is_up;
        LayerMask layer;
        public ray_z(o_character character, bool ceiling)
        {
            this.character = character;
            layer = character.lay;
            is_up = ceiling;
            if (ceiling)
            {
                zpos = character.zpos + character.height;
            }
            else
            {
                zpos = character.zpos;
            }
        }

        public rayhit istouching { get
            {
                if (is_up)
                    zpos = character.zpos + character.height;
                else
                    zpos = character.zpos;

                float raypos = character.collision.size.x / 5;

                float raypos_x = character.collision.size.x / 5;
                float raypos_y = character.collision.size.y / 5;

                for (int y = 1; y != 5; y++)
                {
                    for (int x = 1; x != 5; x++)
                    {

                        Vector2 position = new Vector2(character.collision.bounds.min.x + (raypos_x * x),
                                    character.collision.bounds.min.y + (raypos_y * y) - zpos);
                        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(position, new Vector2(1, 1), 0, layer);
                        if (collider2Ds.Length > 0)
                        {
                            foreach (Collider2D c in collider2Ds)
                            {
                                o_collidableobject collidable = c.transform.parent.GetComponent<o_collidableobject>();
                                if (collidable.zpos + collidable.height > zpos && collidable.zpos < zpos)
                                {
                                    return new rayhit(true, zpos - collidable.zpos);
                                }
                            }
                        }
                    }
                }
                return new rayhit(false, 0);
            }
        }
    }


////////////STUFF FOR THE GRAVITY OF THE Z AXIS////////////////
        if ( zpos < 0.1f)
        {
            if (!grounded )
            {
                gravity = 0;
                grounded = true;
            }
        }
        else
        {
            grounded = false;
            gravity -= Time.deltaTime * wldgravity; floorz = 0;
        }

///////////////DRAWING RAYS////////////////////////////////

void DrawRaysLitHor(ref Bounds bound)
{
    float rayspacing = bound.size.y / raycount;
    int i = 0;
    for (i = 0; i < raycount; i++)
    {
        float signedvec = Mathf.Sign(velocity.x);
        float ray_zpos = this.zpos + 1 + (rayspacing * i);
        float rayleng = Mathf.Abs(signedvec);

        Vector2 raypos = (signedvec != -1 ? new Vector2(bound.max.x, bound.min.y) : new Vector2(bound.min.x, bound.min.y));
        raypos += Vector2.up * (rayspacing * i);
        RaycastHit2D hit = Physics2D.Raycast(raypos, Vector2.right * signedvec, rayleng, 256);

        Debug.DrawRay(raypos, Vector2.right * signedvec, Color.blue);

        if (hit)
        {
            o_collidableobject collidedthing = hit.collider.gameObject.transform.parent.GetComponent<o_collidableobject>();
            if (Mathf.Floor(collidedthing.zpos + collidedthing.height) > ray_zpos   //Player is higher
               && ray_zpos > collidedthing.zpos //Player is lower
               )
            {
                velocity.x = (hit.distance - skinwdth);// *signedvec;
                rayleng = hit.distance;
            }
            else { continue; }


        }
    }
}
void DrawRaysLitVer(ref Bounds bound)
{
    float rayspacing = bound.size.x / raycount;
    int i = 0;
    for (i = 0; i < raycount; i++)
    {
        float signedvec = Mathf.Sign(velocity.y);
        float ray_zpos = this.zpos + 1 + (rayspacing * i);
        float rayleng = Mathf.Abs(signedvec);

        Vector2 raypos = (signedvec != -1 ? new Vector2(bound.min.x, bound.max.y) : new Vector2(bound.min.x, bound.min.y));
        raypos += Vector2.right * (rayspacing * i);
        RaycastHit2D hit = Physics2D.Raycast(raypos, Vector2.up * signedvec, rayleng, 256);


        Debug.DrawRay(raypos, Vector2.up * signedvec, Color.blue);
        //Debug.Log(Vector2.up * signedvec);

        if (hit)
        {
            o_collidableobject collidedthing = hit.collider.gameObject.transform.parent.GetComponent<o_collidableobject>();
            if (Mathf.Floor(collidedthing.zpos + collidedthing.height) > ray_zpos   //Player is higher
                && ray_zpos > collidedthing.zpos //Player is lower
                )
            {
                velocity.y = (hit.distance - skinwdth);
                rayleng = hit.distance;
            }
            else { continue; }
        }
    }
}
*/
#endregion

#region CHECK_VARIABLES
/*
public bool CheckIntegersEqual(string nameofint, int integer)
{
    foreach (ev_integer integr in EventIntegers)
    {
        if (integr.integer_name == nameofint)
        {
            return integer == integr.integer;
        }
    }
    return false;
}

public bool CheckIntegersGreaterThan(string nameofint, int integer)
{
    foreach (ev_integer integr in EventIntegers)
    {
        if (integr.integer_name == nameofint)
        {
            return integer < integr.integer;
        }
    }
    return false;
}

public bool CheckIntegersLessThan(string nameofint, int integer)
{
    foreach (ev_integer integr in EventIntegers)
    {
        if (integr.integer_name == nameofint)
        {
            return integer > integr.integer;
        }
    }
    return false;
}*/
#endregion

#region TRIGGER_WALK
/*
 IEnumerator Walk()
 {
     Vector2 pos = this.transform.position;
     Vector2 lastpos = transform.position;

     List<Vector2> points = new List<Vector2>();

     foreach (trigger_obj trig in movesteps)
     {
         pos = lastpos + (trig.direcion.normalized * trig.steps * speed);
         lastpos = pos;
         points.Add(lastpos);
     }


     Vector2 lgoalpoint = transform.position;
     while (i < points.Count)
     {
         if (!movesteps[i].is_Teleport)
         {
             selobj.velocity = Vector2.zero;
             Vector2 goalpoint = points[i]; 

             float timer = Vector2.Distance(lgoalpoint, goalpoint) / (speed);
             lgoalpoint = points[i];
             print(timer);
             while (timer > 0)
             {
                 selobj.velocity = (goalpoint - (Vector2)selobj.transform.position).normalized * speed;

                 timer -= Time.deltaTime;
                 yield return new WaitForSeconds(Time.deltaTime);
             }
         }
         else {
             Vector2 vecpos = movesteps[i].direcion * 44;
             vecpos *= movesteps[i].steps;
         }
         i++;
     }
 }
             */

#endregion

#region TRIGGER_LOAD_MAP
/*
ADDED ON 17-11-2018
    case ev_details.EVENT_TYPES.LOAD_MAP:

        EditorGUILayout.LabelField("Name of map");
        ev.string0 = EditorGUILayout.TextArea(ev.string0);

        if (GUILayout.Button("Open Level file"))
        {
            string levelload = EditorUtility.OpenFilePanel("Open Json level file", "Assets/Levels/", "");
            if (levelload != null)
                LoadTempLevel(levelload);
        }
        EditorGUILayout.LabelField("Mouse Position X: " + mouseArea.transform.position.y + " Mouse Position Y: " + mouseArea.transform.position.y + " Press S to confirm");

        EditorGUILayout.LabelField("Spawn position from this point");
        ev.float0 = EditorGUILayout.FloatField("XPOS.", ev.float0);
        ev.float1 = EditorGUILayout.FloatField("YPOS.", ev.float1);
        break;
        */

/*
    case ev_details.EVENT_TYPES.LOAD_MAP:

        s_globals.SaveData();
        doingEvents = false;
        s_leveledit led = GameObject.Find("General").GetComponent<s_leveledit>();
        isactive = false;
        led.TriggerSpawn(selobj.GetComponent<o_plcharacter>(), current_ev.string0, new Vector2(current_ev.float0, current_ev.float1), this);

        break;*/
#endregion

#region SAVING_MAP_OLD
/*
 * ADDED ON 12/02/2019
    if (PrefabUtility.FindPrefabRoot(maps[0].gameObject) == null)
    {
        PrefabUtility.CreatePrefab("Assets/Levels/" + "Obj.prefab", current_map.gameObject);
    }
    else
    {
        //loadedLevel = PrefabUtility.FindPrefabRoot(res);
    }
    s_map mapfil = new s_map();

    mapfil.name = current_area;
    o_character[] objectsInMap = null;
    o_trigger[] triggersInMap = null;
    o_collidableobject[] tilesInMap = null;

    if (GameObject.Find(current_area) != null)
    {
        objectsInMap = GameObject.Find(current_area).transform.Find("Entities").GetComponentsInChildren<o_character>();
        triggersInMap = GameObject.Find(current_area).transform.Find("Triggers").GetComponentsInChildren<o_trigger>();
        tilesInMap = GameObject.Find(current_area).transform.Find("Tiles").GetComponentsInChildren<o_collidableobject>();
    }
    else
    {
        objectsInMap = GameObject.Find("New Level").transform.Find("Entities").GetComponentsInChildren<o_character>();
        triggersInMap = GameObject.Find("New Level").transform.Find("Triggers").GetComponentsInChildren<o_trigger>();
        tilesInMap = GameObject.Find("New Level").transform.Find("Tiles").GetComponentsInChildren<o_collidableobject>();
    }

    List<s_map.s_trig> triggerlist = new List<s_map.s_trig>();
    List<s_map.s_chara> charalist = new List<s_map.s_chara>();
    List<s_map.s_tileobj> tilelist = new List<s_map.s_tileobj>();

    foreach (o_trigger obj in triggersInMap)
    {
        triggerlist.Add(new s_map.s_trig(obj.transform.position, obj.Events.ev_Details));
    }
    mapfil.triggerdata = triggerlist;

    foreach (s_object obj in objectsInMap)
    {
        charalist.Add(new s_map.s_chara(obj.positioninworld));
    }
    mapfil.objectdata = charalist;

    foreach (s_object obj in tilesInMap)
    {
        tilelist.Add(new s_map.s_tileobj(obj.positioninworld, obj.height));
    }
    mapfil.tilesdata = tilelist;

    GameObject levedatabase = GameObject.Find("LevelDatabase");
    print(levedatabase.name);
    dat = levedatabase.GetComponent<s_leveldatabase>();

    dat.maps.Add(mapfil);

    dat = null;
    if (GameObject.Find(current_area) != null)
        GameObject.Find(current_area).SetActive(false);
    else
    {
        GameObject.Find("New Level").SetActive(false);
    }

*/
#endregion

#region SPAWN_GRAPHIC_TILES
/*
Added on 11/05/2019
foreach (s_map.s_block b in mapdat.graphicTiles)
{
    if (InEditor)
    {
        o_tile t = Instantiate(FindOBJ("TileDecor"), new Vector3(b.position.x, b.position.y), Quaternion.identity).GetComponent<o_tile>();
        t.SpirteRend = t.GetComponent<SpriteRenderer>();
        t.SpirteRend.sprite = TileSprites.Find(til => til.name == b.sprite_name);
        t.name = "TileDecor";
        t.transform.SetParent(tileIG.transform);
    }
    else
    {
        o_tile t = SpawnObject("TileDecor", new Vector3(b.position.x, b.position.y), Quaternion.identity).GetComponent<o_tile>();
        t.SpirteRend = t.GetComponent<SpriteRenderer>();
        t.SpirteRend.sprite = TileSprites.Find(til => til.name == b.sprite_name);
        t.name = "TileDecor";
        t.transform.SetParent(tileIG.transform);
    }
}
*/
#endregion