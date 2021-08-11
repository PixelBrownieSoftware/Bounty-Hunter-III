using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class u_boundary : s_utility {

    public enum ENEMY_IDLE_MODE { 
        SMOKE,
        NONE
    };
    public ENEMY_IDLE_MODE enMode;

    public bool disableBoundariesUponEnd = true;
    public bool showHP = false;
    public BHIII_character[] characters;
    public string[] characterNames;
    public o_generic[] bounds;
    public string labelToJumpTo;

    HashSet<o_character> defeated = new HashSet<o_character>();
    public bool doLabelToJumpTo = false;

    public new void Update()
    {
        switch (eventState)
        {
            case 0:
                foreach (BHIII_character g in characters)
                {
                    g.enabled = false;
                    switch (enMode) {

                        case ENEMY_IDLE_MODE.SMOKE:
                            g.rendererObj.color = Color.clear;
                            break;
                    }
                    g.collision.enabled = false;
                }
                if (bounds != null)
                    foreach (o_generic g in bounds)
                    {
                        g.enabled = false;
                        g.rendererObj.color = Color.clear;
                        g.collision.enabled = false;
                    }
                break;

            case 1:
                if (showHP) {

                    BHIII_globals.gl.bossChar = new List<o_character>();
                    BHIII_globals.gl.bossDisplayOn = true;
                }
                if (bounds != null)
                    foreach (o_generic g in bounds)
                    {
                        g.enabled = true;
                        g.rendererObj.color = Color.white;
                        g.collision.enabled = true;
                    }
                foreach (BHIII_character g in characters)
                {
                    g.enabled = true;
                    g.rendererObj.color = Color.white;
                    g.collision.enabled = true;
                    if (showHP)
                        BHIII_globals.gl.bossChar.Add(g);
                }
                eventState++;
                break;

            case 2:
                if (bounds != null)
                    if (CheckIfAllCharactersDefeated())
                    {
                        if (showHP)
                        {
                            BHIII_globals.gl.bossChar.Clear();
                            BHIII_globals.gl.bossDisplayOn = false;
                        }
                        if (disableBoundariesUponEnd) {

                            foreach (o_generic b in bounds)
                            {
                                Destroy(b.gameObject);
                            }
                        }
                        if(doLabelToJumpTo)
                            s_trig.trig.JumpToEvent(labelToJumpTo, false);
                       eventState++;
                    }
                break;
        }
    }

    bool CheckIfAllCharactersDefeated() 
    {
        foreach (BHIII_character chr in characters) {
            if (chr == null)
                continue;
            if (chr.health > 0)
                return false;
        }
        return true;
    }


    /*
public override void EventStart()
{
for (int i = 0; i < bounds.Length; i++)
{
o_collidableobject co = bounds[i].GetComponent<o_collidableobject>();
if (!co.isEnabled)
    co.isEnabled = true;
}
}
*/

    /*
    public override void EventUpdate()
    {
        base.EventUpdate();
        foreach (o_character c in characters)
        {
            if (c.health <= 0)
            {
                defeated.Add(c);
            }
        }
        if (defeated.Count == characters.Length)
        {
            for (int i = 0; i < bounds.Length; i++)
            {
}
istriggered = false;
        }
    }
    public override IEnumerator EventTrigger()
    {
        istriggered = true;
        while (defeated.Count != characters.Length)
        {
            print("No.");
            yield return new WaitForSeconds(Time.deltaTime);
        }

        eventState = 0;
        print("Yes.");
        yield return base.EventTrigger();
    }
    */
}
