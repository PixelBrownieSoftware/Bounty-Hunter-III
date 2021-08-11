using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;
using UnityEngine.UI;

public class s_trig : s_triggerhandler
{
    Image fade;
    const float shutterdepth = 1.55f;
    string current_label;
    bool first_move_event = true;
    public Texture bossHPBar;
    public Texture bossHPBarFull;
    public BHIII_character selectedChara;
    BHIII_globals gl;

    ll_BHIII leveled;

    bool activated_shutters = false;

    new private void Awake()
    {
        trig = this;
        base.Awake();
        textBox.gameObject.SetActive(false);
        gl = GetComponent<BHIII_globals>();
    }

    public override IEnumerator EventPlay()
    {
        switch ((EVENT_TYPES)current_ev.eventType)
        {
            default:
                yield return StartCoroutine(base.EventPlay());
                break;

            /*
        case EVENT_TYPES.MOVEMNET:
            yield return StartCoroutine(base.EventPlay());
            if (selectedChara == null)
            {
                selectedChara = selobj.GetComponent<BHIII_character>();
            } else {
                selectedChara.AnimMove();
            }
            break;
            */

            case EVENT_TYPES.CUSTOM_FUNCTION:

                switch (current_ev.funcName) {
                    case "DISPLAY_CHARACTER_HEALTH":
                        if (current_ev.string0 != "") {
                            if (gl.bossChar == null)
                                gl.bossChar = new List<o_character>();
                            gl.bossChar.Add(GameObject.Find(current_ev.string0).GetComponent<o_character>());
                        }
                        gl.bossDisplayOn = current_ev.boolean;
                        if (!current_ev.boolean)
                            gl.bossChar.Clear();
                        break;

                    case "ZOOM_IN":
                        s_camera.cam.GetComponent<Camera>().orthographicSize = 180;
                        break;

                    case "ZOOM_OUT":
                        s_camera.cam.GetComponent<Camera>().orthographicSize = 180 * 1.5f;
                        break;

                    case "PLAY_ENDING":
                        gl.GameEnd();
                        break;

                }
                break;

            case EVENT_TYPES.DISPLAY_CHARACTER_HEALTH:
                if(current_ev.string0 != "")
                    gl.bossChar.Add(GameObject.Find(current_ev.string0).GetComponent<o_character>());
                gl.bossDisplayOn = current_ev.boolean;
                break;

            case EVENT_TYPES.UTILITY_INITIALIZE:
                s_utility ut = GameObject.Find(current_ev.string0).GetComponent<s_utility>();

                if (ut.GetComponent<s_utility>() != null)
                {
                    if (ut.eventState == 0)
                    {
                        //o_plcharacter player = GameObject.Find("Player").GetComponent<o_plcharacter>();
                        //player.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_MOVING;

                        ut.eventState = 1;
                    }
                }
                else
                {
                    break;
                }
                break;
            /*

            */


            case EVENT_TYPES.CHECK_FLAG:
                int integr = s_globals.GetGlobalFlag(current_ev.string0);
                int labelNum = FindLabel(current_ev.string1);

                switch ((LOGIC_TYPE)current_ev.logic)
                {
                    default:
                        yield return StartCoroutine(base.EventPlay());
                        break;

                    case LOGIC_TYPE.VAR_NOT_EQUAL:
                        if (integr != current_ev.int0)
                            pointer = labelNum;   //Label to jump to
                        break;
                }
                break;
                /*
            case EVENT_TYPES.PUT_SHUTTERS:

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
                else
                {

                    for (int i = 0; i < 30; i++)
                    {
                        sh1.rectTransform.position += new Vector3(0, shutterdepth);
                        sh2.rectTransform.position += new Vector3(0, -shutterdepth);
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                }
                break;

            case EVENT_TYPES.ANIMATION:

                Animator character = selobj.GetComponent<Animator>();
                character.Play(current_ev.int0);
                character.speed = current_ev.float0;
                break;

            case EVENT_TYPES.CHANGE_MAP:

                s_globals.SaveData();
                ll_BHIII led = GameObject.Find("General").GetComponent<ll_BHIII>();
                if (current_ev.string0 == "" || current_ev.string1 == null)
                    led.TriggerSpawn(current_ev.string1);
                else
                    led.TriggerSpawn(current_ev.string0, current_ev.string1);

                selobj.GetComponent<o_plcharacter>().control = true;
                break;   

            case EVENT_TYPES.DISPLAY_CHARACTER_HEALTH:
                if (!current_ev.boolean)
                    s_gui.AddToBossHPList(GameObject.Find(current_ev.string0).GetComponent<o_character>());
                else
                    s_gui.ClearBossHPList();
                break;
                
            case EVENT_TYPES.DIALOGUE:
                Dialogue.done_event = false;
                Dialogue.automatic = current_ev.boolean;
                StartCoroutine(Dialogue.DisplayDialogue(current_ev.string0));
                while (!Dialogue.done_event)
                {
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                break;

            case EVENT_TYPES.ADD_SHOP_ITEM:

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

            case EVENT_TYPES.ADD_INVENTORY:
                //s_globals.AddItem(new o_item(current_ev.string0, (o_item.ITEM_TYPE)current_ev.int1));
                break;
                
            case EVENT_TYPES.RUN_CHARACTER_SCRIPT:

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
                else
                {
                    foreach (string characternam in current_ev.stringList)
                    {
                        GameObject.Find(characternam).GetComponent<o_character>().control = true;
                    }
                }
                break;
            case EVENT_TYPES.CAMERA_MOVEMENT:

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
                else
                {

                    float dista = Vector2.Distance(ca.transform.position, new Vector3(current_ev.pos.x, current_ev.pos.y, -15));
                    Vector2 pos = new Vector3(current_ev.pos.x, current_ev.pos.y, -15);
                    bool b = ca.CameraLerp();
                    while (!b)
                    {
                        b = ca.CameraLerp();
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                }
                break;
                
            case EVENT_TYPES.CHECK_FLAG:
                int integr = s_globals.GetGlobalFlag(current_ev.string0);

                switch ((LOGIC_TYPE)current_ev.logic)
                {
                    case LOGIC_TYPE.ITEM_OWNED:
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

                    case LOGIC_TYPE.VAR_EQUAL:
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

                    case LOGIC_TYPE.VAR_GREATER:
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


                    case LOGIC_TYPE.VAR_LESS:
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
                
            case EVENT_TYPES.UTILITY_FOCUS:
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

            case EVENT_TYPES.DISPLAY_ICON:
                
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
            case EVENT_TYPES.TELEPORT:

                if (current_ev.boolean)
                {
                    ca = GameObject.Find("Main Camera").GetComponent<s_camera>();
                    ca.focus = false;
                    ca.transform.position = new Vector3(current_ev.pos.x, current_ev.pos.y);
                }
                else
                {
                }

                break;

            case EVENT_TYPES.SET_HEALTH:
                
                c = GameObject.Find(current_ev.string0).GetComponent<o_character>();
                if (c != null)
                    c.health = current_ev.float0;
                break;


            case EVENT_TYPES.UTILITY_CHECK:

                //Make this like the conditional statements where it checks if the utility is still active

                break;
                
            case EVENT_TYPES.UTILITY_INITIALIZE:
                s_utility ut;

                o_plcharacter player = GameObject.Find("Player").GetComponent<o_plcharacter>();
                player.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_MOVING;

                if (current_trigger.GetComponent<s_utility>() != null)
                {
                    ut = current_trigger.GetComponent<s_utility>();
                    ut.istriggered = true;

                    selobj.GetComponent<o_plcharacter>().control = true;
                    current_trigger.isactive = false;
                }
                else
                {
                    break;
                }
                break;

            case EVENT_TYPES.FADE:

                Color col = new Color(current_ev.colour.a, current_ev.colour.g, current_ev.colour.b, current_ev.colour.a);
                float t = 0;

                while (fade.color != col)
                {
                    t += Time.deltaTime;
                    fade.color = Color.Lerp(fade.color, col, t);
                    yield return new WaitForSeconds(0.01f);
                }
                break;

            case EVENT_TYPES.WAIT:

                yield return new WaitForSeconds(current_ev.float0);

                break;

            case EVENT_TYPES.OBJECT:

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
                
                

            case EVENT_TYPES.SET_FLAG:
                s_globals.SetGlobalFlag(current_ev.string0, current_ev.int0);
                break;
            case EVENT_TYPES.CHOICE:
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
                */
        }
        //yield return new WaitForSeconds(0.5f);
    }

    private void OnGUI()
    {
    }

    private new void Update()
    {
        base.Update();
        if (Events.Count == 0)
        {
            print("FUCKING HELL!!!");
            if (GameObject.Find("MapObject") != null)
                GetMapEvents(null, GameObject.Find("MapObject").GetComponent<s_mapEventholder>().Events);
        }
    }

}
 