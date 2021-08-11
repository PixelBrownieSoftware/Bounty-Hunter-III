using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;
using MoonSharp.Interpreter;

[System.Serializable]
public struct o_attack
{
    public float damage;
    public float attackDuration;
    public float poise;
}

public class BHIII_character : o_character
{
    #region variables

    private int localV1;
    ParticleSystem afterImg;
    //protected float ai_timer = 0;

    //For bosses who want to spawn other characters
    public List<BHIII_character> characterLinks = new List<BHIII_character>();
    const float wldgravity = 3.98f;
    protected float targDistance;

    internal float _attack_timer = 0;

    public string onDefeatLabel;
    public string onHurtLabel;
    public string hurtSound = "impact_damage2";
    public List<string> hurtSounds = new List<string>();

    public o_itemObject itemDrop;
    public bool affectedByKnockback = true;

    public bool goInWater = false;
    internal float swimSpeed = 1;
    public bool dissapearOnDefeat = true;
    public bool enemyHurtSound = true;
    public bool isSwimming = false;
    public float oyxegenAmount = 100; //Max is 100

    public Vector3 offsetDirection;     //Only for AI
    public TextAsset aiTextFile;

    protected float[] healthPhases;
    protected string currentAIFuncName;
    public bool flipSprite = false; 
    protected Vector2 lastTPPosition;    //So that they don't jump to the same place again

    protected int healthPhase
    {
        get {
            if (healthPhases == null)
                return 0;
            else {
                float minHP = health;
                int phase = 0;
                for (int i = 0; i < healthPhases.Length; i++)
                {
                    if (health <= healthPhases[i] * maxHealth) {
                        phase = i;
                    }
                }
                return phase;
            }
        }
    }
    int collisiionTYPE = 0;
    public o_particle afterFX;
    public bool showHealth = true;
    #endregion

    #region Structs

    [System.Serializable]
    public class s_AIAction
    {
        public enum ACTION_TYPE
        {
            END = -2,
            LABEL = -1,
            MOVE_FORWARD,
            WHEN,
            SHOOT,
            DASH,
            JUMP,
            MOVE_BACK,
            WAIT,
            RANDOM_NUM,
            CUSTOM_FUNCTION
        }
        public ACTION_TYPE action;
        public bool boolean0;
        public bool simultaneous = false;
        public string string0;
        public int int0;
        public int int1;
        public int int2;
        public float float0;
        public float float1;
        public Vector2 vector;
    }
    #endregion

    protected new void Start()
    {
        velSlip = 0;
        damage_timer = 0;
        nodegraph = GameObject.Find("General").GetComponent<s_nodegraph>();
        SetAttackObject<o_bullet>();
        Initialize();
        base.Start();
    }


    public Vector2[] TeleportObjectsToPositions(GameObject[] objs) {
        Vector2[] posList = new Vector2[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            GameObject obj = objs[i];
            posList[i] = obj.transform.position;
        }
        return posList;
    }

    public void StartCoroutineState(IEnumerator coroute) {
        SetAIFunction(-1,NothingState);
        StartCoroutine(coroute);
    }

    public enum SPAWN_TYPE { 
    APPEAR,
    JUMP
    }

