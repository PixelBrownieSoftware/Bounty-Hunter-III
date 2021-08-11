using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class s_map
{
    public List<string> characterFileNames = new List<string>();
    public List<s_trig> triggerdata = new List<s_trig>();
    public List<s_block> graphicTiles = new List<s_block>();
    public List<s_block> graphicTilesMiddle = new List<s_block>();
    public List<s_block> graphicTilesTop = new List<s_block>();
    public List<s_chara> objectdata = new List<s_chara>();
    public List<s_tileobj> tilesdata = new List<s_tileobj>();
    public List<s_maplayer> layers = new List<s_maplayer>();
    public s_maplayer collisiondata;
    public s_maplayer object_data;
    public s_save_vector spawnPoint;
    public string FlagNameCheck;
    public string mapscript;

    public s_save_vector mapsize;
    
    public int id;
    public string name;
    public string zone;

    public s_map(string name)
    {
        layers.Add(new s_maplayer());
        this.name = name;
    }
    public s_map(string name, string script)
    {
        layers.Add(new s_maplayer());
        this.name = name;
        mapscript = script;
    }
    [System.Serializable]
    public struct s_save_colour
    {
        public float r, g, b, a;
        public s_save_colour(float r, float g, float b, float a)
        {
            this.r = r;
            this.b = b;
            this.g = g;
            this.a = a;
        }
        public s_save_colour(float r, float g, float b)
        {
            this.r = r;
            this.b = b;
            this.g = g;
            this.a = 255;
        }
        public s_save_colour(Color col)
        {
            r = col.r;
            b = col.b;
            g = col.g;
            a = col.a;
        }
        
    }

    [System.Serializable]
    public struct s_save_vector
    {
        public float x, y;
        public s_save_vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public s_save_vector(Vector2 pos)
        {
            x = pos.x;
            y = pos.y;
        }
        public s_save_vector(Vector3 pos)
        {
            x = pos.x;
            y = pos.y;
        }

        public static s_save_vector operator + (s_save_vector vec, Vector3 vec2)
        {
            vec.x = vec2.x;
            vec.y = vec2.y;
            return vec;
        }

        public static List<s_save_vector> Vector2ToSaveVectors(List<Vector2> ve)
        {
            Debug.Log("Gere");
            List<s_save_vector> vec = new List<s_save_vector>();
            foreach (Vector2 v in ve)
            {
                Debug.Log(v);
                vec.Add(new s_save_vector(v.x, v.y));
            }
            return vec;
        }
        public static s_save_vector Vector2ToSaveVector(Vector2 ve)
        {
            return new s_save_vector(ve.x, ve.y);
        }
        public static List<Vector2> SaveVectorsToVectors(List<s_save_vector> ve)
        {
            Debug.Log("Gere");
            List<Vector2> vec = new List<Vector2>();
            foreach (s_save_vector v in ve)
            {
                Debug.Log(v);
                vec.Add(new Vector2(v.x, v.y));
            }
            return vec;
        }
        public static Vector2 SaveVectorToVector2(s_save_vector ve)
        {
            return new Vector2(ve.x, ve.y);
        }
    }
    [System.Serializable]
    public class s_block
    {
        public s_block(Sprite sprite, Vector2 position)
        {
            this.position = new s_save_vector(position);
            if(sprite != null)
            sprite_name = sprite.name;
        }
        public s_block(string sprite, Vector2 position)
        {
            this.position = new s_save_vector(position);
            if (sprite != null)
                sprite_name = sprite;
        }
        public string sprite_name;
        public s_save_vector texture_offset;
        public s_save_colour[] colour;
        public s_save_vector position;
    }

    [System.Serializable]
    public class s_trig
    {
        [System.Serializable]
        public struct bound
        {
            public s_save_vector pos;
            public bool isen;
            public bound(Vector2 vec, bool isen)
            {
                this.isen = isen;
                pos = new s_save_vector(vec.x, vec.y);
            }
        }
        [System.Serializable]
        public struct shopit
        {
            public int price, item_type;
            public string name;
            public shopit(string name, int item_type, int price)
            {
                this.price = price;
                this.name = name;
                this.item_type = item_type;
            }
        }

        public s_trig(string name, Vector2 pos, ev_details[] ev, o_trigger.TRIGGER_TYPE trig, Vector2 size,bool disable)
        {
            trigSize = new s_save_vector(size.x, size.y);
            this.name = name;
            util = null;
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            listofevents = ev;
            characternames = null;
            boundaryobj = null;
            trigtye = trig;
            IsPermanentlyDisabled = disable;
        }
        public s_trig(string name, Vector2 pos, ev_details[] ev, string util, o_trigger.TRIGGER_TYPE trig, bool disable)
        {
            this.name = name;
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            listofevents = ev;
            this.util = util;
            characternames = null;
            boundaryobj = null;
            trigtye = trig;
            IsPermanentlyDisabled = disable;
        }
        public s_trig(string name, Vector2 pos, ev_details[] ev, string util, o_trigger.TRIGGER_TYPE trig, bool disable, List<shopit> shopitems)
        {
            this.name = name;
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            listofevents = ev;
            this.util = util;
            characternames = null;
            boundaryobj = null;
            trigtye = trig;
            IsPermanentlyDisabled = disable;
            this.shopitems = shopitems.ToArray();
        }

        public s_trig(string name ,Vector2 pos, ev_details[] ev, string util, GameObject[] bound, string[] charnames, o_trigger.TRIGGER_TYPE trig,Vector2 trigSize, bool disable)
        {
            this.name = name;
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            listofevents = ev;
            this.trigSize = new s_save_vector(trigSize.x, trigSize.y);
            this.util = util;
            characternames = charnames;
            boundaryobj = new bound[bound.Length];
            for (int i = 0; i < bound.Length; i++)
            {
                boundaryobj[i] = new bound(bound[i].transform.position, bound[i].GetComponent<o_collidableobject>().isEnabled);
            }
            trigtye = trig;
            IsPermanentlyDisabled = disable;
        }
        public s_trig(string name, Vector2 pos, ev_details[] ev, string util, string[] charnames, int[] intlist, o_trigger.TRIGGER_TYPE trig, Vector2 trigSize, bool disable)
        {
            this.name = name;
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            listofevents = ev;
            this.intlist = intlist;
            this.trigSize = new s_save_vector(trigSize.x, trigSize.y);
            this.util = util;
            characternames = charnames;
            trigtye = trig;
            IsPermanentlyDisabled = disable;
        }
        public bool IsPermanentlyDisabled; // Set to false by default
        public int pos_x, pos_y;
        public string util;
        public string name;
        public o_trigger.TRIGGER_TYPE trigtye;
        public ev_details[] listofevents;
        public string[] characternames;
        public int[] intlist;
        public bound[] boundaryobj;
        public shopit[] shopitems;
        public s_save_vector trigSize;
    }
    
    [System.Serializable]
    public struct s_customType
    {
        public s_customType(string name, object type, object type2, object type3)
        {
            this.name = name;
            this.type = type;
            this.type2 = type2;
            this.type3 = type3;
        }
        public s_customType(string name, object type, object type2)
        {
            this.name = name;
            this.type = type;
            this.type2 = type2;
            type3 = null;
        }
        public s_customType(string name, object type)
        {
            this.name = name;
            this.type = type;
            type2 = null;
            type3 = null;
        }
        public string name;
        public object type;
        public object type2;
        public object type3;
    }

    [System.Serializable]
    public class s_chara
    {
        public s_chara(Vector2 pos, string mapname, string charname,
            string charType, bool enabled, bool spawnthis, 
            bool disableStatic, string faction, bool hastrigger, string triggername)
        {
            this.faction = faction;
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            this.disableStatic = disableStatic;
            this.spawnthis = spawnthis;
            this.charname = charname;
            this.mapname = mapname;
            this.charType = charType;
            has_trigger = hastrigger;
            this.triggername = triggername;
        }
        public string faction;
        public List<string> labelsToCall = new List<string>();
        public string mapname;
        public string charname;
        public string charType; //Checks for the type of the character
        public bool spawnthis;  //Checks if the character is defeated or not present
        public bool disableStatic;
        public bool enabled;    //Checks if the character's control is enabled
        public int pos_x, pos_y;
        public string triggername = null;
        public bool has_trigger = false;
    }
    
    [System.Serializable]
    public class s_tileobj
    {
        public s_tileobj(Vector3 pos, string objname)
        {
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            pos_z = (int)pos.z;
            enumthing = 0;
            TYPENAME = objname;
        }
        public s_tileobj(Vector3 pos, string objname, List<Vector2> vec)
        {
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            pos_z = (int)pos.z;
            enumthing = (int)o_collidableobject.COLLISION_T.MOVING_PLATFORM;
            vectors = s_save_vector.Vector2ToSaveVectors(vec);
            TYPENAME = objname;
        }
        public s_tileobj(Vector3 pos, string type, int enumth, List<Vector2> vec)
        {
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            pos_z = (int)pos.z;
            vectors = s_save_vector.Vector2ToSaveVectors(vec);
            enumthing = enumth;
            TYPENAME = type;
        }
        public s_tileobj(Vector3 pos, string type, int enumth)
        {
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            pos_z = (int)pos.z;
            enumthing = enumth;
            TYPENAME = type;
        }
        public s_tileobj(Vector3 pos, string type, int enumth, string name)
        {
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            pos_z = (int)pos.z;
            this.name = name;
            enumthing = enumth;
            TYPENAME = type;
        }
        public s_tileobj(Vector2 pos, string type, Vector2 teleportpos, string mapname)
        {
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            this.teleportpos.x = teleportpos.x;
            this.teleportpos.y = teleportpos.y;
            this.mapname = mapname;
            TYPENAME = type;
        }
        public s_tileobj(Vector2 pos, string type, Vector2 teleportpos, string mapname, string name)
        {
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            this.teleportpos.x = teleportpos.x;
            this.teleportpos.y = teleportpos.y;
            this.name = name;
            this.mapname = mapname;
            TYPENAME = type;
        }

        public s_tileobj(Vector2 pos, string type, Vector2 teleportpos, string mapname, string flagname, int[] flagchecks, string[] mapnames, string teleportername)
        {
            teleporterName = teleportername;
            pos_x = (int)pos.x;
            pos_y = (int)pos.y;
            this.teleportpos.x = teleportpos.x;
            this.teleportpos.y = teleportpos.y;
            this.mapname = mapname;
            TYPENAME = type;
            this.flagname = flagname;
            this.flagchecks = flagchecks;
            this.mapnames = mapnames;
        }

        public string flagname;
        public int[] flagchecks;
        public string[] mapnames;
        public List<s_save_vector> vectors = new List<s_save_vector>();

        public bool colActive;
        public s_save_vector teleportpos;
        public s_save_vector size;
        public string mapname;
        public string teleporterName = null;
        public string TYPENAME;
        public string name;
        public int enumthing;
        public int pos_x, pos_y, pos_z;
        public float height;
    }
}
*/