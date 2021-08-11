using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class o_maptransition : s_object {
    
    [System.Serializable]
    public struct s_flagcheck
    {
        public int flagCondition;
        public string mapname;
        public s_flagcheck(int flagCondition, string mapname)
        {
            this.flagCondition = flagCondition;
            this.mapname = mapname;
        }

    }

    public string sceneToTransferTo;
    public string teleportObj;
    public Vector2 position;
    BoxCollider2D col;
    public string flagcheck;
    int flagevent;
    public s_flagcheck[] flags;

    private new void Start()
    {
        flagevent = 0;
        collision = GetComponent<BoxCollider2D>();
    }

    private new void Update()
    {
        base.Update();
        Collider2D c = IfTouchingGetCol(collision, typeof(o_character));
        if (c != null)
        {
            if (c.GetComponent<o_character>() != null)
            {
                if (sceneToTransferTo != null || sceneToTransferTo != "")
                {
                    print("fkjsfjsl");
                    Transition();
                }
                else
                {
                    print("JJJKKK");
                    Transition(teleportObj);
                }
                
                if (flagcheck != string.Empty)
                {
                    for (int i = 0; i < flags.Length; i++)
                    {
                        
                        if (s_globals.GetGlobalFlag(flagcheck) == flags[i].flagCondition)
                        {
                            if (flags[i].mapname != string.Empty)
                            {
                                //c.GetComponent<o_bullet>().OnImpact();
                                Transition(flags[i].mapname, teleportObj);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }


    public void Transition()
    {
        s_globals.SaveData();
        s_leveledit led = GameObject.Find("General").GetComponent<s_leveledit>();
        if(teleportObj == "" || teleportObj == null)
            led.TriggerSpawn(teleportObj);
        else
            led.TriggerSpawn(sceneToTransferTo, teleportObj);
        //Call some global stuff 
    }
    public void Transition(string tele)
    {
        s_globals.SaveData();
        s_leveledit led = GameObject.Find("General").GetComponent<s_leveledit>();
        led.TriggerSpawn(tele);
        //Call some global stuff 
    }

}
*/