using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

/*
[CustomEditor(typeof(s_leveledit))]
public class ed_level : EditorWindow {

    enum SELECT_MODE {
        ERASE,
        BRUSH,
        SELECT
    };
    SELECT_MODE SM = SELECT_MODE.BRUSH;

    o_collidableobject.COLLISION_T CollisionType;

    const int tilesize = 25;
    int i = 0;
    int levelsel = 0;
    string nam;
    int layer = 0;
    s_leveledit ed;
    GameObject giz;
    Vector2 calculatedmouse;
    GameObject levelobj;
    GameObject[] brushes_blocks;
    GameObject spawnpoint;
    Tool lasttool;

    GameObject selectedBlock;

    BoxCollider2D gizbox;
    GameObject im;
    bool mousedown = false;

    //Pixel's Ultra Propetary Plane Editing Tool
    [MenuItem("Brownie/PUPPET")]
    static void init()
    {
        GetWindow<ed_level>("PUPPET");
    }

    void LoadStuff()
    {
        ed = GameObject.Find("General").GetComponent<s_leveledit>();
        brushes_blocks = new GameObject[ed.objPoolDatabase.Count];
        for (int i = 0; i < ed.objPoolDatabase.Count; i++)
        {
            s_pooler_data pd = ed.objPoolDatabase[i];
            brushes_blocks[i] = pd.gameobject;
        }
    }

    GameObject GetObjectFromMouse(Vector2 pos)
    {
        ed = GameObject.Find("General").GetComponent<s_leveledit>();
        GameObject mapn = ed.SceneLevelObject;
        s_object[] tilesInMap = null;
        o_character[] objectsInMap = null;
        o_trigger[] triggersInMap = null;
        objectsInMap = mapn.transform.Find("Entities").GetComponentsInChildren<o_character>();
        tilesInMap = mapn.transform.Find("Tiles").GetComponentsInChildren<s_object>();
        triggersInMap = mapn.transform.Find("Triggers").GetComponentsInChildren<o_trigger>();

        List<s_map.s_tileobj> tilelist = new List<s_map.s_tileobj>();

        Vector3 p = pos;
        //Vector2Int p = new Vector2Int((int)pos.x / 25, (int)pos.y / 25);
        switch (layer)
        {
            case 0:
                foreach (s_object o in tilesInMap)
                {
                    if (o.transform.position == p)
                        if(o.GetComponent<o_collidableobject>() ||
                            o.GetComponent<o_maptransition>())
                        return o.gameObject;
                }
                break;
            case 1:

                foreach (s_object o in tilesInMap)
                {
                    if (o.transform.position == p)
                        if (o.GetComponent<o_tile>())
                            return o.gameObject;
                }
                break;

            case 2:

                foreach (o_character o in objectsInMap)
                {
                    if (o.transform.position == p)
                            return o.gameObject;
                }
                break;

            case 3:
                foreach (o_trigger o in triggersInMap)
                {
                    if (o.transform.position == p)
                        if (o.GetComponent<o_trigger>())
                            return o.gameObject;
                }
                break;

        }
        return null;
    }

    List<GameObject> layerMatch()
    {
        List<GameObject> listofObj = new List<GameObject>();

        switch (layer)
        {
            case 0:
                foreach (GameObject g in brushes_blocks)
                {
                    if (g == null)
                        continue;
                    if (g.GetComponent<o_collidableobject>() ||
                        g.GetComponent<o_maptransition>())
                        listofObj.Add(g);
                }
                break;

            case 1:
                foreach (GameObject g in brushes_blocks)
                {
                    if (g == null)
                        continue;
                    if (g.GetComponent<o_tile>())
                        listofObj.Add(g);
                }
                break;

            case 2:
                foreach (GameObject g in brushes_blocks)
                {
                    if (g == null)
                        continue;
                    if (g.GetComponent<o_character>())
                        listofObj.Add(g);
                }
                break;

            case 3:
                foreach (GameObject g in brushes_blocks)
                {
                    if (g == null)
                        continue;
                    if (g.GetComponent<o_itemObj>())
                        listofObj.Add(g);
                }
                break;

            case 4:
                foreach (GameObject g in brushes_blocks)
                {
                    if (g == null)
                        continue;
                    if (g.GetComponent<o_trigger>())
                        listofObj.Add(g);
                }
                break;
        }
        return listofObj;
    }
    void MouseEvent(Event e)
    {
        if (mousedown)
        {
            if (calculatedmouse.x < 0 || calculatedmouse.y < 0)
                return;
            if (calculatedmouse.x > 3000 || calculatedmouse.y > 3000)
                return;
            if (SM == SELECT_MODE.ERASE)
            {
                if (e.button == 0)
                {
                    GameObject delete = GetObjectFromMouse(calculatedmouse);
                    if (delete != null)
                    {
                        if (delete != giz)
                            DestroyImmediate(delete);
                    }
                    //mapsiz[(int)calculatedmouse.x, (int)calculatedmouse.y] = null;
                }
            }
            else if (SM == SELECT_MODE.BRUSH)
            {
                if (e.button == 0)
                {
                    if (im == null)
                        return;

                    GameObject go = GetObjectFromMouse(calculatedmouse);
                    i++;
                    if (im.name == "SpawnPoint")
                    {
                        spawnpoint.transform.position = giz.transform.position;
                        spawnpoint.transform.SetParent(levelobj.transform.Find("Tiles"));
                        return;
                    }
                    if (im.GetComponent<o_tile>())
                        if (levelobj.transform.Find("Tiles") != null)
                        {
                            if (go != null)
                            {
                                Debug.Log(go.name);
                                SpriteRenderer re = go.GetComponent<SpriteRenderer>();
                                re.sprite = im.GetComponent<SpriteRenderer>().sprite;
                                return;
                            }
                        }
                    if (im.GetComponent<o_collidableobject>())
                        if (levelobj.transform.Find("Tiles") != null)
                        {
                            if (go != null)
                            {
                                if (go.name == "Tile") {
                                    o_collidableobject sel = go.GetComponent<o_collidableobject>();
                                    sel.collision_type = CollisionType;

                                    SpriteRenderer spr = go.GetComponent<SpriteRenderer>();

                                    switch (sel.collision_type)
                                    {
                                        case o_collidableobject.COLLISION_T.CLIMBING:
                                            spr.color = sel.climbingColour;
                                            break;
                                        case o_collidableobject.COLLISION_T.DAMAGE:
                                            break;
                                        case o_collidableobject.COLLISION_T.WALL:
                                            spr.color = new Color32(255, 0, 255, 150);
                                            break;
                                        case o_collidableobject.COLLISION_T.MOVING_PLATFORM:
                                            spr.color = Color.blue;
                                            break;
                                        case o_collidableobject.COLLISION_T.FALLING:
                                            spr.color = sel.fallingColour;
                                            break;
                                        case o_collidableobject.COLLISION_T.DITCH:
                                            spr.color = sel.ditchColour;
                                            break;
                                        case o_collidableobject.COLLISION_T.FALLING_ON_LAND:
                                            spr.color = sel.fallingOnGroundColour;
                                            break;
                                    }
                                }

                                return;
                            }
                        }

                    if (go != null)
                    {
                        // if (im.GetComponent<o_character>())
                        return;
                    }
                    GameObject lo = Instantiate(im, giz.transform.position, Quaternion.identity);
                    lo.name = im.name;

                    s_object namer = lo.GetComponent<s_object>();
                    if (namer != null)
                        namer.ID = im.gameObject.name;

                    if (go == null)
                    {
                        Debug.Log(im.GetComponent<o_trigger>());
                        if (im.GetComponent<o_character>())
                        {
                            if (levelobj.transform.Find("Entities") != null)
                            {
                                lo.gameObject.name = lo.gameObject.name + "_" + i;
                                lo.transform.SetParent(levelobj.transform.Find("Entities"));
                                return;
                            }
                        }
                        if (im.GetComponent<o_maptransition>())
                        {
                            Debug.Log(im.GetComponent<o_maptransition>());
                            lo.gameObject.name = lo.gameObject.name + "_" + i;
                            lo.transform.SetParent(levelobj.transform.Find("Tiles"));
                            return;
                        }
                        if (im.GetComponent<o_trigger>())
                        {
                            lo.gameObject.name = lo.gameObject.name + "_" + i;
                            lo.transform.SetParent(levelobj.transform.Find("Triggers"));
                            return;
                        }
                        if (im.GetComponent<o_itemObj>())
                        {
                            if (levelobj.transform.Find("Items") != null)
                            {
                                lo.gameObject.name = lo.gameObject.name;
                                lo.transform.SetParent(levelobj.transform.Find("Items"));
                                return;
                            }
                        }
                    }

                    lo.transform.SetParent(levelobj.transform.Find("Tiles"));
                }
            }
            else if (SM == SELECT_MODE.SELECT)
            {
                if (e.button == 0)
                    selectedBlock = GetObjectFromMouse(calculatedmouse);

                
            }

        }
    }

    void SceneGUI(SceneView sv)
    {
        if (giz == null)
            giz = GameObject.Find("Gizmo");

        if (gizbox == null)
            gizbox = giz.GetComponent<BoxCollider2D>();
        
        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Vector2 mousepos = ray.origin;

        calculatedmouse = new Vector3(Mathf.Round(mousepos.x/ tilesize) * tilesize,
            Mathf.Round(mousepos.y / tilesize) * tilesize);
        
        giz.transform.position = calculatedmouse;

        int contrID = GUIUtility.GetControlID(FocusType.Passive);
        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(contrID);
        }

        switch (e.type)
        {
            case EventType.MouseDown:
                mousedown = true;
                break;
            case EventType.MouseUp:
                mousedown = false;
                break;

            case EventType.KeyDown:

                if (e.keyCode == KeyCode.S)
                {
                    Debug.Log(giz.transform.position);
                }
                break;

        }
        MouseEvent(e);
    }

    private void OnInspectorUpdate()
    {
        


        // Ray wor = 

        //Vector2 mousepos = HandleUtility.GUIPointToScreenPixelCoordinate(e.mousePosition);
        //giz.transform.position = mousepos;
    }

    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate += SceneGUI;
        lasttool = Tools.current;
    }

    void SceneGUI()
    {
        Event e = Event.current;
    }
    
    

    GameObject FindBrush(string nameObj)
    {
        GameObject result = null;
        foreach (GameObject b in brushes_blocks)
        {
            if (nameObj == b.name)
                result = b;
        }
        return result;
    }
    

    public void LoadMapOLD(s_map mapdat)
    {
        //current_area = nam;

        GameObject mapIG = new GameObject();
        GameObject triggerIG = new GameObject();
        GameObject entityIG = new GameObject();
        GameObject tileIG = new GameObject();

        mapIG.name = nam;
        triggerIG.name = "Triggers";
        entityIG.name = "Entities";
        tileIG.name = "Tiles";
        GameObject.Find("Player").GetComponent<o_character>().positioninworld = new Vector3(mapdat.spawnPoint.x, mapdat.spawnPoint.y);

        tileIG.transform.SetParent(mapIG.transform);
        triggerIG.transform.SetParent(mapIG.transform);
        entityIG.transform.SetParent(mapIG.transform);

        for (int i = 0; i < mapdat.triggerdata.Count; i++)
        {
            o_trigger trig = Instantiate(, new Vector2(mapdat.triggerdata[i].pos_x, mapdat.triggerdata[i].pos_y), Quaternion.identity).GetComponent<o_trigger>();
            trig.Events.ev_Details = mapdat.triggerdata[i].listofevents;
            if (mapdat.triggerdata[i].util != null)
            {
                string n = mapdat.triggerdata[i].util.GetType().ToString();
                switch (n)
                {
                    case "util_shop":
                        //TODO: LOAD FROM RESOURCES
                        break;
                }
            }

            trig.transform.SetParent(triggerIG.transform);
        }
        for (int i = 0; i < mapdat.objectdata.Count; i++)
        {
            s_map.s_chara objdata = mapdat.objectdata[i];
            o_character trig = Instantiate(FindBrush("Stairs"), new Vector2(mapdat.objectdata[i].pos_x, mapdat.objectdata[i].pos_y), Quaternion.identity).GetComponent<o_character>();
            trig.positioninworld = new Vector3(mapdat.objectdata[i].pos_x, mapdat.objectdata[i].pos_x, 1);
            trig.transform.SetParent(entityIG.transform);
        }
        for (int i = 0; i < mapdat.tilesdata.Count; i++)
        {
            s_map.s_tileobj tile = mapdat.tilesdata[i];
            o_collidableobject trig = Instantiate(FindBrush("Stairs"), new Vector2(tile.pos_x, tile.pos_y), Quaternion.identity).GetComponent<o_collidableobject>();
            trig.collision_type = (o_collidableobject.COLLISION_T)tile.enumthing;
            trig.positioninworld = new Vector3(tile.pos_x, tile.pos_x, 1);
            trig.transform.SetParent(tileIG.transform);
        }
        
        GameObject levedatabase = Resources.Load("LevelDatabase") as GameObject;

        if(dat == null)
            dat = levedatabase.GetComponent<s_leveldatabase>();
        print(dat.maps.Count);

        
        s_map mapdat = new s_map();
        for (int i = 0; i < dat.maps.Count; i++)
        {
            if(dat.maps[i].name == levelname)
                mapdat = dat.maps[i];
        }
        current_area = levelname;

        GameObject mapIG = new GameObject();
        GameObject triggerIG = new GameObject();
        GameObject entityIG = new GameObject();
        GameObject tileIG = new GameObject();

        mapIG.name = current_area;
        triggerIG.name = "Triggers";
        entityIG.name = "Entities";
        tileIG.name = "Tiles";

        tileIG.transform.SetParent(mapIG.transform);
        triggerIG.transform.SetParent(mapIG.transform);
        entityIG.transform.SetParent(mapIG.transform);

        for (int i = 0; i < mapdat.triggerdata.Count; i++)
        {
            o_trigger trig = Instantiate(triggerprefab, new Vector2(mapdat.triggerdata[i].pos_x, mapdat.triggerdata[i].pos_y), Quaternion.identity).GetComponent<o_trigger>();
            trig.Events.ev_Details = mapdat.triggerdata[i].listofevents;
            if (mapdat.triggerdata[i].util != null)
            {
                string n = mapdat.triggerdata[i].util.GetType().ToString();
                switch (n)
                {
                    case "util_shop":
                        //TODO: LOAD FROM RESOURCES
                        break;
                }
            }

            trig.transform.SetParent(triggerIG.transform);
        }
        for (int i = 0; i < mapdat.objectdata.Count; i++)
        {
            o_character trig = Instantiate(charactersSpawnlist[0], new Vector2(mapdat.objectdata[i].pos_x, mapdat.objectdata[i].pos_y), Quaternion.identity).GetComponent<o_character>();
            trig.positioninworld = new Vector3(mapdat.objectdata[i].pos_x, mapdat.objectdata[i].pos_x,1);
            trig.transform.SetParent(entityIG.transform);
        }
        for (int i = 0; i < mapdat.tilesdata.Count; i++)
        {
            o_collidableobject trig = Instantiate(prefab, new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_collidableobject>();
            trig.height = mapdat.tilesdata[i].height;
            trig.positioninworld = new Vector3(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_x, 1);
            trig.transform.SetParent(tileIG.transform);
        }
    }
    private void OnGUI()
    {
        if (brushes_blocks == null)
            LoadStuff();
        if (ed == null)
            if (GameObject.Find("General"))
            ed = GameObject.Find("General").GetComponent<s_leveledit>();

        if (levelobj == null)
            levelobj = ed.SceneLevelObject;
        if (spawnpoint == null)
            spawnpoint = GameObject.Find("SpawnPoint");

        switch (SM) {
            case SELECT_MODE.BRUSH:
                if (GUI.Button(new Rect(35 * 11, 10, 160, 40), "Brush"))
                    SM = SELECT_MODE.ERASE;
                break;

            case SELECT_MODE.ERASE:
                if (GUI.Button(new Rect(35 * 11, 10, 160, 40), "Erase"))
                    SM = SELECT_MODE.SELECT;
                break;

            case SELECT_MODE.SELECT:
                if (GUI.Button(new Rect(35 * 11, 10, 160, 40), "Select"))
                    SM = SELECT_MODE.BRUSH;
                break;
        }

        if (GUI.Button(new Rect(35 * 17, 50, 80, 40), "New"))
        {
            ed.NewMap();
        }
        if (GUI.Button(new Rect(35 * 11, 50, 80, 40), "Save"))
        {
            string dir = EditorUtility.SaveFilePanel("Save Json level file", "Assets/Levels/", "Unnamed", "txt");
            if (dir != string.Empty)
                ed.SaveMap(dir);
        }
        if (GUI.Button(new Rect(35 * 14, 50, 80, 40), "Load"))
        {
            string dir = EditorUtility.OpenFilePanel("Open Json level file", "Assets/Levels/", "");
            if(dir != string.Empty)
                ed.LoadMap(ed.JsonToObj(dir));
        }
        if (GUI.Button(new Rect(35 * 11, 120, 80, 40), "Tiles : 0")) { layer = 0; }
        if (GUI.Button(new Rect(35 * 13f, 120, 80, 40), "Decor : 1")) { layer = 1; }
        if (GUI.Button(new Rect(35 * 15f, 120, 80, 40), "Entities: 2")) { layer = 2; }
        if (GUI.Button(new Rect(35 * 17f, 120, 80, 40), "Triggers: 3")) { layer = 3; }
        EditorGUI.LabelField(new Rect(35 * 13f, 160, 160, 40), "Current Layer : " + layer);

        ed.zone = GUI.TextArea(new Rect(35 * 14, 95, 80, 25), ed.zone);
        ed.nam = GUI.TextArea(new Rect(35 * 11, 95, 80, 25), ed.nam);
        ed.mapscript = (o_trigger)EditorGUI.ObjectField(new Rect(35 * 17, 95, 80, 25),ed.mapscript, typeof(o_trigger),true);

        if (selectedBlock != null) {
            o_collidableobject sel = selectedBlock.GetComponent<o_collidableobject>();
            if (sel) {
                sel.collision_type = (o_collidableobject.COLLISION_T)EditorGUI.EnumPopup(
                    new Rect(35 * 17, 165, 80, 25),
                    sel.collision_type);

                EditorGUI.LabelField(new Rect(35 * 17, 195, 210, 25), "Position: " + selectedBlock.transform.position.x + ", " + selectedBlock.transform.position.y);
            }
        }
        CollisionType = (o_collidableobject.COLLISION_T)EditorGUI.EnumPopup(
            new Rect(35 * 17, 165, 80, 25),
            CollisionType);

        ed.mapsizeToKeep.x = EditorGUI.FloatField(new Rect(35 * 20, 95, 40, 25), ed.mapsizeToKeep.x);
        ed.mapsizeToKeep.y = EditorGUI.FloatField(new Rect(35 * 22, 95, 40, 25), ed.mapsizeToKeep.y);
        
        int i = 0;
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (i < brushes_blocks.Length)
                {
                    SpriteRenderer spr = brushes_blocks[i].GetComponent<SpriteRenderer>();
                    if (spr)
                    {
                        if (GUI.Button(new Rect(10 + x * 35, 10 + y * 45, 30, 40), brushes_blocks[i].GetComponent<SpriteRenderer>().sprite.texture))
                        {
                            im = brushes_blocks[i];
                        }
                    }
                    else {
                        if (GUI.Button(new Rect(10 + x * 35, 10 + y * 45, 30, 40), "Dummy"))
                        {
                            im = brushes_blocks[i];
                        }
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(10 + x * 35, 10 + y * 45, 30, 40), "???")){
                        im = null;
                    }
                }
                i++;
            }
        }


        if (im != null)
        {
            EditorGUI.LabelField(new Rect(35 * 15, 170, 90, 90), im.name);
            if (im.name == "TileDecor")
            {
                im.GetComponent<SpriteRenderer>().sprite = (Sprite)EditorGUI.ObjectField(new Rect(35 * 15, 140, 90, 90), im.GetComponent<SpriteRenderer>().sprite, typeof(Sprite), true);
            }
        }

        if (Selection.activeGameObject != null)
        {

            ed = Selection.activeGameObject.GetComponent<s_leveledit>();
            if (ed != null)
            {
                if (GUI.Button(new Rect(10, 40, 150, 10), "Load"))
                {
                    string dir = EditorUtility.OpenFilePanel("Open Json level file", "Assets/Levels/", "");
                    ed.LoadMap(ed.JsonToObj(dir));
                }
                if (GUI.Button(new Rect(10, 70, 150, 10), "New"))
                {
                    ed.NewMap();
                }
                if (GUI.Button(new Rect(10, 100, 150, 10), "Save"))
                {
                    string dir = EditorUtility.SaveFilePanel("Save Json level file", "Assets/Levels/", "Unnamed", "txt");
                    ed.SaveMap(dir);
                }
            }
        }
    }

}

/*
if (ed != null) {
if (ed.mapDat != null)
{
Texture2D te = new Texture2D(300, 300);
Color32[] co = te.GetPixels32();


for (int ax = 0; ax < te.width; ax++)
{
for (int ay = 0; ay < te.height; ay++)
{
foreach (s_map.s_tileobj ti in ed.mapDat.tilesdata)
{
if (ti.TYPENAME == "teleport_object") {
    if ((ti.pos_x / 20) == ax && (ti.pos_y / 20) == ay)
    {
        bo[a].b = 200;
        break;
    }
}
if (Mathf.RoundToInt(ti.pos_x / 20) == ax && Mathf.RoundToInt(ti.pos_y / 20) == ay)
{
    //co[ax * ay * te.width] = new Color32(200, 3, 3, 255);
    break;
}

}
}
}
te.SetPixels32(co);
te.Apply(false);

//EditorGUI.DrawPreviewTexture(new Rect(375, 200, 300, 300), te);
}
}
*/
