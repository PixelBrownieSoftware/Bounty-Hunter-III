using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MagnumFoudation;
using MagnumFoundationEditor;

public class bhIII_cutscene : ed_cutscene
{
    Color evcolour;
    List<string> locations;
    List<string> posLocation;
    s_object objectItem;
    ll_BHIII ed;
    o_character selectedCharacter = null;
    //Vector2 pos;

        /*
    Vector2 TeleporterPos(string n, string t)
    {
        List<string> maploc = new List<string>();
        TextAsset te = ed.jsonMaps.Find(x => x.name == n);

        s_map m = JsonUtility.FromJson<s_map>(te.text);

        s_map.s_tileobj til = m.tilesdata.Find(x => x.TYPENAME == "teleport_object" && x.name == t);
        if (til != null)
            return new Vector2(til.pos_x, til.pos_y);
        return new Vector2(0, 0);
    }
    */

    void SetStringToObjectName(ref ev_details det)
    {
        objectItem = (s_object)EditorGUILayout.ObjectField(objectItem, typeof(s_object), true);
        if (GUILayout.Button("Set string to object"))
        {
            if (objectItem != null)
                det.string0 = objectItem.name;
        }
    }

    string GetEntityName()
    {
        if (ed != null) {
            if (ed.entitiesObj != null) {
                o_character[] entities = ed.entitiesObj.GetComponentsInChildren<o_character>();
                foreach (o_character c in entities) {
                    if (GUILayout.Button(c.name)) {
                        return c.name;
                    }
                }
            }
        } else {
            return "";
        }
        return "";
    }

    public override void DisplayCutsceneEditor(int index, ref Rect rect, ref SerializedProperty prp)
    {
        base.DisplayCutsceneEditor(index, ref rect, ref prp);
        switch ((EVENT_TYPES)prp.FindPropertyRelative("eventType").intValue) {

            default:
                base.DisplayCutsceneEditor(index, ref rect, ref prp);
                break;
                /*
            case EVENT_TYPES.DISPLAY_CHARACTER_HEALTH:

                EditorGUILayout.LabelField("Character's health to display: " + ev.string0);

                //DrawProperty()

                if (ev.string0 == "")
                    ev.string0 = GetEntityName();
                else
                {
                    if (GUILayout.Button("None"))
                        ev.string0 = "";
                }

                EditorGUILayout.LabelField("Show health?");
                ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                break;
                */
        }
    }
    }


