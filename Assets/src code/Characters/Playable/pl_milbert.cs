using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class pl_milbert : BHIII_character
{
    float invinciblity_Timer;
    public struct m_action
    {
        public m_action(Vector2 position, MagnumFoudation.o_character obj, TYPE type)
        {
            this.position = position;
            this.obj = obj;
            this.type = type;
        }
        public Vector2 position;
        public MagnumFoudation.o_character obj;
        public enum TYPE
        {
            WALK,
            ACTIVATE,
            MOVE_OBST,
            FIGHT,
            NONE = -1
        }
        public TYPE type;
    }

    public AudioClip lightDashYell;
    public AudioClip mediumDashYell;
    public AudioClip heavyDashYell;

    public o_throwable throwableObject;

    float[] attackChargePoints = new float[3] {
        0.1f,
        0.7f,
        1.55f
    };
    public int attackChargeIndex = 0;

    public float regen_delay = 1f;
    float start_attack_timer = 0;
    bool regenHP = false;
    bool defeatdelayStart = false;
    public float dashDel;   //To not make the player spam dash

    o_plcharacter player;
    public LineRenderer lineDraw;
    //public Queue<s_node> queues = new Queue<s_node>();
    public Queue<m_action> milbertActionQueue = new Queue<m_action>();
    public enum MILBERT_STATE
    {
        MANUAL,
        AUTOMATIC,
        NONE
    }
    public MILBERT_STATE MB_STATE = MILBERT_STATE.NONE;

    new private void Start()
    {
        nodegraph = GameObject.Find("General").GetComponent<s_nodegraph>();
        AI = true;
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        SetAttackObject<o_bullet>();
        player = GameObject.Find("Player").GetComponent<o_plcharacter>();
        SetAIFunction(-1, IdleState);
        base.Start();
        DisableAttack();
        Initialize();
        rbody2d.velocity = Vector2.zero;
    }

    /*
    public bool CanMove(Vector2 p) {

        RaycastHit2D hit = new RaycastHit2D();

        hit = Physics2D.Linecast(transform.position, ((Vector2)transform.position - p).normalized);
        Debug.DrawRay(transform.position, ((Vector2)transform.position - p).normalized, Color.red, Vector2.Distance(transform.position, p));
        
        if (hit) {
            print("Seems like I found " + hit.transform.name + "!");
            return false;
        }
        print("I'll be there!");
        return true;
    }
    public bool CanMove(Vector2 p, Vector2 p2)
    {

        RaycastHit2D hit = new RaycastHit2D();

        hit = Physics2D.Raycast(p2, (p2 - p).normalized, Vector2.Distance(p2, p), 256);
        Debug.DrawRay(p2, (p2 - p).normalized, Color.red, Vector2.Distance(p2, p));
        
        if (hit)
        {
            print("Seems like I found " + hit.transform.name + "!");
            return false;
        }
        print("I'll be there!");
        return true;
    }

    public BHIII_character GetCharacter(Vector2 pos)
    {
        o_plcharacter pl = GameObject.Find("Player").GetComponent<o_plcharacter>();
        BHIII_character[] targ = pl.CharacterTypeS<BHIII_character>(pl.targets);
        foreach (BHIII_character c in targ)
        {
            Vector2 offset = new Vector2(50, 50);
            //print("Mouse pos: " + (pos + offset) + " Target: " + c.positioninworld);
            if (c.transform.position.x < pos.x + offset.x)
            {
                print("Check1");
                if (c.transform.position.x > pos.x - offset.x)
                {
                    print("Check2");
                    if (c.transform.position.y < pos.y + offset.y)
                    {
                        print("Check3");
                        if (c.transform.position.y > pos.y - offset.y)
                        {
                            print("Check4");
                            return c;
                        }
                    }
                }
            }
        }
        return null;
    }

    public void AddPoint()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        BHIII_character c = GetCharacter(pos);
        if (c != null)
        {
            m_action act = new m_action(pos, c, m_action.TYPE.FIGHT);
            milbertActionQueue.Enqueue(act);
            return;
        }
        if (milbertActionQueue.Count > 1)
        {
            m_action act = new m_action(pos, null, m_action.TYPE.WALK);
            milbertActionQueue.Enqueue(act);
            if (CanMove(pos, milbertActionQueue.Peek().position))
            {
                return;
            }
        } else
        {

            m_action act = new m_action(pos, null, m_action.TYPE.WALK);
            milbertActionQueue.Enqueue(act);
            if (CanMove(pos))
            {
            }
        }
    }

    public IEnumerator del()
    {
        isdelayed = true;
        SetAIFunction(-1, RetreatState);
        yield return new WaitForSeconds(0.7f);
        isdelayed = false;
    }

    public IEnumerator AttackTarg()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_ATTACK;
        angle = ReturnAngle(new Vector3(direction.x, direction.y, 0));

        direction = LookAtTarget(target);
        EnableAttack(direction);
        Dash(1.3f);//ShootBullet(1);

        yield return new WaitForSeconds(0.7f);
        DisableAttack();
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
    }
    */

    public override void AttackState()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;

        if (target == null)
        {
            SetAIFunction(-1, IdleState);
        }
        else
        {
            direction = LookAtTarget(target);
            if (target.health <= 0)
            {
                target = player;
                SetAIFunction(-1, IdleState);
            }
            if (!CheckTargetDistance(target, 250) || Physics2D.Linecast(target.transform.position, transform.position, collisionLayer))
            {
                //milbertActionQueue.Dequeue();
                //print("No longer fighting");
                SetAIFunction(-1, IdleState);
            } else if (CheckTargetDistance(target, 120))
            {
                if (dashDel <= 0) {
                    direction = LookAtTarget(target);
                    MilbertDashAI();
                }
            }
        }
       
    }

    public override void RetreatState()
    {
        if (target == null)
        {
            SetAIFunction(-1, IdleState);
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
        base.RetreatState();
        if (AI_timerUp)
        {
            SetAIFunction(-1, IdleState);
        }

    }

    public override void IdleState()
    {
        switch (MB_STATE)
        {
            case MILBERT_STATE.MANUAL:
                AI = false;
                regenHP = false;
                break;

            case MILBERT_STATE.AUTOMATIC:
                regenHP = true;
                milbertActionQueue.Clear();
                o_character pote_target = GetClosestTarget<o_character>(460);
                
                if (pote_target != null)
                {
                    if (!Physics2D.Linecast(pote_target.transform.position, transform.position, collisionLayer))
                    {
                        target = pote_target;
                        if (!CheckTargetDistance(target, 50))
                        {
                            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                            direction = LookAtTarget(target);
                            SetAIFunction(-1, AttackState);
                        }
                        else
                            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                    }
                }
                else
                {
                    if (!CheckTargetDistance(player, 50))
                    {
                        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                        direction = LookAtTarget(player);
                    }
                    else
                        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                }
                break;

                /*
            case MILBERT_STATE.SEMI_AUTO:
                if (milbertActionQueue.Count > 0)
                {
                    switch (milbertActionQueue.Peek().type)
                    {
                        case m_action.TYPE.NONE:
                            CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                            milbertActionQueue.Dequeue();
                            break;

                        case m_action.TYPE.WALK:

                            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                            Vector2 v = milbertActionQueue.Peek().position;
                            if (!CheckTargetDistance(v, 10))
                            {
                                direction = LookAtTarget(v);
                                //MoveMotor();
                            }
                            else
                            {
                                milbertActionQueue.Dequeue();
                            }
                            break;

                        case m_action.TYPE.FIGHT:
                            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                            BHIII_character c = (BHIII_character)milbertActionQueue.Peek().obj;
                            target = c;

                            if (CheckTargetDistance(target, 150))
                            {
                                print("AI FUNCTION ADDED");
                                milbertActionQueue.Clear();
                                SetAIFunction(-1, AttackState);
                            }
                            break;
                    }
                }
                break;
                */
        }
    }

    public override void ArtificialIntelleginceControl()
    {
    }


    public void MilbertDashAI()
    {
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        EnableAttack(direction);
        Dash(0.25f, 5.95f);

    }
    public void MilbertDash() {

        mouse = MouseAng();
        direction = mouse.normalized;

        EnableAttack(direction);
        Dash(0.25f, 5.95f);
    }

    new private void Update()
    {
        switch (CHARACTER_STATE)
        {
            case CHARACTER_STATES.STATE_IDLE:
            case CHARACTER_STATES.STATE_MOVING:
                AnimMove();
                if (dashDel > 0)
                    dashDel -= Time.deltaTime;
                break;
            case CHARACTER_STATES.STATE_DEFEAT:

                UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
                break;

            case CHARACTER_STATES.STATE_DASHING:
                AnimDash(false);
                break;
        }

        if (regenHP)
        {
            if (health < maxHealth)
            {
                if (regen_delay > 0)
                    regen_delay -= Time.deltaTime;
                else
                {
                    health += 1f;
                    regen_delay = 0.85f;
                }
            }
        }
        if (CHARACTER_STATE != CHARACTER_STATES.STATE_DASHING && CHARACTER_STATE != CHARACTER_STATES.STATE_DEFEAT)
        {
            switch (MB_STATE)
            {
                case MILBERT_STATE.NONE:
                    AI = true;
                    break;
                case MILBERT_STATE.AUTOMATIC:
                    if (!CheckTargetDistance(player, 900))
                        transform.position = player.transform.position;
                    AI = true;
                    break;
                case MILBERT_STATE.MANUAL:
                    switch (CHARACTER_STATE)
                    {
                        case CHARACTER_STATES.STATE_IDLE:

                            if (grounded)
                            {
                                if (ArrowKeyControl())
                                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                                else
                                    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;

                                if (Input.GetMouseButton(0))
                                {
                                    if (dashDel <= 0)
                                        MilbertDash();
                                }
                            }
                            break;

                        case CHARACTER_STATES.STATE_MOVING:
                            if (control)
                            {
                                if (grounded)
                                {
                                    if (!ArrowKeyControl())
                                    {
                                        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                                    }
                                    if (Input.GetMouseButton(0))
                                    {
                                        if (dashDel <= 0)
                                            MilbertDash();
                                    }
                                }
                            }

                            break;
                    }
                    AI = false;
                    break;
            }
        }

        base.Update();
    }

    public override void OnHit(BHIII_bullet b)
    {
        if (b != null)
        {

            if (invinciblity_Timer <= 0)
            {
                invinciblity_Timer = 0.7f;
                if (!AI)
                {
                    BHIII_globals.gl.DMGFX();
                    invinciblity_Timer = 1.5f;
                }
                base.OnHit(b);
            }
        }
    }
    new private void FixedUpdate()
    {
        if (invinciblity_Timer > 0)
            invinciblity_Timer -= Time.deltaTime;
        start_attack_timer = Mathf.Clamp(start_attack_timer, 0, attackChargePoints[attackChargePoints.Length - 1]);
        //CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;

        if (lineDraw == null) {
            lineDraw = GameObject.Find("MilbertLine").GetComponent<LineRenderer>();
        }

        base.FixedUpdate();
        /*
        if (milbertActionQueue.Count > 0)
        {
            int i = 0;
            Vector2 lastpoint = transform.position;
            if (lineDraw != null)
            {
                lineDraw.positionCount = milbertActionQueue.Count;
                lineDraw.SetPosition(i, lastpoint);
            }
            foreach (m_action a in milbertActionQueue)
            {
                switch (a.type)
                {
                    case m_action.TYPE.FIGHT:
                        lastpoint = a.obj.transform.position;
                        break;

                    case m_action.TYPE.WALK:
                        lastpoint = a.position;
                        break;
                }
                if (lineDraw != null)
                    lineDraw.SetPosition(i, lastpoint);
                i++;
            }
        }
        */
    }

    public override void AfterDash()
    {
        base.AfterDash();
        dashDel = 0.35f;
        DisableAttack();
        SetRandomDirection();
        if (AI)
            SetAIFunction(0.9f, RetreatState);
    }

}

