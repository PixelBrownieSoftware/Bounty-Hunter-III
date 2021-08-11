using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
public class s_triggerhandler : MonoBehaviour {

    public static s_triggerhandler trig;
    public bool doingEvents = false;
    Image fade;
    private gui_dialogue Dialogue;
    const float shutterdepth = 1.55f;
    public s_events Events;
    public Queue<s_events> QueuedDetails = new Queue<s_events>();
    public o_trigger current_trigger;
    bool first_move_event = true;
    public o_character[] characters;
    public s_object selobj;
    o_character c;
    bool skip = false;
    public List<Sprite> faceSprites = new List<Sprite>();
    public Image muggshot;

    private void Awake()
    {
        if (trig == null)
            trig = this;
        fade = GameObject.Find("GUIFADE").GetComponent<Image>();
        if (GameObject.Find("Dialogue"))
            Dialogue = GameObject.Find("Dialogue").GetComponent<gui_dialogue>();
    }

    public void QueueUpEvent(o_trigger trig, s_events event_o)
    {
        event_o.trigger = trig;
        QueuedDetails.Enqueue(event_o);
    }

    private void Update()
    {
        if (!doingEvents)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                skip = true;
            }
            if (QueuedDetails.Count > 0)
            {
                if (skip)
                    Time.timeScale = 10;
                else
                    Time.timeScale = 1;
                Events = QueuedDetails.Peek();
                StartCoroutine(EventPlayMast());
            }
        }
    }
    public IEnumerator EventPlayMast()
    {
        doingEvents = true;
        current_trigger = Events.trigger;

        while (current_trigger.ev_num < Events.ev_Details.Length)
        {
            if (current_trigger == null)
            {
                QueuedDetails.Dequeue();
                break;
            }
            ev_details current_ev = Events.ev_Details[current_trigger.ev_num];
            if (current_ev.simultaneous)
            {
                StartCoroutine(EventPlay(current_ev));
            }
            else
            {
                yield return StartCoroutine(EventPlay(current_ev));
            }
            if (!current_trigger.isactive)
            {
                skip = false;
                break;
            }

            print("Increment");
            current_trigger.ev_num++;
        }
        skip = false;
        current_trigger = null;
        doingEvents = false;
        Events = null;
        //isskipping = false;
        Time.timeScale = 1;
        first_move_event = true;
    }

    IEnumerator EventPlay(ev_details currnet_ev)
    {
        ev_details current_ev = Events.ev_Details[current_trigger.ev_num];
        switch (current_ev.eventType)
        {
            case ev_details.EVENT_TYPES.PUT_SHUTTERS:

                Image sh1 = GameObject.Find("Shutter1").GetComponent<Image>();
                Image sh2 = GameObject.Find("Shutter2").GetComponent<Image>();

                if (current_ev.boolean)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        sh1.rectTransform.position += new Vector3(0, -shutterdepth);
                        sh2.rectTransform.position += new Vector3(0, shutterdepth);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                }
                else {

                    for (int i = 0; i < 30; i++)
                    {
                        sh1.rectTransform.position += new Vector3(0, shutterdepth);
                        sh2.rectTransform.position += new Vector3(0, -shutterdepth);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                }
                break;

            case ev_details.EVENT_TYPES.ANIMATION:

                Animator character = selobj.GetComponent<Animator>();
                character.Play(current_ev.int0);
                character.speed = current_ev.float0;
                break;

            case ev_details.EVENT_TYPES.CHANGE_MAP:

                s_globals.SaveData();
                s_leveledit led = GameObject.Find("General").GetComponent<s_leveledit>();
                if (current_ev.string0 == "" || current_ev.string1 == null)
                    led.TriggerSpawn(current_ev.string1);
                else
                    led.TriggerSpawn(current_ev.string0, current_ev.string1);


                selobj.GetComponent<o_plcharacter>().control = true;
                current_trigger.isactive = false;
                current_trigger.ev_num = 0;
                QueuedDetails.Dequeue();

                break;

            case ev_details.EVENT_TYPES.DISPLAY_CHARACTER_HEALTH:
                if (!current_ev.boolean)
                    s_gui.AddToBossHPList(GameObject.Find(current_ev.string0).GetComponent<o_character>());
                else
                    s_gui.ClearBossHPList();
                break;

            case ev_details.EVENT_TYPES.MOVEMNET:

                float timer = 1.02f;
                o_character charaMove = GameObject.Find(current_ev.string0).GetComponent<o_character>();

                Vector2 newpos = charaMove.transform.position;

                if (first_move_event)
                    if (!current_ev.boolean)
                    {

                        if (GameObject.Find(current_ev.string0).GetComponent<o_character>() == selobj)
                        {
                            newpos = current_trigger.positioninworld;
                            first_move_event = false;
                        }
                    }

                if (current_ev.boolean)
                {
                    charaMove.transform.position = new Vector3(current_ev.float0, current_ev.float1, 0);
                    break;
                }

                first_move_event = false;
                while (timer > 0)
                {
                    newpos += (current_ev.direcion.normalized * current_ev.float0 * current_ev.float1) * 0.007f;
                    timer -= 0.007f;
                }

                float dist = Vector2.Distance(charaMove.positioninworld, newpos);
                Vector2 dir = (newpos - new Vector2(charaMove.transform.position.x, charaMove.transform.position.y)).normalized;
                print(newpos);


                while (Vector2.Distance(charaMove.transform.position, newpos)
                    > dist * 0.01f)
                {
                    charaMove.transform.position += (Vector3)(dir * current_ev.float0 * current_ev.float1) * 0.007f;
                    yield return new WaitForSeconds(Time.deltaTime);
                }

                

                break;

            case ev_details.EVENT_TYPES.DIALOGUE:
                if (skip)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                    yield return null;
                }
                Dialogue.done_event = false;
                Dialogue.automatic = current_ev.boolean;
                StartCoroutine(Dialogue.DisplayDialogue(current_ev.string0));
                while (!Dialogue.done_event)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                break;

            case ev_details.EVENT_TYPES.ADD_SHOP_ITEM:

                if (current_trigger == null)
                {
                    print("There is no trigger");
                    break;
                }
                u_shop shop = current_trigger.GetComponent<u_shop>();
                if (shop != null)
                {
                    shop.items.Add(new o_shopItem(new o_item(current_ev.string0, (o_item.ITEM_TYPE)current_ev.int1), current_ev.int0));
                }
                else
                    print("Please add shop component.");

                break;

            case ev_details.EVENT_TYPES.ADD_INVENTORY:
                s_globals.AddItem(new o_item(current_ev.string0, (o_item.ITEM_TYPE)current_ev.int1));
                break;

            case ev_details.EVENT_TYPES.BREAK_EVENT:
                selobj.GetComponent<o_plcharacter>().control = true;
                current_trigger.isactive = false;
                if (current_ev.boolean)     //Does this cutscene reset?
                {
                    current_trigger.ev_num  = 0;   //Reset the number
                }
                QueuedDetails.Dequeue();
                break;

            case ev_details.EVENT_TYPES.RUN_CHARACTER_SCRIPT:

                if (!current_ev.boolean)
                {
                    o_character plok = GameObject.Find(current_ev.string0).GetComponent<o_character>();
                    plok.control = current_ev.boolean1;
                    if (!current_ev.boolean1)
                    {
                        plok.rbody.velocity = Vector2.zero;
                        plok.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;
                    }
                    if (current_ev.int0 == 1)
                    {
                        plok.Initialize();
                    }
                }
                else {
                    foreach (string characternam in current_ev.stringList)
                    {
                        GameObject.Find(characternam).GetComponent<o_character>().control = true;
                    }
                }
                break;

            case ev_details.EVENT_TYPES.CAMERA_MOVEMENT:

                s_camera ca = GameObject.Find("Main Camera").GetComponent<s_camera>();
                ca.focus = false;

                float spe = current_ev.float0; //SPEED
                float s = 0;
                ca.CameraLerpInit(ca.transform.position, new Vector3(current_ev.pos.x, current_ev.pos.y));

                if (current_ev.boolean)
                {
                    s_object obje = GameObject.Find(current_ev.string0).GetComponent<s_object>();
                    Vector2 pos = obje.transform.position;
                    ca.CameraLerpInit(ca.transform.position, new Vector3(pos.x, pos.y));
                    float dista = Vector2.Distance(ca.transform.position, new Vector3(pos.x, pos.y));
                    bool b = ca.CameraLerp();
                    while (!b)
                    {
                        b = ca.CameraLerp();
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                    if (current_ev.boolean1)
                    {
                        //ca.GetComponent<s_camera>().focus = true;
                        //ca.GetComponent<s_camera>().player = obje.GetComponent<o_character>();
                    }
                }
                else {

                    float dista = Vector2.Distance(ca.transform.position, new Vector3(current_ev.pos.x, current_ev.pos.y, - 15));
                    Vector2 pos = new Vector3(current_ev.pos.x, current_ev.pos.y, -15);
                    bool b = ca.CameraLerp();
                    while (!b)
                    {
                        b = ca.CameraLerp();
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                }
                break;

            case ev_details.EVENT_TYPES.CHECK_FLAG:
                int integr = s_globals.GetGlobalFlag(current_ev.string0);

                switch (current_ev.logic)
                {
                    case ev_details.LOGIC_TYPE.ITEM_OWNED:
                        if (s_globals.CheckItem(new o_item(current_ev.string0, (o_item.ITEM_TYPE)current_ev.int0)))
                        {
                            current_trigger.ev_num = current_ev.int1 - 1;
                        }
                        else
                        {
                            if (current_ev.boolean)     //Does this have an "else if"?
                                current_trigger.ev_num = current_ev.int2;      //Other Label to jump to
                        }
                        break;

                    case ev_details.LOGIC_TYPE.VAR_EQUAL:
                        if (integr == current_ev.int0)  //Check if it is equal to the value
                        {
                            current_trigger.ev_num = current_ev.int1 - 1;   //Label to jump to
                        }
                        else
                        {
                            if (current_ev.boolean)     //Does this have an "else if"?
                            {
                                current_trigger.ev_num = current_ev.int2;      //Other Label to jump to
                            }
                        }
                        break;

                    case ev_details.LOGIC_TYPE.VAR_GREATER:
                        if (integr > current_ev.int0)  //Check if it is greater
                        {
                            current_trigger.ev_num = current_ev.int1;   //Label to jump to
                        }
                        else
                        {
                            if (current_ev.boolean)     //Does this have an "else if"?
                            {
                                current_trigger.ev_num = current_ev.int2;      //Other Label to jump to
                            }
                        }
                        break;


                    case ev_details.LOGIC_TYPE.VAR_LESS:
                        if (integr < current_ev.int0)  //Check if it is less
                        {
                            current_trigger.ev_num = current_ev.int1;   //Label to jump to
                        }
                        else
                        {
                            if (current_ev.boolean)     //Does this have an "else if"?
                            {
                                current_trigger.ev_num = current_ev.int2;      //Other Label to jump to
                            }
                        }
                        break;
                }
                if (integr == current_ev.int0)
                {

                }
                break;

            case ev_details.EVENT_TYPES.UTILITY_FOCUS:
                s_utility utility;

                if (current_trigger.GetComponent<s_utility>() != null)
                {
                    utility = current_trigger.GetComponent<s_utility>();
                }
                else
                {
                    break;
                }

                o_plcharacter p = GameObject.Find("Player").GetComponent<o_plcharacter>();
                p.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_MOVING;
                p.control = true;
                utility.istriggered = true;
                while (utility.istriggered)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                break;

            case ev_details.EVENT_TYPES.DISPLAY_ICON:

                //trans.sizeDelta = new Vector2(880, trans.sizeDelta.y);
                //trans.position = new Vector2(706, trans.position.y);
                if (current_ev.string0 == "" || current_ev.string0 == null)
                {
                    muggshot.sprite = null;
                    muggshot.gameObject.SetActive(false);
                    break;
                }
                muggshot.gameObject.SetActive(true);
                Sprite ic = faceSprites.Find(x => x.name == current_ev.string0);
                muggshot.sprite = ic;

                break;

            case ev_details.EVENT_TYPES.TELEPORT:

                if (current_ev.boolean)
                {
                    ca = GameObject.Find("Main Camera").GetComponent<s_camera>();
                    ca.focus = false;
                    ca.transform.position = new Vector3(current_ev.pos.x, current_ev.pos.y);
                }
                else
                {
                    c = GameObject.Find(current_ev.string0).GetComponent<o_character>();
                    c.transform.position = new Vector3(current_ev.pos.x, current_ev.pos.y);
                }

                break;

            case ev_details.EVENT_TYPES.SET_HEALTH:

                c = GameObject.Find(current_ev.string0).GetComponent<o_character>();
                if (c != null)
                    c.health = current_ev.float0;

                break;
                

            case ev_details.EVENT_TYPES.UTILITY_CHECK:

                //Make this like the conditional statements where it checks if the utility is still active

                break;

            case ev_details.EVENT_TYPES.UTILITY_INITIALIZE:
                s_utility ut;

                o_plcharacter player = GameObject.Find("Player").GetComponent<o_plcharacter>();
                player.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_MOVING;
                
                if (current_trigger.GetComponent<s_utility>() != null)
                {
                    ut = current_trigger.GetComponent<s_utility>();
                    ut.EventIntialize();
                    ut.istriggered = true;

                    selobj.GetComponent<o_plcharacter>().control = true;
                    current_trigger.isactive = false;
                    QueuedDetails.Dequeue();
                }
                else
                {
                    break;
                }

                break;

            case ev_details.EVENT_TYPES.FADE:

                Color col = new Color(current_ev.colour.a, current_ev.colour.g, current_ev.colour.b, current_ev.colour.a);
                float t = 0;

                while (fade.color != col)
                {
                    t += Time.deltaTime;
                    fade.color = Color.Lerp(fade.color, col, t);
                    yield return new WaitForSeconds(0.01f);
                }
                break;

            case ev_details.EVENT_TYPES.WAIT:

                yield return new WaitForSeconds(current_ev.float0);

                break;

            case ev_details.EVENT_TYPES.OBJECT:

                foreach (string str in current_ev.stringList)
                {
                    GameObject o = GameObject.Find(str);
                    if (o == null)
                        continue;
                    s_object ob = o.GetComponent<s_object>();
                    if (ob == null)
                        continue;

                    if (current_ev.boolean)
                    {
                        ob.DespawnObject();
                    }
                    else
                    {
                        o.SetActive(true);
                    }
                }
                break;
                
            case ev_details.EVENT_TYPES.DISPLAY_CHARACTER_HEALTH:

                bool check = current_ev.stringList.Length < 2;
                if (current_ev.boolean)
                {

                    if (check)
                    {
                        if (current_ev.boolean)
                            s_gui.AddCharacter(GameObject.Find(current_ev.stringList[0]).GetComponent<o_character>(), false);
                        else
                            s_gui.AddCharacter(GameObject.Find(current_ev.stringList[0]).GetComponent<o_character>(), true);
                    }
                    else
                    {
                        List<o_character> cha = new List<o_character>();
                        foreach (string st in current_ev.stringList)
                        {
                            cha.Add(GameObject.Find(st).GetComponent<o_character>());
                        }
                        s_gui.AddCharacter(cha);
                    }

                }
                break;

            case ev_details.EVENT_TYPES.END_EVENT:

                GameObject cam = GameObject.Find("Main Camera");
                cam.GetComponent<s_camera>().focus = true;
                if (selobj)
                {
                    if (selobj.GetComponent<o_plcharacter>())
                    {
                        selobj.GetComponent<o_plcharacter>().control = true;
                    }
                }
                if (current_ev.boolean)
                {
                    string[] cha = new string[characters.Length];

                    int ind = 0;
                    foreach (o_character chara in characters)
                    {
                        cha[ind] = chara.name;
                        ind++;
                    }
                    s_utility u = GetComponent<s_utility>();
                    u_boundary u_b = GetComponent<u_boundary>();
                    if (u != null)
                    {
                        string uti = u.GetType().ToString();
                        s_leveledit.LevEd.savedtriggerdatalist.Add(new s_map.s_trig(name, transform.position, Events.ev_Details, uti, u_b.bounds, cha, current_trigger.TRIGGER_T, current_trigger.collision.size, true));
                    }
                    else
                        s_leveledit.LevEd.savedtriggerdatalist.Add(new s_map.s_trig(name, transform.position, Events.ev_Details, current_trigger.TRIGGER_T, current_trigger.collision.size, true));
                }
                print("Ended event.");
                doingEvents = false;

                if (current_trigger != null) {
                    current_trigger.DespawnObject();
                }
                QueuedDetails.Dequeue();
                break;

            case ev_details.EVENT_TYPES.SET_FLAG:
                s_globals.SetGlobalFlag(current_ev.string0, current_ev.int0);
                break;

            case ev_details.EVENT_TYPES.CHOICE:
                int choice = 0, finalchoice = -1;
                print(choice);
                while (finalchoice == -1)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        choice--;

                    if (Input.GetKeyDown(KeyCode.DownArrow))
                        choice++;

                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        print("Chosen");
                        finalchoice = choice;
                    }
                    Dialogue.textthing.text = "";
                    Dialogue.textthing.text += current_ev.string0 + "\n";
                    for (int i = 0; i < current_ev.stringList.Length; i++)
                    {
                        if (choice == i)
                            Dialogue.textthing.text += "-> ";

                        Dialogue.textthing.text += current_ev.stringList[i] + "\n";
                    }
                    print(choice);
                    choice = Mathf.Clamp(choice, 0, current_ev.stringList.Length - 1);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                Dialogue.textthing.text = "";
                current_trigger.ev_num = current_ev.intList[finalchoice] - 1;
                break;
        }
        //yield return new WaitForSeconds(0.5f);
    }
}
*/
