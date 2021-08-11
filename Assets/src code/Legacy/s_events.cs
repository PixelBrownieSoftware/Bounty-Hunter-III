using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
LABEL = -1,
MOVEMNET,
DIALOGUE,
SET_HEALTH,
RUN_CHARACTER_SCRIPT,
ANIMATION,
SOUND,
ADD_SHOP_ITEM,
SET_FLAG,
CHECK_FLAG,
CAMERA_MOVEMENT,
BREAK_EVENT,
END_EVENT,
UTILITY_FOCUS,
FADE,
OBJECT,
DISPLAY_CHARACTER_HEALTH,
UTILITY_INITIALIZE,
UTILITY_CHECK,
WAIT,
CHOICE,
PUT_SHUTTERS,
ADD_INVENTORY,
TELEPORT,
CHANGE_MAP,
DISPLAY_ICON,
SET_UTILITY_FLAG
*/
/*
public enum EVENT_TYPES
{
    LABEL = -1,
    MOVEMNET,
    DIALOGUE,
    SET_HEALTH = 2,
    RUN_CHARACTER_SCRIPT,
    ANIMATION = 4,
    SOUND,
    SET_FLAG = 7,
    CHECK_FLAG,
    CAMERA_MOVEMENT,
    BREAK_EVENT = 10,
    SET_UTILITY_FLAG = 12,
    FADE,
    CREATE_OBJECT,
    DISPLAY_CHARACTER_HEALTH,
    UTILITY_INITIALIZE,
    UTILITY_CHECK,
    WAIT,
    CHOICE,
    CHANGE_SCENE,
    PUT_SHUTTERS,
    DISPLAY_IMAGE,
    SHOW_TEXT,
    CHANGE_MAP,
    DEPOSSES,
    DELETE_OBJECT,
    SET_OBJ_COLLISION,
    ADD_CHOICE_OPTION,
    CLEAR_CHOICES,
    PRESENT_CHOICES,
    SAVE_DATA,
    ADD_WEAPON
}
*/
/*
public enum LOGIC_TYPE
{
    VAR_GREATER,
    VAR_EQUAL,
    VAR_LESS,
    VAR_NOT_EQUAL,
    ITEM_OWNED,
    CHECK_UTILITY_RETURN_NUM,
    CHECK_CHARACTER,
    CHECK_CHARACTER_NOT
}
*/

/*
[System.Serializable]
public class s_events {

    public enum TRIGGER_TYPE
    {
        CONTACT,
        CONDITION,
        BUTTON_PRESS
    }
    public TRIGGER_TYPE TRIGGER;
    public int index = 0;

    public ev_details[] ev_Details = new ev_details[3];    //Hard-code it to be 3 for now
    public o_trigger trigger;

	void Start () {
		
	}
	
	void Update () {
		
	}
    
    void TriggerFUNC()
    {
        ev_details ev_current = ev_Details[index];
        switch (ev_Details[index].eventType)
        {
            case ev_details.EVENT_TYPES.CHECK_VARIABLES:

                switch (ev_current.logic)
                {
                    case ev_details.LOGIC_TYPE.VAR_EQUAL:
                        if (s_leveledit.system_Leveledit.CheckIntegersEqual(ev_current.string0, ev_current.int0))
                        {
                            index = ev_current.jump;
                            ev_current = ev_Details[index];
                        }
                        break;
                    case ev_details.LOGIC_TYPE.VAR_GREATER:
                        if (s_leveledit.system_Leveledit.CheckIntegersGreaterThan(ev_current.string0, ev_current.int0))
                        {
                            index = ev_current.jump;
                            ev_current = ev_Details[index];
                        }
                        break;
                    case ev_details.LOGIC_TYPE.VAR_LESS:
                        if (s_leveledit.system_Leveledit.CheckIntegersLessThan(ev_current.string0, ev_current.int0))
                        {
                            index = ev_current.jump;
                            ev_current = ev_Details[index];
                        }
                        break;

                }

                

                break;
            case ev_details.EVENT_TYPES.SET_VARIABLE:
                break;
        }
    }

}

[System.Serializable]
public class ev_details
{
    [System.Serializable]
    public struct s_save_colour
    {
        public float r, g, b, a;
        public s_save_colour(Color colour)
        {
            r = colour.r;
            g = colour.g;
            b = colour.b;
            a= colour.a;
        }
    }
    public enum EVENT_TYPES
    {
        MOVEMNET,
        DIALOGUE,
        SET_HEALTH,
        RUN_CHARACTER_SCRIPT,
        ANIMATION,
        SOUND,
        ADD_SHOP_ITEM,
        SET_FLAG,
        CHECK_FLAG,
        CAMERA_MOVEMENT,
        BREAK_EVENT,
        END_EVENT,
        UTILITY_FOCUS,
        FADE,
        OBJECT,
        DISPLAY_CHARACTER_HEALTH,
        UTILITY_INITIALIZE,
        UTILITY_CHECK,
        WAIT,
        CHOICE,
        PUT_SHUTTERS,
        ADD_INVENTORY,
        TELEPORT,
        CHANGE_MAP,
        DISPLAY_ICON
    }

    public enum LOGIC_TYPE
    {
        VAR_GREATER,
        VAR_EQUAL,
        VAR_LESS,
        ITEM_OWNED,
        CHARACTERS_DEAD,
        CHECK_PLAYER
    }

    public enum DIRECTION
    {
        NONE,
        LEFT,
        UP_LEFT,
        UP_RIGHT,
        DOWN_LEFT,
        DOWN_RIGHT,
        DOWN,
        UP,
        RIGHT
    }
    public DIRECTION dir;
    public int steps;

    public bool simultaneous;

    [System.NonSerialized]
    public TextAsset textAsset0;    //Instead of this, it will load the resource via a string that contains the directory path


    public Vector2 direcion
    {
        get
        {
            switch (dir)
            {
                default:
                    return new Vector2(0, 0);

                case DIRECTION.DOWN:
                    return new Vector2(0, -1);

                case DIRECTION.DOWN_LEFT:
                    return new Vector2(-1, -1);

                case DIRECTION.DOWN_RIGHT:
                    return new Vector2(1, -1);

                case DIRECTION.RIGHT:
                    return new Vector2(1, 0);

                case DIRECTION.UP:
                    return new Vector2(0, 1);

                case DIRECTION.UP_LEFT:
                    return new Vector2(-1, 1);

                case DIRECTION.UP_RIGHT:
                    return new Vector2(1, 1);

                case DIRECTION.LEFT:
                    return new Vector2(-1, 0);
            }
        }
    }

    public LOGIC_TYPE logic;
    public EVENT_TYPES eventType;

    public bool waitTillDone;
    public int jump;
    public s_map.s_save_vector pos;
    public s_save_colour colour;
    public int int0;
    public int int1;
    public int int2;
    public bool boolean;
    public bool boolean1;
    public string string0;
    public string string1;
    public float float0;
    public float float1;
    public string[] stringList;
    public int[] intList;
}
*/