/*
public void OnDrawGizmos()
{
    int i = 0;
    Vector2 lastpoint = positioninworld;
    foreach (m_action a in milbertActionQueue)
    {
        switch (a.type)
        {
            case m_action.TYPE.FIGHT:
                Gizmos.DrawLine(lastpoint, a.obj.positioninworld);
                lastpoint = a.obj.positioninworld;
                break;

            case m_action.TYPE.WALK:
                Gizmos.DrawLine(lastpoint, a.position);
                lastpoint = a.position;
                break;
        }
        i++;
    }
}
*/
/*
public IEnumerator Move()
{
    while (queues.Count != 0)
    {
        while (positioninworld != new Vector3(queues.Peek().realPosition.x, queues.Peek().realPosition.y))
        {
            direction = ((Vector3)queues.Peek().realPosition - positioninworld).normalized;

            xvel = direction.x * terminalspd;
            yvel = direction.y * terminalspd;
            if (xvel != 0 || yvel != 0)
            {
                velocity += new Vector2(xvel, 0);
                velocity += new Vector2(0, yvel);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
        queues.Dequeue();
    }
}
public void ClearQueue()
{
    queues.Clear();
}

public void AddToQueue(List<s_node> nodes)
{
    queues.Clear();
    int i = 0;
    while (i != nodes.Count)
    {
        queues.Enqueue(nodes[i]);
        i++;
    }
}
*/
/*
s_node pl = nodegraph.PosToNode
    (
    positioninworld
    );

s_node maus = nodegraph.PosToNode
    (
    Camera.main.ScreenToWorldPoint(Input.mousePosition)
    );

*/
/*
        switch (MB_STATE)
        {
            case MILBERT_STATE.MANUAL:
                AI = false;
                break;

            case MILBERT_STATE.AUTOMATIC:

                if (!CheckTargetDistance(target, 50))
                {
                    direction = LookAtTarget(target);
                    positioninworld += (Vector3)direction * 10;
                }
                break;

            case MILBERT_STATE.SEMI_AUTO:
                if (milbertActionQueue.Count > 0)
                {
                    switch (milbertActionQueue.Peek().type)
                    {
                        case m_action.TYPE.WALK:

                            Vector2 v = milbertActionQueue.Peek().position;
                            if (!CheckTargetDistance(v, 10))
                            {
                                direction = LookAtTarget(v);
                                positioninworld += (Vector3)direction * 10;
                                //MoveMotor();
                            }
                            else
                            {
                                milbertActionQueue.Dequeue();
                            }
                            break;

                        case m_action.TYPE.FIGHT:
                            o_character c = (o_character)milbertActionQueue.Peek().obj;
                            target = c;

                            if (!CheckTargetDistance(c, 20))
                            {
                                direction = LookAtTarget(c);
                                positioninworld += (Vector3)direction * 10;
                            }
                            if (c.health <= 0)
                            {
                                milbertActionQueue.Dequeue();
                            }
                            break;
                    }
                }
                break;
        }
        */
/*
   if (Input.GetMouseButton(1))
   {
       if (pl != null && maus != null)
       {
           print("Sorry!");
           nodegraph.PathFind(pl, maus);
           AddToQueue(nodegraph.path);
           StartCoroutine(Move());
       }
   }
   */