/*
List<string> LocationsMap()
{
    if (ed == null)
        return null;
    if (ed.jsonMaps == null)
        return null;
    List<TextAsset> te = ed.jsonMaps;

    List<string> maploc = new List<string>();
    for (int i = 0; i < te.Count; i++)
    {
        maploc.Add(te[i].name);
    }
    return maploc;
}

List<string> PositionsOnMap(string n)
{
    List<string> maploc = new List<string>();
    TextAsset te = ed.jsonMaps.Find(x => x.name == n);

    s_map m = JsonUtility.FromJson<s_map>(te.text);

    for (int i = 0; i < m.tilesdata.Count; i++)
    {
        //print((s_map.s_mapteleport)mapdat.tilesdata[i]);
        switch (m.tilesdata[i].TYPENAME)
        {
            case "teleport_object":
                maploc.Add(m.tilesdata[i].name);
                break;
        }
    }
    return maploc;
}
*/
/*
public override void EnumChange(ref ev_details ev)
{
    ev.eventType = (int)(EVENT_TYPES)EditorGUILayout.EnumPopup((EVENT_TYPES)ev.eventType);
}
*/
/*
public override string DisplayCode(int eventType, ev_details d)
{
string str = "";
switch ((EVENT_TYPES)d.eventType)
{
    case EVENT_TYPES.LABEL:
        str = "Label: " + d.string0;
        break;
    case EVENT_TYPES.MOVEMNET:
        str = "    Move to position " + d.float0 + ", " + d.float1;
        break;
    case EVENT_TYPES.BREAK_EVENT:
        str = "    End event";
        break;
    case EVENT_TYPES.OBJECT:

        string dele = "";
        if (d.boolean)
        {
            dele = "delete";
        }
        str = "    OBJ " + " mode: " + d.boolean + ", location: " + d.string1;
        break;
    case EVENT_TYPES.CHANGE_MAP:
        str = "    Change current map to " + "map  '" + d.string0 + "', to location '" + d.string1 + "'";
        break;
    case EVENT_TYPES.SET_FLAG:
        str = "    Set flag '" + d.string0 + "' to " + d.int0;
        break;
    case EVENT_TYPES.CHECK_FLAG:

        str = "    Check flag '" + d.string0 + "', for value" + d.int0;

        switch ((LOGIC_TYPE)d.logic)
        {
            case LOGIC_TYPE.VAR_EQUAL:
                EditorGUILayout.LabelField("            If equal, jump to point " + d.int1 + "\n");
                break;
            case LOGIC_TYPE.VAR_GREATER:
                EditorGUILayout.LabelField("            If greater than, jump to point " + d.int1 + "\n");
                break;
            case LOGIC_TYPE.VAR_LESS:
                EditorGUILayout.LabelField("            If less than, jump to point " + d.int1 + "\n");
                break;
            case LOGIC_TYPE.CHECK_UTILITY_RETURN_NUM:
                EditorGUILayout.LabelField("            If utility '" + d.string0 + "' number is equal, jump to point " + d.int1 + "\n");
                break;
            case LOGIC_TYPE.CHECK_CHARACTER:
                EditorGUILayout.LabelField("            If chracter is " + d.string0 + "jump to label " + d.int1 + "\n");
                break;
            case LOGIC_TYPE.CHECK_CHARACTER_NOT:
                EditorGUILayout.LabelField("            If chracter is not " + d.string0 + "jump to label " + d.int1 + "\n");
                break;
        }
        break;

    case EVENT_TYPES.UTILITY_INITIALIZE:
        str = "    Initialize utility '" + d.string0 + "'";
        break;
    case EVENT_TYPES.SET_UTILITY_FLAG:
        str = "    Set utility '" + d.string0 + "' flag to " + d.int0;
        break;
    case EVENT_TYPES.UTILITY_CHECK:
        str = "    Check if utility '" + d.string0 + "' event state is " + d.int0;
        break;
}
return str;
}
*/
/*
[CustomEditor(typeof(o_trigger))]
public class ed_cutscene : EditorWindow
{
    public bool [] foldoutlist = null;
    s_object[] objlist = null;
    s_object objectItem = null;
    o_character selectedCharacter = null;
    Vector2 scrollview;
    Vector2 scrollview2;    //Used for individual events 
    TextAsset utilityobj;
    TextAsset textobj;
    Vector2 mousepos;
    GameObject mouseArea;
    Vector2 dialogue_scroll;
    int leng;
    Vector2 pos;
    Color evcolour; Event e;

    Tool lasttool = Tool.None;

    //Pixel's Outstandingly Ultumate Cutscene Handler
    [MenuItem("Brownie/POUCH")]
    static void init()
    {
        GetWindow<ed_cutscene>("POUCH");
    }

    private void OnEnable()
    {
        if (mouseArea == null)
            mouseArea = GameObject.Find("Gizmo");


        SceneView.onSceneGUIDelegate += SceneGUI;
        lasttool = Tools.current;
    }
    void SceneGUI(SceneView sv)
    {
        e = Event.current;
        if (e.keyCode == KeyCode.S)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            pos = ray.origin;
        }
    }

    private void OnGUI()
    {
        Event eventI = Event.current;

        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {
            o_trigger trig = obj.GetComponent<o_trigger>();
            ev_details[] details = new ev_details[0];
            o_collidableobject collider = obj.GetComponent<o_collidableobject>();

            if (trig != null)
            {
                trig.GetComponent<BoxCollider2D>().size = EditorGUILayout.Vector2Field("Trigger size: ", trig.GetComponent<BoxCollider2D>().size);
                details = trig.Events.ev_Details;


                    if (foldoutlist == null)
                    foldoutlist = new bool[details.Length];

                scrollview = EditorGUILayout.BeginScrollView(scrollview);

                #region UTILITY STUFF
                if (trig.gameObject.GetComponent<s_utility>() == null)
                {
                    utilityobj = (TextAsset)EditorGUILayout.ObjectField(utilityobj, typeof(TextAsset), true);
                    if (GUILayout.Button("ADD UTILITY"))
                    {
                        if (utilityobj != null)
                        {
                            string objtype = utilityobj.name;
                            switch (objtype)
                            {
                                case "u_shop":
                                    trig.gameObject.AddComponent<u_shop>();
                                    break;
                                case "u_boundary":
                                    trig.gameObject.AddComponent<u_boundary>();
                                    break;
                                case "u_save":
                                    trig.gameObject.AddComponent<u_save>();
                                    break;
                                case "u_spawner":
                                    trig.gameObject.AddComponent<u_spawner>();
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("REMOVE UTILITY"))
                    {
                        s_utility util = trig.gameObject.GetComponent<s_utility>();
                        DestroyImmediate(util);
                    }
                }
                #endregion
                #region ADD EVENT
                if (GUILayout.Button("+"))
                {
                    List<ev_details> evobj = new List<ev_details>();
                    for (int i = 0; i < details.Length; i++)
                    {
                        evobj.Add(details[i]);
                    }
                    evobj.Add(new ev_details());
                    trig.Events.ev_Details = evobj.ToArray();

                    foldoutlist = new bool[trig.Events.ev_Details.Length];
                }
                #endregion

                DrawDetailsTrigger(trig);
                EditorGUILayout.EndScrollView();
            }

            if (collider != null)
            {
                EditorGUILayout.LabelField("Original position" + collider.transform.position);
                for (int i = 0; i < collider.movepositions.Count; i++)
                {
                    Vector2 vec = collider.movepositions[i];
                    collider.movepositions[i] = EditorGUILayout.Vector2Field("Position: ", collider.movepositions[i]);
                    if (GUILayout.Button("Remove position"))
                    {
                        collider.movepositions.Remove(vec);
                    }
                }
                if (GUILayout.Button("Add new position"))
                {
                    if(collider.movepositions.Count == 0)
                        collider.movepositions.Add(collider.transform.position);
                    else
                        collider.movepositions.Add(collider.movepositions[collider.movepositions.Count - 1]);

                }

            }
        }
        else
            foldoutlist = null;
    }

    void DrawDetailsTrigger(o_trigger trig)
    {
        int EVNUM = 0;
        ev_details[] details = trig.Events.ev_Details;
        for (int i = 0; i < details.Length; i++)
        {
            ev_details ev = details[i];
            if (ev != null)
            {
                EditorGUILayout.BeginHorizontal();
                foldoutlist[i] = EditorGUILayout.Foldout(foldoutlist[i], "Event Numeber: " + EVNUM, true);

                #region REMOVE EVENT
                if (GUILayout.Button("-"))
                {
                    ev_details[] evobj = new ev_details[trig.Events.ev_Details.Length - 1];
                    int index = 0; //For the index of the new object above
                    for (int e = 0; e < trig.Events.ev_Details.Length; e++)
                    {
                        if (e == i)
                            continue;
                        evobj[index] = trig.Events.ev_Details[e];
                        index++;
                    }
                    trig.Events.ev_Details = evobj;
                    foldoutlist = new bool[trig.Events.ev_Details.Length];
                }
                #endregion

                #region MOVE EVENT

                if (0 < i)
                {
                    if (GUILayout.Button("^"))
                    {
                        ev_details[] evobj = new ev_details[trig.Events.ev_Details.Length];
                        int index = i - 1; //For the index of the new object above

                        ev_details swapobj2 = null;
                        ev_details swapobj = null;

                        for (int e = 0; e < trig.Events.ev_Details.Length; e++)
                        {
                            if (e == i)
                            {
                                swapobj = trig.Events.ev_Details[e];
                                continue;
                            }
                            if (e == index)
                            {
                                swapobj2 = trig.Events.ev_Details[e];
                                continue;
                            }
                            evobj[e] = trig.Events.ev_Details[e];
                        }
                        evobj[index] = swapobj;
                        evobj[i] = swapobj2;

                        trig.Events.ev_Details = evobj;
                        foldoutlist = new bool[trig.Events.ev_Details.Length];
                    }
                }
                if (trig.Events.ev_Details.Length > i + 1)
                {

                    if (GUILayout.Button("v"))
                    {
                        ev_details[] evobj = new ev_details[trig.Events.ev_Details.Length];
                        int index = i + 1; //For the index of the new object above

                        ev_details swapobj2 = null;
                        ev_details swapobj = null;

                        for (int e = 0; e < trig.Events.ev_Details.Length; e++)
                        {
                            if (e == i)
                            {
                                swapobj = trig.Events.ev_Details[e];
                                continue;
                            }
                            if (e == index)
                            {
                                swapobj2 = trig.Events.ev_Details[e];
                                continue;
                            }
                            evobj[e] = trig.Events.ev_Details[e];
                        }
                        evobj[index] = swapobj;
                        evobj[i] = swapobj2;

                        trig.Events.ev_Details = evobj;
                        foldoutlist = new bool[trig.Events.ev_Details.Length];
                    }
                }
                #endregion

                ev.simultaneous = EditorGUILayout.Toggle("Simultaneous?", ev.simultaneous);

                ev.eventType = (ev_details.EVENT_TYPES)EditorGUILayout.EnumPopup(ev.eventType);

                EditorGUILayout.EndHorizontal();
            }

            if (foldoutlist[i])
            {
                switch (ev.eventType)
                {
                    #region MOVE
                    case ev_details.EVENT_TYPES.MOVEMNET:

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Teleport?");
                        ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                        if (!ev.boolean)
                        {
                            EditorGUILayout.LabelField("Character name");
                            ev.string0 = EditorGUILayout.TextArea(ev.string0);
                            ev.dir = (ev_details.DIRECTION)EditorGUILayout.EnumPopup("Direction: ", ev.dir);
                            ev.float0 = EditorGUILayout.FloatField("Distance", ev.float0);
                            ev.float1 = EditorGUILayout.FloatField("Speed", ev.float1);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Character name");
                            ev.string0 = EditorGUILayout.TextArea(ev.string0);
                            ev.float0 = EditorGUILayout.FloatField("X", ev.float0);
                            ev.float1 = EditorGUILayout.FloatField("Y", ev.float1);
                        }
                        break;
                    #endregion

                    #region DIALOGUE
                    case ev_details.EVENT_TYPES.DIALOGUE:
                        EditorGUILayout.LabelField("Name of map");
                        textobj = (TextAsset)EditorGUILayout.ObjectField(utilityobj, typeof(TextAsset), true);
                        ev.string0 = EditorGUILayout.TextArea(ev.string0);
                        EditorGUILayout.LabelField("Is automatic");
                        ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                        if (GUILayout.Button("Open Level file"))
                        {
                            string levelload = EditorUtility.OpenFilePanel("Open Json level file", "Assets/Levels/", "");
                            if (levelload != null)
                            {
                                // = textobj.text;
                            }
                        }
                        EditorGUILayout.LabelField("Syntax:");

                        EditorGUILayout.LabelField("Text delay: <pwait=[time]>");
                        EditorGUILayout.LabelField("Text colour: <pcol=#[8 hexdec value]>");
                        EditorGUILayout.LabelField("Text italics: <pita>");
                        EditorGUILayout.LabelField("End line: <endlin>");
                        EditorGUILayout.LabelField("Text speed: <ptxspd=[float]>, default = 0.5");
                        EditorGUILayout.LabelField("Icon display: <pcharsel=[name of icon]>");

                        break;
                    #endregion

                    #region CHECK FLAG
                    case ev_details.EVENT_TYPES.CHECK_FLAG:
                        ev.logic = (ev_details.LOGIC_TYPE)EditorGUILayout.EnumPopup("Logic Type", ev.logic);
                        switch (ev.logic)
                        {
                            case ev_details.LOGIC_TYPE.VAR_EQUAL:
                                EditorGUILayout.BeginHorizontal();

                                EditorGUILayout.LabelField("IF ");
                                ev.string0 = EditorGUILayout.TextField(ev.string0);
                                EditorGUILayout.LabelField(" = ");
                                ev.int0 = EditorGUILayout.IntField(ev.int0);
                                EditorGUILayout.Space();
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("THEN JUMP TO ");
                                ev.int1 = EditorGUILayout.IntField(ev.int1);
                                EditorGUILayout.EndHorizontal();
                                break;

                            case ev_details.LOGIC_TYPE.VAR_GREATER:

                                EditorGUILayout.BeginHorizontal();

                                EditorGUILayout.LabelField("IF ");
                                ev.string0 = EditorGUILayout.TextField(ev.string0);
                                EditorGUILayout.LabelField(" > ");
                                ev.int0 = EditorGUILayout.IntField(ev.int0);
                                EditorGUILayout.Space();
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("THEN JUMP TO ");
                                ev.int1 = EditorGUILayout.IntField(ev.int1);
                                EditorGUILayout.EndHorizontal();
                                break;

                            case ev_details.LOGIC_TYPE.VAR_LESS:

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("IF ");
                                ev.string0 = EditorGUILayout.TextField(ev.string0);
                                EditorGUILayout.LabelField(" < ");
                                ev.int0 = EditorGUILayout.IntField(ev.int0);
                                EditorGUILayout.Space();
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("THEN JUMP TO ");
                                ev.int1 = EditorGUILayout.IntField(ev.int1);
                                EditorGUILayout.EndHorizontal();
                                break;

                            case ev_details.LOGIC_TYPE.ITEM_OWNED:

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("IF "); EditorGUILayout.LabelField("ITEM ");
                                ev.string0 = EditorGUILayout.TextField(ev.string0);
                                EditorGUILayout.LabelField(" WITH TYPE ");
                                ev.int0 = EditorGUILayout.IntField(ev.int0);
                                EditorGUILayout.LabelField(" (" + (o_item.ITEM_TYPE)ev.int0 + ")");
                                EditorGUILayout.LabelField(" POSSESED.");
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("THEN JUMP TO ");
                                ev.int1 = EditorGUILayout.IntField(ev.int1);
                                EditorGUILayout.EndHorizontal();
                                break;
                        }
                        break;
                    #endregion

                    #region SET VAR
                    case ev_details.EVENT_TYPES.SET_FLAG:

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("SET VARIABLE ");
                        ev.string0 = EditorGUILayout.TextField(ev.string0);
                        EditorGUILayout.LabelField(" TO ");
                        ev.int0 = EditorGUILayout.IntField(ev.int0);
                        EditorGUILayout.EndHorizontal();
                        break;
                    #endregion

                    #region SHUTTERS
                    case ev_details.EVENT_TYPES.PUT_SHUTTERS:

                        EditorGUILayout.LabelField("Shut down");
                        ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                        break;
                    #endregion

                    #region CHANGE_MAP
                    case ev_details.EVENT_TYPES.CHANGE_MAP:
                        EditorGUILayout.LabelField("Name of map");

                        ev.string0 = EditorGUILayout.TextArea(ev.string0);
                        s_leveledit mapdat = GameObject.Find("General").GetComponent<s_leveledit>();

                        if (mapdat != null)
                        {
                            TextAsset t = mapdat.jsonMaps.Find(x => x.name == ev.string0);
                            if (t == null)
                            {
                                EditorGUILayout.LabelField("Map was not found.");
                                break;
                            }
                            s_map m = JsonUtility.FromJson<s_map>(t.text);
                            List<s_map.s_tileobj> tilesInMap = m.tilesdata;

                            EditorGUILayout.LabelField("Teleportation areas:");
                            for (int o = 0; o < tilesInMap.Count; o++)
                            {
                                //print((s_map.s_mapteleport)mapdat.tilesdata[i]);
                                if (tilesInMap[o].TYPENAME == "teleport_object")
                                {
                                    EditorGUILayout.TextField(tilesInMap[o].name);
                                }
                            }
                        }
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Name of teleportation");
                        ev.string1 = EditorGUILayout.TextArea(ev.string1);

                        break;
                    #endregion

                    #region BREAK EVENT
                    case ev_details.EVENT_TYPES.BREAK_EVENT:

                        EditorGUILayout.LabelField("Reset cutscene?");
                        ev.boolean = EditorGUILayout.Toggle(ev.boolean);

                        break;
                    #endregion

                    #region END_EVENT
                    case ev_details.EVENT_TYPES.END_EVENT:

                        EditorGUILayout.LabelField("Permanement disable?");
                        ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                        break;
                    #endregion

                    #region FADE
                    case ev_details.EVENT_TYPES.FADE:

                        EditorGUILayout.LabelField("Colour of this part");
                        EditorGUILayout.ColorField(new Color(ev.colour.r, ev.colour.g, ev.colour.b, ev.colour.a));
                        EditorGUILayout.LabelField("New Colour of this part");
                        evcolour = EditorGUILayout.ColorField(evcolour);

                        if (GUILayout.Button("Set Colour"))
                        {
                            ev.colour = new ev_details.s_save_colour(evcolour);
                        }

                        break;
                    #endregion

                    #region OBJECT

                    case ev_details.EVENT_TYPES.OBJECT:

                        leng = EditorGUILayout.IntField(leng);
                        if (GUILayout.Button("New list"))
                        {
                            ev.stringList = new string[leng];
                            objlist = new s_object[leng];
                        }

                        if (ev.stringList != null || ev.stringList.Length > 0)
                        {
                            if (objlist == null)
                                objlist = new s_object[leng];
                            for (int o = 0; o < ev.stringList.Length; o++)
                            {
                                objlist[o] = (o_character)EditorGUILayout.ObjectField(objlist[o], typeof(o_character), true);

                                if (objlist[o] != null)
                                    ev.stringList[o] = objlist[o].name;
                            }
                            for (int o = 0; o < ev.stringList.Length; o++)
                            {
                                EditorGUILayout.LabelField(ev.stringList[o]);
                            }
                        }
                        EditorGUILayout.LabelField("Hide object?");
                        ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                        break;
                    #endregion

                    #region CAMERA MOVEMENT
                    case ev_details.EVENT_TYPES.CAMERA_MOVEMENT:

                        EditorGUILayout.LabelField("Focus on character?");
                        ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                        EditorGUILayout.LabelField("Stay on character");
                        ev.boolean1 = EditorGUILayout.Toggle(ev.boolean1);

                        EditorGUILayout.LabelField("Speed");
                        ev.float0 = EditorGUILayout.FloatField(ev.float0);
                        if (!ev.boolean)
                        {
                            EditorGUILayout.LabelField("Position: " + ev.pos.x + ", " + ev.pos.y);

                            if (GUILayout.Button("Set Pos"))
                                ev.pos = new s_map.s_save_vector(pos);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Character to focus on.");
                            ev.string0 = EditorGUILayout.TextArea(ev.string0);
                        }

                        break;
                    #endregion

                    #region TELEPORT
                    case ev_details.EVENT_TYPES.TELEPORT:

                        EditorGUILayout.LabelField("Position: " + ev.pos.x + ", " + ev.pos.y);

                        if (GUILayout.Button("Set Pos"))
                            ev.pos = new s_map.s_save_vector(pos);

                        EditorGUILayout.LabelField("Camera?");
                        ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                        if (!ev.boolean)
                        {
                            objectItem = (o_character)EditorGUILayout.ObjectField(objectItem, typeof(o_character), true);
                            if (GUILayout.Button("Set string to object"))
                            {
                                if (objectItem != null)
                                    ev.string0 = objectItem.name;
                            }
                            EditorGUILayout.LabelField("Character to teleport: " + ev.string0);
                        }
                        break;
                    #endregion

                    #region DISPLAY CHARACTER HEALTH
                    case ev_details.EVENT_TYPES.DISPLAY_CHARACTER_HEALTH:

                        objectItem = (o_character)EditorGUILayout.ObjectField(objectItem, typeof(o_character), true);

                        if (GUILayout.Button("Set health to boss"))
                        {
                            if (objectItem != null)
                                ev.string0 = objectItem.name;
                        }
                        EditorGUILayout.LabelField("Health to show: " + ev.string0);
                        ev.boolean = EditorGUILayout.Toggle("Clear?", ev.boolean);


                        break;
                    #endregion

                    #region WAIT
                    case ev_details.EVENT_TYPES.WAIT:

                        ev.float0 = EditorGUILayout.FloatField("Length: ", ev.float0);

                        break;

                    #endregion

                    #region SET_HEALTH
                    case ev_details.EVENT_TYPES.SET_HEALTH:

                        selectedCharacter = (o_character)EditorGUILayout.ObjectField(selectedCharacter, typeof(o_character), true);
                        if (selectedCharacter != null)
                            ev.string0 = selectedCharacter.name;
                        ev.float0 = EditorGUILayout.FloatField("Set health to: ", ev.float0);
                        break;
                    #endregion

                    #region CHOICE 
                    case ev_details.EVENT_TYPES.CHOICE:
                        ev.string0 = EditorGUILayout.TextField("Question label:", ev.string0);
                        leng = EditorGUILayout.IntField(leng);
                        if (GUILayout.Button("New list"))
                        {
                            ev.stringList = new string[leng];
                            ev.intList = new int[leng];
                        }
                        if (ev.stringList != null)
                        {
                            //EditorGUILayout.BeginScrollView(scrollview2);
                            for (int o = 0; o < ev.stringList.Length; o++)
                            {
                                ev.stringList[o] = EditorGUILayout.TextField("Name of choice" + o + " :", ev.stringList[o]);
                                ev.intList[o] = EditorGUILayout.IntField("Event position to jump to: ", ev.intList[o]);
                                EditorGUILayout.Space();
                            }
                            //EditorGUILayout.EndScrollView();

                            for (int o = 0; o < ev.stringList.Length; o++)
                            {
                                EditorGUILayout.LabelField(ev.stringList[o]);
                                EditorGUILayout.LabelField("" + ev.intList[o]);
                            }
                        }
                        break;
                    #endregion

                    #region ACTIVATE_SCRIPT
                    case ev_details.EVENT_TYPES.RUN_CHARACTER_SCRIPT:

                        EditorGUILayout.LabelField("Run which character's script?");
                        EditorGUILayout.Space();
                        objectItem = (o_character)EditorGUILayout.ObjectField(objectItem, typeof(o_character), true);
                        if (GUILayout.Button("Set string to object"))
                        {
                            if (objectItem != null)
                                ev.string0 = objectItem.name;
                        }
                        EditorGUILayout.LabelField("Character name: " + ev.string0);
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Enable character? (check for yes)");
                        ev.boolean1 = EditorGUILayout.Toggle(ev.boolean1);

                        break;
                    #endregion

                    case ev_details.EVENT_TYPES.DISPLAY_ICON:

                        EditorGUILayout.LabelField("Icon name: ");
                        ev.string0 = EditorGUILayout.TextArea(ev.string0);
                        EditorGUILayout.LabelField("Icon position: X: " + ev.int0 + ", " + ev.int1);
                        ev.int0 = EditorGUILayout.IntField(ev.int0);
                        ev.int1 = EditorGUILayout.IntField(ev.int1);
                        break;

                    #region ADD SHOP ITEM
                    case ev_details.EVENT_TYPES.ADD_SHOP_ITEM:

                        ev.int0 = EditorGUILayout.IntField(ev.int0);
                        ev.int1 = EditorGUILayout.IntField(ev.int1);
                        EditorGUILayout.LabelField(((o_item.ITEM_TYPE)ev.int1).ToString());
                        ev.string0 = EditorGUILayout.TextField(ev.string0);

                        break;
                        #endregion
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();
            EVNUM++;

        }
    }

    void LoadTempLevel(string dir)
    {
        s_leveledit ed = GameObject.Find("Main Camera").GetComponent<s_leveledit>();
        ed.LoadTempMap(ed.JsonToObj(dir));
    }
}
*/
/*
           switch ((EVENT_TYPES)eventType)
           {
               #region LABEL
               case EVENT_TYPES.LABEL:

                   EditorGUILayout.Space();
                   EditorGUILayout.LabelField("Label name");
                   ev.string0 = EditorGUILayout.TextArea(ev.string0);
                   ev.int0 = i;
                   EditorGUILayout.LabelField("At " + i);
                   break;
               #endregion

               #region MOVE
               case EVENT_TYPES.MOVEMNET:

                   EditorGUILayout.Space();
                   EditorGUILayout.LabelField("Teleport?");
                   ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                   if (!ev.boolean)
                   {
                       EditorGUILayout.LabelField("Character name");
                       ev.string0 = EditorGUILayout.TextArea(ev.string0);
                       ev.float0 = EditorGUILayout.FloatField("X", ev.float0);
                       ev.float1 = EditorGUILayout.FloatField("Y", ev.float1);
                   }
                   else
                   {
                       EditorGUILayout.LabelField("Character name");
                       ev.string0 = EditorGUILayout.TextArea(ev.string0);
                       ev.float0 = EditorGUILayout.FloatField("X", ev.float0);
                       ev.float1 = EditorGUILayout.FloatField("Y", ev.float1);
                   }
                   break;
               #endregion

               #region DIALOGUE
               case EVENT_TYPES.DIALOGUE:

                   ev.string0 = EditorGUILayout.TextArea(ev.string0);
                   EditorGUILayout.Space();
                   break;
               #endregion

               #region CHECK FLAG
               case EVENT_TYPES.CHECK_FLAG:
                   ev.logic = (int)(LOGIC_TYPE)EditorGUILayout.EnumPopup("Logic Type", (LOGIC_TYPE)ev.logic);
                   switch ((LOGIC_TYPE)ev.logic)
                   {
                       case LOGIC_TYPE.VAR_EQUAL:
                           EditorGUILayout.BeginHorizontal();

                           EditorGUILayout.LabelField("IF ");
                           ev.string0 = EditorGUILayout.TextField(ev.string0);
                           EditorGUILayout.LabelField(" = ");
                           ev.int0 = EditorGUILayout.IntField(ev.int0);
                           EditorGUILayout.Space();
                           EditorGUILayout.EndHorizontal();

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("THEN JUMP TO ");
                           ev.int1 = EditorGUILayout.IntField(ev.int1);
                           EditorGUILayout.EndHorizontal();
                           break;

                       case LOGIC_TYPE.VAR_GREATER:

                           EditorGUILayout.BeginHorizontal();

                           EditorGUILayout.LabelField("IF ");
                           ev.string0 = EditorGUILayout.TextField(ev.string0);
                           EditorGUILayout.LabelField(" > ");
                           ev.int0 = EditorGUILayout.IntField(ev.int0);
                           EditorGUILayout.Space();
                           EditorGUILayout.EndHorizontal();

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("THEN JUMP TO ");
                           ev.int1 = EditorGUILayout.IntField(ev.int1);
                           EditorGUILayout.EndHorizontal();
                           break;

                       case LOGIC_TYPE.VAR_LESS:

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("IF ");
                           ev.string0 = EditorGUILayout.TextField(ev.string0);
                           EditorGUILayout.LabelField(" < ");
                           ev.int0 = EditorGUILayout.IntField(ev.int0);
                           EditorGUILayout.Space();
                           EditorGUILayout.EndHorizontal();

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("THEN JUMP TO ");
                           ev.int1 = EditorGUILayout.IntField(ev.int1);
                           EditorGUILayout.EndHorizontal();
                           break;

                       case LOGIC_TYPE.ITEM_OWNED:

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("IF "); EditorGUILayout.LabelField("ITEM ");
                           ev.string0 = EditorGUILayout.TextField(ev.string0);
                           EditorGUILayout.LabelField(" WITH TYPE ");
                           ev.int0 = EditorGUILayout.IntField(ev.int0);
                           EditorGUILayout.LabelField(" (" + (o_item.ITEM_TYPE)ev.int0 + ")");
                           EditorGUILayout.LabelField(" POSSESED.");
                           EditorGUILayout.EndHorizontal();

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("THEN JUMP TO ");
                           ev.int1 = EditorGUILayout.IntField(ev.int1);
                           EditorGUILayout.EndHorizontal();
                           break;

                       case LOGIC_TYPE.CHECK_CHARACTER:

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("IF CHARACTER IS ");
                           ev.string0 = EditorGUILayout.TextField(ev.string0);
                           EditorGUILayout.EndHorizontal();

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("THEN JUMP TO ");
                           ev.int1 = EditorGUILayout.IntField(ev.int1);
                           EditorGUILayout.EndHorizontal();
                           break;

                       case LOGIC_TYPE.CHECK_CHARACTER_NOT:

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("IF CHARACTER IS NOT");
                           ev.string0 = EditorGUILayout.TextField(ev.string0);
                           EditorGUILayout.EndHorizontal();

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("THEN JUMP TO ");
                           ev.int1 = EditorGUILayout.IntField(ev.int1);
                           EditorGUILayout.EndHorizontal();
                           break;

                       case LOGIC_TYPE.CHECK_UTILITY_RETURN_NUM:

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("IF UITILITY ");
                           SetStringToObjectName(ref ev);
                           EditorGUILayout.LabelField(ev.string0);
                           EditorGUILayout.LabelField(" RETURN NUMBER IS ");
                           ev.int0 = EditorGUILayout.IntField(ev.int0);
                           EditorGUILayout.EndHorizontal();

                           EditorGUILayout.BeginHorizontal();
                           EditorGUILayout.LabelField("THEN JUMP TO ");
                           ev.int1 = EditorGUILayout.IntField(ev.int1);
                           EditorGUILayout.EndHorizontal();
                           break;
                   }
                   break;
               #endregion

               #region SET VAR
               case EVENT_TYPES.SET_FLAG:

                   EditorGUILayout.BeginHorizontal();

                   EditorGUILayout.LabelField("SET VARIABLE ");
                   ev.string0 = EditorGUILayout.TextField(ev.string0);
                   EditorGUILayout.LabelField(" TO ");
                   ev.int0 = EditorGUILayout.IntField(ev.int0);
                   EditorGUILayout.EndHorizontal();
                   break;
               #endregion

               #region FADE
               case EVENT_TYPES.FADE:

                   EditorGUILayout.LabelField("Colour of this part");
                   EditorGUILayout.ColorField(new Color(ev.colour.r, ev.colour.g, ev.colour.b, ev.colour.a));
                   EditorGUILayout.LabelField("New Colour of this part");
                   evcolour = EditorGUILayout.ColorField(evcolour);

                   if (GUILayout.Button("Set Colour"))
                   {
                       ev.colour = new ev_details.s_save_colour(evcolour);
                   }

                   break;
               #endregion

               #region OBJECT
               case EVENT_TYPES.OBJECT:

                   SetStringToObjectName(ref ev);
                   EditorGUILayout.LabelField("Object name: " + ev.string0);
                   EditorGUILayout.LabelField("Hide object?");
                   ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                   break;
               #endregion

               #region CAMERA MOVEMENT
               case EVENT_TYPES.CAMERA_MOVEMENT:

                   EditorGUILayout.LabelField("Focus on character?");
                   ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                   EditorGUILayout.LabelField("Stay on character");
                   ev.boolean1 = EditorGUILayout.Toggle(ev.boolean1);

                   EditorGUILayout.LabelField("Speed");
                   ev.float0 = EditorGUILayout.FloatField(ev.float0);
                   if (!ev.boolean)
                   {
                       EditorGUILayout.LabelField("Position: " + ev.pos.x + ", " + ev.pos.y);

                       if (GUILayout.Button("Set Pos"))
                           ev.pos = new s_save_vector(pos);
                   }
                   else
                   {
                       EditorGUILayout.LabelField("Character to focus on.");
                       ev.string0 = EditorGUILayout.TextArea(ev.string0);
                   }

                   break;
               #endregion

               #region DISPLAY CHARACTER HEALTH
               case EVENT_TYPES.DISPLAY_CHARACTER_HEALTH:
                   //objlist[o] = (o_character)EditorGUILayout.ObjectField(objlist[o], typeof(o_character), true);
                   //There will be one GUI for the top (near main character health) and one in the bottom (boss health)
                   ev.boolean1 = EditorGUILayout.Toggle("Top?", ev.boolean1);
                   //
                   ev.boolean = EditorGUILayout.Toggle("Show?", ev.boolean);

                   break;
               #endregion

               #region WAIT
               case EVENT_TYPES.WAIT:

                   ev.float0 = EditorGUILayout.FloatField("Length: ", ev.float0);

                   break;

               #endregion

               #region SET_HEALTH
               case EVENT_TYPES.SET_HEALTH:

                   selectedCharacter = (o_character)EditorGUILayout.ObjectField(selectedCharacter, typeof(o_character), true);
                   if (selectedCharacter != null)
                       ev.string0 = selectedCharacter.name;
                   ev.float0 = EditorGUILayout.FloatField("Set health to: ", ev.float0);
                   break;
               #endregion

               #region CHOICE 
               case EVENT_TYPES.CHOICE:
                   ev.string0 = EditorGUILayout.TextField("Question label:", ev.string0);
                   leng = EditorGUILayout.IntField(leng);
                   if (GUILayout.Button("New list"))
                   {
                       ev.stringList = new string[leng];
                       ev.intList = new int[leng];
                   }
                   if (ev.stringList != null)
                   {
                       //EditorGUILayout.BeginScrollView(scrollview2);
                       for (int o = 0; o < ev.stringList.Length; o++)
                       {
                           ev.stringList[o] = EditorGUILayout.TextField("Name of choice" + o + " :", ev.stringList[o]);
                           ev.intList[o] = EditorGUILayout.IntField("Event position to jump to: ", ev.intList[o]);
                           EditorGUILayout.Space();
                       }
                       //EditorGUILayout.EndScrollView();

                       for (int o = 0; o < ev.stringList.Length; o++)
                       {
                           EditorGUILayout.LabelField(ev.stringList[o]);
                           EditorGUILayout.LabelField("" + ev.intList[o]);
                       }
                   }
                   break;
               #endregion

               #region ACTIVATE_SCRIPT
               case EVENT_TYPES.RUN_CHARACTER_SCRIPT:
                   EditorGUILayout.LabelField("Toggle Character script?");
                   ev.boolean = EditorGUILayout.Toggle(ev.boolean);
                   if (!ev.boolean)
                   {
                       EditorGUILayout.LabelField("Run which character's script?");
                   }
                   else
                   {
                       EditorGUILayout.LabelField("Enable/Disable");
                       ev.boolean1 = EditorGUILayout.Toggle(ev.boolean1);
                   }

                   break;
               #endregion

               #region SET_UTILITY_FLAG
               case EVENT_TYPES.SET_UTILITY_FLAG:
                   SetStringToObjectName(ref ev);
                   ev.int0 = EditorGUILayout.IntField(ev.int0);
                   EditorGUILayout.LabelField("Set utility: " + ev.string0 + " state to " + ev.int0);
                   break;
               #endregion

               #region CHECK UTILITY FLAG
               case EVENT_TYPES.UTILITY_CHECK:

                   SetStringToObjectName(ref ev);
                   ev.int0 = EditorGUILayout.IntField(ev.int0);
                   EditorGUILayout.LabelField("Check utility: " + ev.string0 + " flag if state is: " + ev.int0);
                   EditorGUILayout.LabelField("If true, jump to: " + ev.int1);
                   ev.int1 = EditorGUILayout.IntField(ev.int1);
                   break;
               #endregion

               #region UTILITY_INITIALIZE
               case EVENT_TYPES.UTILITY_INITIALIZE:
                   SetStringToObjectName(ref ev);
                   EditorGUILayout.LabelField("Initialize utility: " + ev.string0);
                   break;
               #endregion

               #region CHANGE_MAP
               case EVENT_TYPES.CHANGE_MAP:

                   locations = new List<string>();
                   locations = LocationsMap();

                   EditorGUILayout.LabelField("Map: " + ev.string0);
                   if (ev.string0 == "")
                   {
                       if (locations != null)
                           for (int ind = 0; ind < locations.Count; ind++)
                           {
                               if (ev.string0 == locations[ind])
                                   continue;
                               if (GUILayout.Button(locations[ind]))
                               {
                                   ev.string0 = locations[ind];
                               }
                           }
                   }
                   else
                   {

                       if (GUILayout.Button("None"))
                       {
                           ev.string0 = "";
                       }
                       string mapN = ev.string0;
                       EditorGUILayout.Space();
                       if (locations != null)
                           if (ed.jsonMaps.Find(x => x.name == mapN) != null)
                           {
                               EditorGUILayout.LabelField("Teleport Object: " + ev.string1);
                               posLocation = PositionsOnMap(ev.string0);
                           }
                       if (posLocation != null)
                           for (int ind = 0; ind < posLocation.Count; ind++)
                           {
                               if (GUILayout.Button(posLocation[ind]))
                               {
                                   ev.string1 = posLocation[ind];

                                   Vector2 v = TeleporterPos(ev.string0, ev.string1);
                                   ev.float0 = v.x;
                                   ev.float1 = v.y;
                               }
                           }
                   }
                   break;
                   #endregion
           }
           */
/// <summary>
/// This has to be called from the DisplayCutsceneEditor function
/// This returns a clicked entityname
/// </summary>
/// <returns></returns>