    public void AddCharacter(BHIII_character chara, Vector3 position)
    {
        StartCoroutine(SpawnChar(chara, position, SPAWN_TYPE.APPEAR));
    }
    public void AddCharacter(BHIII_character chara, Vector3 position, SPAWN_TYPE sp_type )
    {
        StartCoroutine(SpawnChar(chara, position, sp_type));
    }
    IEnumerator SpawnChar(BHIII_character chara, Vector3 position, SPAWN_TYPE sp_type) {

        s_mapEventholder mp = GameObject.Find("MapObject").GetComponent<s_mapEventholder>();
        BHIII_character ch = mp.AddCharacter<BHIII_character>(chara, position);
        
        ch.control = false;
        if (ch.collision == null)
            ch.collision = ch.GetComponent<BoxCollider2D>();
        ch.isInvicible = true;

        float t = 0;
        switch (sp_type) {
            case SPAWN_TYPE.APPEAR:

                ch.rendererObj.color = Color.clear;
                while (ch.rendererObj.color != Color.white)
                {
                    t += Time.deltaTime * 1.2f;
                    //print(t);
                    ch.rendererObj.color = Color.Lerp(Color.clear, Color.white, t);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                break;

            case SPAWN_TYPE.JUMP:
                ch._Z_offset = 330;
                ch.JumpWithoutGround(0.001f, 300);

                while (!ch.grounded)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                break;
        }

        mp.ReAddTargets();
        ch.control = true;
        ch.isInvicible = false;
        characterLinks.Add(ch);
    }

    public void KillAllCharacterLinks()
    {
        s_soundmanager.sound.PlaySound("impact_damage_finish");
        foreach (BHIII_character ob in characterLinks)
        {
            ob.health = 0;
            s_mapManager.LevEd.SpawnObject<o_particle>("finish effect", ob.transform.position, Quaternion.identity);
        }
        characterLinks.Clear();
    }

    public void CheckCharacterLinks() {
        foreach (BHIII_character c in characterLinks) {
            if (c.CHARACTER_STATE == CHARACTER_STATES.STATE_DEFEAT)
                characterLinks.Remove(c);
        }
    }

    public void HurtFunction(float damage, float damage_time)
    {
        health -= damage;
        damage_timer = damage_time;
        if (health <= 0)
        {
                CHARACTER_STATE = CHARACTER_STATES.STATE_DEFEAT;
            // s_triggerhandler2.trig.JumpToEvent(onDefeatLabel);
        }
        //Perhaps spawn particles and things here
    }

    public override void AfterDefeat()
    {
        isInvicible = true;
        base.AfterDefeat();
        if (itemDrop != null)
            s_mapManager.LevEd.SpawnObject<o_itemObject>(itemDrop.ID, transform.position, Quaternion.identity);
        if (dissapearOnDefeat)
            Destroy(gameObject);
    }

    public void NothingState()
    {
    }

    public Vector2 RandomPosition(Vector2[] tpPos)
    {
        Vector2 pos = tpPos[0];
        foreach (Vector2 v in tpPos)
        {
            if (lastTPPosition == v)
                continue;
            int chance = Random.Range(0, 3);
            if (chance == 2)
                pos = v;

        }
        lastTPPosition = pos;
        return pos;
    }
    public Vector2 FurtherestAwayPosition(Vector2[] tpPos)
    {
        float min = 0;
        Vector2 pos = tpPos[0];
        foreach (Vector2 v in tpPos)
        {
            if (lastTPPosition == v)
                continue;
            if (min < Vector2.Distance(transform.position, v))
            {
                pos = v;
                min = Vector2.Distance(transform.position, v);
            }
        }
        lastTPPosition = pos;
        return pos;
    }
    public Vector2 NearestToTargetPosition(Vector2[] tpPos, Vector2 targPos)
    {
        float min = 0;
        Vector2 pos = tpPos[0];
        foreach (Vector2 v in tpPos)
        {
            if (lastTPPosition == v)
                continue;
            if (min > Vector2.Distance(targPos, v))
            {
                pos = v;
                min = Vector2.Distance(targPos, v);
            }
        }
        lastTPPosition = pos;
        return pos;
    }

    public void ShootBullet(string bulletName, int pow, float pointAngle, float timer, float spd)
    {
        float a = Mathf.Deg2Rad * pointAngle;
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>(bulletName, transform.position + (Vector3)collision.offset, Quaternion.identity);
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.attack_pow = pow;
        bullt.direction = new Vector2(Mathf.Cos(a), Mathf.Sin(a)).normalized;
        bullt.SetTimer(timer);
        bullt.terminalSpeedOrigin = spd;
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBullet(int pow, float pointAngle, float timer, float spd)
    {
        float a = Mathf.Deg2Rad * pointAngle;
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>("Bullet", transform.position + (Vector3)collision.offset, Quaternion.identity);
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.attack_pow = pow;
        bullt.direction = new Vector2(Mathf.Sin(a), Mathf.Cos(a)).normalized;
        bullt.SetTimer(timer);
        bullt.terminalSpeedOrigin = spd;
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBullet(o_weapon weap, Vector2 pointdr, float timer, float spd)
    {
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>("Bullet", transform.position, Quaternion.identity);
        bullt.attack_pow = weap.attackPow;
        bullt.direction = pointdr.normalized; 
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.SetTimer(timer);
        bullt.terminalSpeedOrigin = spd;
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBullet(int pow, Vector2 pointdr, float timer, float spd)
    {
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>("Bullet", transform.position, Quaternion.identity);
        bullt.attack_pow = pow;
        bullt.direction = pointdr;
        bullt.terminalSpeedOrigin = spd;
        bullt.SetTimer(timer);
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBullet(Vector2 pointdir, string bulletName, float timer, float spd)
    {
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>(bulletName, transform.position, Quaternion.identity);
        bullt.direction = pointdir;
        bullt.terminalSpeedOrigin = spd;
        bullt.SetTimer(timer);
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }

    public void ShootBullet(string bulletName, int pow, float pointAngle, float timer)
    {
        float a = Mathf.Deg2Rad * pointAngle;
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>(bulletName, transform.position, Quaternion.identity);
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.attack_pow = pow;
        bullt.direction = new Vector2(Mathf.Cos(a), Mathf.Sin(a)).normalized;
        bullt.SetTimer(timer);
        bullt.terminalSpeedOrigin = 10f;
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBullet(int pow, float pointAngle, float timer)
    {
        float a = Mathf.Deg2Rad * pointAngle;
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>("Bullet", transform.position, Quaternion.identity);
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0,0, angle));
        bullt.attack_pow = pow;
        bullt.direction =  new Vector2(Mathf.Sin(a),Mathf.Cos(a)).normalized;
        bullt.SetTimer(timer);
        bullt.terminalSpeedOrigin = 10f;
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBullet(o_weapon weap, Vector2 pointdr, float timer)
    {
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>("Bullet", transform.position, Quaternion.identity);
        bullt.attack_pow = weap.attackPow;
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.direction = pointdr.normalized;
        bullt.SetTimer(timer);
        bullt.terminalSpeedOrigin = 10f;
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBullet(int pow, Vector2 pointdr, float timer)
    {
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>("Bullet", transform.position, Quaternion.identity);
        bullt.attack_pow = pow;
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.direction = pointdr;
        bullt.terminalSpeedOrigin = 10f;
        bullt.SetTimer(timer);
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBullet(Vector2 pointdir, string bulletName, float timer)
    {
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>(bulletName, transform.position, Quaternion.identity);
        bullt.direction = pointdir;
        bullt.terminalSpeedOrigin = 10f;
        bullt.SetTimer(timer);
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBulletAtPositionWithDir(int pow, Vector2 pointdr, Vector2 pos, float timer)
    {
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>("Bullet", transform.position, Quaternion.identity);
        bullt.attack_pow = pow;
        bullt.direction = pointdr;
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.transform.position = pos;
        bullt.terminalSpeedOrigin = 10f;
        bullt.SetTimer(timer);
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }

    public void ShootBulletAtPosition(int pow, Vector2 pos, float timer)
    {
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>("Bullet", transform.position, Quaternion.identity);
        bullt.attack_pow = pow;
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.transform.position = pos;
        bullt.terminalSpeedOrigin = 10f;
        bullt.SetTimer(timer);
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }
    public void ShootBulletAtPosition(int pow, Vector2 pos, string bulletName, float timer)
    {
        o_bullet bullt = ll_BHIII.LevEd.SpawnObject<o_bullet>(bulletName, transform.position, Quaternion.identity);
        bullt.attack_pow = pow;
        bullt.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullt.transform.position = pos;
        bullt.terminalSpeedOrigin = 10f;
        bullt.SetTimer(timer);
        bullt.parent = this;
        bullt.GetComponent<IPoolerObj>().SpawnStart();
    }

    //When the character jumps into the water
    public IEnumerator SplashWater()
    {
        s_soundmanager.sound.PlaySound("water_splash");
        rbody2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.25f);
        CHARACTER_STATE = (CHARACTER_STATES)10;
    }
    //When the character jumps into the water
    public IEnumerator SubmergeFromWater()
    {
        rbody2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
    }
    public IEnumerator FallDown()
    {
        CHARACTER_STATE = (CHARACTER_STATES)(-1);
        if (!AI)
            HurtFunction(1, 0.7f);
        else
            health = 0;
        isInvicible = true;
        rbody2d.velocity = Vector2.zero;
        rendererObj.color = Color.clear;
        yield return new WaitForSeconds(0.5f);
        transform.position = lastposbeforefall;
        rendererObj.color = Color.white;
        isInvicible = false;
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
    }

    public void CheckCollisionWithLayers()
    {

        bool nothing = Physics2D.IsTouchingLayers(collision, -1);

        bool fallingLedge = Physics2D.OverlapBox(transform.position + (Vector3)collision.offset, collision.size, 0, 1 << 10);
        bool falling = Physics2D.OverlapBox(transform.position + (Vector3)collision.offset, collision.size, 0, 1 << 11);

        bool water = false;
        bool ditch = false;

        if (CHARACTER_STATE != CHARACTER_STATES.STATE_DASHING )
        {
            if (!isHover)
            {
                water = Physics2D.OverlapBox(transform.position + (Vector3)collision.offset, collision.size, 0, 1 << 13);
                ditch = Physics2D.OverlapBox(transform.position + (Vector3)collision.offset, collision.size, 0, 1 << 12);
                //To make sure the main character dosen't constantly fall down by just simply moving into a ditch
                if (grounded && !ditch)
                {
                    Vector2 checkPoint = (Vector2)transform.position + collision.offset + (direction * 3);
                    bool ditchBef = Physics2D.OverlapBox(checkPoint, collision.size, 0, 1 << 12);
                    if (ditchBef)
                        transform.position = lastposbeforefall;
                }
            }
        }


        if (fallingLedge)
            collisiionTYPE = 1;
        else if (water)
            collisiionTYPE = 2;
        else if (ditch)
            collisiionTYPE = 3;
        else if (falling)
            collisiionTYPE = 4;
        else
            collisiionTYPE = 0;

        if (!water)
        {
            if (oyxegenAmount < 100)
                oyxegenAmount += Time.deltaTime * 35.65f;
        }

        switch (collisiionTYPE)
        {
            default:
                if (CHARACTER_STATE == CHARACTER_STATES.STATE_FALLING)
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                }
                if (CHARACTER_STATE == (CHARACTER_STATES)10)
                {
                    CHARACTER_STATE = (CHARACTER_STATES)11;
                    StartCoroutine(SubmergeFromWater());
                }
                if (grounded && CHARACTER_STATE != CHARACTER_STATES.STATE_DASHING)
                {
                    lastposbeforefall = transform.position;
                }
                break;

            //falling ledge
            case 1:
                if (grounded)
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_FALLING;
                }
                break;

            //water
            case 2:
                if (goInWater && !AI) {
                    if (CHARACTER_STATE != (CHARACTER_STATES)11 && CHARACTER_STATE != (CHARACTER_STATES)10)
                    {
                        CHARACTER_STATE = (CHARACTER_STATES)11;
                        StartCoroutine(SplashWater());
                    }
                }
                else if(!goInWater)
                    Destroy(gameObject);
                break;

            //ditch
            case 3:
                if (grounded && CHARACTER_STATE != CHARACTER_STATES.STATE_DASHING && CHARACTER_STATE != (CHARACTER_STATES)(-1))
                {
                    StartCoroutine(FallDown());
                }
                break;

            //falling
            case 4:
                CHARACTER_STATE = CHARACTER_STATES.STATE_FALLING;
                break;

        }
    }

    public void AnimDir1D()
    {
        int horizontalDir = Mathf.RoundToInt(direction.x);
        if (horizontalDir < 0)
        {
            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (horizontalDir >= 0)
        {
            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
        }
    }

    public void Play1DirAnim(string up, bool loop, Vector2 direc, float speed)
    {
        {
            int verticalDir = Mathf.RoundToInt(direc.y);
            int horizontalDir = Mathf.RoundToInt(direc.x);
            if (horizontalDir == -1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                SetAnimation(up, loop, speed);
            }
            else if (horizontalDir == 1 || horizontalDir == 0)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                SetAnimation(up, loop, speed);
            }
        }
    }
    public void Play1DirAnim(string up, bool loop, Vector2 direc)
    {
        {
            int verticalDir = Mathf.RoundToInt(direc.y);
            int horizontalDir = Mathf.RoundToInt(direc.x);
            if (horizontalDir == -1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                SetAnimation(up, loop);
            }
            else if (horizontalDir == 1 || horizontalDir == 0)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                SetAnimation(up, loop);
            }
        }
    }
    public void Play1DirAnim(string up, bool loop)
    {
        {
            int verticalDir = Mathf.RoundToInt(direction.y);
            int horizontalDir = Mathf.RoundToInt(direction.x);
            if (horizontalDir == -1 && verticalDir == 1 ||
                horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                SetAnimation(up, loop);
            }
            else if (horizontalDir == 1 && verticalDir == 1 ||
                horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                SetAnimation(up, loop);
            }
        }
    }

    public void AnimDash(bool loop)
    {
        if (detailedAnim)
        {
            int verticalDir = Mathf.RoundToInt(direction.y);
            int horizontalDir = Mathf.RoundToInt(direction.x);

            if (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
            {
                if (verticalDir == -1 && horizontalDir == 0)
                    SetAnimation("attack_d", loop);
                else if (verticalDir == 1 && horizontalDir == 0)
                    SetAnimation("attack_u", loop);
                else if (horizontalDir == -1 && verticalDir == 1 ||
                    horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
                {
                    rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    SetAnimation("attack_s", loop);
                }
                else if (horizontalDir == 1 && verticalDir == 1 ||
                    horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
                {
                    rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                    SetAnimation("attack_s", loop);
                }
            }
        }
    }

    public override void CrashAfterDash()
    {
        s_soundmanager.sound.PlaySound("impact_crashwall");
        s_mapManager.LevEd.SpawnObject<o_particle>("hit effect", transform.position, Quaternion.identity);
        base.CrashAfterDash();
    }

    public new void Update()
    {
        base.Update();
        COLLISIONDET(); 
        CheckCollisionWithLayers();

        if (!detailedAnim) {

            int horizontalDir = Mathf.RoundToInt(direction.x);
            if (flipSprite)
            {
                if (horizontalDir == -1)
                {
                    rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                }
                else if (horizontalDir == 1 || horizontalDir == 0)
                {
                    rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
            }
            else {

                if (horizontalDir == -1)
                {
                    rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                else if (horizontalDir == 1 || horizontalDir == 0)
                {
                    rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                }
            }
        }

        if(currentAIFunction != null)
            currentAIFuncName = currentAIFunction.Method.Name.ToString();

        //print(Time.timeScale);

        switch (CHARACTER_STATE)
        {


            case CHARACTER_STATES.STATE_ATTACK:
                dashdelay -= Time.deltaTime;
                if (dashdelay <= 0)
                {
                    DisableAttack();
                    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                }
                break;
            case (CHARACTER_STATES)11:
                rbody2d.velocity = Vector2.zero;
                break;

            case (CHARACTER_STATES)10:

                if (rbody2d != null)
                    rbody2d.velocity = (direction * terminalspd) * swimSpeed / 2.55f;
                break;
        }
        if (characterLinks != null) {
            for (int i = 0; i < characterLinks.Count; i++)
            {
                if (characterLinks[i] == null)
                    characterLinks.Remove(characterLinks[i]);
            }
        }

        if (AI)
        {
            switch (CHARACTER_STATE)
            {
                default:
                    if (control)
                    {
                        if (currentAIFunction != null)
                            currentAIFunction.Invoke();
                    }
                    break;

                case CHARACTER_STATES.STATE_DASHING:
                case CHARACTER_STATES.STATE_HURT:
                case CHARACTER_STATES.STATE_NOTHING:
                case CHARACTER_STATES.STATE_ATTACK:
                    break;

            }
        }
        if (CHARACTER_STATE == CHARACTER_STATES.STATE_DASHING)
        {
            if (afterFX != null)
            {
                afterFX.StartThing(rendererObj.transform.rotation.z);
                afterFX.SetSprite(rendererObj);
                afterFX.parent = this;
            }
        }
        else
        {
            if (afterFX != null)
                afterFX.Desp();
        }
    }

    public bool CheckForDitch() {
        Vector2 checkPoint = (Vector2)transform.position + (direction * 2);
        s_node nod = CheckNode(checkPoint);
        if (nod == null)
            return false;
        else {
            if ((COLLISION_T)nod.COLTYPE == COLLISION_T.DITCH)
                return true;
            else
                return false;
        }
    }

    public override void AfterDash()
    {
        base.AfterDash();
        DisableAttack();
        ai_timer = 0;
    }

    public void AttackGo(float del)
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_ATTACK;
        dashdelay = del;
        EnableAttack(mouse.normalized);
        Pushforce(mouse.normalized, 50);
    }
    public void AttackGoStand(float del)
    {
        rbody2d.velocity = Vector2.zero;
        CHARACTER_STATE = CHARACTER_STATES.STATE_ATTACK;
        dashdelay = del;
    }
    public void AttackGoNoThing(float del, Vector2 dir)
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_ATTACK;
        dashdelay = del;
        Pushforce(dir.normalized, 50);
    }

    public void AttackGo(float del, Vector2 dir)
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_ATTACK;
        dashdelay = del;
        EnableAttack(dir.normalized);
        Pushforce(dir.normalized, 50);
    }
    public void SetRandomDirection() {

        offsetDirection = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
        direction = (-LookAtTarget(target) + offsetDirection).normalized;
    }
    public Vector2 GetRandomDirection()
    {
        offsetDirection = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
        return (-LookAtTarget(target) + offsetDirection).normalized;
    }


    protected new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    void COLLISIONDET()
    {
        if (nodegraph != null)
        {
            if (IS_KINEMATIC)
            {
                CurrentNode = CheckNode(transform.position + offsetCOL);
                // s_gui.AddText(nodeg.PosToVec(new Vector2( positioninworld.x + collision.size.x / 2,  positioninworld.y + collision.size.y / 2)).ToString());

                if (CurrentNode != null)
                {
                    switch ((COLLISION_T)CurrentNode.COLTYPE)
                    {
                        case COLLISION_T.FALLING:
                            if (CHARACTER_STATE != CHARACTER_STATES.STATE_FALLING)
                            {
                                //print("check");
                                rbody2d.velocity = Vector2.zero;
                                if (grounded)
                                {
                                    fallposy = nodegraph.CheckYFall(CurrentNode, (int)COLLISION_T.FALLING);
                                    //print(fallposy);
                                    transform.position = CurrentNode.realPosition;
                                    CHARACTER_STATE = CHARACTER_STATES.STATE_FALLING;
                                }
                            }
                            break;

                        case COLLISION_T.FALLING_ON_LAND:
                            if (grounded && CHARACTER_STATE != CHARACTER_STATES.STATE_FALLING)
                            {
                                rbody2d.velocity = Vector2.zero;
                                fallposy = nodegraph.CheckYFall(CurrentNode, (int)COLLISION_T.FALLING_ON_LAND);
                                print(fallposy);
                                transform.position = CurrentNode.realPosition;
                                CHARACTER_STATE = CHARACTER_STATES.STATE_FALLING;
                            }
                            break;

                        case COLLISION_T.DITCH:
                            if (grounded && CHARACTER_STATE != CHARACTER_STATES.STATE_DASHING)
                            {
                                health--;
                                transform.position = lastposbeforefall;
                            }
                            break;

                        case COLLISION_T.NONE:
                            if (isSwimming) {
                                isSwimming = false;
                                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                            }
                            break;

                        case COLLISION_T.WATER_TILE:

                            if (grounded && CHARACTER_STATE != CHARACTER_STATES.STATE_DASHING)
                            {
                                if (goInWater)
                                    isSwimming = true;
                                else
                                    transform.position = lastposbeforefall;
                            }
                            break;
                    }
                }

            }
        }

        BHIII_bullet b = GetBullet<BHIII_bullet>(collision);
        if (b != null)
        {
            if (CHARACTER_STATE != CHARACTER_STATES.STATE_DASHING)
            {
                if (b.isDashingBullet || b.isbullet) {
                    if (!isInvicible)
                    {
                        if (b.avoidOnJump)
                        {
                            if (grounded)
                                OnHit(b);
                        }
                        else
                            OnHit(b);
                    }
                }
            }
        }

    }

    public virtual void AfterHit()
    {

    }

    public IEnumerator CharacterShake()
    {
        if (AI)
        {
            if (health <= 0)
            {
                BHIII_globals.gl.DefeatFX();
                s_soundmanager.sound.PlaySound("impact_damage_finish");
                s_mapManager.LevEd.SpawnObject<o_particle>("finish effect", transform.position, Quaternion.identity);
            }
            else {
                s_soundmanager.sound.PlaySound("impact_damage");
            }
        }
        else
        {
            s_soundmanager.sound.PlaySound("impact_damage2");
        }
        s_mapManager.LevEd.SpawnObject<o_particle>("hit effect", transform.position, Quaternion.identity);
        for (int i = 0; i < 20; i++) {
            spriteOffset = new Vector2(Random.Range(5, -5), Random.Range(5, -5));
            yield return new WaitForSeconds(0.005f);
        }
        spriteOffset = new Vector2(0, 0);
    }

    public virtual void OnHit(BHIII_bullet b)
    {
        if (damage_timer > 0)
            return;
        dashdelay = 0;
        CHARACTER_STATE = CHARACTER_STATES.STATE_HURT;
        HurtFunction(b.attack_pow, b.damageImpact_timer);
        
        if (hurtSounds != null)
        {
            if (hurtSounds.Count > 0)
            {
                if(!enemyHurtSound)
                    s_soundmanager.sound.PlaySound("impact_damage2");
                PlaySound(hurtSounds[Random.Range(0, hurtSounds.Count)]);
            }
        }
        if(affectedByKnockback)
            rbody2d.velocity = b.direction * 60;
        StartCoroutine(CharacterShake());
        target = b.parent;
        if (b != null)
        {
            if (!b.parent.AI) {
                b.parent.target = this;
            }
            b.OnImpact();
        }
    }


    public IEnumerator ExplosionDeath()
    {
        rbody2d.velocity = Vector2.zero;
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        isInvicible = true;
        for (int i = 0; i < 12; i++) {

            PlaySound("small_explosion");
            s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
                transform.position + new Vector3(Random.Range(-50,50), Random.Range(-50, 50)), Quaternion.identity);
            for (int i2 = 0; i2 < 20; i2++)
            {
                spriteOffset = new Vector2(Random.Range(15, -15), Random.Range(15, -15));
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitForSeconds(0.05f);
            spriteOffset = new Vector2(0, 0);
        }
        yield return new WaitForSeconds(0.1f);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(50, 50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-50, 50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(50, -50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-50, -50), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-25, -25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(25, -25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(-25, 25), Quaternion.identity);
        s_mapManager.LevEd.SpawnObject<o_particle>("Explosion",
            transform.position + new Vector3(25, 25), Quaternion.identity);
        PlaySound("explode_sound");
        Destroy(gameObject);
    }

}