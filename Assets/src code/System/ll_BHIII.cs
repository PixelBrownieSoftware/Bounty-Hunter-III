using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Runtime.Serialization.Formatters.Binary;
using MagnumFoudation;

using UnityEngine.UI;
using System;

/*
[System.Serializable]
public struct ev_integer
{
    public int integer;
    public string integer_name;
}
[System.Serializable]
public struct s_pooler_data
{
    public int amount;
    public GameObject gameobject;
}
public class s_maplayer
{
    public s_maplayer()
    {
        objects = new s_object[100, 100];
    }
    public s_object[,] objects;
}
*/

public class ll_BHIII : s_mapManager
{
    public pl_milbert milbert;
    public List<RuleTile> ruleTiles = new List<RuleTile>();

    public Tilemap ditchCol;
    public Tilemap waterCol;
    public Tilemap ledgeCol;
    public Tilemap ledgeFallCol;

    private new void Awake()
    {
        if(LevEd == null)
            LevEd = this;
        base.Awake();
    }

    public override void InitializeManager()
    {
        base.InitializeManager();
        InitializeLoader();
        InitializeGameWorld();
        player.gameObject.SetActive(true);
        s_camera.cam = GameObject.Find("Main Camera").GetComponent<s_camera>();
        s_camera.cam.SetPlayer(player);
        canv.gameObject.SetActive(true);
    }

    public override void InitializePlayer()
    {
        player.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;
        o_plcharacter pc = player.GetComponent<o_plcharacter>();
        BHIII_save sav = s_mainmenu.GetSaveFile<BHIII_save>();
        if (sav != null)
        {
            int apMX = sav.apMax;
            float ap = sav.ap;
            pc.maxAmmoPoints = apMX;
            pc.ammoPoints = ap;
            pc.maxHealth = sav.MAXhp;
            pc.health = sav.hp;
            pc.transform.position = new Vector3(sav.location.x, sav.location.y);
        }
        pc.control = true;
    }

}

