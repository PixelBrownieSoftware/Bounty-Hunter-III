using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gui_status : MonoBehaviour
{
    public BHIII_character character;
    public static BHIII_character[] othercharacter;
    public static BHIII_character allycharacter;
    public static void RemoveCharacters(bool top)
    {
        if (top)
        {
            allycharacter = null;
        }
        else
        {
            othercharacter = null;
        }
    }

    public void ChangeCOlour(Color colou)
    {
        character.GetComponent<SpriteRenderer>().color = colou;
    }

    public static void AddCharacter(List<BHIII_character> cha)
    {
        othercharacter = cha.ToArray();
    }
    public static void AddCharacter(BHIII_character cha, bool top)
    {
        if (top)
        {
            allycharacter = cha;
        }
        else
        {
            othercharacter = new BHIII_character[1];
            othercharacter[0] = cha;
        }
    }
}
