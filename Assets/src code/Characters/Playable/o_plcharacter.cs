using System.Collections;
using MagnumFoudation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class o_plcharacter : BHIII_character
{
    public List<o_weapon> weapons = new List<o_weapon>();
    public float ammoPoints;
    public int maxAmmoPoints;
    public string selected_weapon = "Pistol";
    public o_weapon weapon_current;
    public LayerMask trig;
    public o_weapon primary_weapon;
    public o_weapon waterboat_gun;
    public o_item heal_item;
    int selected_weapon_num;
    UnityEngine.UI.Text gui;
    Vector2 dashDirInitial;
    public float dashDot;
    public float dashDel;   //To not make the player spam dash
    public float dashSlipDel;   //To not make the player spam dash

    public int melee_stage = 0;
    public bool meleeDelay;
    public float meleeTimer = 0.2f;
    public bool meleeAgain = true;

    float invinciblity_Timer;

    float waterBoatGunDelay = 0;

    bool canDashSwim = true;
    bool isWaterBoat = false;

    public GameObject systgui;
    public Text txt;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    new void Start ()
    {
        weapons.Add(new o_weapon(1, "Pistol", o_weapon.WEAPON_TYPE.WEAPON_RANGED, 1));
        //weapons.Add(new o_weapon(2, "Club", o_weapon.WEAPON_TYPE.WEAPON_MELEE));
        //weapons.Add(new o_weapon(10, "Shotgun", o_weapon.WEAPON_TYPE.WEAPON_RANGED));
        primary_weapon = new o_weapon(1, "Fist", o_weapon.WEAPON_TYPE.WEAPON_MELEE, 0);
        gui = GameObject.Find("General").GetComponent<UnityEngine.UI.Text>();
        waterboat_gun = new o_weapon(0.45f, "Waterboat gun", o_weapon.WEAPON_TYPE.WEAPON_RANGED_NO_STAND, 0.5f);
        SetAttackObject<o_bullet>();
        weapon_current = weapons[0];
        DisableAttack();
        Initialize();
        animHand = SpriteObj.GetComponent<s_animhandler>();
        base.Start();
        //collision = transform.GetChild(1).GetComponent<BoxCollider2D>();
    }

    IEnumerator SwimDelay() {
        s_soundmanager.sound.PlaySound("water_swim");

        canDashSwim = false;
        swimSpeed = 2.55f;
        yield return new WaitForSeconds(0.55f);
        swimSpeed = 1;
        canDashSwim = true;
    }
    public override void CrashAfterDash()
    {
        base.CrashAfterDash();
    }

    public void AttackPl(o_weapon weap)
    {
        if (weap.ammoConsumption > ammoPoints)
            return;
        switch (weap.weapon_type) 
        {
            case o_weapon.WEAPON_TYPE.WEAPON_MELEE:

                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                rbody2d.velocity = Vector2.zero;
                mouse = MouseAng().normalized; 
                PlaySound("slash2");
                Play1DirAnim("attack_d_1", false, mouse);
                AttackGo(0.3f, mouse);
                meleeTimer = 0.2f;
                //attackobject.GetComponent<BHIII_bullet>().OneTimeHit();
                break;

            case o_weapon.WEAPON_TYPE.WEAPON_RANGED:

                mouse = MouseAng().normalized;
                AttackGoStand(0.25f); 
                PlayShootAnim();
                angle = ReturnAngle2(mouse.normalized);
                switch (weapon_current.name) {
                    case "Pistol":
                        s_soundmanager.sound.PlaySound("peacemaker_shoot");
                        s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                        ShootBullet(weap, mouse.normalized, 0.32f,15f);
                        break;

                    case "sgun":

                        ShootBullet(weap, mouse.normalized + new Vector3(0.1f,0), 0.12f);
                        ShootBullet(weap, mouse.normalized, 0.12f);
                        break;
                }
                break;

            case o_weapon.WEAPON_TYPE.WEAPON_RANGED_NO_STAND:

                s_mapManager.LevEd.SpawnObject<o_particle>("shoot fx", transform.position, Quaternion.identity);
                mouse = MouseAng().normalized;
                ShootBullet(weap, mouse, 0.92f);
                break;
        }
        ammoPoints -= weap.ammoConsumption;
    }


    public void Play2DirAnim(string up, string down)
    {
        {
            int verticalDir = Mathf.RoundToInt(direction.y);
            int horizontalDir = Mathf.RoundToInt(direction.x);
            if (verticalDir == -1 && horizontalDir == 0)
                SetAnimation(down, true);
            else if (verticalDir == 1 && horizontalDir == 0)
                SetAnimation(up, true);
            else if (horizontalDir == -1 && verticalDir == 1 ||
                horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                SetAnimation(down, true);
            }
            else if (horizontalDir == 1 && verticalDir == 1 ||
                horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                SetAnimation(down, true);
            }
        }
    }
    public void PlayFourDirAnim(string up, string down, string side)
    {
        {
            int verticalDir = Mathf.RoundToInt(direction.y);
            int horizontalDir = Mathf.RoundToInt(direction.x);
            if (verticalDir == -1 && horizontalDir == 0)
                SetAnimation(down, true);
            else if (verticalDir == 1 && horizontalDir == 0)
                SetAnimation(up, true);
            else if (horizontalDir == -1 && verticalDir == 1 ||
                horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                SetAnimation(side, true);
            }
            else if (horizontalDir == 1 && verticalDir == 1 ||
                horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                SetAnimation(side, true);
            }
        }
    }
    public override void PlayerControl()
    {
        if (isWaterBoat) {
            if(terminalspd == terminalSpeedOrigin * 1.5f)
                isWaterBoat = false;
        }

        if (s_globals.GetGlobalFlag("WTR_BT_CONTROL") == 1)
        {
            isWaterBoat = true;
            SetAnimation("waterboat", false);
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
            s_camera.cam.camWithMouseOffset = true;
            //s_camera.cam.offset = (GetClosestTarget<BHIII_character>().transform.position - transform.position) * 3.5f;

            terminalspd = terminalSpeedOrigin * 1.5f;
            ArrowKeyControlPref();
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            AnimDir1D();

            if (Input.GetMouseButton(0))
            {
                if (waterBoatGunDelay <= 0)
                {
                    waterBoatGunDelay = 0.1f;
                    if(ammoPoints > 0.9f)
                        PlaySound("machine_gun_boat");
                    AttackPl(waterboat_gun);
                }
            }
            else
                ammoPoints+= 2f * Time.deltaTime;
            if (waterBoatGunDelay > 0)
                waterBoatGunDelay -= Time.deltaTime;
        }
        else
        {
            s_camera.cam.camWithMouseOffset = false;
            if (dashSlipDel > 0)
                dashSlipDel -= Time.deltaTime;
            AnimMove();
            switch (CHARACTER_STATE)
            {
                case CHARACTER_STATES.STATE_IDLE:
                case CHARACTER_STATES.STATE_MOVING:
                    if (dashDel > 0)
                        dashDel -= Time.deltaTime; 
                    break;
            }

            switch (CHARACTER_STATE)
            {
                case CHARACTER_STATES.STATE_DASHING:
                    dashDirection = dashDirInitial;
                    {
                        int verticalDir = Mathf.RoundToInt(dashDirection.y);
                        int horizontalDir = Mathf.RoundToInt(dashDirection.x);
                        if (verticalDir == -1 && horizontalDir == 0)
                            SetAnimation("dash_d", true);
                        else if (verticalDir == 1 && horizontalDir == 0)
                            SetAnimation("dash_u", true);
                        else if (horizontalDir == -1 && verticalDir == 1 ||
                            horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
                        {
                            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            SetAnimation("dash_s", true);
                        }
                        else if (horizontalDir == 1 && verticalDir == 1 ||
                            horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
                        {
                            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                            SetAnimation("dash_s", true);
                        }
                    }

                    break;

                case CHARACTER_STATES.STATE_IDLE:
                    if (dashSlipDel <= 0)
                    {
                        if (ArrowKeyControlPref())
                        {
                            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                        }
                        if (grounded) {

                            AnimMove();

                            if (Input.GetMouseButtonDown(0))
                            {
                                if (meleeAgain)
                                    AttackPl(primary_weapon);
                            }

                            if (Input.GetMouseButtonDown(1))
                            {
                                AttackPl(weapons[selected_weapon_num]);
                            }

                            if (dashDel <= 0)
                            {

                                if (Input.GetKeyDown(s_globals.GetKeyPref("dash")))
                                {
                                    if (PlayerPrefs.GetInt("Dash direction") == 1)
                                    {
                                        dashDirInitial = MouseAng().normalized;
                                    }
                                    else {
                                        dashDirInitial = direction;
                                    }
                                    PlaySound("dash3");
                                    Dash(0.25f, 5.85f);
                                }
                            }
                        }

                    }

                    break;

                case CHARACTER_STATES.STATE_MOVING:
                    if (control)
                    {
                        if (!ArrowKeyControlPref())
                        {
                            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                        }
                        if (grounded)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                if (meleeAgain)
                                    AttackPl(primary_weapon);
                            }

                            if (Input.GetMouseButtonDown(1))
                            {
                                AttackPl(weapons[selected_weapon_num]);
                            }

                            if (dashDel <= 0)
                            {

                                if (Input.GetKeyDown(s_globals.GetKeyPref("dash")))
                                {
                                    //dashDirection = MouseAng().normalized;
                                    if (PlayerPrefs.GetInt("Dash direction") == 1)
                                    {
                                        dashDirInitial = MouseAng().normalized;
                                    }
                                    else
                                    {
                                        dashDirInitial = direction;
                                    }
                                    /*
                                    Vector2 oldDir = dashDirection;
                                    dashDirection = Vector3.RotateTowards(oldDir, direction, 1 * Time.deltaTime, 1.3f);
                                    dashDirInitial = direction;
                                    */
                                    PlaySound("dash3");
                                    Dash(0.25f, 6.85f);
                                }
                            }
                        }
                    }

                    break;

                case CHARACTER_STATES.STATE_CLIMBING:

                    if (Input.GetAxisRaw("Vertical") != 0)
                    {
                        direction = new Vector2(0, Input.GetAxisRaw("Vertical"));
                    }
                    else
                        direction = new Vector2(0, 0);

                    break;

                case (CHARACTER_STATES)10: //SWIMMING

                    if (canDashSwim) {
                        if (ArrowKeyControlPref()) {
                            if (Input.GetKeyDown(s_globals.GetKeyPref("dash")))
                            {
                                dashDirInitial = MouseAng().normalized;
                                StartCoroutine(SwimDelay());
                            }
                        }
                    }

                    break;
            }
        }
    }


    public void PlayShootAnim() {

        int verticalDir = Mathf.RoundToInt(direction.y);
        int horizontalDir = Mathf.RoundToInt(direction.x);
        if (verticalDir == -1 && horizontalDir == 0)
            SetAnimation("shoot_d", false);
        else if (verticalDir == 1 && horizontalDir == 0)
            SetAnimation("shoot_u", false);
        else if (horizontalDir == -1 && verticalDir == 1 ||
            horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
        {
            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            SetAnimation("shoot_s", false);
        }
        else if (horizontalDir == 1 && verticalDir == 1 ||
            horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
        {
            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
            SetAnimation("shoot_s", false);
        }
    }

    public override void AnimMove()
    {
        base.AnimMove();
        int verticalDir = Mathf.RoundToInt(direction.y);
        int horizontalDir = Mathf.RoundToInt(direction.x);

        if (CHARACTER_STATE == (CHARACTER_STATES)11)
        {
            SetAnimation("fall", true);
        }
        if (CHARACTER_STATE == (CHARACTER_STATES)11)
        {
            SetAnimation("fall", true);
        }
        if (CHARACTER_STATE == (CHARACTER_STATES)10)
        {
            if (verticalDir == -1 && horizontalDir == 0)
                SetAnimation("swim_d", true);
            else if (verticalDir == 1 && horizontalDir == 0)
                SetAnimation("swim_u", true);
            else if (horizontalDir == -1 && verticalDir == 1 ||
                horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                SetAnimation("swim_s", true);
            }
            else if (horizontalDir == 1 && verticalDir == 1 ||
                horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                SetAnimation("swim_s", true);
            }
        }
    }

    public override void OnHit(BHIII_bullet b)
    {
        if (b != null) {

            if (invinciblity_Timer <= 0)
            {
                BHIII_globals.gl.DMGFX();
                invinciblity_Timer = 1.5f;
                base.OnHit(b);
            }
        }
    }

    public override void AfterDash()
    {
        dashDel = 0.25f;
        base.AfterDash();
        dashSlipDel = 0.2f;
         CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
    }

    public new void Update ()
    {
        base.Update();
        if (invinciblity_Timer > 0)
            invinciblity_Timer -= Time.deltaTime;
        
        if (gui != null)
            gui.text = "Current weapon: " + weapons[selected_weapon_num].name;
        /*
        if (heal_item != null)
            gui.text += "\n Heal item: " + heal_item.name + "\nPress Q to use item.";
        */
        //GetComponent<SpriteRenderer>().sortingOrder = (int)(positioninworld.y / 20);

        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        angle = ReturnAngle(new Vector3(mouse.x, mouse.y, 0));

        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            selected_weapon_num++;
            selected_weapon_num = Mathf.Clamp(selected_weapon_num, 0, weapons.Count);
            if (selected_weapon_num == weapons.Count)
                selected_weapon_num = 0;
        }
        */
        o_trigger trig = IfTouchingGetCol<o_trigger>(collision);
        if (trig != null)
        {
            if (trig.TRIGGER_T == o_trigger.TRIGGER_TYPE.CONTACT_INPUT)
            {
                systgui.gameObject.SetActive(true);
                txt.text = "Press " + s_globals.GetKeyPref("select").ToString();
            }
        }
        else
        {
            systgui.gameObject.SetActive(false);
            txt.text = "";
        }
        ammoPoints = Mathf.Clamp(ammoPoints, 0, maxAmmoPoints);


    }
}
/*
public new void FixedUpdate()
{
    base.FixedUpdate();
    if (s_globals.GetGlobalFlag("WTR_BT_CONTROL") != 1)
        if (isSwimming && oyxegenAmount < 100.99f)
        {
            oyxegenAmount += Time.deltaTime * 15.85f;
        }

    }
*/
/*
void ShootWeapons() 
{
    switch (weapons[selected_weapon_num].name) 
    {
        case "Pistol":
            //1 hp damage
            ammoPoints -= weap.ammoConsumption;
            break;

        case "Shotgun":
            //4 hp damage
            ammoPoints -= 2f;
            break;

        case "Machinegun":
            //0.5 hp damage
            ammoPoints -= 0.25f;
            break;

        case "Lazer gun":
            //Consumes depending on how much you hold
            break;
    }
}
*/
/*
public IEnumerator Attack(o_weapon weap)
{
    CHARACTER_STATE = CHARACTER_STATES.STATE_ATTACK;
    if (weap.weapon_type == o_weapon.WEAPON_TYPE.WEAPON_MELEE)
    {
        AttackGo(0.5f);
        mouse = MouseAng().normalized;
        angle = ReturnAngle(new Vector3(mouse.x, mouse.y, 0));
        //direction = mouse.normalized;

        EnableAttack(mouse);
        Pushforce(mouse, 50);

        yield return new WaitForSeconds(0.2f);
        DisableAttack();
    }
    else if (weap.weapon_type == o_weapon.WEAPON_TYPE.WEAPON_RANGED)
    {
        mouse = MouseAng().normalized;
        ShootBullet(weap, mouse.normalized, 1.2f);

        o_bullet bullt = bull.GetComponent<o_bullet>();
        bullt.attack_pow = weap.attackPow;
        bullt.parent = this;
    }
    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
}
*/
