using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_zone : MonoBehaviour {

    public List<TextAsset> maps = new List<TextAsset>();
    public s_zonedata zonedat;

	void Start () {
		
	}

    /// <summary>
    /// TODO:
    /// - Reset every flag of every non-static object
    /// - delete every data of non static objects
    /// </summary>
    void ResetFlags()
    {

    }
    /// <summary>
    /// TODO:
    ///  - Set event number of triggers
    ///  - If characters are defeated don't spawn them
    /// </summary>
    void SetFlags()
    {


    }

    /// <summary>
    /// TODO:
    ///  - Get the dead state of every enemy, if it's ture set it accordingly
    ///  - Get the event number of the trigger and set it in the data
    ///  - Check a trigger flag if it's static
    /// </summary>
    void GetFlags()
    {

    }

	void Update () {
		
	}
}

[System.Serializable]
public class s_zonedata
{
    public List<character_data> characterdat = new List<character_data>();
    public List<trigger_data> triggerdat = new List<trigger_data>();

    public class trigger_data
    {
        public int evnum;
        public string map;
        public bool isstatic;
    }
    public class character_data
    {
        public bool isdefaeted;
    }

}