/*
public override GameObject SetTrigger(s_map.s_trig trigger)
{
    GameObject trigObj = null;
    Vector2 pos = new Vector2(trigger.pos_x, trigger.pos_y);

    if (InEditor)
    {
#if UNITY_EDITOR
        switch (trigger.util)
        {
            default:
                trigObj = PrefabUtility.InstantiatePrefab(FindOBJ("Trigger")) as GameObject;
                break;

            case "util_button":
                trigObj = PrefabUtility.InstantiatePrefab(FindOBJ("buttonObject")) as GameObject;
                break;

            case "util_door":
                trigObj = PrefabUtility.InstantiatePrefab(FindOBJ("lockedDoor")) as GameObject;
                break;

            case "u_boundary":
                trigObj = PrefabUtility.InstantiatePrefab(FindOBJ("boundOBJ")) as GameObject;
                break;

            case "util_throwUnlock":
                trigObj = PrefabUtility.InstantiatePrefab(FindOBJ("throw_unlock")) as GameObject;
                break;
        }
#endif
    }
    else
        switch (trigger.util)
        {
            default:
                trigObj = SpawnObject<s_object>(FindOBJ("Trigger"), pos, Quaternion.identity).gameObject;
                break;

            case "util_button":
                trigObj = SpawnObject<s_object>("buttonObject", pos, Quaternion.identity).gameObject;
                break;

            case "util_door":
                trigObj = SpawnObject<s_object>("lockedDoor", pos, Quaternion.identity).gameObject;
                break;

            case "util_throwUnlock":
                trigObj = SpawnObject<s_object>("throw_unlock", pos, Quaternion.identity).gameObject;
                break;

            case "u_boundary":
                trigObj = SpawnObject<s_object>("boundOBJ", pos, Quaternion.identity).gameObject;
                break;
        }
    trigObj.transform.position = pos;
    trigObj.transform.SetParent(triggersObj.transform);
    o_trigger trig = trigObj.GetComponent<o_trigger>();
    s_utility util = null;
    if (trig == null)
        util = trigObj.GetComponent<s_utility>();

    //trig.TRIGGER_T = mapdat.triggerdata[i].trigtye;
    if (trigger.util != null)
    {
        string n = trigger.util;
    }
    if (trig != null)
    {
        trig.ev_num = 0;    //TODO: IF THE TRIGGER DOES NOT STATICALLY STORE ITS EVENT NUMBER, SET IT TO 0

        if (trigger.name != "")
            trig.name = trigger.name;
        trig.isactive = false;
        trig.LabelToJumpTo = trigger.labelToJumpTo;
        trig.stringLabelToJumpTo = trigger.stringLabelToJumpTo;
        trig.TRIGGER_T = trigger.trigtye;
        //trig.TRIGGER_T = mapdat.triggerdata[i].trigtye;

        s_save_vector ve = trigger.trigSize;
        trig.GetComponent<BoxCollider2D>().size = new Vector2(ve.x, ve.y);

        if (trig.GetComponent<util_door>()) {
            util_door d = trig.GetComponent<util_door>();
            d.FindSprite(trigger.GetCustomStringTag("sprite"));
        }

        trig.transform.SetParent(trig.transform);
    }
    else if (util != null)
    {
        if (trigger.name != "")
            util.name = trigger.name;

        s_save_vector ve = trigger.trigSize;

        if (util.GetComponent<BoxCollider2D>())
            util.GetComponent<BoxCollider2D>().size = new Vector2(ve.x, ve.y);


        util.transform.SetParent(util.transform);
    }

    //trig.Events.ev_Details = mapdat.triggerdata[i].listofevents;

    return trigObj;
}
public override List<s_map.s_trig> GetTriggers(s_object[] triggers)
{
    List<s_map.s_trig> Tr = new List<s_map.s_trig>();
    foreach (s_object obj in triggers)
    {
        s_map.s_trig tri = null;
        if (obj.GetComponent<BoxCollider2D>())
            tri = new s_map.s_trig(obj.name, obj.transform.position, 0, obj.GetComponent<BoxCollider2D>().size, false);
        else
            tri = new s_map.s_trig(obj.name, obj.transform.position, 0, new Vector2(0,0), false);

        if (obj.GetComponent<util_button>())
        {
            util_button b = obj.GetComponent<util_button>();
            if(b.isMilbertButton)
                tri.AddCustomTag("is_milbert", 1);
            else
                tri.AddCustomTag("is_milbert", 0);
            tri.util = b.GetType().ToString();
        }
        if (obj.GetComponent<util_throwUnlock>())
        {
            util_throwUnlock b = obj.GetComponent<util_throwUnlock>();
            List<string> s = new List<string>();
            tri.CustomStrings = new List<StringList>();
            if (b.door != null)
            {
                s.Add(b.door.name);
                tri.CustomStrings.Add(new StringList("door", s));
            }
            tri.util = b.GetType().ToString();
        }
        if (obj.GetComponent<util_door>())
        {
            util_door b = obj.GetComponent<util_door>();
            tri.util = b.GetType().ToString(); 
                tri.AddCustomTag("sprite", b.rendererObj.sprite.name);
            tri.trigSize = new s_save_vector(b.collision.size.x, b.collision.size.y);
            foreach (util_button bu in b.buttons)
            {
                tri.AddCustomTag("button", bu.name);
            }
            tri.requiresDependency = true;
            Tr.Add(tri);
            continue;
        }
        if (obj.GetComponent<u_boundary>())
        {
            tri = new s_map.s_trig(obj.name, obj.transform.position);
            u_boundary b = obj.GetComponent<u_boundary>();
            tri.trigSize = new s_save_vector(obj.transform.localScale);
            print(b.GetType().ToString());

            List<string> bound = new List<string>();
            List<string> names = new List<string>();
            int ind = 0;
            if (b.characters != null)
                foreach (o_character n in b.characters) //Copy the names of the characters into the data
                {
                    names.Add(n.name);
                    ind++;
                }
            if (b.bounds != null)
                foreach (o_generic n in b.bounds) //Copy the names of the characters into the data
                {
                    bound.Add(n.name);
                    ind++;
                }
            tri.CustomStrings = new List<StringList>();

            tri.CustomStrings.Add(new StringList("chars", names));
            tri.CustomStrings.Add(new StringList("bounds", bound));
            tri.util = b.GetType().ToString();
            Tr.Add(tri);
            continue;
            // return new s_map.s_trig(obj.name, obj.transform.position, obj.Events.ev_Details, b.GetType().ToString(), b.bounds, names, obj.TRIGGER_T, obj.GetComponent<BoxCollider2D>().size, false);
        }
        if (obj.GetComponent<u_shop>())
        {
            u_shop b = obj.GetComponent<u_shop>();

            foreach (o_shopItem itemshop in b.items)
            {
             //   shopitems.Add(new s_save_item(new Vector2(0, 0), 3, ITEM_TYPE.WEAPON, "ff", "ff"));
            }

            print(b.GetType().ToString());
            Tr.Add(new s_map.s_trig(obj.name, obj.transform.position));
            continue;
        }
        if (obj.GetComponent<u_save>())
        {
            u_save b = obj.GetComponent<u_save>();
            print(b.GetType().ToString());
            Tr.Add(new s_map.s_trig(obj.name, obj.transform.position, b.GetType().ToString()));
            continue;
        }
        if (obj.GetComponent<u_spawner>())
        {
            u_spawner b = obj.GetComponent<u_spawner>();
            print(b.GetType().ToString());
            Tr.Add(new s_map.s_trig(obj.name, obj.transform.position, b.GetType().ToString()));
            continue;
        }
        if (obj.GetComponent<o_trigger>())
        {
            // i know there are more efficent ways to do this, but it's not done every frame
            tri = new s_map.s_trig(obj.name, obj.transform.position, obj.GetComponent<o_trigger>().TRIGGER_T, obj.GetComponent<BoxCollider2D>().size, false);
            tri.labelToJumpTo = obj.GetComponent<o_trigger>().LabelToJumpTo;
            tri.stringLabelToJumpTo = obj.GetComponent<o_trigger>().stringLabelToJumpTo;
            tri.trigtye = obj.GetComponent<o_trigger>().TRIGGER_T;
        }

        Tr.Add(tri);
    }
    return Tr;
}
public override void SetTriggerDependency(ref List<Tuple<GameObject, List<s_map.s_customType>>> triggers)
{
    foreach (Tuple<GameObject, List<s_map.s_customType>> tup in triggers)
    {
        util_door uD = tup.Item1.GetComponent<util_door>();
        util_button uB = tup.Item1.GetComponent<util_button>();
        util_throwUnlock uTU = tup.Item1.GetComponent<util_throwUnlock>();
        u_boundary uBo = tup.Item1.GetComponent<u_boundary>();

        if (uB != null)
        {
            for (int i = 0; i < tup.Item2.Count; i++)
            {
                if (tup.Item2[i].type == 1)
                    uB.isMilbertButton = true;
                else
                    uB.isMilbertButton = false;
            }
        }
        if (uD != null)
        {
            for (int i = 0; i < tup.Item2.Count; i++)
            {
                GameObject go = GameObject.Find(tup.Item2[i].type3);
                if (go != null)
                {
                    util_button ub = go.GetComponent<util_button>();
                    if (ub != null)
                    {
                        if (uD.buttons == null)
                        {
                            uD.buttons = new List<util_button>();
                            uD.buttons.Add(ub);
                        }
                        else
                            uD.buttons.Add(ub);
                    }
                }
            }
            uD.InitializeButtons();
        }
    }
}
public override void SetTriggerDependencyAlt(ref List<s_map.s_trig> triggers)
{
    foreach (s_map.s_trig tup in triggers)
    {
        GameObject.Find(tup.name);
        u_boundary uBo = GameObject.Find(tup.name).GetComponent<u_boundary>();
        util_throwUnlock uThUn = GameObject.Find(tup.name).GetComponent<util_throwUnlock>();
        print(tup.name);
        if (uBo != null)
        {
            List<o_generic> bounds = new List<o_generic>();
            if (tup.CustomStrings != null)
                foreach (StringList st in tup.CustomStrings)
                {
                    if (st.name == "bounds")
                    {
                        uBo.bounds = new o_generic[st.listOfStrings.Count];
                        for (int i = 0; i < st.listOfStrings.Count; i++)
                        {
                            string strin = st.listOfStrings[i];

                            uBo.bounds[i] = GameObject.Find(strin).GetComponent<o_generic>();
                            //uBo.bounds[i].gameObject.SetActive(false);
                        }
                    }
                    if (st.name == "chars")
                    {
                        uBo.characters = new BHIII_character[st.listOfStrings.Count];
                        for (int i = 0; i < st.listOfStrings.Count; i++)
                        {
                            string strin = st.listOfStrings[i];

                            uBo.characters[i] = GameObject.Find(strin).GetComponent<BHIII_character>();
                        }
                    }
                }
            //uBo.bounds = 
        }
        if (uThUn != null) {

            List<o_generic> bounds = new List<o_generic>();
            if (tup.CustomStrings != null)
                foreach (StringList st in tup.CustomStrings)
                {
                    if (st.name == "door")
                    {
                        uThUn.door = GameObject.Find(st.listOfStrings[0]).GetComponent<util_door>(); ;
                    }
                }
        }
    }
}

public override List<s_map.s_tileobj> GetTiles(s_object[] tiles)
{
    List<s_map.s_tileobj> tilelist = new List<s_map.s_tileobj>();
    print(tiles.Length);
    foreach (s_object obj in tiles)
    {
        if (obj.GetComponent<o_generic>())
        {
            if (obj.ID == "teleport_object")
            {
                s_map.s_tileobj ob = new s_map.s_tileobj(obj.transform.position, "teleport_object", (int)COLLISION_T.NONE);
                print(obj.name);
                ob.name = obj.name;
                tilelist.Add(ob);
            }
            if (obj.ID == "Boundary")
            {
                s_map.s_tileobj ob = new s_map.s_tileobj(obj.transform.position, "Boundary", (int)COLLISION_T.NONE);
                ob.size = new s_save_vector(obj.transform.localScale.x, obj.transform.localScale.y);
                print(obj.name);
                ob.name = obj.name;
                tilelist.Add(ob);
            }
        }
    }
    return tilelist;
}

public override List<s_map.s_chara> GetEntities(o_character[] characters)
{
    List<s_map.s_chara> charalist = new List<s_map.s_chara>();
    foreach (o_character c in characters)
    {
        bool defeat = false;
        if (c.CHARACTER_STATE == o_character.CHARACTER_STATES.STATE_DEFEAT)
            defeat = true;

        charalist.Add(new s_map.s_chara(
            c.transform.position,
            c.name,
            c.ID,
            c.control,
            defeat,
            false,
            c.faction));
    }
    return charalist;
}

public override void GetTileDat(ref s_map mapfil)
{
    Tile[] tiles = new Tile[(int)mapsizeToKeep.x * (int)mapsizeToKeep.y];
    Tile[] colTiles = new Tile[(int)mapsizeToKeep.x * (int)mapsizeToKeep.y];
    Vector2Int vec = new Vector2Int(0, 0);

    for (int x = 0; x < mapsizeToKeep.x; x++)
    {
        for (int y = 0; y < mapsizeToKeep.y; y++)
        {
            Tile coltil = colmp.GetTile<Tile>(new Vector3Int(x, y, 0));
            COLLISION_T colltype = COLLISION_T.NONE;
            if (coltil != null)
            {
                string tileName = coltil.name;

                switch (tileName)
                {
                    case "falling_on_land":
                        colltype = COLLISION_T.FALLING_ON_LAND;
                        break;

                    case "falling":
                        colltype = COLLISION_T.FALLING;
                        break;

                    case "ditch":
                        colltype = COLLISION_T.DITCH;
                        break;

                    case "collision":
                        colltype = COLLISION_T.WALL;
                        break;

                    case "water":
                        colltype = COLLISION_T.WATER_TILE;
                        break;
                }
                mapfil.tilesdata.Add(
                    new s_map.s_tileobj(
                    new Vector2(x * tilesize, y * tilesize), null,
                    (int)colltype));
            }

            RuleTile til = tm.GetTile<RuleTile>(new Vector3Int(x, y, 0));
            if (til != null)
            {
                mapfil.graphicTiles.Add(
                           new s_map.s_block(til.name,
                           new Vector2(x * tilesize, y * tilesize)));
            }
            RuleTile tilmid = tm2.GetTile<RuleTile>(new Vector3Int(x, y, 0));
            if (tilmid != null)
            {
                mapfil.graphicTilesMiddle.Add(
                           new s_map.s_block(tilmid.name,
                           new Vector2(x * tilesize, y * tilesize)));
            }
            RuleTile tiltop = tm3.GetTile<RuleTile>(new Vector3Int(x, y, 0));
            if (tiltop != null)
            {
                mapfil.graphicTilesTop.Add(
                           new s_map.s_block(tiltop.name,
                           new Vector2(x * tilesize, y * tilesize)));
            }

        }
    }
    print(mapfil.graphicTiles.Count + " 1");
    base.GetTileDat(ref mapfil);
    print(mapfil.graphicTiles.Count + " 2");
}

public override void NewMap()
{
    base.NewMap();
    ledgeFallCol.ClearAllTiles();
    ledgeCol.ClearAllTiles();
    ditchCol.ClearAllTiles();
    waterCol.ClearAllTiles();
}
public override void SetTileMap(s_map mp)
{
    List<s_map.s_tileobj> colTile = mp.tilesdata;
    foreach (s_map.s_block b in mp.graphicTiles)
    {
        tm.SetTile(new Vector3Int((int)b.position.x / (int)tilesize, (int)b.position.y / (int)tilesize, 0), ruleTiles.Find(x => x.name == b.sprite));
    }
    foreach (s_map.s_block b in mp.graphicTilesMiddle)
    {
        tm2.SetTile(new Vector3Int((int)b.position.x / (int)tilesize, (int)b.position.y / (int)tilesize, 0), ruleTiles.Find(x => x.name == b.sprite));
    }
    foreach (s_map.s_block b in mp.graphicTilesTop)
    {
        tm3.SetTile(new Vector3Int((int)b.position.x / (int)tilesize, (int)b.position.y / (int)tilesize, 0), ruleTiles.Find(x => x.name == b.sprite));
    }
    base.SetTileMap(mp);

    foreach (s_map.s_tileobj b in colTile)
    {
        string tilename = "";
        COLLISION_T tileType = (COLLISION_T)b.enumthing;
        if (InEditor)
        {
            switch (tileType)
            {
                case COLLISION_T.CLIMBING:
                    tilename = "climbing";
                    break;
                case COLLISION_T.DITCH:
                    tilename = "ditch"; 
                    ditchCol.SetTile(new Vector3Int((b.pos_x / (int)tilesize), (b.pos_y / (int)tilesize), 0), collisionList.Find(ti => ti.name == tilename));
                    break;
                case COLLISION_T.FALLING_ON_LAND:
                    tilename = "falling_on_land";
                    ledgeCol.SetTile(new Vector3Int((b.pos_x / (int)tilesize), (b.pos_y / (int)tilesize), 0), collisionList.Find(ti => ti.name == tilename));
                    break;
                case COLLISION_T.FALLING:
                    tilename = "falling";
                    ledgeFallCol.SetTile(new Vector3Int((b.pos_x / (int)tilesize), (b.pos_y / (int)tilesize), 0), collisionList.Find(ti => ti.name == tilename));
                    break;
                case COLLISION_T.WALL:
                    tilename = "collision";
                    break;
                case COLLISION_T.WATER_TILE:
                    tilename = "water"; 
                    waterCol.SetTile(new Vector3Int((b.pos_x / (int)tilesize), (b.pos_y / (int)tilesize), 0), collisionList.Find(ti => ti.name == tilename));
                    break;
            }
            colmp.SetTile(new Vector3Int((b.pos_x / (int)tilesize), (b.pos_y / (int)tilesize), 0), collisionList.Find(ti => ti.name == tilename));
        }
        else
        {
            switch (tileType)
            {
                case COLLISION_T.CLIMBING:
                    tilename = "climbing";
                    break;
                case COLLISION_T.DITCH:
                    tilename = "ditch";
                    break;
                case COLLISION_T.FALLING_ON_LAND:
                    tilename = "falling_on_land";
                    break;
                case COLLISION_T.FALLING:
                    tilename = "falling";
                    break;
                case COLLISION_T.WALL:
                    tilename = "collision";
                    break;
                case COLLISION_T.WATER_TILE:
                    tilename = "water";
                    break;
            }
            //nodegraph.SetNode(b.pos_x / tilesize, b.pos_y / tilesize, tileType);
            if (tilename == "collision")
                if (b.TYPENAME != "teleport_object")
                    colmp.SetTile(new Vector3Int(b.pos_x / (int)tilesize, b.pos_y / (int)tilesize, 0), collisionList.Find(ti => ti.name == "collision"));

        }
    }
    for (int i = 0; i < mp.tilesdata.Count; i++)
    {
        s_map.s_tileobj ma = mp.tilesdata[i];
        GameObject trig = null;
        GameObject targname = null;
        if (InEditor)
        {
#if UNITY_EDITOR
            switch (ma.TYPENAME)
            {
                default:
                    continue;

                case "teleport_object":
                    trig = PrefabUtility.InstantiatePrefab(FindOBJ("teleport_object")) as GameObject;
                    trig.name = ma.name;
                    trig.transform.position = new Vector3(ma.pos_x, ma.pos_y, 0);
                    trig.transform.SetParent(tilesObj.transform);
                    break;

                case "bound":
                    trig = PrefabUtility.InstantiatePrefab(FindOBJ("bound")) as GameObject;
                    break;

                case "money":
                    trig = PrefabUtility.InstantiatePrefab(FindOBJ("money")) as GameObject;
                    break;

                case "health_increase":

                    trig = PrefabUtility.InstantiatePrefab(FindOBJ("health_increase")) as GameObject;
                    targname = FindOBJ("health_increase");
                    break;
                case "Boundary":

                    trig = PrefabUtility.InstantiatePrefab(FindOBJ("Boundary")) as GameObject;
                    targname = FindOBJ("Boundary");
                    trig.transform.localScale = new Vector3(ma.size.x, ma.size.y);
                    trig.name = ma.name;
                    break;
            }
#endif

        }
        else
        {
            switch (ma.TYPENAME)
            {
                default:
                    continue;

            case "mapteleport":
                //trig = SpawnObject("Teleporter", new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_maptransition>();
                break;

            case "teleport_object":
                trig = SpawnObject<s_object>("teleport_object", new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity);
                break;
                case "button":
                    trig = SpawnObject<s_object>("bound", new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).gameObject;
                    break;

                case "lock_door":
                    trig = SpawnObject<s_object>("bound", new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).gameObject;
                    break;

                case "bound":
                    trig = SpawnObject<s_object>("bound", new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).gameObject;
                    break;

                case "money":
                    trig = SpawnObject<s_object>("money", new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).gameObject;
                    break;

                case "health_increase":
                    trig = SpawnObject<o_itemObj>("health_increase", new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).gameObject;
                    break;

                case "Boundary":
                    trig = SpawnObject<o_generic>("Boundary", new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).gameObject;
                    targname = FindOBJ("Boundary");
                    trig.transform.localScale = new Vector3(ma.size.x, ma.size.y);
                    trig.name = ma.name;
                    break;

            }
        }

        if (trig == null)
            continue;

        trig.transform.position = new Vector2(ma.pos_x, ma.pos_y);
        if (trig.GetComponent<o_generic>())
        {
            o_generic col = trig.GetComponent<o_generic>();

            col.character = ma.exceptionChar;
            if (ma.TYPENAME == "teleport_object")
            {
                col.name = ma.name;
                print(ma.name);
            }
            if (ma.TYPENAME == "bound")
            {
                col.name = ma.name;
                //col.transform.localScale = new Vector3(ma.scale.x, ma.scale.y);
            }
            //col.collision_type = (COLLISION_T)mapdat.tilesdata[i].enumthing;
            if (!ma.issolid)
            {
                col.characterCannot = ma.cannotPassChar;
                col.issolid = false;
            }
            else
            {
                col.issolid = true;
                col.characterCannot = null;
            }

            SpriteRenderer spr = trig.GetComponent<SpriteRenderer>();
            BoxCollider2D bx = trig.GetComponent<BoxCollider2D>();
            if (bx)
                bx.size = new Vector2(ma.size.x, ma.size.y);
            if (ma.TYPENAME == "bound")
            {
                if (bx)
                    bx.size = new Vector2(20, 20);
            }
            if (ma.TYPENAME == "Boundary")
                col.transform.localScale = new Vector3(ma.size.x, ma.size.y);
        }
        trig.transform.position = new Vector3(ma.pos_x, ma.pos_y, 0);
        trig.transform.SetParent(tilesObj.transform);

        if (targname != null)
            trig.name = targname.name;
        if (ma.TYPENAME == "Boundary")
            trig.name = ma.name;
    }
    print("Done");
}

public override void SetItem(s_save_item item)
{
    o_itemObj itemObj = null;
    if (InEditor)
        itemObj = Instantiate(FindOBJ<o_itemObj>(item.stID), new Vector3(item.pos.x, item.pos.y), Quaternion.identity);
    else
        itemObj = SpawnObject<o_itemObj>(item.stID, new Vector3(item.pos.x, item.pos.y), Quaternion.identity);
    if (itemObj != null) {
        itemObj.amount = 1;
        itemObj.transform.SetParent(itemsObj.transform);
    }
}

public override void SetEntities(List<s_map.s_chara> characters)
{

}

public override T SetEntity<T>(s_map.s_chara character)
{
    switch (character.charType)
    {
        case "Milbert":
            pl_milbert mil = base.SetEntity<pl_milbert>(character);
            mil.name = "Milbert";
            milbert = mil;
            mil.MB_STATE = pl_milbert.MILBERT_STATE.AUTOMATIC;
            GetComponent<BHIII_globals>().player2 = mil;
            break;
    }
    if (InEditor)
        return SetEntityEditor<T>(character);
    else
        return base.SetEntity<T>(character);
}

public virtual T SetEntityEditor<T>(s_map.s_chara character) where T : o_character
{
    s_map.s_chara characterdat = character;
    Vector2 characterPos = new Vector2(characterdat.pos_x, characterdat.pos_y);

    T trig = null;
    if (InEditor)
    {
        if (characterdat.charType == "")
            return null;
        if (FindOBJ(characterdat.charType) == null)
        {
            //print("Couldn't find object '" + characterdat.charType + "' in the pool, please add it to the pooler.");
            return null;
        }
#if UNITY_EDITOR
        if (FindOBJ<o_character>(characterdat.charType))
            trig = PrefabUtility.InstantiatePrefab(FindOBJ<T>(characterdat.charType)) as T;
        else
            return null;
#endif
    }
    else
    {
        if (characterdat.charType == "")
            return null;
        trig = SpawnObject<T>(characterdat.charType, characterPos, Quaternion.identity);
    }
    trig.transform.position = characterPos;
    trig.control = true;
    trig.SetSpawnPoint(characterPos);
    trig.name = characterdat.charname;
    trig.transform.position = new Vector3(characterdat.pos_x, characterdat.pos_y, 0);
    trig.transform.SetParent(entitiesObj.transform);

    allcharacters.Add(trig);
    return trig;
}

public override List<s_save_item> GetItems(o_itemObj[] itemsInMap)
{
    //return base.GetItems(itemsInMap);
    List<s_save_item> itemlist = new List<s_save_item>();
    foreach (o_itemObj c in itemsInMap)
    {
        itemlist.Add(new s_save_item(new Vector2(c.transform.position.x, c.transform.position.y), 0, 0, c.name, c.ID));
    }
    return itemlist;
}
*/
/*
public override void SetEntities(List<s_map.s_chara> characters)
{
    base.SetEntities(characters);
    for (int i = 0; i < characters.Count; i++)
    {
        s_map.s_chara characterdat = characters[i];
        Vector2 characterPos = new Vector2(characterdat.pos_x, characterdat.pos_y);

        BHIII_character trig = null;
        if (InEditor)
        {
            if (characterdat.charType == "")
                continue;
            if (characterdat.charType == "Milbert")
            {
                trig = milbert;
                trig.SetSpawnPoint(characterPos);
                trig.name = characterdat.charname;
                trig.transform.position = characterPos;
                continue;
            }
            if (FindOBJ(characterdat.charType) == null)
            {
                print("Couldn't find object '" + characterdat.charType + "' in the pool, please add it to the pooler.");
                continue;
            }
            trig = Instantiate(FindOBJ(characterdat.charType), characterPos, Quaternion.identity).GetComponent<BHIII_character>();
        }
        else
        {
            //print(characters[i].charType);
            if (characterdat.charType == "")
                continue;
            if (characterdat.charType == "Milbert")
            {
                trig = milbert;
                trig.SetSpawnPoint(characterPos);
                trig.name = characterdat.charname;
                trig.transform.position = characterPos;
                continue;
            }
            //TODO find character equal to the id and spawn that
            if (characters[i].possesed)
                continue;
            trig = SpawnObject<BHIII_character>(characters[i].charType, characterPos, Quaternion.identity);

        }
        trig.control = true;
        trig.SetSpawnPoint(characterPos);
        trig.name = characterdat.charname;
        trig.transform.position = new Vector3(characterdat.pos_x, characterdat.pos_y, 1);
        trig.transform.SetParent(entitiesObj.transform);

        allcharacters.Add(trig);
    }
    allcharacters.Add(milbert);
}
    */
