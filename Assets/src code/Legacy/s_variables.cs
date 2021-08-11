using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct s_integers
{
    public string name;
    public int integer;
}

/*
public class s_variables : MonoBehaviour
{
    public Dictionary<string, List<o_character>> Factions = new Dictionary<string, List<o_character>>();
    public List<misc_factions> factions = new List<misc_factions>();

    public static s_variables Static_Variables;
    private void Awake()
    {

        if(Static_Variables == null)
            Static_Variables = this;
    }

    public List<s_integers> statc_integers = new List<s_integers>();

    public static void CheckFactions(System.Type character)
    {
        foreach (misc_factions fac in Static_Variables.factions)
        {
            for (int i = 0; i < fac.characters.Count; i++)
            {
                if (fac.characters[i].GetType() == character)
                {

                }
            }
        }
    }

    public s_integers GetInteger(string nameofint)
    {
        foreach (s_integers integer in statc_integers)
        {
            if (integer.name == nameofint)
                return integer;
        }
        return new s_integers();
    }
}
*/