/*
if (trig.GetComponent<o_maptransition>())
{
    o_maptransition col = trig.GetComponent<o_maptransition>();
    if (ma.flagchecks != null)
    {
        col.flagcheck = ma.flagname;
        col.flags = new o_maptransition.s_flagcheck[ma.flagchecks.Length];
        for (int a = 0; a < ma.flagchecks.Length; a++)
        {
            col.flags[a] = new o_maptransition.s_flagcheck(ma.flagchecks[a], ma.mapnames[a]);
        }
    }
    //print(col);

    col.position = new Vector2(ma.teleportpos.x, ma.teleportpos.y);
    col.sceneToTransferTo = ma.mapname;
    col.areaInScene = ma.areaname;
}
*/
/*
if (obj.GetComponent<o_collidableobject>())
{
    switch (obj.GetComponent<o_collidableobject>().collision_type)
    {
        case o_collidableobject.COLLISION_T.MOVING_PLATFORM:
            Debug.Log("Gere");
            //tilelist.Add(new s_map.s_tileobj(obj.transform.position, obj.ID, obj.GetComponent<o_collidableobject>().movepositions));
            break;

        default:
            s_map.s_tileobj o = new s_map.s_tileobj(obj.transform.position, "tile", (int)obj.GetComponent<o_collidableobject>().collision_type);
            o.size = new s_save_vector(obj.GetComponent<BoxCollider2D>().size.x, obj.GetComponent<BoxCollider2D>().size.y);
            tilelist.Add(o);
            break;

    }
    continue;
}
*/
/*
public s_map GetMap()
{
    s_map mapfil = new s_map();
    if (mapscript != null)
        mapfil.mapscript = mapscript.name;
    mapfil.zone = zone;
    GameObject mapn = SceneLevelObject;
    o_character[] objectsInMap = null;
    o_trigger[] triggersInMap = null;
    s_object[] tilesInMap = null;
    mapfil.mapsize = mapsizeToKeep;

    tilesInMap = mapn.transform.Find("Tiles").GetComponentsInChildren<s_object>();
    objectsInMap = mapn.transform.Find("Entities").GetComponentsInChildren<o_character>();
    triggersInMap = mapn.transform.Find("Triggers").GetComponentsInChildren<o_trigger>();
    List<s_map.s_trig> triggerlist = new List<s_map.s_trig>();
    List<s_map.s_chara> charalist = new List<s_map.s_chara>();
    List<s_map.s_tileobj> tilelist = new List<s_map.s_tileobj>();

    foreach (o_trigger obj in triggersInMap)
    {
        triggerlist.Add(AddTrigger(obj));
    }
    mapfil.triggerdata = triggerlist;

    foreach (s_object obj in objectsInMap)
    {
        bool defeat = false;
        if (obj.GetComponent<o_character>().CHARACTER_STATE == o_character.CHARACTER_STATES.STATE_DEFEAT)
            defeat = true;

        o_trigger attached_trigger = obj.GetComponent<o_trigger>();
        if (attached_trigger != null)
        {
            s_map.s_trig tr = AddTrigger(attached_trigger);

            triggerlist.Add(tr);

            print("attached trigger detected");
            charalist.Add(new s_map.s_chara(
                obj.transform.position,
                mapfil.name,
                obj.name,
                obj.ID,
                obj.GetComponent<o_character>().control,
                defeat,
                false,
                obj.GetComponent<o_character>().faction));
            continue;
        }

        charalist.Add(new s_map.s_chara(
            obj.transform.position,
            mapfil.name,
            obj.name,
            obj.ID,
            obj.GetComponent<o_character>().control,
            defeat,
            false,
            obj.GetComponent<o_character>().faction));
    }
    mapfil.objectdata = charalist;

    mapfil.graphicTiles.Clear();
    GetTileDat(ref mapfil);
    foreach (s_object obj in tilesInMap)
    {
        if (obj.name == "SpawnPoint")
            mapfil.spawnPoint = new s_save_vector(obj.transform.position.x, obj.transform.position.y);    //Make the spawnpoint value of the vector
        else
        {
            if (obj.name == "BoundTile")  //This will be spawned within the boundary rather than the tiles    
            {
                continue;
            }
            if (obj.GetComponent<o_maptransition>())
            {
                print("MAPTRANS");
                o_maptransition trans = obj.GetComponent<o_maptransition>();

                int[] flags = new int[trans.flags.Length];
                string[] mapnames = new string[trans.flags.Length];

                for (int a = 0; a < trans.flags.Length; a++)
                {
                    flags[a] = trans.flags[a].flagCondition;
                    mapnames[a] = trans.flags[a].mapname;
                }
                tilelist.Add(new s_map.s_tileobj(
                    obj.transform.position, 
                    "mapteleport", 
                    trans.position, 
                    trans.sceneToTransferTo, 
                    trans.flagcheck, 
                    flags, mapnames, 
                    trans.teleportObj));
                continue;
            }
            if (obj.ID == "teleport_object")
            {
                print(obj.name);
                tilelist.Add(new s_map.s_tileobj(obj.transform.position, "teleport_object", (int)o_collidableobject.COLLISION_T.NONE, obj.name));
                continue;
            }
            if (obj.GetComponent<o_collidableobject>())
            {
                switch (obj.GetComponent<o_collidableobject>().collision_type)
                {
                    case o_collidableobject.COLLISION_T.MOVING_PLATFORM:
                        Debug.Log("Gere");
                        //tilelist.Add(new s_map.s_tileobj(obj.transform.position, obj.ID, obj.GetComponent<o_collidableobject>().movepositions));
                        break;

                    default:
                        s_map.s_tileobj o = new s_map.s_tileobj(obj.transform.position, "tile", (int)obj.GetComponent<o_collidableobject>().collision_type);
                        o.size = new s_save_vector(obj.GetComponent<BoxCollider2D>().size.x, obj.GetComponent<BoxCollider2D>().size.y);
                        tilelist.Add(o);
                        break;

                }

                continue;
            }

        }
    }
    mapfil.tilesdata.AddRange(tilelist);

    print(mapfil.tilesdata);

    return mapfil;
}
*/
/*
if (obj.GetComponent<o_maptransition>())
{
    print("MAPTRANS");
    o_maptransition trans = obj.GetComponent<o_maptransition>();

    int[] flags = new int[trans.flags.Length];
    string[] mapnames = new string[trans.flags.Length];

    for (int a = 0; a < trans.flags.Length; a++)
    {
        flags[a] = trans.flags[a].flagCondition;
        mapnames[a] = trans.flags[a].mapname;
    }
    tilelist.Add(new s_map.s_tileobj(
        obj.transform.position, 
        "mapteleport", 
        trans.position, 
        trans.sceneToTransferTo, 
        trans.flagcheck, 
        flags, mapnames, 
        trans.teleportObj));
    continue;
}
*/
/*
public class s_leveleditLegacy : MonoBehaviour
{
    public static s_leveledit system_Leveledit;
    public List<s_map> maps = new List<s_map>();
    public List<TextAsset> jsonMaps = new List<TextAsset>();
    public List<ev_integer> EventIntegers = new List<ev_integer>();
    public List<o_character> charactersSpawnlist = new List<o_character>();
    public s_map current_map;
    Dictionary<string, Queue<s_object>> objectPoolList = new Dictionary<string, Queue<s_object>>();
    public GameObject base_map;
    public List<s_pooler_data> objPoolDatabase = new List<s_pooler_data>();
    public static s_leveledit LevEd;
    public string zone;
    string Lastzone;
    public s_map mapDat;


    public List<o_character> allcharacters = new List<o_character>();

    public List<s_map.s_trig> savedtriggerdatalist = new List<s_map.s_trig>();
    public List<s_map.s_chara> savedcharalist = new List<s_map.s_chara>();
    List<s_map.s_tileobj> savedtilelist = new List<s_map.s_tileobj>();
    public o_trigger mapscript;

    public List<Sprite> TileSprites = new List<Sprite>();

    public enum EDITMODE
    {
        BRUSH,
        PROPERTIES
    }
    EDITMODE EDIT_MODE;

    public s_map.s_save_vector mapsizeToKeep = new s_map.s_save_vector(0, 0);

    public s_leveldatabase dat { get; set; }
    int o = 0;  //For selecting enums
    public GameObject prefab;
    public GameObject SceneLevelObject;
    public GameObject triggerprefab;
    public List<GameObject> prefabs = new List<GameObject>();
    public GameObject[] brushes_blocks;

    public int boxsize;
    public Vector3 graphsize;
    public Vector3 startpos;
    public LayerMask layerMask;
    o_collidableobject prpo = null;
    dat_save sa = new dat_save(new dat_globalflags(), 0, "NULL", null, null);

    s_nodegraph nodegraph;

    public float hieght;
    public int zposition;
    public bool debug_mode;

    const int tilesize = 25;
    int index = 0;
    public bool isWire;
    public int current_area;
    int prefabselect = 0;
    public string nam;
    bool InEditor = true;  //Dictate the loading style

    public Tilemap tm;
    public Tilemap tm2;
    public Tilemap tm3;
    public Tilemap colmp;
    public List<Tile> tilesList = new List<Tile>();
    public List<Tile> collisionList = new List<Tile>();
    public Canvas canv;

    private void Awake()
    {
        colmp.color = Color.clear;
        canv.gameObject.SetActive(true);
        nodegraph = GetComponent<s_nodegraph>();
        InEditor = false;
        if (LevEd == null)
            LevEd = this;

        InitializeGameWorld();
    }

    public void InitializeGameWorld()
    {
        savedtriggerdatalist.Clear();
        savedcharalist.Clear();
        savedtilelist.Clear();
        maps.Clear();

        SetList();
        LoadMaps();
        if (s_mainmenu.isload)
        {
            if (sa.currentmap == "NULL")
                sa = s_mainmenu.save;

            foreach (KeyValuePair<string, int> s in sa.gbflg.Flags)
            {
                s_globals.SetGlobalFlag(s.Key, s.Value);
            }
            foreach (s_map.s_chara c in sa.charflags)
            {
                savedcharalist.Add(c);
            }
            foreach (s_map.s_trig c in sa.trigflags)
            {
                savedtriggerdatalist.Add(c);
            }
            LoadMap(maps.Find(x => x.name == sa.currentmap));
        }
        else
        {
            print("bnaf");
            LoadMap(maps[0]);
        }
        o_plcharacter selobj = null;
        selobj = GameObject.Find("Player").GetComponent<o_plcharacter>();

        selobj.health = selobj.maxHealth;

        Screen.SetResolution(1280, 720, false);
    }

    public void LoadMaps()
    {
        foreach (TextAsset asset in jsonMaps)
        {
            s_map cu = JsonUtility.FromJson<s_map>(asset.text);
            cu.name = asset.name;
            maps.Add(cu);
        }
    }

    public void TriggerSpawn(string string0, string teleporter)
    {
        NewMap();
        s_map thing = maps.Find(x => x.name == string0);
        LoadMap(thing);
        CheckCharacters();
        o_plcharacter selobj = null;
        selobj = GameObject.Find("Player").GetComponent<o_plcharacter>();

        GameObject mapn = SceneLevelObject;
        s_object[] tilesInMap = null;

        tilesInMap = mapn.transform.Find("Tiles").GetComponentsInChildren<s_object>();

        s_object ma = null;
        foreach (s_object o in tilesInMap)
        {
            o_collidableobject col = o.GetComponent<o_collidableobject>();
            if (col == null)
                continue;
            if (col.GetCollisionType() == o_collidableobject.COLLISION_T.NONE)
            {
                if (col.name == teleporter)
                {
                    ma = o;
                    break;
                }
            }
        }
        if (ma != null)
        {
            if (!selobj.GetComponent<Rigidbody2D>())
                selobj.positioninworld = new Vector3(ma.positioninworld.x, ma.positioninworld.y);
            else
                selobj.transform.position = new Vector3(ma.positioninworld.x, ma.positioninworld.y);
        }
    }
    public void TriggerSpawn(string teleporter)
    {
        o_plcharacter selobj = null;
        selobj = GameObject.Find("Player").GetComponent<o_plcharacter>();

        GameObject mapn = SceneLevelObject;
        s_object[] tilesInMap = null;

        tilesInMap = mapn.transform.Find("Tiles").GetComponentsInChildren<s_object>();

        s_object ma = null;
        foreach (s_object o in tilesInMap)
        {
            o_collidableobject col = o.GetComponent<o_collidableobject>();
            if (col == null)
                continue;
            if (col.GetCollisionType() == o_collidableobject.COLLISION_T.NONE)
            {
                if (col.name == teleporter)
                {
                    ma = o;
                    break;
                }
            }
        }
        if (ma != null)
        {
            if (!selobj.GetComponent<Rigidbody2D>())
                selobj.positioninworld = new Vector3(ma.positioninworld.x, ma.positioninworld.y);
            else
                selobj.transform.position = new Vector3(ma.positioninworld.x, ma.positioninworld.y);
        }
    }

    public void NewMap()
    {
        zone = "";
        nam = "";
        mapscript = null;
        GameObject mapn = SceneLevelObject;
        o_character[] objectsInMap = null;
        o_trigger[] triggersInMap = null;
        s_object[] tilesInMap = null;

        tilesInMap = mapn.transform.Find("Tiles").GetComponentsInChildren<s_object>();
        objectsInMap = mapn.transform.Find("Entities").GetComponentsInChildren<o_character>();
        triggersInMap = mapn.transform.Find("Triggers").GetComponentsInChildren<o_trigger>();

        tm.ClearAllTiles();
        tm2.ClearAllTiles();
        tm3.ClearAllTiles();
        colmp.ClearAllTiles();

        if (triggersInMap != null)
        {
            foreach (o_trigger obj in triggersInMap)
            {
                print(obj.ID);
                if (InEditor)
                    DestroyImmediate(obj.gameObject, true);
                else
                {
                    obj.isactive = false;
                    DespawnObject(obj);
                }
            }
        }

        if (objectsInMap != null)
        {
            foreach (s_object obj in objectsInMap)
            {
                //print(obj.ID);
                if (InEditor)
                    DestroyImmediate(obj.gameObject, true);
                else
                {
                    //Save this for OtherMind
                    bool dead = true;// This isn't dead despite the name
                    if (obj.GetComponent<o_character>().CHARACTER_STATE == o_character.CHARACTER_STATES.STATE_DEFEAT)
                        dead = false;

                    //If a save for this character already exists, just replace the data.
                    s_map.s_chara exist = savedcharalist.Find(x => x.charname == obj.name && x.mapname == mapn.name);
                    
                    if (exist != null)
                    {
                        if (mapn.name == "lvl3")
                        {
                            print("BANISHED IN THE NAME OF GOD");
                            DespawnObject(obj);
                            savedcharalist.Remove(exist);
                            continue;
                        }

                        print(obj.name + " is not dead: " + dead);
                        exist.spawnthis = dead;
                        DespawnObject(obj);
                        continue;
                    }

                    savedcharalist.Add(new s_map.s_chara(obj.positioninworld, mapn.name, obj.name, obj.ID, dead, false));
                    DespawnObject(obj);
                }
            }
        }

        foreach (s_object obj in tilesInMap)
        {
            //print(obj.ID + " " + mapDat.name);
            if (obj.name == "Teleporter")
            {
                DespawnObject(obj);
            }
            if (obj.name == "SpawnPoint")
            {
                continue;
            }
            else
            {
                if (InEditor)
                    DestroyImmediate(obj.gameObject, true);
                else
                {
                    obj.transform.parent = null;
                    DespawnObject(obj);
                }
            }
        }
        index++;
        //FileStream fs = new FileStream("Assets/Levels/" + nam + ".txt", FileMode.CreateNew, FileAccess.Write);
        
        current_map = new s_map("New map " + i);
        current_map.layers.Add(new s_maplayer());
        GameObject o = Instantiate(base_map, transform.position, Quaternion.identity);
        o.name = current_map.name;
        
        
        if (GameObject.Find(maps[current_area].name) != null)
             GameObject.Find(maps[current_area].name).SetActive(false);
         else
         {
            // GameObject.Find("New Level").SetActive(false);
         }
        index++;

        // = Instantiate(base_map);
        //s_map map = PrefabUtility.CreatePrefab("Assets/Levels/" + "New map " + i + ".prefab", base_map.gameObject).GetComponent<s_map>();
        //map.name = ;
        //maps.Add(map);
    }

    public void SaveMap(string dir)
    {
        string mapdat = JsonUtility.ToJson(GetMap());
        print(mapdat);

        File.WriteAllText(dir, mapdat);
        if (File.Exists("Assets/Levels/" + nam + ".txt"))
        {
            //File.WriteAllText("Assets/" + nam + ".txt", string.Empty);
        }
    }
    public s_map GetMap()
    {
        s_map mapfil = new s_map(nam);
        if (mapscript != null)
            mapfil.mapscript = mapscript.name;
        mapfil.zone = zone;
        GameObject mapn = SceneLevelObject;
        o_character[] objectsInMap = null;
        o_trigger[] triggersInMap = null;
        s_object[] tilesInMap = null;
        mapfil.mapsize = mapsizeToKeep;

        tilesInMap = mapn.transform.Find("Tiles").GetComponentsInChildren<s_object>();
        objectsInMap = mapn.transform.Find("Entities").GetComponentsInChildren<o_character>();
        triggersInMap = mapn.transform.Find("Triggers").GetComponentsInChildren<o_trigger>();
        List<s_map.s_trig> triggerlist = new List<s_map.s_trig>();
        List<s_map.s_chara> charalist = new List<s_map.s_chara>();
        List<s_map.s_tileobj> tilelist = new List<s_map.s_tileobj>();

        foreach (o_trigger obj in triggersInMap)
        {
            triggerlist.Add(AddTrigger(obj));
        }
        mapfil.triggerdata = triggerlist;

        foreach (s_object obj in objectsInMap)
        {
            bool defeat = false;
            if (obj.GetComponent<o_character>().CHARACTER_STATE == o_character.CHARACTER_STATES.STATE_DEFEAT)
                defeat = true;

            o_trigger attached_trigger = obj.GetComponent<o_trigger>();
            if (attached_trigger != null)
            {
                s_map.s_trig tr = AddTrigger(attached_trigger);

                triggerlist.Add(tr);

                print("attached trigger detected");
                charalist.Add(new s_map.s_chara(
                    obj.transform.position,
                    mapfil.name,
                    obj.name,
                    obj.ID,
                    obj.GetComponent<o_character>().control,
                    defeat,
                    false,
                    obj.GetComponent<o_character>().faction,
                    true,
                    obj.name));
                continue;
            }

            charalist.Add(new s_map.s_chara(
                obj.transform.position,
                mapfil.name,
                obj.name,
                obj.ID,
                obj.GetComponent<o_character>().control,
                defeat,
                false,
                obj.GetComponent<o_character>().faction, false,
                null));
        }
        mapfil.objectdata = charalist;

        mapfil.graphicTiles.Clear();
        GetTileDat(ref mapfil);
        foreach (s_object obj in tilesInMap)
        {
            if (obj.name == "SpawnPoint")
                mapfil.spawnPoint = new s_map.s_save_vector(obj.transform.position.x, obj.transform.position.y);    //Make the spawnpoint value of the vector
            else
            {
                if (obj.name == "BoundTile")  //This will be spawned within the boundary rather than the tiles    
                {
                    continue;
                }
                if (obj.GetComponent<o_maptransition>())
                {
                    print("MAPTRANS");
                    o_maptransition trans = obj.GetComponent<o_maptransition>();

                    int[] flags = new int[trans.flags.Length];
                    string[] mapnames = new string[trans.flags.Length];

                    for (int a = 0; a < trans.flags.Length; a++)
                    {
                        flags[a] = trans.flags[a].flagCondition;
                        mapnames[a] = trans.flags[a].mapname;
                    }
                    tilelist.Add(new s_map.s_tileobj(obj.transform.position, "mapteleport", trans.position, trans.sceneToTransferTo, trans.flagcheck, flags, mapnames, trans.teleportObj));
                    continue;
                }
                if (obj.ID == "teleport_object")
                {
                    print(obj.name);
                    tilelist.Add(new s_map.s_tileobj(obj.transform.position, "teleport_object", (int)o_collidableobject.COLLISION_T.NONE, obj.name));
                    continue;
                }
                if (obj.GetComponent<o_collidableobject>())
                {
                    switch (obj.GetComponent<o_collidableobject>().collision_type)
                    {
                        case o_collidableobject.COLLISION_T.MOVING_PLATFORM:
                            Debug.Log("Gere");
                            tilelist.Add(new s_map.s_tileobj(obj.transform.position, obj.ID, obj.GetComponent<o_collidableobject>().movepositions));
                            break;

                        default:
                            s_map.s_tileobj o = new s_map.s_tileobj(obj.transform.position, "tile", (int)obj.GetComponent<o_collidableobject>().collision_type);
                            o.colActive = obj.GetComponent<o_collidableobject>().isEnabled;
                            o.size = new s_map.s_save_vector(obj.GetComponent<BoxCollider2D>().size.x, obj.GetComponent<BoxCollider2D>().size.y);
                            tilelist.Add(o);
                            break;

                    }

                    continue;
                }

            }
        }
        mapfil.tilesdata.AddRange(tilelist);

        print(mapfil.tilesdata);
    
        return mapfil;
    }

    public s_map.s_trig AddTrigger(o_trigger obj)
    {

        if (obj.GetComponent<u_boundary>())
        {
            u_boundary b = obj.GetComponent<u_boundary>();
            print(b.GetType().ToString());

            string[] names = new string[b.characters.Length];
            int ind = 0;
            foreach (o_character n in b.characters) //Copy the names of the characters into the data
            {
                names[ind] = n.name;
                ind++;
            }
            return new s_map.s_trig(obj.name, obj.transform.position, obj.Events.ev_Details, b.GetType().ToString(), b.bounds, names, obj.TRIGGER_T, obj.GetComponent<BoxCollider2D>().size, false);
        }
        if (obj.GetComponent<u_shop>())
        {
            u_shop b = obj.GetComponent<u_shop>();

            List<s_map.s_trig.shopit> shopitems = new List<s_map.s_trig.shopit>();
            foreach (o_shopItem itemshop in b.items)
            {
                shopitems.Add(new s_map.s_trig.shopit(itemshop.item.name, (int)itemshop.item.TYPE, itemshop.price));
            }

            print(b.GetType().ToString());
            return new s_map.s_trig(obj.name, obj.transform.position, obj.Events.ev_Details, b.GetType().ToString(), obj.TRIGGER_T, false, shopitems);
        }
        if (obj.GetComponent<u_save>())
        {
            u_save b = obj.GetComponent<u_save>();
            print(b.GetType().ToString());
            return new s_map.s_trig(obj.name, obj.transform.position, obj.Events.ev_Details, b.GetType().ToString(), obj.TRIGGER_T, false);
        }
        if (obj.GetComponent<u_spawner>())
        {
            u_spawner b = obj.GetComponent<u_spawner>();
            print(b.GetType().ToString());
            return new s_map.s_trig(obj.name, obj.transform.position, obj.Events.ev_Details, b.GetType().ToString(), obj.TRIGGER_T, false);
        }
        return new s_map.s_trig(obj.name, obj.transform.position, obj.Events.ev_Details, obj.TRIGGER_T, obj.GetComponent<BoxCollider2D>().size, false);
    }

    public void GetTileDat(ref s_map mapfil)
    {
        Tile[] tiles = new Tile[(int)mapsizeToKeep.x * (int)mapsizeToKeep.y];
        Vector2Int vec = new Vector2Int(0, 0);

        for (int x = 0; x < mapsizeToKeep.x; x++)
        {
            for (int y = 0; y < mapsizeToKeep.y; y++)
            {
                Tile coltil = colmp.GetTile<Tile>(new Vector3Int(x, y, 0));
                o_collidableobject.COLLISION_T colltype = o_collidableobject.COLLISION_T.NONE;
                if (coltil != null)
                {
                    string tileName = coltil.name;

                    switch (tileName)
                    {
                        case "falling_on_land":
                            colltype = o_collidableobject.COLLISION_T.FALLING_ON_LAND;
                            break;

                        case "falling":
                            colltype = o_collidableobject.COLLISION_T.FALLING;
                            break;

                        case "ditch":
                            colltype = o_collidableobject.COLLISION_T.DITCH;
                            break;

                        case "collision":
                            colltype = o_collidableobject.COLLISION_T.WALL;
                            break;
                    }
                    mapfil.tilesdata.Add(
                        new s_map.s_tileobj(
                        new Vector2(x * tilesize, y * tilesize), null,
                        (int)colltype));
                }

                Tile til = tm.GetTile<Tile>(new Vector3Int(x, y, 0));
                if (til != null)
                {
                    mapfil.graphicTiles.Add(
                               new s_map.s_block(til.sprite,
                               new Vector2(x * tilesize, y * tilesize)));
                }

                Tile tilmid = tm2.GetTile<Tile>(new Vector3Int(x, y, 0));
                if (tilmid != null)
                {
                    mapfil.graphicTilesMiddle.Add(
                               new s_map.s_block(tilmid.sprite,
                               new Vector2(x * tilesize, y * tilesize)));
                }

                Tile tiltop = tm3.GetTile<Tile>(new Vector3Int(x, y, 0));
                if (tiltop != null)
                {
                    mapfil.graphicTilesTop.Add(
                               new s_map.s_block(tiltop.sprite,
                               new Vector2(x * tilesize, y * tilesize)));
                }
            }
        }
    }

    public void SetTileMap(s_map mp)
    {
        List<s_map.s_block> tile = mp.graphicTiles;
        List<s_map.s_block> tileMid = mp.graphicTilesMiddle;
        List<s_map.s_block> tileTop = mp.graphicTilesTop;
        List<s_map.s_tileobj> colTile = mp.tilesdata;

        foreach (s_map.s_block b in tile)
        {
            tm.SetTile(new Vector3Int((int)b.position.x / tilesize, (int)b.position.y / tilesize, 0), tilesList.Find(ti => ti.name == b.sprite_name));
        }
        foreach (s_map.s_block b in tileMid)
        {
            tm2.SetTile(new Vector3Int((int)b.position.x / tilesize, (int)b.position.y / tilesize, 0), tilesList.Find(ti => ti.name == b.sprite_name));
        }
        foreach (s_map.s_block b in tileTop)
        {
            tm3.SetTile(new Vector3Int((int)b.position.x / tilesize, (int)b.position.y / tilesize, 0), tilesList.Find(ti => ti.name == b.sprite_name));
        }
        foreach (s_map.s_tileobj b in colTile)
        {
            string tilename = "";
            o_collidableobject.COLLISION_T tileType = (o_collidableobject.COLLISION_T)b.enumthing;
            if (InEditor)
            {
                switch (tileType)
                {
                    case o_collidableobject.COLLISION_T.CLIMBING:
                        tilename = "climbing";
                        break;
                    case o_collidableobject.COLLISION_T.DITCH:
                        tilename = "ditch";
                        break;
                    case o_collidableobject.COLLISION_T.FALLING_ON_LAND:
                        tilename = "falling_on_land";
                        break;
                    case o_collidableobject.COLLISION_T.FALLING:
                        tilename = "falling";
                        break;
                    case o_collidableobject.COLLISION_T.WALL:
                        tilename = "collision";
                        break;
                }
                colmp.SetTile(new Vector3Int(b.pos_x / tilesize, b.pos_y / tilesize, 0), collisionList.Find(ti => ti.name == tilename));
            }
            else
            {
                //nodegraph.SetNode(b.pos_x / tilesize, b.pos_y / tilesize, tileType);
                if (tileType != o_collidableobject.COLLISION_T.WALL)
                    continue;
                if (b.TYPENAME != "teleport_object")
                    colmp.SetTile(new Vector3Int(b.pos_x / tilesize, b.pos_y / tilesize, 0), collisionList.Find(ti => ti.name == "collision"));
            }
        }
        print("Done");
    }

    public s_map JsonToObj(string directory)
    {
        string jso = File.ReadAllText(directory);
        //print(jso);
        //current_map = maps[current_area];
        return JsonUtility.FromJson<s_map>(jso);
        //LoadMap(current_map);
    }

    #region SPAWN FUNCTION
    public void SetList()
    {
        for (int i = 0; i < objPoolDatabase.Count; i++)
        {
            Queue<s_object> objque = new Queue<s_object>();
            GameObject obj = objPoolDatabase[i].gameobject;
            if (objectPoolList.ContainsKey(obj.name))
                continue;
            for (int oc = 0; oc < objPoolDatabase[i].amount; oc++)
            {
                s_object newobj = Instantiate(obj).GetComponent<s_object>();
                objque.Enqueue(newobj);
                newobj.gameObject.SetActive(false);
            }
            objectPoolList.Add(obj.name, objque);
        }
    }

    public void DespawnObject(s_object obj)
    {
        objectPoolList[obj.ID].Enqueue(obj);
        obj.transform.parent = null;
        obj.gameObject.SetActive(false);
    }

    public s_object SpawnObject(string obj)
    {
        if (objectPoolList[obj] == null)
        {
            print(obj + " could not be spawned");
        }
        s_object ob = null;
        if (objectPoolList[obj].Count < 1)
        {
            ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj).gameobject).GetComponent<s_object>();
            ob.gameObject.SetActive(true);
            ob.ID = obj;
            return ob;
        }

        ob = objectPoolList[obj].Dequeue();
        ob.gameObject.SetActive(true);
        ob.ID = obj;
        return ob;
    }
    public s_object SpawnObject(string obj, Transform transformp)
    {
        if (objectPoolList[obj] == null)
        {
            print(obj + " could not be spawned");
        }
        s_object ob = null;
        if (objectPoolList[obj].Count < 1)
        {
            ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj).gameobject, transformp).GetComponent<s_object>();
            ob.positioninworld = transform.position;
            ob.gameObject.SetActive(true);
            ob.ID = obj;
            return ob;
        }

        ob = objectPoolList[obj].Dequeue();
        ob.gameObject.SetActive(true);
        ob.ID = obj;
        return ob;
    }
    public s_object SpawnObject(string obj, Vector3 pos, Quaternion quant)
    {

        if (objectPoolList[obj] == null)
        {
            print(obj + " could not be spawned");
        }
        //print(obj + " name " + objectPoolList[obj].Count);
        s_object ob = null;
        if (objectPoolList[obj].Count < 1)
        {
            ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj).gameobject, pos, quant).GetComponent<s_object>();
            ob.positioninworld = pos;
            ob.gameObject.SetActive(true);
            ob.ID = obj;
            return ob;
        }

        ob = objectPoolList[obj].Dequeue();
        ob.gameObject.transform.localRotation = quant;
        ob.positioninworld = pos;
        ob.transform.position = pos;
        ob.gameObject.SetActive(true);
        ob.ID = obj;
        return ob;
    }
    public s_object SpawnObject(GameObject obj, Vector3 pos, Quaternion quant)
    {

        if (objectPoolList[obj.name] == null)
        {
            print(obj + " could not be spawned");
        }
        print(obj + " name " + objectPoolList[obj.name].Count);
        s_object ob = null;
        if (objectPoolList[obj.name].Count < 1)
        {
            ob = Instantiate(objPoolDatabase.Find(x => x.gameobject.name == obj.name).gameobject, pos, quant).GetComponent<s_object>();
            ob.positioninworld = pos;
            ob.gameObject.SetActive(true);
            ob.ID = obj.name;
            return ob;
        }

        ob = objectPoolList[obj.name].Dequeue();
        ob.gameObject.transform.localRotation = quant;
        ob.positioninworld = pos;
        ob.transform.position = pos;
        ob.gameObject.SetActive(true);
        ob.ID = obj.name;
        return ob;
    }
    #endregion

    public void LoadTempMap(s_map mapdat)
    {
        //current_area = nam;
        GameObject levobj = GameObject.Find("TempLevel");

        GameObject triggerIG = GameObject.Find("TempTriggers");
        GameObject entityIG = GameObject.Find("TempEntities");
        GameObject tileIG = GameObject.Find("TempTiles");

        GameObject mapn = SceneLevelObject;
        o_character[] objectsInMap = null;
        o_trigger[] triggersInMap = null;
        o_collidableobject[] tilesInMap = null;

        tilesInMap = tileIG.GetComponentsInChildren<o_collidableobject>();
        objectsInMap = entityIG.GetComponentsInChildren<o_character>();
        triggersInMap = triggerIG.GetComponentsInChildren<o_trigger>();

        if (triggersInMap != null)
        {
            foreach (o_trigger obj in triggersInMap)
            {
                if (InEditor)
                    DestroyImmediate(obj.gameObject, true);
                else
                    DespawnObject(obj);

            }
        }

        if (objectsInMap != null)
        {
            foreach (s_object obj in objectsInMap)
            {
                if (InEditor)
                    DestroyImmediate(obj.gameObject, true);
                else
                    DespawnObject(obj);
            }
        }

        foreach (s_object obj in tilesInMap)
        {
            if (obj.name == "SpawnPoint")
            {
                continue;
            }
            else
            {
                DestroyImmediate(obj.gameObject, true);
                continue;
            }
        }

        for (int i = 0; i <
            mapdat.triggerdata.Count;
            i++)
        {
            GameObject trigObj = Instantiate(triggerprefab, new Vector2(mapdat.triggerdata[i].pos_x, mapdat.triggerdata[i].pos_y), Quaternion.identity);
            o_trigger trig = trigObj.GetComponent<o_trigger>();
            if (trig == null)
                continue;

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
            o_character trig = Instantiate(prefabs[1], new Vector2(mapdat.objectdata[i].pos_x, mapdat.objectdata[i].pos_y), Quaternion.identity).GetComponent<o_character>();
            trig.positioninworld = new Vector3(mapdat.objectdata[i].pos_x, mapdat.objectdata[i].pos_x, 1);
            trig.transform.SetParent(entityIG.transform);
        }
        for (int i = 0; i < mapdat.tilesdata.Count; i++)
        {
            o_collidableobject trig = Instantiate(prefab, new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_collidableobject>();
            trig.collision_type = (o_collidableobject.COLLISION_T)mapdat.tilesdata[i].enumthing;
            trig.positioninworld = new Vector3(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y, 1);
            trig.transform.position = new Vector3(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y, 1);
            trig.transform.SetParent(tileIG.transform);
        }

    }

    public void CheckCharacters()
    {
        GameObject mapn = SceneLevelObject;
        o_character[] objectsInMap = null;
        objectsInMap = mapn.transform.Find("Entities").GetComponentsInChildren<o_character>();
        foreach (o_character obj in objectsInMap)
        {
            for (int i = 0; i < mapDat.objectdata.Count; i++)
            {
                s_map.s_chara characterdat = mapDat.objectdata[i];
                s_map.s_chara compare = savedcharalist.Find(x => x.charname == characterdat.charname && x.mapname == characterdat.mapname);
                if (compare != null)
                {

                    if (!compare.spawnthis)     //Don't spawn if this character has previously been dead
                    {
                        obj.health = 0;
                        obj.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_DEFEAT;
                    }
                    else
                    {
                        obj.GetComponent<SpriteRenderer>().color = Color.white;
                        obj.health = obj.maxHealth;
                        obj.CHARACTER_STATE = o_character.CHARACTER_STATES.STATE_IDLE;
                    }
                }
            }


        }
    }

    public void LoadMap(s_map mapdat)
    {
        allcharacters.Clear();

        o_character pl = GameObject.Find("Player").GetComponent<o_character>();
        o_character pl2 = GameObject.Find("Milbert").GetComponent<npc_milbert>();
        allcharacters.Add(pl);
        allcharacters.Add(pl2);

        mapscript = null;
        mapDat = mapdat;
        //current_area = nam;
        if (mapdat == null)
            return;

        mapsizeToKeep = mapDat.mapsize;
        SceneLevelObject.name = mapdat.name;

        if (mapdat.zone != Lastzone)
        {
            for (int i = 0; i < savedcharalist.Count; i++)
            {
                if (savedcharalist[i].mapname == Lastzone)
                    savedcharalist.Remove(savedcharalist[i]);
            }
        }
        if (nodegraph == null)
            nodegraph = GetComponent<s_nodegraph>();
        nodegraph.SetNodeSize((int)mapDat.mapsize.x, (int)mapDat.mapsize.y);
        //nodegraph.CreateNodeGraph(mapDat.tilesdata);

        GameObject triggerIG = GameObject.Find("Triggers");
        GameObject entityIG = GameObject.Find("Entities");
        GameObject tileIG = GameObject.Find("Tiles");
        zone = mapdat.zone;
        Lastzone = mapdat.zone;


        #region SPAWN_ENTITIES
        for (int i = 0; i < mapdat.objectdata.Count; i++)
        {
            s_map.s_chara characterdat = mapdat.objectdata[i];

            o_character trig = null;
            if (InEditor)
            {
                print(mapdat.objectdata[i].charType);
                if (mapdat.objectdata[i].charType == "")
                    continue;
                if (mapdat.objectdata[i].charType == null)
                    continue;
                if (FindOBJ(mapdat.objectdata[i].charType) == null)
                    continue;
                trig = Instantiate(FindOBJ(mapdat.objectdata[i].charType), new Vector2(mapdat.objectdata[i].pos_x, mapdat.objectdata[i].pos_y), Quaternion.identity).GetComponent<o_character>();
            }
            else
            {
                //print(mapdat.objectdata[i].charType);
                if (mapdat.objectdata[i].charType == "")
                    continue;
                //TODO find character equal to the id and spawn that
                trig = (o_character)SpawnObject(mapdat.objectdata[i].charType, new Vector2(mapdat.objectdata[i].pos_x, mapdat.objectdata[i].pos_y), Quaternion.identity);

            }
            trig.control = characterdat.enabled;
            trig.name = characterdat.charname;
            trig.faction = mapdat.objectdata[i].faction;
            trig.positioninworld = new Vector3(characterdat.pos_x, characterdat.pos_y, 1);
            trig.transform.position = new Vector3(characterdat.pos_x, characterdat.pos_y, 1);
            trig.transform.SetParent(entityIG.transform);

            allcharacters.Add(trig);
        }
        #endregion

        #region SPAWN_TRIGGERS
        for (int i = 0; i <
            mapdat.triggerdata.Count;
            i++)
        {
            GameObject trigObj = null;
            o_trigger trig = null;

            s_map.s_trig t = mapdat.triggerdata[i];
            s_map.s_trig match = null;

            //Check if this matches with any triggerdata saved 
            //This rigorous check is done so that the 

            foreach (s_map.s_trig dat in savedtriggerdatalist)
            {
                print(dat.listofevents == t.listofevents);
                if (dat.pos_x == t.pos_x)
                {
                    print("Correct");
                    if (dat.pos_y == t.pos_y)
                    {
                        print("Correct");
                        if (dat.trigtye == t.trigtye)
                        {
                            match = dat;
                            break;
                            if (dat.listofevents == t.listofevents)
                            {
                                print(dat.listofevents == t.listofevents);

                                print("Correct");
                                print("DESPAWNED TRIGGER");
                            }
                        }
                    }

                }
                if (dat.boundaryobj == t.boundaryobj)
                {
                    print("Correct");
                    if (dat.characternames == t.characternames)
                    {
                        print("Correct");
                        if (dat.util == t.util)
                        {
                            break;
                        }
                    }
                }
            }
            if (match != null)
                if (match.IsPermanentlyDisabled)
                {
                    continue;
                }
            s_map.s_chara ch = mapDat.objectdata.Find(x => x.has_trigger && x.triggername == t.name);
            print(t.name);
            if (ch != null)
            {
                print("T");
                entityIG.transform.Find(ch.charname).gameObject.AddComponent<o_trigger>();
                trig = entityIG.transform.Find(ch.charname).GetComponent<o_trigger>();
            }
            else
            {
                if (InEditor)
                    trigObj = Instantiate(triggerprefab, new Vector2(mapdat.triggerdata[i].pos_x, mapdat.triggerdata[i].pos_y), Quaternion.identity);
                else
                    trigObj = SpawnObject("Trigger", new Vector2(mapdat.triggerdata[i].pos_x, mapdat.triggerdata[i].pos_y), Quaternion.identity).gameObject;

                trig = trigObj.GetComponent<o_trigger>();
            }

            if (trig == null)
                continue;

            trig.ev_num = 0;    //TODO: IF THE TRIGGER DOES NOT STATICALLY STORE ITS EVENT NUMBER, SET IT TO 0
            trig.Events = new s_events();
            trig.Events.ev_Details = t.listofevents;
            //trig.TRIGGER_T = mapdat.triggerdata[i].trigtye;
            if (mapdat.triggerdata[i].util != null)
            {
                string n = mapdat.triggerdata[i].util;
                switch (n)
                {
                    case "u_shop":
                        u_shop sh = trig.gameObject.AddComponent<u_shop>();
                        if (mapdat.triggerdata[i].shopitems != null)
                        {
                            foreach (s_map.s_trig.shopit it in mapdat.triggerdata[i].shopitems)
                            {
                                sh.AddItem(it.name, it.price, it.item_type);
                            }
                        }
                        break;
                    case "u_boundary":
                        u_boundary bo = trig.gameObject.AddComponent<u_boundary>();
                        bo.bounds = new GameObject[mapdat.triggerdata[i].boundaryobj.Length];
                        int id = 0;
                        foreach (s_map.s_trig.bound b in mapdat.triggerdata[i].boundaryobj)
                        {
                            if (InEditor)
                            {
                                GameObject ob = Instantiate(FindOBJ("BoundTile"), new Vector2(b.pos.x, b.pos.y), Quaternion.identity);
                                ob.name = "BoundTile";
                                bo.bounds[id] = ob;
                                ob.transform.SetParent(tileIG.transform);
                                ob.GetComponent<o_collidableobject>().isEnabled = b.isen;
                            }
                            else
                            {
                                GameObject ob = SpawnObject("BoundTile", new Vector2(b.pos.x, b.pos.y), Quaternion.identity).gameObject;
                                ob.name = "BoundTile";
                                bo.bounds[id] = ob;
                                ob.transform.SetParent(tileIG.transform);
                                ob.GetComponent<o_collidableobject>().isEnabled = b.isen;
                            }
                            id++;
                        }
                        id = 0;
                        bo.characters = new o_character[mapdat.triggerdata[i].characternames.Length];
                        foreach (string b in mapdat.triggerdata[i].characternames)
                        {
                            GameObject ob = GameObject.Find(b);
                            bo.characters[id] = ob.GetComponent<o_character>();

                            id++;
                        }
                        break;

                    case "u_save":
                        trig.gameObject.AddComponent<u_save>();
                        break;

                    case "u_spawner":
                        u_spawner spa = trig.gameObject.AddComponent<u_spawner>();
                        spa.character = "Solider";
                        spa.maxTimer = 1.5f;
                        spa.capacity = 10;
                        spa.limit = true;
                        spa.limitToSpawn = 5;
                        break;
                }
            }
            if (mapdat.triggerdata[i].name != "")
                trig.name = mapdat.triggerdata[i].name;
            trig.isactive = false;
            trig.TRIGGER_T = mapdat.triggerdata[i].trigtye;

            s_map.s_save_vector ve = mapdat.triggerdata[i].trigSize;
            trig.GetComponent<BoxCollider2D>().size = new Vector2(ve.x, ve.y);

            if (mapdat.mapscript != null)
                if (trig.name == mapdat.mapscript)
                    mapscript = trig;

            trig.transform.SetParent(triggerIG.transform);
        }
        #endregion

        SetTileMap(mapdat);

        #region SPAWN_TILES
        for (int i = 0; i < mapdat.tilesdata.Count; i++)
        {
            s_object trig = null;
            GameObject targname = null;
            string objname = null;
            //print((s_map.s_mapteleport)mapdat.tilesdata[i]);
            if (InEditor)
            {
                switch (mapdat.tilesdata[i].TYPENAME)
                {
                    default:

                        continue;

                    case "mapteleport":

                        trig = Instantiate(FindOBJ("Teleporter"), new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_maptransition>();
                        targname = prefabs[4];
                        break;

                    case "money":

                        trig = Instantiate(FindOBJ("money"), new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_itemObj>();
                        targname = prefabs[2];
                        break;

                    case "health_increase":

                        trig = Instantiate(FindOBJ("health_increase"), new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_itemObj>();
                        targname = FindOBJ("health_increase");
                        break;

                    case "teleport_object":

                        trig = Instantiate(FindOBJ("teleport_object"), new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_collidableobject>();
                        objname = mapdat.tilesdata[i].name;
                        break;
                }
            }
            else
            {

                switch (mapdat.tilesdata[i].TYPENAME)
                {
                    default:
                        continue;

                    case "mapteleport":
                        trig = SpawnObject("Teleporter", new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_maptransition>();
                        break;

                    case "money":
                        trig = SpawnObject("money", new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_itemObj>();
                        break;

                    case "health_increase":
                        trig = SpawnObject("health_increase", new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_itemObj>();
                        break;

                    case "teleport_object":
                        trig = SpawnObject("teleport_object", new Vector2(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y), Quaternion.identity).GetComponent<o_collidableobject>();
                        objname = mapdat.tilesdata[i].name;
                        trig.GetComponent<SpriteRenderer>().sprite = null;
                        break;
                }
            }
            float divx, divy;
            divx = (float)mapdat.tilesdata[i].pos_x / 25;
            divy = (float)mapdat.tilesdata[i].pos_y / 25;

            trig.transform.SetParent(tileIG.transform);
            trig.transform.position = new Vector3(trig.transform.position.x, trig.transform.position.y, 0);

            trig.positioninworld = new Vector3(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y, 1);
            //trig.transform.position = new Vector3(mapdat.tilesdata[i].pos_x, mapdat.tilesdata[i].pos_y, 1);

            if (trig.GetComponent<o_collidableobject>())
            {
                o_collidableobject col = trig.GetComponent<o_collidableobject>();
                col.isEnabled = mapdat.tilesdata[i].colActive;
                col.collision_type = (o_collidableobject.COLLISION_T)mapdat.tilesdata[i].enumthing;
                col.collision = col.GetComponent<BoxCollider2D>();
                if (col.collision != null)
                    col.collision.size = new Vector2(mapdat.tilesdata[i].size.x, mapdat.tilesdata[i].size.y);
                col.Intialize();
            }
            if (trig.GetComponent<o_maptransition>())
            {
                o_maptransition col = trig.GetComponent<o_maptransition>();
                s_map.s_tileobj ma = mapdat.tilesdata[i];
                if (ma.flagchecks != null)
                {
                    col.flagcheck = ma.flagname;
                    col.flags = new o_maptransition.s_flagcheck[ma.flagchecks.Length];
                    for (int a = 0; a < ma.flagchecks.Length; a++)
                    {
                        col.flags[a] = new o_maptransition.s_flagcheck(ma.flagchecks[a], ma.mapnames[a]);
                    }
                    col.teleportObj = ma.teleporterName;
                }
                //print(col);
                col.position = new Vector2(ma.teleportpos.x, ma.teleportpos.y);
                col.sceneToTransferTo = ma.mapname;
            }
            if (targname != null)
                trig.name = targname.name;

            if (objname != null)
                trig.name = objname;
        }
        #endregion

        if (!InEditor)
        {

            foreach (o_character c in allcharacters)
            {
                if (c != null)
                    c.AddFactions(allcharacters);
            }
            //pl.AddFactions(allcharacters);

            s_triggerhandler.trig = GetComponent<s_triggerhandler>();

            if (mapscript != null)
                mapscript.CallTrigger();
        }
        nodegraph.nodeGraph2 = nodegraph.CreateNodeArray(mapDat.tilesdata.ToArray());
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

    
    public GameObject FindOBJ(string obname)
    {
        foreach (s_pooler_data ga in objPoolDatabase)
        {
            if (ga.gameobject.name == obname)
            {
                return ga.gameobject;
            }
        }
        return null;
    }
    

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = Snap(mousePosition);
        switch (EDIT_MODE)
        {
            case EDITMODE.BRUSH:

                Vector2Int mappos = snapvec(mousePosition);
                if (Input.GetMouseButton(0))
                {
                    if (current_map.layers == null)
                        return;
                    if (current_map.layers[0].objects[mappos.x, mappos.y] != null)
                        return;
                    GameObject obj = Instantiate(prefabs[prefabselect], mousePosition, Quaternion.identity);

                    o_collidableobject collidable = obj.GetComponent<o_collidableobject>();
                    if (collidable != null)
                    {
                        float zposplacement = zposition * boxsize;
                        collidable.positioninworld = new Vector3(mousePosition.x, mousePosition.y + zposplacement, zposition);
                    }
                    i++;
                    string nom = "Thing " + i;
                    obj.name = nom;
                    AssignParent("Tiles", obj.gameObject);
                    print(current_map.layers);
                    current_map.layers[0].objects[mappos.x, mappos.y] = collidable;
                }
                
                if (Input.GetKeyDown(KeyCode.S))
                {
                    o_npcharacter chara = Instantiate(charactersSpawnlist[0].gameObject, mousePosition, Quaternion.identity).GetComponent<o_npcharacter>();
                    float zposplacement = zposition * boxsize;
                    chara.positioninworld = new Vector3(mousePosition.x, mousePosition.y + zposplacement, zposition);
                    i++;
                    string nom = "character " + i;
                    chara.name = nom;
                    //chara.SetShadowPos();
                    AssignParent("Entities", chara.gameObject);
                }
                
        break;

            case EDITMODE.PROPERTIES:
                if (Input.GetMouseButtonDown(1))
                {
                    Collider2D col = Physics2D.OverlapBox(mousePosition, new Vector2(20, 20), 0);
                    if (col != null)
                    {
                        prpo = col.gameObject.GetComponent<o_collidableobject>();
                        if (prpo != null)
                        {
                            print("dgfd");
                            debug_mode = true;
                        }
                        else
                            debug_mode = false;
                    }
                    else
                        debug_mode = false;
                }
                if (debug_mode)
                {
                    if(prpo)
                        EditProperties(prpo);
                }

                break;
        }

    public void EditProperties(o_collidableobject obj)
    {
        GUI.Label(new Rect(10, 270, 150, 90), obj.collision_type.ToString());
        o = (int)GUI.HorizontalSlider(new Rect(10, 290, 150, 20), o, 0, 4);
        if (GUI.Button(new Rect(10, 400, 150, 30), "Set Enum"))
        {
            obj.collision_type = (o_collidableobject.COLLISION_T)o;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3((mapsizeToKeep.x * tilesize) / 2, (mapsizeToKeep.y * tilesize) / 2), new Vector3(mapsizeToKeep.x * tilesize, mapsizeToKeep.y * tilesize));


#if UNITY_EDITOR
        for (int z = (int)startpos.z; z < graphsize.x; z++)
        {
            for (int y = (int)startpos.y; y < graphsize.y; y++)
            {
                for (int x = (int)startpos.x; x < graphsize.x; x++)
                {
                    Gizmos.color = Color.green;
                    if (Physics2D.Raycast(new Vector2(x * boxsize, y * boxsize + z * boxsize), Vector2.up, 1, layerMask))
                    {
                        Gizmos.color = Color.red;
                    }
                    if (isWire)
                        Gizmos.DrawWireCube(new Vector2(x * boxsize, y * boxsize + z * boxsize), new Vector2(1, 1) * boxsize);
                    else
                    {
                        //DUMMY CODE
                        if (x == 3 && y == 9)
                        {
                            Gizmos.color = Color.blue;

                            Gizmos.DrawCube(new Vector2(x * boxsize, y * boxsize + hieght + z * boxsize), new Vector2(1, 1) * boxsize);
                        }
                        else
                        {
                            Gizmos.DrawCube(new Vector2(x * boxsize, y * boxsize + z * boxsize), new Vector2(1, 1) * boxsize);
                        }
                    }

                }
            }

        }
#endif
    }
}
*/
/*
                 List<s_map.s_block> tile = mp.graphicTiles;
                List<s_map.s_block> tileMid = mp.graphicTilesMiddle;
                List<s_map.s_block> tileTop = mp.graphicTilesTop;

                foreach (s_map.s_block b in tile)
                {
                    Vector3Int pos = new Vector3Int((int)b.position.x / (int)tilesize, (int)b.position.y / (int)tilesize, 0);
                    if (tilesNew.Find(ti => ti.name == b.sprite) != null)
                        tm.SetTile(pos, tilesNew.Find(ti => ti.name == b.sprite));
                }
                foreach (s_map.s_block b in tileMid)
                {
                    tm2.SetTile(new Vector3Int((int)b.position.x / (int)tilesize, (int)b.position.y / (int)tilesize, 0), tilesNew.Find(ti => ti.name == b.sprite));
                }
                foreach (s_map.s_block b in tileTop)
                {
                    tm3.SetTile(new Vector3Int((int)b.position.x / (int)tilesize, (int)b.position.y / (int)tilesize, 0), tilesNew.Find(ti => ti.name == b.sprite));
                }

                */
/*
switch (ma.TYPENAME)
{
    default:
        continue;

    case "teleport_object":
        trig = Instantiate(FindOBJ("teleport_object"), new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).GetComponent<o_generic>();
        break;

    case "bound":
        trig = Instantiate(FindOBJ("bound"), new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).GetComponent<o_generic>();
        break;

    case "money":

        trig = Instantiate(FindOBJ("money"), new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).GetComponent<o_itemObj>();
        break;

    case "health_increase":

        trig = Instantiate(FindOBJ("health_increase"), new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).GetComponent<o_itemObj>();
        targname = FindOBJ("health_increase");
        break;
    case "Boundary":

        trig = Instantiate(FindOBJ("Boundary"), new Vector2(ma.pos_x, ma.pos_y), Quaternion.identity).GetComponent<o_generic>();
        targname = FindOBJ("Boundary");
        trig.transform.localScale = new Vector3(ma.size.x, ma.size.y);
        trig.name = ma.name;
        break;
}
*/
